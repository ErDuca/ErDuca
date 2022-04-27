using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public enum Ptype
{
    Move,
    Jump,
    Slide,
    Fly,
    Strike,
    Command
}

public struct Movement
{
    public int mOffsetX;
    public int mOffsetY;
    public Ptype mType;

    public Movement(int x, int y, Ptype type)
    {
        mOffsetX = x;
        mOffsetY = y;
        mType = type;
    }
}

public class ErDucaPiece : NetworkBehaviour
{
    private string _UnitName;
    private Sprite _UnitSprite;

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
