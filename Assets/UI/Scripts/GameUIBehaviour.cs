using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.U2D.Animation;
using Mirror;
using Mirror.Discovery;

public class GameUIBehaviour : MonoBehaviour
{
    [Header("Info card related")]
    [SerializeField] private float infoTurnAnimTime;
    [SerializeField] private GameObject infoImage;
    [SerializeField] private GameObject infoBlock;
    [SerializeField] private Animator infoAnimator;
    [SerializeField] private BattleAnimationsScript battleAnimScript;
    [SerializeField] private SpriteRenderer infoSpriteRenderer;
    [SerializeField] private SpriteLibrary infoSpriteLibrary;
    [SerializeField] private int pieceInfo;
    [SerializeField] private string[] unitNames;
    [SerializeField] private Sprite[] unitMovingTables;
    [SerializeField] private Text pieceInfoName;
    [SerializeField] private Image pieceInfoMoveTable1;
    [SerializeField] private Image pieceInfoMoveTable2;
    [SerializeField] private GameObject pieceInfoMoveBackground1;
    [SerializeField] private GameObject pieceInfoMoveBackground2;

    [Header("Turns related")]
    [SerializeField] private GameObject thinkingIcon;
    [SerializeField] private Text turnText;
    [SerializeField] private Color colorBlue;
    [SerializeField] private Color colorRed;

    [Header("Timer related")]
    [SerializeField] private Text timeText;
    [SerializeField] private Slider timeSlider; 
    [SerializeField] private GameObject timeSliderFill;
    [SerializeField] private GameObject timeSliderImageLeft;
    [SerializeField] private GameObject timeSliderImageRight;
    [SerializeField] private int maxPenalties;
    private float timeRemaining;
    private float timeToDisplay;
    private int timeoutPenalty = 0;
    [SerializeField] private float turnTime;
    [SerializeField] private GameObject messageToast;
    [HideInInspector] public bool changingTurn;
    //private bool hasMatchBegun = false;

    [Header("Options menu related")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Slider musicSliderGO;
    [SerializeField] private Slider sfxSliderGO;
    [SerializeField] private GameObject confirmationWindowGO;

    [Header("Transitions related")]
    [SerializeField] private GameObject transitionManagerGO;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Animator gameAnimator;
    [SerializeField] private bool isFirstTurn;

    public bool IsFirstTurn
    {
        get => isFirstTurn;
        set
        {
            isFirstTurn = value;
        }
    }
    public Animator GameAnimator
    {
        get => gameAnimator;
    }

    [Header("Host menu related")]
    [SerializeField] private GameObject hostMenuGO;
    private bool isHost;

    [Header("Draw Box Related")]
    [SerializeField] private GameObject drawBoxGO;
    [SerializeField] private Animator drawBoxAnimator;
    [SerializeField] private Image drawBoxUnitIcon;
    [SerializeField] private Image drawBoxBaseColor;
    [SerializeField] private Sprite[] unitIcons;

    [Header("Game Over Screen Related")]
    [SerializeField] private GameObject gameOverScreenGO;
    [SerializeField] private GameObject blueWinsScreenGO;
    [SerializeField] private GameObject redWinsScreenGO;

    [Header("Transitions related")]
    [SerializeField] private GameObject sceneTransitionManager;
    private TransitionScript transitionScript;
    public NetworkDiscovery networkDiscovery;

    [Header("Turn related")]
    [SerializeField] private Text firstTurnBoxText;
    [SerializeField] private GameObject firstTurnBoxGO;
    [SerializeField] private GameObject centerPoint;

    private SoundManager soundManager;

    public bool IsHost
    {
        get => isHost;
        set
        {
            isHost = value;
        }
    }

    private void Start()
    {
        transitionAnimator.SetTrigger("sceneStart");

        musicSliderGO.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSliderGO.value = PlayerPrefs.GetFloat("SFXVolume");

        soundManager = GetComponent<SoundManager>();

        //This makes sure that the timer does not start until the animations are done
        changingTurn = true;
    }

    private void Update()
    {
        if(!changingTurn)
            if (timeRemaining > 0)
            {
                
                timeRemaining -= Time.deltaTime;
                //We add 1 so it's more intuitive (if I set 70 secs it starts as 01:10 and not 01:09; time ends as soon as displaying 0)
                timeToDisplay = timeRemaining + 1;
                //Sets time text to minutes:seconds, both always displayed as 2 digits
                timeText.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(timeToDisplay / 60), Mathf.FloorToInt(timeToDisplay % 60));
                timeSlider.value = timeRemaining / turnTime;

            }
            else
            {
                //If a player times out there are maxPenalties "warnings" with a toast appearing on the screen and additional time added
                //After that the player automatically loses
                if (ErDucaPlayer.LocalPlayer.isMyTurn && !(ErDucaPlayer.LocalPlayer.myCurrentBattleState.Equals(BattleState.PWin)))
                {
                    timeoutPenalty++;
                    if (timeoutPenalty > maxPenalties)
                    {
                        int winnerId = (ErDucaPlayer.LocalPlayer.MyNetId == 1) ? 2 : 1;
                        ErDucaPlayer.LocalPlayer.CmdForfeitMatch(winnerId);
                    }
                    else
                    {
                        StartCoroutine(ShowToast("PLACEHOLDER WARNING\nYOUR TIME IS UP!\nNEXT TIME THIS HAPPENS, YOU'LL AUTOMATICALLY LOSE THE MATCH"));
                        timeRemaining = turnTime;
                    }
                }

                else
                {
                    StartCoroutine(ShowToast("PLACEHOLDER WARNING\nOPPONENT'S TIME IS UP!\nONE-PER-MATCH ADDITIONAL TIME GRANTED"));
                    timeRemaining = turnTime;
                }
            }
    }

