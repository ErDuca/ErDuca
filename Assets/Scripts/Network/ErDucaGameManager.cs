using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum BattleState
{
    CoinFlip,
    PDuke,
    PPikemen,
    PTurn,
    PWin
}

public class ErDucaGameManager : NetworkBehaviour
{
    [SerializeField]
    private bool isOurTurn = false;
    [SerializeField]
    private BattleState currentState = BattleState.CoinFlip;
    public BattleState CurrentState
    {
        get => currentState;
    }
    public bool IsOurTurn
    {
        get => isOurTurn;
    }

    [ClientRpc]
    public void RpcSetTurn()
    {
        // If isOurTurn was true, set it false. If it was false, set it true.
        isOurTurn = !isOurTurn;

        // If isOurTurn (after updating the bool above)
        if (isOurTurn)
        {
            switch (currentState)
            {
                case BattleState.CoinFlip:
                    currentState = BattleState.PDuke;
                    ErDucaPlayer._localPlayer.SpawnDuke();
                    break;

                case BattleState.PDuke:
                    currentState = BattleState.PPikemen;
                    ErDucaPlayer._localPlayer.SpawnPikemen();
                    break;

                case BattleState.PPikemen:
                    currentState = BattleState.PTurn;
                    break;
            }
        }
    }

    [TargetRpc]
    public void RpcBeginMatch(NetworkConnection target)
    {
        isOurTurn = !isOurTurn;

        if (isOurTurn)
        {
            switch (currentState)
            {
                case BattleState.CoinFlip:
                    currentState = BattleState.PDuke;
                    ErDucaPlayer._localPlayer.SpawnDuke();
                    break;
            }
        }
    }
}
