using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIBehaviour : MonoBehaviour
{
    //For the "card turning" effect when examining piece info
    [SerializeField] private float infoTurnAnimTime;
    [SerializeField] private GameObject infoImage;
    //For the animations when changing turns
    [SerializeField] private GameObject thinkingIcon;
    [SerializeField] private GameObject playersTurnLogos;
    [SerializeField] private GameObject opponentsTurnLogos;
    [SerializeField] private AnimationClip turnSwapAnimation;
    [SerializeField] private Text turnText;

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
        thinkingIcon.SetActive(false);
        playersTurnLogos.SetActive(true);
        yield return new WaitForSeconds(turnSwapAnimation.length);
        turnText.text = "PLAYER'S\nTURN";
        playersTurnLogos.SetActive(false);
    }

    public void OpponentsTurnStart() => StartCoroutine(OpponentsTurnStartCoroutine());
    IEnumerator OpponentsTurnStartCoroutine()
    {
        opponentsTurnLogos.SetActive(true);
        yield return new WaitForSeconds(turnSwapAnimation.length);
        turnText.text = "OPPONENT'S\nTURN";
        opponentsTurnLogos.SetActive(false);
        thinkingIcon.SetActive(true);
    }
}
