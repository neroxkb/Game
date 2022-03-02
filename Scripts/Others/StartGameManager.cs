using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Text;

public class StartGameManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
{
    public static StartGameManager instance;
    public GameObject MainMenuCanvas;
    public GameObject SettingsCanvas;
    public GameObject CreateOrJoinCanvas;
    public InputField NickNameInputFiled;
    public Text moneyText;
    public Text PingText;
    public bool ConnectToChina = true;
    // Start is called before the first frame update

  
    public Text InfoText;
    const string playerNamePrefKey = "PlayerName";
    const string playerMoneyPrefKey = "PlayerMoney";
    const string playerMoneyBoardKey = "UploadMoneyToBoard";
    const string playerMoneyStore = "PlayerMoneyStore";
    const string playerSteamId = "PlayerSteamId";
    const string FirstInGame = "FirstInGame";
    const string SubmitScore = "SubmitScore";

    private Texture2D downloadedAvatar;
    public RawImage image;
    public ulong steamId;
    private uint imageWidth;
    private uint imageHeight;
    public float PingCooldown = 1;
    private int steamAppId= 1758390;
    private AuthTicket hAuthTicket;

    private LoadBalancingClient lbc;
    public Text ConnectingStatus;
    public GameObject ConnectingCanvas;
    public GameObject ReConnectCanvas;
    public GameObject Light2Off;
    public GameObject Light2On;
    public GameObject Light3Off;
    public GameObject Light3On;
    public bool Connected = false;
    public bool SwitchingRegion = false;
    public string SwitchRegionCode = "";
    private void OnEnable()
    {
        //DontDestroyOnLoad(this);
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        try
        {
            SteamClient.Init((uint)steamAppId,true);
        }
        catch (System.Exception e)
        {
            // Couldn't init for some reason - it's one of these:
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
        }
    }
    void Start()
    {
        this.lbc = new LoadBalancingClient();
        this.lbc.AddCallbackTarget(this);
        //MainMenuCanvas.SetActive(true);
        //SettingsCanvas.SetActive(false);
        //CreateOrJoinCanvas.SetActive(false);
        
        
        //ConnectedToMaster();
        if (!PhotonNetwork.IsConnected)
        {
            if (ConnectToChina)
            {
                ConnectToMasterChina();
            }
            else
            {
                //ConnectInternationServer();
                ConnectedToMaster();
            }
            
        }
        else
        {
            InfoText.text = "Connect To Master Success.";
        }
        int money;

        //money = 10000;
        //PlayerPrefs.SetInt(playerMoneyPrefKey, money);
        //moneyText.text = moneyToString(money);

        //UseDefaultName();
        UseSteamName();
        
        LoadMoneyBoard();
        var t = getSteamAvatarAsync();
        
    }
    public void OnClick_AddMoney()
    {
        int money = -1;
        money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        money += 10000;
        moneyText.text = moneyToString(money);
        PlayerPrefs.SetInt(playerMoneyPrefKey, money);
    }
    public void OnClick_ResetMoney()
    {
        int money = -1;
        money = 10000;
        PlayerPrefs.SetInt(playerMoneyPrefKey, money);
        moneyText.text = moneyToString(money);
    }
    void addStoreMoney()
    {
        int storeMoney = PlayerPrefs.GetInt(playerMoneyStore);
        if (storeMoney > 0)
        {
            int money = -1;
            money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            money += storeMoney;
            TestGameManager.instance.moneyPrefText.text = "$" + moneyToString(money);
            PlayerPrefs.SetInt(playerMoneyPrefKey, money);
            PlayerPrefs.SetInt(playerMoneyStore, 0);
        }
    }
    void UseDefaultName()
    {
        string defaultName = string.Empty;
        if (NickNameInputFiled != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                NickNameInputFiled.text = defaultName;
                Debug.Log("Using Default Name:"+defaultName);
            }
        }
        PhotonNetwork.NickName = defaultName;
    }
    void UseSteamName()
    {
        if (Steamworks.SteamClient.IsLoggedOn)
        {
            string steamName = Steamworks.SteamClient.Name;
            //string steamLang = SteamApps.GameLanguage;
            string steamLang = SteamUtils.SteamUILanguage;
            //NickNameInputFiled.text = steamName;
            Debug.Log("Using Steam Name:" + steamName);
            Debug.Log("SteamUILanguage:" + steamLang);
            
            PhotonNetwork.NickName = steamName;

            Hashtable props = new Hashtable
            {
                {playerSteamId, SteamClient.SteamId.ToString()}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            Debug.Log("Setting steam ID:"+ SteamClient.SteamId.ToString());
            PlayerPrefs.SetString(playerNamePrefKey, steamName);
            UIManager.instance.SetName(steamName);
            SettingsManager.instance.SetLanguageBySteamLang(steamLang);
        }
        
    }
    
    async Task getSteamAvatarAsync()
    {
        var AvatarLarge = await SteamFriends.GetLargeAvatarAsync(Steamworks.SteamClient.SteamId);
        if (AvatarLarge.HasValue)
        {
            imageHeight = AvatarLarge.Value.Height;
            imageWidth = AvatarLarge.Value.Width;
            Debug.Log(imageWidth + "x" + imageHeight);
            downloadedAvatar = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false);
            downloadedAvatar.LoadRawTextureData(AvatarLarge.Value.Data);
            downloadedAvatar.Apply();

            image.texture = downloadedAvatar;
            return;
        }
        var AvatarMedium = await SteamFriends.GetMediumAvatarAsync(Steamworks.SteamClient.SteamId);
        if (AvatarMedium.HasValue)
        {
            imageHeight = AvatarMedium.Value.Height;
            imageWidth = AvatarMedium.Value.Width;
            Debug.Log(imageWidth + "x" + imageHeight);
            return;
        }
        var AvatarSmall = SteamFriends.GetSmallAvatarAsync(Steamworks.SteamClient.SteamId);
        
    }
    void LoadMoney()
    {
        int money = -1;
        System.DateTime pData = SteamApps.PurchaseTime(steamAppId);
        System.DateTime dData = new System.DateTime(2022,2,11);
        Debug.Log("PurchaseTime:" + pData.ToString() + "DeadLineTime:" + dData.ToString());
        if (pData.CompareTo(dData) == -1)
        {
            var t = MoneyBoardManager.instance.FirstUploadMoney(money, moneyText);
        }
        if (PlayerPrefs.HasKey(playerMoneyPrefKey))
        {
            Debug.Log("LoadMoney  playerMoneyPrefKey");
            money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            var t = MoneyBoardManager.instance.FirstUploadMoney(money, moneyText);
            PlayerPrefs.SetInt(playerMoneyBoardKey, 1);
            PlayerPrefs.DeleteKey(playerMoneyPrefKey);
            //moneyText.text = moneyToString(money);
        }
        else if (PlayerPrefs.HasKey(playerMoneyBoardKey))
        {
            Debug.Log("LoadMoney  playerMoneyBoardKey");
            var t = MoneyBoardManager.instance.SetUIMoney(moneyText);
        }
        else
        {
            Debug.Log("LoadMoney  First");
            money = 10000;
            var t = MoneyBoardManager.instance.FirstUploadMoney(money, moneyText);
            PlayerPrefs.SetInt(playerMoneyBoardKey, 1);
            //moneyText.text = moneyToString(money);
        }
    }
    void LoadMoneyBoard()
    {
        //var lm = MoneyBoardManager.instance.LoadMoney(moneyText);
        if (MoneyBoardManager.instance.MyCurrentMoney != -1)
        {
            moneyText.text = moneyToString(MoneyBoardManager.instance.MyCurrentMoney);
        }
    }
    void AddDLCMoney(int money)
    {

    }
    string moneyToString(int money)
    {
        string moneyStr = money.ToString();
        int len = moneyStr.Length;
        for (int i = 0; i < len; i++)
        {
            print(i + " " + (i + 1) % 3);
            if ((i + 1) % 3 == 0)
            {
                if ((len - i - 1) > 0 && moneyStr[0]!='-')
                {
                    moneyStr = moneyStr.Insert((len - i - 1), ",");
                    print(moneyStr);
                }
                if ((len - i - 1) > 1 && moneyStr[0] == '-')
                {
                    moneyStr = moneyStr.Insert((len - i - 1), ",");
                    print(moneyStr);
                }

            }
        }
        return moneyStr;
    }
    void ConnectedToMaster()
    {
        
        string steamAuthSessionTicket = GetSteamAuthTicket(out hAuthTicket);
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = SteamClient.SteamId.ToString();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Steam;
        PhotonNetwork.AuthValues.AddAuthParameter("ticket", steamAuthSessionTicket);

        /*if (SettingsManager.instance.GetCurrentLang() == "chinese")
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        }
        else
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
        }*/
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "f570b874-8abf-4359-8d30-1ffa09c55e53";
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "ns.photonengine.com";
        
        Debug.Log("Connecting To Master...");
        InfoText.text = "Connecting To Master...";
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
        
        
    }
    void ConnectedToMasterByRegion(string region)
    {

        string steamAuthSessionTicket = GetSteamAuthTicket(out hAuthTicket);
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = SteamClient.SteamId.ToString();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Steam;
        PhotonNetwork.AuthValues.AddAuthParameter("ticket", steamAuthSessionTicket);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "f570b874-8abf-4359-8d30-1ffa09c55e53";
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "ns.photonengine.com";

        Debug.Log("Connecting To Master...");
        InfoText.text = "Connecting To Master...";
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();


    }
    public void ConnectToRegion(string region)
    {
        SwitchingRegion = true;
        SwitchRegionCode = region;
        PhotonNetwork.Disconnect();
        ServerRegionManager.instance.OnClick_Close();
        
    }
    void DisconnectToServer()
    {
        PhotonNetwork.Disconnect();
    }
    void ConnectToMasterChina()
    {
        string steamAuthSessionTicket = GetSteamAuthTicket(out hAuthTicket);
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = SteamClient.SteamId.ToString();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Steam;
        PhotonNetwork.AuthValues.AddAuthParameter("ticket", steamAuthSessionTicket);
        
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "cn";
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "cbb81c42-77d0-4709-94cc-6f9100142710"; // 替换为您自己的国内区appID
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "ns.photonengine.cn";

        Debug.Log("Connecting To China Master...");
        InfoText.text = "Connecting To Master...";
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }
# region IConnectionCallbacks
    public void OnConnectedToMaster()
    {
        Debug.Log("Connect To Master Success.");
        InfoText.text = "Connect To Master Success.";
        
        PhotonNetwork.JoinLobby();
    }
    public void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby Success.");
    }
    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Connect To Master Failed. Because: " + cause);
        InfoText.text = "Connect To Master Failed.";
        //SceneManager.LoadScene("StartScene");
    }
    public void OnConnected()
    {
        Debug.Log("OnConnected.");
    }
    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived");
        regionHandler.PingMinimumOfRegions(this.OnRegionPingCompleted, null);
        //this.regionHandler = regionHandler;
    }
    private void OnRegionPingCompleted(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionPingCompleted");
        int countRegion = regionHandler.EnabledRegions.Count;
        
    }
    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnLeftLobby()
    {
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnCreatedRoom()
    {
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinedRoom()
    {
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
    }

    public void OnLeftRoom()
    {
    }
    #endregion
    private string GetSteamAuthTicket(out AuthTicket ticket)
    {
        ticket = SteamUser.GetAuthSessionTicket();
        StringBuilder ticketString = new StringBuilder();
        for (int i = 0; i < ticket.Data.Length; i++)
        {
            ticketString.AppendFormat("{0:x2}", ticket.Data[i]);
        }
        return ticketString.ToString();
    }

    private void CancelAuthTicket(AuthTicket ticket)
    {
        if (ticket != null)
        {
            ticket.Cancel();
        }
    }
    public void ShowPing()
    {
        if (PingCooldown > 0)
        {
            PingCooldown -= Time.deltaTime;
            //print(PingCooldown);
        } 
        else
        {
            //PingText.text =PhotonNetwork.CloudRegion+ ":" + PhotonNetwork.GetPing() + "ms";
            PingText.text = PhotonNetwork.GetPing() + "ms";
            PingCooldown = 1f;
        }
    }
    public void OnClick_Reconnect()
    {
        ReConnectCanvas.SetActive(false);
        Connected = false;
        ResetConnectLights();
        ConnectedToMaster();
    }
    public void ResetConnectLights()
    {
        Light2Off.SetActive(true);
        Light2On.SetActive(false);
        Light3Off.SetActive(true);
        Light3On.SetActive(false);
    }

    public void OnClick_Disconnect()
    {
        DisconnectToServer();
    }
    public void ConnectingUpdate()
    {
        //Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        if (PhotonNetwork.NetworkClientState.ToString().Equals("Disconnected"))
        {
            if (SwitchingRegion)
            {
                //ConnectedToMasterByRegion(SwitchRegionCode);
                PhotonNetwork.ConnectToRegion(SwitchRegionCode);
                Connected = false;
                ResetConnectLights();

            }
            else
            {
                ReConnectCanvas.SetActive(true);
                WaitingManager.instance.CloseWaitingCanvas();
                ConnectingStatus.text = PhotonNetwork.NetworkClientState.ToString();
            }
            //Connected = true;
        }
        if (Connected)
        {
            return;
        }
        ConnectingCanvas.SetActive(true);
        ConnectingStatus.text = PhotonNetwork.NetworkClientState.ToString();
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        if (ConnectingStatus.text.Equals("Authenticating"))
        {
            Light2Off.SetActive(false);
            Light2On.SetActive(true);
        }
        if (ConnectingStatus.text.Equals("ConnectedToMasterServer"))
        {
            Light2Off.SetActive(false);
            Light2On.SetActive(true);
            Light3Off.SetActive(false);
            Light3On.SetActive(true);
            SwitchingRegion = false;
            ServerRegionManager.instance.SetButtonUI();
        }
        if (PhotonNetwork.NetworkClientState.ToString().Equals("ConnectedToMasterServer"))
        {
            
            ConnectingCanvas.SetActive(false);
            FirstLoadGame();
            Connected = true;
            
        }
        
    }
    public void FirstLoadGame()
    {
        if (!PlayerPrefs.HasKey(FirstInGame))
        {
            PlayerPrefs.SetInt(FirstInGame, 1);
            GuideManager.instance.ShowGuideCancas();
            
        }
        /*if (!PlayerPrefs.HasKey(SubmitScore))
        {
            PlayerPrefs.SetInt(SubmitScore, 1);
            var fslb = LeaderBoardManager.instance.FirstSubmitScoreAsync();
        }*/
        
    }
    
    private void Update()
    {
        SteamClient.RunCallbacks();
        ConnectingUpdate();
        ShowPing();
        //Debug.Log(PhotonNetwork.NetworkClientState);
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }

}
