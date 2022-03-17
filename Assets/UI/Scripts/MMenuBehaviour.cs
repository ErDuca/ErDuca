using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMenuBehaviour : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject mMenuGO;
    [Header("Animations Times")]
    [SerializeField] private float screenChangeWaitTime;
    private Vector3 screen1Position;
    private Vector3 screen2Position;
    private Vector3 screen3Position;
    
    //Screen 1 = Main Menu
    //Screen 2 = Options Menu
    //Screen 3 = Multiplayer Match Settings Menu

    private void Awake()
    {
        screen1Position = mMenuGO.transform.position;      
        screen2Position = new Vector3(mMenuGO.transform.position.x, mMenuGO.transform.position.y + Screen.height, mMenuGO.transform.position.z);
        screen3Position = new Vector3(mMenuGO.transform.position.x + Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
    }




    //Main Menu -> Options Menu
    public void GoToScreen2() => StartCoroutine(nameof(GoToScreen2Coroutine));
    IEnumerator GoToScreen2Coroutine()
    {
        float elapsedTime = 0f;
        eventSystem.SetActive(false);
        while(elapsedTime < screenChangeWaitTime)
        {
            mMenuGO.transform.position = Vector3.Lerp(screen1Position, screen2Position, (elapsedTime / screenChangeWaitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mMenuGO.transform.position = screen2Position;
        eventSystem.SetActive(true);
        yield return null;
    }

    //Options Menu -> Main Menu
    public void GoToScreen1From2() => StartCoroutine(nameof(GoToScreen1From2Coroutine));
    IEnumerator GoToScreen1From2Coroutine()
    {
        float elapsedTime = 0f;
        eventSystem.SetActive(false);
        while (elapsedTime < screenChangeWaitTime)
        {
            mMenuGO.transform.position = Vector3.Lerp(screen2Position, screen1Position, (elapsedTime / screenChangeWaitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mMenuGO.transform.position = screen1Position;
        eventSystem.SetActive(true);
        yield return null;
    }

    //Main Menu -> Multiplayer Menu
    public void GoToScreen3() => StartCoroutine(nameof(GoToScreen3Coroutine));
    IEnumerator GoToScreen3Coroutine()
    {
        float elapsedTime = 0f;
        eventSystem.SetActive(false);
        while (elapsedTime < screenChangeWaitTime)
        {
            mMenuGO.transform.position = Vector3.Lerp(screen1Position, screen3Position, (elapsedTime / screenChangeWaitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mMenuGO.transform.position = screen3Position;
        eventSystem.SetActive(true);
        yield return null;
    }

    //Multiplayer Menu -> Main Menu
    public void GoToScreen1From3() => StartCoroutine(nameof(GoToScreen1From3Coroutine));
    IEnumerator GoToScreen1From3Coroutine()
    {
        float elapsedTime = 0f;
        eventSystem.SetActive(false);
        while (elapsedTime < screenChangeWaitTime)
        {
            mMenuGO.transform.position = Vector3.Lerp(screen3Position, screen1Position, (elapsedTime / screenChangeWaitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mMenuGO.transform.position = screen1Position;
        eventSystem.SetActive(true);
        yield return null;
    }




    public void GoToGame()
    {
        SceneManager.LoadScene("Ricci_Test", LoadSceneMode.Single);
    }
}