using System.Collections;
using UnityEngine;

public class FirstSelectScript : MonoBehaviour
{
    private enum Symbols
    {
        Rock, //0
        Paper, //1
        Scissors //2
    }

    [Header("Animations")] 
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private GameObject symbol1;
    [SerializeField] private GameObject symbol2;
    [SerializeField] private GameObject symbol3;
    [SerializeField] private float choiceAnimationLength;

    private void Start()
    {
        transitionAnimator.SetTrigger("sceneStart");
    }

    public void SymbolPressed(int value) => StartCoroutine(SymbolPressedCoroutine(value));
    private IEnumerator SymbolPressedCoroutine(int value)
    {
        switch(value)
        {
            case (int)Symbols.Rock:
                Debug.Log("rock pressed");







                HideArrows();






                break;

            case (int)Symbols.Paper:
                Debug.Log("paper pressed");
                break;

            case (int)Symbols.Scissors:
                Debug.Log("scissors pressed");
                break;

            default:
                Debug.Log("ERROR: Invalid symbol value");
                break;
        }
        yield return null;
    }

    private void HideArrows()
    {

    }
}
