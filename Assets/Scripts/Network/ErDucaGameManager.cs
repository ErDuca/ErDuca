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
    PWin,
    PLost
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

                case BattleState.PTurn:
                    //Turno normale
                    //It's your turn
                    break;
            }
        }
    }

    [ClientRpc]
    public void RpcWinMatch()
    {
        isOurTurn = !isOurTurn;

        //The one who has the turn now, is the one who lost
        if (isOurTurn)
        {
            currentState = BattleState.PLost;
        }
        else
        {
            currentState = BattleState.PWin;
        }
    }

    [TargetRpc]
    public void RpcBeginMatch(NetworkConnection target)
    {
        isOurTurn = !isOurTurn;
        currentState = BattleState.PDuke;
        ErDucaPlayer._localPlayer.SpawnDuke();
    }
}
