using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRankPrefab : MonoBehaviour
{
    public int RankNum;
    public int Score;
    public SteamId steamId;
    public string steamIdStr;
    public string PlayerName;

    public Text NameText;
    public Text RankNumText;
    public Text ScoreText;
    public Text RankTitleText;
    public RawImage Avatar;
    public RawImage RankMark;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public async void setAvatarAsync()
    {
        var AvatarLarge = await SteamFriends.GetLargeAvatarAsync(this.steamId);
        if (AvatarLarge.HasValue)
        {
            uint imageHeight = AvatarLarge.Value.Height;
            uint imageWidth = AvatarLarge.Value.Width;
            Debug.Log(imageWidth + "x" + imageHeight);
            Texture2D downloadedAvatar = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false);
            downloadedAvatar.LoadRawTextureData(AvatarLarge.Value.Data);
            downloadedAvatar.Apply();

            Avatar.texture = downloadedAvatar;
        }
    }
    public void SetName(string name)
    {
        this.PlayerName = name;
        this.NameText.text = name;
    }
    public void SetSteamID(SteamId id)
    {
        this.steamId = id;
        steamIdStr = this.steamId.ToString();
    }
    public void SetScore(int score)
    {
        this.Score = score;
        this.ScoreText.text = score.ToString();
    }
    public void SetRankNum(int rankNum)
    {
        this.RankNum = rankNum;
        this.RankNumText.text = rankNum.ToString();
    }
    public void SetMark()
    {
        if (Score >= 100000 && RankNum == 1)
        {
            Debug.Log("SetMyMark 6");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[6];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[6].text;
        }
        else if (Score >= 50000 && RankNum == 1)
        {
            Debug.Log("SetMyMark 5");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[5];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[5].text;
        }
        else if (Score >= 10000)
        {
            Debug.Log("SetMyMark 4");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[4];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[4].text;
        }
        else if (Score >= 5000)
        {
            Debug.Log("SetMyMark 3");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[3];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[3].text;
        }
        else if (Score >= 2000)
        {
            Debug.Log("SetMyMark 2");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[2];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[2].text;
        }
        else if (Score >= 500)
        {
            Debug.Log("SetMyMark 1");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[1];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[1].text;
        }
        else
        {
            Debug.Log("SetMyMark 0");
            RankMark.texture = LeaderBoardUIManagger.instance.PreLoad_Marks[0];
            RankTitleText.text = LeaderBoardUIManagger.instance.PreLoad_Titles[0].text;
        }
    }
    public void Initialize(Steamworks.Data.LeaderboardEntry entry)
    {
        Debug.Log("Initialize Rank User:" + entry.User + "  Rank Score:" + entry.Score + "  Global Rank:" + entry.GlobalRank);
        SetName(entry.User.Name);
        SetSteamID(entry.User.Id);
        setAvatarAsync();
        SetScore(entry.Score);
        SetRankNum(entry.GlobalRank);
        SetMark();
    }
}
