using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaTile : NetworkBehaviour
{
    private BoxCollider _myCollider;

    [SerializeField]
    [SyncVar]private int _i;
    [SerializeField]
    [SyncVar]private int _j;

    public int I
    {
        get => _i;
        set
        {
                _i = value;
        }
    }
    public int J
    {
        get => _j;
        set
        {
            _j = value;
        }
    }

    private void Start()
    {
        _myCollider = GetComponent<BoxCollider>();
    }

    public void enableCollider()
    {
        _myCollider.enabled = true;
    }
    public void disableCollider()
    {
        _myCollider.enabled = false;
    }
}
