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
    private Vector3 screen4Position;

    //Screen 1 = Main Menu
    //Screen 2 = Options Menu
    //Screen 3 = Multiplayer Match Settings Menu
    //Screen 4 = TODO Stats? Deck builder?

    private void Awake()
    {
        screen1Position = mMenuGO.transform.position;      
        screen2Position = new Vector3(mMenuGO.transform.position.x, mMenuGO.transform.position.y + Screen.height, mMenuGO.transform.position.z);
        screen3Position = new Vector3(mMenuGO.transform.position.x + Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
        screen4Position = new Vector3(mMenuGO.transform.position.x - Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
    }

    //Main Menu -> Options Menu
    public void GoToScreen2() => StartCoroutine(MoveToScreenCoroutine(screen1Position, screen2Position));
    //Main Menu -> Multiplayer Menu
    public void GoToScreen3() => StartCoroutine(MoveToScreenCoroutine(screen1Position, screen3Position));
    //Main Menu -> Screen 4
    public void GoToScreen4() => StartCoroutine(MoveToScreenCoroutine(screen1Position, screen4Position));
    //Options Menu -> Main Menu
    public void GoToScreen1From2() => StartCoroutine(MoveToScreenCoroutine(screen2Position, screen1Position));
    //Multiplayer Menu -> Main Menu
    public void GoToScreen1From3() => StartCoroutine(MoveToScreenCoroutine(screen3Position, screen1Position));
    //Screen 4 -> Main Menu
    public void GoToScreen1From4() => StartCoroutine(MoveToScreenCoroutine(screen4Position, screen1Position));
    IEnumerator MoveToScreenCoroutine(Vector3 fromPos, Vector3 toPos)
    {
        float elapsedTime = 0f;
        eventSystem.SetActive(false);
        while (elapsedTime < screenChangeWaitTime)
        {
            mMenuGO.transform.position = Vector3.Lerp(fromPos, toPos, (elapsedTime / screenChangeWaitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mMenuGO.transform.position = toPos;
        eventSystem.SetActive(true);
        yield return null;
    }
}