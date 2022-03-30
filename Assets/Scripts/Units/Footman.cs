using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footman : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(0, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(0, 1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Type.Move));

        mPhaseTwoMovementArray.Add(new Movement(-1, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(1, -1, Type.Move));
        mPhaseTwoMovementArray.Add(new Movement(0, -2, Type.Move));



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
