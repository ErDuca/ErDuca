using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaPlayer : NetworkBehaviour
{
    private Camera _camera;
    private ErDucaNetworkManager _erDucaNetworkManager;
    private ErDucaMoveManager _erDucaMoveManager;

    [SerializeField]
    private ErDucaPiece _currentSelectedPiece;
    [SerializeField]
    private List<Tuple<int, int>> _currentAvailableMoves = new List<Tuple<int, int>>();

    [SerializeField]
    [SyncVar] public uint _myNetId;
    [SerializeField]
    [SyncVar] public bool _isMyTurn;
    [SerializeField]
    [SyncVar] public bool _isPlayerOne;
    [SerializeField]
    [SyncVar] public Color _myColor;

    [SerializeField] private GameObject piecePrefab1;

    [Command]
    public void CmdSpawnPiece(Transform spawnTransform, int i, int j)
    {
        GameObject piece = Instantiate(piecePrefab1, spawnTransform.position + new Vector3(0f, 30f, 0f), Quaternion.Euler(90,0,0));
        NetworkServer.Spawn(piece);

        piece.GetComponent<ErDucaPiece>().MyPlayerNetId = _myNetId;
        piece.GetComponent<ErDucaPiece>().I = i;
        piece.GetComponent<ErDucaPiece>().J = j;

        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
        RpcOnSpawnPiece(piece.GetComponent<ErDucaPiece>(), _myNetId);
    }

    [ClientRpc]
    public void RpcOnSpawnPiece(ErDucaPiece edp, uint netId)
    {
        if(netId != 1)
        {
            edp.GetComponent<SpriteRenderer>().color = _myColor;
        }
        else
        {
            edp.GetComponent<SpriteRenderer>().color = _myColor;
        }

        if (isServer)
        {
            edp.GetComponentsInChildren<SpriteRenderer>()[1].flipY = true;
            edp.GetComponentsInChildren<SpriteRenderer>()[1].flipX = true;
        }
    }

    //{BROKEN!!!!!!}
    //Da mettere come TargetRPC!
    /*
    [ClientRpc]
    public void RpcOnMovesDisplayed(List<Tuple<int, int>> currentAvailableMoves)
    {
        //ErDucaNetworkManager.singleton.HighlightsTiles(currentAvailableMoves);
    }
    */

    [Command]
    public void CmdMovePiece(GameObject selectedPieceScript, Transform newTransform, int i, int j)
    {
        RpcUpdateLocalNetIdMatrix(selectedPieceScript.GetComponent<ErDucaPiece>().I, selectedPieceScript.GetComponent<ErDucaPiece>().J, 0);

        //selectedPieceScript.transform.position = newTransform.position + new Vector3(0f, 30f, 0f);
        selectedPieceScript.GetComponent<ErDucaPiece>().StartMoveTo(newTransform.position + new Vector3(0f, 30f, 0f));

        selectedPieceScript.GetComponent<ErDucaPiece>().I = i;
        selectedPieceScript.GetComponent<ErDucaPiece>().J = j;

        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
    }

    [Command]
    public void CmdEatPiece(GameObject selectedPieceScript, Transform newTransform, ErDucaPiece enemyPiece, int i, int j)
    {
        RpcUpdateLocalNetIdMatrix(selectedPieceScript.GetComponent<ErDucaPiece>().I, selectedPieceScript.GetComponent<ErDucaPiece>().J, 0);

        //selectedPieceScript.transform.position = newTransform.position + new Vector3(0f, 30f, 0f);
        selectedPieceScript.GetComponent<ErDucaPiece>().StartMoveTo(newTransform.position + new Vector3(0f, 30f, 0f));

        //Triggerare animazione qui
        NetworkServer.Destroy(enemyPiece.gameObject);

        selectedPieceScript.GetComponent<ErDucaPiece>().I = i;
        selectedPieceScript.GetComponent<ErDucaPiece>().J = j;

        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
    }

    [ClientRpc]
    void RpcUpdateLocalNetIdMatrix(int i, int j, uint _myNetId)
    {
        ErDucaNetworkManager.singleton.SetMatrixIdAt(_myNetId, i, j);
    }

    public void Start()
    {
        _camera = Camera.main;
        _erDucaNetworkManager = ErDucaNetworkManager.singleton;
        _erDucaMoveManager = ErDucaNetworkManager.singleton.GetComponent<ErDucaMoveManager>();
        //Pigliarsi anche le altre reference

        if (_myNetId > 1)
        {
            Camera.main.transform.Rotate(0f, 0f, 180f);
        }
    }

    public void Update()
    {
        if (isLocalPlayer && _isMyTurn && Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    public void HandleInput()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            //CASO NON HO SELEZIONATO NULLA, E CLICCO SU UNA PEDINA
            if (_currentSelectedPiece == null && objectHit.CompareTag("Piece"))
            {
                if (objectHit.gameObject.GetComponent<ErDucaPiece>().MyPlayerNetId == _myNetId)
                {
                    Debug.Log("Ho selezionato una mia pedina");
                    _currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();
                    int piece_i_index = _currentSelectedPiece.I;
                    int piece_j_index = _currentSelectedPiece.J;

                    //Aggionare la UI
                    //Placeholder

                    //UiManager.singleton.MakeInfoAppear(int PedinaInfoboxIndex);
                   
                    if (_currentSelectedPiece.IsPhaseOne)
                    {
                        _currentAvailableMoves = _erDucaMoveManager.
                            GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P1MOVARR);
                    }
                    else
                    {
                        _currentAvailableMoves = _erDucaMoveManager.
                            GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P1MOVARR);
                    }

                    //Mostrare le mosse disponibili
                    //RpcOnMovesDisplayed(_currentAvailableMoves);
                }
            }

            //CASO HO SELEZIONATO UNA PEDINA E SELEZIONO UN PIECE NEMICO
            else if (_currentSelectedPiece != null && objectHit.CompareTag("Piece"))
            {
                ErDucaPiece enemyPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();

                if (enemyPiece.MyPlayerNetId != _myNetId)
                {
                    Debug.Log("Ho selezionato una pedina nemica");
                    int enemyPiece_i_index = enemyPiece.I;
                    int enemyPiece_j_index = enemyPiece.J;

                    foreach (Tuple<int, int> tuple in _currentAvailableMoves)
                    {
                        if (tuple.Item1 == enemyPiece_i_index && tuple.Item2 == enemyPiece_j_index)
                        {
                            CmdEatPiece(_currentSelectedPiece.gameObject, objectHit.transform, enemyPiece, enemyPiece_i_index, enemyPiece_j_index);
                        }
                    }

                    _currentSelectedPiece = null;
                    _currentAvailableMoves.Clear();
                }
            }

            //CASO HO SELEZIONATO UNA PEDINA E SELEZIONO UN TILE
            else if (_currentSelectedPiece != null && objectHit.CompareTag("Tile"))
            {
                Debug.Log("Ho selezionato un tile mentre ho una pedina selezionata");
                int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                foreach (Tuple<int, int> tuple in _currentAvailableMoves)
                {
                    if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                    {
                        CmdMovePiece(_currentSelectedPiece.gameObject, objectHit.transform, tile_i_index, tile_j_index);
                    }
                }

                _currentSelectedPiece = null;
                _currentAvailableMoves.Clear();
            }

            //CASO TILE VUOTO -> SPAWNING
            else if (_currentSelectedPiece == null && objectHit.CompareTag("Tile"))
            {
                int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;
                CmdSpawnPiece(objectHit.transform, tile_i_index, tile_j_index);
            }

            else
            {
                _currentSelectedPiece = null;
                _currentAvailableMoves.Clear();
            }
        }

        else
        {
            _currentSelectedPiece = null;
            _currentAvailableMoves.Clear();
        }
    }
}
