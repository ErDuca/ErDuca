using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirstSelectScript : MonoBehaviour
{
    private enum Symbols
    {
        Rock, //0
        Paper, //1
        Scissors //2
    }

    private enum Results
    {
        PlayerWin, //0
        Draw, //1
        PlayerLose //2
    }

    [Header("Animations related")]
    [SerializeField] private GameObject SceneTransitionManager;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Animator choiceAnimator;
    [SerializeField] private GameObject firstPhaseGroup;
    [SerializeField] private GameObject secondPhaseGroup;
    private int chosenByPlayer;
    private int chosenByOpponent;
    [SerializeField] private GameObject chosenByPlayerImageGO;
    [SerializeField] private GameObject chosenByOpponentImageGO;
    [SerializeField] private Sprite rockImage;
    [SerializeField] private Sprite paperImage;
    [SerializeField] private Sprite scissorsImage;
    private int result;
    private TransitionScript transitionScript;
    [Header("Timer related")]
    [SerializeField] private GameObject temp;

    private void Start()
    {
        transitionAnimator.SetTrigger("sceneStart");
        choiceAnimator.SetInteger("symbolChosen", -1);
        chosenByPlayer = -1;
        chosenByOpponent = -1;
        secondPhaseGroup.SetActive(false);
        transitionScript = SceneTransitionManager.GetComponent<TransitionScript>();
    }

    public void SymbolPressed(int value) => StartCoroutine(SymbolPressedCoroutine(value));
    private IEnumerator SymbolPressedCoroutine(int value)
    {
        switch (value)
        {
            case (int)Symbols.Rock:
                chosenByPlayer = (int)Symbols.Rock;
                choiceAnimator.SetInteger("symbolChosen", (int)Symbols.Rock);
                chosenByPlayerImageGO.GetComponent<Image>().sprite = rockImage;
                break;
            case (int)Symbols.Paper:
                chosenByPlayer = (int)Symbols.Paper;
                choiceAnimator.SetInteger("symbolChosen", (int)Symbols.Paper);
                chosenByPlayerImageGO.GetComponent<Image>().sprite = paperImage;
                break;
            case (int)Symbols.Scissors:
                chosenByPlayer = (int)Symbols.Scissors;
                choiceAnimator.SetInteger("symbolChosen", (int)Symbols.Scissors);
                chosenByPlayerImageGO.GetComponent<Image>().sprite = scissorsImage;
                break;
            default:
                Debug.Log("ERROR: Invalid symbol value");
                yield return 1;
                break;
        }
        yield return new WaitForSeconds(choiceAnimator.runtimeAnimatorController.animationClips[0].length);
        firstPhaseGroup.SetActive(false);
        secondPhaseGroup.SetActive(true);
        yield return new WaitUntil(() => chosenByOpponent !=-1);

        switch(chosenByPlayer)
        {
            case (int)Symbols.Rock:
                switch(chosenByOpponent)
                {
                    case (int)Symbols.Rock:
                        result = (int)Results.Draw;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = rockImage;
                        break;
                    case (int)Symbols.Paper:
                        result = (int)Results.PlayerLose;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = paperImage;
                        break;
                    case (int)Symbols.Scissors:
                        result = (int)Results.PlayerWin;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = scissorsImage;
                        break;
                    default:
                        Debug.Log("ERROR: Invalid opponent's symbol value");
                        yield return 1;
                        break;
                }
                break;
            case (int)Symbols.Paper:
                switch (chosenByOpponent)
                {
                    case (int)Symbols.Rock:
                        result = (int)Results.PlayerWin;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = rockImage;
                        break;
                    case (int)Symbols.Paper:
                        result = (int)Results.Draw;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = paperImage;
                        break;
                    case (int)Symbols.Scissors:
                        result = (int)Results.PlayerLose;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = scissorsImage;
                        break;
                    default:
                        Debug.Log("ERROR: Invalid opponent's symbol value");
                        yield return 1;
                        break;
                }
                break;
            case (int)Symbols.Scissors:
                switch (chosenByOpponent)
                {
                    case (int)Symbols.Rock:
                        result = (int)Results.PlayerWin;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = rockImage;
                        break;
                    case (int)Symbols.Paper:
                        result = (int)Results.PlayerLose;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = paperImage;
                        break;
                    case (int)Symbols.Scissors:
                        result = (int)Results.Draw;
                        chosenByOpponentImageGO.GetComponent<Image>().sprite = scissorsImage;
                        break;
                    default:
                        Debug.Log("ERROR: Invalid opponent's symbol value");
                        yield return 1;
                        break;
                }
                break;
            default:
                Debug.Log("ERROR: Invalid symbol value");
                yield return 1;
                break;
        }

        switch(result)
        {
            case (int)Results.PlayerWin:
                choiceAnimator.SetInteger("result", (int)Results.PlayerWin);
                break;
            case (int)Results.Draw:
                choiceAnimator.SetInteger("result", (int)Results.Draw);
                break;
            case (int)Results.PlayerLose:
                choiceAnimator.SetInteger("result", (int)Results.PlayerLose);
                break;
            default:
                Debug.Log("ERROR: Invalid result value");
                yield return 1;
                break;
        }

        //TODO: Update the waiting values with exact animationClip names (this is too confusing)
        yield return new WaitForSeconds(choiceAnimator.runtimeAnimatorController.animationClips[4].length);

        if(result == (int)Results.Draw)
        {
            transitionScript.LoadSceneByID(1);
        }
        else
        {
            //TODO: Set in some way the result so that next scene can read it            
            transitionScript.LoadSceneByID(2);
        }
    }

    public void OpponentRandomChoice()
    {
        //TODO: implement the real way to get chosenByOpponent
        chosenByOpponent = Random.Range(0, 3);
    }
}
