using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin : ErDucaPiece
{
    void Start()
    {
        _PhaseOneMovementArray.Add(new Movement(0, -1, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(-1, 1, Ptype.Slide));
        _PhaseOneMovementArray.Add(new Movement(1, 1, Ptype.Slide));

        _PhaseTwoMovementArray.Add(new Movement(-1, -1, Ptype.Slide));
        _PhaseTwoMovementArray.Add(new Movement(1, -1, Ptype.Slide));
        _PhaseTwoMovementArray.Add(new Movement(0, 1, Ptype.Slide));
    }
}
