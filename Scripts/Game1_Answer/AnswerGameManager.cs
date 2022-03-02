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

public class AnswerGameManager : MonoBehaviourPunCallbacks
{
    public static AnswerGameManager instance;
    

    private List<Steamworks.Ugc.Item> AllItemList;
    private List<string> itemIdList;
    private List<Steamworks.Ugc.Item> RandomItemList;
    private GameObject questionSet;

    public static event Action<float> DownloadProgress;
    private bool GenerateQuestionSetFinished=false;
    public bool isLoaded = false;
    public bool isGenerated = false;
    public bool isReady = false;
    public bool gameEnd = false;

    public int QUESTION_NUMBER = 10;
    private const string Room_State = "RoomState";
    public string PLAYER_QUESTION_GENERATED = "playerQuestionGenerated";
    public string PLAYER_ANSWER_OVER = "playerAnswerOver";
    public string QUESTION_END = "QuestionEnd";
    public string PLAYER_ANSWER_CORRECT = "playerAnswerCorrect";
    public string PLAYER_READY = "playerReady";
    public string PLAYER_EXIT = "playerExit";

    public string QuestionTag = "default";
    public string QuestionLang;

    private const int ROOM_STATE_QUESTION_SET_GENERATED = 101;
    private const int ROOM_STATE_PLAYER_READY = 102;
    public Vector3 chatterPosition = new Vector3(0,0, 0);
    public Text info;

