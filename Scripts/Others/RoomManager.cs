using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Steamworks;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject InfoCanvas;
    public GameObject RoomCanvas;
    public GameObject PlayerListCanvas;
    public GameObject StartGameButton;
    public PlayerDisplay playerDisplay;
    public GameObject playerHighLight;
    public bool ConnectToChina=true;
    public Text MinPlayerNum;
    public Text NeedPlayers;
    public float MaxCreatingTime = 30;
    public float CreatingTime = 0;
    //Matching
    public GameObject MatchingPanel;
    public bool Matching = false;
    public int MatchingGameCode=-1;
    public string MatchingGameLanuage;
    public byte MatchingExpectedMaxPlayers ;
    public float MatchingTime;
    public Text MatchingText;
    public Text NoRoomText;
    public Slider MatchingSlider;
    public int MaxMatchingTime;
    public Dropdown MatchingTimeDropDown;
    public List<int> MatchingTimes;
    // Start is called before the first frame update
    private string roomId;
    private bool isConnecting;
    private bool creatingRoom;
    private bool createRoom;
    private Text InfoText;
    private Text RoomId;
    public Text TagSelectText;
    public GameObject TagDropdown;

    //private List<Player> playerList=new List<Player>();

    public static RoomManager instance;
    private Vector3[] positionList;
    private bool[] positionIsEmpty;
    private Player[] playerList;
    private PlayerDisplay[] playerDisplayList;

    public int maxPlayer = 4;
    public Vector3 chatterPosition = new Vector3(0, 0, 0);

    const string playerSteamId = "PlayerSteamId";
    public GameObject RoomPlayerPrefab;
    public GameObject RoomPlayerList;

    const string GameType = "GameType";
    const string GameLanguage = "GameLanguage";

    public bool findingChatter=false;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        instance = this;
    }
    void Start()
    {
        
        InfoText = InfoCanvas.GetComponentInChildren<Text>();
        RoomCanvas.SetActive(false);
        RoomId=RoomCanvas.GetComponentInChildren<Text>();

    }
    private void Update()
    {
        MatchUpdate();
        CreateRoomUpdate();
        if (findingChatter)
        {
            Debug.Log("finding");
            GameObject chatter = GameObject.Find("Chatter(Clone)");
            if (chatter)
            {
                Chatter c = chatter.GetComponent<Chatter>();
                c.OnClick_ShowChatter();
                findingChatter = false;
            }
            
        }

    }
    
    #region CreateRoom
    public void CreateRoom(bool isPublic,int GameCode, byte expectedMaxPlayers ,string lang)
    {
        Debug.Log("Creating Room...");
        InfoText.text = "Creating Room...";
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            return;
        }
        creatingRoom = true;
        WaitingManager.instance.ShowWaitingCanvas(1);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)expectedMaxPlayers;
        roomOptions.IsVisible = isPublic;

        string[] propertiesForLobby= { GameType,GameLanguage };
        roomOptions.CustomRoomPropertiesForLobby = propertiesForLobby;
        roomOptions.CustomRoomProperties = new Hashtable { { GameType, GameCode }, { GameLanguage, lang } };


        int RoomID = UnityEngine.Random.Range(0, 1000000);
        roomId = RoomID.ToString();
        
        PhotonNetwork.CreateRoom(roomId,roomOptions,TypedLobby.Default);
        
    }
    public override void OnCreatedRoom()
    {
        object RoomGameType;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameType, out RoomGameType))
        {
            Debug.Log(RoomGameType);
            if ((int)RoomGameType != UIManager.instance.CurrentGameIndex || !UIManager.instance.TapeConfirm)
            {
                PhotonNetwork.LeaveRoom();
                return;
            }
        }
        Debug.Log("Create Room Successful.Room ID :" + roomId + "  Current rooms:" + PhotonNetwork.CountOfRooms);
        WaitingManager.instance.CloseWaitingCanvas();
        MatchingEnd();
        InfoText.text = "Create Room Successful.Room ID :" + roomId+"    "+PhotonNetwork.CurrentRoom.IsVisible;
        RoomId.text = "ID :" + roomId;
        createRoom = true;
        creatingRoom = false;
        //InfoCanvas.SetActive(false);
        UIManager.instance.GameCanvas.gameObject.SetActive(false);
        UIManager.instance.RoomCanvas.gameObject.SetActive(true);

        //SetPlayers();
        GameObject chatter = PhotonNetwork.InstantiateRoomObject("Chatter", chatterPosition, Quaternion.identity);
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed.");
        InfoText.text = "Create Room Failed.";
        creatingRoom = false;
        WaitingManager.instance.CloseWaitingCanvas();
    }
    #endregion
    #region JoinRoom
    public void JoinRoom(string roomToJoin)
    {
        ;
        Debug.Log("Joining Room...Current rooms:"+PhotonNetwork.CountOfRooms);
        InfoText.text = "Joining Room...";
        PhotonNetwork.JoinRoom(roomToJoin);
        Debug.Log("Joining Other's Room Success:"+ roomToJoin);
        WaitingManager.instance.ShowWaitingCanvas(2);
        creatingRoom = false;

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed."+message);
        WaitingManager.instance.CloseWaitingCanvas();
        creatingRoom = false;
    }
    public override void OnJoinedRoom()
    {
        object RoomGameType;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameType, out RoomGameType))
        {
            Debug.Log(RoomGameType);
            if ((int)RoomGameType != UIManager.instance.CurrentGameIndex || !UIManager.instance.TapeConfirm)
            {
                PhotonNetwork.LeaveRoom();
                return;
            }
        }
        Debug.Log("Join Room Success.");
        creatingRoom = false;
        WaitingManager.instance.CloseWaitingCanvas();
        MatchingEnd();
        RoomId.text = "ID :" + PhotonNetwork.CurrentRoom.Name;
        SetPlayers();
        UIManager.instance.GameCanvas.gameObject.SetActive(false);
        UIManager.instance.RoomCanvas.gameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(true);
            if (UIManager.instance.CurrentGameIndex == 1)
            {
                //TagSelectText.gameObject.SetActive(true);
                //TagDropdown.SetActive(true);
            }
        }
        else
        {
            StartGameButton.SetActive(false);
            TagSelectText.gameObject.SetActive(false);
            TagDropdown.SetActive(false);
        }
        if (UIManager.instance.CurrentGameIndex == 0)
        {
            MinPlayerNum.text = ""+2;
        }
        else if (UIManager.instance.CurrentGameIndex == 1)
        {
            MinPlayerNum.text = "" + 1;
        }
        else if (UIManager.instance.CurrentGameIndex == 2)
        {
            MinPlayerNum.text = "" + 3;
        }

        //UIForStartScene.instance.JoinRoomCanvas.SetActive(false);
        findingChatter = true;
    }
    #endregion
    #region LeaveRoom
    public void LeaveRoom()
    {
        creatingRoom = false;
        Debug.Log("Leaving Room...");
        InfoText.text = "Leaving Room...";
        PhotonNetwork.LeaveRoom();
        
        WaitingManager.instance.CloseWaitingCanvas();
    }

    public override void OnLeftRoom()
    {
        creatingRoom = false;
        Debug.Log("Left Room.");
        //RemovePlayer(PhotonNetwork.LocalPlayer);
        TagSelectText.gameObject.SetActive(false);
        TagDropdown.SetActive(false);
        UIManager.instance.RoomCanvas.gameObject.SetActive(false);
        if (UIManager.instance.TapeConfirm)
        {
            UIManager.instance.GameCanvas.gameObject.SetActive(true);
        }
        WaitingManager.instance.CloseWaitingCanvas();
        RemoveAllPlayer();

    }
    public void DestroyAllObject()
    {
        PhotonNetwork.DestroyAll();
    }

    #endregion
    #region Match
    
    public void Match(int GameCode,byte expectedMaxPlayers,string GameLang)
    {
        creatingRoom = false;
        WaitingManager.instance.CloseWaitingCanvas();
        MatchingGameCode = GameCode;
        MatchingGameLanuage = GameLang;
        MatchingExpectedMaxPlayers = expectedMaxPlayers;
        MatchingPanel.SetActive(true);
        MatchingText.gameObject.SetActive(true);
        NoRoomText.gameObject.SetActive(false);
        int index = MatchingTimeDropDown.value;
        MaxMatchingTime = MatchingTimes[index];
        MatchingSlider.value = 1;
        Matching = true;
        //Hashtable expectedCustomRoomProperties = new Hashtable { { GameType, GameCode } };
        //PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
    }
    public void MatchUpdate()
    {
        if (!Matching) return;
        Hashtable expectedCustomRoomProperties = new Hashtable { { GameType, MatchingGameCode }, { GameLanguage, MatchingGameLanuage } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, MatchingExpectedMaxPlayers);
        MatchingTime += Time.deltaTime;
        MatchingSlider.value = 1 - MatchingTime/ MaxMatchingTime;
        if (MatchingTime > MaxMatchingTime)
        {
            //CreateRoom(true, MatchingGameCode, MatchingExpectedMaxPlayers);
            //MatchingEnd();
            MatchingText.gameObject.SetActive(false);
            NoRoomText.gameObject.SetActive(true);
            Matching = false;

        }

    }
    public void CreateRoomUpdate()
    {
        if (!creatingRoom) return;

        CreatingTime += Time.deltaTime;
        //Debug.Log(CreatingTime);
        if (CreatingTime > MaxCreatingTime)
        {
            //CreateRoom(true, MatchingGameCode, MatchingExpectedMaxPlayers);
            //MatchingEnd();

            WaitingManager.instance.CloseWaitingCanvas();
            CreatingTime = 0;

        }

    }
    public void OnClick_CancelMatching()
    {
        MatchingEnd();
    }
    public void MatchingEnd()
    {
        WaitingManager.instance.CloseWaitingCanvas();
        Matching = false;
        MatchingPanel.SetActive(false);
        MatchingTime = 0;
    }
    #endregion
    #region RoomPlayer
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player " + newPlayer.NickName + " Entered Room.");

        AddPlayer(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        RemovePlayerByActorNumber(otherPlayer);
        Debug.Log("Player " + otherPlayer.NickName + " Left Room.");
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(true);
            if (UIManager.instance.CurrentGameIndex == 1)
            {
                //TagSelectText.gameObject.SetActive(true);
                //TagDropdown.SetActive(true);
            }
        }

    }
    public void SetPlayers()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object steamId;

            if (p.CustomProperties.TryGetValue(playerSteamId, out steamId))
            {
                ulong steamIdValue = Convert.ToUInt64((string)steamId);
                SteamId steamID = steamIdValue;
                GameObject roomPlayer = Instantiate(RoomPlayerPrefab, RoomPlayerList.transform);
                Debug.Log(steamID);
                roomPlayer.GetComponent<RoomPlayerDisplay>().init(steamID, p.ActorNumber);
                /*if (UIManager.instance.CurrentGameIndex == 0)
                {
                    roomPlayer.transform.localScale = new Vector3(1.6f, 1.6f, 1);
                }
                else
                {
                    roomPlayer.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                }*/

            }
        }
    }
    public void AddPlayer(Player player)
    {
        object steamId;

        if (player.CustomProperties.TryGetValue(playerSteamId, out steamId))
        {
            ulong steamIdValue = Convert.ToUInt64((string)steamId);
            SteamId steamID = steamIdValue;
            GameObject roomPlayer = Instantiate(RoomPlayerPrefab, RoomPlayerList.transform);
            Debug.Log(steamID);
            roomPlayer.GetComponent<RoomPlayerDisplay>().init(steamID, player.ActorNumber);
            /*if (UIManager.instance.CurrentGameIndex == 0)
            {
                roomPlayer.transform.localScale = new Vector3(1.6f, 1.6f, 1);
            }
            else
            {
                roomPlayer.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            }*/
            

        }

    }
    public void RemovePlayerBySteamID(Player player)
    {
        int PlayerNum = RoomPlayerList.transform.childCount;

        object steamId;

        if (player.CustomProperties.TryGetValue(playerSteamId, out steamId))
        {
            Debug.Log("Remove PlayerID" + steamId.ToString());
            for (int i = PlayerNum - 1; i >= 0; i--)
            {
                GameObject roomPlayer = RoomPlayerList.transform.GetChild(i).gameObject;
                SteamId roomPlayerId = roomPlayer.GetComponent<AnswerGamePlayerPrefab>().steamId;
                Debug.Log("roomPlayerId" + roomPlayerId.ToString());
                if (steamId.Equals(roomPlayerId))
                {
                    Destroy(RoomPlayerList.transform.GetChild(i).gameObject);
                }
            }
        }
    }
    public void RemovePlayerByActorNumber(Player player)
    {
        int PlayerNum = RoomPlayerList.transform.childCount;


        Debug.Log("Remove PlayerID" + player.ActorNumber);
        for (int i = 0; i< PlayerNum; i++)
        {
            GameObject roomPlayer = RoomPlayerList.transform.GetChild(i).gameObject;
            int roomPlayerActorNumber = roomPlayer.GetComponent<RoomPlayerDisplay>().ActorNumber;
            Debug.Log("roomPlayerId" + roomPlayerActorNumber);
            if (player.ActorNumber== roomPlayerActorNumber)
            {
                Destroy(RoomPlayerList.transform.GetChild(i).gameObject);
            }
        }
        
    }
    public void RemoveAllPlayer()
    {
        int PlayerNum = RoomPlayerList.transform.childCount;
        for (int i = PlayerNum-1; i >= 0; i--)
        {
            Destroy(RoomPlayerList.transform.GetChild(i).gameObject);
        }
        
    }
    #endregion

    public void StartGame(int SenceCode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (UIManager.instance.CurrentGameIndex == 0 && PhotonNetwork.PlayerList.Length < 2)
            {
                return;
            }

            if (UIManager.instance.CurrentGameIndex == 2 && PhotonNetwork.PlayerList.Length < 3)
            {
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(SenceCode);
        }

    }



    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master Client Switched ");
        //UIForStartScene.instance.OnClick_LeaveRoom();
    }


    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(1);
        }

    }
    public void OnClick_StartGameManDown100()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(2);
        }

    }
    public void OnClick_StartGameAnswer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(3);
        }

    }
    /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player enter room");
    }*/
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
       
        Debug.Log("Current Rooms:" + roomList.Count);
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                Debug.Log("Remove Room:"+info.Name);
                //Debug.Log("OnRoomListUpdate:" + UIForStartScene.instance.MainMenuCanvas.active);
            }
            else Debug.Log(info.Name);
        }
        
    }
    

}