    #region host menu

    public void CloseHostMenu() => StartCoroutine(CloseHostMenuCoroutine());
    private IEnumerator CloseHostMenuCoroutine()
    {
        gameAnimator.SetBool("hostScreen", false);
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("DefaultState"));
        hostMenuGO.SetActive(false);
        GameStart();
    }

    #endregion

    #region game start
    private void GameStart()
    {
        timeRemaining = turnTime;
        changingTurn = true;
        isFirstTurn = true;
        //hasMatchBegun = true;
        
        eventSystem.SetActive(false);
        StartCoroutine(GameStartAnimationCoroutine());
    }

    private IEnumerator GameStartAnimationCoroutine()
    {
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        TurnStart(gameAnimator.GetInteger("startingPlayer"));
    }

    public void SetPlayerInitialValues(int colorPlayerId, int startingPlayerId, bool amIHost)
    {
        gameAnimator.SetInteger("playerColor", colorPlayerId);
        gameAnimator.SetInteger("startingPlayer", startingPlayerId);
        SetHost(amIHost);
    }

    public void SetHost(bool amIHost)
    {
        if (amIHost)
        {
            string myName = PlayerPrefs.GetString("Room Name");
            hostMenuGO.transform.Find("MenuWindow/RoomNameText").gameObject.GetComponent<Text>().text = "room name: " + myName;
            hostMenuGO.SetActive(true);
            gameAnimator.SetBool("hostScreen", true);

        }
        else
        {
            gameAnimator.SetBool("hostScreen", false);
            GameStart();
        }
    }

    #endregion

    #region pause menu

    public void PauseGame() => StartCoroutine(PauseGameCoroutine());
    private IEnumerator PauseGameCoroutine()
    {
        eventSystem.SetActive(false);
        if(infoBlock.activeSelf)
            HidePieceInfo();
        pauseMenu.SetActive(true);
        gameAnimator.SetBool("paused", true);
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("paused"));
        eventSystem.SetActive(true);
    }

    public void ResumeGame() => StartCoroutine(ResumeGameCoroutine());
    private IEnumerator ResumeGameCoroutine()
    {
        eventSystem.SetActive(false);
        gameAnimator.SetBool("paused", false);
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        pauseMenu.SetActive(false);
        eventSystem.SetActive(true);
    }

    public void UpdateMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void UpdateSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void OpenConfirmationWindow()
    {
        confirmationWindowGO.SetActive(true);
    }

    public void CloseConfirmationWindow()
    {
        confirmationWindowGO.SetActive(false);
    }

    public void GiveUpMatch()
    {
        // host
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            ErDucaPlayer.LocalPlayer.IGaveUp = true;
            PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
            ErDucaNetworkManager.singleton.StopHost();
            networkDiscovery.StopDiscovery();
        }

        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            ErDucaPlayer.LocalPlayer.IGaveUp = true;
            PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
            ErDucaNetworkManager.singleton.StopClient();
            networkDiscovery.StopDiscovery();
        }
    }

    public void CancelButton()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            networkDiscovery.StopDiscovery();
        }

        /*
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
            networkDiscovery.StopDiscovery();
        }
        transitionScript.LoadSceneByID(0);
        */
    }

    #endregion

    #region info box
    public void ShowPieceInfo(int piece, int netId, bool isPhaseOne, bool isOpponentPiece)
    {
        int mappedPieceValue = (netId % 2 == 0) ? piece * 2 : (piece * 2 ) + 1;

        infoBlock.SetActive(true);

        if (netId == 2)
        {
            infoSpriteRenderer.flipX = false;
            battleAnimScript.AssignSpriteLibrary(infoSpriteLibrary, mappedPieceValue);

            pieceInfoName.text = unitNames[piece];
            pieceInfoMoveTable1.sprite = unitMovingTables[(piece * 2)];
            pieceInfoMoveTable2.sprite = unitMovingTables[(piece * 2) + 1];

            if (isPhaseOne)
            {
                pieceInfoMoveBackground1.SetActive(true);
                pieceInfoMoveBackground2.SetActive(false);
            }
            else
            {
                pieceInfoMoveBackground2.SetActive(true);
                pieceInfoMoveBackground1.SetActive(false);
            }

            if (isOpponentPiece)
            {
                pieceInfoMoveTable1.rectTransform.localScale = new Vector2(-1, -1);
                pieceInfoMoveTable2.rectTransform.localScale = new Vector2(-1, -1);
            }
            else
            {
                pieceInfoMoveTable1.rectTransform.localScale = new Vector2(1, 1);
                pieceInfoMoveTable2.rectTransform.localScale = new Vector2(1, 1);
            }
        }

        else
        {
            infoSpriteRenderer.flipX = true;
            battleAnimScript.AssignSpriteLibrary(infoSpriteLibrary, mappedPieceValue);

            pieceInfoName.text = unitNames[piece];
            pieceInfoMoveTable1.sprite = unitMovingTables[(piece * 2)];
            pieceInfoMoveTable2.sprite = unitMovingTables[(piece * 2) + 1];

            if (isPhaseOne)
            {
                pieceInfoMoveBackground1.SetActive(true);
                pieceInfoMoveBackground2.SetActive(false);
            }
            else
            {
                pieceInfoMoveBackground2.SetActive(true);
                pieceInfoMoveBackground1.SetActive(false);
            }

            if (isOpponentPiece)
            {
                pieceInfoMoveTable1.rectTransform.localScale = new Vector2(-1, -1);
                pieceInfoMoveTable2.rectTransform.localScale = new Vector2(-1, -1);
            }
            else
            {
                pieceInfoMoveTable1.rectTransform.localScale = new Vector2(1, 1);
                pieceInfoMoveTable2.rectTransform.localScale = new Vector2(1, 1);
            }
        }        
    }

    public void HidePieceInfo() => StartCoroutine(HidePieceInfoCoroutine());
    private IEnumerator HidePieceInfoCoroutine()
    {
        if(infoBlock.activeSelf == true)
        {
            infoAnimator.SetTrigger("hide");
            yield return new WaitUntil(() => infoAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hidden"));
            infoBlock.SetActive(false);
        }
        
    }

    //Old method with the tile turning animation

    //public void PieceInfoTurn(int piece) => StartCoroutine(PieceInfoTurnCoroutine(piece));
    //private IEnumerator PieceInfoTurnCoroutine(int piece)
    //{
    //    float elapsedTime = 0f;
    //    while (elapsedTime < infoTurnAnimTime)
    //    {
    //        //TODO: get values from current scale
    //        infoImage.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0f, 1f, 1f), (elapsedTime / infoTurnAnimTime));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    infoImage.transform.localScale = Vector3.zero;
    //    //TODO: add image change
    //    elapsedTime = 0f;
    //    while (elapsedTime < infoTurnAnimTime)
    //    {
    //        //TODO: get values from current scale
    //        infoImage.transform.localScale = Vector3.Lerp(new Vector3(0f, 1f, 1f), Vector3.one, (elapsedTime / infoTurnAnimTime));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    infoImage.transform.localScale = Vector3.one;

    //    yield return null;
    //}

    #endregion

    #region turn management

    public void TurnStart(int playerId)
    {
        switch (playerId)
        {
            //Host - Red
            case 1:
                OpponentsTurnStart();
                break;

            //Remote Client - Blue
            case 2: PlayersTurnStart();
                break;

            default:
                PlayersTurnStart();
                break;
        }
    }
    private void PlayersTurnStart() => StartCoroutine(PlayersTurnStartCoroutine());
    private IEnumerator PlayersTurnStartCoroutine()
    {
        eventSystem.SetActive(false);
        //changingturn stops time during turn swap animations
        changingTurn = true;
        //TODO: Implement true behaviour for thinking icon (it should disappear when the opponent's move animations start playing out)
        thinkingIcon.SetActive(false);
        gameAnimator.SetTrigger("playersTurn");
        yield return new WaitUntil(() => (gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("turnChangeDone")));

        //Everything turn-related gets moved to the left side
        timeSliderImageRight.SetActive(false);
        timeSliderImageLeft.SetActive(true);
        turnText.alignment = TextAnchor.MiddleLeft;
        turnText.text = "bLUE'S\nTURN";
        timeSlider.direction = Slider.Direction.LeftToRight;
        timeSliderFill.GetComponent<Image>().color = colorBlue;

        timeRemaining = turnTime;
        changingTurn = false;
        eventSystem.SetActive(true);

        PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 1);
    }

    private void OpponentsTurnStart() => StartCoroutine(OpponentsTurnStartCoroutine());
    private IEnumerator OpponentsTurnStartCoroutine()
    {
        eventSystem.SetActive(false);
        //changingturn stops turn timer during turn swap animations
        changingTurn = true;
        gameAnimator.SetTrigger("opponentsTurn");
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("turnChangeDone"));

        //Everything turn-related gets moved to the right side
        timeSliderImageLeft.SetActive(false);
        timeSliderImageRight.SetActive(true);
        turnText.alignment = TextAnchor.MiddleRight;
        turnText.text = "RED'S\nTURN";
        timeSlider.direction = Slider.Direction.RightToLeft;
        timeSliderFill.GetComponent<Image>().color = colorRed;

        //TODO: Implement true behaviour for thinking icon (it should disappear when the opponent's move animations start playing out)
        thinkingIcon.SetActive(true);
        timeRemaining = turnTime;
        changingTurn = false;
        eventSystem.SetActive(true);

        PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 1);
    }

    public void ShowFirstTurnMessage(string message)
    {
        if (isFirstTurn)
        {
            firstTurnBoxGO.SetActive(true);
            firstTurnBoxText.text = message;
        }
    }

    public void HideFirstTurnMessage()
    {
        if (isFirstTurn)
        {
            StartCoroutine(HideFirstTurnMessageCoroutine());
        }
    }

    public void KillFirstTurnMessage()
    {
        firstTurnBoxGO.SetActive(false);
    }
    
    public IEnumerator HideFirstTurnMessageCoroutine()
    {
        firstTurnBoxGO.GetComponent<Animator>().SetTrigger("firstMessageFadeOut");
        yield return new WaitUntil(() => firstTurnBoxGO.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("firstTurnMessageDone"));
        firstTurnBoxGO.SetActive(false);
    }
    
    private IEnumerator ShowToast(string message)
    {
        messageToast.GetComponentInChildren<Text>().text = message;
        messageToast.SetActive(true);
        yield return new WaitUntil(() => messageToast.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ToastShown"));
        messageToast.SetActive(false);
    }

    #endregion

    #region draw box

    public void ShowDrawBox()
    {
        drawBoxGO.SetActive(true);
    }

    public void HideDrawBox() => StartCoroutine(HideDrawBoxCoroutine());
    private IEnumerator HideDrawBoxCoroutine()
    {
        drawBoxAnimator.SetTrigger("drawOut");
        yield return new WaitUntil(() => drawBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("drawBoxHidden"));
        drawBoxGO.SetActive(false);
    }

    public void DrawUnit()
    {
        drawBoxAnimator.SetTrigger("unitDrawn");
        int cardIndex = ErDucaPlayer.LocalPlayer.DrawCard();
        int playerColor = ErDucaPlayer.LocalPlayer.MyNetId;

        //Pass in some way the unit to display
        drawBoxBaseColor.color = ErDucaPlayer.LocalPlayer.MyColor;
        drawBoxUnitIcon.sprite = unitIcons[cardIndex];

        ShowPieceInfo(cardIndex, playerColor, true, false);
    }

    public void DrawnUnitPlaced() => StartCoroutine(DrawnUnitPlacedCoroutine());
    private IEnumerator DrawnUnitPlacedCoroutine()
    {
        drawBoxAnimator.SetTrigger("unitPlaced");
        //HidePieceInfo();
        yield return new WaitUntil(() => drawBoxAnimator.GetCurrentAnimatorStateInfo(0).IsName("drawBoxHidden"));
        drawBoxGO.SetActive(false);           
    }

    #endregion

    #region game over screen

    public void HideTimeSlider()
    {
        timeSlider.gameObject.SetActive(false);
    }

    public void ShowGameOverScreen(int winner)
    {
        gameOverScreenGO.SetActive(true);
        if(winner == 2)
        {
            blueWinsScreenGO.SetActive(true);
            if (gameAnimator.GetInteger("startingPlayer") == 2)
            {
                PlayerPrefsUtility.SetEncryptedInt("Wins", PlayerPrefsUtility.GetEncryptedInt("Wins") + 1);
            }
            else if(gameAnimator.GetInteger("startingPlayer") == 1)
            {
                PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            }
        }
        else if(winner == 1)
        {
            redWinsScreenGO.SetActive(true);
            if (gameAnimator.GetInteger("startingPlayer") == 2)
            {
                PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            }
            else if (gameAnimator.GetInteger("startingPlayer") == 1)
            {
                PlayerPrefsUtility.SetEncryptedInt("Wins", PlayerPrefsUtility.GetEncryptedInt("Wins") + 1);
            }
        }
        else
        {
            //Just a default, it doesn't add wins or losses (but it shows blue as the winner)
            blueWinsScreenGO.SetActive(true);
        }
        //This is an "anti-ragequit": if the player doesn't finish the match, this is not set to 0 and the next time
        //the main menu is open the player gets a loss
        PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
        gameAnimator.SetTrigger("gameOver");
    }

    #endregion

    #region sound

    public void CallSoundManager(Sound element)
    {
        soundManager.PlaySound(element);
    }

    public void CallSoundManagerAttacks()
    {

    }

    #endregion
}