    public AudioSource GoodGoodAudio;
    public AudioSource ZhaXinLeAudio;
    public AudioSource JustThisAudio;
    public AudioSource YouAreNothingAudio;
    public AudioSource SomethingBadAudio;
    public AudioSource NotBadAudio;
    public AudioSource MoreWorkAudio;
    public AudioSource WatchQuestionAudio;
    public AudioSource WinAudio;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.PlayerList.Length ==1)
        {
            AnswerUIManager.instance.CloseWaitingCanvas();
        }
        AllItemList = new List<Steamworks.Ugc.Item>();
        RandomItemList = new List<Steamworks.Ugc.Item>();
        //QuestionLang = SettingsManager.instance.GetCurrentLang();
        QuestionTag = TagManager.instance.TagOptions_English[TagManager.instance.CurrentGameTagValue];
        Hashtable props = new Hashtable
            {
                {GamePlayer.PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
        //showQuestion();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Player " + targetPlayer.NickName + " Prop Update");

        if (changedProps.ContainsKey(PLAYER_ANSWER_CORRECT))
        {
            object correct;
            if (changedProps.TryGetValue(PLAYER_ANSWER_CORRECT, out correct))
            {
                AnswerUIManager.instance.setPlayerCorrectOrWrong(targetPlayer, (bool)correct);
            }

        }

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        

        if (CheckAllPlayerLoadedLevel() && !isLoaded)
        {
            isLoaded = true;
            GameObject chatter = PhotonNetwork.InstantiateRoomObject("ChatterInGame", chatterPosition, Quaternion.identity);
            
            GenerateQuestionSet();
        }
        else if (CheckAllPlayerQuestionGenerated() && ! isGenerated)
        {
            isGenerated = true;
            Hashtable props = new Hashtable
            {
                {Room_State,ROOM_STATE_QUESTION_SET_GENERATED}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        else if (CheckAllPlayerReady() && !isReady)
        {
            isReady = true;
            Hashtable props = new Hashtable
            {
                {Room_State,ROOM_STATE_PLAYER_READY}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        else if (CheckAllPlayerAnswerOver() && !gameEnd)
        {

            SetNextQuestion();
        }
        else
        {
            // not all players loaded yet. wait:
            Debug.Log("setting text waiting for players! ");
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("Player Left");
        AnswerUIManager.instance.DeletePlayer(otherPlayer.ActorNumber);
        Hashtable props = new Hashtable
            {
                {PLAYER_EXIT, true}
            };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        OnPlayerPropertiesUpdate(PhotonNetwork.LocalPlayer, props);
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey(Room_State))
        {
            object roomState;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(Room_State, out roomState))
            {
                if ((int)roomState == ROOM_STATE_QUESTION_SET_GENERATED)
                {
                    AnswerUIManager.instance.CloseWaitingCanvas();
                    isGenerated = true;
                    this.questionSet = GameObject.FindObjectsOfType<QuestionSet>()[0].gameObject;
                    this.RandomItemList = questionSet.GetComponent<QuestionSet>().RandomItemList;
                    if (this.RandomItemList.Count == 0)
                    {
                        Debug.LogError("something wrong with RandomItemList");
                        return;
                    }
                    info.text = "ROOM_STATE_QUESTION_SET_GENERATED CheckItemsAndStartGame";
                    CheckItems();
                }
                else if ((int)roomState == ROOM_STATE_PLAYER_READY)
                {
                    isReady = true;
                    Debug.Log("RoomState ROOM_STATE_PLAYER_READY");
                    StartGame();
                }
            }
        }
        else if (propertiesThatChanged.ContainsKey(QUESTION_END))
        {
            object questionEnd;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(QUESTION_END, out questionEnd))
            {
                if ((bool)questionEnd)
                {

                    info.text = "Game End";
                    EndGame();
                }
            }
        }
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

    private bool CheckAllPlayerQuestionGenerated()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerQuestionGenerated;

            if (p.CustomProperties.TryGetValue(PLAYER_QUESTION_GENERATED, out playerQuestionGenerated))
            {
                if ((bool)playerQuestionGenerated)
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
            object playerReady;

            if (p.CustomProperties.TryGetValue(PLAYER_READY, out playerReady))
            {
                if ((bool)playerReady)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
    private bool CheckAllPlayerAnswerOver()
    {
        Debug.Log("check CheckAllPlayerAnswerOver");
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerAnswerOver;

            if (p.CustomProperties.TryGetValue(PLAYER_ANSWER_OVER, out playerAnswerOver))
            {
                Debug.Log("Player " + p.ActorNumber + " " + p.NickName + " answer over?" + (bool)playerAnswerOver);
                if ((bool)playerAnswerOver)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
    public void EndGame()
    {
        Debug.Log("GAME END");
        gameEnd = true;
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        Hashtable propsLoaded = new Hashtable
            {
                {GamePlayer.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propsLoaded);

        AnswerUIManager.instance.SetPlayerRank();
        AnswerUIManager.instance.RankCanvas.gameObject.SetActive(true);
        PlayEndGameAudio();
    }
    public void PlayEndGameAudio()
    {
        if (AnswerPlayerManager.instance.AnswerCorrectNum == 0)
        {
            System.Random ran = new System.Random((int)System.DateTime.Now.Ticks);
            float r = ran.Next(0, 1);
            if (r <= 0.25)
            {
                ZhaXinLeAudio.Play();
            }
            else if (r > 0.25 && r <= 0.5)
            {
                YouAreNothingAudio.Play();
            }
            else if (r > 0.5 && r <= 0.75)
            {
                SomethingBadAudio.Play();
            }
            else
            {
                JustThisAudio.Play();
            }
        }
        else if (AnswerPlayerManager.instance.AnswerCorrectNum == 1)
        {
            MoreWorkAudio.Play();
        }
        else if (AnswerPlayerManager.instance.AnswerCorrectNum == 2)
        {
            MoreWorkAudio.Play();
        }
        else if (AnswerPlayerManager.instance.AnswerCorrectNum == 3)
        {
            NotBadAudio.Play();
        }
        else if (AnswerPlayerManager.instance.AnswerCorrectNum == 4)
        {
            GoodGoodAudio.Play();
        }
        else if (AnswerPlayerManager.instance.AnswerCorrectNum >= 5)
        {
            WinAudio.Play();
        }
    }
    public void OnClick_OneMoreGame()
    {

        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        SceneManager.LoadScene(2);
    }
    public void StartGame()
    {
        info.text = "Start Game";
        AnswerUIManager.instance.CloseWaitingCanvas();
        AnswerUIManager.instance.CloseWaitForDownloadCanvas();
        SetOtherPlayers();
        SetNextQuestion();
        //AnswerUIManager.instance.SetActive_AnswerCanvas();
        Debug.Log("StartGame");
    }
    private void SetOtherPlayers()
    {
        AnswerUIManager.instance.SetPlayers();
    }
    private void SetNextQuestion()
    {
        Debug.Log("SetNextQuestion");
        if (PhotonNetwork.IsMasterClient)
        {
            this.questionSet.GetComponent<QuestionSet>().photonView.RPC("setNextQuestion", RpcTarget.All,PhotonNetwork.ServerTimestamp);
        }
        
    }
    private bool CheckItemsInstalled()
    {
        foreach (Steamworks.Ugc.Item item in RandomItemList)
        {
            if (!item.IsSubscribed)
            {
                return false;
            }
        }
        return true;
    }
    private void CheckItems()
    {
        if (CheckItemsInstalled())
        {
            Hashtable props = new Hashtable
            {
                {PLAYER_READY, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            AnswerUIManager.instance.SetActive_WaitForDownloadCanvas();
            info.text = "Waiting for others to ready";
        }
        else
        {
            AnswerUIManager.instance.SetActive_DialogCanvas();
        }
    }
    public void DownloadItemsAndStartGame()
    {
        DownloadQuestionsAsync();
    }

    private void GenerateQuestionSet()
    {
        int seed = GetRandomSeed();
        GameObject questionSet = PhotonNetwork.InstantiateRoomObject("QuestionSet", Vector3.zero, Quaternion.identity);
        questionSet.GetComponent<PhotonView>().RPC("init", RpcTarget.All, seed,this.QuestionTag,this.QuestionLang);

    }
    private int GetRandomSeed()
    {
        return (int)System.DateTime.Now.Ticks;
    }
    private async void DownloadQuestionsAsync()
    {
        int count = 0;
        foreach (Steamworks.Ugc.Item entry in RandomItemList)
        {
            count += 1;
            if (!entry.IsSubscribed)
            {
                var sub = await entry.Subscribe();

                var download = await entry.DownloadAsync(
                    DownloadProgress = delegate (float progress)
                    {
                        AnswerUIManager.instance.DownloadingSlider.value = progress;
                        Debug.Log(count+"DownLoading:" + progress);
                    });
                AddTitleDescription(entry.Directory, entry.Title, entry.Description);
            }
            
        }
        Hashtable props = new Hashtable
            {
                {PLAYER_READY, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        info.text = "Waiting for others to ready";

    }
    public void AddTitleDescription(string Directory, string Title, string Description)
    {
        QuestionData questionData = AnswerFileManager.instance.readJson(Directory);
        questionData.title = Title;
        questionData.description = Description;
        AnswerFileManager.instance.writeJson(questionData, Directory);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        Debug.Log("OnLeftRoom");
        SceneManager.LoadScene("StartScene");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
