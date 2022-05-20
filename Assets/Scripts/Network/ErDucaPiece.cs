using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum Unit
{
    DUKE = 0,
    ASSASSIN = 1,
    BARBARIAN = 2,
    PLAYER_BOWMAN = 3,
    CHAMPION = 4,
    DRAGOON = 5,
    ENGINEER = 6,
    FOOTMAN = 7,
    KNIGHT = 8,
    LONGBOWMAN= 9,
    MAGE = 10,
    PIKEMAN = 11,
    PRIEST = 12,
    SEER = 13,
}

public enum Ptype
{
    Walk,
    Jump,
    Slide,
    Fly,
    Strike
}
public struct Movement
{
    public int _offsetX;
    public int _offsetY;
    public Ptype _mType;

    public Movement(int x, int y, Ptype type)
    {
        _offsetX = x;
        _offsetY = y;
        _mType = type;
    }
}
public abstract class ErDucaPiece : NetworkBehaviour
{
    protected List<Movement> _PhaseOneMovementArray = new List<Movement>();
    protected List<Movement> _PhaseTwoMovementArray = new List<Movement>();

    [SerializeField]
    [SyncVar] private int _i;
    [SerializeField]
    [SyncVar] private int _j;
    [SerializeField]
    [SyncVar] private int _myPlayerNetId;
    [SerializeField]
    [SyncVar] private bool _isPhaseOne = true;

    public List<Movement> P1MOVARR
    {
        get => _PhaseOneMovementArray;
    }
    public List<Movement> P2MOVARR
    {
        get => _PhaseTwoMovementArray;
    }
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
    public int MyPlayerNetId
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

    public abstract int UnitIndex();

    public void SwitchPhase()
    {
        _isPhaseOne = !_isPhaseOne;
    }
    public void StartMoveTo(Vector3 target)
    {
        StartCoroutine(MoveTo(target));
    }
    public IEnumerator MoveTo(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float fraction = 0f;

        while(transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, fraction);
            fraction += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
