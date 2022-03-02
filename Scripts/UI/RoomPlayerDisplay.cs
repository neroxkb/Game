using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomPlayerDisplay : MonoBehaviour
{
    public int ActorNumber;
    public string PlayerName;
    public RawImage Avatar;
    public Text Name;

    public SteamId steamId;
    public string steamIdStr;

    // Start is called before the first frame update
    public void init(SteamId SteamId, int ActorNumber)
    {
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
