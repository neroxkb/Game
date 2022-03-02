using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerGamePlayerPrefab : MonoBehaviour
{
    public int ActorNumber;
    public string PlayerName;
    public Image Wrong;
    public Image Correct;
    public RawImage Avatar;
    public Text Name;

    public SteamId steamId;
    public string steamIdStr;

    public int AnswerCorrectNum;
    public string AnswerNumText;

    private void OnAwake()
    {
        
    }
    public void init(SteamId SteamId,int ActorNumber)
    {
        Wrong.gameObject.SetActive(false);
        Correct.gameObject.SetActive(false);
        this.ActorNumber = ActorNumber;
        AnswerCorrectNum = 0;
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
        this.steamId = SteamId;
        steamIdStr = SteamId.ToString();
        setAvatarAsync();

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
    public void setImage(bool correct)
    {
        if (correct)
        {
            Correct.gameObject.SetActive(true);
            AnswerCorrectNum += 1;
        }
        else
        {
            Wrong.gameObject.SetActive(true);
        }
    }
    public void reSetImage()
    {
        Wrong.gameObject.SetActive(false);
        Correct.gameObject.SetActive(false);
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
