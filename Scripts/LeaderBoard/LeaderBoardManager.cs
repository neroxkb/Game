using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager instance;
    const string LeaderBoardName = "SohaLeaderBoard";
    public Steamworks.Data.Leaderboard leaderboard;
    public int AllItemCount;
    Steamworks.Data.LeaderboardEntry myEntry;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
    }
    public System.Int32[] GetFirstSubmitDetails()
    {
        System.Int32[] details = new System.Int32[1];
        details[0] = -1;
        return details;
    }
    public async System.Threading.Tasks.Task FirstSubmitScoreAsync()
    {
        Steamworks.Data.Leaderboard? leaderboard = await SteamUserStats.FindLeaderboardAsync(LeaderBoardName);
        Steamworks.Data.LeaderboardUpdate? submit = await leaderboard.Value.SubmitScoreAsync(0);
        Debug.Log("FirstSubmitScore");

    }
    public async System.Threading.Tasks.Task GetLeaderboardContentAsync()
    {
        Steamworks.Data.Leaderboard? leaderboard = await SteamUserStats.FindLeaderboardAsync(LeaderBoardName);
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await leaderboard.Value.GetScoresAsync(10);
        Debug.Log("Get Leaderboard Entrys :" + leaderboardEntry.Length);
        foreach (Steamworks.Data.LeaderboardEntry entry in leaderboardEntry)
        {
            Debug.Log("Rank User:" + entry.User);
            Debug.Log("Rank Score:" + entry.Score);
            Debug.Log("Global Rank:" + entry.GlobalRank);
        }
    }
    
    public async System.Threading.Tasks.Task UI_GetLeaderboardContentByPage(int page)
    {
        Steamworks.Data.Leaderboard? leaderboard = await SteamUserStats.FindLeaderboardAsync(LeaderBoardName);
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await leaderboard.Value.GetScoresAsync(8,(page - 1) * 8 +1);
        Debug.Log("Get Leaderboard Entrys :" + leaderboardEntry.Length);
        
        if (leaderboardEntry.Length > 8)
        {
            Steamworks.Data.LeaderboardEntry[] leaderboardEntry_8 = new Steamworks.Data.LeaderboardEntry[8];
            for (int i = 0; i < 8; i++)
            {
                leaderboardEntry_8[i] = leaderboardEntry[i];
            }
            if (page == LeaderBoardUIManagger.instance.CurrentPage)
            {
                LeaderBoardUIManagger.instance.SetRankList(leaderboardEntry_8);
            }
        }
        else
        {
            LeaderBoardUIManagger.instance.SetRankList(leaderboardEntry);
        }
        
            
        
    }
    public async System.Threading.Tasks.Task UI_GetLeaderboardContentByPageTest(int page)
    {
        Steamworks.Data.Leaderboard? leaderboard = await SteamUserStats.FindLeaderboardAsync(LeaderBoardName);
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await leaderboard.Value.GetScoresAsync(8, 1);
        Debug.Log("Get Leaderboard Entrys :" + leaderboardEntry.Length);
        //LeaderBoardUIManagger.instance.SetRankList(leaderboardEntry);
        //test
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry_test = new Steamworks.Data.LeaderboardEntry[100];
        for (int i = 0; i < 100; i++)
        {
            leaderboardEntry_test[i] = leaderboardEntry[0];
            leaderboardEntry_test[i].Score = i;
        }

        Steamworks.Data.LeaderboardEntry[] leaderboardEntry_c = new Steamworks.Data.LeaderboardEntry[8];
        for (int i = 0; i < 8; i++)
        {
            if(((page - 1) * 8 + i)>=100) break;
            leaderboardEntry_c[i] = leaderboardEntry_test[(page - 1) * 8 + i];
        }
        if(page==LeaderBoardUIManagger.instance.CurrentPage)
        LeaderBoardUIManagger.instance.SetRankList(leaderboardEntry_c);
        /*foreach (Steamworks.Data.LeaderboardEntry entry in leaderboardEntry)
        {
            Debug.Log("Rank User:" + entry.User);
            Debug.Log("Rank Score:" + entry.Score);
            Debug.Log("Global Rank:" + entry.GlobalRank);
        }*/
    }
    public async System.Threading.Tasks.Task UI_SetMyLeaderboard()
    {
        
        Debug.Log("Get My Leaderboard Entrys : Score" + myEntry.Score+"  Rank" + myEntry.GlobalRank);
        LeaderBoardUIManagger.instance.MyScore = myEntry.Score;
        LeaderBoardUIManagger.instance.MyRankNum = myEntry.GlobalRank;
        LeaderBoardUIManagger.instance.MyScoreText.text = myEntry.Score.ToString();
        LeaderBoardUIManagger.instance.MyRankNumText.text = myEntry.GlobalRank.ToString();
        LeaderBoardUIManagger.instance.SetMyMark();

    }
    public async System.Threading.Tasks.Task Query()
    {
        Steamworks.Data.Leaderboard? leaderboard = await SteamUserStats.FindLeaderboardAsync(LeaderBoardName);
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await leaderboard.Value.GetScoresAsync(200);
        int entryCount = leaderboardEntry.Length;
        while (true)
        {
            if (entryCount < 200)
            {
                break;
            }

        }
        Debug.Log("Get Leaderboard Entrys :" + leaderboardEntry.Length);
        foreach (Steamworks.Data.LeaderboardEntry entry in leaderboardEntry)
        {
            Debug.Log("Rank User:" + entry.User);
            Debug.Log("Rank Score:" + entry.Score);
            Debug.Log("Global Rank:" + entry.GlobalRank);
        }
    }
    public async System.Threading.Tasks.Task ModifyScore(int socre)
    {
        int newScore = myEntry.Score + socre;
        Steamworks.Data.LeaderboardUpdate? update = await this.leaderboard.ReplaceScore(newScore);
        myEntry.Score = newScore;

        Debug.Log("ModifyScore To " + newScore);
    }
    
    public async System.Threading.Tasks.Task GetLeaderBoardAsync()
    {
        Steamworks.Data.Leaderboard? leaderboard = await Steamworks.SteamUserStats.FindLeaderboardAsync(LeaderBoardName);
        this.leaderboard =leaderboard.Value;
        
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.leaderboard.GetScoresAroundUserAsync(0, 0);
        if (leaderboardEntry == null)
        {
            Steamworks.Data.LeaderboardUpdate? submit = await leaderboard.Value.SubmitScoreAsync(0);
            Debug.Log("leaderboardEntry is null , First Submit Score");
        }
        this.myEntry = leaderboardEntry[0];

        this.AllItemCount = this.leaderboard.EntryCount;
        
        Debug.Log("LeaderBoard "+leaderboard.Value);
    }
    // Start is called before the first frame update
    void Start()
    {
        var t = GetLeaderBoardAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
