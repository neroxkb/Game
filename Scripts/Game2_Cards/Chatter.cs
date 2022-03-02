using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;

public class Chatter : MonoBehaviourPunCallbacks
{
    private PhotonView photonView;
    public Text messageInput;
    public GameObject PlayerMessagePrefab;
    public GameObject MessageListContent;
    public ScrollRect scrollRect;
    public bool isOn = false;

    public GameObject messagesScroll;
    public GameObject InputField;
    public GameObject SendButton;
    public GameObject ShowButton;

    public Vector3 offset = new Vector3(-60, -55, 0);
    public void OnClick_ShowChatter()
    {
        if (isOn)
        {
            isOn = !isOn;
            messagesScroll.SetActive(false);
            InputField.SetActive(false);
            SendButton.SetActive(false);
        }
        else
        {
            isOn = !isOn;
            messagesScroll.SetActive(true);
            InputField.SetActive(true);
            SendButton.SetActive(true);
        }
    }
    public void OnClick_Send()
    {
 
        Debug.Log("OnClick_Send");

        if (string.IsNullOrEmpty(messageInput.text))
        {
            Debug.LogError("Player  is null or empty:" + messageInput.text);
            return;
        }
        string message = messageInput.text;
        this.InputField.GetComponent<InputField>().text = string.Empty;
        photonView.RPC("addNewMessage", RpcTarget.All,PhotonNetwork.LocalPlayer.NickName,message);
        


    }
    public void SetOffset()
    {
        
        messagesScroll.transform.position+=offset;
        InputField.transform.position += offset;
        SendButton.transform.position += offset;
        ShowButton.transform.position += offset;
    }
    [PunRPC]
    void showChatter()
    {
        if (!isOn)
        {
            isOn = !isOn;
            messagesScroll.SetActive(true);
            InputField.SetActive(true);
            SendButton.SetActive(true);
        }
    }
    [PunRPC]
    void addNewMessage(string name,string message)
    {
        if (!isOn)
        {
            OnClick_ShowChatter();
        }
        GameObject messageEntry = Instantiate(PlayerMessagePrefab, MessageListContent.transform);
        //messageEntry.transform.SetParent(MessageListContent.transform);
        messageEntry.transform.localScale = Vector3.one;
        messageEntry.GetComponent<PlayerMessageEntry>().Initialize(name, message);
        //this.GetComponent<Canvas>().ForceUpdateCanvases();

        Canvas.ForceUpdateCanvases();       
        scrollRect.verticalNormalizedPosition = 0f;  
        Canvas.ForceUpdateCanvases();  

    }
    IEnumerator InsSrollBar()
    {

        yield return new WaitForEndOfFrame();

    }
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isOn);
        
        
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && isOn)
        {
            Debug.Log(isOn);
            OnClick_Send();
        }
        if (Input.GetKeyDown(KeyCode.Return) && isOn)
        {
            Debug.Log(isOn);
            OnClick_Send();
        }
        
    }
}
