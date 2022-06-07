using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScript : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private AnimationClip transitionStartAnimation;

    public void LoadSceneByID(int id) => StartCoroutine(LoadSceneByIDCoroutine(id));
    private IEnumerator LoadSceneByIDCoroutine(int id)
    {
        //TODO: cannot set EventSystem inactive during scene swap animation
        transitionAnimator.SetTrigger("sceneSwap");        
        yield return new WaitForSeconds(transitionStartAnimation.length);
        SceneManager.LoadSceneAsync(id, LoadSceneMode.Single);
    }

    //Method that just executes the animation without changing the scene (for the host button)
    public void SceneTransitionPlay()
    {
        transitionAnimator.SetTrigger("sceneSwap");
    }
}