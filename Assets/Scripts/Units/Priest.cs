using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Piece
{
    
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(-1, -1, Ptype.Slide));
        mPhaseOneMovementArray.Add(new Movement(-1, 1, Ptype.Slide));
        mPhaseOneMovementArray.Add(new Movement(1, 1, Ptype.Slide));
        mPhaseOneMovementArray.Add(new Movement(1, -1, Ptype.Slide));

        mPhaseTwoMovementArray.Add(new Movement(-1, -1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, -1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-2, -2, Ptype.Jump));
        mPhaseTwoMovementArray.Add(new Movement(-2, 2, Ptype.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, 2, Ptype.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, -2, Ptype.Jump));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
