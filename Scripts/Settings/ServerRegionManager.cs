using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System.Text;

public class ServerRegionManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static ServerRegionManager instance;
    public GameObject ServerRegionCanvas;

    public string CurrentRegionCode;

    public List<RegionUI> RegionUIs;
    

    

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void OnClick_Show()
    {
        ServerRegionCanvas.SetActive(true);
    }
    public void OnClick_Close()
    {
        ServerRegionCanvas.SetActive(false);
    }
    public void OnClick_ConnectToRegion(string region)
    {
        StartGameManager.instance.ConnectToRegion(region);
    }
    public void SetButtonUI()
    {
        Debug.Log("SetButtonUI");
        Debug.Log(PhotonNetwork.CloudRegion);
        Debug.Log(PhotonNetwork.CloudRegion + "\\*");
        foreach (RegionUI regionUI in RegionUIs)
        {
            if (regionUI.RegionCode == PhotonNetwork.CloudRegion || regionUI.RegionCode + "/*" == PhotonNetwork.CloudRegion)
            {
                regionUI.RegionButton.interactable = false;
                CurrentRegionCode = PhotonNetwork.CloudRegion;
                
            }
            else
            {
                regionUI.RegionButton.interactable = true;
            }
        }
    }
    
    void Start()
    {
        

    }
   
    // Update is called once per frame
    void Update()
    {
        //CurrentRegionCode = PhotonNetwork.CloudRegion;


    }

    



}
