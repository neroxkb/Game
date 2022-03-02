using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorkshopListEntry : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DownloadingPanel;
    public Text DownloadingText;
    public Text itemInfoText;

    public Image DownloadingProcess;
    public RawImage SelectHightLight;
    public RawImage PreviewImage;
    public bool IsSelect;
    public string Title;
    public string Path;
    Steamworks.Ugc.Item Item;

    public bool ShowDownload = false;
    public void Start()
    {
        this.transform.localScale = new Vector3(0.4f, 0.4f, 1);
    }
    public void Initialize(Steamworks.Ugc.Item item)
    {
        Item = item;
        itemInfoText.text = item.Title;
        if (item.IsDownloading || item.IsDownloadPending)
        {
            
            ShowDownloadProgress();
        }
        StartCoroutine(LoadImageAssert(PreviewImage, item.PreviewImageUrl));
    }
    public void OnClick_Select()
    {
        if (Item.IsDownloading || Item.IsDownloadPending)
        {
            UIManager.instance.ItemInfoSize.text = UIManager.instance.GetSize(Item.DownloadBytesTotal);
        }
        UIManager.instance.UnSelectAllItems();
        UIManager.instance.ShowItemInfo(Item,PreviewImage);
        UIManager.instance.CurrentAnswerItemPrefab = this.gameObject;
        SelectHightLight.gameObject.SetActive(true);
        IsSelect = true;
    }
    public void CancelSelect()
    {
        SelectHightLight.gameObject.SetActive(false);
        IsSelect = false;
    }
    public void ShowDownloadProgress()
    {
        Debug.Log("ShowDownloadProgress");
        
        DownloadingPanel.SetActive(true);
       
    }
    public void Update()
    {
        if (Item.IsDownloading)
        {
            if (IsSelect)
            {
                UIManager.instance.ItemInfoSize.text = UIManager.instance.GetSize(Item.DownloadBytesTotal);
            }
            float progress = Item.DownloadAmount;

            DownloadingProcess.fillAmount = progress;
            DownloadingText.text = (int)(progress * 100) + "%";
            if (progress >= 1)
            {
                DownloadingPanel.SetActive(false);
                //ShowDownload = false;
            }
        }
        else if (Item.IsDownloadPending)
        {
            DownloadingText.text = "Pending";
        }
        else
        {
            DownloadingPanel.SetActive(false);
        }
        
        //Debug.Log("DownloadBytesDownloaded:" + Item.DownloadBytesDownloaded + "    DownloadBytesTotal:" + Item.DownloadBytesTotal + "   DownloadAmount:"+Item.DownloadAmount);
        
        
    }
    public IEnumerator LoadImageAssert(RawImage image,string path)
    {

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

            image.texture = tt;
        }
    }
}
