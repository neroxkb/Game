using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static UIManager instance;
    const string playerMoneyPrefKey = "PlayerMoney";
    #region UIObject
    public TextMeshProUGUI PlayerName;
    public Text PlayerNameText;
    public Canvas GameCanvas;
    public Canvas RoomCanvas;
    public Toggle RoomPublicToggle;
    public Text RoomID;
    #endregion
    #region select game
    public GameObject Select;
    public GameObject NotEnoughMoneyPanel;
    public GameObject GameIntroPanel_1;
    public GameObject GameIntroPanel_2;
    public GameObject GameIntroPanel_3;
    public GameObject[] GameList = new GameObject[3];
    public GameObject[] SelectPointList = new GameObject[3];
    public int CurrentGameIndex;
    public bool TapeConfirm;
    #endregion
    #region AnswerWorkShop
    public Canvas AnswerWorkshopCanvas;
    public Canvas AnswerUploadCanvas;
    public GameObject GoAnswerWorkshopPanel;
    public GameObject WorkshopListEntry;
    public GameObject WorkshopListPanel;
    public GameObject PageButtonPrefab;
    public GameObject PageButtonsListPanel;
    public GameObject WaitForQueryPanel;
    public GameObject ItemInfoPanel;
    public GameObject WaitForDownloadPanel;
    public RawImage ItemInfoPreview;
    public Image DownloadProcessImage;
    public Text ItemInfoTitle;
    public Text ItemInfoPath;
    public Text ItemInfoSize;

    public Button InstalledButton;
    public Button UninstalledButton;
    public Button NextPageButton;
    public Button PrevPageButton;
    public Button NextPageListButton;
    public Button PrevPageListButton;
    public Button DownloadButton;
    public Button UnsubButton;
    public Button OpenPathButton;
    //public Button DownloadAllButton;
    public int ONLY_QUERY = 1;
    public int QUERY_AND_SHOW = 2;
    public int InstalledPageNum;
    public int InstalledPageListNum;
    public int InstalledCurrentPage;
    public int UninstalledPageNum;
    public int UninstalledPageListNum;
    public int UninstalledCurrentPage;
    public const int itemInPageNum = 16;
    public const int PageButtonInPanelNum = 10;
    public Steamworks.Ugc.Item CurrentItem;
    public GameObject CurrentAnswerItemPrefab;
    public List<GameObject> CurrentPageItemList;
    public List<Steamworks.Ugc.Item> InstalledAnswerItemList;
    public List<Steamworks.Ugc.Item> UninstalledAnswerItemList;
    public static event Action<float> DownloadProgress;
    #endregion
    #region UnderCoverWorkshop
    public Canvas UnderCoverWorkshopCanvas;
    public GameObject GoUnderCoverWorkshopPanel;
    public GameObject WaitForUpdatePanel_UnderCover;
    public GameObject UnderCoverAddPanel;
    public GameObject PageButtonsListPanel_UnderCover;
    public Button NextPageButton_UnderCover;
    public Button PrevPageButton_UnderCover;
    public Button NextPageListButton_UnderCover;
    public Button PrevPageListButton_UnderCover;
    public int CurrentPage_UnderCover;
    public int PageNum_UnderCover;
    public int PageListNum_UnderCover;
    public const int itemInPageNum_UnderCover = 6;
    public GameObject UnderCoverItemPrefab;
    public GameObject UnderCoverItemListPanel;
    public List<GameObject> CurrentPageItemList_UnderCover;
    public List<Steamworks.Ugc.Item> UnderCoverItemList;
    #endregion
    #region leaderboard
    public GameObject LeaderboardCanvas;
    public GameObject GoLeaderboardPanel;
    #endregion
    #region moneyboard
    public GameObject PurchaseRewardCanvas;
    public GameObject DLC1MoneyCanvas;
    public Text DLC1MoneyText;
    public GameObject DLC2MoneyCanvas;
    public Text DLC2MoneyText;
    public GameObject DLC3MoneyCanvas;
    public Text DLC3MoneyText;
    #endregion
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
        TapeConfirm = false;
        SelectPointList[0].GetComponent<SelectPointManager>().TurnPointOn();
        if (CurrentGameIndex != TagManager.instance.StoreGameTape)
        {
            SelectPointList[0].GetComponent<SelectPointManager>().TurnPointOff();
            SetGameActive(TagManager.instance.StoreGameTape);
            CurrentGameIndex = TagManager.instance.StoreGameTape;
            SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOn();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region Utils
    public void SetName(string name)
    {
        PlayerNameText.text = name;
    }
    public string GetSize(long byteSize)
    {
        if (byteSize > 1024*1024)
        {
            int size = (int)byteSize / (1024 * 1024);
            return size + "MB";
        }
        else if (byteSize > 1024)
        {
            int size = (int)byteSize / (1024);
            return size + "KB";
        }
        else
        {
            return byteSize + "B";
        }
    }
    #endregion
    public void SetGameActive(int index)
    {
        for (int i = 0; i < GameList.Length; i++)
        {
            if (i == index)
            {
                GameList[i].SetActive(true);
            }
            else
            {
                GameList[i].SetActive(false);
            }
        }
    }
    public void OnClick_NextGame()
    {
        SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOff();
        if (CurrentGameIndex == GameList.Length - 1)
        {
            SetGameActive(0);
            CurrentGameIndex = 0;
            SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        else
        {
            SetGameActive(CurrentGameIndex + 1);
            CurrentGameIndex += 1;
            SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        TagManager.instance.StoreGameTape = CurrentGameIndex;
    }
    public void OnClick_PrevGame()
    {
        SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOff();
        if (CurrentGameIndex == 0)
        {
            SetGameActive(GameList.Length - 1);
            CurrentGameIndex = GameList.Length - 1;
            SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        else
        {
            SetGameActive(CurrentGameIndex - 1);
            CurrentGameIndex -= 1;
            SelectPointList[CurrentGameIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        TagManager.instance.StoreGameTape = CurrentGameIndex;
    }
    #region TapeConfirm
    public void OnClick_TapeConfirm()
    {
        if (!TapeConfirm)
        {
            Debug.Log("Tape Confirm");
            GameList[CurrentGameIndex].GetComponent<TapeManager>().TapeConfirm();
            TapeConfirm = true;
            Select.SetActive(false);
            if (CurrentGameIndex == 1)
            {
                GameIntroPanel_2.SetActive(true);
                GameIntroPanel_1.SetActive(false);
                GameIntroPanel_3.SetActive(false);
                GoAnswerWorkshopPanel.gameObject.SetActive(true);
                GoUnderCoverWorkshopPanel.gameObject.SetActive(false);
                GoLeaderboardPanel.SetActive(false);
            }
            else if (CurrentGameIndex == 2)
            {
                GameIntroPanel_3.SetActive(true);
                GameIntroPanel_2.SetActive(false);
                GameIntroPanel_1.SetActive(false);
                GoUnderCoverWorkshopPanel.gameObject.SetActive(true);
                GoAnswerWorkshopPanel.gameObject.SetActive(false);
                GoLeaderboardPanel.SetActive(false);
            }
            else
            {
                GameIntroPanel_1.SetActive(true);
                GameIntroPanel_2.SetActive(false);
                GameIntroPanel_3.SetActive(false);
                GoLeaderboardPanel.SetActive(true);
                GoAnswerWorkshopPanel.gameObject.SetActive(false);
                GoUnderCoverWorkshopPanel.gameObject.SetActive(false);
            }
            //ShowGameCanvas();
        }
    }
    public void ShowGameCanvas()
    {
        GameCanvas.gameObject.SetActive(true);
    }
    #endregion
    #region TapeRemove
    public void OnClick_TapeRemove()
    {
        if (TapeConfirm)
        {
            GameList[CurrentGameIndex].GetComponent<TapeManager>().TapeRemove();
            TapeConfirm = false;
            Select.SetActive(true);
            CloseGameCanvas();
        }
        
    }
    public void CloseGameCanvas()
    {
        
        if (RoomCanvas.gameObject.activeSelf)
        {
            RoomManager.instance.DestroyAllObject();
            OnClick_LeaveRoom();
            RoomCanvas.gameObject.SetActive(false);
        }
        AnswerWorkshopCanvas.gameObject.SetActive(false);
        UnderCoverWorkshopCanvas.gameObject.SetActive(false);
        AnswerUploadCanvas.gameObject.SetActive(false);
        GoAnswerWorkshopPanel.gameObject.SetActive(false);
        GoUnderCoverWorkshopPanel.gameObject.SetActive(false);
        GoLeaderboardPanel.gameObject.SetActive(false);
        LeaderboardCanvas.gameObject.SetActive(false);
        GameIntroPanel_1.SetActive(false);
        GameIntroPanel_2.SetActive(false);
        GameIntroPanel_3.SetActive(false);
        GameCanvas.gameObject.SetActive(false);
    }
    #endregion
    #region CreateRoom
    public void OnClick_CreateRoom()
    {
        //int money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        int money = MoneyBoardManager.instance.MyCurrentMoney;
        if (CurrentGameIndex == 0 && money <= 2000)
        {
            NotEnoughMoneyPanel.SetActive(true);
            return;
        }
        string lang = SettingsManager.instance.GetCurrentLang();
        if (CurrentGameIndex == 0 && money>2000)
        {
            RoomManager.instance.CreateRoom(RoomPublicToggle.isOn,0,4,"All");
        }
        else if (CurrentGameIndex == 1)
        {
            RoomManager.instance.CreateRoom(RoomPublicToggle.isOn, 1, 11,lang);
        }
        else if (CurrentGameIndex == 2)
        {
            RoomManager.instance.CreateRoom(RoomPublicToggle.isOn, 2, 8, lang);
        }
        

    }
    #endregion
    #region JoinRoom
    public void OnClick_JoinRoom()
    {
        //int money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        int money = MoneyBoardManager.instance.MyCurrentMoney;
        if (!string.IsNullOrEmpty(RoomID.text))
        {
            if (CurrentGameIndex == 0 && money <= 2000)
            {
                NotEnoughMoneyPanel.SetActive(true);
                return;
            }
            RoomManager.instance.JoinRoom(RoomID.text);
        }
        else
        {
            Debug.Log("Room Id is null");
        }
    }
    #endregion
    #region JoinRoom
    public void OnClick_Match()
    {
        //int money = PlayerPrefs.GetInt(playerMoneyPrefKey);
        int money = MoneyBoardManager.instance.MyCurrentMoney;
        if (CurrentGameIndex == 0 && money <= 2000)
        {
            NotEnoughMoneyPanel.SetActive(true);
            return;
        }
        string lang = SettingsManager.instance.GetCurrentLang();
        if (CurrentGameIndex == 0 && money > 2000)
        {
            RoomManager.instance.Match(CurrentGameIndex,4, "All");
        }
        else if (CurrentGameIndex == 1)
        {
            RoomManager.instance.Match(CurrentGameIndex, 11, lang);
        }
        else if (CurrentGameIndex == 2)
        {
            RoomManager.instance.Match(CurrentGameIndex, 8, lang);
        }
        

    }
    #endregion
    #region LeaveRoom
    public void OnClick_LeaveRoom()
    {
        RoomManager.instance.LeaveRoom();
        
    }
    #endregion
    #region StartGame
    public void OnClick_StartGame()
    {

        RoomManager.instance.StartGame(CurrentGameIndex+1);

    }
    #endregion
    #region Leaderboard
    public void OnClick_GoLeaderboard()
    {
        GameCanvas.gameObject.SetActive(false);
        LeaderboardCanvas.gameObject.SetActive(true);
        LeaderBoardUIManagger.instance.Show();
    }
    public void OnClick_LeaveLeaderboard()
    {
        LeaderboardCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);

    }
    #endregion
    #region moneyboard
    public void OnClick_ShowPurchaseRewardCanvas()
    {
        PurchaseRewardCanvas.gameObject.SetActive(true);
    }
    public void OnClick_ClosePurchaseRewardCanvas()
    {
        PurchaseRewardCanvas.gameObject.SetActive(false);
    }
    public void OnClick_ShowDLC1MoneyCanvas(string money)
    {
        DLC1MoneyCanvas.gameObject.SetActive(true);
        DLC1MoneyText.text = money;
    }
    public void OnClick_CloseDLC1MoneyCanvas()
    {
        DLC1MoneyCanvas.gameObject.SetActive(false);
    }
    public void OnClick_ShowDLC2MoneyCanvas(string money)
    {
        DLC2MoneyCanvas.gameObject.SetActive(true);
        DLC2MoneyText.text = money;
    }
    public void OnClick_CloseDLC2MoneyCanvas()
    {
        DLC2MoneyCanvas.gameObject.SetActive(false);
    }
    public void OnClick_ShowDLC3MoneyCanvas(string money)
    {
        DLC3MoneyCanvas.gameObject.SetActive(true);
        DLC3MoneyText.text = money;
    }
    public void OnClick_CloseDLC3MoneyCanvas()
    {
        DLC3MoneyCanvas.gameObject.SetActive(false);
    }
    #endregion
    #region Workshop
    public void OnClick_GoAnswerWorkshop()
    {
        GameCanvas.gameObject.SetActive(false);
        AnswerWorkshopCanvas.gameObject.SetActive(true);
        InstalledButton.interactable = false;
        UninstalledButton.interactable = true;
        InstalledCurrentPage = 1;
        UninstalledCurrentPage = 1;
        WaitForQueryPanel.SetActive(true);
        AnswerWorkshopManager.instance.OnClick_Query(QUERY_AND_SHOW);
    }
    public void OnClick_WorkshopAdd()
    {
        AnswerWorkshopCanvas.gameObject.SetActive(false);
        AnswerUploadCanvas.gameObject.SetActive(true);
    }
    public void OnClick_Workshop_AddBack()
    {
        AnswerWorkshopCanvas.gameObject.SetActive(true);
        AnswerUploadCanvas.gameObject.SetActive(false);
        //AnswerWorkshopManager.instance.ClearCanvas();
    }
    public void OnClick_WorkshopAddClear()
    {
        AnswerWorkshopManager.instance.ClearCanvas();
    }
    public void OnClick_Workshop_QueryBack()
    {
        AnswerWorkshopCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);
        
    }
    public void OnClick_InstalledButton()
    {
        if (InstalledButton.interactable)
        {
            ShowInstalledAnswerItems();
            InstalledButton.interactable = false;
            UninstalledButton.interactable = true;
            //DownloadAllButton.gameObject.SetActive(false);
        }
    }
    public void OnClick_UninstalledButton()
    {
        if (UninstalledButton.interactable)
        {
            ShowUninstalledAnswerItems();
            InstalledButton.interactable = true;
            UninstalledButton.interactable = false;
            //DownloadAllButton.gameObject.SetActive(true);
        }
    }
    public void OnClick_Refresh()
    {
        WaitForQueryPanel.SetActive(true);
        AnswerWorkshopManager.instance.TagDropDown.value = 0;
        AnswerWorkshopManager.instance.OnClick_Query(QUERY_AND_SHOW);
    }
    public void OnClick_NextPage()
    {
        if (InstalledButton.interactable)
        {
            JumpPage(UninstalledCurrentPage + 1);
        }
        else
        {
            JumpPage(InstalledCurrentPage + 1);
        }
    }
    public void OnClick_NextPageList()
    {
        if (InstalledButton.interactable)
        {
            int UninstalledPageListStartIndex = ((UninstalledCurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
            JumpPage(UninstalledPageListStartIndex + PageButtonInPanelNum);
        }
        else
        {
            int InstalledPageListStartIndex = ((InstalledCurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
            JumpPage(InstalledPageListStartIndex + PageButtonInPanelNum);
        }
    }
    public void OnClick_PrevPage()
    {
        if (InstalledButton.interactable)
        {
            JumpPage(UninstalledCurrentPage - 1);
        }
        else
        {
            JumpPage(InstalledCurrentPage - 1);
        }
    }
    public void OnClick_PrevPageList()
    {
        if (InstalledButton.interactable)
        {
            int UninstalledPageListStartIndex = ((UninstalledCurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
            JumpPage(UninstalledPageListStartIndex - PageButtonInPanelNum);
        }
        else
        {
            int InstalledPageListStartIndex = ((InstalledCurrentPage - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
            JumpPage(InstalledPageListStartIndex - PageButtonInPanelNum);
        }
    }
    public void JumpPage(int PageIndex)
    {
        RemoveAllItems();
        if (InstalledButton.interactable)
        {
            UninstalledCurrentPage = PageIndex;
            ShowUninstalledAnswerItems();
        }
        else
        {
            InstalledCurrentPage = PageIndex;
            ShowInstalledAnswerItems();
        }
    }
    public void ShowAnswerItems()
    {
        WaitForQueryPanel.SetActive(false);
        InstalledCurrentPage = 1;
        UninstalledCurrentPage = 1;
        this.InstalledAnswerItemList = AnswerWorkshopManager.instance.InstalledAnswerItemList;
        this.UninstalledAnswerItemList = AnswerWorkshopManager.instance.UninstalledAnswerItemList;
        int installedNum = InstalledAnswerItemList.Count;
        int uninstalledNum = UninstalledAnswerItemList.Count;
        InstalledPageNum = installedNum / itemInPageNum + 1;
        UninstalledPageNum = uninstalledNum / itemInPageNum + 1;
        InstalledPageListNum = (InstalledPageNum - 1) / PageButtonInPanelNum + 1;
        UninstalledPageListNum = (UninstalledPageNum - 1) / PageButtonInPanelNum + 1;
        Debug.Log(installedNum+" "+InstalledPageNum +" "+ InstalledPageListNum + "  "+ uninstalledNum + " "+ UninstalledPageNum+" "+ UninstalledPageListNum);

        CurrentPageItemList = new List<GameObject>();
        if (InstalledButton.interactable)
        {
            ShowUninstalledAnswerItems();
        }
        else
        {
            ShowInstalledAnswerItems();
        }
    }
    public void ShowAnswerItemsWithTag(string Tag)
    {
        WaitForQueryPanel.SetActive(false);
        InstalledCurrentPage = 1;
        UninstalledCurrentPage = 1;
        this.InstalledAnswerItemList = AnswerWorkshopManager.instance.GetInstalledAnswerItemListWithTag(Tag);
        this.UninstalledAnswerItemList = AnswerWorkshopManager.instance.GetUninstalledAnswerItemListWithTag(Tag);
        int installedNum = InstalledAnswerItemList.Count;
        int uninstalledNum = UninstalledAnswerItemList.Count;
        InstalledPageNum = installedNum / itemInPageNum + 1;
        UninstalledPageNum = uninstalledNum / itemInPageNum + 1;
        InstalledPageListNum = (InstalledPageNum - 1) / PageButtonInPanelNum + 1;
        UninstalledPageListNum = (UninstalledPageNum - 1) / PageButtonInPanelNum + 1;
        Debug.Log(installedNum + " " + InstalledPageNum + " " + InstalledPageListNum + "  " + uninstalledNum + " " + UninstalledPageNum + " " + UninstalledPageListNum);

        CurrentPageItemList = new List<GameObject>();
        if (InstalledButton.interactable)
        {
            ShowUninstalledAnswerItems();
        }
        else
        {
            ShowInstalledAnswerItems();
        }
    }
    public bool CheckItemR18(Steamworks.Ugc.Item item)
    {
        string[] itemTags = item.Tags;
        foreach (string tag in itemTags)
        {
            if (tag == "r-18+")
            {
                return true;
            }
        }
        return false;
    }
    public void ShowInstalledAnswerItems()
    {
        ShowInstalledNextPrevButtons();
        ShowInstalledPageButtons();
        RemoveAllItems();
        int installedNum = InstalledAnswerItemList.Count;
        int startIndex = itemInPageNum * (InstalledCurrentPage - 1);
        int endIndex = Mathf.Min(itemInPageNum * InstalledCurrentPage, installedNum);
        for (int i = startIndex; i < endIndex; i++)
        {
            Steamworks.Ugc.Item item = InstalledAnswerItemList[i];
            AddNewItemEntry(item);
            /*if (CheckItemR18(item)) Debug.Log("r-18+ item");
            if (CheckItemR18(item) && AnswerWorkshopManager.instance.GetCurrentTag() == "r-18+")
            {
                AddNewItemEntry(item);
            }
            else if (!CheckItemR18(item))
            {
                AddNewItemEntry(item);
            }*/
        }
    }
    public void ShowInstalledPageButtons()
    {
        RemoveAllPageButtons();
        int startIndex = ((InstalledCurrentPage-1)/PageButtonInPanelNum)* PageButtonInPanelNum + 1;
        int endIndex = Mathf.Min(InstalledPageNum, startIndex + PageButtonInPanelNum - 1);
        for (int i = startIndex; i <= endIndex; i++)
        {
            bool Selected = i == InstalledCurrentPage;
            AddNewPageButton(i, Selected);
        }
    }
    public void ShowUninstalledAnswerItems()
    {
        ShowUninstalledNextPrevButtons();
        ShowUninstalledPageButtons();
        RemoveAllItems();
        int uninstalledNum = UninstalledAnswerItemList.Count;
        int startIndex = itemInPageNum * (UninstalledCurrentPage - 1);
        int endIndex = Mathf.Min(itemInPageNum * UninstalledCurrentPage, uninstalledNum);
        for (int i = startIndex; i < endIndex; i++)
        {
            Steamworks.Ugc.Item item = UninstalledAnswerItemList[i];
            AddNewItemEntry(item);
            /*if (CheckItemR18(item)) Debug.Log("r-18+ item");
            if (CheckItemR18(item) && AnswerWorkshopManager.instance.GetCurrentTag() == "r-18+")
            {
                AddNewItemEntry(item);
            }
            else if (!CheckItemR18(item))
            {
                AddNewItemEntry(item);
            }*/
        }
    }
    public void ShowUninstalledPageButtons()
    {
        RemoveAllPageButtons();
        int startIndex = ((UninstalledCurrentPage-1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        int endIndex = Mathf.Min(UninstalledPageNum, startIndex + PageButtonInPanelNum - 1);
        for (int i = startIndex; i <= endIndex; i++)
        {
            bool Selected = i == UninstalledCurrentPage;
            AddNewPageButton(i,Selected);
        }
    }
    public void ShowItemInfo(Steamworks.Ugc.Item item,RawImage preview)
    {
        ItemInfoPanel.SetActive(true);
        ItemInfoTitle.text = item.Title;
        ItemInfoPreview.texture = preview.texture;
        CurrentItem = item;
        
        foreach (GameObject ItemPrefab in CurrentPageItemList)
        {
            if (ItemPrefab.GetComponent<WorkshopListEntry>().IsSelect)
            {
                CurrentAnswerItemPrefab = ItemPrefab;
            }

        }
        if (item.IsDownloading|| item.IsDownloadPending)
        {
            ItemInfoSize.text = GetSize(item.DownloadBytesTotal);
            ItemInfoPath.text = "";
            UnsubButton.gameObject.SetActive(false);
            DownloadButton.gameObject.SetActive(false);
            OpenPathButton.gameObject.SetActive(false);
            DownloadProcessImage.gameObject.SetActive(false);
        }
        else if (item.IsSubscribed)
        {
            ItemInfoSize.text = GetSize(item.SizeBytes);
            ItemInfoPath.text = item.Directory;
            UnsubButton.gameObject.SetActive(true);
            DownloadButton.gameObject.SetActive(false);
            OpenPathButton.gameObject.SetActive(true);
            DownloadProcessImage.gameObject.SetActive(false);
        }
        else
        {
            ItemInfoSize.text = "";
            
            ItemInfoPath.text = "";
            UnsubButton.gameObject.SetActive(false);
            DownloadButton.gameObject.SetActive(true);
            OpenPathButton.gameObject.SetActive(false);
            DownloadProcessImage.gameObject.SetActive(true);
        }
    }

    void ShowInstalledNextPrevButtons()
    {
        
        if (InstalledPageNum == 1)
        {
            PrevPageButton.gameObject.SetActive(false);
            NextPageButton.gameObject.SetActive(false);
        }
        else if (InstalledCurrentPage == 1)
        {
            PrevPageButton.gameObject.SetActive(false);
            NextPageButton.gameObject.SetActive(true);
        }
        else if (InstalledCurrentPage == InstalledPageNum)
        {
            PrevPageButton.gameObject.SetActive(true);
            NextPageButton.gameObject.SetActive(false);
        }
        else
        {
            PrevPageButton.gameObject.SetActive(true);
            NextPageButton.gameObject.SetActive(true);
        }
        int InstalledCurrentPageList = (InstalledCurrentPage - 1) / PageButtonInPanelNum + 1;
        if (InstalledPageListNum == 1)
        {
            PrevPageListButton.gameObject.SetActive(false);
            NextPageListButton.gameObject.SetActive(false);
        }
        else if (InstalledCurrentPageList == 1)
        {
            PrevPageListButton.gameObject.SetActive(false);
            NextPageListButton.gameObject.SetActive(true);
        }
        else if (InstalledCurrentPageList == InstalledPageListNum)
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
    void ShowUninstalledNextPrevButtons()
    {
        if (UninstalledPageNum == 1)
        {
            PrevPageButton.gameObject.SetActive(false);
            NextPageButton.gameObject.SetActive(false);
        }
        else if (UninstalledCurrentPage == 1)
        {
            PrevPageButton.gameObject.SetActive(false);
            NextPageButton.gameObject.SetActive(true);
        }
        else if (UninstalledCurrentPage == UninstalledPageNum)
        {
            PrevPageButton.gameObject.SetActive(true);
            NextPageButton.gameObject.SetActive(false);
        }
        else
        {
            PrevPageButton.gameObject.SetActive(true);
            NextPageButton.gameObject.SetActive(true);
        }
        int UninstalledCurrentPageList = (UninstalledCurrentPage - 1) / PageButtonInPanelNum + 1;
        if (UninstalledPageListNum == 1)
        {
            PrevPageListButton.gameObject.SetActive(false);
            NextPageListButton.gameObject.SetActive(false);
        }
        else if (UninstalledCurrentPageList == 1)
        {
            PrevPageListButton.gameObject.SetActive(false);
            NextPageListButton.gameObject.SetActive(true);
        }
        else if (UninstalledCurrentPageList == UninstalledPageListNum)
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
    void AddNewItemEntry(Steamworks.Ugc.Item item)
    {
        GameObject itemEntry = Instantiate(WorkshopListEntry, WorkshopListPanel.transform);
        itemEntry.GetComponent<WorkshopListEntry>().Initialize(item);
        CurrentPageItemList.Add(itemEntry);
    }
    void AddNewPageButton(int pageIndex,bool Selected)
    {
        GameObject pageButton = Instantiate(PageButtonPrefab, PageButtonsListPanel.transform);
        pageButton.GetComponent<PageButtonPrefab>().Initialize(pageIndex, Selected);
    }
    
    public void UnSelectAllItems()
    {
        foreach (GameObject itemEntry in CurrentPageItemList)
        {
            itemEntry.GetComponent<WorkshopListEntry>().CancelSelect();
        }
    }
    public void RemoveAllItems()
    {
        ItemInfoPanel.SetActive(false);
        int ItemNum = WorkshopListPanel.transform.childCount;
        for (int i = ItemNum - 1; i >= 0; i--)
        {
            Destroy(WorkshopListPanel.transform.GetChild(i).gameObject);
        }
        CurrentPageItemList = new List<GameObject>();

    }
    public void RemoveAllPageButtons()
    {
        int ItemNum = PageButtonsListPanel.transform.childCount;
        for (int i = ItemNum - 1; i >= 0; i--)
        {
            Destroy(PageButtonsListPanel.transform.GetChild(i).gameObject);
        }

    }
    #endregion
    #region DownloadItem
    public void OnClick_DownloadItem()
    {
        Debug.Log("OnClick_DownloadItem");
        //WaitForDownloadPanel.SetActive(true);
        DownloadButton.gameObject.SetActive(false);
        SubscribeDownloadItemAsync(CurrentItem);
    }
    private async void SubscribeDownloadItemAsync(Steamworks.Ugc.Item item)
    {
        if (!item.IsSubscribed)
        {
            Debug.Log("downloading " + item.Title);
            var sub = await item.Subscribe();
            item.Download();
            CurrentAnswerItemPrefab.GetComponent<WorkshopListEntry>().ShowDownloadProgress();
            //AddTitleDescription(item.Directory,item.Title,item.Description);
        }
        
    }
    private async void DownloadItemAsync(Steamworks.Ugc.Item item)
    {
        if (!item.IsSubscribed)
        {
            Debug.Log("downloading "+item.Title);
            var sub = await item.Subscribe();
            if (CurrentAnswerItemPrefab != null)
            {
                CurrentAnswerItemPrefab.GetComponent<WorkshopListEntry>().DownloadingPanel.SetActive(true);
                CurrentAnswerItemPrefab.GetComponent<WorkshopListEntry>().DownloadingProcess.fillAmount = 0;
                CurrentAnswerItemPrefab.GetComponent<WorkshopListEntry>().DownloadingText.text = 0 + "%";
            }
            
            var download = await item.DownloadAsync(
                DownloadProgress = delegate (float progress)
                {
                    DownloadProcessImage.fillAmount = progress;
                    if (CurrentAnswerItemPrefab != null)
                    {
                        CurrentAnswerItemPrefab.GetComponent<WorkshopListEntry>().DownloadingProcess.fillAmount = progress;
                        CurrentAnswerItemPrefab.GetComponent<WorkshopListEntry>().DownloadingText.text = progress + "%";
                    }
                });
            //AddTitleDescription(item.Directory,item.Title,item.Description);
        }
        DownloadProcessImage.fillAmount = 0;
        WaitForDownloadPanel.SetActive(false);
        OnClick_Refresh();
    }
    public void AddTitleDescription(string Directory, string Title, string Description)
    {
        QuestionData questionData = AnswerFileManager.instance.readJson(Directory);
        questionData.title = Title;
        questionData.description = Description;
        AnswerFileManager.instance.writeJson(questionData, Directory);
    }
    #endregion
    #region UnsubItem
    public void OnClick_UnsubscribeItem()
    {
        Debug.Log("OnClick_UnsubscribeItem");
        //WaitForQueryPanel.SetActive(true);
        UnsubscribeItemAsync(CurrentItem);
    }
    private async void UnsubscribeItemAsync(Steamworks.Ugc.Item item)
    {
        if (item.IsSubscribed)
        {
            Debug.Log("unsubscribing " + item.Title);
            UnsubButton.gameObject.SetActive(false);
            OpenPathButton.gameObject.SetActive(false);
            var unsub = await item.Unsubscribe();
            if (Directory.Exists(item.Directory))
            {
                Directory.Delete(item.Directory, true);
            }
            
        }
        
        //OnClick_Refresh();
    }
    #endregion
    #region UnderCoverWorkshop
    public void OnClick_GoUnderCoverWorkshop()
    {
        GameCanvas.gameObject.SetActive(false);
        UnderCoverWorkshopCanvas.gameObject.SetActive(true);
        CurrentPage_UnderCover = 1;

        WaitForUpdatePanel_UnderCover.SetActive(true);
        UnderCoverWorkshopManager.instance.OnClick_Update();
    }
    public void OnClick_UnderCover_Add()
    {
        //UnderCoverWorkshopCanvas.gameObject.SetActive(false);
        UnderCoverAddPanel.gameObject.SetActive(true);
    }
    public void OnClick_UnderCover_CancelAdd()
    {
        UnderCoverWorkshopCanvas.gameObject.SetActive(true);
        UnderCoverAddPanel.gameObject.SetActive(false);
    }
    public void OnClick_UnderCover_AddClear()
    {
        UnderCoverWorkshopManager.instance.ClearAddPanel();
    }
    public void OnClick_UnderCover_WorkshopBack()
    {
        UnderCoverWorkshopCanvas.gameObject.SetActive(false);
        GameCanvas.gameObject.SetActive(true);

    }
    
    public void OnClick_Refresh_UnderCover()
    {
        WaitForUpdatePanel_UnderCover.SetActive(true);
        UnderCoverWorkshopManager.instance.OnClick_Update();
    }
    public void OnClick_NextPage_UnderCover()
    {
        JumpPage_UnderCover(CurrentPage_UnderCover + 1);
    }
    public void OnClick_NextPageList_UnderCover()
    {
        int PageListStartIndex = ((CurrentPage_UnderCover - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        JumpPage_UnderCover(PageListStartIndex + PageButtonInPanelNum);
        
    }
    public void OnClick_PrevPage_UnderCover()
    {
        JumpPage_UnderCover(CurrentPage_UnderCover - 1);
    }
    public void OnClick_PrevPageList_UnderCover()
    {
        int PageListStartIndex = ((CurrentPage_UnderCover - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        JumpPage_UnderCover(PageListStartIndex - PageButtonInPanelNum);
        
    }
    public void JumpPage_UnderCover(int PageIndex)
    {
        RemoveAllItems_UnderCover();

        CurrentPage_UnderCover = PageIndex;
        ShowUnderCoverItems();
        
    }
    public void ShowUnderCoverItems()
    {
        WaitForUpdatePanel_UnderCover.SetActive(false);
        this.UnderCoverItemList = UnderCoverWorkshopManager.instance.UnderCoverItemList;
        int ItemNum_UnderCover = UnderCoverItemList.Count;
        
        PageNum_UnderCover = ItemNum_UnderCover / itemInPageNum_UnderCover + 1;
        PageListNum_UnderCover = (PageNum_UnderCover - 1) / PageButtonInPanelNum + 1;
        Debug.Log(ItemNum_UnderCover + " " + PageNum_UnderCover + " " + PageListNum_UnderCover);
        ShowItems_UnderCover();
        CurrentPageItemList = new List<GameObject>();
    }
    public void ShowItems_UnderCover()
    {
        ShowNextPrevButtons_UnderCover();
        ShowPageButtons_UnderCover();
        RemoveAllItems_UnderCover();
        int ItemNum_UnderCover = UnderCoverItemList.Count;
        int startIndex = itemInPageNum_UnderCover * (CurrentPage_UnderCover - 1);
        int endIndex = Mathf.Min(itemInPageNum_UnderCover * CurrentPage_UnderCover, ItemNum_UnderCover);
        for (int i = startIndex; i < endIndex; i++)
        {
            Steamworks.Ugc.Item item = UnderCoverItemList[i];
            AddNewItem_UnderCover(item);
        }
    }
    public void ShowPageButtons_UnderCover()
    {
        RemoveAllPageButtons_UnderCover();
        int startIndex = ((CurrentPage_UnderCover - 1) / PageButtonInPanelNum) * PageButtonInPanelNum + 1;
        int endIndex = Mathf.Min(PageNum_UnderCover, startIndex + PageButtonInPanelNum - 1);
        for (int i = startIndex; i <= endIndex; i++)
        {
            bool Selected = i == CurrentPage_UnderCover;
            AddNewPageButton_UnderCover(i, Selected);
        }
    }
    void ShowNextPrevButtons_UnderCover()
    {

        if (PageNum_UnderCover == 1)
        {
            PrevPageButton_UnderCover.gameObject.SetActive(false);
            NextPageButton_UnderCover.gameObject.SetActive(false);
        }
        else if (CurrentPage_UnderCover == 1)
        {
            PrevPageButton_UnderCover.gameObject.SetActive(false);
            NextPageButton_UnderCover.gameObject.SetActive(true);
        }
        else if (CurrentPage_UnderCover == PageNum_UnderCover)
        {
            PrevPageButton_UnderCover.gameObject.SetActive(true);
            NextPageButton_UnderCover.gameObject.SetActive(false);
        }
        else
        {
            PrevPageButton_UnderCover.gameObject.SetActive(true);
            NextPageButton_UnderCover.gameObject.SetActive(true);
        }
        int CurrentPageList_UnderCover = (CurrentPage_UnderCover - 1) / PageButtonInPanelNum + 1;
        if (PageListNum_UnderCover == 1)
        {
            PrevPageListButton_UnderCover.gameObject.SetActive(false);
            NextPageListButton_UnderCover.gameObject.SetActive(false);
        }
        else if (CurrentPageList_UnderCover == 1)
        {
            PrevPageListButton_UnderCover.gameObject.SetActive(false);
            NextPageListButton_UnderCover.gameObject.SetActive(true);
        }
        else if (CurrentPageList_UnderCover == PageListNum_UnderCover)
        {
            PrevPageListButton_UnderCover.gameObject.SetActive(true);
            NextPageListButton_UnderCover.gameObject.SetActive(false);
        }
        else
        {
            PrevPageListButton_UnderCover.gameObject.SetActive(true);
            NextPageListButton_UnderCover.gameObject.SetActive(true);
        }
    }

    void AddNewPageButton_UnderCover(int pageIndex, bool Selected)
    {
        GameObject pageButton = Instantiate(PageButtonPrefab, PageButtonsListPanel_UnderCover.transform);
        pageButton.GetComponent<PageButtonPrefab>().Initialize(pageIndex, Selected);
    }
    void AddNewItem_UnderCover(Steamworks.Ugc.Item item)
    {
        GameObject itemEntry = Instantiate(UnderCoverItemPrefab, UnderCoverItemListPanel.transform);
        itemEntry.GetComponent<UnderCoverItemPrefab>().Initialize(item);
        CurrentPageItemList_UnderCover.Add(itemEntry);
    }
    public void RemoveAllItems_UnderCover()
    {
        int ItemNum = UnderCoverItemListPanel.transform.childCount;
        for (int i = ItemNum - 1; i >= 0; i--)
        {
            Destroy(UnderCoverItemListPanel.transform.GetChild(i).gameObject);
        }
        CurrentPageItemList_UnderCover = new List<GameObject>();

    }
    public void RemoveAllPageButtons_UnderCover()
    {
        int ItemNum = PageButtonsListPanel_UnderCover.transform.childCount;
        for (int i = ItemNum - 1; i >= 0; i--)
        {
            Destroy(PageButtonsListPanel_UnderCover.transform.GetChild(i).gameObject);
        }

    }
    #endregion
    public void OpenDirectory(bool isFile = false)
    {
        string path = ItemInfoPath.text;
        if (string.IsNullOrEmpty(path)) return;
        path = path.Replace("/", "\\");
        if (isFile)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("No File: " + path);
                return;
            }
            path = string.Format("/Select, {0}", path);
        }
        else
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError("No Directory: " + path);
                return;
            }
        }
        
        System.Diagnostics.Process.Start("explorer.exe", path);
    }
    public void CloseNotEnoughMoneyPanel()
    {
        NotEnoughMoneyPanel.SetActive(false);
    }
    public void ExitGame()
    {
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
