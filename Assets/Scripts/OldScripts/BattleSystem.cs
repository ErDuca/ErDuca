using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public enum BattleState { Start, Player1Turn, Player2Turn, Player1Won, Player2Won }

    public BattleState mState;

    void Start()
    {
        mState = BattleState.Start;
        Setup();
    }

    void Setup() {

    }
}
