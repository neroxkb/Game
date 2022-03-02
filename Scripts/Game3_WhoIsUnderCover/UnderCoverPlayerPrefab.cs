using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UnderCoverPlayerPrefab : MonoBehaviour
{
    public int ActorNumber;
    public string PlayerName;
    //public RawImage UnderCover;
    //public RawImage Civilian;
    public GameObject UnderCover;
    public GameObject Civilian;
    public RawImage Avatar;
    public Text Name;
    public Text Statement;

    public GameObject VotePanel;
    public Text VoteNumsText;
    public Button VoteButton;
    public int VoteNums;

    public SteamId steamId;
    public string steamIdStr;
    //public RawImage Identity;
    public GameObject Identity;
    public Vector3 OriginScale;
    public Vector3 BigScale;
    public float ShowIdentitySpeed=100;
    public float TimeDelay = 3f;
    public bool ShowIdentityRunning = false;
    public bool GameOverShowIdentityRunning = false;
    public bool IsUnderCover;
    public bool IsDead;

    public const byte VOTE_EVENT = 101;
    private void OnAwake()
    {

    }
    public void init(SteamId SteamId, int ActorNumber, int UnderCoverActorNumber)
    {
        VoteNums = 0;
        UnderCover.gameObject.SetActive(false);
        Civilian.gameObject.SetActive(false);
        this.ActorNumber = ActorNumber;
        Player player = getPlayerByActorNumber(ActorNumber);
        if (!(player == null))
        {
            PlayerName = player.NickName;
            Name.text = player.NickName;
        }
        else
        {
            Name.text = "Something Wrong";
        }
        if (ActorNumber == UnderCoverActorNumber)
        {
            IsUnderCover = true;
        }
        else
        {
            IsUnderCover = false;
        }
        this.steamId = SteamId;
        steamIdStr = SteamId.ToString();
        setAvatarAsync();
    }
    public void SetStatement(string statement)
    {
        Statement.text = statement;
    }
    
    public Player getPlayerByActorNumber(int ActorNumber)
    {
        Player player = null;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == ActorNumber)
            {
                return p;
            }
        }
        return player;
    }
    public void setImage(bool isUnderCover)
    {
        if (isUnderCover)
        {
            UnderCover.gameObject.SetActive(true);
        }
        else
        {
            Civilian.gameObject.SetActive(true);
        }
    }
    public void reSetImage()
    {
        UnderCover.gameObject.SetActive(false);
        Civilian.gameObject.SetActive(false);
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

    #region Vote
    public void ShowVotePanel()
    {
        VotePanel.SetActive(true);
    }
    public void ShowVoteButton()
    {
        VoteButton.gameObject.SetActive(true);
    }
    public void CloseVoteButton()
    {
        VoteButton.gameObject.SetActive(false);
    }
    public void ResetVoteNum()
    {
        VoteNums = 0;
        VoteNumsText.text = "" + VoteNums;
    }
    public void CloseVotePanel()
    {
        VotePanel.SetActive(false);
        VoteButton.gameObject.SetActive(false);
        VoteNums = 0;
        VoteNumsText.text = ""+VoteNums;
    }
    public void OnClick_Vote()
    {
        object[] datas = new object[] { this.ActorNumber };
        PhotonNetwork.RaiseEvent(VOTE_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
        Debug.Log("RaiseEvent VOTE_EVENT.Vote Number " + ActorNumber);
        UnderCoverUIManager.instance.SetVote(ActorNumber);
        UnderCoverUIManager.instance.CloseVoteButton();
        UnderCoverUIManager.instance.StopVoteCountDown();

        Hashtable props = new Hashtable
            {
                {UnderCoverGameManager.instance.PLAYER_VOTE_OVER, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void AddVote()
    {
        VoteNums+=1;
        VoteNumsText.text = "" + VoteNums;
    }
    #endregion
    #region Dead
    public void Dead()
    {
        ShowIdentity();
        IsDead = true;
    }
    public void ShowIdentity()
    {
        if (IsUnderCover)
        {
            Identity = UnderCover;
        }
        else
        {
            Identity = Civilian;
        }
        Identity.gameObject.SetActive(true);
        OriginScale = Identity.transform.localScale;
        BigScale = new Vector3(10 * OriginScale.x, 10 * OriginScale.y, 1f);
        Identity.transform.localScale = BigScale;
        ShowIdentityRunning = true;
    }
    public void GameOverShowIdentity()
    {
        if (IsUnderCover)
        {
            Identity = UnderCover;
        }
        else
        {
            Identity = Civilian;
        }
        Identity.gameObject.SetActive(true);
        OriginScale = Identity.transform.localScale;
        BigScale = new Vector3(10 * OriginScale.x, 10 * OriginScale.y, 1f);
        Identity.transform.localScale = BigScale;
        GameOverShowIdentityRunning = true;
    }
    public void ShowIdentityUpdate()
    {
        if (!this.ShowIdentityRunning) return;
        //ShowIdentitySpeed = ShowIdentitySpeed + 300 * Time.deltaTime;
        Identity.transform.localScale = Vector3.Lerp(Identity.transform.localScale, OriginScale, 25 * Time.deltaTime);
        //Debug.Log("Identity.transform.localScale " + Identity.transform.localScale);
        if (Identity.transform.localScale.x - OriginScale.x <0.5)
        {
            if (TimeDelay > 0)
            {
                TimeDelay -= Time.deltaTime;
                return;
            }
            
            ShowIdentityRunning = false;
            TimeDelay = 3f;
            if (IsUnderCover)
            {
                UnderCoverGameManager.instance.CivilianWin();
            }
            else
            {
                UnderCoverGameManager.instance.CheckUnderCoverWin();
            }


        }

    }
    public void GameOverShowIdentityUpdate()
    {
        if (!this.GameOverShowIdentityRunning) return;
        //ShowIdentitySpeed = ShowIdentitySpeed + 300 * Time.deltaTime;
        Identity.transform.localScale = Vector3.Lerp(Identity.transform.localScale, OriginScale, 25 * Time.deltaTime);
        //Debug.Log("Identity.transform.localScale " + Identity.transform.localScale);
        if (Identity.transform.localScale.x - OriginScale.x < 0.5)
        {
            

            GameOverShowIdentityRunning = false;
            
        }

    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ShowIdentityUpdate();
        GameOverShowIdentityUpdate();
    }
}
