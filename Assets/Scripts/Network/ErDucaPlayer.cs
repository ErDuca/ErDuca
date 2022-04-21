using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaPlayer : NetworkBehaviour
{
    private Camera _camera;
    private GameObject _currentSelectedPiece;
    private uint _myNetId;

    //Sync Variables
    [SyncVar] public bool _isMyTurn = false;
    [SyncVar] public Color _myColor;

    [SerializeField] private GameObject piecePrefab1;

    [Command]
    public void CmdSpawnPiece(Transform spawnTransform)
    {
        //Debug.Log("Spawning");
        GameObject piece = Instantiate(piecePrefab1, spawnTransform.position + new Vector3(0f, 30f, 0f), Quaternion.Euler(90,0,0));
        NetworkServer.Spawn(piece);
    }

    [Command]
    public void CmdSwitchTurn()
    {
        ErDucaNetworkManager.singleton.SwitchTurn();
    }

    [Command]
    public void CmdMovePiece(GameObject selectedPieceScript, Transform newTransform)
    {
        selectedPieceScript.transform.position = newTransform.position + new Vector3(0f, 1f, 0f);
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
            Debug.Log("RayCasting");
            Transform objectHit = hit.transform;

            if (objectHit.CompareTag("Tile") && _currentSelectedPiece == null)
            {
                CmdSpawnPiece(objectHit);
                CmdSwitchTurn();
            }
            else if (objectHit.CompareTag("Tile") && _currentSelectedPiece != null)
            {
                CmdMovePiece(_currentSelectedPiece, objectHit);
                CmdSwitchTurn();
                _currentSelectedPiece = null;
            }
            /*
            else if (objectHit.CompareTag("Piece"))
            {
                _currentSelectedPiece = objectHit.gameObject;
            }
            */
        }
    }

    public override void OnStartClient()
    {
        GameObject player = gameObject;
        _myNetId = player.GetComponent<NetworkIdentity>().netId;
    }
}
