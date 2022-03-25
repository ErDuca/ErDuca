using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIBehaviour : MonoBehaviour
{
    //For the "card turning" effect when examining piece info
    [Header("Info card turning")]
    [SerializeField] private float infoTurnAnimTime;
    [SerializeField] private GameObject infoImage;
    //For the animations when changing turns
    [Header("Turns")]
    [SerializeField] private GameObject thinkingIcon;
    [SerializeField] private GameObject playersTurnLogos;
    [SerializeField] private GameObject opponentsTurnLogos;
    [SerializeField] private AnimationClip turnSwapAnimation;
    [SerializeField] private Text turnText;
    //For the timer objects
    [Header("Timer")]
    [SerializeField] private Text timeText;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject timeSliderFill;
    private float timeRemaining;
    [SerializeField] private float turnTime;
    private bool changingTurn;

    //TODO: Replace int with proper GameObject type
    public void PieceInfoTurn(int piece) => StartCoroutine(PieceInfoTurnCoroutine(piece));
    IEnumerator PieceInfoTurnCoroutine(int piece)
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
    IEnumerator PlayersTurnStartCoroutine()
    {
        changingTurn = true;
        thinkingIcon.SetActive(false);
        playersTurnLogos.SetActive(true);
        yield return new WaitForSeconds(turnSwapAnimation.length);
        turnText.text = "PLAYER'S\nTURN";
        timeSliderFill.GetComponent<Image>().color = new Color(0.35f, 0.41f, 0.96f);
        playersTurnLogos.SetActive(false);
        timeRemaining = turnTime;
        changingTurn = false;
    }

    public void OpponentsTurnStart() => StartCoroutine(OpponentsTurnStartCoroutine());
    IEnumerator OpponentsTurnStartCoroutine()
    {
        changingTurn = true;
        opponentsTurnLogos.SetActive(true);
        yield return new WaitForSeconds(turnSwapAnimation.length);
        turnText.text = "OPPONENT'S\nTURN";
        timeSliderFill.GetComponent<Image>().color = new Color(0.96f, 0.41f, 0.35f);
        opponentsTurnLogos.SetActive(false);
        thinkingIcon.SetActive(true);
        timeRemaining = turnTime;
        changingTurn = false;
    }

    private void Start()
    {
        turnText.text = "PLAYER'S\nTURN";
        timeText.text = "00:00";
        timeRemaining = turnTime;
        changingTurn = false;
    }

    private void Update()
    {
        if(timeRemaining > 0)
        {
            if(!changingTurn)
            {
                timeRemaining -= Time.deltaTime;
                timeText.text = "00:" + (int)timeRemaining;
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
