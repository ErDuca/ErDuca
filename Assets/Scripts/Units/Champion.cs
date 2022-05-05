using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Champion : ErDucaPiece
{
    void Start()
    {
        _PhaseOneMovementArray.Add(new Movement(0, -1, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(1, 0, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(0, 1, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(-1, 0, Ptype.Walk));
        _PhaseOneMovementArray.Add(new Movement(0, -2, Ptype.Jump));
        _PhaseOneMovementArray.Add(new Movement(2, 0, Ptype.Jump));
        _PhaseOneMovementArray.Add(new Movement(0, 2, Ptype.Jump));
        _PhaseOneMovementArray.Add(new Movement(-2, 0, Ptype.Jump));

        _PhaseTwoMovementArray.Add(new Movement(0, -1, Ptype.Strike));
        _PhaseTwoMovementArray.Add(new Movement(1, 0, Ptype.Strike));
        _PhaseTwoMovementArray.Add(new Movement(0, 1, Ptype.Strike));
        _PhaseTwoMovementArray.Add(new Movement(-1, 0, Ptype.Strike));
        _PhaseTwoMovementArray.Add(new Movement(0, -2, Ptype.Jump));
        _PhaseTwoMovementArray.Add(new Movement(2, 0, Ptype.Jump));
        _PhaseTwoMovementArray.Add(new Movement(0, 2, Ptype.Jump));
        _PhaseTwoMovementArray.Add(new Movement(-2, 0, Ptype.Jump));
    }
}
