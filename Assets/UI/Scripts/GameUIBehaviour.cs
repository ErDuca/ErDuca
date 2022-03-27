using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIBehaviour : MonoBehaviour
{
    [Header("Info card related")]
    [SerializeField] private float infoTurnAnimTime;
    [SerializeField] private GameObject infoImage;
    [SerializeField] private GameObject infoBlock;

    [Header("Turns related")]
    [SerializeField] private GameObject thinkingIcon;
    [SerializeField] private GameObject playersTurnLogos;
    [SerializeField] private GameObject opponentsTurnLogos;
    [SerializeField] private AnimationClip turnSwapAnimation;
    [SerializeField] private Text turnText;
    private Color colorBlue = new Color(0.35f, 0.41f, 0.96f);
    private Color colorRed = new Color(0.96f, 0.41f, 0.35f);

    [Header("Timer related")]
    [SerializeField] private Text timeText;
    [SerializeField] private Slider timeSlider; 
    private RectTransform timerSliderRectTransform; //used to get timeSlider's width to move the image on the other side
    [SerializeField] private GameObject timeSliderFill;
    [SerializeField] private GameObject timeSliderImage;
    private bool changingTurn;
    private float timeRemaining;
    private float timeToDisplay;
    [SerializeField] private float turnTime;

    [Header("Pause menu related")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Animator pauseAnimator;
    [SerializeField] private AnimationClip pauseMenuOpenAnimation;
    [SerializeField] private AnimationClip pauseMenuCloseAnimation;

    [Header("Transitions related")]
    [SerializeField] private Animator transitionAnimator;

    public void PauseGame() => StartCoroutine(PauseGameCoroutine());
    private IEnumerator PauseGameCoroutine()
    {
        eventSystem.SetActive(false);
        pauseMenu.SetActive(true);
        yield return new WaitForSeconds(pauseMenuOpenAnimation.length);
        eventSystem.SetActive(true);
    }

    public void ResumeGame() => StartCoroutine(ResumeGameCoroutine());
    private IEnumerator ResumeGameCoroutine()
    {
        eventSystem.SetActive(false);
        pauseAnimator.SetTrigger("closePauseMenu");
        yield return new WaitForSeconds(pauseMenuCloseAnimation.length);
        pauseAnimator.SetTrigger("closePauseMenu");
        pauseMenu.SetActive(false);
        eventSystem.SetActive(true);
    }

    //TODO: Replace int with proper GameObject type
    public void ShowPieceInfo(int piece)
    {
        infoBlock.SetActive(true);
    }
    public void HidePieceInfo()
    {
        infoBlock.SetActive(false);
    }

    //TODO: Replace int with proper GameObject type
    public void PieceInfoTurn(int piece) => StartCoroutine(PieceInfoTurnCoroutine(piece));
    private IEnumerator PieceInfoTurnCoroutine(int piece)
    {
        float elapsedTime = 0f;
        while (elapsedTime < infoTurnAnimTime)
        {
            //TODO: get values from current scale
            infoImage.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0f, 1f, 1f), (elapsedTime / infoTurnAnimTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        infoImage.transform.localScale = Vector3.zero;
        //TODO: add image change
        elapsedTime = 0f;
        while (elapsedTime < infoTurnAnimTime)
        {
            //TODO: get values from current scale
            infoImage.transform.localScale = Vector3.Lerp(new Vector3(0f, 1f, 1f), Vector3.one, (elapsedTime / infoTurnAnimTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        infoImage.transform.localScale = Vector3.one;

        yield return null;
    }

    public void PlayersTurnStart() => StartCoroutine(PlayersTurnStartCoroutine());
    private IEnumerator PlayersTurnStartCoroutine()
    {
        //changingturn stops time during turn swap animations
        changingTurn = true;
        thinkingIcon.SetActive(false);
        playersTurnLogos.SetActive(true);
        //animation starts immediately when enabled
        yield return new WaitForSeconds(turnSwapAnimation.length);

        //Everything tunr-related gets moved on the left side
        turnText.alignment = TextAnchor.MiddleLeft;
        turnText.text = "PLAYER'S\nTURN";
        timeSlider.direction = Slider.Direction.LeftToRight;
        timeSliderFill.GetComponent<Image>().color = colorBlue;
        //TODO: The image bugs out (the position is resolution dependent)
        timeSliderImage.transform.position = new Vector3(timeSlider.transform.position.x - timerSliderRectTransform.rect.width / 2, timeSlider.transform.position.y, timeSlider.transform.position.z);

        playersTurnLogos.SetActive(false);
        timeRemaining = turnTime;
        changingTurn = false;
    }

    public void OpponentsTurnStart() => StartCoroutine(OpponentsTurnStartCoroutine());
    private IEnumerator OpponentsTurnStartCoroutine()
    {
        //changingturn stops turn timer during turn swap animations
        changingTurn = true;
        opponentsTurnLogos.SetActive(true);
        //animation starts immediately when enabled
        yield return new WaitForSeconds(turnSwapAnimation.length);

        //Everything tunr-related gets moved on the right side
        turnText.alignment = TextAnchor.MiddleRight;
        turnText.text = "OPPONENT'S\nTURN";
        timeSlider.direction = Slider.Direction.RightToLeft;
        timeSliderFill.GetComponent<Image>().color = colorRed;
        //TODO: The image bugs out (position is resolution dependent)
        timeSliderImage.transform.position = new Vector3(timeSlider.transform.position.x + timerSliderRectTransform.rect.width / 2, timeSlider.transform.position.y, timeSlider.transform.position.z);

        opponentsTurnLogos.SetActive(false);
        thinkingIcon.SetActive(true);
        timeRemaining = turnTime;
        changingTurn = false;
    }

    private void Start()
    {
        //TODO: A mechanism to choose who starts (other scene?)

        transitionAnimator.SetTrigger("sceneStart");
        timerSliderRectTransform = (RectTransform)timeSlider.transform;
        turnText.text = "PLAYER'S\nTURN";
        timeText.text = "00:00";
        timeRemaining = turnTime;
        changingTurn = false;
        pauseMenu.SetActive(false);
        thinkingIcon.SetActive(false);
        playersTurnLogos.SetActive(false);
        opponentsTurnLogos.SetActive(false);
        infoBlock.SetActive(false);
    }

    private void Update()
    {
        if(timeRemaining > 0)
        {
            if(!changingTurn)
            {
                timeRemaining -= Time.deltaTime;
                //We add 1 because it's more intuitive (if I set 70 secs it starts as 01:10 and not 01:09; time ends as soon as displaying 0)
                timeToDisplay = timeRemaining + 1;
                //Sets time text to minutes:seconds, both always displayed as 2 digits
                timeText.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(timeToDisplay / 60), Mathf.FloorToInt(timeToDisplay % 60));
                timeSlider.value = timeRemaining / turnTime;
            } 
        }
        else
        {
            //TODO: add real action for when time runs out
            Debug.Log("TIME'S UP");
        }
    }
}