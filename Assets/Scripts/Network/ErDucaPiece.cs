using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Ptype
{
    Walk
    /*
    Jump
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
    public Ptype _mType;

    public Movement(int x, int y, Ptype type)
    {
        _OffsetX = x;
        _OffsetY = y;
        _mType = type;
    }
}

public class ErDucaPiece : NetworkBehaviour
{
    protected List<Movement> _PhaseOneMovementArray = new List<Movement>();
    protected List<Movement> _PhaseTwoMovementArray = new List<Movement>();

    [SerializeField]
    [SyncVar]private uint _myPlayerNetId;
    [SerializeField]
    [SyncVar]private bool _isPhaseOne = true;

    [SerializeField]
    [SyncVar] private int _i;
    [SerializeField]
    [SyncVar] private int _j;

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

    public List<Movement> P1MOVARR
    {
        get => _PhaseOneMovementArray;
    }
    public List<Movement> P2MOVARR
    {
        get => _PhaseTwoMovementArray;
    }
}
