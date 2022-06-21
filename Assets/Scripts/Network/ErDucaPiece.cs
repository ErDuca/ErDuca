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
    Strike,
    Spawn
}
public struct Movement
{
    public int offsetX;
    public int offsetY;
    public Ptype mType;

    public Movement(int x, int y, Ptype type)
    {
        offsetX = x;
        offsetY = y;
        mType = type;
    }
}
public abstract class ErDucaPiece : NetworkBehaviour
{
    protected List<Movement> _PhaseOneMovementArray = new List<Movement>();
    protected List<Movement> _PhaseTwoMovementArray = new List<Movement>();

    [SerializeField]
    [SyncVar] private int i;
    [SerializeField]
    [SyncVar] private int j;
    [SerializeField]
    [SyncVar] private int myPlayerNetId;
    [SerializeField]
    [SyncVar] private bool isPhaseOne = true;

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
        get => i;
        set
        {
            i = value;
        }
    }
    public int J
    {
        get => j;
        set
        {
            j = value;
        }
    }
    public int MyPlayerNetId
    {
        get => myPlayerNetId;
        set
        {
            myPlayerNetId = value;
        }
    }
    public bool IsPhaseOne
    {
        get => isPhaseOne;
        set
        {
            isPhaseOne = value;
        }
    }

    public abstract int UnitIndex();
    public void SwitchPhase()
    {
        isPhaseOne = !isPhaseOne;
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
