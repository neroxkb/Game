using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
public class TestGameManager : MonoBehaviourPunCallbacks
{
    public static TestGameManager instance; 
    public readonly string SeedPropertiesKey = "Seed";
    public readonly string RoomRound = "RoomRound";
    public int Round = 1;
    public int maxRound = 5;
    public int MoneyForOneGame = -1;

    public Text InfoText;
    public Text moneyPrefText;
    public Text CardsTypeText;
    public Text CardsTypeCode;

    public GameObject CardSet;
    public Canvas RankCanvas;
    public Canvas WaitingCanvas;
    public Canvas NotEnoughPlayerCanvas;

    public Button OneMoreGameButton;
    public Text OutOfMoneyText;
    public Text WinOrLose;
    public Text RankMoneyText;
    public Text RankScoreText;
    private int nextPlayerId = 1;
    const string playerMoneyPrefKey = "PlayerMoney";

    public GameObject CharacterStand;
    public GameObject CharacterGone1;
    public GameObject CharacterGone2;
    public GameObject CharacterHold;
    // Start is called before the first frame update
    private int getSeed()
    {
        return (int)System.DateTime.Now.Ticks;
    }
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
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            WaitingCanvas.gameObject.SetActive(false);
            NotEnoughPlayerCanvas.gameObject.SetActive(true);
            return;
        }
        int money = MoneyBoardManager.instance.MyCurrentMoney;
        //money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        moneyPrefText.text = moneyToString(money);
        Hashtable props = new Hashtable
            {
                {GamePlayer.PLAYER_MONEY, money}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        //moneyPrefText.text = moneyToString(MoneyBoardManager.instance.MyCurrentMoney);
        //var t = MoneyBoardManager.instance.TestGameLoadMoney(moneyPrefText);
        Hashtable propsLoaded = new Hashtable
            {
                {GamePlayer.PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsLoaded);
        

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
    public override void OnEnable()
    {
        base.OnEnable();

        TestTimer.OnCountdownTimerEnd += CountdownTimerEnd;
    }
    public override void OnDisable()
    {
        base.OnDisable();

        TestTimer.OnCountdownTimerEnd -= CountdownTimerEnd;
    }
    private void DeliverACardToPlayer()
    {
        Player[] playerList = PhotonNetwork.PlayerList;
        int playerActorNumber = -1;
        foreach (Player player in playerList)
        {
            object playerState;
            if (player.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                if ((int)playerState == GameInfo.PLAYER_WAIT_FOR_FIRST_CARD)
                {
                    this.CardSet.GetComponent<PhotonView>().RPC("deliverNextCard", RpcTarget.All, player.ActorNumber, GameInfo.PLAYER_WAIT_FOR_FIRST_CARD, false);
                }
            }
        }
        
    }
    private IEnumerator DeliverFirstCard()
    {
        List<int> playerKeys = new List<int>();
        foreach (var key in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            playerKeys.Add((int)key);
        }
        foreach (int key in playerKeys)
        {
            int playerNumber = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;
            this.CardSet.GetComponent<PhotonView>().RPC("deliverNextCard", RpcTarget.All, playerNumber, GameInfo.PLAYER_WAIT_FOR_FIRST_CARD, false);
            yield return new WaitForSeconds(1f);
            Debug.Log(key);
        }
        

    }
    /*private IEnumerator DeliverSecondCard()
    {
        foreach (var key in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            int playerNumber = PhotonNetwork.CurrentRoom.Players[(int)key].ActorNumber;
            this.CardSet.GetComponent<PhotonView>().RPC("deliverNextCard", RpcTarget.All, playerNumber, GameInfo.PLAYER_WAIT_FOR_SECOND_CARD, true);
            yield return new WaitForSeconds(0.5f);
            Debug.Log(key);
        }

    }*/
    private IEnumerator DeliverSecondCard()
    {
        List<int> playerKeys = new List<int>();
        foreach (var key in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            playerKeys.Add((int)key);
        }
        foreach (int key in playerKeys)
        {
            int playerNumber = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;
            this.CardSet.GetComponent<PhotonView>().RPC("deliverNextCard", RpcTarget.All, playerNumber, GameInfo.PLAYER_WAIT_FOR_SECOND_CARD, true);
            yield return new WaitForSeconds(1f);
            Debug.Log(key);
        }

    }
    /*private IEnumerator DeliverACardToAll()
    {

        foreach (var key in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            int playerNumber = PhotonNetwork.CurrentRoom.Players[(int)key].ActorNumber;
            this.CardSet.GetComponent<PhotonView>().RPC("deliverACardToAll", RpcTarget.All, playerNumber, true);
            yield return new WaitForSeconds(0.5f);
            //Debug.Log(key);
        }

    }*/
    private IEnumerator DeliverACardToAll()
    {
        yield return new WaitForSeconds(1f);
        List<int> playerKeys = new List<int>();
        foreach (var key in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            playerKeys.Add((int)key);
        }
        foreach (int key in playerKeys)
        {
            int playerNumber = PhotonNetwork.CurrentRoom.Players[(int)key].ActorNumber;
            this.CardSet.GetComponent<PhotonView>().RPC("deliverACardToAll", RpcTarget.All, playerNumber, true);
            yield return new WaitForSeconds(1f);
            //Debug.Log(key);
        }
        
    }
    private GamePlayer getBiggestPlayerSeenCards()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        GamePlayer biggestPlayer = playerList[0];

        for (int i = 1; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.State != GameInfo.PLAYER_FOLD)
            {
                if (biggestPlayer.State == GameInfo.PLAYER_FOLD)
                {
                    biggestPlayer = gp;
                }
                else if (CardsManager.instance.compareCards(gp.seenCardList, biggestPlayer.seenCardList) == 1)
                {
                    biggestPlayer = gp;
                }
            }
        }
        return biggestPlayer;
    }
    private GamePlayer getBiggestPlayerAllCards()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        GamePlayer biggestPlayer = playerList[0];

        for (int i = 1; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.State != GameInfo.PLAYER_FOLD)
            {
                if (biggestPlayer.State == GameInfo.PLAYER_FOLD)
                {
                    biggestPlayer = gp;
                }
                else if (CardsManager.instance.compareCards(gp.cardList, biggestPlayer.cardList) == 1)
                {
                    biggestPlayer = gp;
                }
            }
        }
        return biggestPlayer;
    }
    private void biggestPlayerMakingChoice()
    {
        GamePlayer biggestPlayer = getBiggestPlayerSeenCards();
        int biggestPlayerId = biggestPlayer.photonView.Owner.ActorNumber;
        InfoText.text = "Player " + biggestPlayerId + " will make choice.";
        Debug.Log("Player " + biggestPlayerId + " will make choice.");
        biggestPlayer.photonView.RPC("firstMakeChoice",RpcTarget.All, PhotonNetwork.ServerTimestamp);
        
        //this.CardSet.GetComponent<PhotonView>().RPC("nextPlayerMakeChoice", RpcTarget.All, biggestPlayerId);
    }
    private void MakeWinner()
    {
        DestroyCardBack();
        GamePlayer biggestPlayer = getBiggestPlayerAllCards();
        biggestPlayer.photonView.RPC("winnerSetMoney", RpcTarget.All, 1000000);
        int biggestPlayerId = biggestPlayer.photonView.Owner.ActorNumber;
        Debug.Log("Player " + biggestPlayerId + " win!");
        InfoText.text = "Player " + biggestPlayerId + " win!";

        Round++;
        

    }
    private void DestroyCardBack()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            gp.photonView.RPC("destroyCardBack", RpcTarget.All);
        }
    }
    void ClearRoom()
    {
        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        /*foreach (var key in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            Player player = PhotonNetwork.CurrentRoom.Players[(int)key];
            player.CustomProperties.Clear();
            PhotonNetwork.DestroyPlayerObjects(player);
        }*/
    }
    void RoundStart()
    {
        print("RoundStart");
        SetRoomRound();
        GenerateRandomCardSet();
        OnClick_Restart();
    }
    void GameOver()
    {

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (changedProps.ContainsKey(GamePlayer.PLAYER_MONEY))
        {
            CheckPlayerMoneyAndSetMoney();
        }

        //Debug.Log("Player " + targetPlayer.NickName + " Prop Update");
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start

        
        if (changedProps.ContainsKey(GamePlayer.PLAYER_LOADED_LEVEL))
        {
            CheckPlayerLoadedAndStartGame();
        }

        
        if (changedProps.ContainsKey(GamePlayer.PLAYER_STATE))
        {
            object changedState;

            if (targetPlayer.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out changedState))
            {
                if ((int)changedState == GameInfo.PLAYER_FOLD)
                {
                    GameManageAfterFold((int)changedState);
                }
                /*else if ((int)changedState == GameInfo.PLAYER_WAIT_FOR_SECOND_CARD)
                {
                    int state = CheckAllPlayerState();
                    if (state != GameInfo.PLAYER_WAIT_FOR_SECOND_CARD)
                    {
                        DeliverACardToPlayer();
                    }
                }*/
                else
                {
                    GameManage();
                }
            }
            
        }

    }
    public void GameManageAfterFold(int changedState)
    {
        if (check_OthersFold_OneWin() > 0)
        {
            DestroyCardBack();
            int winnerActorNumber = check_OthersFold_OneWin();
            GamePlayer winner = getGamePlayerByActorNumber(winnerActorNumber);
            winner.photonView.RPC("winnerSetMoney", RpcTarget.All, 1000000);
            Debug.Log("Player " + winnerActorNumber + " win!");
            InfoText.text = "Player " + winnerActorNumber + " win!";
        }
        else
        {
            int signal=checkPlayerMakeChoice();
            if (signal == 1)
            {
                chooseNextPlayerToMakeChoice(true);
            }
            else if (signal == 2 || signal == 3)
            {
                chooseNextPlayerToMakeChoice(false);
            }
            else
            {
                GameManage();
            }
        } 
    }
    public int GameManage()
    {
        
        int state = CheckAllPlayerState();
        Debug.Log("all player state:" + state);
        int deliverCard = checkDeliverCard();
        if (checkSomeOneMakingChioce())
        {
            Debug.Log("someone is making choice,do nothing");
            return -1;
        }
        if (state == GameInfo.PLAYER_WAIT_FOR_FIRST_CARD)
        {
            Debug.Log("It's time to deliver first card");
            InfoText.text = "It's time to deliver first card";
            StartCoroutine(DeliverFirstCard());
        }
        if (state == GameInfo.PLAYER_WAIT_FOR_SECOND_CARD)
        {
            StopAllCoroutines();
            Debug.Log("It's time to deliver second card");
            InfoText.text = "It's time to deliver second card";
            StartCoroutine(DeliverSecondCard());
        }
        else if (check_OthersFold_OneWin() > 0)
        {
            //DestroyCardBack();
            int winnerActorNumber = check_OthersFold_OneWin();
            /*GamePlayer winner = getGamePlayerByActorNumber(winnerActorNumber);
            winner.photonView.RPC("winnerSetMoney", RpcTarget.All, 1000000);*/
            Debug.Log("Player " + winnerActorNumber + " win!");
            InfoText.text = "Player " + winnerActorNumber + " win!";
        }
        else if (state == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE)
        {
            StopAllCoroutines();
            Debug.Log("It's time to making chioce");
            InfoText.text = "It's time to making chioce";
            biggestPlayerMakingChoice();
            //StartCoroutine(DeliverCard());
            //nextPlayerMakingChoice();
        }
        
        else if (checkAllSoHa() && checkPlayerCardsCountSame())
        {
            StopAllCoroutines();
            Debug.Log("It's time to deliver a card");
            InfoText.text = "It's time to deliver a card";
            int cardCount = checkPlayerCardsCount();

            if (cardCount == 5)
            {
                MakeWinner();
            }
            else
            {
                StartCoroutine(DeliverACardToAll());
            }

            
            //MakeUnseenCardSeen();
            

        }
        else if (deliverCard == 1)
        {
            StopAllCoroutines();
            Debug.Log("It's time to deliver next card");
            InfoText.text = "It's time to deliver next card";
            StartCoroutine(DeliverACardToAll());
            //biggestPlayerMakingChoice();
            //StartCoroutine(DeliverSeenCard());
        }
        else if (deliverCard == 5)
        {
            StopAllCoroutines();
            Debug.Log("cards full , compare and winner");
            InfoText.text = "cards full , compare and winner";
            MakeWinner();
            //StartCoroutine(DeliverSeenCard());
        }
        return -1;
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
    public void chooseNextPlayerToMakeChoice(bool first)
    {
        int nextActorNumber = -1;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;
            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                if ((int)playerState == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE 
                    || (int)playerState == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA)
                {
                    nextActorNumber = p.ActorNumber;
                    break;
                }
            }
        }
        if (nextActorNumber == -1)
        {
            Debug.LogError("Something wrong here.");
            return;
        } 
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = playerList.Length;
        for (int i = 0; i < playerCount; i++)
        {
            GamePlayer gp = playerList[i];
            if (gp.photonView.Owner.ActorNumber == nextActorNumber)
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
    public void GameManage2(Player targetPlayer, Hashtable changedProps)
    {
        int state = CheckAllPlayerState();
        Debug.Log("all player state:" + state);
        int deliverCard = checkDeliverCard();
        if (state == GameInfo.PLAYER_WAIT_FOR_FIRST_CARD)
        {
            Debug.Log("It's time to deliver first card");
            InfoText.text = "It's time to deliver first card";
            StartCoroutine(DeliverFirstCard());
        }
        if (state == GameInfo.PLAYER_WAIT_FOR_SECOND_CARD)
        {
            StopAllCoroutines();
            Debug.Log("It's time to deliver second card");
            InfoText.text = "It's time to deliver second card";
            StartCoroutine(DeliverSecondCard());
        }
        else if (state == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE)
        {
            StopAllCoroutines();
            Debug.Log("It's time to making chioce");
            InfoText.text = "It's time to making chioce";
            biggestPlayerMakingChoice();
            //StartCoroutine(DeliverCard());
            //nextPlayerMakingChoice();
        }
        else if (check_OthersFold_OneWin() > 0)
        {
            //DestroyCardBack();
            int winnerId = check_OthersFold_OneWin();
            //GamePlayer winner = GamePlayer.instance.getGamePlayerByActorNumber(winnerId);
            //winner.photonView.RPC("winnerSetMoney", RpcTarget.All, 1000000);
            Debug.Log("Player " + winnerId + " win!");
            InfoText.text = "Player " + winnerId + " win!";
        }
        else if (checkAllSoHa())
        {
            StopAllCoroutines();
            Debug.Log("It's time to deliver all card");
            InfoText.text = "It's time to deliver all card";
            int cardCount = checkPlayerCardsCount();
            for (int i = 0; i < 5 - cardCount; i++)
            {
                DeliverACardToAll();
            }
            //MakeUnseenCardSeen();
            MakeWinner();
        }
        else if (deliverCard == 1)
        {
            StopAllCoroutines();
            Debug.Log("It's time to deliver next card");
            InfoText.text = "It's time to deliver next card";
            DeliverACardToAll();
            //biggestPlayerMakingChoice();
            //StartCoroutine(DeliverSeenCard());
        }
        else if (deliverCard == 5)
        {
            StopAllCoroutines();
            Debug.Log("cards full , compare and winner");
            InfoText.text = "cards full , compare and winner";
            MakeWinner();
            //StartCoroutine(DeliverSeenCard());
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        object state;
        Debug.Log("PlayerLeft");
        if (WaitingCanvas.gameObject.activeSelf)
        {
            Debug.Log("Restart2");
            Restart();
            return;
        }
        if (!RankCanvas.gameObject.activeSelf)
        {
            Debug.Log("Restart");
            OnClick_Restart();
            return;
        }
        if (PhotonNetwork.PlayerList.Length < 2 && WaitingCanvas.gameObject.activeSelf)
        {
            WaitingCanvas.gameObject.SetActive(false);
            NotEnoughPlayerCanvas.gameObject.SetActive(true);
            return;
        }
        
        /*if (otherPlayer.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out state))
        {
            if ((int)state != GameInfo.PLAYER_FOLD)
            {
                OnClick_Restart();
            }
            else
            {
                Debug.Log("Player " + otherPlayer.ActorNumber + " leave room.");
                if (PhotonNetwork.IsMasterClient)
                {
                    if (check_OthersFold_OneWin() > 0)
                    {
                        int winnerActorNumber = check_OthersFold_OneWin();
                        GamePlayer winner = getGamePlayerByActorNumber(winnerActorNumber);
                        winner.photonView.RPC("winnerSetMoney", RpcTarget.All, 1000000);
                        Debug.Log("Player " + winnerActorNumber + " win!");
                        InfoText.text = "Player " + winnerActorNumber + " win!";
                        return;
                    }
                    object playerState;
                    if (otherPlayer.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
                    {
                        if ((int)playerState == GameInfo.PLAYER_MAKING_CHOICE || (int)playerState == GameInfo.PLAYER_MAKING_CHOICE_FOLD_OR_SOHA)
                        {
                            //next player make choice
                            //he is the first one,other player should be Fold or WaitForMakeChoice
                            //he is the last one , we should continue game 
                            int signal = checkPlayerMakeChoice();
                            if (signal == 1)
                            {
                                chooseNextPlayerToMakeChoice(true);
                            }
                            else if (signal == 2 || signal == 3)
                            {
                                chooseNextPlayerToMakeChoice(false);
                            }
                            else
                            {
                                GameManage();
                            }
                        }
                        else
                        {
                            Debug.Log("run game manage");
                            GameManage();
                        }
                    }
                }
            }
        }
        else
        {
            OnClick_Restart();
        }*/


    }
    /*public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            InfoText.text = "Im new master.";
            GameManage();
        }
    }*/
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(SeedPropertiesKey))
        {
            object seed;
            if (propertiesThatChanged.TryGetValue(SeedPropertiesKey, out seed))
                Debug.Log("Update Seed:" + (int)seed);
        }
        /*if (propertiesThatChanged.ContainsKey(RoomState))
        {
            object state;
            if (propertiesThatChanged.TryGetValue(RoomState, out state))
            {
                InfoText.text = GameState.instance.getRoomStateString((int)state);
                Debug.Log(GameState.instance.getRoomStateString((int)state));
            }
        }*/
    }
    public void SetRoom_RandomSeed()
    {
        int Seed = getSeed();
        Hashtable properties = new Hashtable
                {
                    {SeedPropertiesKey, Seed},
                };
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    public void SetRoomRound()
    {
        Hashtable properties = new Hashtable
                {
                    {RoomRound, this.Round},
                };
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    public void OnClick_Restart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
            GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
            int playerCount = playerList.Length;

            for (int i = 0; i < playerCount; i++)
            {
                GamePlayer gp = playerList[i];
                gp.photonView.RPC("reStart", RpcTarget.All);
            }
            
        }
    }
    public void Restart()
    {
        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        SceneManager.LoadScene(1);
    }
    public void OnClick_OneMoreGame()
    {
        
        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        SceneManager.LoadScene(1);
    }
    public void OnClick_Leave()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene("StartScene");
    }
    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
    private bool CheckAllPlayerSetMoney()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerMoney;

            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_MONEY, out playerMoney))
            {
                if ((int)playerMoney>0)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
    public void CheckPlayerMoneyAndSetMoney()
    {
        if (CheckAllPlayerSetMoney())
        {
            int minMoney = int.MaxValue;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerMoney;

                if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_MONEY, out playerMoney))
                {
                    if ((int)playerMoney < minMoney)
                    {
                        minMoney = (int)playerMoney;
                    }
                }

                
            }
            this.MoneyForOneGame = minMoney;
        }
    }
    public void CheckPlayerLoadedAndStartGame()
    {
        int startTimestamp;
        int roundOut;
        bool startTimeIsSet = TestTimer.TryGetStartTime(out startTimestamp,out roundOut);
        if (CheckAllPlayerLoadedLevel())
        {
            InfoText.text = "All Player Loaded Level...";
            
            GameObject chatter = PhotonNetwork.InstantiateRoomObject("ChatterInGame", Vector3.zero, Quaternion.identity);
            //SetRoom_RandomSeed();
            SetRoomRound();
            GenerateRandomCardSet();
            if (!startTimeIsSet)
            {
                TestTimer.SetStartTime();
            }
        }
        else
        {
            // not all players loaded yet. wait:
            Debug.Log("setting text waiting for players! ", this.InfoText);
            InfoText.text = "Waiting for other players...";
        }
    }
    public int CheckAllPlayerState()
    {
        int lastState = -1;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;

            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                
                if ((int)playerState == GameInfo.PLAYER_FOLD)
                {
                    continue;
                }
                else if (lastState == -1)
                {
                    lastState = (int)playerState;
                }
                else if (lastState != (int)playerState)
                {
                    Debug.Log("different state.Player "+p.ActorNumber+" state  "+ (int)playerState);
                    lastState = -1;
                    break;
                }
            }
            else
            {
                lastState = -1;
                break;
            }
        }

        return lastState;
    }
    public int check_OthersFold_OneWin()
    {
        List<int> stateList = new List<int>();
        int foldCount = 0;
        int winnerId = -1;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;
            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
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
        if (foldCount == PhotonNetwork.PlayerList.Length - 1 && foldCount>0)
        {
            return winnerId;
        }
        return -1;
    }
    public int checkDeliverCard()
    {
        if (checkAllBetOver())
        {
            int cardCount = checkPlayerCardsCount();
            if (cardCount == 5)
            {
                return 5;
            }
            else
            {
                return 1;
            }

        }

        return -1;
    }
    public bool checkAllSoHa()
    {
        Debug.Log("checking soha.");
        List<int> stateList = new List<int>();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;
            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                stateList.Add((int)playerState);
            }
        }
        for (int i = 0; i < stateList.Count; i++)
        {
            if (stateList[i] == GameInfo.PLAYER_FOLD)
            {
                continue;
            }
            else if(stateList[i]!=GameInfo.PLAYER_SOHA)
            {
                Debug.Log("checking soha false.");
                return false;
            }
        }
        return true;
    }
    public bool checkAllBetOver()
    {
        Debug.Log("checking all bet over.");
        List<int> stateList = new List<int>();
        int raiseBetCount = 0;
        int addBetCount = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;
            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                stateList.Add((int)playerState);
            }
        }
        for (int i = 0; i < stateList.Count; i++)
        {//有一个bet，其他都是call或者fold
            if (stateList[i] == GameInfo.PLAYER_FOLD )
            {
                continue;
            }
            else if (stateList[i] == GameInfo.PLAYER_RAISE_BET)
            {
                raiseBetCount++;
            }
            else if (stateList[i] == GameInfo.PLAYER_ADD_BET)
            {
                addBetCount++;
            }
            else if (stateList[i] != GameInfo.PLAYER_CALL)
            {
                Debug.Log("checking all bet over false.");
                return false;
            }
        }
        if (raiseBetCount > 1 || addBetCount > 1 ||(raiseBetCount >0 && addBetCount>0))
        {
            return false;
        }
        return true;
    }
    public int checkPlayerCardsCount()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = 2;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].State != GameInfo.PLAYER_FOLD)
            {
                playerCount = playerList[i].cardList.Count;
            }
        }
        

        return playerCount;
    }
    public bool checkPlayerCardsCountSame()
    {
        GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
        int playerCount = -1;
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].State != GameInfo.PLAYER_FOLD)
            {
                if (playerCount == -1)
                {
                    playerCount = playerList[i].cardList.Count;
                }
                else if (playerCount != playerList[i].cardList.Count)
                {
                    return false;
                }
            }
        }


        return true;
    }
    public bool checkSomeOneMakingChioce()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;

            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                if ((int)playerState == GameInfo.PLAYER_MAKING_CHOICE
                    || (int)playerState == GameInfo.PLAYER_MAKING_CHOICE_FOLD_OR_SOHA)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void ShowRankCanvas(int money,bool winner,int score)
    {
        /*int myMoney = -1;
        myMoney = PlayerPrefs.GetInt(playerMoneyPrefKey);*/
        int myMoney = MoneyBoardManager.instance.MyCurrentMoney;
        if (myMoney < 2000)
        {
            OneMoreGameButton.gameObject.SetActive(false);
            OutOfMoneyText.gameObject.SetActive(true);
        }
        else
        {
            OneMoreGameButton.gameObject.SetActive(true);
            OutOfMoneyText.gameObject.SetActive(false);
        }
        if (winner)
        {
            WinOrLose.text = "You Win";
        }
        else
        {
            WinOrLose.text = "You Lose";
        }
        //int addMoney = money - MoneyForOneGame;
        if (winner)
        {
            //RankMoneyText.text = "-" + MoneyForOneGame + "\r\n" + "+" + money + "\r\n" + "+" + addMoney;
            RankMoneyText.text = "+" + money;
            RankScoreText.text = "+" + score;
        }
        else
        {
            //RankMoneyText.text = "-" + MoneyForOneGame + "\r\n" + "+" + money + "\r\n" + addMoney;
            RankMoneyText.text = "-" + money;
            RankScoreText.text = "-" + score;
            
        }
        Hashtable propsLoaded = new Hashtable
            {
                {GamePlayer.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsLoaded);
        RankCanvas.gameObject.SetActive(true);
    }
    public int checkPlayerMakeChoice()
    {
        int signal = -1;//1 for first make choice,2 for follow make choice,3 for fold or soha,4 for continue game
        int raise_bet_count = 0;
        int add_bet_count = 0;
        int wait_for_choice_count = 0;
        int wait_for_choice_fold_or_soha_count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerState;

            if (p.CustomProperties.TryGetValue(GamePlayer.PLAYER_STATE, out playerState))
            {
                if ((int)playerState == GameInfo.PLAYER_FOLD)
                {
                    continue;
                }
                else if ((int)playerState == GameInfo.PLAYER_ADD_BET)
                {
                    add_bet_count++;
                }
                else if ((int)playerState == GameInfo.PLAYER_RAISE_BET)
                {
                    raise_bet_count++;
                }
                else if ((int)playerState == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE)
                {
                    wait_for_choice_count++;
                }
                else if ((int)playerState == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA)
                {
                    wait_for_choice_fold_or_soha_count++;
                }
            }
        }
        if (raise_bet_count > 1) Debug.LogError("Raise bet should be only 1.");
        if (add_bet_count > 1) Debug.LogError("Add bet should be only 1.");
        if (raise_bet_count > 0 && add_bet_count>0) Debug.LogError("Raise bet and Add bet shouldn't appare at same time.");
        if (wait_for_choice_count > 0 && wait_for_choice_fold_or_soha_count > 0) Debug.LogError("wait for choice and soha shouldn't appare at same time.");
        if ((raise_bet_count == 1 || add_bet_count==1) && wait_for_choice_count > 0) signal = 2;
        else if (raise_bet_count == 0 && add_bet_count == 0 && wait_for_choice_count > 0) signal = 1;
        else if (raise_bet_count == 0 && add_bet_count == 0 && wait_for_choice_fold_or_soha_count > 0) signal = 3;
        else if (wait_for_choice_count == 0) signal = 4;
        else Debug.LogError("Something wrong here.");
        return signal;
        
    }
    public void GenerateRandomCardSet()
    {
        Vector3 position = new Vector3(0, 0.0f, 0);
        GameObject cardSet=PhotonNetwork.InstantiateRoomObject("CardSet", position, Quaternion.identity);
        int Seed = getSeed();
        cardSet.GetComponent<PhotonView>().RPC("initSoHaCards", RpcTarget.AllViaServer, Seed);
        this.CardSet = cardSet;
    }
    void StartGame()
    {
        InfoText.text = "StartGame";
        CardsTypeText.gameObject.SetActive(true);
        WaitingCanvas.gameObject.SetActive(false);
        Vector3 position = new Vector3(0, 0.0f, 0);
        GameObject player = PhotonNetwork.Instantiate("GamePlayer", position, Quaternion.identity);
        Debug.Log(player.name);
        this.CardSet=GameObject.FindObjectsOfType<CardSet>()[0].gameObject;
        GamePlayer[] PlayerList = GameObject.FindObjectsOfType<GamePlayer>();
    }

    private void CountdownTimerEnd()
    {
        StartGame();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

