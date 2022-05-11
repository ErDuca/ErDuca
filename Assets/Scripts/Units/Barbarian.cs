using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian : ErDucaPiece
{
    [SerializeField]
    private static int unitIndex = 2;
    public override int UnitIndex()
    {
        return unitIndex;
    }
    void Start()
    {
        //Command not implementable -> Generale is about to be deprecated!
        /*
        mPhaseOneMovementArray.Add(new Movement(0, -1, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(2, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(0, -1, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(-2, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(-2, -1, Ptype.Jump));
        mPhaseOneMovementArray.Add(new Movement(-2, 1, Ptype.Jump));

        mPhaseOneMovementArray.Add(new Movement(0, -1, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(2, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(-2, 0, Ptype.Move));

        mPhaseOneMovementArray.Add(new Movement(1, -1, Ptype.Command));
        mPhaseOneMovementArray.Add(new Movement(1, -1, Ptype.Command));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Ptype.Command));
        mPhaseOneMovementArray.Add(new Movement(1, 1, Ptype.Command));
        */
        // Mancano 2 command che si sovrappongono
    }
}
