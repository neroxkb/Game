using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class UnderCoverGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    // Start is called before the first frame update
    public static UnderCoverGameManager instance;

    public string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public string PLAYER_READY = "PlayerReady";
    public string PLAYER_VOTE_OVER = "PlayerVoteOver";

    public const byte SET_PLAYER_EVENT = 1;
    public const byte START_GAME_EVENT = 2;
    public const byte PLAYER_STATEMENT_EVENT = 3;
    public const byte SET_STATEMENT_EVENT = 4;
    public const byte VOTE_ROUND_EVENT = 5;
    public const byte VOTE_OVER_EVENT = 6;
    public const byte UNDERCOVER_WIN_EVENT = 7;
    public const byte CIVILIAN_WIN_EVENT = 8;
    public const byte VOTE_EVENT = 101;

    public AudioSource WinAudio;
    public AudioSource LoseAudio;

    public int[] ActorNumbers;
    public int CurrentActorNumber;
    public int UnderCoverBonus = 0;
    public int CivilianBonus = 0;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    void Start()
    {
        if (PhotonNetwork.PlayerList.Length < 3)
        {
            UnderCoverUIManager.instance.CloseWaitingCanvas();
            UnderCoverUIManager.instance.ShowNotEnoughPlayerCanvas();
            return;
        }
        int PlayersNum = PhotonNetwork.PlayerList.Length;
        ActorNumbers = GetActorNumbers();
        CurrentActorNumber = 0;
        CivilianBonus = PlayersNum * 1000;
        UnderCoverBonus = PlayersNum * 1000;
        Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        /*if (PhotonNetwork.IsMasterClient)
        {
            OnClick_OneMoreGame();
        }*/
        if (PhotonNetwork.PlayerList.Length < 3 && UnderCoverUIManager.instance.WaitingCanvas.gameObject.activeSelf)
        {
            UnderCoverUIManager.instance.CloseWaitingCanvas();
            UnderCoverUIManager.instance.ShowNotEnoughPlayerCanvas();
            return;
        }
        else if(!UnderCoverUIManager.instance.EndCanvas.gameObject.activeSelf)
        {
            OnClick_OneMoreGame();
        }

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        /*if (changedProps.ContainsKey(PLAYER_DEAD))
        {
            CheckEndOfGame();
            return;
        }*/
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }


        // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
        

        if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                int UnderCoverActorNumber = GetRandomActorNumber();
                string[] WordPair = GetRandomWordPair();
                string UnderCoverWord = WordPair[0];
                string CivilianWord = WordPair[1];
                object[] datas = new object[] { UnderCoverActorNumber, UnderCoverWord , CivilianWord };
                PhotonNetwork.RaiseEvent(SET_PLAYER_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
                Debug.Log("RaiseEvent SET_PLAYER_EVENT.UnderCover is "+UnderCoverActorNumber);
                UnderCoverUIManager.instance.SetPlayers(UnderCoverActorNumber, UnderCoverWord, CivilianWord);
                
                GameObject chatter = PhotonNetwork.InstantiateRoomObject("ChatterInGame", Vector3.zero, Quaternion.identity);
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ");
                UnderCoverUIManager.instance.SetInfoText("Waiting for other players...");
            }
        }
        else if (changedProps.ContainsKey(PLAYER_READY))
        {
            if (CheckAllPlayerReady())
            {
                object[] datas = new object[] { };
                PhotonNetwork.RaiseEvent(START_GAME_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
                Debug.Log("RaiseEvent PLAYER_READY.");
                StartGame();
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ");
                UnderCoverUIManager.instance.SetInfoText("Waiting for other players Ready");
            }
        }
        else if (changedProps.ContainsKey(PLAYER_VOTE_OVER))
        {
            if (CheckAllPlayerVoteOver())
            {
                object[] datas = new object[] { };
                PhotonNetwork.RaiseEvent(VOTE_OVER_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
                Debug.Log("RaiseEvent VOTE_OVER_EVENT.");
                VoteOver();
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ");
                UnderCoverUIManager.instance.SetInfoText("Waiting for other players Vote");
            }
        }

    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SET_PLAYER_EVENT)
        {
            object[] datas = (object[])photonEvent.CustomData;
            int UnderCoverActorNumber = (int)datas[0];
            string UnderCoverWord = (string)datas[1];
            string CivilianWord = (string)datas[2];
            Debug.Log("event SET_PLAYER_EVENT received.");
            UnderCoverUIManager.instance.SetPlayers(UnderCoverActorNumber, UnderCoverWord, CivilianWord);
        }
        else if (photonEvent.Code == START_GAME_EVENT)
        {
            StartGame();
        }
        else if (photonEvent.Code == SET_STATEMENT_EVENT)
        {
            object[] datas = (object[])photonEvent.CustomData;
            int ActorNumber = (int)datas[0];
            string statement = (string)datas[1];
            Debug.Log("event PLAYER_STATEMENT_EVENT received.");
            UnderCoverUIManager.instance.SetStatement(ActorNumber,statement);
            NextPlayerStatement();
        }
        else if (photonEvent.Code == VOTE_ROUND_EVENT)
        {
            object[] datas = (object[])photonEvent.CustomData;
            int startTimestamp = (int)datas[0];
            Debug.Log("event VOTE_ROUND_EVENT received.");
            UnderCoverUIManager.instance.BeginVoteRound(startTimestamp);
        }
        else if (photonEvent.Code == VOTE_EVENT)
        {
            object[] datas = (object[])photonEvent.CustomData;
            int ActorNumber = (int)datas[0];
            Debug.Log("event VOTE_EVENT received.");
            UnderCoverUIManager.instance.SetVote(ActorNumber);
        }
        else if (photonEvent.Code == VOTE_OVER_EVENT)
        {
            UnderCoverUIManager.instance.SetInfoText("event VOTE_OVER_EVENT received.");
            Debug.Log("event VOTE_OVER_EVENT received.");
            VoteOver();
        }
    }
    public int[] GetActorNumbers()
    {
        int PlayersNum = PhotonNetwork.PlayerList.Length;
        int[] ActorNumberArray = new int[PlayersNum];
        for (int i = 0; i < PlayersNum; i++)
        {
            ActorNumberArray[i] = PhotonNetwork.PlayerList[i].ActorNumber;
        }
        return ActorNumberArray;
    }
    public int GetSeed()
    {
        return (int)System.DateTime.Now.Ticks;
    }
    public int GetRandomActorNumber()
    {
        int nums = ActorNumbers.Length;
        int seed = GetSeed();
        System.Random ran = new System.Random(seed);
        int randomIndex = ran.Next(0, nums);
        Debug.Log("UnderCover is " + ActorNumbers[randomIndex]);
        return ActorNumbers[randomIndex];
    }
    public string[] GetRandomWordPair()
    {
        int seed = GetSeed();
        string lang = SettingsManager.instance.GetCurrentLang();
        string[] WordPairs = GetWordPairsFromLocalFile(lang);
        if (WordPairs != null)
        {
            System.Random ran = new System.Random(seed);
            int nums = WordPairs.Length;
            int randomIndex = ran.Next(0, nums-1);
            Debug.Log("WordPairsNum:" + nums);
            string WordPair = WordPairs[randomIndex];
            string[] Words = WordPair.Split(',');
            Debug.Log("WordPair:" + WordPair + "WordsNum" + Words.Length);
            float r = ran.Next(0, 1);
            string[] RandomWords = new string[2];
            if (r > 0.5)
            {
                RandomWords[0] = Words[0];
                RandomWords[1] = Words[1];
            }
            else
            {
                RandomWords[0] = Words[1];
                RandomWords[1] = Words[0];
            }

            Debug.Log("RandomWords is " + RandomWords);
            return RandomWords;
        }
        else
        {
            return null;
        }
        
    }
    public string[] GetWordPairsFromLocalFile(string lang)
    {
        string WordDataPath = Application.streamingAssetsPath + "/WordPairSet/WordPairSet_" + lang + ".txt";
        if (File.Exists(WordDataPath))
        {
            print(WordDataPath);
            string textData = File.ReadAllText(WordDataPath);
            string[] WordPairs = textData.Split(';');
            return WordPairs;
        }
        else
        {
            return null;
        }
    }
    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(PLAYER_LOADED_LEVEL, out playerLoadedLevel))
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
    private bool CheckAllPlayerReady()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(PLAYER_READY, out playerLoadedLevel))
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
    private bool CheckAllPlayerVoteOver()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(PLAYER_VOTE_OVER, out playerLoadedLevel))
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
    public void StartGame()
    {
        int startTimestamp;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);
        UnderCoverUIManager.instance.StartGameCountDown(startTimestamp);
    }
    public void StatementRound()
    {
        NextPlayerStatement();
    }
    public void VoteRound()
    {
        CurrentActorNumber = 0;
        UnderCoverUIManager.instance.SetInfoText("VoteRound");
        int startTimestamp;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);
        object[] datas = new object[] { startTimestamp };
        PhotonNetwork.RaiseEvent(VOTE_ROUND_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
        UnderCoverUIManager.instance.BeginVoteRound(startTimestamp);
    }
    public void VoteOver()
    {
        UnderCoverUIManager.instance.VoteOver();
    }
    public void NextPlayerStatement()
    {

        if (CurrentActorNumber >= ActorNumbers.Length)
        {
            VoteRound();
            return;
        }
        UnderCoverUIManager.instance.SetInfoText("Player " + ActorNumbers[CurrentActorNumber] + "will statement");
        if (PhotonNetwork.LocalPlayer.ActorNumber == ActorNumbers[CurrentActorNumber])
        {
            if (!UnderCoverPlayerManager.instance.IsDead)
            {
                CurrentActorNumber += 1;
                int startTimestamp;
                bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);
                UnderCoverUIManager.instance.PlayerMakeStatement(startTimestamp);
                
            }
            else
            {
                CurrentActorNumber += 1;
                NextPlayerStatement();
            }
        }
        else
        {

            if (UnderCoverUIManager.instance.CheckPlayerDead(ActorNumbers[CurrentActorNumber]))
            {
                CurrentActorNumber += 1;
                NextPlayerStatement();
            }
            else
            {
                CurrentActorNumber += 1;
            }
        }
        
    }
    public void CheckUnderCoverWin()
    {
        bool IsUnderCoverWin=UnderCoverUIManager.instance.CheckUnderCoverWin();
        if (IsUnderCoverWin)
        {
            UnderCoverWin();
        }
        else
        {
            CivilianBonus -= 1000;
            UnderCoverUIManager.instance.BeginStatementRound();
        }
    }
    public void CivilianWin()
    {
        //PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        Hashtable propsLoaded = new Hashtable
            {
                {GamePlayer.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsLoaded);
        UnderCoverUIManager.instance.ShowAllIdentity();
        UnderCoverUIManager.instance.SetInfoText("CivilianWin");
        UnderCoverUIManager.instance.EndCanvas.gameObject.SetActive(true);
        if (UnderCoverPlayerManager.instance.IsUnderCover)
        {
            UnderCoverUIManager.instance.Lose.gameObject.SetActive(true);
            UnderCoverUIManager.instance.BonusMoneyText.text = "" + 0;
            LoseAudio.Play();
        }
        else
        {
            UnderCoverUIManager.instance.AddMoney(CivilianBonus);
            UnderCoverUIManager.instance.Win.gameObject.SetActive(true);
            UnderCoverUIManager.instance.BonusMoneyText.text = "" + CivilianBonus;
            WinAudio.Play();
        }
    }
    public void UnderCoverWin()
    {
        //PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        Hashtable propsLoaded = new Hashtable
            {
                {GamePlayer.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsLoaded);
        UnderCoverUIManager.instance.ShowAllIdentity();
        UnderCoverUIManager.instance.SetInfoText("UnderCoverWin");
        UnderCoverUIManager.instance.EndCanvas.gameObject.SetActive(true);
        if (UnderCoverPlayerManager.instance.IsUnderCover)
        {
            UnderCoverUIManager.instance.AddMoney(UnderCoverBonus);
            UnderCoverUIManager.instance.Win.gameObject.SetActive(true);
            UnderCoverUIManager.instance.BonusMoneyText.text = "" + UnderCoverBonus;
            WinAudio.Play();
        }
        else
        {
            UnderCoverUIManager.instance.Lose.gameObject.SetActive(true);
            UnderCoverUIManager.instance.BonusMoneyText.text = "" + 0;
            LoseAudio.Play();
        }
    }

    public void OnClick_OneMoreGame()
    {

        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        SceneManager.LoadScene(3);
    }

}
