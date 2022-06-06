using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin : ErDucaPiece
{
    [SerializeField]
    private static int unitIndex = 1;
    public override int UnitIndex()
    {
        return unitIndex;
    }

    void Start()
    {
        //Primo valore : su e giu
        //Secondo valore : destra e sinistra
        _PhaseOneMovementArray.Add(new Movement(-2, 0, Ptype.Fly));
        _PhaseOneMovementArray.Add(new Movement(2, -2, Ptype.Fly));
        _PhaseOneMovementArray.Add(new Movement(2, 2, Ptype.Fly));

        _PhaseTwoMovementArray.Add(new Movement(-2, -2, Ptype.Fly));
        _PhaseTwoMovementArray.Add(new Movement(-2, 2, Ptype.Fly));
        _PhaseTwoMovementArray.Add(new Movement(2, 0, Ptype.Fly));
    }
}
