using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Runtime.InteropServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Threading.Tasks;

public class AnswerPlayerManager : MonoBehaviourPunCallbacks
{
    public static AnswerPlayerManager instance;
    public string PLAYER_ANSWER_OVER = "playerAnswerOver";
    public string PLAYER_ANSWER_CORRECT = "playerAnswerCorrect";
    public int AnswerCorrectNum;
    public RawImage Avatar;
    private void OnEnable()
    {
        
    }
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
        AnswerCorrectNum = 0;
        var t = getSteamAvatarAsync();
    }
    public void SetProp_AnswerOver()
    {
        Hashtable props = new Hashtable
            {
                {PLAYER_ANSWER_OVER, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void SetProp_AnswerCorrect(bool correct)
    {
        Hashtable props = new Hashtable
            {
                {PLAYER_ANSWER_CORRECT, correct}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #region On_Click
    public void OnClick_A()
    {
        Debug.Log("OnClick_A");
        AnswerUIManager.instance.ButtonA_On.SetActive(false);
        AnswerUIManager.instance.ButtonA_Off.SetActive(true);
        AnswerUIManager.instance.ButtonA.interactable = false;
        AnswerUIManager.instance.ButtonB.interactable = false;
        AnswerUIManager.instance.ButtonC.interactable = false;
        AnswerUIManager.instance.ButtonD.interactable = false;
        //AnswerUIManager.instance.StopPlayerChoiceCountDown();
        if (AnswerUIManager.instance.correctA)
        {
            SetTableGreen();
            AnswerCorrectNum += 1;
            AnswerUIManager.instance.CorrectAudio.Play();
            SetProp_AnswerCorrect(true);
            Debug.Log("Answer correct.");
        }
        else
        {
            SetTableRed();
            AnswerUIManager.instance.WrongAudio.Play();
            SetProp_AnswerCorrect(false);
            Debug.Log("Answer wrong.");
        }
        StartCoroutine(Delay_SetProp());
        //SetProp_AnswerOver();
    }
    public void OnClick_B()
    {
        AnswerUIManager.instance.ButtonB_On.SetActive(false);
        AnswerUIManager.instance.ButtonB_Off.SetActive(true);
        AnswerUIManager.instance.ButtonA.interactable = false;
        AnswerUIManager.instance.ButtonB.interactable = false;
        AnswerUIManager.instance.ButtonC.interactable = false;
        AnswerUIManager.instance.ButtonD.interactable = false;
        //AnswerUIManager.instance.StopPlayerChoiceCountDown();
        Debug.Log("OnClick_B");
        if (AnswerUIManager.instance.correctB)
        {
            SetTableGreen();
            AnswerCorrectNum += 1;
            AnswerUIManager.instance.CorrectAudio.Play();
            SetProp_AnswerCorrect(true);
            Debug.Log("Answer correct.");
        }
        else
        {
            SetTableRed();
            AnswerUIManager.instance.WrongAudio.Play();
            SetProp_AnswerCorrect(false);
            Debug.Log("Answer wrong.");
        }
        StartCoroutine(Delay_SetProp());
        //SetProp_AnswerOver();
    }
    public void OnClick_C()
    {
        AnswerUIManager.instance.ButtonC_On.SetActive(false);
        AnswerUIManager.instance.ButtonC_Off.SetActive(true);
        AnswerUIManager.instance.ButtonA.interactable = false;
        AnswerUIManager.instance.ButtonB.interactable = false;
        AnswerUIManager.instance.ButtonC.interactable = false;
        AnswerUIManager.instance.ButtonD.interactable = false;
        //AnswerUIManager.instance.StopPlayerChoiceCountDown();
        Debug.Log("OnClick_C");
        if (AnswerUIManager.instance.correctC)
        {
            SetTableGreen();
            AnswerCorrectNum += 1;
            AnswerUIManager.instance.CorrectAudio.Play();
            SetProp_AnswerCorrect(true);
            Debug.Log("Answer correct.");
        }
        else
        {
            SetTableRed();
            AnswerUIManager.instance.WrongAudio.Play();
            SetProp_AnswerCorrect(false);
            Debug.Log("Answer wrong.");
        }
        StartCoroutine(Delay_SetProp());
        //SetProp_AnswerOver();
    }
    public void OnClick_D()
    {
        AnswerUIManager.instance.ButtonD_On.SetActive(false);
        AnswerUIManager.instance.ButtonD_Off.SetActive(true);
        AnswerUIManager.instance.ButtonA.interactable = false;
        AnswerUIManager.instance.ButtonB.interactable = false;
        AnswerUIManager.instance.ButtonC.interactable = false;
        AnswerUIManager.instance.ButtonD.interactable = false;
        //AnswerUIManager.instance.StopPlayerChoiceCountDown();
        Debug.Log("OnClick_D");
        if (AnswerUIManager.instance.correctD)
        {
            SetTableGreen();
            AnswerCorrectNum += 1;
            AnswerUIManager.instance.CorrectAudio.Play();
            SetProp_AnswerCorrect(true);
            Debug.Log("Answer correct.");
        }
        else
        {
            SetTableRed();
            AnswerUIManager.instance.WrongAudio.Play();
            SetProp_AnswerCorrect(false);
            Debug.Log("Answer wrong.");
        }
        StartCoroutine(Delay_SetProp());
        //SetProp_AnswerOver();
    }

    public void OnClick_Confirm()
    {
        AnswerUIManager.instance.SetActive_DownloadingCanvas();
        AnswerGameManager.instance.DownloadItemsAndStartGame();
    }
    public void OnClick_Cancel()
    {
        Debug.Log("OnClick_Cancel");
        AnswerGameManager.instance.LeaveRoom();
    }


    #endregion
    #region utils
    private IEnumerator Delay_SetProp()
    {
        yield return new WaitForSeconds(3f);
        // AssetBundle ab = DownloadHandlerAssetBundle.GetContent (request );
        SetProp_AnswerOver();
    }
    public void ChoiceTimeOver()
    {
        if (!AnswerUIManager.instance.Table_base.activeSelf)
        {
            return;
        }
        SetTableRed();
        AnswerUIManager.instance.WrongAudio.Play();
        SetProp_AnswerCorrect(false);
        StartCoroutine(Delay_SetProp());
    }
    public void SetTableRed()
    {
        AnswerUIManager.instance.Table_base.SetActive(false);
        AnswerUIManager.instance.Table_red.SetActive(true);
    }
    public void SetTableGreen()
    {
        AnswerUIManager.instance.Table_base.SetActive(false);
        AnswerUIManager.instance.Table_green.SetActive(true);
    }
    async Task getSteamAvatarAsync()
    {
        var AvatarLarge = await SteamFriends.GetLargeAvatarAsync(Steamworks.SteamClient.SteamId);
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
    #endregion


}
