using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class BattleAnimationsScript : MonoBehaviour
{
    [Header("DEBUG animation sprite setter")]
    [SerializeField] private int animationID;
    [SerializeField] private int playerSpriteID;
    [SerializeField] private int opponentSpriteID;

    [Header("GameObjects references")]
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Animator gameAnimator;

    [Header("Sprite Library components")]
    [SerializeField] private SpriteLibrary playerSpriteLibrary;
    [SerializeField] private SpriteLibrary opponentSpriteLibrary;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private SpriteRenderer opponentSpriteRenderer;

    [Header("Sprite library Assets")]
    [SerializeField] private SpriteLibraryAsset playerDuke;
    [SerializeField] private SpriteLibraryAsset opponentDuke;
    [SerializeField] private SpriteLibraryAsset playerAssassin;
    [SerializeField] private SpriteLibraryAsset opponentAssassin;
    [SerializeField] private SpriteLibraryAsset playerBarbarian;
    [SerializeField] private SpriteLibraryAsset opponentBarbarian;
    [SerializeField] private SpriteLibraryAsset playerBowman;
    [SerializeField] private SpriteLibraryAsset opponentBowman;
    [SerializeField] private SpriteLibraryAsset playerChampion;
    [SerializeField] private SpriteLibraryAsset opponentChampion;
    [SerializeField] private SpriteLibraryAsset playerDragoon;
    [SerializeField] private SpriteLibraryAsset opponentDragoon;
    [SerializeField] private SpriteLibraryAsset playerEngineer;
    [SerializeField] private SpriteLibraryAsset opponentEngineer;
    [SerializeField] private SpriteLibraryAsset playerFootman;
    [SerializeField] private SpriteLibraryAsset opponentFootman;
    [SerializeField] private SpriteLibraryAsset playerKnight;
    [SerializeField] private SpriteLibraryAsset opponentKnight;
    [SerializeField] private SpriteLibraryAsset playerLongbowman;
    [SerializeField] private SpriteLibraryAsset opponentLongbowman;
    [SerializeField] private SpriteLibraryAsset playerMage;
    [SerializeField] private SpriteLibraryAsset opponentMage;
    [SerializeField] private SpriteLibraryAsset playerPikeman;
    [SerializeField] private SpriteLibraryAsset opponentPikeman;
    [SerializeField] private SpriteLibraryAsset playerPriest;
    [SerializeField] private SpriteLibraryAsset opponentPriest;
    [SerializeField] private SpriteLibraryAsset playerSeer;
    [SerializeField] private SpriteLibraryAsset opponentSeer;

    private enum AnimID
    {
        IDLE,                //0
        PLAYER_ATTACK,       //1
        OPPONENT_ATTACK,     //2
        PLAYER_MOVES,        //3
        OPPONENT_MOVES,      //4
        PLAYER_JUMPS,        //5
        OPPONENT_JUMPS,      //6
        PLAYER_SLIDES,       //7
        OPPONENT_SLIDES,     //8
        PLAYER_JUMPSLIDES,   //9
        OPPONENT_JUMPSLIDES, //10
        PLAYER_PLACES,       //11
        OPPONENT_PLACES      //12
    }

    private enum SpriteID
    {
        PLAYER_DUKE,         //0
        OPPONENT_DUKE,       //1
        PLAYER_ASSASSIN,     //2
        OPPONENT_ASSASSIN,   //3
        PLAYER_BARBARIAN,    //4
        OPPONENT_BARBARIAN,  //5
        PLAYER_BOWMAN,       //6
        OPPONENT_BOWMAN,     //7
        PLAYER_CHAMPION,     //8
        OPPONENT_CHAMPION,   //9
        PLAYER_DRAGOON,      //10
        OPPONENT_DRAGOON,    //11
        PLAYER_ENGINEER,     //12
        OPPONENT_ENGINEER,   //13
        PLAYER_FOOTMAN,      //14
        OPPONENT_FOOTMAN,    //15
        PLAYER_KNIGHT,       //16
        OPPONENT_KNIGHT,     //17
        PLAYER_LONGBOWMAN,   //18
        OPPONENT_LONGBOWMAN, //19
        PLAYER_MAGE,         //20
        OPPONENT_MAGE,       //21
        PLAYER_PIKEMAN,      //22
        OPPONENT_PIKEMAN,    //23
        PLAYER_PRIEST,       //24
        OPPONENT_PRIEST,     //25
        PLAYER_SEER,         //24
        OPPONENT_SEER        //25
    }

    private void AssignPlayerSpriteLibrary(int _playerId)
    {
        playerSpriteLibrary.spriteLibraryAsset = _playerId switch
        {
            (int)SpriteID.PLAYER_DUKE => playerDuke,
            (int)SpriteID.PLAYER_ASSASSIN => playerAssassin,
            (int)SpriteID.PLAYER_BARBARIAN => playerBarbarian,
            (int)SpriteID.PLAYER_BOWMAN => playerBowman,
            (int)SpriteID.PLAYER_CHAMPION => playerChampion,
            (int)SpriteID.PLAYER_DRAGOON => playerDragoon,
            (int)SpriteID.PLAYER_ENGINEER => playerEngineer,
            (int)SpriteID.PLAYER_FOOTMAN => playerFootman,
            (int)SpriteID.PLAYER_KNIGHT => playerKnight,
            (int)SpriteID.PLAYER_LONGBOWMAN => playerLongbowman,
            (int)SpriteID.PLAYER_MAGE => playerMage,
            (int)SpriteID.PLAYER_PIKEMAN => playerPikeman,
            (int)SpriteID.PLAYER_PRIEST => playerPriest,
            (int)SpriteID.PLAYER_SEER => playerSeer,
            _ => null,
        };
    }

    private void AssignOpponentSpriteLibrary(int _opponentId)
    {
        opponentSpriteLibrary.spriteLibraryAsset = _opponentId switch
        {
            (int)SpriteID.OPPONENT_DUKE => opponentDuke,
            (int)SpriteID.OPPONENT_ASSASSIN => opponentAssassin,
            (int)SpriteID.OPPONENT_BARBARIAN => opponentBarbarian,
            (int)SpriteID.OPPONENT_BOWMAN => opponentBowman,
            (int)SpriteID.OPPONENT_CHAMPION => opponentChampion,
            (int)SpriteID.OPPONENT_DRAGOON => opponentDragoon,
            (int)SpriteID.OPPONENT_ENGINEER => opponentEngineer,
            (int)SpriteID.OPPONENT_FOOTMAN => opponentFootman,
            (int)SpriteID.OPPONENT_KNIGHT => opponentKnight,
            (int)SpriteID.OPPONENT_LONGBOWMAN => opponentLongbowman,
            (int)SpriteID.OPPONENT_MAGE => opponentMage,
            (int)SpriteID.OPPONENT_PIKEMAN => opponentPikeman,
            (int)SpriteID.OPPONENT_PRIEST => opponentPriest,
            (int)SpriteID.OPPONENT_SEER => opponentSeer,
            _ => null,
        };
    }

    //TEMP: Necessary to assign the method to an UI button. Remove later
    public void OnClickAttack() => SpritesAnimation(animationID, playerSpriteID, opponentSpriteID);
    public void SpritesAnimation(int animationID, int playerId, int opponentId)
    {
        eventSystem.SetActive(false);

        switch (animationID)
        {
            //Player attacks
            case (int)AnimID.PLAYER_ATTACK:
                AssignPlayerSpriteLibrary(playerId);
                AssignOpponentSpriteLibrary(opponentId);
                //Makes it so player's sprite is in front when it wins and behind when it loses (puts the dead body always in the back).
                opponentSpriteRenderer.sortingOrder = 0;
                playerSpriteRenderer.sortingOrder = 1;
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.PLAYER_ATTACK));
                break;
            //Opponent attacks
            case (int)AnimID.OPPONENT_ATTACK:
                AssignPlayerSpriteLibrary(playerId);
                AssignOpponentSpriteLibrary(opponentId);
                //Makes it so player's sprite is in front when it wins and behind when it loses (puts the dead body always in the back).
                opponentSpriteRenderer.sortingOrder = 1;
                playerSpriteRenderer.sortingOrder = 0;
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.OPPONENT_ATTACK));
                break;
            //Player moves
            case (int)AnimID.PLAYER_MOVES:
                AssignPlayerSpriteLibrary(playerId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.PLAYER_MOVES));
                break;
            //Opponent moves
            case (int)AnimID.OPPONENT_MOVES:
                AssignOpponentSpriteLibrary(opponentId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.OPPONENT_MOVES));
                break;
            //Player jumps
            case (int)AnimID.PLAYER_JUMPS:
                AssignPlayerSpriteLibrary(playerId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.PLAYER_JUMPS));
                break;
            //Opponent jumps
            case (int)AnimID.OPPONENT_JUMPS:
                AssignOpponentSpriteLibrary(opponentId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.OPPONENT_JUMPS));
                break;
            //Player slides
            case (int)AnimID.PLAYER_SLIDES:
                AssignPlayerSpriteLibrary(playerId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.PLAYER_SLIDES));
                break;
            //Opponent slides
            case (int)AnimID.OPPONENT_SLIDES:
                AssignOpponentSpriteLibrary(opponentId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.OPPONENT_SLIDES));
                break;
            //Player jumpslides
            case (int)AnimID.PLAYER_JUMPSLIDES:
                AssignPlayerSpriteLibrary(playerId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.PLAYER_JUMPSLIDES));
                break;
            //Opponent jumpslides
            case (int)AnimID.OPPONENT_JUMPSLIDES:
                AssignOpponentSpriteLibrary(opponentId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.OPPONENT_JUMPSLIDES));
                break;
            //Player places
            case (int)AnimID.PLAYER_PLACES:
                AssignPlayerSpriteLibrary(playerId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.PLAYER_PLACES));
                break;
            //Opponent places
            case (int)AnimID.OPPONENT_PLACES:
                AssignOpponentSpriteLibrary(opponentId);
                StartCoroutine(ExecuteAnimationCoroutine((int)AnimID.OPPONENT_PLACES));
                break;
            default:
                eventSystem.SetActive(true);
                break;

        }        
    }
        
    private IEnumerator ExecuteAnimationCoroutine(int animationID)
    {
        gameAnimator.SetInteger("animID", animationID);
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("animationDone"));
        gameAnimator.SetInteger("animID", (int)AnimID.IDLE);
        eventSystem.SetActive(true);
    }
}
