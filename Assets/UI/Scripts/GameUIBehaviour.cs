using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.U2D.Animation;
using Mirror;
using Mirror.Discovery;

public class GameUIBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource ostSource;

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
    private int opponentTimeoutPenalty = 0;
    [SerializeField] private float turnTime;
    [SerializeField] private GameObject messageToast;
    [HideInInspector] public bool changingTurn;

    [Header("Options menu related")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Slider musicSliderGO;
    [SerializeField] private Slider sfxSliderGO;
    [SerializeField] private GameObject confirmationWindowGO;
    [SerializeField] private GameObject rulesScreenGO;
    [SerializeField] private GameObject unitsGuideScreenGO;
    [SerializeField] private GameObject extraScreenButtonsGO;
    private int currentExtraPage;
    [SerializeField] private GameObject[] rulesPages;
    [SerializeField] private GameObject[] unitGuidePages;
    private GameObject[] currentPagesArray;
    [SerializeField] private Text pageNumberText;
    private bool isPauseMenuActive;

    [Header("Transitions related")]
    [SerializeField] private GameObject transitionManagerGO;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Animator gameAnimator;

    [Header("Host menu related")]
    [SerializeField] private GameObject hostMenuGO;

    [Header("Draw Box Related")]
    [SerializeField] private GameObject drawBoxGO;
    [SerializeField] private Animator drawBoxAnimator;
    [SerializeField] private Image drawBoxUnitIcon;
    [SerializeField] private Image drawBoxBaseColor;
    [SerializeField] private Sprite[] unitIcons;
    [SerializeField] private Text unitsInPoolTextbox;

    [Header("Game Over Screen Related")]
    [SerializeField] private GameObject gameOverScreenGO;
    [SerializeField] private GameObject blueWinsScreenGO;
    [SerializeField] private GameObject redWinsScreenGO;

    [Header("Transitions related")]
    [SerializeField] private GameObject sceneTransitionManager;
    public NetworkDiscovery networkDiscovery;

    [Header("Turn related")]
    [SerializeField] private Text firstTurnBoxText;
    [SerializeField] private GameObject firstTurnBoxGO;
    private bool isFirstTurn;
    [SerializeField] private string[] firstTurnMessages;
    private string[] myFirstTurnMessages;
    private int firstTurnIndex = 0;
    [SerializeField] private Animator firstTurnMessageAnimator;

    private SoundManager soundManager;

    public bool IsFirstTurn
    {
        get => isFirstTurn;
        set
        {
            isFirstTurn = value;
        }
    }

    public bool IsPauseMenuActive
    {
        get => isPauseMenuActive;
    }

    #region start & update

    private void Start()
    {
        transitionAnimator.SetTrigger("sceneStart");

        //Volume settings
        musicSliderGO.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        sfxSliderGO.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        ostSource.volume = PlayerPrefs.GetFloat("MusicVolume");

        soundManager = GetComponent<SoundManager>();

        //This makes sure that the timer does not start until the animations are done
        changingTurn = true;

        //Creates an array of 4 strings to store the first turn messages (set from editor)
        myFirstTurnMessages = new string[] {"", "", "", ""};
    }

    private void Update()
    {
        //Timer management
        if (!changingTurn)
        {
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
                //If a player times out there are #maxPenalties "warnings" with a toast appearing on the screen and additional time granted
                //Next time time is over after that the player automatically loses
                if (ErDucaPlayer.LocalPlayer.isMyTurn && !(ErDucaPlayer.LocalPlayer.myCurrentBattleState.Equals(BattleState.PWin)))
                {
                    timeoutPenalty++;
                    if(timeoutPenalty > maxPenalties)
                    {
                        //Closes the pause menu if open
                        ResumeGame();
                        //Gives the win to the other player
                        int winnerId = (ErDucaPlayer.LocalPlayer.MyNetId == 1) ? 2 : 1;
                        ErDucaPlayer.LocalPlayer.CmdForfeitMatch(winnerId);
                    }
                    else
                    {
                        StartCoroutine(ShowToast("YOUR TIME IS UP!\nONCE-PER-MATCH ADDITIONAL TIME GRANTED\nNEXT TIME THIS HAPPENS, YOU'LL AUTOMATICALLY LOSE THE MATCH"));
                        timeRemaining = turnTime;
                    }
                }
                //If it's not my turn show a message saying that more time was granted to the opponent
                else
                {
                    opponentTimeoutPenalty++;
                    if(opponentTimeoutPenalty<=maxPenalties)
                    {
                        StartCoroutine(ShowToast("OPPONENT'S TIME IS UP!\nONCE-PER-MATCH ADDITIONAL TIME GRANTED"));
                        timeRemaining = turnTime;
                    }                    
                }
            }
        }
    }

    #endregion

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

        //If the game is not finished "normally" (aka ragequit) LastGameComplete will not be set to 1 and the 
        //player will be getting a loss
        PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 1);
        PlayerPrefsUtility.SetEncryptedInt("GivenUpMatch", 0);

        //Define the order of messages during the first turn of the match
        //depending if you're the first or the second to start
        if ((ErDucaPlayer.LocalPlayer.MyNetId == gameAnimator.GetInteger("startingPlayer")) || ((ErDucaPlayer.LocalPlayer.MyNetId == 0 && gameAnimator.GetInteger("startingPlayer") == 2)))
        {
            myFirstTurnMessages = firstTurnMessages;
        }
        else
        {
            myFirstTurnMessages[0] = firstTurnMessages[1];
            myFirstTurnMessages[1] = firstTurnMessages[0];
            myFirstTurnMessages[2] = firstTurnMessages[3];
            myFirstTurnMessages[3] = firstTurnMessages[2];
        }

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
        isPauseMenuActive = true;
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
        isPauseMenuActive = false;
        eventSystem.SetActive(true);
        rulesScreenGO.SetActive(false);
        unitsGuideScreenGO.SetActive(false);
        extraScreenButtonsGO.SetActive(false);
    }

    public void UpdateMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ostSource.volume = value;
    }

    public void UpdateSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    //Confirmation window when giving up the match
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
        //Case host
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
            StartCoroutine(GiveUpMatchCoroutine(true));
        }

        //Case client
        else if (NetworkClient.isConnected)
        {
            PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
            StartCoroutine(GiveUpMatchCoroutine(false));
        }
    }

    private IEnumerator GiveUpMatchCoroutine(bool hostMode)
    {
        ErDucaPlayer.LocalPlayer.CmdSetGivenUpMatchPref();
        yield return new WaitForSeconds(0.75f);
        if (hostMode)
        {
            ErDucaNetworkManager.singleton.StopHost();
        }
        else
        {
            ErDucaNetworkManager.singleton.StopClient();
        }
        networkDiscovery.StopDiscovery();
    }

    //Subpages of the options menu
    public void OpenRulesScreen()
    {
        currentExtraPage = 0;
        currentPagesArray = rulesPages;
        foreach (GameObject page in currentPagesArray)
            page.SetActive(false);
        rulesScreenGO.SetActive(true);
        unitsGuideScreenGO.SetActive(false);
        extraScreenButtonsGO.SetActive(true);
        currentPagesArray[currentExtraPage].SetActive(true);
        pageNumberText.text = "Page " + (currentExtraPage + 1) + " / " + currentPagesArray.Length;
    }

    public void OpenUnitsGuideScreen()
    {
        currentExtraPage = 0;
        currentPagesArray = unitGuidePages;
        foreach (GameObject page in currentPagesArray)
            page.SetActive(false);
        rulesScreenGO.SetActive(false);
        unitsGuideScreenGO.SetActive(true);
        extraScreenButtonsGO.SetActive(true);
        currentPagesArray[currentExtraPage].SetActive(true);
        pageNumberText.text = "Page " + (currentExtraPage + 1) + " / " + currentPagesArray.Length;
    }

    //Closessubpages of the options menu
    public void CloseExtraScreen()
    {
        rulesScreenGO.SetActive(false);
        unitsGuideScreenGO.SetActive(false);
        extraScreenButtonsGO.SetActive(false);
    }

    public void ChangePage(int offset)
    {
        soundManager.PlaySound(Sound.pageLeft);
        if (currentExtraPage + offset >= 0 && currentExtraPage + offset < currentPagesArray.Length)
        {
            currentPagesArray[currentExtraPage].SetActive(false);
            currentPagesArray[currentExtraPage += offset].SetActive(true);
            pageNumberText.text = "Page " + (currentExtraPage + 1) + " / " + currentPagesArray.Length;
        }
    }

    //Cancel button when waiting for opponent as host (stops searching and goes back to main menu)
    public void CancelButton()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            networkDiscovery.StopDiscovery();
        }
    }

    #endregion

    #region info box
    public void ShowPieceInfo(int piece, int netId, bool isPhaseOne, bool isOpponentPiece)
    {
        int mappedPieceValue = (netId % 2 == 0) ? piece * 2 : (piece * 2 ) + 1;

        infoBlock.SetActive(true);
        soundManager.PlaySound(Sound.openInfoBox);

        //Shows piece info for the BLUE player
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
        //Shows piece info for the RED player
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
                break;
        }
    }
    private void PlayersTurnStart() => StartCoroutine(PlayersTurnStartCoroutine());
    private IEnumerator PlayersTurnStartCoroutine()
    {
        eventSystem.SetActive(false);
        //changingturn stops time during turn swap animations
        changingTurn = true;
        //thinkingIcon.SetActive(false);
        gameAnimator.SetTrigger("playersTurn");
        yield return new WaitUntil(() => (gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("turnChangeDone")));

        //Every turn-related UI thing gets moved to the left side
        timeSliderImageRight.SetActive(false);
        timeSliderImageLeft.SetActive(true);
        turnText.alignment = TextAnchor.MiddleLeft;
        turnText.text = "bLUE'S\nTURN";
        timeSlider.direction = Slider.Direction.LeftToRight;
        timeSliderFill.GetComponent<Image>().color = colorBlue;

        timeRemaining = turnTime;
        changingTurn = false;
        eventSystem.SetActive(true);
    }

    private void OpponentsTurnStart() => StartCoroutine(OpponentsTurnStartCoroutine());
    private IEnumerator OpponentsTurnStartCoroutine()
    {
        eventSystem.SetActive(false);
        //changingturn stops turn timer during turn swap animations
        changingTurn = true;
        gameAnimator.SetTrigger("opponentsTurn");
        yield return new WaitUntil(() => gameAnimator.GetCurrentAnimatorStateInfo(0).IsName("turnChangeDone"));

        //Every turn-related UI thing gets moved to the left side
        timeSliderImageLeft.SetActive(false);
        timeSliderImageRight.SetActive(true);
        turnText.alignment = TextAnchor.MiddleRight;
        turnText.text = "RED'S\nTURN";
        timeSlider.direction = Slider.Direction.RightToLeft;
        timeSliderFill.GetComponent<Image>().color = colorRed;
        //thinkingIcon.SetActive(true);

        timeRemaining = turnTime;
        changingTurn = false;
        eventSystem.SetActive(true);
    }

    //Shows on screen message during the first "special" turns of the match
    //(e.g. place the duke, opponent is placing the footmen, etc.)
    public void ShowFirstTurnMessage()
    {
        if (isFirstTurn)
        {
            firstTurnBoxGO.SetActive(true);

            firstTurnBoxText.text = myFirstTurnMessages[firstTurnIndex];
            if(firstTurnIndex < 4)
                firstTurnIndex++;
        }
    }

    public void HideFirstTurnMessage()
    {
        if (isFirstTurn)
        {
            StartCoroutine(HideFirstTurnMessageCoroutine());
        }
    }

    //After the first turn we don't want on screen messages anymore
    public void KillFirstTurnMessage()
    {
        firstTurnBoxGO.SetActive(false);
    }
    
    public IEnumerator HideFirstTurnMessageCoroutine()
    {
        if(firstTurnMessageAnimator.gameObject.activeSelf)
        {
            firstTurnMessageAnimator.SetTrigger("firstMessageFadeOut");
            yield return new WaitUntil(() => firstTurnMessageAnimator.GetCurrentAnimatorStateInfo(0).IsName("firstTurnMessageDone"));
        }        
        firstTurnBoxGO.SetActive(false);
    }
    
    //Toasts warning the player when its time is up
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
        unitsInPoolTextbox.text = ErDucaPlayer.LocalPlayer.Cards.Count.ToString();
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
        soundManager.PlaySound(Sound.unitDraw);
        drawBoxAnimator.SetTrigger("unitDrawn");
        int cardIndex = ErDucaPlayer.LocalPlayer.DrawCard();
        int playerColor = ErDucaPlayer.LocalPlayer.MyNetId;

        drawBoxBaseColor.color = ErDucaPlayer.LocalPlayer.MyColor;
        drawBoxUnitIcon.sprite = unitIcons[cardIndex];

        ShowPieceInfo(cardIndex, playerColor, true, false);
    }

    public void DrawnUnitPlaced() => StartCoroutine(DrawnUnitPlacedCoroutine());
    private IEnumerator DrawnUnitPlacedCoroutine()
    {
        drawBoxAnimator.SetTrigger("unitPlaced");
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
        //If the options menu is open when a turn ends, it automatically closes
        if (IsPauseMenuActive)
            ResumeGame();

        gameOverScreenGO.SetActive(true);
        // 1 = red
        // 2 = blue
        //Gives wins and losses shown on the stats page
        if(winner == 2)
        {
            blueWinsScreenGO.SetActive(true);
            if (gameAnimator.GetInteger("playerColor") == 2)
            {
                PlayerPrefsUtility.SetEncryptedInt("Wins", PlayerPrefsUtility.GetEncryptedInt("Wins") + 1);
            }
            else if(gameAnimator.GetInteger("playerColor") == 1)
            {
                PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            }
        }
        else if(winner == 1)
        {
            redWinsScreenGO.SetActive(true);
            if (gameAnimator.GetInteger("playerColor") == 2)
            {
                PlayerPrefsUtility.SetEncryptedInt("Losses", PlayerPrefsUtility.GetEncryptedInt("Losses") + 1);
            }
            else if (gameAnimator.GetInteger("playerColor") == 1)
            {
                PlayerPrefsUtility.SetEncryptedInt("Wins", PlayerPrefsUtility.GetEncryptedInt("Wins") + 1);
            }
        }
        else
        {
            //Just a default for consistency, it doesn't add wins or losses (but it shows blue as the winner)
            blueWinsScreenGO.SetActive(true);
        }
        //This is an "anti-ragequit": if the player doesn't finish the match, this is not set to 0 and the next time
        //the main menu is open the player gets a loss
        PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
        gameAnimator.SetTrigger("gameOver");
    }

    public void BackToMenuButton()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            ErDucaNetworkManager.singleton.StopHost();
            networkDiscovery.StopDiscovery();
        }

        else if (NetworkClient.isConnected)
        { 
            ErDucaNetworkManager.singleton.StopClient();
            networkDiscovery.StopDiscovery();
        }
    }

    #endregion

    #region sound

    public void CallSoundManager(Sound element)
    {
        soundManager.PlaySound(element);
    }

    public void PlayNextPageSound() {
        soundManager.PlaySound(Sound.pageLeft);
    }

    public void playClickSound() {
        soundManager.PlaySound(Sound.click);
    }

    public void playDrawSound() {
        soundManager.PlaySound(Sound.unitDraw);
    }

    #endregion
}