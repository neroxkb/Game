using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class UnderCoverUIManager : MonoBehaviourPunCallbacks
{
    public static UnderCoverUIManager instance;

    public int StartGameCountDownTime = 3; 
    public int StartGameStartTime;
    public bool StartGameTimeRunning = false;

    public Slider StatementCountDownSlider;
    public int StatementCountDownTime = 60;
    public int StatementStartTime;
    public bool StatementTimeRunning = false;

    public Slider VoteCountDownSlider;
    public int VoteCountDownTime = 60;
    public int VoteStartTime;
    public bool VoteTimeRunning = false;

    public Canvas PlayerInputCanvas;
    public Canvas CountDownTimeCanvas;
    public Canvas EndCanvas;
    public Canvas WaitingCanvas;
    public Canvas NotEnoughPlayerCanvas;


    public InputField PlayerInputField;
    public Text countDownText;
    public Text infoText;
    public Text PlayerMoneyText;
    public Text MyWordText;
    public Text OtherWordText;
    public Text BonusMoneyText;
    public RawImage Win;
    public RawImage Lose;

    public AudioSource StatementRoundAudio;
    public AudioSource VoteRoundAudio;

    public GameObject RoundShowPanel;
    public Text RoundShowText;

    public GameObject UnderCoverPlayerPrefab;
    public GameObject PlayerList;
    public List<GameObject> RoomPlayers;
    const string playerSteamId = "PlayerSteamId";
    const string playerMoneyPrefKey = "PlayerMoney";
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        AddMoney(0);
    }

    // Update is called once per frame
    public void SetInfoText(string info)
    {
        this.infoText.text = info;
    }
    public void RemoveAllPlayer()
    {
        int PlayerNum = PlayerList.transform.childCount;
        for (int i = PlayerNum - 1; i >= 0; i--)
        {
            Destroy(PlayerList.transform.GetChild(i).gameObject);
        }

    }
    public void SetPlayers(int UnderCoverActorNumber,string UnderCoverWord, string CivilianWord)
    {
        Debug.Log("Setting Player Displays");
        SetInfoText("Setting Player Displays");
        RemoveAllPlayer();
        RoomPlayers = new List<GameObject>();
        if (PhotonNetwork.LocalPlayer.ActorNumber == UnderCoverActorNumber)
        {
            UnderCoverPlayerManager.instance.IsUnderCover = true;
            MyWordText.text =  UnderCoverWord;
            OtherWordText.text = CivilianWord;
        }
        else
        {
            UnderCoverPlayerManager.instance.IsUnderCover = false;
            MyWordText.text = CivilianWord;
            OtherWordText.text = UnderCoverWord;
        }
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object steamId;

            if (p.CustomProperties.TryGetValue(playerSteamId, out steamId))
            {
                Debug.Log("Setting Player Displays  "+p.ActorNumber);
                ulong steamIdValue = Convert.ToUInt64((string)steamId);
                SteamId steamID = steamIdValue;
                GameObject roomPlayer = Instantiate(UnderCoverPlayerPrefab, PlayerList.transform);
                Debug.Log(steamID);
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().init(steamID, p.ActorNumber, UnderCoverActorNumber);
                roomPlayer.transform.localScale = new Vector3(1f, 1f, 1);
                RoomPlayers.Add(roomPlayer);

            }
        }

        Hashtable props = new Hashtable
            {
                {UnderCoverGameManager.instance.PLAYER_READY, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void SetStatement(int ActorNumber,string Statement)
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ActorNumber == ActorNumber)
            {
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().SetStatement(Statement);
            }
        }
    }
    public void PlayerMakeStatement(int startTimestamp)
    {
        PlayerInputCanvas.gameObject.SetActive(true);
        
        StatementCountDown(startTimestamp);
    }

    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        Debug.Log("OnLeftRoom");
        SceneManager.LoadScene("StartScene");
    }
    public void AddMoney(int addMoney)
    {
        /*int money = -1;
        if (PlayerPrefs.HasKey(playerMoneyPrefKey))
        {
            money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            money += addMoney;
            PlayerPrefs.SetInt(playerMoneyPrefKey, money);
            PlayerMoneyText.text = moneyToString(money);
        }*/
        //var mm = MoneyBoardManager.instance.ModifyMoney(addMoney, PlayerMoneyText);
        MoneyBoardManager.instance.ModifyMoney(addMoney, PlayerMoneyText);

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
                if ((len - i - 1) > 0 && moneyStr[0] != '-')
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

    public void CloseWaitingCanvas()
    {
        WaitingCanvas.gameObject.SetActive(false);
    }
    public void ShowNotEnoughPlayerCanvas()
    {
        NotEnoughPlayerCanvas.gameObject.SetActive(true);
    }
    #region StartGameCountDown
    public void StartGameCountDown(int startTime)
    {
        CloseWaitingCanvas();
        StartGameStartTime = startTime;
        StartGameTimeRunning = true;
        CountDownTimeCanvas.gameObject.SetActive(true);
    }

    private float StartGameTimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.StartGameStartTime;
        return this.StartGameCountDownTime - timer / 1000f;
    }
    private void OnStartGameTimerEnds()
    {
        this.StartGameTimeRunning = false;

        this.countDownText.text = string.Empty;
        CountDownTimeCanvas.gameObject.SetActive(false);
        Debug.Log("Count down end.");

        BeginStatementRound();
        //UnderCoverGameManager.instance.StatementRound();

    }
    public void StartGameCountDownUpdate()
    {
        if (!this.StartGameTimeRunning) return;

        float countdown = StartGameTimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        this.countDownText.text = string.Format("{0}", countdown.ToString("n0"));
        if (countdown > 0.0f) return;
        OnStartGameTimerEnds();
    }
    #endregion
    #region StatementCountDown
    public void BeginStatementRound()
    {
        if (!UnderCoverPlayerManager.instance.IsDead)
        {
            Hashtable props = new Hashtable
            {
                {UnderCoverGameManager.instance.PLAYER_VOTE_OVER, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        CloseVotePanel();
        StartCoroutine(ShowStatementRound());
    }
    public IEnumerator ShowStatementRound()
    {
        StatementRoundAudio.Play();
        RoundShowPanel.SetActive(true);
        RoundShowText.text = "Statement Round";
        yield return new WaitForSeconds(1.5f);
        RoundShowPanel.SetActive(false);
        RoundShowText.text = "";

        UnderCoverGameManager.instance.StatementRound();

    }
    public void StatementCountDown(int startTime)
    {
        StatementStartTime = startTime;
        StatementCountDownSlider.value = 1;
        StatementTimeRunning = true;
    }
    public void StopStatementCountDown()
    {
        StatementCountDownSlider.value = 1;
        StatementTimeRunning = false;
    }
    private float StatementTimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.StatementStartTime;
        return this.StatementCountDownTime - timer / 1000f;
    }
    private void OnStatementTimerEnds()
    {
        this.StatementTimeRunning = false;
        OnClick_LeaveRoom();
    }
    public void StatementCountDownUpdate()
    {
        if (!this.StatementTimeRunning) return;

        float countdown = StatementTimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        StatementCountDownSlider.value = countdown / StatementCountDownTime;
        if (countdown > 0.0f) return;
        OnStatementTimerEnds();
    }
    #endregion
    #region PlayerStatementInput
    public void OnClick_Confirm()
    {
        if (PlayerInputField.text != string.Empty)
        {
            string statement = PlayerInputField.text;
            object[] datas = new object[] {PhotonNetwork.LocalPlayer.ActorNumber,statement };
            PhotonNetwork.RaiseEvent(UnderCoverGameManager.SET_STATEMENT_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
            SetStatement(PhotonNetwork.LocalPlayer.ActorNumber, statement);
            PlayerInputCanvas.gameObject.SetActive(false);
            PlayerInputField.text = string.Empty;
            StopStatementCountDown();
            UnderCoverGameManager.instance.NextPlayerStatement();
        }
    }
    #endregion
    #region PlayerVote
    public void BeginVoteRound(int startTimestamp)
    {
        StartCoroutine(ShowVoteRound(startTimestamp));
    }
    public IEnumerator ShowVoteRound(int startTimestamp)
    {
        VoteRoundAudio.Play();
        RoundShowPanel.SetActive(true);
        RoundShowText.text = "Vote Round";
        yield return new WaitForSeconds(1.5f);
        RoundShowPanel.SetActive(false);
        RoundShowText.text = "";
        VoteCountDownSlider.gameObject.SetActive(true);
        VoteCountDown(startTimestamp + 1500);
    }
    public void VoteCountDown(int startTime)
    {
        ShowVoteUI();
        VoteStartTime = startTime;
        VoteCountDownSlider.value = 1;
        VoteTimeRunning = true;
        if (UnderCoverPlayerManager.instance.IsDead)
        {
            StopVoteCountDown();
        }

    }
    public void StopVoteCountDown()
    {
        VoteCountDownSlider.value = 1;
        VoteTimeRunning = false;
        VoteCountDownSlider.gameObject.SetActive(false);
    }
    private float VoteTimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.VoteStartTime;
        return this.VoteCountDownTime - timer / 1000f;
    }
    private void OnVoteTimerEnds()
    {
        this.VoteTimeRunning = false;
        OnClick_LeaveRoom();
    }
    public void VoteCountDownUpdate()
    {
        if (!this.VoteTimeRunning) return;

        float countdown = VoteTimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        VoteCountDownSlider.value = countdown / VoteCountDownTime;
        if (countdown > 0.0f) return;
        OnVoteTimerEnds();
    }
    public void ShowVoteUI()
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ShowVotePanel();
            if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber
                && !roomPlayer.GetComponent<UnderCoverPlayerPrefab>().IsDead)
            {
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ShowVoteButton();
            }
            else
            {
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().CloseVoteButton();
            }
            if (UnderCoverPlayerManager.instance.IsDead)
            {
                CloseVoteButton();
            }
        }
    }
    public void SetVote(int ActorNumber)
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ActorNumber == ActorNumber)
            {
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().AddVote();
            }
            
        }
    }
    public void CloseVoteButton()
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            roomPlayer.GetComponent<UnderCoverPlayerPrefab>().CloseVoteButton();
        }
    }
    public void CloseVotePanel()
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            roomPlayer.GetComponent<UnderCoverPlayerPrefab>().CloseVotePanel();
        }
    }
    public void VoteOver()
    {
        Debug.Log("UIManager VoteOver");
        int maxVote = 0;
        int maxVoteActorNumber=-1;
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().VoteNums > maxVote)
            {
                maxVote = roomPlayer.GetComponent<UnderCoverPlayerPrefab>().VoteNums;
                maxVoteActorNumber = roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ActorNumber;
            }
        }
        if (maxVoteActorNumber == -1)
        {
            SetInfoText("Something wrong");
        }
        SetInfoText("maxVoteActorNumber is "+maxVoteActorNumber);
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ActorNumber == maxVoteActorNumber)
            {
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().Dead();
            }
        }
        if (maxVoteActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            UnderCoverPlayerManager.instance.IsDead = true;
        }

    }
    #endregion
    #region Check
    public bool CheckUnderCoverWin()
    {
        int AliveNum = 0;

        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (!roomPlayer.GetComponent<UnderCoverPlayerPrefab>().IsDead)
            {
                AliveNum += 1;
            }
        }
        if (AliveNum == 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CheckPlayerDead(int ActorNumber)
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().ActorNumber==ActorNumber)
            {
                if (roomPlayer.GetComponent<UnderCoverPlayerPrefab>().IsDead)
                {
                    return true; 
                }
            }
        }
        return false;
    }
    #endregion
    public void ShowAllIdentity()
    {
        foreach (GameObject roomPlayer in RoomPlayers)
        {
            if (!roomPlayer.GetComponent<UnderCoverPlayerPrefab>().IsDead)
            {
                roomPlayer.GetComponent<UnderCoverPlayerPrefab>().GameOverShowIdentity();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        StartGameCountDownUpdate();
        StatementCountDownUpdate();
        VoteCountDownUpdate();
    }
}
