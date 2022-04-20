using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaPlayer : NetworkBehaviour
{
    private Camera _camera;
    private uint _myNetId;

    //Sync Variables
    [SyncVar]public bool _isMyTurn = false;

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
            Transform objectHit = hit.transform;

            if (objectHit.CompareTag("Tile"))// && _currentSelectedPiece == null)
            {

            }
        }
    }

    public override void OnStartClient()
    {
        GameObject player = gameObject;
        _myNetId = player.GetComponent<NetworkIdentity>().netId;
    }
}
