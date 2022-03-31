using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duke : Piece
{
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(1, 0, Ptype.Slide));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Ptype.Slide));

        mPhaseTwoMovementArray.Add(new Movement(0, 1, Ptype.Slide));
        mPhaseTwoMovementArray.Add(new Movement(0, 1, Ptype.Slide));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
