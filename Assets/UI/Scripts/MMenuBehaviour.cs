using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MMenuBehaviour : MonoBehaviour
{
    [Header("Animations related")]
    [SerializeField] private GameObject mMenuGO;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private float screenChangeWaitTime;
    private Vector3 screen1Position;
    private Vector3 screen2Position;
    private Vector3 screen3Position;
    private Vector3 screen4Position;
    private Vector3 screen5Position;
    private Vector3 screen6Position;
    [SerializeField] private GameObject hostScreen;
    [SerializeField] private GameObject joinScreen;

    [Header("Transitions related")]
    [SerializeField] private GameObject sceneTransitionManager;
    private TransitionScript transitionScript;

    [Header("Join/Host screen related")]
    [SerializeField] private GameObject scrollViewContentGO;
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] private Text roomNameTextGO;
    private string roomName;

    [Header("Extras screen related")]
    [SerializeField] private GameObject rulesScreenGO;
    [SerializeField] private GameObject unitsGuideScreenGO;
    [SerializeField] private GameObject creditsScreenGO;
    private int currentScreen6Page;
    private enum Screen6IDs
    {
        RULES,       //0
        UNITSGUIDE,  //1
        CREDITS      //2
    }    

    //Screen 1 = Main Menu
    //Screen 2 = Options Menu
    //Screen 3 = Multiplayer Match Settings Menu
    //Screen 4 = Extras Menu
    //Screen 5 = Join Menu
    //Screen 6 = Extras Pages (Rules, Unit guide, Credits)

    private void Start()
    {
        screen1Position = mMenuGO.transform.position;
        screen2Position = new Vector3(mMenuGO.transform.position.x, mMenuGO.transform.position.y + Screen.height, mMenuGO.transform.position.z);
        screen3Position = new Vector3(mMenuGO.transform.position.x + Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
        screen4Position = new Vector3(mMenuGO.transform.position.x - Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
        screen5Position = new Vector3(screen3Position.x + Screen.width, screen3Position.y, screen3Position.z);
        screen6Position = new Vector3(screen4Position.x - Screen.width, screen4Position.y, screen4Position.z);

        transitionScript = sceneTransitionManager.GetComponent<TransitionScript>();
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
    //Screen Multiplayer Menu -> Join Menu
    public void GoToScreen5From3() => StartCoroutine(MoveToScreenCoroutine(screen3Position, screen5Position));
    //Screen Join Menu -> Multiplayer Menu
    public void GoToScreen3From5() => StartCoroutine(MoveToScreenCoroutine(screen5Position, screen3Position));
    //Screen Guide -> Screen Extras
    public void GoToScreen4From6() => StartCoroutine(MoveToScreenCoroutine(screen6Position, screen4Position));
    //Screen Extras -> Screen Guide
    public void GoToScreen6From4(int screenId)
    {
        switch (screenId)
        {
            case (int)Screen6IDs.RULES:
                rulesScreenGO.SetActive(true);
                unitsGuideScreenGO.SetActive(false);
                creditsScreenGO.SetActive(false);
                currentScreen6Page = 0;
                StartCoroutine(MoveToScreenCoroutine(screen4Position, screen6Position));
                break;
            case (int)Screen6IDs.UNITSGUIDE:
                rulesScreenGO.SetActive(false);
                unitsGuideScreenGO.SetActive(true);
                creditsScreenGO.SetActive(false);
                currentScreen6Page = 0;
                StartCoroutine(MoveToScreenCoroutine(screen4Position, screen6Position));
                break;
            case (int)Screen6IDs.CREDITS:
                rulesScreenGO.SetActive(false);
                unitsGuideScreenGO.SetActive(false);
                creditsScreenGO.SetActive(true);
                currentScreen6Page = 0;
                StartCoroutine(MoveToScreenCoroutine(screen4Position, screen6Position));
                break;
            default:
                break;
        };
    }

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

    public void HostBeginMatch()
    {
        roomName = roomNameTextGO.text;
        if (roomName.Equals(""))
        {
            roomName = "GameRoom"; 
        }

        transitionScript.LoadSceneByID(1);
    }

    public void JoinSearch()
    {
        //TODO: Actual searching of the match

        //Placeholders string
        //TODO: Set a max of 40 characters for the name
        string roomName = "Lorem Ipsum dolor sit";

        CreateRoomButton(roomName);
    }

    public void CreateRoomButton(string roomName)
    {
        GameObject roomButton = Instantiate(roomButtonPrefab);
        roomButton.transform.SetParent(scrollViewContentGO.transform);
        //TODO: This is a stupid fix, why these value changes from prefab???
        roomButton.GetComponent<RectTransform>().localScale = Vector3.one;
        roomButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 492);
        roomButton.GetComponentInChildren<Text>().text = roomName;
    }

    public void JoinBeginMatch()
    {
        //TODO: Pass variables to game scene
        transitionScript.LoadSceneByID(1);
    }
}




//TEST PLAYERPREFSEDITOR

//ON WINDOWS PLAYERPREFS ARE STORED IN HKCU\Software\ExampleCompanyName\ExampleProductName registry key.
//ON ANDROID PLAYERPREFS ARE STORED IN /data/data/pkg-name/shared_prefs/pkg-name.v2.playerprefs.xml

//PlayerPrefsUtility.SetEncryptedInt("test", 50);
//int loadedNumber = PlayerPrefsUtility.GetEncryptedInt("test");
//Debug.Log(loadedNumber);
//TO CHECK IF PLAYERPREFS HAS ALREADY BEEN SET
//if (!PlayerPrefs.HasKey("example"))
//TEST PLAYERPREFSEDITOR