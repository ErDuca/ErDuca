using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duke : ErDucaPiece
{
    [SerializeField]
    private static int unitIndex = 0;
    public override int UnitIndex()
    {
        return unitIndex;
    }
    void Start()
    {
        /*
        _PhaseOneMovementArray.Add(new Movement(0,1, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(0,-1, Ptype.Slide));

        _PhaseTwoMovementArray.Add(new Movement(1,0, Ptype.Slide));
        _PhaseTwoMovementArray.Add(new Movement(-1,0, Ptype.Slide));
        */
        _PhaseOneMovementArray.Add(new Movement(3, 0, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(0, -3, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(-3, 0, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(0, 3, Ptype.Walk));

        _PhaseTwoMovementArray.Add(new Movement(3, 0, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(0, -3, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-3, 0, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(0, 3, Ptype.Walk));
    }
}

