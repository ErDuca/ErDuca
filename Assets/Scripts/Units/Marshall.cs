using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marshall : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(-2, 0, Ptype.Slide));
        mPhaseOneMovementArray.Add(new Movement(2, 0, Ptype.Slide));
        mPhaseOneMovementArray.Add(new Movement(-2, -2, Ptype.Jump));
        mPhaseOneMovementArray.Add(new Movement(-2, 2, Ptype.Jump));
        mPhaseOneMovementArray.Add(new Movement(0, 2, Ptype.Jump));

        mPhaseTwoMovementArray.Add(new Movement(-1, -1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(0, -1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, -1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 0, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(2, 0, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 0, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-2, 0, Ptype.Move));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
