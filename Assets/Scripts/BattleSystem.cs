using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { Start, Player1Turn, Player2Turn, Player1Won, Player2Won }

    public Player mPlayerOne;
    public Player mPlayerTwo;

    public Board mBoard;

    public BattleState mState;
    void Start()
    {
        mState = BattleState.Start;
        Setup();
    }

    void Update() {
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Spawn");
            mBoard.SpawnUnit(mBoard.mBoardTiles[0, 0], mPlayerOne.mUnits[0]);
        }
    }

    void Setup() {
        //Instanzia tutta la roba
    }
}
