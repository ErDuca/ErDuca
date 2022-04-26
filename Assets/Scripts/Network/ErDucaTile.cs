using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ErDucaTile : NetworkBehaviour
{
    [SerializeField]
    [SyncVar] private ErDucaPiece _occupier;

    private BoxCollider _myCollider;

    [SerializeField]
    private int _i;
    [SerializeField]
    private int _j;

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
    public ErDucaPiece Occupier
    {
        get => _occupier;
        set
        {
            _occupier = value;
        }
    }

    private void Start()
    {
        _myCollider = GetComponent<BoxCollider>();
    }

    public bool IsTileOccupiedByNetId(uint netId)
    {
        if (_occupier)
            return netId == _occupier.MyPlayerNetId;
        else
            return false;
    }

    public void setOccupier(ErDucaPiece piece)
    {
        _occupier = piece;
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
