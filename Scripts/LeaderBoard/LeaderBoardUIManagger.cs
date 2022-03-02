using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderBoardUIManagger : MonoBehaviour
{
    public static LeaderBoardUIManagger instance;

    string RankMarkRoot = Application.streamingAssetsPath + "/RankMark/";

    public int MyRankNum;
    public int MyScore;

    public int CurrentPage;
    public const int PageButtonInPanelNum = 10;
    public const int itemInPageNum = 8;

    public Texture[] PreLoad_Marks = new Texture[7];
    public Text[] PreLoad_Titles = new Text[7];
    public Text MyNameText;
    public Text MyRankNumText;
    public Text MyScoreText;
    public Text MyRankTitleText;
    public RawImage Avatar;
    public RawImage MyRankMark;

    public GameObject RankDisplayPanel;
    public GameObject RankListPanel;
    public GameObject RankRankPrefab;

    public GameObject PageButtonPrefab;
    public GameObject PageButtonsListPanel;
    public Button NextPageButton;
    public Button PrevPageButton;
    public Button NextPageListButton;
    public Button PrevPageListButton;

    public GameObject WaitingForQueryPanel;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetPreLoadMarks()
    {
        PreLoad_Marks = new Texture[7];
        for (int i = 0; i < 7; i++)
        {
            StartCoroutine(SetMark(i+1, PreLoad_Marks[i]));
        }
    }
    public void OnClick_Query()
    {
        WaitingForQueryPanel.SetActive(true);
        var t = LeaderBoardManager.instance.GetLeaderboardContentAsync();
    }
    public void OnClick_ShowRankDisplay()
    {
        if (RankDisplayPanel.activeSelf)
        {
            RankDisplayPanel.SetActive(false);
        }
        else
        {
            RankDisplayPanel.SetActive(true);
        }
        
    }
    public void OnClick_CloseRankDisplay()
    {
        
    }
    public void Show()
    {
        SetMyRank();
        ShowPageButtons();
        JumpToPage(1);
        //SetFirstPage();
    }
    public void SetMyRank()
    {
        Avatar.texture = StartGameManager.instance.image.texture;
        MyNameText.text = Steamworks.SteamClient.Name;
        var t = LeaderBoardManager.instance.UI_SetMyLeaderboard();

    }
    public void ClearRankList()
    {
        int ItemNum = RankListPanel.transform.childCount;
        for (int i = ItemNum - 1; i >= 0; i--)
        {
            Destroy(RankListPanel.transform.GetChild(i).gameObject);
        }
        
    }
    public void JumpToPage(int page)
    {
        Debug.Log("JumpToPage:" + page);
        ClearRankList();
        CurrentPage = page;
        ShowPageButtons();
        var t = LeaderBoardManager.instance.UI_GetLeaderboardContentByPage(page);
    }
    public void SetRankList(Steamworks.Data.LeaderboardEntry[] leaderboardEntry)
    {
        foreach (Steamworks.Data.LeaderboardEntry entry in leaderboardEntry)
        {
            AddNewItemEntry(entry);
        }
    }
    void AddNewItemEntry(Steamworks.Data.LeaderboardEntry entry)
    {
        GameObject itemEntry = Instantiate(RankRankPrefab, RankListPanel.transform);
        itemEntry.GetComponent<PlayerRankPrefab>().Initialize(entry);
    }
   
    public void ShowPageButtons()
    {
        RemoveAllPageButtons();
        ShowInstalledNextPrevButtons();

        int PageNum = GetPageNum();

        int startIndex = ((CurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        int endIndex = Mathf.Min(PageNum, startIndex + PageButtonInPanelNum - 1);
        for (int i = startIndex; i <= endIndex; i++)
        {
            bool Selected = i == CurrentPage;
            AddNewPageButton(i, Selected);
        }
    }
    void AddNewPageButton(int pageIndex, bool Selected)
    {
        GameObject pageButton = Instantiate(PageButtonPrefab, PageButtonsListPanel.transform);
        pageButton.GetComponent<PageButtonPrefab>().Initialize(pageIndex, Selected);
    }
    public void RemoveAllPageButtons()
    {
        int ItemNum = PageButtonsListPanel.transform.childCount;
        for (int i = ItemNum - 1; i >= 0; i--)
        {
            Destroy(PageButtonsListPanel.transform.GetChild(i).gameObject);
        }

    }
    void ShowInstalledNextPrevButtons()
    {
        int PageNum = GetPageNum();
        int PageListNum = (PageNum - 1) / PageButtonInPanelNum + 1;
        if (PageNum == 1)
        {
            PrevPageButton.gameObject.SetActive(false);
            NextPageButton.gameObject.SetActive(false);
        }
        else if (CurrentPage == 1)
        {
            PrevPageButton.gameObject.SetActive(false);
            NextPageButton.gameObject.SetActive(true);
        }
        else if (CurrentPage == PageNum)
        {
            PrevPageButton.gameObject.SetActive(true);
            NextPageButton.gameObject.SetActive(false);
        }
        else
        {
            PrevPageButton.gameObject.SetActive(true);
            NextPageButton.gameObject.SetActive(true);
        }
        int CurrentPageList = (CurrentPage - 1) / PageButtonInPanelNum + 1;
        if (PageListNum == 1)
        {
            PrevPageListButton.gameObject.SetActive(false);
            NextPageListButton.gameObject.SetActive(false);
        }
        else if (CurrentPageList == 1)
        {
            PrevPageListButton.gameObject.SetActive(false);
            NextPageListButton.gameObject.SetActive(true);
        }
        else if (CurrentPageList == PageListNum)
        {
            PrevPageListButton.gameObject.SetActive(true);
            NextPageListButton.gameObject.SetActive(false);
        }
        else
        {
            PrevPageListButton.gameObject.SetActive(true);
            NextPageListButton.gameObject.SetActive(true);
        }
    }
    public void OnClick_NextPage()
    {
        JumpToPage(CurrentPage + 1);
    }
    public void OnClick_NextPageList()
    {
        int PageListStartIndex = ((CurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        JumpToPage(PageListStartIndex + PageButtonInPanelNum);
    }
    public void OnClick_PrevPage()
    {
        JumpToPage(CurrentPage - 1);

    }
    public void OnClick_PrevPageList()
    {

        int PageListStartIndex = ((CurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        JumpToPage(PageListStartIndex - PageButtonInPanelNum);
        
        
    }
    #region utils
    public int GetPageNum()
    {
        int AllItemNum = LeaderBoardManager.instance.AllItemCount;
        int PageNum = AllItemNum / itemInPageNum + 1;
        return PageNum;
    }
    public void SetMyMark()
    {
        if (MyScore >= 100000 && MyRankNum == 1)
        {
            Debug.Log("SetMyMark 6");
            MyRankMark.texture = PreLoad_Marks[6];
            MyRankTitleText.text = PreLoad_Titles[6].text;
        }
        else if (MyScore >= 50000 && MyRankNum <= 500)
        {
            Debug.Log("SetMyMark 5");
            MyRankMark.texture = PreLoad_Marks[5];
            MyRankTitleText.text = PreLoad_Titles[5].text;
        }
        else if (MyScore >= 10000)
        {
            Debug.Log("SetMyMark 4");
            MyRankMark.texture = PreLoad_Marks[4];
            MyRankTitleText.text = PreLoad_Titles[4].text;
        }
        else if (MyScore >= 5000)
        {
            Debug.Log("SetMyMark 3");
            MyRankMark.texture = PreLoad_Marks[3];
            MyRankTitleText.text = PreLoad_Titles[3].text;
        }
        else if (MyScore >= 2000)
        {
            Debug.Log("SetMyMark 2");
            MyRankMark.texture = PreLoad_Marks[2];
            MyRankTitleText.text = PreLoad_Titles[2].text;
        }
        else if (MyScore >= 500)
        {
            Debug.Log("SetMyMark 1");
            MyRankMark.texture = PreLoad_Marks[1];
            MyRankTitleText.text = PreLoad_Titles[1].text;
        }
        else
        {
            Debug.Log("SetMyMark 0");
            MyRankMark.texture = PreLoad_Marks[0];
            MyRankTitleText.text = PreLoad_Titles[0].text;
        }
    }
    public string GetMarkPath(int rankMark)
    {
        return RankMarkRoot + "Mark" + rankMark + ".png";
    }
    private IEnumerator SetMark(int rankMark,Texture mark)
    {
        string path = GetMarkPath(rankMark);
        Debug.Log(path);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(@path);
        yield return request.SendWebRequest();
        // AssetBundle ab = DownloadHandlerAssetBundle.GetContent (request );

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture tt = DownloadHandlerTexture.GetContent(request);
            mark = tt;
        }

    }
    #endregion
}
