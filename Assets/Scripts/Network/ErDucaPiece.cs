using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Ptype
{
    
    Move,
    Jump
    ,/*
    Slide,
    Fly,
    Strike,
    Command
    */
}

public struct Movement
{
    public int _OffsetX;
    public int _OffsetY;
    public Ptype mType;

    public Movement(int x, int y, Ptype type)
    {
        _OffsetX = x;
        _OffsetY = y;
        mType = type;
    }
}

public class ErDucaPiece : NetworkBehaviour
{
    public List<Movement> _PhaseOneMovementArray;
    public List<Movement> _PhaseTwoMovementArray;

    [SerializeField]
    [SyncVar]private uint _myPlayerNetId;
    [SerializeField]
    [SyncVar]private bool _isPhaseOne = true;

    public uint MyPlayerNetId
    {
        get => _myPlayerNetId;
        set
        {
            _myPlayerNetId = value;
        }
    }
    public bool IsPhaseOne
    {
        get => _isPhaseOne;
        set
        {
            _isPhaseOne = value;
        }
    }
}
