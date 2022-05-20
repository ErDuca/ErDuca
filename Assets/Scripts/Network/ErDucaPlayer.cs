using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ErDucaPlayer : NetworkBehaviour
{
    #region Class stuff
    //Game State static References
    [SerializeField]
    private static ErDucaPlayer _localPlayer;
    [SerializeField]
    private static ErDucaGameManager _erDucaGameManager;

    //References
    private Camera _camera;
    private ErDucaNetworkManager _erDucaNetworkManager;
    private ErDucaMoveManager _erDucaMoveManager;
    private GameUIBehaviour _gameUIBehaviour;
    private BattleAnimationsScript _battleAnimationScript;
    private Animator _canvasAnimator;

    //Deck, Duke and Grid's info
    [SerializeField]
    private int _dukeI = 0;
    [SerializeField]
    private int _dukeJ = 0;

    private static int _numberOfUnits = 14;
    private int _numberOfStartingPikeman = 2;
    private int _gridSize;
    private List<int> _cards = new List<int>();

    //Currently selected elements info
    [SerializeField]
    private int _currentDrawnCard;
    [SerializeField]
    private List<Tuple<int, int>> _currentAvailableSpawnPositions = new List<Tuple<int, int>>();
    [SerializeField]
    private ErDucaPiece _currentSelectedPiece;
    [SerializeField]
    private List<Tuple<int, int>> _currentAvailableMoves = new List<Tuple<int, int>>();

    //Sync var elements
    [SerializeField]
    [SyncVar] private int _myNetId;
    [SerializeField]
    [SyncVar] private Color _myColor;
    [SerializeField]
    [SyncVar] private bool _hasDrawn = false;

    //Getters & Setters
    public bool HasDrawn
    {
        get => _hasDrawn;
        set
        {
            _hasDrawn = value;
        }
    }
    public int MyNetId
    {
        get => _myNetId;
        set
        {
            _myNetId = value;
        }
    }
    public Color MyColor
    {
        get => _myColor;
        set
        {
            _myColor = value;
        }
    }
    public GameUIBehaviour GameUIBehavior
    {
        get => _gameUIBehaviour;
    }
    public BattleAnimationsScript BattleAnimationScript
    {
        get => _battleAnimationScript;
    }
    public Animator CanvasAnimator
    {
        get => _canvasAnimator;
    }
    public static ErDucaGameManager ErDucaGameManager
    {
        get => _erDucaGameManager;
    }
    public static ErDucaPlayer LocalPlayer
    {
        get => _localPlayer;
    }
    #endregion

    #region Commands and RPCs
    [Command]
    public void CmdWinMatch(int winnerId)
    {
        _erDucaGameManager.RpcWinMatch(winnerId);
    }

    [Command]
    public void CmdPlayAnimation(int idAnim, int idBlue, int idRed)
    {
        _erDucaGameManager.RpcPlayAnimation(idAnim, idBlue, idRed);
    }

    [Command]
    public void CmdStartNewTurn()
    {
        _erDucaGameManager.RpcSetTurn();
    }

    [Command]
    public void CmdHighlightTile(int i, int j, NetworkConnectionToClient conn)
    {
        ErDucaTile tileToHighlight;
        Tuple<int, int> t = new Tuple<int, int>(i, j);

        if (ErDucaNetworkManager.singleton._tiles.ContainsKey(t))
        {
            ErDucaNetworkManager.singleton._tiles.TryGetValue(new Tuple<int, int>(t.Item1, t.Item2), out tileToHighlight);
            RpcHighlightLocalTile(conn, tileToHighlight);
        }
    }

    [Command]
    public void CmdDeHighlightAllTiles(NetworkConnectionToClient conn)
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                ErDucaTile tileToDeHighlight;
                Tuple<int, int> t = new Tuple<int, int>(i, j);

                if (ErDucaNetworkManager.singleton._tiles.ContainsKey(t))
                {
                    ErDucaNetworkManager.singleton._tiles.TryGetValue(new Tuple<int, int>(t.Item1, t.Item2), out tileToDeHighlight);
                    RpcDeHighlightLocalTile(conn, tileToDeHighlight);
                }
            }
        }
    }

    [TargetRpc]
    public void RpcHighlightLocalTile(NetworkConnection target, ErDucaTile tile)
    {
        tile.SetMaterialColor(Color.white);
    }

    [TargetRpc]
    public void RpcDeHighlightLocalTile(NetworkConnection target, ErDucaTile tile)
    {
        tile.SetOriginalColor();
    }

    [Command]
    public void CmdSpawnPiece(int spawningPieceIndex, Transform spawnTransform, int i, int j)
    {
        // +1 since units prefabs start from the second element of the array, the first one is the Tile Prefab
        GameObject piece = Instantiate(_erDucaNetworkManager.spawnPrefabs[spawningPieceIndex + 1],
            spawnTransform.position + new Vector3(0f, 30f, 0f), Quaternion.Euler(90, 0, 0));
        NetworkServer.Spawn(piece);

        piece.GetComponent<ErDucaPiece>().MyPlayerNetId = _myNetId;
        piece.GetComponent<ErDucaPiece>().I = i;
        piece.GetComponent<ErDucaPiece>().J = j;

        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
        RpcOnSpawnPiece(piece.GetComponent<ErDucaPiece>(), _myNetId);
    }

    [ClientRpc]
    public void RpcOnSpawnPiece(ErDucaPiece edp, int netId)
    {
        edp.GetComponent<SpriteRenderer>().color = _myColor;

        if (isServer)
        {
            edp.GetComponentsInChildren<SpriteRenderer>()[1].flipY = true;
            edp.GetComponentsInChildren<SpriteRenderer>()[1].flipX = true;
        }
    }

    [Command]
    public void CmdMovePiece(GameObject selectedPieceScript, Transform newTransform, int i, int j)
    {
        RpcUpdateLocalNetIdMatrix(selectedPieceScript.GetComponent<ErDucaPiece>().I, selectedPieceScript.GetComponent<ErDucaPiece>().J, 0);

        selectedPieceScript.GetComponent<ErDucaPiece>().StartMoveTo(newTransform.position + new Vector3(0f, 30f, 0f));
        selectedPieceScript.GetComponent<ErDucaPiece>().I = i;
        selectedPieceScript.GetComponent<ErDucaPiece>().J = j;
        selectedPieceScript.GetComponent<ErDucaPiece>().SwitchPhase();
        
        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
    }

    [Command]
    public void CmdEatPiece(GameObject selectedPieceScript, Transform newTransform, ErDucaPiece enemyPiece, int i, int j)
    {
        RpcUpdateLocalNetIdMatrix(selectedPieceScript.GetComponent<ErDucaPiece>().I, selectedPieceScript.GetComponent<ErDucaPiece>().J, 0);

        selectedPieceScript.GetComponent<ErDucaPiece>().StartMoveTo(newTransform.position + new Vector3(0f, 30f, 0f));

        NetworkServer.Destroy(enemyPiece.gameObject);

        selectedPieceScript.GetComponent<ErDucaPiece>().I = i;
        selectedPieceScript.GetComponent<ErDucaPiece>().J = j;

        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
    }

    [ClientRpc]
    public void RpcUpdateLocalNetIdMatrix(int i, int j, int _myNetId)
    {
        ErDucaNetworkManager.singleton.SetMatrixIdAt(_myNetId, i, j);
    }
    #endregion

    #region Network Callbacks
    public override void OnStartClient()
    {
        base.OnStartClient();

        _canvasAnimator = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Animator>();
       
        _camera = Camera.main;
        _erDucaNetworkManager = ErDucaNetworkManager.singleton;
        _erDucaMoveManager = ErDucaNetworkManager.singleton.GetComponent<ErDucaMoveManager>();
        _gameUIBehaviour = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<GameUIBehaviour>();
        _battleAnimationScript = GameObject.FindGameObjectWithTag("GameAnims").GetComponent<BattleAnimationsScript>();
        _gridSize = _erDucaNetworkManager.GridRowsNumber;

        //Rotate the Opponent's camera
        if(_myNetId > 1)
        {
            Camera.main.transform.Rotate(0f, 0f, 180f);
        }

        //Initialize Deck (without the Duke!!)
        for (int i = 1; i < _numberOfUnits - 1; i++)
        {
            _cards.Add(i);
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _localPlayer = this;

    }
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        _erDucaGameManager = GameObject.FindObjectOfType<ErDucaGameManager>();

        //Non riesce a settarlo, perchè legge prima del set in NetworkManager!
        //_GameUIBehaviour.SetPlayerInitialValues((int)_myNetId, 1);
        //! FIX!
    }

    private void Update()
    {
        Debug.Log("Sono in fase Duke" + isLocalPlayer + " "+ _erDucaGameManager.IsOurTurn + " "+ Input.GetMouseButtonDown(0)+ " " + _gameUIBehaviour.changingTurn);
        if (isLocalPlayer && _erDucaGameManager.IsOurTurn && Input.GetMouseButtonDown(0) && !_gameUIBehaviour.changingTurn)
        {
            StartCoroutine(HandleInput(_erDucaGameManager.CurrentState));
        }
    }
    #endregion

    #region Class Functions
    public IEnumerator HandleInput(BattleState currentBattleState)
    {
        switch (currentBattleState)
        {
            case BattleState.PDuke:
                RaycastHit hitDuke;
                Ray rayDuke = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayDuke, out hitDuke))
                {
                    Transform objectHit = hitDuke.transform;
                    if (objectHit.CompareTag("Tile"))
                    {
                        Debug.Log("Ho selezionato un tile e ci voglio spawnare sopra il Duca");
                        int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                        int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                        CmdDeHighlightAllTiles(this.connectionToClient);

                        foreach (Tuple<int, int> tuple in _currentAvailableSpawnPositions)
                        {
                            if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                            {
                                CmdSpawnPiece(0 /*duke unit index*/, objectHit.transform, tile_i_index, tile_j_index);
                                
                                _dukeI = tile_i_index;
                                _dukeJ = tile_j_index;

                                _currentAvailableSpawnPositions.Clear();
                                CmdStartNewTurn();
                                break;
                            }
                        }
                    }
                }
                break;

            case BattleState.PPikemen:
                RaycastHit hitPikemen;
                Ray rayPikemen = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rayPikemen, out hitPikemen))
                {
                    Transform objectHit = hitPikemen.transform;
                    if (objectHit.CompareTag("Tile"))
                    {
                        Debug.Log("Ho selezionato un tile e ci voglio spawnare il Footman");
                        int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                        int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                        CmdDeHighlightAllTiles(this.connectionToClient);

                        foreach (Tuple<int, int> tuple in _currentAvailableSpawnPositions)
                        {
                            if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                            {
                                CmdSpawnPiece(7 /*Footman unit index*/, objectHit.transform, tile_i_index, tile_j_index);

                                _numberOfStartingPikeman--;
                                
                                if (_numberOfStartingPikeman == 0)
                                {
                                    _currentAvailableSpawnPositions.Clear();
                                    CmdStartNewTurn();
                                }
                                else
                                {
                                    SpawnPikemen();
                                }
                                
                                break;
                            }
                        }
                    }
                }
                break;

            case BattleState.PTurn:
                //Handling moves
                if (!_hasDrawn)
                {
                    RaycastHit hit;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                    //Hit something
                    if (Physics.Raycast(ray, out hit))
                    {
                        Transform objectHit = hit.transform;

                        //No piece in cache, hit a piece (AVAILABLE MOVES)
                        if (_currentSelectedPiece == null && objectHit.CompareTag("Piece"))
                        {
                            if (objectHit.gameObject.GetComponent<ErDucaPiece>().MyPlayerNetId == _myNetId)
                            {
                                Debug.Log("Ho selezionato una mia pedina");
                                _currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();
                                int piece_i_index = _currentSelectedPiece.I;
                                int piece_j_index = _currentSelectedPiece.J;

                                if (_currentSelectedPiece.IsPhaseOne)
                                {
                                    _currentAvailableMoves = _erDucaMoveManager.
                                        GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P1MOVARR);
                                }
                                else
                                {
                                    _currentAvailableMoves = _erDucaMoveManager.
                                        GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P2MOVARR);
                                }

                                //Mostrare le mosse disponibili
                                foreach (Tuple<int, int> t in _currentAvailableMoves)
                                {
                                    CmdHighlightTile(t.Item1, t.Item2, this.connectionToClient);
                                }
                            }
                        }

                        //Piece in cache, hit a piece (KILLING ENEMY)
                        else if (_currentSelectedPiece != null && objectHit.CompareTag("Piece"))
                        {
                            CmdDeHighlightAllTiles(this.connectionToClient);

                            ErDucaPiece enemyPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();

                            if (enemyPiece.MyPlayerNetId != _myNetId)
                            {
                                Debug.Log("Ho selezionato una pedina nemica");
                                int enemyPiece_i_index = enemyPiece.I;
                                int enemyPiece_j_index = enemyPiece.J;
                                int enemyPieceUnitIndex = enemyPiece.UnitIndex();
                                int currentPieceUnitIndex = _currentSelectedPiece.UnitIndex();
                                Debug.Log("Indice pedina nemica: " + enemyPieceUnitIndex);

                                //Prova
                                float xTarget = objectHit.transform.position.x;
                                float zTarget = objectHit.transform.position.z;

                                foreach (Tuple<int, int> tuple in _currentAvailableMoves)
                                {
                                    if (tuple.Item1 == enemyPiece_i_index && tuple.Item2 == enemyPiece_j_index)
                                    {
                                        CmdEatPiece(_currentSelectedPiece.gameObject, objectHit.transform, enemyPiece, 
                                        enemyPiece_i_index, enemyPiece_j_index);

                                        //ATTENZIONE CASO STRIKE!

                                        yield return new WaitUntil(() => _currentSelectedPiece.transform.position.x == xTarget &&
                                        _currentSelectedPiece.transform.position.z == zTarget);

                                        yield return StartCoroutine(BattleAnimationCoroutine(_myNetId, enemyPieceUnitIndex, currentPieceUnitIndex));
                                        //yield return new WaitUntil(() => _canvasAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle")); 
                                        //yield return new WaitForSeconds(.05f);

                                        //Ho mangiato una pedina col duca, quindi aggiorno i suoi indici
                                        if (_currentSelectedPiece.UnitIndex() == 0)
                                        {
                                            _dukeI = enemyPiece_i_index;
                                            _dukeJ = enemyPiece_j_index;
                                        }

                                        //Ho mangiato il duca nemico!
                                        if(enemyPieceUnitIndex == 0)
                                        {
                                            CmdWinMatch(MyNetId);
                                        }
                                        else
                                        {
                                            CmdStartNewTurn();
                                        }
                                    }
                                }

                                _currentSelectedPiece = null;
                                _currentAvailableMoves.Clear();
                            }

                            else if (enemyPiece.MyPlayerNetId == _myNetId)
                            {
                                Debug.Log("Ho selezionato una mia pedina");
                                _currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();
                                int piece_i_index = _currentSelectedPiece.I;
                                int piece_j_index = _currentSelectedPiece.J;

                                if (_currentSelectedPiece.IsPhaseOne)
                                {
                                    _currentAvailableMoves = _erDucaMoveManager.
                                        GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P1MOVARR);
                                }
                                else
                                {
                                    _currentAvailableMoves = _erDucaMoveManager.
                                        GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P2MOVARR);
                                }

                                //Show available moves
                                foreach (Tuple<int, int> t in _currentAvailableMoves)
                                {
                                    CmdHighlightTile(t.Item1, t.Item2, this.connectionToClient);
                                }
                            }
                        }

                        //Piece in cache, hit a tile (MOVING PIECE)
                        else if (_currentSelectedPiece != null && objectHit.CompareTag("Tile"))
                        {
                            CmdDeHighlightAllTiles(this.connectionToClient);
                            Debug.Log("Ho selezionato un tile mentre ho una pedina selezionata");
                            int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                            int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                            foreach (Tuple<int, int> tuple in _currentAvailableMoves)
                            {
                                if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                                {
                                    CmdMovePiece(_currentSelectedPiece.gameObject, objectHit.transform, tile_i_index, tile_j_index);

                                    //Se muovo il duca aggiorno i suoi indici
                                    if (_currentSelectedPiece.UnitIndex() == 0)
                                    {
                                        _dukeI = tile_i_index;
                                        _dukeJ = tile_j_index;
                                    }

                                    CmdStartNewTurn();
                                }
                            }

                            _currentSelectedPiece = null;
                            _currentAvailableMoves.Clear();
                        }

                        //Hit the board
                        else
                        {
                            CmdDeHighlightAllTiles(this.connectionToClient);

                            _currentSelectedPiece = null;
                            _currentAvailableMoves.Clear();
                        }
                    }

                    //No Hit (Background) -> "deselect" and clear references
                    else
                    {
                        CmdDeHighlightAllTiles(this.connectionToClient);

                        _currentSelectedPiece = null;
                        _currentAvailableMoves.Clear();
                    }
                }

                //Drawn a card, player has to put it on the board
                else
                {
                    RaycastHit hit;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        Transform objectHit = hit.transform;

                        if (objectHit.CompareTag("Tile"))
                        {
                            Debug.Log("Ho selezionato un tile e devo spawnare la carta che ho pescato");
                            int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                            int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                            CmdDeHighlightAllTiles(this.connectionToClient);

                            foreach (Tuple<int, int> tuple in _currentAvailableSpawnPositions)
                            {
                                if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                                {
                                    Debug.Log("Pedina spawnata!");
                                    _gameUIBehaviour.DrawnUnitPlaced();

                                    CmdSpawnPiece(_currentDrawnCard, objectHit.transform, tile_i_index, tile_j_index);

                                    _currentDrawnCard = 0;
                                    _currentAvailableSpawnPositions.Clear();

                                    _hasDrawn = false;

                                    CmdStartNewTurn();

                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            case BattleState.PWin:
                Debug.Log("Hai vinto");
                break;

            case BattleState.PLost:
                Debug.Log("Hai perso");
                break;

            default:
                break;
        }
        yield return null;
    }

    //TEMPORARY ANIMATION STUFF
    private IEnumerator BattleAnimationCoroutine(int netId, int blueUnitIndex, int redUnitIndex)
    {
        switch (netId)
        {
            case 1:
                switch (redUnitIndex)
                {
                    //Ranged Red attacks Blue
                    case 3: case 6: case 9: case 10: case 12:
                        CmdPlayAnimation(14, (blueUnitIndex * 2), (redUnitIndex * 2) + 1);
                        break;

                    //Melee Red attacks Blue
                    case 0: case 1: case 2: case 4: case 5: case 7: case 8: case 11: case 13:
                        CmdPlayAnimation(2, (blueUnitIndex * 2), (redUnitIndex * 2) + 1);
                        break;

                    default:
                        break;
                }
                break;

            case 2:
                switch (blueUnitIndex)
                {
                    //Ranged Blue attacks Red
                    case 3: case 6: case 9: case 10: case 12:
                        CmdPlayAnimation(13, (blueUnitIndex * 2), (redUnitIndex * 2) + 1);
                        break;

                    //Melee Blue attacks Red
                    case 0: case 1: case 2: case 4: case 5: case 7: case 8: case 11: case 13:
                        CmdPlayAnimation(1, (blueUnitIndex * 2), (redUnitIndex * 2) + 1);
                        break;

                    default:
                        break;
                }
                break;
        }
    

        yield return new WaitUntil(() => _canvasAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationDone"));
        //yield return null;
    }

    public bool IsDeckEmpty()
    {
        return _cards.Count == 0;
    }

    public int GetDrawnCard()
    {
        int toBeRemovedIndex = UnityEngine.Random.Range(0, _cards.Count);
        int toBeReturnedElement = _cards[toBeRemovedIndex];
        _cards.RemoveAt(toBeRemovedIndex);

        return toBeReturnedElement;
    }

    public int DrawCard()
    {
        if (isLocalPlayer)
        {
            _hasDrawn = true;
            _currentDrawnCard = GetDrawnCard();

            Debug.Log("Posizione del duca: " + _dukeI + " " + _dukeJ);
            Debug.Log("Indice carta pescata: " + _currentDrawnCard);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int iSpawnPos = _dukeI + i;
                    int jSpawnPos = _dukeJ + j;

                    //Bounds Checking
                    if (iSpawnPos <= 5 && jSpawnPos <= 5 && iSpawnPos >= 0 && jSpawnPos >= 0)
                    {
                        //Checking i am not in the duke's position
                        if (!(i == 0 && j == 0))
                        {
                            Debug.Log("Controllo se posso spawnare in " + iSpawnPos + ":" + jSpawnPos + " ...");

                            if (_erDucaNetworkManager.GetMatrixIdAt(iSpawnPos, jSpawnPos) == 0)
                            {
                                _currentAvailableSpawnPositions.Add(new Tuple<int, int>(iSpawnPos, jSpawnPos));
                                Debug.Log("Posso spawnare in " + (iSpawnPos) + " " + (jSpawnPos));
                                CmdHighlightTile(iSpawnPos, jSpawnPos, this.connectionToClient);
                            }
                        }
                    }
                }
            }
        }

        return _currentDrawnCard;
    }

    public bool AreDukeNearTilesFree()
    {
        if (isLocalPlayer)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int iSpawnPos = _dukeI + i;
                    int jSpawnPos = _dukeJ + j;

                    //Bounds Checking
                    if (iSpawnPos <= 5 && jSpawnPos <= 5 && iSpawnPos >= 0 && jSpawnPos >= 0)
                    {
                        //Checking i am not in the duke's position
                        if (!(i == 0 && j == 0))
                        {
                            if (_erDucaNetworkManager.GetMatrixIdAt(iSpawnPos, jSpawnPos) == 0)
                            {
                                Debug.Log("Possibile spawning in " + (iSpawnPos) + " " + (jSpawnPos));
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        return false;
    }

    public void SpawnDuke()
    {
        if (isServer)
        {
            for (int i = 1; i < _gridSize - 1; i++)
            {
                _currentAvailableSpawnPositions.Add(new Tuple<int, int>(5, i));
                CmdHighlightTile(5, i, this.connectionToClient);
            }
        }
        else
        {
            for (int i = 1; i < _gridSize - 1; i++)
            {
                _currentAvailableSpawnPositions.Add(new Tuple<int, int>(0, i));
                CmdHighlightTile(0, i, this.connectionToClient);
            }
        }
    }

    public void SpawnPikemen()
    {
        Debug.Log("Posizione del duca: " + _dukeI + " " + _dukeJ);

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int iSpawnPos = _dukeI + i;
                int jSpawnPos = _dukeJ + j;

                Debug.Log("Controllo in: " + iSpawnPos + " " + jSpawnPos);

                //Bounds Checking
                if (iSpawnPos<= 5 && jSpawnPos<= 5 && iSpawnPos>= 0 && jSpawnPos >= 0)
                {
                    Debug.Log("Check bound superato");

                    //Checking i am not in the duke's position
                    if (!(i == 0 && j == 0))
                    {
                        Debug.Log("Controllo se posso spawnare in " + iSpawnPos + ":" + jSpawnPos + " ...");

                        if (_erDucaNetworkManager.GetMatrixIdAt(iSpawnPos, jSpawnPos) == 0)
                        {
                            _currentAvailableSpawnPositions.Add(new Tuple<int, int>(iSpawnPos, jSpawnPos));
                            Debug.Log("Posso spawnare in " + (iSpawnPos) + " " + (jSpawnPos));
                            CmdHighlightTile(iSpawnPos, jSpawnPos, this.connectionToClient);
                        }
                    }
                }
            }
        }
    }

    #endregion
}
