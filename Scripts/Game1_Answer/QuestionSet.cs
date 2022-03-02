using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class QuestionSet : MonoBehaviour
{
    public static QuestionSet instance;

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
    public RawImage rawImage;

    


    public PhotonView photonView;
    private List<Steamworks.Ugc.Item> AllItemList;
    public List<Steamworks.Ugc.Item> RandomItemList;

    
    private const int QUESTION_NUMBER = 10;
    public string PLAYER_QUESTION_GENERATED = "playerQuestionGenerated";
    public string PLAYER_ANSWER_OVER = "playerAnswerOver";
    public string PLAYER_ANSWER_CORRECT = "playerAnswerCorrect";
    public string QUESTION_END = "QuestionEnd";
    public const string QUESTION_TAG = "default";

    private int currentIndex = 0;
    private void Awake()
    {
        AllItemList = new List<Steamworks.Ugc.Item>();
        RandomItemList = new List<Steamworks.Ugc.Item>();
        instance = this;
        photonView = GetComponent<PhotonView>();
        
    }
   
    public async void initItemListAsync(int seed,string tag,string lang)
    {
        var q = Steamworks.Ugc.Query.Items.WithTag(tag).WithTag(lang).WithoutTag("r-18+");
        //var q = Steamworks.Ugc.Query.Screenshots.CreatedByFollowedUsers();
        int pagenum = 1;
        while (true)
        {
            var page = await q.GetPageAsync(pagenum);
            Debug.Log($"page : {pagenum}");
            if (page.HasValue)
            {
                Debug.Log($"This page has {page.Value.ResultCount}");
                if (page.Value.ResultCount == 0)
                {
                    break;
                }
                foreach (Steamworks.Ugc.Item entry in page.Value.Entries)
                {
                    Debug.Log("add entry " + entry.Title + " , installed:" + entry.IsInstalled);

                    AllItemList.Add(entry);
                }
                pagenum++;
            }
            else
            {
                break;
            }
        }
        RandomItemList = GetRandomItem(AllItemList.Count, QUESTION_NUMBER,seed);
        Hashtable props = new Hashtable
            {
                {PLAYER_QUESTION_GENERATED, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }

    public List<Steamworks.Ugc.Item> GetRandomItem(int total, int count,int seed)
    {
        System.Random ran = new System.Random(seed);
        List<Steamworks.Ugc.Item> ret = new List<Steamworks.Ugc.Item>();
        int[] sequence = new int[total];
        int[] index = new int[count];
        for (int i = 0; i < total; i++)
        {
            sequence[i] = i;
        }
        int end = total - 1;
        for (int i = 0; i < count; i++)
        {
            int num = ran.Next(0, end + 1);
            index[i] = sequence[num];
            sequence[num] = sequence[end];
            end--;
        }
        for (int i = 0; i < count; i++)
        {
            int ind = index[i];
            ret.Add(AllItemList[ind]);
        }
        return ret;
    }
    public List<string> GetItemIdList(List<Steamworks.Ugc.Item> itemList)
    {
        List<string> ret = new List<string>();
        foreach (Steamworks.Ugc.Item item in itemList)
        {
            ret.Add(item.Id.ToString());
        }
        return ret;
    }

    
    

    [PunRPC]
    public void init(int seed,string tag,string lang)
    {
        AnswerGameManager.instance.isLoaded = true;
        initItemListAsync(seed,tag,lang);
    }
    [PunRPC]
    public void setNextQuestion(int startTime)
    {
        if (currentIndex == QUESTION_NUMBER)
        {
            //All Question End
            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable roomProps = new Hashtable
                {
                    {QUESTION_END, true}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            }
            
            return;
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(PLAYER_ANSWER_CORRECT))
        {
            Debug.Log("Remove PLAYER_ANSWER_CORRECT");
            PhotonNetwork.LocalPlayer.CustomProperties.Remove(PLAYER_ANSWER_CORRECT);
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(PLAYER_ANSWER_OVER))
        {
            Debug.Log("Reset PLAYER_ANSWER_OVER");
            Hashtable props = new Hashtable
            {
                {PLAYER_ANSWER_OVER, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        Debug.Log("questionset reveive rpc setNextQuestion");
        Steamworks.Ugc.Item item = RandomItemList[currentIndex];
        AnswerUIManager.instance.setNextQuestion(item,startTime);
        currentIndex++;
        
    }
    

    void Update()
    {
        
    }

}
