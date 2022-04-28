using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaPlayer : NetworkBehaviour
{
    private Camera _camera;
    private uint _myNetId;

    [SerializeField]
    private ErDucaPiece _currentSelectedPiece;
    [SerializeField]
    private List<Tuple<int, int>> _currentAvailableMoves = new List<Tuple<int, int>>();

    [SerializeField]
    [SyncVar] public bool _isMyTurn;
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
    }

    [Command]
    public void CmdMovePiece(GameObject selectedPieceScript, Transform newTransform, int i, int j)
    {
        RpcUpdateLocalNetIdMatrix(selectedPieceScript.GetComponent<ErDucaPiece>().I, selectedPieceScript.GetComponent<ErDucaPiece>().J, 0);
        selectedPieceScript.transform.position = newTransform.position + new Vector3(0f, 30f, 0f);
        selectedPieceScript.GetComponent<ErDucaPiece>().I = i;
        selectedPieceScript.GetComponent<ErDucaPiece>().J = j;
        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
    }

    [ClientRpc]
    void RpcUpdateLocalNetIdMatrix(int i, int j, uint _myNetId)
    {
        ErDucaNetworkManager.singleton.setMatrixIdAt(_myNetId, i, j);
    }

    public void Start()
    {
        _camera = Camera.main;
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
            /*
            for (int ix = 0; ix < 6; ix++)
            {
                Debug.Log(ErDucaNetworkManager.singleton._netIdMatrix[ix, 0] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 1] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 2] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 3] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 4] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 5]);
            }
            
            Debug.Log("///CONFRONTO///");
            Debug.Log("Indici selezionati: " + tile_i_index + " " + tile_j_index);
            Debug.Log("Valore in matrice = " + ErDucaNetworkManager.singleton._netIdMatrix[tile_i_index, tile_j_index]);
            Debug.Log("Valore myNetId = " + _myNetId);
            Debug.Log("///////////////");

            */

            Transform objectHit = hit.transform;

            //CASO NON HO SELEZIONATO NULLA, E CLICCO SU PEDINA
            if (_currentSelectedPiece == null && objectHit.CompareTag("Piece"))
            {
                if (objectHit.gameObject.GetComponent<ErDucaPiece>().MyPlayerNetId == _myNetId)
                {
                    _currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaPiece>();
                    int piece_i_index = _currentSelectedPiece.I;
                    int piece_j_index = _currentSelectedPiece.J;

                    //Aggionare la UI
                    //Placeholder
                   
                    if (_currentSelectedPiece.IsPhaseOne)
                    {
                        //PRENDERE REFERENCE!
                        _currentAvailableMoves = ErDucaNetworkManager.singleton.
                            GetComponent<ErDucaMoveManager>().
                            GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P1MOVARR);
                    }
                    else
                    {
                        _currentAvailableMoves = ErDucaNetworkManager.singleton.
                            GetComponent<ErDucaMoveManager>().
                            GetAvailableMoves(_myNetId, piece_i_index, piece_j_index, _currentSelectedPiece.P1MOVARR);
                    }

                    //Mostrare le mosse disponibili
                }
            }

            //CASO HO SELEZIONATO UNA PEDINA ( HO LE SUE MOSSE ) E SELEZIONO UN TILE
            else if (_currentSelectedPiece != null && objectHit.CompareTag("Tile"))
            {
                
                int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;
                Debug.Log("Voglio andà in: " + tile_i_index + tile_j_index);

                foreach (Tuple<int, int> tuple in _currentAvailableMoves)
                {
                    Debug.Log("Possibile Mossa: " + tuple.Item1 + tuple.Item2);

                    if (tuple.Item1 == tile_i_index && tuple.Item2 == tile_j_index)
                    {
                        Debug.Log("Sto per muovere il piece");
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

    public override void OnStartClient()
    {
        GameObject player = gameObject;

        _myNetId = player.GetComponent<NetworkIdentity>().netId;
    }
}
