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
    private bool iStartFirst = false;

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
        int localPlayerId = ErDucaPlayer.LocalPlayer.MyNetId;

        //If isOurTurn was true, set it false. If it was false, set it true.
        isOurTurn = !isOurTurn;

        //Player who's about to start the turn - Logic
        if (isOurTurn)
        {
            switch (currentState)
            {
                case BattleState.CoinFlip:
                    currentState = BattleState.PDuke;
                    ErDucaPlayer.LocalPlayer.SpawnDuke();
                    ErDucaPlayer.LocalPlayer.hasDoneSomething = false;
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(localPlayerId);
                    break;

                case BattleState.PDuke:
                    currentState = BattleState.PPikemen;
                    ErDucaPlayer.LocalPlayer.SpawnPikemen();
                    ErDucaPlayer.LocalPlayer.hasDoneSomething = false;
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(localPlayerId);
                    break;

                case BattleState.PPikemen:
                    currentState = BattleState.PTurn;

                    ErDucaPlayer.LocalPlayer.GameUIBehavior.IsFirstTurn = false;
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.KillFirstTurnMessage();

                    if (!ErDucaPlayer.LocalPlayer.IsDeckEmpty() && ErDucaPlayer.LocalPlayer.AreDukeNearTilesFree())
                    {
                        ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowDrawBox();
                    }

                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(localPlayerId);
                    ErDucaPlayer.LocalPlayer.hasDoneSomething = false;
                    break;

                case BattleState.PTurn:
                    if (!ErDucaPlayer.LocalPlayer.IsDeckEmpty() && ErDucaPlayer.LocalPlayer.AreDukeNearTilesFree())
                    {
                        ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowDrawBox();
                    }

                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(localPlayerId);
                    ErDucaPlayer.LocalPlayer.hasDoneSomething = false;

                    break;
            }
        }

        //Player who just finished its turn - Logic
        else
        {
            int invertedIdForAnimation = localPlayerId == 1 ? 2 : 1;

            switch (currentState)
            {
                case BattleState.CoinFlip:
                    break;

                case BattleState.PDuke:
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(invertedIdForAnimation);
                    break;

                case BattleState.PPikemen:

                    if (!iStartFirst)
                    {
                        //Stai uscendo da Pikemen, e vedendo il primo turno
                        ErDucaPlayer.LocalPlayer.GameUIBehavior.IsFirstTurn = false;
                        ErDucaPlayer.LocalPlayer.GameUIBehavior.KillFirstTurnMessage();
                    }

                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(invertedIdForAnimation);
                    break;

                case BattleState.PTurn:
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.HideDrawBox();
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.HidePieceInfo();
                    ErDucaPlayer.LocalPlayer.GameUIBehavior.TurnStart(invertedIdForAnimation);
                    break;
            }
        }
    }

    [ClientRpc]
    public void RpcWinMatch(int winnerId)
    {
        isOurTurn = !isOurTurn;

        //The one who has the turn now, is the one who lost (its duke has just been killed)
        if (isOurTurn)
        {
            currentState = BattleState.PLost;
            ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowGameOverScreen(winnerId);
            ErDucaPlayer.LocalPlayer.GameUIBehavior.changingTurn = true;
        }
        else
        {
            currentState = BattleState.PWin;
            ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowGameOverScreen(winnerId);
            ErDucaPlayer.LocalPlayer.GameUIBehavior.changingTurn = true;
        }
    }

    [ClientRpc]
    public void RpcForfeitMatch(int winnerId)
    {
        isOurTurn = !isOurTurn;

        if (isOurTurn)
        {
            currentState = BattleState.PWin;
            ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowGameOverScreen(winnerId);
            ErDucaPlayer.LocalPlayer.GameUIBehavior.HidePieceInfo();
            ErDucaPlayer.LocalPlayer.GameUIBehavior.HideTimeSlider();
            ErDucaPlayer.LocalPlayer.GameUIBehavior.changingTurn = true;
        }
        else
        {
            currentState = BattleState.PLost;
            ErDucaPlayer.LocalPlayer.GameUIBehavior.ShowGameOverScreen(winnerId);
            ErDucaPlayer.LocalPlayer.GameUIBehavior.HidePieceInfo();
            ErDucaPlayer.LocalPlayer.GameUIBehavior.HideTimeSlider();
            ErDucaPlayer.LocalPlayer.GameUIBehavior.changingTurn = true;
        }
    }

    //Sprites combat animation RPC
    [ClientRpc]
    public void RpcPlayAnimation(int animId, int idBlue, int idRed)
    {
        ErDucaPlayer.LocalPlayer.GameUIBehavior.HidePieceInfo();
        ErDucaPlayer.LocalPlayer.BattleAnimationScript.SpritesAnimation(animId, idBlue, idRed);
    }

    //First RPC to be called, to start the match
    [TargetRpc]
    public void RpcBeginMatch(NetworkConnection target)
    {
        isOurTurn = !isOurTurn;
        iStartFirst = true;
        currentState = BattleState.PDuke;
        ErDucaPlayer.LocalPlayer.SpawnDuke();
    }

    //Setting the clients Animator values
    [TargetRpc]
    public void RpcSetAnimatorValues(NetworkConnection target, int playerColor, int coinFlipResult, bool isHost)
    {
        ErDucaPlayer.LocalPlayer.GameUIBehavior.SetPlayerInitialValues(playerColor, coinFlipResult, isHost);
    }
}
