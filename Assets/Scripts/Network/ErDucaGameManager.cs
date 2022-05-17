using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum BattleState
{
    CoinFlip,
    PDuke,
    PTurn,
    PWin
}

public class ErDucaGameManager : NetworkBehaviour
{
    [SerializeField]
    public bool isOurTurn = false;
    [SerializeField]
    public BattleState currentState = BattleState.CoinFlip;

    [ClientRpc]
    public void RpcSetTurn()
    {
        // If isOurTurn was true, set it false. If it was false, set it true.
        isOurTurn = !isOurTurn;

        // If isOurTurn (after updating the bool above)
        if (isOurTurn)
        {
            if (currentState == BattleState.CoinFlip)
            {
                currentState = BattleState.PDuke;
                ErDucaPlayer._localPlayer.SpawnDuke();
            }

            else if (currentState == BattleState.PDuke)
            {
                currentState = BattleState.PTurn;
            }
        }
    }
}
