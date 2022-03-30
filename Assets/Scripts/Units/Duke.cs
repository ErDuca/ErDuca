using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duke : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(1, 0, Type.Slide));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Type.Slide));

        mPhaseTwoMovementArray.Add(new Movement(0, 1, Type.Slide));
        mPhaseTwoMovementArray.Add(new Movement(0, 1, Type.Slide));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
