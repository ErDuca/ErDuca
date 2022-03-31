using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragoon : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Ptype.Move));
        mPhaseOneMovementArray.Add(new Movement(0, -2, Ptype.Strike));
        mPhaseOneMovementArray.Add(new Movement(-2, -2, Ptype.Strike));
        mPhaseOneMovementArray.Add(new Movement(2, -2, Ptype.Strike));
        

        mPhaseTwoMovementArray.Add(new Movement(0, -1, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(0, -2, Ptype.Move));
        mPhaseTwoMovementArray.Add(new Movement(-1, -2, Ptype.Jump));
        mPhaseTwoMovementArray.Add(new Movement(1, -2, Ptype.Jump));
        mPhaseTwoMovementArray.Add(new Movement(-1, 1, Ptype.Slide));
        mPhaseTwoMovementArray.Add(new Movement(1, 1, Ptype.Slide));

        
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
