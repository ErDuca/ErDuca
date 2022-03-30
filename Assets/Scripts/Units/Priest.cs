using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Piece
{
    
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(-1, -1, Type.Slide));
        mPhaseOneMovementArray.Add(new Movement(-1, 1, Type.Slide));
        mPhaseOneMovementArray.Add(new Movement(1, 1, Type.Slide));
        mPhaseOneMovementArray.Add(new Movement(1, -1, Type.Slide));

        mPhaseTwoMovementArray.Add(new Movement(-1, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(-2, -2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(-2, 2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, 2, Type.Jump));
        mPhaseTwoMovementArray.Add(new Movement(2, -2, Type.Jump));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
