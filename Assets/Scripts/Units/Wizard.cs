using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(-1, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(0, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(1, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(1, 1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(0, 1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, 1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Type.Move));

        mPhaseTwoMovementArray.Add(new Movement(-2, -2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(0, -2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, -2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, 0, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, 2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(0, 2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(-2, 2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(-2, 0, Type.Jump));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
