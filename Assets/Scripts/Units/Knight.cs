using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(1, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(0, 1, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(0, 2, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, -2, Ptype.Jump));
        mPhaseOneMovementArray.Add(new Movement(1, -2, Ptype.Jump));

        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-2, 2, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(2, 2, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(0, 1, Ptype.Slide));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
