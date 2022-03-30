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
    private Vector3 screen3_1Position;
    [SerializeField] private GameObject hostScreen;
    [SerializeField] private GameObject joinScreen;
    [Header("IP Insertion related")]
    [SerializeField] private Text hostIPText;
    [SerializeField] private InputField ipInput0Text;
    [SerializeField] private InputField ipInput1Text;
    [SerializeField] private InputField ipInput2Text;
    [SerializeField] private InputField ipInput3Text;
    [SerializeField] private InputField ipInputPortText;
    private char[] ipDelimitators;
    private Regex regex;
    [Header("Transitions related")]
    [SerializeField] private GameObject sceneTransitionManager;
    private TransitionScript transitionScript;

    //Screen 1 = Main Menu
    //Screen 2 = Options Menu
    //Screen 3 = Multiplayer Match Settings Menu
    //Screen 4 = TODO Stats? Deck builder?
    //Screen 3_1 = Host/Join Menu

    private void Start()
    {
        screen1Position = mMenuGO.transform.position;      
        screen2Position = new Vector3(mMenuGO.transform.position.x, mMenuGO.transform.position.y + Screen.height, mMenuGO.transform.position.z);
        screen3Position = new Vector3(mMenuGO.transform.position.x + Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
        screen4Position = new Vector3(mMenuGO.transform.position.x - Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
        screen3_1Position = new Vector3(screen3Position.x + Screen.width, screen3Position.y, screen3Position.z);

        ipDelimitators = new char[] { '.', ':' };
        regex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\:\d{1,5}\b");

        transitionScript = sceneTransitionManager.GetComponent<TransitionScript>();
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
    //Screen Multiplayer Menu -> Host Menu
    public void GoToScreen3_1From3Host()
    {
        joinScreen.SetActive(false);
        hostScreen.SetActive(true);
        StartCoroutine(MoveToScreenCoroutine(screen3Position, screen3_1Position));
    }
    //Screen Multiplayer Menu -> Join Menu
    public void GoToScreen3_1From3Join()
    {
        joinScreen.SetActive(true);
        hostScreen.SetActive(false);
        StartCoroutine(MoveToScreenCoroutine(screen3Position, screen3_1Position));
    }
    //Screen Host/Join Menu -> Multiplayer Menu
    public void GoToScreen3From3_1() => StartCoroutine(MoveToScreenCoroutine(screen3_1Position, screen3Position));
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

    public void CopyIPToClipboard()
    {
        UniClipboard.SetText(hostIPText.text);
    }

    public void PasteIPFromClipboard()
    {
        string clipboardText = UniClipboard.GetText();
        if(IPRegexCheck(clipboardText))
        {
            Debug.Log("IP IS VALID");
            string[] ipParts = clipboardText.Split(ipDelimitators);
            ipInput0Text.text = ipParts[0];
            ipInput1Text.text = ipParts[1];
            ipInput2Text.text = ipParts[2];
            ipInput3Text.text = ipParts[3];
            ipInputPortText.text = ipParts[4];
        }
        else
        {
            Debug.Log("IP IS INVALID");
        }   
    }

    private bool IPRegexCheck(string str)
    {
        return regex.IsMatch(str);
    }

    public void HostBeginMatch()
    {
        transitionScript.LoadSceneByID(1);
    }

    public void JoinSearch()
    {
        string enteredIpAddress = ipInput0Text.text+'.'+ipInput1Text.text+'.'+ipInput2Text.text+'.'+ipInput3Text.text+':'+ipInputPortText.text;
        if(IPRegexCheck(enteredIpAddress))
        {
            //TODO: Wait for host's reply
            Debug.Log("ENTERED IP VALID");
            JoinBeginMatch();
        }
        else
        {
            Debug.Log("ENTERED IP INVALID");
            //TODO: Show error toast (invalid IP address entered)
        }
        
    }

    public void JoinBeginMatch()
    {
        transitionScript.LoadSceneByID(1);
    }
}