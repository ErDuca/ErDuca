using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : Piece
{
    
    void Start()
    {
        mPhaseOneMovementArray.Add(new Movement(0, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(2, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(0, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-2, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-2, -1, Type.Jump));
        mPhaseOneMovementArray.Add(new Movement(-2, 1, Type.Jump));

        mPhaseOneMovementArray.Add(new Movement(0, -1, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(2, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-1, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(-2, 0, Type.Move));
        mPhaseOneMovementArray.Add(new Movement(1, -1, Type.Command));
        mPhaseOneMovementArray.Add(new Movement(1, -1, Type.Command));
        mPhaseOneMovementArray.Add(new Movement(1, 0, Type.Command));
        mPhaseOneMovementArray.Add(new Movement(1, 1, Type.Command));

        // Mancano 2 command che si sovrappongono





    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
