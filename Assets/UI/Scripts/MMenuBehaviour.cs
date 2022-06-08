using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
///
using Mirror;
using Mirror.Discovery;
using UnityEngine.Events;
using System.Net;
using System;
///
public class MMenuBehaviour : MonoBehaviour
{
    [SerializeField] private AudioSource ostSource;

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

    [Header("Extras screen related")]
    [SerializeField] private Text recordsText;
    private int gamesPlayed;
    private int wins;
    private int losses;
    [SerializeField] private GameObject rulesScreenGO;
    [SerializeField] private GameObject unitsGuideScreenGO;
    [SerializeField] private GameObject creditsScreenGO;
    private int currentScreen6Page;
    [SerializeField] private GameObject[] rulesPages;
    [SerializeField] private GameObject[] unitGuidePages;
    [SerializeField] private GameObject[] creditsPages;
    private GameObject[] currentPagesArray;
    [SerializeField] private Text pageNumberText;

    [Header("First time page related")]
    [SerializeField] private GameObject firstTimeScreenGO;
    [SerializeField] private Camera mainCamera;

    [Header("Settings page related")]
    [SerializeField] private Slider musicSliderGO;
    [SerializeField] private Slider sfxSliderGO;

    public SoundManager soundManager;

    /// 
    public struct HostMessage : NetworkMessage
    {
        public IPEndPoint EndPoint { get; set; }

        public Uri uri;

        // Prevent duplicate server appearance when a connection can be made via LAN on multiple NICs
        public long serverId;
        public string roomName;
    }
    public NetworkDiscovery networkDiscovery;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    ///
    
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
        soundManager = GetComponent<SoundManager>();

