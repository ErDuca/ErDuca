using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class BattleAnimationsScript : MonoBehaviour
{
    [Header("GameObjects references")]
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Animator gameAnimator;

    [Header("Sprite Library components")]
    [SerializeField] private SpriteLibrary playerSpriteLibrary;
    [SerializeField] private SpriteLibrary opponentSpriteLibrary;

    [Header("Sprite library Assets")]
    [SerializeField] private SpriteLibraryAsset playerDuke;
    [SerializeField] private SpriteLibraryAsset opponentDuke;
    [SerializeField] private SpriteLibraryAsset playerAssassin;
    [SerializeField] private SpriteLibraryAsset opponentAssassin;

    private enum AnimID
    {
        IDLE,                //0
        PLAYER_ATTACK,       //1
        OPPONENT_ATTACK      //2
    }

    private enum SpriteID
    {
        PLAYER_DUKE,         //0
        OPPONENT_DUKE,       //1
        PLAYER_ASSASSIN,     //2
        OPPONENT_ASSASSIN    //3
    }

    //TEMP: Necessary to assign the method to an UI button. Remove later
    public void OnClickAttack() => AttackAnimation(true, 0, 3);
    public void OnClickAttack2() => AttackAnimation(true, 2, 1);
    public void AttackAnimation(bool isPlayerWinner, int playerId, int opponentId) =>
        StartCoroutine(AttackAnimationCoroutine(isPlayerWinner, playerId, opponentId));
    private IEnumerator AttackAnimationCoroutine(bool isPlayerWinner, int playerId, int opponentId)
    {
        eventSystem.SetActive(false);

        switch (playerId)
        {
            case (int)SpriteID.PLAYER_DUKE:
                playerSpriteLibrary.spriteLibraryAsset = playerDuke;
                break;
            case (int)SpriteID.PLAYER_ASSASSIN:
                playerSpriteLibrary.spriteLibraryAsset = playerAssassin;
                break;
            default:
                break;
        }

        switch (opponentId)
        {
            case (int)SpriteID.OPPONENT_DUKE:
                opponentSpriteLibrary.spriteLibraryAsset = opponentDuke;
                break;
            case (int)SpriteID.OPPONENT_ASSASSIN:
                opponentSpriteLibrary.spriteLibraryAsset = opponentAssassin;
                break;
            default:
                break;
        }

        if (isPlayerWinner)
        {
            gameAnimator.SetInteger("animID", (int)AnimID.PLAYER_ATTACK);
        }
        else
        {
            gameAnimator.SetInteger("animID", (int)AnimID.OPPONENT_ATTACK);
        }
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationDone"));
        gameAnimator.SetInteger("animID", 0);
        eventSystem.SetActive(true);
    }
}
