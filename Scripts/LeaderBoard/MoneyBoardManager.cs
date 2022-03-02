using Photon.Pun;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MoneyBoardManager : MonoBehaviour
{
    public static MoneyBoardManager instance;
    const string MoneyBoardName = "MoneyBoard";
    public Steamworks.Data.Leaderboard moneyboard;
    private int steamAppId = 1758390;
    const string playerMoneyPrefKey = "PlayerMoney";
    const string playerMoneyBoardKey = "UploadMoneyToBoard";


    private int purchaseRewardMoney = 4000000;
    public int MyCurrentMoney = -1;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("MoneyBoardManager instance not null");
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
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
    bool CheckPurchased()
    {
        System.DateTime pData = SteamApps.PurchaseTime(steamAppId);
        System.DateTime dData = new System.DateTime(2022, 2, 12);
        Debug.Log("PurchaseTime:" + pData.ToString() + "DeadLineTime:" + dData.ToString());
        if (pData.CompareTo(dData) == -1)
        {
            Debug.Log("pData.CompareTo(dData):" + pData.CompareTo(dData));
            return true;
        }
        return false;
    }
    int GetDLC1Money()
    {
        AppId dlcId = new AppId();
        dlcId.Value = 1885220;
        Debug.Log("dlc1install:" + SteamApps.IsDlcInstalled(dlcId));
        if (SteamApps.IsDlcInstalled(dlcId))
        {
            return 1;
        }
        return -1;
    }
    int GetDLC2Money()
    {
        AppId dlcId = new AppId();
        dlcId.Value = 1904080;
        Debug.Log("dlc2install:" + SteamApps.IsDlcInstalled(dlcId));
        if (SteamApps.IsDlcInstalled(dlcId))
        {
            return 1;
        }
        return -1;
    }
    int GetDLC3Money()
    {
        AppId dlcId = new AppId();
        dlcId.Value = 1904140;
        Debug.Log("dlc3install:" + SteamApps.IsDlcInstalled(dlcId));
        if (SteamApps.IsDlcInstalled(dlcId))
        {
            return 1;
        }
        return -1;
    }
    public async System.Threading.Tasks.Task ModifyMoney(int money)
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);

        Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];

        int newMoney = myEntry.Score + money;
        Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(newMoney,myEntry.Details);

        MyCurrentMoney = newMoney;
        Debug.Log("Modify Money To " + newMoney);
    }
    public async System.Threading.Tasks.Task ModifyMoney_Net(int money,Text uiText)
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);

        Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];

        int newMoney = myEntry.Score + money;
        Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(newMoney, myEntry.Details);
        
        uiText.text = moneyToString(newMoney);

        MyCurrentMoney = newMoney;
        Debug.Log("Modify Money To " + newMoney);
    }
    public void ModifyMoney(int money, Text uiText)
    {
        int myMoney = PlayerPrefs.GetInt(playerMoneyPrefKey);

        int newMoney = myMoney + money;

        PlayerPrefs.SetInt(playerMoneyPrefKey,newMoney);

        uiText.text = moneyToString(newMoney);

        MyCurrentMoney = newMoney;
        Debug.Log("Modify Local Money To " + newMoney);
    }
    public async System.Threading.Tasks.Task TestGameLoadMoney(Text uiText)
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);

        Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];

        int money = myEntry.Score;
        Hashtable props = new Hashtable
            {
                {GamePlayer.PLAYER_MONEY, money}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        uiText.text = moneyToString(money);
        Debug.Log("TestGameLoadMoney " + money);
    }
    public async System.Threading.Tasks.Task FirstUploadMoney(int money,Text uiText)
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);
        if (leaderboardEntry != null)
        {
            Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];
            uiText.text = moneyToString(myEntry.Score);
            Debug.Log("First Upload Money Failed, already has money: " + myEntry.Score);
            return;
        }
        
        Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(money);
        uiText.text = moneyToString(money);
        Debug.Log("First Upload Money " + money);
    }
    public async System.Threading.Tasks.Task DLCMoney()
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);
        if (leaderboardEntry != null)
        {
            Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];
            if (myEntry.Details[0] == 1)
            {
                return;
            }
            else
            {
                int newMoney = myEntry.Score + 2000000;
                int[] details = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(newMoney, details);
                Debug.Log("PurchaseRewardMoney: " + newMoney);
            }

        }
    }
    public async System.Threading.Tasks.Task PurchaseRewardMoney()
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);
        if (leaderboardEntry != null)
        {
            Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];
            if (myEntry.Details[0] == 1)
            {
                return;
            }
            else
            {
                int newMoney = myEntry.Score + 2000000;
                int[] details = new int[] {1,0,0,0,0,0,0,0,0,0};
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(newMoney,details);
                Debug.Log("PurchaseRewardMoney: " + newMoney);
            }
            
        }
    }
    public async System.Threading.Tasks.Task LoadMoney_Net(Text uiText)
    {
        Steamworks.Data.Leaderboard? leaderboard = await Steamworks.SteamUserStats.FindLeaderboardAsync(MoneyBoardName);
        this.moneyboard = leaderboard.Value;
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);
        if (leaderboardEntry != null)
        {
            Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];
            int myMoney = myEntry.Score;
            int[] myDetails = myEntry.Details;

            if (CheckPurchased() && myDetails[0] == 0)
            {
                Debug.Log("CheckPurchased");
                myMoney += purchaseRewardMoney;
                myDetails[0] = 1;
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
                UIManager.instance.OnClick_ShowPurchaseRewardCanvas();
            }
            if (GetDLC1Money() == 1 && myDetails[1] == 0)
            {
                myMoney += 500000;
                myDetails[1] = 1;
                UIManager.instance.OnClick_ShowDLC1MoneyCanvas(moneyToString(500000));
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
            }
            if (GetDLC2Money() == 1 && myDetails[2] == 0)
            {
                myMoney += 1500000;
                myDetails[2] = 1;
                UIManager.instance.OnClick_ShowDLC2MoneyCanvas(moneyToString(1500000));
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
            }
            if (GetDLC3Money() == 1 && myDetails[3] == 0)
            {
                myMoney += 3000000;
                myDetails[3] = 1;
                UIManager.instance.OnClick_ShowDLC3MoneyCanvas(moneyToString(3000000));
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
            }
            //Debug.Log("LoadMoneyBoard");
            uiText.text = moneyToString(myMoney);
            MyCurrentMoney = myMoney;
            Debug.Log("LoadMoney: " + uiText.text);
        }
        else
        {
            if (PlayerPrefs.HasKey(playerMoneyPrefKey))
            {//Already Purchased Before
                Debug.Log("LoadMoney  playerMoneyPrefKey");
                int money = PlayerPrefs.GetInt(playerMoneyPrefKey);

                int[] details = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                if (CheckPurchased())
                {
                    Debug.Log("CheckPurchased");
                    money += purchaseRewardMoney;
                    details[0] = 1;
                    UIManager.instance.OnClick_ShowPurchaseRewardCanvas();
                }
                if (GetDLC1Money() == 1)
                {
                    money += 500000;
                    details[1] = 1;
                    UIManager.instance.OnClick_ShowDLC1MoneyCanvas(moneyToString(500000));
                }
                if (GetDLC2Money() == 1)
                {
                    money += 1500000;
                    details[2] = 1;
                    UIManager.instance.OnClick_ShowDLC2MoneyCanvas(moneyToString(1500000));
                }
                if (GetDLC3Money() == 1)
                {
                    money += 3000000;
                    details[3] = 1;
                    UIManager.instance.OnClick_ShowDLC3MoneyCanvas(moneyToString(3000000));
                }
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(money,details);
                uiText.text = moneyToString(money);
                MyCurrentMoney = money;
                //PlayerPrefs.SetInt(playerMoneyBoardKey, 1);
                //PlayerPrefs.DeleteKey(playerMoneyPrefKey);
                //moneyText.text = moneyToString(money);
            }
            else
            {//First in game
                Debug.Log("First in game");
                int money = 10000;

                int[] details = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                if (CheckPurchased())
                {
                    Debug.Log("CheckPurchased");
                    money += purchaseRewardMoney;
                    details[0] = 1;
                    UIManager.instance.OnClick_ShowPurchaseRewardCanvas();
                }
                if (GetDLC1Money() == 1)
                {
                    money += 500000;
                    details[1] = 1;
                    UIManager.instance.OnClick_ShowDLC1MoneyCanvas(moneyToString(500000));
                }
                if (GetDLC2Money() == 1)
                {
                    money += 1500000;
                    details[2] = 1;
                    UIManager.instance.OnClick_ShowDLC2MoneyCanvas(moneyToString(1500000));
                }
                if (GetDLC3Money() == 1)
                {
                    money += 3000000;
                    details[3] = 1;
                    UIManager.instance.OnClick_ShowDLC3MoneyCanvas(moneyToString(3000000));
                }
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(money,details);
                uiText.text = moneyToString(money);
                MyCurrentMoney = money;
                //PlayerPrefs.SetInt(playerMoneyPrefKey, 40000);
            }
        }
    }
    public async System.Threading.Tasks.Task LoadMoney(Text uiText)
    {
        Steamworks.Data.Leaderboard? leaderboard = await Steamworks.SteamUserStats.FindLeaderboardAsync(MoneyBoardName);
        this.moneyboard = leaderboard.Value;
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);
        if (leaderboardEntry != null)
        {
            Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];
            int myMoney = PlayerPrefs.GetInt(playerMoneyPrefKey);
            int[] myDetails = myEntry.Details;

            if (CheckPurchased() && myDetails[0] == 0)
            {
                Debug.Log("CheckPurchased");
                myMoney += purchaseRewardMoney;
                myDetails[0] = 1;
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
                UIManager.instance.OnClick_ShowPurchaseRewardCanvas();
            }
            if (GetDLC1Money() == 1 && myDetails[1] == 0)
            {
                myMoney += 500000;
                myDetails[1] = 1;
                UIManager.instance.OnClick_ShowDLC1MoneyCanvas(moneyToString(500000));
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
            }
            if (GetDLC2Money() == 1 && myDetails[2] == 0)
            {
                myMoney += 1500000;
                myDetails[2] = 1;
                UIManager.instance.OnClick_ShowDLC2MoneyCanvas(moneyToString(1500000));
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
            }
            if (GetDLC3Money() == 1 && myDetails[3] == 0)
            {
                myMoney += 3000000;
                myDetails[3] = 1;
                UIManager.instance.OnClick_ShowDLC3MoneyCanvas(moneyToString(3000000));
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(myMoney, myDetails);
            }
            //Debug.Log("LoadMoneyBoard");
            uiText.text = moneyToString(myMoney);
            MyCurrentMoney = myMoney;
            PlayerPrefs.SetInt(playerMoneyPrefKey, myMoney);
            Debug.Log("LoadMoney: " + uiText.text);
        }
        else
        {
            if (PlayerPrefs.HasKey(playerMoneyPrefKey))
            {//Already Purchased Before
                Debug.Log("LoadMoney  playerMoneyPrefKey");
                int money = PlayerPrefs.GetInt(playerMoneyPrefKey);

                int[] details = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                if (CheckPurchased())
                {
                    Debug.Log("CheckPurchased");
                    money += purchaseRewardMoney;
                    details[0] = 1;
                    UIManager.instance.OnClick_ShowPurchaseRewardCanvas();
                }
                if (GetDLC1Money() == 1)
                {
                    money += 500000;
                    details[1] = 1;
                    UIManager.instance.OnClick_ShowDLC1MoneyCanvas(moneyToString(500000));
                }
                if (GetDLC2Money() == 1)
                {
                    money += 1500000;
                    details[2] = 1;
                    UIManager.instance.OnClick_ShowDLC2MoneyCanvas(moneyToString(1500000));
                }
                if (GetDLC3Money() == 1)
                {
                    money += 3000000;
                    details[3] = 1;
                    UIManager.instance.OnClick_ShowDLC3MoneyCanvas(moneyToString(3000000));
                }
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(money, details);
                uiText.text = moneyToString(money);
                MyCurrentMoney = money;
                PlayerPrefs.SetInt(playerMoneyPrefKey, money);
                //PlayerPrefs.SetInt(playerMoneyBoardKey, 1);
                //PlayerPrefs.DeleteKey(playerMoneyPrefKey);
                //moneyText.text = moneyToString(money);
            }
            else
            {//First in game
                Debug.Log("First in game");
                int money = 10000;

                int[] details = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                if (CheckPurchased())
                {
                    Debug.Log("CheckPurchased");
                    money += purchaseRewardMoney;
                    details[0] = 1;
                    UIManager.instance.OnClick_ShowPurchaseRewardCanvas();
                }
                if (GetDLC1Money() == 1)
                {
                    money += 500000;
                    details[1] = 1;
                    UIManager.instance.OnClick_ShowDLC1MoneyCanvas(moneyToString(500000));
                }
                if (GetDLC2Money() == 1)
                {
                    money += 1500000;
                    details[2] = 1;
                    UIManager.instance.OnClick_ShowDLC2MoneyCanvas(moneyToString(1500000));
                }
                if (GetDLC3Money() == 1)
                {
                    money += 3000000;
                    details[3] = 1;
                    UIManager.instance.OnClick_ShowDLC3MoneyCanvas(moneyToString(3000000));
                }
                Steamworks.Data.LeaderboardUpdate? update = await this.moneyboard.ReplaceScore(money, details);
                uiText.text = moneyToString(money);
                MyCurrentMoney = money;
                PlayerPrefs.SetInt(playerMoneyPrefKey, money);
                //PlayerPrefs.SetInt(playerMoneyPrefKey, 40000);
            }
        }
    }
    public async System.Threading.Tasks.Task SetUIMoney(Text uiText)
    {
        Steamworks.Data.LeaderboardEntry[] leaderboardEntry = await this.moneyboard.GetScoresAroundUserAsync(0, 0);

        Steamworks.Data.LeaderboardEntry myEntry = leaderboardEntry[0];

        int myMoney = myEntry.Score;
        uiText.text = moneyToString(myMoney);

        Debug.Log("Set UI Money " + uiText.text);
    }
    // Start is called before the first frame update
    public async System.Threading.Tasks.Task GetLeaderBoardAsync()
    {
        Steamworks.Data.Leaderboard? leaderboard = await Steamworks.SteamUserStats.FindLeaderboardAsync(MoneyBoardName);
        this.moneyboard = leaderboard.Value;
        Debug.Log("MoneyBoard " + leaderboard.Value);
    }
    // Start is called before the first frame update
    void Start()
    {
        var t = LoadMoney(StartGameManager.instance.moneyText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
