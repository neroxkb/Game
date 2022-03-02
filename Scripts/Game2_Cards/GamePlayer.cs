using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class GamePlayer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const string PLAYER_MONEY = "PlayerMoney";
    public const string PLAYER_STATE = "PlayerState";
    public const string ROOM_CALL_NUM = "RoomCallNum";
    public const string ROOM_BET_NUM = "RoomBetNum";
    const string playerMoneyPrefKey = "PlayerMoney";
    const string playerMoneyStore = "PlayerMoneyStore";
    const string playerSteamId = "PlayerSteamId";

    private Texture2D DownloadedAvatar;
    public RawImage SteamAvatar;
    public Image CountdownImage;

    public PhotonView photonView;
    public Vector3[] playerPositionList = new Vector3[4];
    public bool[] positionEmptyList = new bool[4];
    public GamePlayer[] gamePlayerList = new GamePlayer[4];

    public static GamePlayer instance;

 
    public GameObject Cards;
    public GameObject CardBackPosition;
    public GameObject CardBackObject;
    public GameObject BetCanvas;
    public GameObject FirstChoiceCanvas;
    public GameObject FollowChoiceCanvas;
    public GameObject FoldOrSoHaCanvas;
    public GameObject Choice_BetCanvas;
    public GameObject Choice_RaiseCanvas;
    public GameObject Choice_AnyBetCanvas;
    public GameObject Choice_AnyRaiseCanvas;
    public GameObject WinStatePanel;
    public GameObject FoldStatePanel;
    public Slider AnyBetSlider;
    public Slider AlreadyBetSlider;
    public Slider CallSlider;
    public Slider AnyRaiseSlider;
    public Text PlayerNameText;
    public Text InfoText;
    public Text MoneyAndBetText;
    public Text BetNumInput;
    public Text StateDisplay;
    public Text cardsTypeText;
    public Text AnyBetNumText;
    public Text AlreadyBetNumText;
    public Text AnyRaiseNumText;
    public Text MaxMoneyText;
    public Text CallNumText;
    public Text BetDisplayText;
    public bool followChoice = false;
    public bool raiseBet = false;
    public int State = TwentyOneEventCode.NEW_INCOME;

    public List<Card> cardList = new List<Card>();
    public List<Card> seenCardList= new List<Card>();
    public int cardType=-1;
    public int seenCardType=-1;

    public int moneyForOneGame = 200000;
    public int allMoney = 200000;
    public int betNum = 0;
    public int newBetNum = 0;
    public int roomCallNum = 0;
    public int roomBetNum = 0;
    public int myScore = 0;

    public float offsetBetweenCards = 200f;

    public int choiceCountDownTime =60;
    public int choiceStartTime;
    public bool countDownTimeRunning = false;
    public GameObject CountDownTimeCanvas;
    public Text countDownText;

    #region cardAnmi
    public bool deliverCardIng = false;
    public GameObject CardAnmiObject;
    public Vector3 CardAnmiTargetPosition;
    public Card CardAnmiNewCard;
    #endregion
    public AudioSource DeliverAudio;
    public AudioSource BadCardAudio;
    public AudioSource WinAudio;
    public AudioSource LoseAudio;
    public AudioSource SoHaAudio;
    public AudioSource YourTurnAudio;
    public const string CountdownStartTime = "PlayerChoiceTime";
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        instance = this;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        PlayerChoiceTimer.OnCountdownTimerEnd += ChoiceTimeEnd;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PlayerChoiceTimer.OnCountdownTimerEnd -= ChoiceTimeEnd;
    }
    void Start()
    {
        moneyForOneGame = TestGameManager.instance.MoneyForOneGame;
        if (moneyForOneGame < 0)
        {
            Debug.LogError("money wrong.");
        }
        allMoney = moneyForOneGame;
        initPlayerPrefMoney();
        setPlayersPosition();
        //photonView.RPC("InitPlayer", RpcTarget.AllViaServer);
        InitPlayer();
        photonView.RPC("setNameAndAvatar", RpcTarget.AllViaServer);

    }
    void initPlayerPrefMoney()
    {
        if (photonView.IsMine)
        {
            //int money = -1;
            //money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            //money -= moneyForOneGame;
            //TestGameManager.instance.moneyPrefText.text =moneyToString(money);

            //PlayerPrefs.SetInt(playerMoneyPrefKey, money);
            //PlayerPrefs.SetInt(playerMoneyStore, money);

            TestGameManager.instance.moneyPrefText.text = moneyToString(MoneyBoardManager.instance.MyCurrentMoney);
        }
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
    void setPlayersPosition()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.photonView.IsMine)
            {
                gamePlayerList[0] = playerList[i];
                transform.position = playerPositionList[0];
                Debug.Log("set my  position " + playerPositionList[0]);
            }
        }
        int index = 1;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (!gp.photonView.IsMine)
            {
                gamePlayerList[index] = playerList[i];
                gp.transform.position = playerPositionList[index];
                Debug.Log("set player " + index + " position " + playerPositionList[index]);
                index++;
                
            }
        }
    }
    public void MinusPlayerPrefabMoney(int minusMoney)
    {
        var t = LeaderBoardManager.instance.ModifyScore(-(int)minusMoney / 1000);
        //myScore = -(int)minusMoney / 1000;
        //var mm = MoneyBoardManager.instance.ModifyMoney(-(int)minusMoney, TestGameManager.instance.moneyPrefText);
        MoneyBoardManager.instance.ModifyMoney(-(int)minusMoney, TestGameManager.instance.moneyPrefText);
        /*int money = -1;
        money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        money -= minusMoney;
        TestGameManager.instance.moneyPrefText.text = moneyToString(money);
        PlayerPrefs.SetInt(playerMoneyPrefKey, money);*/

    }
    public void ReturnBetMoney()
    {
        //myScore = 0;
        var t = LeaderBoardManager.instance.ModifyScore((int)betNum / 1000);

        int money = -1;
        //var mm = MoneyBoardManager.instance.ModifyMoney((int)betNum, TestGameManager.instance.moneyPrefText);
        MoneyBoardManager.instance.ModifyMoney((int)betNum, TestGameManager.instance.moneyPrefText);
        /*money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        money += betNum;
        TestGameManager.instance.moneyPrefText.text = moneyToString(money);
        PlayerPrefs.SetInt(playerMoneyPrefKey, money);*/

    }
    public void UpdateCardType()
    {
        this.cardType = CardsManager.instance.GetCardsType(this.cardList);
        if (seenCardList.Count > 0)
        {
            this.seenCardType = CardsManager.instance.GetCardsType(this.seenCardList);
        }
        cardsTypeText.text = "card type:" + cardType + " seen card type:" + seenCardType;
        Debug.Log("card type:" + cardType + " seen card type:" + seenCardType);
        if (photonView.IsMine)
        {
            TestGameManager.instance.CardsTypeCode.text = cardType + "";
        }
        
        //UnLockAchievement(this.cardType);
    }
    
    /*public void UnLockAchievement(int cardType)
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        switch (cardType)
        {

            case GameInfo.CARD_TYPE_ONE_PAIR:
                {
                    
                    bool isSet;
                    SteamUserStats.GetAchievement("Achievement_OnePair", out isSet);
                    if (!isSet)
                    {
                        Debug.Log("UnLock new Achievement One Pair");
                        SteamUserStats.SetAchievement("Achievement_OnePair");
                        SteamUserStats.StoreStats();
                    }
                    SteamUserStats.ClearAchievement("Achievement_OnePair");
                    SteamUserStats.StoreStats();
                    break;
                }
            
            default:
                break;
        }

    }*/
    //[PunRPC]
    public void InitPlayer()
    {
        
        BetCanvas.SetActive(false);
        Bet(1000);

    }
    [PunRPC]
    public void setNameAndAvatar()
    {
        
        string s = "Player " + photonView.Owner.ActorNumber + ":" + photonView.Owner.NickName;
        PlayerNameText.text = s;

        object steamId;

        if (photonView.Owner.CustomProperties.TryGetValue(playerSteamId, out steamId))
        {
            
            ulong steamIdValue = Convert.ToUInt64((string)steamId);
            SteamId steamID = steamIdValue;
            var t = getSteamAvatarAsync(steamID);

        }
        //photonView.RPC("setPlayerName", RpcTarget.AllViaServer, s);
    }
    async Task getSteamAvatarAsync(SteamId steamID)
    {
        var AvatarLarge = await SteamFriends.GetLargeAvatarAsync(steamID);
        if (AvatarLarge.HasValue)
        {
            uint imageHeight = AvatarLarge.Value.Height;
            uint imageWidth = AvatarLarge.Value.Width;
            Debug.Log(imageWidth + "x" + imageHeight);
            DownloadedAvatar = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false);
            DownloadedAvatar.LoadRawTextureData(AvatarLarge.Value.Data);
            DownloadedAvatar.Apply();

            SteamAvatar.texture = DownloadedAvatar;
            return;
        }
       
    }
    public void setState(int stateCode)
    {

        Debug.Log("Player " + photonView.Owner.ActorNumber + " set state:" + stateCode);
        if (photonView.IsMine)
        {
            Hashtable props = new Hashtable
            {
                {GamePlayer.PLAYER_STATE, stateCode}
            };
            photonView.Owner.SetCustomProperties(props);
        }
        
        
        State = stateCode;
        switch (stateCode)
        {

            case GameInfo.PLAYER_WAIT_FOR_FIRST_CARD:
                {
                    StateDisplay.text = "wait for first card";
                    break;
                }
            case GameInfo.PLAYER_WAIT_FOR_SECOND_CARD:
                {
                    StateDisplay.text = "wait for second card";
                    break;
                }
            case GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE:
                {
                    StateDisplay.text = "wait for making chioce";
                    break;
                }
            case GameInfo.PLAYER_MAKING_CHOICE:
                {
                    StateDisplay.text = "making chioce";
                    break;
                }
            case GameInfo.PLAYER_FOLD:
                {
                    StateDisplay.text = "Fold";
                    break;
                }
            case GameInfo.PLAYER_ADD_BET:
                {
                    StateDisplay.text = "add bet @" + newBetNum;
                    break;
                }
            case GameInfo.PLAYER_RAISE_BET:
                {
                    StateDisplay.text = "raise bet @" + newBetNum;
                    break;
                }
            case GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA:
                {
                    StateDisplay.text = "wait for making chioce fold or soha";
                    break;
                }
            case GameInfo.PLAYER_MAKING_CHOICE_FOLD_OR_SOHA:
                {
                    StateDisplay.text = "making chioce fold or soha";
                    break;
                }
            case GameInfo.PLAYER_SOHA:
                {
                    StateDisplay.text = "SoHa" ;
                    break;
                }
            case GameInfo.PLAYER_CALL:
                {
                    StateDisplay.text = "Call";
                    break;
                }
            case GameInfo.PLAYER_WINNER:
                {
                    StateDisplay.text = "Winner";
                    break;
                }
            default:
                break;
        }
        
    }
    public void setRoomCallNum(int roomCallNum)
    {
        if (photonView.IsMine)
        {
            Hashtable properties = new Hashtable
                {
                    {ROOM_CALL_NUM, roomCallNum},
                };
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
    }
    public void addRoomBetNum(int addNum)
    {
        if (photonView.IsMine)
        {
            Hashtable properties = new Hashtable
                {
                    {ROOM_BET_NUM, roomBetNum+addNum},
                };
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            Debug.Log("add roomBetNum:" + addNum);

            
        }
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(ROOM_CALL_NUM))
        {
            object roomCallNum;
            if (propertiesThatChanged.TryGetValue(ROOM_CALL_NUM, out roomCallNum))
            {
                this.roomCallNum = (int)roomCallNum;
                Debug.Log("Update roomCallNum:" + (int)roomCallNum);
            }
        }
        if (propertiesThatChanged.ContainsKey(ROOM_BET_NUM))
        {
            object roomBetNum;
            if (propertiesThatChanged.TryGetValue(ROOM_BET_NUM, out roomBetNum))
            {
                this.roomBetNum = (int)roomBetNum;
                Debug.Log("Update roomBetNum:" + (int)roomBetNum);
            }
        }


    }
    public GamePlayer getGamePlayerByActorNumber(int actorNumber)
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        GamePlayer retPlayer = null;

        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.photonView.Owner.ActorNumber == actorNumber)
            {
                retPlayer = gp;
            }
        }
        return retPlayer;
    }
    public int check_OthersFold_OneWin()
    {
        List<int> stateList = new List<int>();
        int foldCount = 0;
        int winnerId = -1;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;
            if (p == PhotonNetwork.LocalPlayer)
            {
                stateList.Add(this.State);
            }
            else if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                stateList.Add((int)playerState);
            }
        }
        for (int i = 0; i < stateList.Count; i++)
        {
            if (stateList[i] == GameInfo.PLAYER_FOLD)
            {
                foldCount++;
            }
            else
            {
                winnerId = PhotonNetwork.PlayerList[i].ActorNumber;
            }

        }
        if (foldCount == PhotonNetwork.PlayerList.Length - 1 && foldCount > 0)
        {
            return winnerId;
        }
        return -1;
    }



    [PunRPC]
    public void addNewCard(int currendCardSetIndex,bool canBeSeen)
    {
        CardSet[] cardSet = GameObject.FindObjectsOfType<CardSet>();
        List<Card> allCards = cardSet[0].allCards;
        GameObject cardBack = cardSet[0].cardBack;
        int playerCardCount = this.cardList.Count;
        
        this.cardList.Add(allCards[currendCardSetIndex]);
        
        if (!canBeSeen)
        {
            if (!photonView.IsMine)
            {
                Vector3 cardBackPosition = this.CardBackPosition.transform.position;
                GameObject CardBack = Instantiate(cardBack, cardBackPosition, Quaternion.identity, this.CardBackPosition.transform);
                CardBack.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
                CardBack.transform.localScale = new Vector3(15, 15, 1);
                this.CardBackObject = CardBack;
            }
            Vector3 position = this.CardBackPosition.transform.position;
            Card newCard = Instantiate(allCards[currendCardSetIndex], position, Quaternion.identity, this.Cards.transform);
            newCard.gameObject.GetComponent<SpriteRenderer>().sortingOrder = playerCardCount + 1;

        }
        else
        {
            Vector3 position = this.Cards.transform.position + new Vector3((this.cardList.Count - 1) * offsetBetweenCards, 0, 0);

            GameObject CardBack = Instantiate(cardBack, Vector3.zero, Quaternion.identity);
            CardBack.transform.localScale = new Vector3(15, 15, 1);
            DeliverAudio.Play();
            this.CardAnmiObject = CardBack;
            this.CardAnmiTargetPosition = position;
            this.CardAnmiNewCard = allCards[currendCardSetIndex];
            this.deliverCardIng = true;
            seenCardList.Add(allCards[currendCardSetIndex]);
            //Card newCard = Instantiate(allCards[currendCardSetIndex], position, Quaternion.identity, this.Cards.transform);
            //newCard.gameObject.GetComponent<SpriteRenderer>().sortingOrder = playerCardCount + 1;
            
        }
        
        UpdateCardType();
        if (State == GameInfo.PLAYER_WAIT_FOR_FIRST_CARD)
        {
            setState(GameInfo.PLAYER_WAIT_FOR_SECOND_CARD);
            //this.photonView.RPC("setState", RpcTarget.All, GameState.PLAYER_WAIT_FOR_ROUND_TWO);
        }
        /*else if (State == GameInfo.PLAYER_WAIT_FOR_SECOND_CARD || State == GameInfo.PLAYER_RAISE_BET
            || State == GameInfo.PLAYER_ADD_BET|| State == GameInfo.PLAYER_CALL)
        {
            setState(GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE);
            //this.photonView.RPC("setState", RpcTarget.All, GameState.PLAYER_WAIT_FOR_ROUND_TWO);
        }*/
        
    }
    [PunRPC]
    public void firstMakeChoice(int startTime)
    {
        /*if (PhotonNetwork.IsMasterClient)
        {
            int startTimestamp;
            bool startTimeIsSet = PlayerChoiceTimer.TryGetStartTime(out startTimestamp);
            if (!startTimeIsSet)
            {
                PlayerChoiceTimer.SetStartTime();
            }
        }*/
        followChoice = false;
        Debug.Log(photonView.Owner.ActorNumber + " will make first choice");
        if (photonView.IsMine)
        {
            FirstChoiceCanvas.SetActive(true);
        }
        //CountDownTimeCanvas.SetActive(true);
        choiceStartTime = startTime;
        countDownTimeRunning = true;
        setState(GameInfo.PLAYER_MAKING_CHOICE);
    }
    [PunRPC]
    public void followMakeChoice(int startTime)
    {
        /*if (PhotonNetwork.IsMasterClient)
        {
            int startTimestamp;
            bool startTimeIsSet = PlayerChoiceTimer.TryGetStartTime(out startTimestamp);
            if (!startTimeIsSet)
            {
                PlayerChoiceTimer.SetStartTime();
            }
        }*/
        
        followChoice = true;

        if (State == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA)
        {
            if (photonView.IsMine)
            {
                FoldOrSoHaCanvas.SetActive(true);
                //YourTurnAudio.Play();
            }
            setState(GameInfo.PLAYER_MAKING_CHOICE_FOLD_OR_SOHA);
        }
        else
        {
            if (photonView.IsMine)
            {
                FollowChoiceCanvas.SetActive(true);
                //YourTurnAudio.Play();
            }
                
            setState(GameInfo.PLAYER_MAKING_CHOICE);
        }
        
        //CountDownTimeCanvas.SetActive(true);
        choiceStartTime = startTime;
        countDownTimeRunning = true;
        

    }
    public void SetAnyBetSlider()
    {
        int alreadyBetMoney = moneyForOneGame - allMoney;
        int callMoney = this.roomCallNum - this.betNum;
        if (callMoney < 0) callMoney = 0;
        int max = moneyForOneGame/1000;
        int alreadyBetValue= alreadyBetMoney / 1000;
        int callValue = callMoney / 1000;
        int r = allMoney % 1000;
        if (r > 0) max += 1;
        AnyBetSlider.minValue = 0;
        AnyBetSlider.maxValue = max;
        AlreadyBetSlider.minValue = 0;
        AlreadyBetSlider.maxValue = max;
        CallSlider.minValue = 0;
        CallSlider.maxValue = max;

        AnyBetSlider.value = alreadyBetValue + callValue;
        AlreadyBetSlider.value = alreadyBetValue;
        CallSlider.value = alreadyBetValue + callValue;

        AlreadyBetNumText.transform.position = AlreadyBetSlider.handleRect.transform.position + new Vector3(0,10,0);
        AlreadyBetNumText.text = ""+ alreadyBetMoney;
        CallNumText.transform.position = CallSlider.handleRect.transform.position + new Vector3(0, 10, 0);
        CallNumText.text = "" + (alreadyBetMoney + callMoney);
        MaxMoneyText.text = "" + moneyForOneGame;
    }
    public void SetAnyRaiseSlider()
    {
        int max = allMoney / 1000;
        int r = allMoney % 1000;
        if (r > 0) max += 1;
        AnyRaiseSlider.minValue = 0;
        AnyRaiseSlider.maxValue = max;
        AnyRaiseSlider.value = (this.roomCallNum - this.betNum) /1000;
        AnyRaiseNumText.text = "BetNum:" + (this.roomCallNum - this.betNum);
    }
    public void OnValueChange_AnyBetSlider()
    {
        int alreadyBetMoney = moneyForOneGame - allMoney;
        int callMoney = this.roomCallNum - this.betNum;
        if (callMoney < 0) callMoney = 0;
        int alreadyBetValue = alreadyBetMoney / 1000;
        int callValue = callMoney / 1000;
        if (AnyBetSlider.value < alreadyBetValue + callValue)
        {
            AnyBetSlider.value = alreadyBetValue + callValue;
        }
        int betNum = 1000 * ((int)AnyBetSlider.value - (int)CallSlider.value);
        AnyBetNumText.transform.position = AnyBetSlider.handleRect.transform.position + new Vector3(0, -20, 0);
        if (raiseBet)
        {
            AnyBetNumText.text = "" + callMoney+" + "+betNum;
        }
        else
        {
            AnyBetNumText.text = "" + betNum;
        }
        
    }
    public void OnValueChange_AnyRaiseSlider()
    {
        int betNum = 1000 * (int)AnyRaiseSlider.value;
        if (AnyRaiseSlider.value < (this.roomCallNum - this.betNum) / 1000)
        {
            AnyRaiseSlider.value = (this.roomCallNum - this.betNum) / 1000;
        }
        else
        {
            AnyRaiseNumText.text = "BetNum:" + betNum;
        }

    }
    public void OnClick_Bet()
    {
        Debug.Log("OnClick_Bet");

        if (string.IsNullOrEmpty(BetNumInput.text))
        {
            Debug.LogError("Player  is null or empty:" + BetNumInput.text);
            return;
        }
        int betNum;
        if (int.TryParse(BetNumInput.text, out betNum))
        {
            photonView.RPC("Bet", RpcTarget.All, betNum);
            //photonView.RPC("setState", RpcTarget.All, GameState.PLAYER_WAIT_FOR_CARD);
        }
        else
        {
            Debug.LogError("Please input int");
            return;
        }
    }
    public void OnClick_Fold()
    {
        Debug.Log("OnClick_Fold");
        photonView.RPC("Fold", RpcTarget.All);
    }
    public void OnClick_ChoiceBet()
    {
        raiseBet = false;
        FirstChoiceCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        //Choice_BetCanvas.SetActive(true);
        Choice_AnyBetCanvas.SetActive(true);
        SetAnyBetSlider();

    }
    public void OnClick_ChoiceRaise()
    {
        raiseBet = true;
        FirstChoiceCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        //Choice_RaiseCanvas.SetActive(true);
        Choice_AnyBetCanvas.SetActive(true);
        SetAnyBetSlider();
    }
    public void OnClick_ChoiceBack()
    {
        
        if (followChoice)
        {
            Choice_RaiseCanvas.SetActive(false);
            Choice_AnyBetCanvas.SetActive(false);
            FollowChoiceCanvas.SetActive(true);
        }
        else
        {
            Choice_BetCanvas.SetActive(false);
            Choice_AnyBetCanvas.SetActive(false);
            FirstChoiceCanvas.SetActive(true);
        }
    }
    public void OnClick_Bet10K()
    {
        Debug.Log("OnClick_Bet10K   "+allMoney);
        int callMoney = this.roomCallNum - this.betNum;
        if ((allMoney- callMoney) < 10000)
        {
            return;
        }
        if ((allMoney - callMoney) == 10000)
        {
            OnClick_SOHA();
            return;
        }
        if (raiseBet)
        {
            photonView.RPC("RaiseBet", RpcTarget.All, 10000);
        }
        else
        {
            photonView.RPC("AddBet", RpcTarget.All, 10000);
        }
        
        
    }
    public void OnClick_Bet100K()
    {
        Debug.Log("OnClick_Bet100K   " + allMoney);
        int callMoney = this.roomCallNum - this.betNum;
        if ((allMoney - callMoney) < 100000)
        {
            return;
        }
        if ((allMoney - callMoney) == 100000)
        {
            OnClick_SOHA();
            return;
        }
        if (raiseBet)
        {
            photonView.RPC("RaiseBet", RpcTarget.All, 100000);
        }
        else
        {
            photonView.RPC("AddBet", RpcTarget.All, 100000);
        }
    }
    
    public void OnClick_AnyBetConfirm()
    {
        int betNum = 1000 * ((int)AnyBetSlider.value - (int)CallSlider.value);
        int callMoney = this.roomCallNum - this.betNum;
        if (callMoney < 0) callMoney = 0;
        if (raiseBet)
        {

            if (betNum == 0)
            {
                OnClick_ChoiceCall();
                return;
            }
            if (betNum + callMoney >= allMoney)
            {
                betNum = allMoney;
                OnClick_SOHA();
                return;
            }
            else
            {
                photonView.RPC("RaiseBet", RpcTarget.All, betNum);
            }
        }
        else
        {
            if (betNum == 0)
            {
                return;
            }
            if (betNum >= allMoney)
            {
                betNum = allMoney;
                OnClick_SOHA();
            }
            else
            {
                photonView.RPC("AddBet", RpcTarget.All, betNum);
            }
        }
        
        

    }
    public void OnClick_AnyRaiseConfirm()
    {
        int betNum = 1000 * (int)AnyRaiseSlider.value - (this.roomCallNum - this.betNum);
        if (betNum == 0)
        {
            return;
        }
        if (AnyRaiseSlider.value == (this.roomCallNum - this.betNum) / 1000)
        {
            OnClick_ChoiceCall();
            return;
        }
        if (betNum >= allMoney)
        {
            betNum = allMoney;
            OnClick_SOHA();
            return;
        }
        else
        {
            photonView.RPC("RaiseBet", RpcTarget.All, betNum);
        }


    }
    public void OnClick_ChoiceCall()
    {
        photonView.RPC("Call", RpcTarget.All);

    }
    public void OnClick_Raise10K()
    {
        if (allMoney < this.roomCallNum - this.betNum + 10000)
        {
            return;
        }
        photonView.RPC("RaiseBet", RpcTarget.All, 10000);

    }
    public void OnClick_Raise100K()
    {
        if (allMoney < this.roomCallNum - this.betNum + 100000)
        {
            return;
        }
        photonView.RPC("RaiseBet", RpcTarget.All, 100000);
    }
    public void OnClick_SOHA()
    {
        photonView.RPC("SoHa", RpcTarget.All);
    }
    public void makeOtherPlayerFollow_AfterRaiseBet()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.State != GameInfo.PLAYER_FOLD && gp.photonView.Owner.ActorNumber!=photonView.Owner.ActorNumber)
            {
                InfoText.text = "Player " + gp.photonView.Owner.ActorNumber + " SetState to make choice .";
                Debug.Log("Player " + gp.photonView.Owner.ActorNumber + " SetState to make choice.");
                gp.setState( GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE);
            }
        }
    }
    public void makeOtherPlayerFollow_AfterSoHa()
    {
        Debug.Log("make Other Player Follow After SoHa");
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.State != GameInfo.PLAYER_FOLD && gp.photonView.Owner.ActorNumber != photonView.Owner.ActorNumber)
            {
                InfoText.text = "Player " + gp.photonView.Owner.ActorNumber + " fold or soha .";
                Debug.Log("Player " + gp.photonView.Owner.ActorNumber + "fold or soha.");
                gp.setState(GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA);
            }
        }
    }
    public void chooseNextPlayerToMakeChoice(bool first)
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if ((gp.State == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE|| gp.State == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA) 
                && gp.photonView.Owner.ActorNumber != photonView.Owner.ActorNumber)
            {
                InfoText.text = "Player " + gp.photonView.Owner.ActorNumber + " will make choice.";
                Debug.Log("Player " + gp.photonView.Owner.ActorNumber + " will make choice.");
                if (first)
                {
                    gp.photonView.RPC("firstMakeChoice", RpcTarget.All, PhotonNetwork.ServerTimestamp);
                }
                else
                {
                    gp.photonView.RPC("followMakeChoice", RpcTarget.All, PhotonNetwork.ServerTimestamp);
                }
                break;
            }
        }
    }
    private IEnumerator DisplayBetNum(int num)
    {
        BetDisplayText.text = moneyToString(num);
        BetDisplayText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        BetDisplayText.gameObject.SetActive(false);
        BetDisplayText.text = "";

    }
    [PunRPC]
    public void AddBet(int betNum)
    {
        this.betNum += betNum;
        allMoney -= betNum;
        newBetNum += betNum;
        setRoomCallNum(this.betNum);
        addRoomBetNum(betNum);

        MoneyAndBetText.text = "$"+allMoney+" @" + this.betNum;
        StartCoroutine(DisplayBetNum(betNum));
        Choice_BetCanvas.SetActive(false);
        Choice_AnyBetCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        FirstChoiceCanvas.SetActive(false);
        setState(GameInfo.PLAYER_ADD_BET);
        Debug.Log("Player " + photonView.Owner.ActorNumber + " addbet,find next player.");
        if (photonView.IsMine)
        {
            MinusPlayerPrefabMoney(betNum);
            chooseNextPlayerToMakeChoice(false);
        }
        closeTimer();
    }
    [PunRPC]
    public void RaiseBet(int betNum)
    {
        int shouldBet = this.roomCallNum - this.betNum+betNum;//have bet and other raise
        this.betNum = this.roomCallNum + betNum;
        //allMoney = moneyForOneGame-this.betNum;
        allMoney -= shouldBet;
        newBetNum = this.betNum;
        setRoomCallNum(this.betNum);
        addRoomBetNum(shouldBet);
        //TODO
        //²¥·ÅÉùÒô£º´óÄã
        MoneyAndBetText.text = "$" + allMoney + " @" + this.betNum;
        StartCoroutine(DisplayBetNum(betNum));
        Choice_RaiseCanvas.SetActive(false);
        Choice_AnyBetCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        FirstChoiceCanvas.SetActive(false);

        Debug.Log("Player " + photonView.Owner.ActorNumber + " raise bet,find next player.");
        makeOtherPlayerFollow_AfterRaiseBet();
        setState(GameInfo.PLAYER_RAISE_BET);

        if (photonView.IsMine)
        {
            MinusPlayerPrefabMoney(shouldBet);
            chooseNextPlayerToMakeChoice(false);
        }
        closeTimer();
    }
    [PunRPC]
    public void Bet(int betNum)
    {
        this.betNum += betNum;
        allMoney -= betNum;
        MoneyAndBetText.text = "$" + allMoney + " @" + this.betNum;
        StartCoroutine(DisplayBetNum(betNum));
        BetCanvas.SetActive(false);
        setState(GameInfo.PLAYER_WAIT_FOR_FIRST_CARD);

        if (photonView.IsMine)
        {
            //PlayerPrefs.SetInt(playerMoneyStore, allMoney);
            MinusPlayerPrefabMoney(betNum);
            if (PhotonNetwork.IsMasterClient)
            {
                int playerCount = PhotonNetwork.PlayerList.Length;
                print("count:" + playerCount);
                addRoomBetNum(betNum * playerCount);
            }
        }

        
    }
    
    [PunRPC]
    public void Fold()
    {
        BadCardAudio.Play();
        FirstChoiceCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        FoldOrSoHaCanvas.SetActive(false);
        setState(GameInfo.PLAYER_FOLD);
        FoldStatePanel.SetActive(true);
        Debug.Log("Player " + photonView.Owner.ActorNumber + " fold,find next player.");
        if (photonView.IsMine)
        {
            /*if (check_OthersFold_OneWin() > 0)
            {
                int winnerActorNumber = check_OthersFold_OneWin();
                GamePlayer winner = getGamePlayerByActorNumber(winnerActorNumber);
                winner.photonView.RPC("winnerSetMoney", RpcTarget.All, 1000000);
                Debug.Log("Player " + winnerActorNumber + " win!");
                InfoText.text = "Player " + winnerActorNumber + " win!";
            }
            else
            {
                if (!this.followChoice)
                {
                    //chooseNextPlayerToMakeChoice(true);
                }
                else
                {
                    chooseNextPlayerToMakeChoice(false);
                }
            }*/
            
        }
        closeTimer();
    }
    [PunRPC]
    public void Call()
    {
        int shouldBet = this.roomCallNum - this.betNum;//have bet and other raise
        this.betNum = this.roomCallNum;
        allMoney -= shouldBet;
        newBetNum = this.roomCallNum;
        addRoomBetNum(shouldBet);
        //TODO
        //²¥·ÅÉùÒô£º´óÄã
        MoneyAndBetText.text = "$" + allMoney + " @" + this.betNum;
        StartCoroutine(DisplayBetNum(shouldBet));
        Choice_BetCanvas.SetActive(false);
        Choice_AnyBetCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        FirstChoiceCanvas.SetActive(false);
        setState(GameInfo.PLAYER_CALL);
        Debug.Log("Player " + photonView.Owner.ActorNumber + " fold,find next player.");
        if (photonView.IsMine)
        {
            MinusPlayerPrefabMoney(shouldBet);
            chooseNextPlayerToMakeChoice(false);
        }
        closeTimer();
    }
    [PunRPC]
    public void SoHa()
    {
        setRoomCallNum(moneyForOneGame);
        this.betNum += allMoney;
        addRoomBetNum(allMoney);
        if (photonView.IsMine)
        {
            MinusPlayerPrefabMoney(allMoney);
        }
        StartCoroutine(DisplayBetNum(allMoney));
        allMoney =0;
        //TODO
        SoHaAudio.Play();
        //²¥·ÅÉùÒô£ºËó¹þ
        MoneyAndBetText.text = "$" + allMoney + " @" + this.betNum;
        
        Choice_BetCanvas.SetActive(false);
        Choice_AnyBetCanvas.SetActive(false);
        FollowChoiceCanvas.SetActive(false);
        FirstChoiceCanvas.SetActive(false);
        FoldOrSoHaCanvas.SetActive(false);

        Debug.Log("Player " + photonView.Owner.ActorNumber + " fold,find next player.");
        if (State == GameInfo.PLAYER_MAKING_CHOICE)
        {
            makeOtherPlayerFollow_AfterSoHa();
        }
        if (photonView.IsMine)
        {
            
            chooseNextPlayerToMakeChoice(false);
        }
        setState(GameInfo.PLAYER_SOHA);
        closeTimer();
    }
    [PunRPC]
    public void reStart()
    {
        if (photonView.IsMine)
        {
            ReturnBetMoney();
            /*int storeMoney = PlayerPrefs.GetInt(playerMoneyStore);
            if (storeMoney > 0)
            {
                int money = -1;
                money = PlayerPrefs.GetInt(playerMoneyPrefKey);
                money += storeMoney;
                TestGameManager.instance.moneyPrefText.text = "$" + moneyToString(money);
                PlayerPrefs.SetInt(playerMoneyPrefKey, money);
                PlayerPrefs.SetInt(playerMoneyStore, 0);
            }*/

            PhotonNetwork.CurrentRoom.CustomProperties.Clear();
            SceneManager.LoadScene(1);
        }
        closeTimer();

    }
    [PunRPC]
    public void destroyCardBack()
    {
        if (!photonView.IsMine)
        {
            Destroy(CardBackObject);
        }
    }
    [PunRPC]
    public void winnerSetMoney(int winMoney)
    {
        GameObject character = TestGameManager.instance.CharacterStand;
        GameObject characterGone1 = TestGameManager.instance.CharacterGone1;
        GameObject characterGone2 = TestGameManager.instance.CharacterGone2;
        GameObject characterHold = TestGameManager.instance.CharacterHold;
        
        Debug.Log(character.transform.position);
        //character.transform.position = this.transform.position;
        StartCoroutine(characterMoveAnmi(character, characterGone1, characterGone2, characterHold, this.transform.position));
    }
    IEnumerator characterMoveAnmi(GameObject character, GameObject characterGone1, GameObject characterGone2, GameObject characterHold, Vector3 targetPosition)
    {
        if (photonView.IsMine)
        {
            Debug.Log("winnerSetMoney");

            int myWinMoney = 0;
            if (roomBetNum >= betNum) myWinMoney = roomBetNum - betNum;
            int myWinScore = (int)myWinMoney / 1000;

            var t = LeaderBoardManager.instance.ModifyScore((int)roomBetNum / 1000);

            
            /*int money = -1;
            money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            money += roomBetNum;
            PlayerPrefs.SetInt(playerMoneyPrefKey, money);
            TestGameManager.instance.moneyPrefText.text = moneyToString(money);*/

            //var mm = MoneyBoardManager.instance.ModifyMoney(roomBetNum, TestGameManager.instance.moneyPrefText);
            MoneyBoardManager.instance.ModifyMoney(roomBetNum, TestGameManager.instance.moneyPrefText);

            setState(GameInfo.PLAYER_WINNER);
            makeOtherPlayerSetRestMoney();

        }
        Vector3 offset = new Vector3(16.5f, -16.5f, 0);
        character.SetActive(false);
        characterGone1.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        characterGone1.SetActive(false);
        characterGone2.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        characterGone2.SetActive(false);
        character.transform.position = targetPosition;
        characterGone1.transform.position = targetPosition;
        characterGone2.transform.position = targetPosition;
        characterHold.transform.position = targetPosition + offset;
        yield return new WaitForSeconds(0.07f);
        characterGone2.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        characterGone2.SetActive(false);
        characterGone1.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        characterGone1.SetActive(false);
        character.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        character.SetActive(false);
        characterHold.SetActive(true);
        WinStatePanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        if (photonView.IsMine)
        {
            int myWinMoney = 0;
            if (roomBetNum >= betNum) myWinMoney = roomBetNum - betNum;
            int myWinScore = (int)myWinMoney / 1000;
            WinAudio.Play();
            TestGameManager.instance.ShowRankCanvas(roomBetNum, true, myWinScore);
        }

    }
    public void makeOtherPlayerSetRestMoney()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (!gp.photonView.IsMine)
            {
                gp.photonView.RPC("setRestMoney", RpcTarget.All);
            }
            
        }
    }
    [PunRPC]
    public void setRestMoney()
    {
        if (photonView.IsMine)
        {
            Debug.Log("setRestMoney");


            int score = (int)betNum / 1000;

            TestGameManager.instance.ShowRankCanvas(betNum, false , score);
            LoseAudio.Play();
            /*int money = -1;
            money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            money -= betNum;
            PlayerPrefs.SetInt(playerMoneyPrefKey, money);
            TestGameManager.instance.moneyPrefText.text = moneyToString(money);*/

            PlayerPrefs.SetInt(playerMoneyStore, 0);
        }
    }
    private void ChoiceTimeEnd()
    {
        
    }
    // Update is called once per frame
    
    private float TimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.choiceStartTime;
        return this.choiceCountDownTime - timer / 1000f;
    }
    private void OnTimerEnds()
    {
        this.countDownTimeRunning = false;

        Debug.Log("Emptying info text.", this.countDownText);
        this.countDownText.text = string.Empty;
        CountDownTimeCanvas.SetActive(false);
        OnClick_Fold();
    }
    private void closeTimer()
    {
        this.countDownTimeRunning = false;

        Debug.Log("Emptying info text.", this.countDownText);
        this.countDownText.text = string.Empty;
        CountdownImage.fillAmount = 1;
        CountDownTimeCanvas.SetActive(false);
    }
    

    void cardMove(GameObject cardBack, Vector3 targetPosition)
    {
        cardBack.transform.localPosition = Vector3.MoveTowards(cardBack.transform.localPosition, targetPosition, 2000 * Time.deltaTime);
        if (cardBack.transform.position == targetPosition)
        {
            this.deliverCardIng = false;
            Card newCard = Instantiate(this.CardAnmiNewCard, targetPosition, Quaternion.identity, this.Cards.transform);
            newCard.gameObject.GetComponent<SpriteRenderer>().sortingOrder = this.cardList.Count + 1;
            //seenCardList.Add(this.CardAnmiNewCard);
            Destroy(cardBack);
            if (State == GameInfo.PLAYER_WAIT_FOR_FIRST_CARD)
            {
                setState(GameInfo.PLAYER_WAIT_FOR_SECOND_CARD);
                //this.photonView.RPC("setState", RpcTarget.All, GameState.PLAYER_WAIT_FOR_ROUND_TWO);
            }
            else if (State == GameInfo.PLAYER_WAIT_FOR_SECOND_CARD || State == GameInfo.PLAYER_RAISE_BET
                || State == GameInfo.PLAYER_ADD_BET || State == GameInfo.PLAYER_CALL)
            {
                setState(GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE);
                //this.photonView.RPC("setState", RpcTarget.All, GameState.PLAYER_WAIT_FOR_ROUND_TWO);
            }
            else if (State == GameInfo.PLAYER_SOHA)
            {
                setState(GameInfo.PLAYER_SOHA);
            }
        }
    }

    IEnumerator cardMoveAnmi(GameObject card, Vector3 targetPosition)
    {
       
        while (card.transform.localPosition != targetPosition)
        {
            //print(card.transform.localPosition);
            card.transform.localPosition = Vector3.MoveTowards(card.transform.localPosition, targetPosition, 100 * Time.deltaTime);
            yield return 0;
        }
    }

    void Update()
    {
        if (this.deliverCardIng)
        {
            cardMove(this.CardAnmiObject, this.CardAnmiTargetPosition);
        }
        if (!this.countDownTimeRunning) return;

        float countdown = TimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        this.countDownText.text = string.Format("{0}", countdown.ToString("n0"));
        CountdownImage.fillAmount = countdown / choiceCountDownTime;
        if (countdown > 0.0f) return;
        OnTimerEnds();
    }


}

