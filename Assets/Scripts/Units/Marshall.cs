using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marshall : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(-2, 0, Type.Slide));
        mPhaseOneMovementArray.Add(new Movement(2, 0, Type.Slide));
        mPhaseOneMovementArray.Add(new Movement(-2, -2, Type.Jump));
        mPhaseOneMovementArray.Add(new Movement(-2, 2, Type.Jump));
        mPhaseOneMovementArray.Add(new Movement(0, 2, Type.Jump));

        mPhaseTwoMovementArray.Add(new Movement(-1, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(0, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 0, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(2, 0, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 0, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(-2, 0, Type.Move));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
