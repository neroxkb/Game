using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class AnswerUIManager : MonoBehaviour
{
    public static AnswerUIManager instance;
    const string playerMoneyPrefKey = "PlayerMoney";
    public Canvas AnswerCanvas;
    public Canvas ImageSwitchCanvas;
    public Canvas PlayerListCanvas;
    public Canvas DialogCanvas;
    public Canvas DownloadingCanvas;
    public Canvas VideoPlayerCanvas;
    public Canvas ImagePlayerCanvas;
    public Canvas TableCanvas;
    public Canvas RankCanvas;
    public Canvas WaitingCanvas;
    public Canvas WaitForDownloadCanvas;
    public Slider DownloadingSlider;
    public RawImage videoPlayerImage;
    public GameObject AnswerGamePlayerPrefab;
    public GameObject ButtonA_On;
    public GameObject ButtonA_Off;
    public GameObject ButtonB_On;
    public GameObject ButtonB_Off;
    public GameObject ButtonC_On;
    public GameObject ButtonC_Off;
    public GameObject ButtonD_On;
    public GameObject ButtonD_Off;
    public GameObject Table_base;
    public GameObject Table_red;
    public GameObject Table_green;
    public Button ButtonA;
    public Button ButtonB;
    public Button ButtonC;
    public Button ButtonD;
    const string playerSteamId = "PlayerSteamId";

    public string path;
    public Text QuestionText;
    public Text AText;
    public Text BText;
    public Text CText;
    public Text DText;
    public bool correctA;
    public bool correctB;
    public bool correctC;
    public bool correctD;
    public string QuestionObjectNmae;
    public string correctAnswer;

    public VideoPlayer videoPlayer;
    private bool isPlay;

    public RawImage ImagePlayer;
    public bool ImagePlaying = false;
    public float ImagePlayCountDownTime = 5f;
    public bool ImageType = false;

    private const int TYPE_VIDEO = 1;
    private const int TYPE_IMAGE = 2;

    //Timer
    public int countDownTime = 2;
    public int startTime;
    public bool countDownTimeRunning = false;
    public Canvas CountDownTimeCanvas;
    public Text countDownText;

    public int PlayerChoiceStartTime;
    public Slider PlayerChoiceCountDownSlider;
    public bool PlayerChoiceTimeRunning;
    public int PlayerChoiceCountDownTime = 60;
    //GamePlayer
    public List<GameObject> gamePlayerList;
    //Sound
    public AudioSource CorrectAudio;
    public AudioSource WrongAudio;
    public string CorrectAudioPath= Application.streamingAssetsPath+"/Audio/CorrectAudio.wav";
    public string WrongAudioPath = Application.streamingAssetsPath + "/Audio/WrongAudio.wav";
    //rank
    public GameObject AnswerPlayerRank;
    public GameObject OthersPlayerRankContent;
    public RawImage MyAvatar;
    public Text MyName;
    public Text MyAnswerNum;
    public Text MyBonusNum;
    //PlayerInfo
    public Text MoneyText;
    //Chatter
    public Vector3 chatterPosition = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    private void OnEnable()
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
        StartCoroutine(SetCorrectAudio());
        StartCoroutine(SetWrongAudio());
        AddMoney(0);
    }
    public void SetAllCanvasNotActive()
    {
        AnswerCanvas.gameObject.SetActive(false);
        TableCanvas.gameObject.SetActive(false);
        //PlayerListCanvas.gameObject.SetActive(false);
        DialogCanvas.gameObject.SetActive(false);
        DownloadingCanvas.gameObject.SetActive(false);
        VideoPlayerCanvas.gameObject.SetActive(false);
        CountDownTimeCanvas.gameObject.SetActive(false);
        ImagePlayerCanvas.gameObject.SetActive(false);
        RankCanvas.gameObject.SetActive(false);
        //ImageSwitchCanvas.gameObject.SetActive(false);
    }
    public void SetActive_AnswerCanvas()
    {
        //start player countdown
        int startTimestamp;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);
        PlayerChoiceCountDown(startTimestamp);
        SetAllCanvasNotActive();
        if (ImageType)
        {
            ImageSwitchCanvas.gameObject.SetActive(true);
        }
        else
        {
            ImageSwitchCanvas.gameObject.SetActive(false);
        }
        AnswerCanvas.gameObject.SetActive(true);
        ReSetTableAndButton();
        TableCanvas.gameObject.SetActive(true);
    }
    public void SetActive_PlayerListCanvas()
    {
        SetAllCanvasNotActive();
        PlayerListCanvas.gameObject.SetActive(true);
    }
    public void SetActive_DialogCanvas()
    {
        SetAllCanvasNotActive();
        DialogCanvas.gameObject.SetActive(true);
    }
    public void SetActive_WaitForDownloadCanvas()
    {
        SetAllCanvasNotActive();
        WaitForDownloadCanvas.gameObject.SetActive(true);
    }
    public void SetActive_DownloadingCanvas()
    {
        SetAllCanvasNotActive();
        DownloadingCanvas.gameObject.SetActive(true);
    }
    public void SetActive_VideoPlayerCanvas()
    {
        SetAllCanvasNotActive();
        VideoPlayerCanvas.gameObject.SetActive(true);
    }
    public void SetActive_ImagePlayerCanvas()
    {
        SetAllCanvasNotActive();
        ImagePlayerCanvas.gameObject.SetActive(true);
    }

    public void SetActive_CountDownTimeCanvas()
    {
        SetAllCanvasNotActive();
        CountDownTimeCanvas.gameObject.SetActive(true);
    }
    public void SwitchImagePlayer()
    {
        Debug.Log("0");
        if (AnswerCanvas.gameObject.activeSelf)
        {
            Debug.Log("1");
            AnswerCanvas.gameObject.SetActive(false);
            ImagePlayerCanvas.gameObject.SetActive(true);
        }
        else if (ImagePlayerCanvas.gameObject.activeSelf)
        {
            Debug.Log("2");
            ImagePlayerCanvas.gameObject.SetActive(false);
            AnswerCanvas.gameObject.SetActive(true);
        }
    }
    public void setPlayerCorrectOrWrong(Player targetPlayer,bool correct)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            return;
        }
        foreach (GameObject gamePlayer in gamePlayerList)
        {
            AnswerGamePlayerPrefab player = gamePlayer.GetComponent<AnswerGamePlayerPrefab>();
            if (player.ActorNumber == targetPlayer.ActorNumber)
            {
                player.setImage(correct);
            }
        }
    }
    public void SetPlayerRank()
    {
        
        SetMyRank();
        List<GameObject> sortPlayer = new List<GameObject>();
        
        for (int i = AnswerGameManager.instance.QUESTION_NUMBER; i >= 0; i--)
        {
            foreach (GameObject gamePlayer in gamePlayerList)
            {
                AnswerGamePlayerPrefab player = gamePlayer.GetComponent<AnswerGamePlayerPrefab>();
                if (player.AnswerCorrectNum == i)
                {
                    sortPlayer.Add(gamePlayer);
                }
            }
        }
        foreach (GameObject gamePlayer in sortPlayer)
        {
            AnswerGamePlayerPrefab player = gamePlayer.GetComponent<AnswerGamePlayerPrefab>();
            GameObject playerRank = Instantiate(AnswerPlayerRank, OthersPlayerRankContent.transform);
            playerRank.GetComponent<AnswerPlayerRankPrefab>().Initialize(player.Avatar, player.PlayerName, player.AnswerCorrectNum);

        }

    }
    public void SetMyRank()
    {
        MyAvatar.texture = AnswerPlayerManager.instance.Avatar.texture;
        MyName.text = SteamClient.Name;
        MyAnswerNum.text = AnswerPlayerManager.instance.AnswerCorrectNum + "/" + AnswerGameManager.instance.QUESTION_NUMBER;
        int bonusNum = AnswerPlayerManager.instance.AnswerCorrectNum * 1000;
        MyBonusNum.text = "" + bonusNum;
        AddMoney(bonusNum);
    }
    public void AddMoney(int addMoney)
    {
        /*int money = -1;
        if (PlayerPrefs.HasKey(playerMoneyPrefKey))
        {
            money = PlayerPrefs.GetInt(playerMoneyPrefKey);
            money += addMoney;
            PlayerPrefs.SetInt(playerMoneyPrefKey, money);
            MoneyText.text = moneyToString(money);
        }*/
        //var mm = MoneyBoardManager.instance.ModifyMoney(addMoney, MoneyText);
        MoneyBoardManager.instance.ModifyMoney(addMoney, MoneyText);

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
    public void CloseWaitForDownloadCanvas()
    {
        WaitForDownloadCanvas.gameObject.SetActive(false);
    }
    public void SetPlayers()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (PhotonNetwork.LocalPlayer == p)
            {
                continue;
            }
            object steamId;

            if (p.CustomProperties.TryGetValue(playerSteamId, out steamId))
            {
                ulong steamIdValue = Convert.ToUInt64((string)steamId);
                SteamId steamID = steamIdValue;
                GameObject gamePlayer = Instantiate(AnswerGamePlayerPrefab, PlayerListCanvas.transform);
                Debug.Log(steamID);
                gamePlayer.GetComponent<AnswerGamePlayerPrefab>().init(steamID, p.ActorNumber);
                gamePlayerList.Add(gamePlayer);
            }
        }
        PlayerListCanvas.gameObject.SetActive(true);

    }
    public void DeletePlayer(int ActorNumber)
    {
        RemoveGamePlayerInList(ActorNumber);
        for (int i = 0; i < PlayerListCanvas.transform.childCount; i++)
        {
            GameObject gamePlayer = PlayerListCanvas.transform.GetChild(i).gameObject;
            if (gamePlayer.GetComponent<AnswerGamePlayerPrefab>().ActorNumber == ActorNumber)
            {
                
                Destroy( PlayerListCanvas.transform.GetChild(i).gameObject);
                break;
            }
        }
       
    }
    public void RemoveGamePlayerInList(int ActorNumber)
    {
        for (int i = 0; i < gamePlayerList.Count; i++)
        {
            if (gamePlayerList[i].GetComponent<AnswerGamePlayerPrefab>().ActorNumber == ActorNumber)
            {
                gamePlayerList.RemoveAt(i);
                break;
            }
        }
    }
    public void reSetPlayersImage()
    {
        foreach (GameObject gamePlayer in gamePlayerList)
        {
            gamePlayer.GetComponent<AnswerGamePlayerPrefab>().reSetImage();
        }
        

    }
    public void SetChatter()
    {
        
    }
    public void ReSetTableAndButton()
    {
        ButtonA_On.SetActive(true);
        ButtonA_Off.SetActive(false);
        ButtonB_On.SetActive(true);
        ButtonB_Off.SetActive(false);
        ButtonC_On.SetActive(true);
        ButtonC_Off.SetActive(false);
        ButtonD_On.SetActive(true);
        ButtonD_Off.SetActive(false);
        Table_base.SetActive(true);
        Table_red.SetActive(false);
        Table_green.SetActive(false);
        ButtonA.interactable = true;
        ButtonB.interactable = true;
        ButtonC.interactable = true;
        ButtonD.interactable = true;
    }
    public void setNextQuestion(Steamworks.Ugc.Item item,int startTime)
    {
        TVManager.instance.TurnTVOff();
        if (!item.IsInstalled)
        {
            Debug.LogError("something wrong here");
            return;
        }
        reSetPlayersImage();
        this.startTime = startTime;
        this.path = item.Directory;
        ImageSwitchCanvas.gameObject.SetActive(false);
        //ShowQuestion();
        AnswerGameManager.instance.WatchQuestionAudio.Play();
        this.countDownTimeRunning = true;
        SetActive_CountDownTimeCanvas();

    }
    public void ShowQuestion()
    {
        TVManager.instance.TurnTVOn();
        QuestionData questionData = AnswerFileManager.instance.readJson(this.path);
        QuestionText.text = questionData.question;
        AText.text = questionData.answerA;
        BText.text = questionData.answerB;
        CText.text = questionData.answerC;
        DText.text = questionData.answerD;
        correctA = questionData.correctA;
        correctB = questionData.correctB;
        correctC = questionData.correctC;
        correctD = questionData.correctD;
        QuestionObjectNmae = questionData.objectName;
        if (!QuestionObjectNmae.Equals(""))
        {
            int ObjectType = CheckObjectType(QuestionObjectNmae);
            if (ObjectType == TYPE_VIDEO)
            {
                ImageType = false;
                Debug.Log("TYPE_VIDEO");
                string videoPath = path + "/" + QuestionObjectNmae;
                print(QuestionObjectNmae);
                videoPlayer.url = videoPath;
                videoPlayer.Play();
                isPlay = true;
                SetActive_VideoPlayerCanvas();
            }
            else if (ObjectType == TYPE_IMAGE)
            {
                Debug.Log("TYPE_IMAGE");
                string imagePath = path + "/" + QuestionObjectNmae;
                print(QuestionObjectNmae);
                StartCoroutine(SetImagePlayer(imagePath));
                
            }
            else
            {
                ImageType = false;
                Debug.Log("TYPE_SIMPLE");
                SetActive_AnswerCanvas();

            }
        }
        else
        {
            ImageType = false;
            Debug.Log("TYPE_SIMPLE");
            SetActive_AnswerCanvas();
        }
        //SetActive_AnswerCanvas();
    }
    
    public int CheckObjectType(string objectNmae)
    {
        if (objectNmae.EndsWith(".mp4")|| objectNmae.EndsWith(".MP4"))
        {
            ImageType = false;
            return TYPE_VIDEO;
        }
        else if (objectNmae.EndsWith(".png") || objectNmae.EndsWith(".PNG") || objectNmae.EndsWith(".jpg") || objectNmae.EndsWith(".JPG"))
        {
            ImageType = true;
            return TYPE_IMAGE;
        }
        else
        {
            ImageType = false;
            return -1;
        }
    }

    
    private IEnumerator SetImagePlayer(string path)
    {

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(@path);
        yield return request.SendWebRequest();
        // AssetBundle ab = DownloadHandlerAssetBundle.GetContent (request );

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            ImageType = false;
            Debug.Log(request.error);
        }
        else
        {
            Texture tt = DownloadHandlerTexture.GetContent(request);

            ImagePlayer.texture = tt;
            ImagePlaying = true;
            SetActive_ImagePlayerCanvas();
            ImagePlayer.gameObject.SetActive(true);
        }

    }
    private IEnumerator SetCorrectAudio()
    {

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(CorrectAudioPath,AudioType.WAV);
        yield return request.SendWebRequest();
        // AssetBundle ab = DownloadHandlerAssetBundle.GetContent (request );

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            DownloadHandlerAudioClip.GetContent(request);
            CorrectAudio.clip= DownloadHandlerAudioClip.GetContent(request);
            Debug.Log("set CorrectAudio");
        }

    }
    private IEnumerator SetWrongAudio()
    {

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(WrongAudioPath, AudioType.WAV);
        yield return request.SendWebRequest();
        // AssetBundle ab = DownloadHandlerAssetBundle.GetContent (request );

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            WrongAudio.clip = DownloadHandlerAudioClip.GetContent(request);
            Debug.Log("set WrongAudio");
        }

    }
    #region PlayerChoiceCountDown
    public void PlayerChoiceCountDown(int startTime)
    {
        PlayerChoiceStartTime = startTime;
        PlayerChoiceCountDownSlider.value = 1;
        PlayerChoiceTimeRunning = true;
    }
    public void StopPlayerChoiceCountDown()
    {
        //PlayerChoiceCountDownSlider.value = 1;
        PlayerChoiceTimeRunning = false;
    }
    private float PlayerChoiceTimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.PlayerChoiceStartTime;
        return this.PlayerChoiceCountDownTime - timer / 1000f;
    }
    private void OnPlayerChoiceTimerEnds()
    {
        this.PlayerChoiceTimeRunning = false;
        AnswerPlayerManager.instance.ChoiceTimeOver();
    }
    public void PlayerChoiceCountDownUpdate()
    {
        if (!this.PlayerChoiceTimeRunning) return;

        float countdown = PlayerChoiceTimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        PlayerChoiceCountDownSlider.value = countdown / PlayerChoiceCountDownTime;
        if (countdown > 0.0f) return;
        OnPlayerChoiceTimerEnds();
    }
    #endregion
    private float TimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.startTime;
        return this.countDownTime - timer / 1000f;
    }
    private void OnTimerEnds()
    {
        this.countDownTimeRunning = false;

        this.countDownText.text = string.Empty;
        CountDownTimeCanvas.gameObject.SetActive(false);
        Debug.Log("Count down end.");
        ShowQuestion();

    }

    private void OnImagePlayTimerEnds()
    {
        this.ImagePlaying = false;
        SetActive_AnswerCanvas();
        ImagePlayCountDownTime = 5f;


    }
    void PlayMovie()
    {
        if (videoPlayer.texture == null)
        {
            return;
        }
        videoPlayerImage.texture = videoPlayer.texture;
        if (isPlay)
        {
            if (videoPlayer.frame == (long)videoPlayer.frameCount - 1)
            {
                //videoPlayerImage.gameObject.SetActive(false);
                videoPlayer.Stop();
                SetActive_AnswerCanvas();
                isPlay = false;

            }
        }
    }
    void PlayImage()
    {
        if (!ImagePlaying)
        {
            return;
        }
        if (ImagePlayer.texture == null)
        {
            return;
        }
        ImagePlayCountDownTime -= Time.deltaTime;
        //Debug.Log(ImagePlayCountDownTime);
        if (ImagePlayCountDownTime > 0.0f) return;
        OnImagePlayTimerEnds();
    }
    // Update is called once per frame
    void Update()
    {
        PlayMovie();
        PlayImage();
        PlayerChoiceCountDownUpdate();
        if (!this.countDownTimeRunning) return;

        float countdown = TimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        //this.countDownText.text = string.Format("{0}", countdown.ToString("n0"));
        //this.countDownText.text = "Next Question";
        if (countdown > 0.0f) return;
        OnTimerEnds();
    }
}
