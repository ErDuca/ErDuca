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
        RpcUpdateLocalNetIdMatrix(i, j, _myNetId);
    }

    [ClientRpc]
    void RpcUpdateLocalNetIdMatrix(int i, int j, uint _myNetId)
    {
        ErDucaNetworkManager.singleton._netIdMatrix[i, j] = _myNetId;
    }

    /*
    [Command]
    public void CmdMovePiece(GameObject selectedPieceScript, Transform newTransform)
    {
        selectedPieceScript.transform.position = newTransform.position + new Vector3(0f, 1f, 0f);
    }

    [Command]
    public void CmdIsOccupiedByMe(int i, int j, uint myNetId)
    {
        return ErDucaNetworkManager.singleton.TileIsOccupiedByNetId(i, j, myNetId);
    }
    */

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
            */

            Transform objectHit = hit.transform;

            if (_currentSelectedPiece == null && objectHit.CompareTag("Tile"))
            {
                int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                Debug.Log("///CONFRONTO///");
                Debug.Log("Indici selezionati: " + tile_i_index + " " + tile_j_index);
                Debug.Log("Valore in matrice = " + ErDucaNetworkManager.singleton._netIdMatrix[tile_i_index, tile_j_index]);
                Debug.Log("Valore myNetId = " + _myNetId);
                Debug.Log("///////////////");

                
                if (!(ErDucaNetworkManager.singleton._netIdMatrix[tile_i_index, tile_j_index] == 0))
                {
                    if (ErDucaNetworkManager.singleton._netIdMatrix[tile_i_index, tile_j_index] == _myNetId)
                    {
                        Debug.Log("Ho cliccato su un TILE NON-VUOTO dove c'è una MIA pedina");
                        //Mostra Mosse Disponibili
                    }
                    else
                    {
                        Debug.Log("Ho cliccato su un TILE NON-VUOTO dove c'è una pedina NEMICA");
                    }
                }
                else
                {
                    Debug.Log("Ho cliccato su un TILE VUOTO");
                    CmdSpawnPiece(objectHit.transform, tile_i_index, tile_j_index);
                }
            }
            
            else if(_currentSelectedPiece != null && objectHit.CompareTag("Tile"))
            {
                int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;
            }
        }
    }

    public override void OnStartClient()
    {
        GameObject player = gameObject;
        Debug.Log("sto settando il netId!");
        _myNetId = player.GetComponent<NetworkIdentity>().netId;
    }
}
