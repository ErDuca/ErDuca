using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : ErDucaPiece
{
    [SerializeField]
    private static int unitIndex = 6;
    public override int UnitIndex()
    {
        return unitIndex;
    }
    void Start()
    {
        _PhaseOneMovementArray.Add(new Movement(-2, 0, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(2, 0, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(-2, -2, Ptype.Jump));
        _PhaseOneMovementArray.Add(new Movement(-2, 2, Ptype.Jump));
        _PhaseOneMovementArray.Add(new Movement(0, 2, Ptype.Jump));

        _PhaseTwoMovementArray.Add(new Movement(-1, -1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(0, -1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(1, -1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(1, 0, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(2, 0, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(1, 1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-1, 1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-1, 0, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-2, 0, Ptype.Walk));
    }
}
