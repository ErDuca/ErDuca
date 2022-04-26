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
    
    [SyncVar] public bool _isMyTurn;
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
        // MODIFICA IL PRIMO TILE DELLA MATRICE
        // WHY???
        // CONTROLLARE TIPOLOGIA STRUTTURA DATI
        Debug.Log("Intra RPC");
        ErDucaNetworkManager.singleton._netIdMatrix[i, j] = _myNetId;
        
        Debug.Log("Post RPC");
    }

    [Command]
    public void CmdSwitchTurn()
    {
        ErDucaNetworkManager.singleton.SwitchTurn();
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

            for (int ix = 0; ix < 6; ix++)
            {
                Debug.Log(ErDucaNetworkManager.singleton._netIdMatrix[ix, 0] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 1] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 2] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 3] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 4] + " "
                + ErDucaNetworkManager.singleton._netIdMatrix[ix, 5]);
            }

            Transform objectHit = hit.transform;

            if (_currentSelectedPiece == null && objectHit.CompareTag("Tile"))
            {
                int tile_i_index = objectHit.gameObject.GetComponent<ErDucaTile>().I;
                int tile_j_index = objectHit.gameObject.GetComponent<ErDucaTile>().J;

                if (!(ErDucaNetworkManager.singleton._netIdMatrix[tile_i_index, tile_j_index] == 0))
                {
                    Debug.Log("Ho cliccato su un tile NON-vuoto");
                    if (ErDucaNetworkManager.singleton._netIdMatrix[tile_i_index, tile_j_index] == _myNetId)
                    {
                        Debug.Log("Ho cliccato su un tile dove c'è una mia pedina");
                        //SELEZIONE
                        /*
                        //_currentSelectedPiece = objectHit.gameObject.GetComponent<ErDucaTile>().Occupier;
                        
                        Fare recuperare le mosse al server, tramite il puntatore in tile
                        Abilito i tile su cui posso andare
                        
                        //objectHit.gameObject.GetComponent<ErDucaTile>().disableCollider();
                        */
                    }
                }
                else
                {
                    Debug.Log("Ho cliccato su un tile vuoto");
                    Debug.Log("Spawning..");
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
