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
    public bool IsOurTurn
    {
        get => isOurTurn;
    }
    public BattleState CurrentState
    {
        get => currentState;
    }

    [ClientRpc]
    public void RpcSetTurn()
    {
        int localPlayerId = (int)ErDucaPlayer.LocalPlayer.MyNetId;

        // If isOurTurn was true, set it false. If it was false, set it true.
        isOurTurn = !isOurTurn;

        // Player who's about to start the turn - Logic
        if (isOurTurn)
        {
            switch (currentState)
            {
                case BattleState.CoinFlip:
                    currentState = BattleState.PDuke;
                    ErDucaPlayer.LocalPlayer.SpawnDuke();
                    break;

                case BattleState.PDuke:
                    currentState = BattleState.PPikemen;
                    ErDucaPlayer.LocalPlayer.SpawnPikemen();
                    break;

                case BattleState.PPikemen:
                    currentState = BattleState.PTurn;

                    if (!ErDucaPlayer.LocalPlayer.IsDeckEmpty() && ErDucaPlayer.LocalPlayer.AreDukeNearTilesFree())
                    {
                        ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowDrawBox();
                    }

                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(localPlayerId);
                    break;

                case BattleState.PTurn:

                    //Se il mazzo non è vuoto / duca ha posti liberi
                    if (!ErDucaPlayer.LocalPlayer.IsDeckEmpty() && ErDucaPlayer.LocalPlayer.AreDukeNearTilesFree())
                    {
                        ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowDrawBox();
                    }

                  
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(localPlayerId);
                    break;
            }
        }
        //Player who just finished its turn - Logic
        else
        {
            //Hides the draw button
            ErDucaPlayer.LocalPlayer.GameUIBehavior.HideDrawBox();

            int invertedIdForAnimation = localPlayerId;

            if (invertedIdForAnimation == 1)
            {
                invertedIdForAnimation = 2;
            }
            else if (invertedIdForAnimation == 2)
            {
                invertedIdForAnimation = 1;
            }

            switch (currentState)
            {
                /*
                case BattleState.PDuke:
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(invertedIdForAnimation);
                    break;
                */

                case BattleState.PPikemen:
                    //currentState = BattleState.PTurn;
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(invertedIdForAnimation);
                    //ErDucaPlayer.LocalPlayer.GameUIBehavior.IconaPensante
                    break;
                    
                case BattleState.PTurn:
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(invertedIdForAnimation);
                    //ErDucaPlayer.LocalPlayer.GameUIBehavior.IconaPensante
                    break;
                    
            }
        }
    }

    [ClientRpc]
    public void RpcWinMatch(uint winnerId)
    {
        isOurTurn = !isOurTurn;

        //The one who has the turn now, is the one who lost (its duke has just been killed)
        if (isOurTurn)
        {
            currentState = BattleState.PLost;
            ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowGameOverScreen((int)winnerId);
        }
        else
        {
            currentState = BattleState.PWin;
            ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowGameOverScreen((int)winnerId);
        }
    }

    //Sprites combat animation RPC
    [ClientRpc]
    public void RpcPlayAnimation(int animId, int idBlue, int idRed)
    {
        ErDucaPlayer.LocalPlayer.BattleAnimationScript.SpritesAnimation(animId, idBlue, idRed);
    }

    //First RPC to be called, to start the match
    [TargetRpc]
    public void RpcBeginMatch(NetworkConnection target)
    {
        isOurTurn = !isOurTurn;
        currentState = BattleState.PDuke;
        ErDucaPlayer.LocalPlayer.SpawnDuke();
    }
    [TargetRpc]
    public void RpcSetAnimatorValues(NetworkConnection target, int playerColor, int coinFlipResult)
    {
        Debug.Log("Sto assegnando i valori");
        ErDucaPlayer.LocalPlayer.GameUIBehavior.SetPlayerInitialValues(playerColor, coinFlipResult);
    }
}
