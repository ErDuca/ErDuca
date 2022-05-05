using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : ErDucaPiece
{
    
    void Start()
    {
        _PhaseOneMovementArray.Add(new Movement(-1, -1, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(-1, 1, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(1, 1, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(1, -1, Ptype.Slide));

        _PhaseTwoMovementArray.Add(new Movement(-1, -1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-1, 1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(1, 1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(1, -1, Ptype.Walk));
        _PhaseTwoMovementArray.Add(new Movement(-2, -2, Ptype.Jump));
        _PhaseTwoMovementArray.Add(new Movement(-2, 2, Ptype.Jump));
        _PhaseTwoMovementArray.Add(new Movement(2, 2, Ptype.Jump));
        _PhaseTwoMovementArray.Add(new Movement(2, -2, Ptype.Jump));
    }
}
