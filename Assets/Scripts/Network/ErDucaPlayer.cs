using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class ErDucaPlayer : NetworkBehaviour
{
    #region Class stuff
    //Game State static References
    [SerializeField]
    private static ErDucaPlayer _localPlayer;
    [SerializeField]
    private static ErDucaGameManager _erDucaGameManager;

    public bool isMyTurn
    {
        get => _erDucaGameManager.IsOurTurn;
    }
    public BattleState myCurrentBattleState
    {
        get => _erDucaGameManager.CurrentState;
    }

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
    private int _numberOfStartingUnits = 2;
    private Unit _startingUnitType = Unit.FOOTMAN;
    private int _gridSize;
    private List<int> _cards = new List<int>();

    public bool hasDoneSomething = false;

    //Currently selected elements info
    [SerializeField]
    private int _currentDrawnCard;
    [SerializeField]
    private List<Tuple<int, int>> _currentAvailableSpawnPositions = new List<Tuple<int, int>>();
    [SerializeField]
    private ErDucaPiece _currentSelectedPiece;
    [SerializeField]
    private Dictionary<Tuple<int, int>, Ptype> _currentAvailableMoves = new Dictionary<Tuple<int, int>, Ptype>();
    [SerializeField]
    private ErDucaPiece pressedPiece;

    //Sync var elements
    [SerializeField]
    [SyncVar] private int _myNetId;
    [SerializeField]
    [SyncVar] private Color _myColor;
    [SerializeField]
    [SyncVar] private bool _hasDrawn = false;

    [SerializeField]
    [SyncVar] private bool _iGaveUp = false;
    [SerializeField]
    [SyncVar] private bool _iAmHostAtStart = false;

    public bool IGaveUp
    {
        get => _iGaveUp;
        set
        {
            _iGaveUp = value;
        }
    }

    public bool IAmHostAtStart
    {
        get => _iAmHostAtStart;
        set
        {
            _iAmHostAtStart = value;
        }
    }

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
    public void CmdForfeitMatch(int winnerId)
    {
        _erDucaGameManager.RpcForfeitMatch(winnerId);
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
    public void CmdHighlightTile(int i, int j, Ptype moveType, NetworkConnectionToClient conn)
    {
        ErDucaTile tileToHighlight;
        Tuple<int, int> t = new Tuple<int, int>(i, j);

        if (ErDucaNetworkManager.singleton._tiles.ContainsKey(t))
        {
            ErDucaNetworkManager.singleton._tiles.TryGetValue(new Tuple<int, int>(t.Item1, t.Item2), out tileToHighlight);
            RpcHighlightLocalTile(conn, tileToHighlight, moveType);
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
    public void RpcHighlightLocalTile(NetworkConnection target, ErDucaTile tile, Ptype moveType)
    {
        switch (moveType)
        {
            case Ptype.Walk:
                tile.SetMaterialColor(Color.yellow);
                break;

            case Ptype.Jump:
                tile.SetMaterialColor(Color.green);
                break;

            case Ptype.Slide:
                tile.SetMaterialColor(Color.blue);
                break;

            case Ptype.Fly:
                tile.SetMaterialColor(Color.white);
                break;

            case Ptype.Strike:
                tile.SetMaterialColor(Color.cyan);
                break;

            case Ptype.Spawn:
                tile.SetMaterialColor(Color.magenta);
                break;

            default:
                break;
        }
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

    [Command]
    public void CmdStrikePiece(ErDucaPiece enemyPiece)
    {
        RpcUpdateLocalNetIdMatrix(enemyPiece.I, enemyPiece.J, 0);
        NetworkServer.Destroy(enemyPiece.gameObject);
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
    }

    private void Update()
    {
        
        if (isLocalPlayer && _erDucaGameManager.IsOurTurn && Input.GetMouseButtonDown(0) && !_gameUIBehaviour.changingTurn &&!hasDoneSomething)
        {
            StartCoroutine(HandleInput(_erDucaGameManager.CurrentState));
        } 

        //Clicking on a piece, during the opponent's turn
        else if (isLocalPlayer && Input.GetMouseButtonDown(0) && !_gameUIBehaviour.changingTurn)
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (objectHit.CompareTag("Piece"))
                {
                    pressedPiece = objectHit.GetComponent<ErDucaPiece>();
                    bool isOpponentPiece = (pressedPiece.MyPlayerNetId != _myNetId);

                    _gameUIBehaviour.ShowPieceInfo(pressedPiece.UnitIndex(), pressedPiece.MyPlayerNetId, pressedPiece.IsPhaseOne, isOpponentPiece);
                }
                else
                {
                    _gameUIBehaviour.HidePieceInfo();
                }
            }
            else
            {
                _gameUIBehaviour.HidePieceInfo();
            }
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

                //Hit something - Gotta place the Duke
                if (Physics.Raycast(rayDuke, out hitDuke))
                {
                    Transform objectHit = hitDuke.transform;

                    if (objectHit.CompareTag("Tile"))
                    {
                        //Debug.Log("Ho selezionato un tile e ci voglio spawnare sopra il Duca");
                        int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                        int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                        CmdDeHighlightAllTiles(this.connectionToClient);

                        foreach (Tuple<int, int> tuple in _currentAvailableSpawnPositions)
                        {
                            if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                            {
                                CmdSpawnPiece((int)Unit.DUKE, objectHit.transform, tile_i_index, tile_j_index);
                                
                                _dukeI = tile_i_index;
                                _dukeJ = tile_j_index;

                                hasDoneSomething = true;

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

                //Hit something - Gotta place the Pikemen
                if (Physics.Raycast(rayPikemen, out hitPikemen))
                {
                    Transform objectHit = hitPikemen.transform;
                    if (objectHit.CompareTag("Tile"))
                    {
                        //Debug.Log("Ho selezionato un tile e ci voglio spawnare il Footman");
                        int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                        int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                        CmdDeHighlightAllTiles(this.connectionToClient);

                        foreach (Tuple<int, int> tuple in _currentAvailableSpawnPositions)
                        {
                            if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                            {
                                CmdSpawnPiece((int)_startingUnitType, objectHit.transform, tile_i_index, tile_j_index);

                                _numberOfStartingUnits--;
                                
                                if (_numberOfStartingUnits == 0)
                                {
                                    _currentAvailableSpawnPositions.Clear();
                                    hasDoneSomething = true;
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
                if (!_hasDrawn)// && !hasMoved)
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
                            _currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();
                            bool isOpponentPiece = (_currentSelectedPiece.MyPlayerNetId != _myNetId);

                            _gameUIBehaviour.ShowPieceInfo(_currentSelectedPiece.UnitIndex(), _currentSelectedPiece.MyPlayerNetId, _currentSelectedPiece.IsPhaseOne, isOpponentPiece);

                            if (objectHit.gameObject.GetComponent<ErDucaPiece>().MyPlayerNetId == _myNetId)
                            {
                                //Debug.Log("Ho selezionato una mia pedina");
                                int piece_i_index = _currentSelectedPiece.I;
                                int piece_j_index = _currentSelectedPiece.J;

                                CmdHighlightTile(piece_i_index, piece_j_index, Ptype.Spawn, this.connectionToClient);

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

                                foreach(var item in _currentAvailableMoves)
                                {
                                    CmdHighlightTile(item.Key.Item1, item.Key.Item2, item.Value, this.connectionToClient);
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
                                _gameUIBehaviour.HidePieceInfo();

                                //Debug.Log("Ho selezionato una pedina nemica");
                                int enemyPiece_i_index = enemyPiece.I;
                                int enemyPiece_j_index = enemyPiece.J;
                                int enemyPieceUnitIndex = enemyPiece.UnitIndex();

                                int currentPieceUnitIndex = _currentSelectedPiece.UnitIndex();

                                float xTarget = objectHit.transform.position.x;
                                float zTarget = objectHit.transform.position.z;

                                foreach(var item in _currentAvailableMoves)
                                {
                                    if(item.Key.Item1 == enemyPiece_i_index && item.Key.Item2 == enemyPiece_j_index)
                                    {
                                        //Strike
                                        if (item.Value.Equals(Ptype.Strike))
                                        {
                                            hasDoneSomething = true;
                                            CmdStrikePiece(enemyPiece);
                                        }
                                        else
                                        {
                                            hasDoneSomething = true;
                                            CmdEatPiece(_currentSelectedPiece.gameObject, objectHit.transform, enemyPiece,
                                            enemyPiece_i_index, enemyPiece_j_index);

                                            yield return new WaitUntil(() => _currentSelectedPiece.transform.position.x == xTarget &&
                                            _currentSelectedPiece.transform.position.z == zTarget);
                                        }
                                        
                                        yield return StartCoroutine(BattleAnimationCoroutine(_myNetId, enemyPieceUnitIndex, currentPieceUnitIndex));

                                        //Moved the Duke, so i update his indexes
                                        if (_currentSelectedPiece.UnitIndex() == 0)
                                        {
                                            _dukeI = enemyPiece_i_index;
                                            _dukeJ = enemyPiece_j_index;
                                        }

                                        //Killed the enemy duke!
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
                                //Debug.Log("Ho ri-selezionato una mia pedina");
                                _currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();
                                _gameUIBehaviour.ShowPieceInfo(_currentSelectedPiece.UnitIndex(), _myNetId, _currentSelectedPiece.IsPhaseOne, false);

                                int piece_i_index = _currentSelectedPiece.I;
                                int piece_j_index = _currentSelectedPiece.J;

                                CmdHighlightTile(piece_i_index, piece_j_index, Ptype.Spawn, this.connectionToClient);

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

                                foreach (var item in _currentAvailableMoves)
                                {
                                    CmdHighlightTile(item.Key.Item1, item.Key.Item2, item.Value, this.connectionToClient);
                                }
                            }
                        }

                        //Piece in cache, hit a tile (MOVING PIECE)
                        else if (_currentSelectedPiece != null && objectHit.CompareTag("Tile"))
                        {
                            CmdDeHighlightAllTiles(this.connectionToClient);
                            _gameUIBehaviour.HidePieceInfo();

                            //Debug.Log("Ho selezionato un tile mentre ho una pedina selezionata");
                            int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                            int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                            int currentPieceUnitIndex = _currentSelectedPiece.UnitIndex();

                            foreach (var item in _currentAvailableMoves)
                            {
                                if (item.Key.Item1 == tile_i_index && item.Key.Item2 == tile_j_index)
                                {
                                    hasDoneSomething = true;
                                    yield return StartCoroutine(MovingAnimationCoroutine(_myNetId, currentPieceUnitIndex, item.Value));

                                    CmdMovePiece(_currentSelectedPiece.gameObject, objectHit.transform, tile_i_index, tile_j_index);

                                    yield return new WaitUntil(() => _currentSelectedPiece.transform.position.x == objectHit.transform.position.x &&
                                    _currentSelectedPiece.transform.position.z == objectHit.transform.position.z);

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

                        //Hit the DrawButton
                        else if (objectHit.CompareTag("DrawButton"))
                        {
                            
                        }

                        //Hit the board (or something else which is not a tile)
                        else
                        {
                            CmdDeHighlightAllTiles(this.connectionToClient);
                            
                            _gameUIBehaviour.HidePieceInfo();

                            _currentSelectedPiece = null;
                            _currentAvailableMoves.Clear();
                        }
                    }

                    //No Hit (Background) -> "deselect" and Clear references
                    else
                    {
                        CmdDeHighlightAllTiles(this.connectionToClient);

                        _gameUIBehaviour.HidePieceInfo();

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
                            //Debug.Log("Ho selezionato un tile e devo spawnare la carta che ho pescato");
                            int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                            int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                            foreach (Tuple<int, int> tuple in _currentAvailableSpawnPositions)
                            {
                                if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                                {
                                    CmdDeHighlightAllTiles(this.connectionToClient);

                                    _gameUIBehaviour.HidePieceInfo();
                                    yield return StartCoroutine(SpawningAnimationCoroutine(_myNetId, _currentDrawnCard));
                                    _gameUIBehaviour.DrawnUnitPlaced();

                                    if(_currentDrawnCard != 0)
                                    {
                                        hasDoneSomething = true;
                                        CmdSpawnPiece(_currentDrawnCard, objectHit.transform, tile_i_index, tile_j_index);
                                    }

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

            default:
                break;
        }

        yield return null;
    }

    //ANIMATION STUFF
    private IEnumerator BattleAnimationCoroutine(int netId, int attackedUnitIndex, int attackerUnitIndex)
    {
        switch (netId)
        {
            case 1:
                switch (attackerUnitIndex)
                {
                    //Ranged Red attacks Blue
                    case 3: case 6: case 9: case 10: case 12: case 13:
                        CmdPlayAnimation(14, (attackedUnitIndex * 2), (attackerUnitIndex * 2) + 1);
                        break;

                    //Melee Red attacks Blue
                    case 0: case 1: case 2: case 4: case 5: case 7: case 8: case 11:
                        CmdPlayAnimation(2, (attackedUnitIndex * 2), (attackerUnitIndex * 2) + 1);
                        break;

                    default:
                        break;
                }
                break;

            case 2:
                switch (attackerUnitIndex)
                {
                    //Ranged Blue attacks Red
                    case 3: case 6: case 9: case 10: case 12: case 13:
                        CmdPlayAnimation(13, (attackerUnitIndex * 2), (attackedUnitIndex * 2) + 1);
                        break;

                    //Melee Blue attacks Red
                    case 0: case 1: case 2: case 4: case 5: case 7: case 8: case 11:
                        CmdPlayAnimation(1, (attackerUnitIndex * 2), (attackedUnitIndex * 2) + 1);
                        break;

                    default:
                        break;
                }
                break;
        }
    
        yield return new WaitUntil(() => _canvasAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationDone"));
    }

    private IEnumerator MovingAnimationCoroutine(int netId, int movingUnitIndex, Ptype moveType)
    {
        switch (netId)
        {
            //RED
            case 1:
                switch (moveType)
                {
                    case Ptype.Walk:
                        CmdPlayAnimation(4, 0, (movingUnitIndex * 2) + 1);
                        break;
                    case Ptype.Jump:
                        CmdPlayAnimation(6, 0, (movingUnitIndex * 2) + 1);
                        break;
                    case Ptype.Slide:
                        CmdPlayAnimation(8, 0, (movingUnitIndex * 2) + 1);
                        break;
                    case Ptype.Fly:
                        CmdPlayAnimation(10, 0, (movingUnitIndex * 2) + 1);
                        break;
                }
                break;

            //BLUE
            case 2:
                switch (moveType)
                {
                    case Ptype.Walk:
                        CmdPlayAnimation(3, (movingUnitIndex * 2), 1);
                        break;
                    case Ptype.Jump:
                        CmdPlayAnimation(5, (movingUnitIndex * 2), 1);
                        break;
                    case Ptype.Slide:
                        CmdPlayAnimation(7, (movingUnitIndex * 2), 1);
                        break;
                    case Ptype.Fly:
                        CmdPlayAnimation(9, (movingUnitIndex * 2), 1);
                        break;
                }
                break;
        }

        yield return new WaitUntil(() => _canvasAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationDone"));
    }

    private IEnumerator SpawningAnimationCoroutine(int netId, int spawningUnitIndex)
    {
        switch (netId)
        {
            //RED
            case 1:
                CmdPlayAnimation(12, 0, (spawningUnitIndex * 2) + 1);
                break;

            //BLUE
            case 2:
                CmdPlayAnimation(11, (spawningUnitIndex * 2), 1);
                break;
        }

        yield return new WaitUntil(() => _canvasAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationDone"));
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
            CmdDeHighlightAllTiles(this.connectionToClient);

            _hasDrawn = true;
            _currentDrawnCard = GetDrawnCard();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int iSpawnPos = _dukeI + i;
                    int jSpawnPos = _dukeJ + j;

                    if (iSpawnPos <= 5 && jSpawnPos <= 5 && iSpawnPos >= 0 && jSpawnPos >= 0)
                    {
                        if (!(Math.Abs(i) == Math.Abs(j)))
                        {
                            if (_erDucaNetworkManager.GetMatrixIdAt(iSpawnPos, jSpawnPos) == 0)
                            {
                                _currentAvailableSpawnPositions.Add(new Tuple<int, int>(iSpawnPos, jSpawnPos));
                                CmdHighlightTile(iSpawnPos, jSpawnPos, Ptype.Spawn, this.connectionToClient);
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

                    if (iSpawnPos <= 5 && jSpawnPos <= 5 && iSpawnPos >= 0 && jSpawnPos >= 0)
                    {
                        if (!(Math.Abs(i) == Math.Abs(j)))
                        {
                            if (_erDucaNetworkManager.GetMatrixIdAt(iSpawnPos, jSpawnPos) == 0)
                            {
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
            for (int i = 0; i < _gridSize; i++)
            {
                _currentAvailableSpawnPositions.Add(new Tuple<int, int>(5, i));
                CmdHighlightTile(5, i, Ptype.Spawn, this.connectionToClient);
            }
        }
        else
        {
            for (int i = 0; i < _gridSize; i++)
            {
                _currentAvailableSpawnPositions.Add(new Tuple<int, int>(0, i));
                CmdHighlightTile(0, i, Ptype.Spawn, this.connectionToClient);
            }
        }
    }

    public void SpawnPikemen()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int iSpawnPos = _dukeI + i;
                int jSpawnPos = _dukeJ + j;

                if (iSpawnPos <= 5 && jSpawnPos <= 5 && iSpawnPos >= 0 && jSpawnPos >= 0)
                {
                    if (!(Math.Abs(i) == Math.Abs(j)))
                    {
                        if (_erDucaNetworkManager.GetMatrixIdAt(iSpawnPos, jSpawnPos) == 0)
                        {
                            _currentAvailableSpawnPositions.Add(new Tuple<int, int>(iSpawnPos, jSpawnPos));
                            CmdHighlightTile(iSpawnPos, jSpawnPos, Ptype.Spawn, this.connectionToClient);
                        }
                    }
                }
            }
        }
    }

    public override void OnStopClient()
    {
        /*
        Debug.Log(_erDucaNetworkManager.numPlayers);
        if(_erDucaNetworkManager.numPlayers == 2)
        {
            if (!_erDucaNetworkManager.IGaveUp)
            {
                Debug.Log("Ho ragequittato io");
            }
        }
        */
    }

    #endregion
}
