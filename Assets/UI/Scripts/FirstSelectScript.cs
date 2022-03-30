using System.Collections;
using UnityEngine;

public class FirstSelectScript : MonoBehaviour
{
    private enum Result
    {
        Undefined = -1, //-1
        Player, //0
        Opponent, //1
    }

    [Header("Animations related")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject youGoGO;
    [SerializeField] private GameObject opponentGoesGO;
    [SerializeField] private GameObject firstBlueGO;
    [SerializeField] private GameObject firstRedGO;

    [Header("Transitions related")] 
    [SerializeField] private GameObject SceneTransitionManager;
    [SerializeField] private Animator transitionAnimator;
    private TransitionScript transitionScript;

    public static int startingPlayer;

    private void Start()
    {
        transitionAnimator.SetTrigger("sceneStart");
        animator.SetInteger("StartingPlayer", (int)Result.Undefined);
        transitionScript = SceneTransitionManager.GetComponent<TransitionScript>();

        //TODO: Change with the proper value (networking)
        startingPlayer = Random.Range(0, 2);
        animator.SetInteger("StartingPlayer", startingPlayer);

        StartCoroutine(WaitForAnimationCoroutine());
    }

    private IEnumerator WaitForAnimationCoroutine()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EndState"));
        transitionScript.LoadSceneByID(2);
    }
}