        if(PlayerPrefs.GetInt("FirstTimePlayer") == 0)
        {
            firstTimeScreenGO.SetActive(true);
            AudioListener.volume = 0;
        }
        else
        {
            firstTimeScreenGO.SetActive(false);
            //TODO: See if this needs changing when the sound volume will be linked to the playerprefs' values
            AudioListener.volume = 1;

            screen1Position = mMenuGO.transform.position;
            screen2Position = new Vector3(mMenuGO.transform.position.x, mMenuGO.transform.position.y + Screen.height, mMenuGO.transform.position.z);
            screen3Position = new Vector3(mMenuGO.transform.position.x + Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
            screen4Position = new Vector3(mMenuGO.transform.position.x - Screen.width, mMenuGO.transform.position.y, mMenuGO.transform.position.z);
            screen5Position = new Vector3(screen3Position.x + Screen.width, screen3Position.y, screen3Position.z);
            screen6Position = new Vector3(screen4Position.x - Screen.width, screen4Position.y, screen4Position.z);

            transitionScript = sceneTransitionManager.GetComponent<TransitionScript>();

            //Extra menu stats
            if (PlayerPrefsUtility.GetEncryptedInt("LastGameComplete") == 1)
            {
                PlayerPrefsUtility.SetEncryptedInt("Wins", PlayerPrefsUtility.GetEncryptedInt("Wins") + 1);
                PlayerPrefsUtility.SetEncryptedInt("LastGameComplete", 0);
            }

            wins = PlayerPrefsUtility.GetEncryptedInt("Wins");
            losses = PlayerPrefsUtility.GetEncryptedInt("Losses");
            gamesPlayed = wins + losses;
            if(gamesPlayed != 0)
                recordsText.text = "GAMES PLAYED: " + gamesPlayed + "\n\nWINS: " + wins + " (" + ((float)wins / (float)gamesPlayed * 100f).ToString("N1")
                + "%)      LOSSES: " + losses + " (" + ((float)losses / (float)gamesPlayed * 100f).ToString("N1") + "%)";
            else
                recordsText.text = "NO GAMES PLAYED YET!";

            //Setting sliders
            musicSliderGO.value = PlayerPrefs.GetFloat("MusicVolume");
            sfxSliderGO.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        //UnityEvent OnNewServer = networkDiscovery.OnServerFound
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        discoveredServers.Clear();
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
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
    public void GoToScreen5From3() { 
        StartCoroutine(MoveToScreenCoroutine(screen3Position, screen5Position));
        JoinSearch();
    }

    //Screen Join Menu -> Multiplayer Menu
    public void GoToScreen3From5() => StartCoroutine(MoveToScreenCoroutine(screen5Position, screen3Position));
    //Screen Guide -> Screen Extras
    public void GoToScreen4From6() => StartCoroutine(MoveToScreenCoroutine(screen6Position, screen4Position));
    //Screen Extras -> Screen Guide
    public void GoToScreen6From4(int screenId) => SwitchExtraScreen(screenId);

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

    public void ReloadSceneFirstTimeScreen()
    {
        PlayerPrefs.SetInt("FirstTimePlayer", 1);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void HostBeginMatch()
    {
        //Only shows transition without changing scene
        transitionScript.SceneTransitionPlay();

        networkDiscovery.globalRoomName = roomNameTextGO.text;
        if (string.IsNullOrEmpty(networkDiscovery.globalRoomName))
            networkDiscovery.globalRoomName = "GameRoom";
        /////
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
        /////

        //TODO: This is limited to 20 characters, look for the reason and increase it to 40
        PlayerPrefs.SetString("Room Name", networkDiscovery.globalRoomName);
    }

    public void JoinSearch()
    {
        ///////
        ///scrollViewContentGO.transform
        
        foreach (Transform child in scrollViewContentGO.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        JoinSpawn(); 
        ///////
    }

    public void JoinSpawn()
    {
        ////
        foreach (ServerResponse info in discoveredServers.Values)
        {
            //string roomName = info.EndPoint.Address.ToString();
            string roomName = info.name;
            CreateRoomButton(roomName, info);
        }
        /////
        //string roomName = "Lorem Ipsum dolor sit";
        //CreateRoomButton(roomName, new ServerResponse()); 
    }

    private void CreateRoomButton(string roomName, ServerResponse info)
    {
        GameObject roomButton = Instantiate(roomButtonPrefab);
        roomButton.transform.SetParent(scrollViewContentGO.transform);
        //TODO: This is a stupid fix, why these value are different from the ones in the prefab???
        roomButton.GetComponent<RectTransform>().localScale = Vector3.one;
        roomButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 492);
        roomButton.GetComponentInChildren<Text>().text = roomName;
        ///
        roomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinBeginMatch(info); });
        ///
    }

    public void JoinBeginMatch(ServerResponse info)
    {
        //TODO: Pass variables to game scene
        ///
        Connect(info);
        ///
        //transitionScript.LoadSceneByID(1); 
    }

    private void SwitchExtraScreen(int screenId)
    {
        currentScreen6Page = 0;
        switch (screenId)
        {
            case (int)Screen6IDs.RULES:
                currentPagesArray = rulesPages;
                foreach(GameObject page in currentPagesArray)
                    page.SetActive(false);
                rulesScreenGO.SetActive(true);
                unitsGuideScreenGO.SetActive(false);
                creditsScreenGO.SetActive(false);
                currentPagesArray[currentScreen6Page].SetActive(true);
                StartCoroutine(MoveToScreenCoroutine(screen4Position, screen6Position));
                break;
            case (int)Screen6IDs.UNITSGUIDE:
                currentPagesArray = unitGuidePages;
                foreach (GameObject page in currentPagesArray)
                    page.SetActive(false);
                rulesScreenGO.SetActive(false);
                unitsGuideScreenGO.SetActive(true);
                creditsScreenGO.SetActive(false);
                currentPagesArray[currentScreen6Page].SetActive(true);
                StartCoroutine(MoveToScreenCoroutine(screen4Position, screen6Position));
                break;
            case (int)Screen6IDs.CREDITS:
                currentPagesArray = creditsPages;
                foreach (GameObject page in currentPagesArray)
                    page.SetActive(false);
                rulesScreenGO.SetActive(false);
                unitsGuideScreenGO.SetActive(false);
                creditsScreenGO.SetActive(true);
                currentPagesArray[currentScreen6Page].SetActive(true);
                StartCoroutine(MoveToScreenCoroutine(screen4Position, screen6Position));
                break;
            default:
                break;
        };
        pageNumberText.text = "Page " + (currentScreen6Page + 1) + " / " + currentPagesArray.Length;
    }

    public void ChangePage(int offset)
    {
        soundManager.PlaySound(Sound.pageLeft);
        if(currentScreen6Page + offset >= 0 && currentScreen6Page + offset < currentPagesArray.Length)
        {
            currentPagesArray[currentScreen6Page].SetActive(false);
            currentPagesArray[currentScreen6Page += offset].SetActive(true);
            pageNumberText.text = "Page " + (currentScreen6Page + 1) + " / " + currentPagesArray.Length;
        }
    }

    public void UpdateMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        ostSource.volume = value;
    }

    public void UpdateSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void CallSoundManagerClick() {
        soundManager.PlaySound(Sound.click);
    }

    public void CallSoundManager(Sound sound) {
        soundManager.PlaySound(sound);
    }

    ///////////
    void Connect(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        if (!(discoveredServers.ContainsKey(info.serverId)))
        {
            discoveredServers[info.serverId] = info;
            JoinSpawn(); 
        }
        
    }

    
    ///////////
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