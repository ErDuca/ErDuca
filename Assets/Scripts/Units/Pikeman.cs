using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikeman : ErDucaPiece
{
    [SerializeField]
    private static int unitIndex = 11;
    public override int UnitIndex()
    {
        return unitIndex;
    }
    void Start()
    {
        _PhaseOneMovementArray.Add(new Movement(-1, -1, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(-2, -2, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(1, -1, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(2, -2, Ptype.Walk));

        _PhaseTwoMovementArray.Add(new Movement(0, -1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(0, 1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(0, 2, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-1, -2, Ptype.Strike));
        _PhaseTwoMovementArray.Add(new Movement(1, -2, Ptype.Strike));
    }
}
