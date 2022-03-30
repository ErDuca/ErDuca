using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Type {
    Move,
    Jump,
    Slide,
    Fly,
    Strike,
    Command
}
public class Piece : MonoBehaviour
{
    private string mUnitName;
    private Sprite mUnitSprite;
    public int mPositionX;
    public int mPositionY;

    public bool bIsSelected;
    public bool bIsPhaseOne;

    public List<Movement> mPhaseOneMovementArray;
    public List<Movement> mPhaseTwoMovementArray;


    public Piece(/*int coord_x, int coord_y*/) {
        //mPositionX = coord_x);
        //mPositionY = coord_y);
        bIsSelected = false;
        bIsPhaseOne = true;
        
    }

    public void Move(Movement move) {
        
    }
}

public struct Movement { 
    public int mOffsetX;
    public int mOffsetY;
    public Type mType;

    public Movement(int x, int y, Type type) {
        mOffsetX = x;
        mOffsetY = y;
        mType = type;
    }
}
