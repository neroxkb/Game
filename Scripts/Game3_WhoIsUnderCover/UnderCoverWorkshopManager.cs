using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UnderCoverWorkshopManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static UnderCoverWorkshopManager instance;


    private string tempPath = Application.streamingAssetsPath + "/Temp";
    private string WordPairSetPath = Application.streamingAssetsPath + "/WordPairSet";


    public InputField WordAInputField;
    public InputField WordBInputField;
    
    public Text ErrorDisplay;

    public GameObject UnderCoverItemPrefab;
    public GameObject UnderCoverItemListPanel;
    public Text DownloadProgressText;

    public Text UploadProgressText;
    public Slider UploadProgressSlider;
    public GameObject UploadProgressPanel;
    public GameObject ConfirmSubmitPanel;
    public GameObject UploadSuccessPanel;
    public GameObject UploadFailedPanel;
    public GameObject ErrorDisplayPanel;
    public GameObject ErrorDisplayPanel1;
    public GameObject ErrorDisplayPanel2;

    public List<Steamworks.Ugc.Item> UnderCoverItemList;

    public static event Action<float> DownloadProgress;

    public const string WORD_PAIR_TAG = "WordPairItem";

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
        UnderCoverItemList = new List<Steamworks.Ugc.Item>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region utils
    public WordPairData GetData()
    {
        WordPairData wpd = new WordPairData();
        wpd.wordA = WordAInputField.text;
        wpd.wordB = WordBInputField.text;
        return wpd;
    }
    public void writeJson(WordPairData wordPairData, string path)
    {
        string text = JsonUtility.ToJson(wordPairData);
        File.Create(path + "/WordPair.json").Dispose();
        File.WriteAllText(path + "/WordPair.json", text);
    }
    public WordPairData readJson(string assertPath)
    {
        string questionDataPath = assertPath + "/WordPair.json";
        if (File.Exists(questionDataPath))
        {
            print(questionDataPath);
            string textData = File.ReadAllText(questionDataPath);
            print(textData);
            WordPairData wordPairData = JsonUtility.FromJson<WordPairData>(textData);
            return wordPairData;

        }
        else
        {
            return null;
        }
    }
    public WordPairData readJsonFromFile(string filePath)
    {
        string questionDataPath = filePath;
        if (File.Exists(questionDataPath))
        {
            print(questionDataPath);
            string textData = File.ReadAllText(questionDataPath);
            print(textData);
            WordPairData wordPairData = JsonUtility.FromJson<WordPairData>(textData);
            return wordPairData;

        }
        else
        {
            return null;
        }
    }
    public void WriteToLocalWordPair(string lang)
    {
        if (!Directory.Exists(WordPairSetPath))
        {
            Directory.CreateDirectory(WordPairSetPath);
        }
        string WordPairSetFilePath = WordPairSetPath + "/WordPairSet_"+lang+".txt";
        if (!File.Exists(WordPairSetFilePath))
        {
            File.Create(WordPairSetFilePath).Dispose();
        }
        
        string text = WordListToText(UnderCoverItemList);
        File.WriteAllText(WordPairSetFilePath, text);
        Debug.Log(text);
    }
    public string WordListToText(List<Steamworks.Ugc.Item> UnderCoverItemList)
    {
        string text = "";
        foreach (Steamworks.Ugc.Item item in UnderCoverItemList)
        {
            WordPairData wpd = readJson(item.Directory);
            if (wpd != null)
            {
                text = text + wpd.wordA + "," + wpd.wordB + ";";
            }
            
        }
        return text;
    }
    
    public void CreateTempDir(WordPairData wordPairData)
    {
        Directory.CreateDirectory(this.tempPath);
        writeJson(wordPairData, this.tempPath);
    }
    public void DeleteTempDir()
    {
        Directory.Delete(this.tempPath, true);
    }
    public bool CheckData()
    {
        if (WordAInputField.text.Equals(""))
        {
            ErrorDisplayPanel = ErrorDisplayPanel1;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("Word A is necessary"));
            return false;
        }
        if (WordBInputField.text.Equals(""))
        {
            ErrorDisplayPanel = ErrorDisplayPanel2;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("Word B is necessary"));
            return false;
        }

        return true;
    }
    public IEnumerator DisplayError(string info)
    {
        float countTime = 10.0f;
        ErrorDisplay.text = info;
        while (countTime > 0)
        {
            countTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ErrorDisplay.text = "";
    }
    public IEnumerator DisplayErrorPanel()
    {
        float countTime = 1.0f;
        ErrorDisplayPanel.SetActive(true);
        while (countTime > 0)
        {
            countTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ErrorDisplayPanel.SetActive(false);
    }
    public void ClearAddPanel()
    {
        WordAInputField.text = string.Empty;
        WordBInputField.text = string.Empty;
    }
    #endregion
    public void OnClick_SubmitAsync()
    {

        if (CheckData())
        {
            StopAllCoroutines();
            ErrorDisplay.text = "";
            ErrorDisplayPanel.SetActive(false);
            ConfirmSubmitPanel.SetActive(true);

            UploadProgressPanel.SetActive(false);
            UploadSuccessPanel.SetActive(false);
            UploadFailedPanel.SetActive(false);
        }

    }
    public void OnClick_WorkshopURL()
    {
        Steamworks.SteamFriends.OpenWebOverlay("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
    }
    public void OnClick_SubmitConfirm()
    {
        ConfirmSubmitPanel.SetActive(false);
        UploadSuccessPanel.SetActive(false);
        UploadProgressPanel.SetActive(true);
        WordPairData wordpairData = GetData();
        CreateTempDir(wordpairData);
        var t = CreateItem();
    }
    public void OnClick_SubmitCancel()
    {
        ConfirmSubmitPanel.SetActive(false);
        UploadProgressPanel.SetActive(false);
        UploadSuccessPanel.SetActive(false);
        UploadFailedPanel.SetActive(false);
    }
    public void OnClick_SuccessConfirm()
    {
        ConfirmSubmitPanel.SetActive(false);
        UploadProgressPanel.SetActive(false);
        UploadSuccessPanel.SetActive(false);
        UploadFailedPanel.SetActive(false);
        ClearAddPanel();
    }
    public void OnClick_Update()
    {
        
        var t = UpdateItems();
    }
    #region CreateWorkshopItem
    private async Task CreateItem()
    {
        string lang = SettingsManager.instance.GetCurrentLang();
        Directory.CreateDirectory(this.tempPath);
        var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle("WordPair")
                    .WithDescription(WordAInputField.text+" --- "+WordBInputField.text)
                    .WithPublicVisibility()
                    .WithTag(WORD_PAIR_TAG)
                    .WithTag(lang)
                    .WithContent(this.tempPath)
                    .SubmitAsync(new UnderCoverProgressClass());
        Debug.Log(result.Success);
        if (!result.Success)
        {
            //StartCoroutine(DisplayError("submit failed"));
            UploadSuccessPanel.SetActive(false);
            UploadFailedPanel.SetActive(true);
        }
        DeleteTempDir();
    }
    #endregion

    #region Query
    private async Task UpdateItems()
    {
        UnderCoverItemList = new List<Steamworks.Ugc.Item>();
        string lang = SettingsManager.instance.GetCurrentLang();
        var q = Steamworks.Ugc.Query.Items.WithTag(WORD_PAIR_TAG).WithTag(lang);
        int pagenum = 1;
        while (true)
        {
            var page = await q.GetPageAsync(pagenum);
            Debug.Log($"page : {pagenum}");
            if (page.HasValue)
            {
                Debug.Log($"This page has {page.Value.ResultCount}");
                if (page.Value.ResultCount == 0)
                {
                    break;
                }
                foreach (Steamworks.Ugc.Item item in page.Value.Entries)
                {
                    //Debug.Log($"Entry: {entry.Title}");
                    //Debug.Log($"Entry: {entry.Id}");

                    if (item.IsSubscribed)
                    {
                        Debug.Log(item.Title + " is IsSubscribed:" + item.Directory);
                        UnderCoverItemList.Add(item);
                        
                    }
                    else
                    {
                        var sub = await item.Subscribe();

                        var download = await item.DownloadAsync(
                            DownloadProgress = delegate (float progress)
                            {
                                //DownloadProgressText.text = "DownLoading:" + progress;
                                Debug.Log("DownLoading:" + progress);
                            });
                        Debug.Log("Downloaded:" + item.Directory);
                        UnderCoverItemList.Add(item);

                    }
                    
                }
                pagenum++;
            }
            else
            {
                break;
            }
        }
        WriteToLocalWordPair(lang);
        UIManager.instance.ShowUnderCoverItems();
        

    }
    #endregion
}

public class UnderCoverProgressClass : IProgress<float>
{
    float lastvalue = 0;
    public Text ErrorDisplay;
    public Text UploadProgressText;
    public Slider UploadProgressSlider;
    public UnderCoverProgressClass()
    {
        ErrorDisplay = UnderCoverWorkshopManager.instance.ErrorDisplay;
        UploadProgressText = UnderCoverWorkshopManager.instance.UploadProgressText;
        UploadProgressSlider = UnderCoverWorkshopManager.instance.UploadProgressSlider;
    }
    public void Report(float value)
    {

        if (lastvalue >= value)
        {
            return;
        }

        lastvalue = value;
        Debug.Log(value);
        //ErrorDisplay.text = "uploading:" + lastvalue * 100 + "%";
        //Console.WriteLine(value);
        UploadProgressText.text = lastvalue * 100 + "%";
        UploadProgressSlider.value = lastvalue;
        if (lastvalue >= 1)
        {
            UnderCoverWorkshopManager.instance.ConfirmSubmitPanel.SetActive(false);
            UnderCoverWorkshopManager.instance.UploadProgressPanel.SetActive(false);
            UnderCoverWorkshopManager.instance.UploadSuccessPanel.SetActive(true);
            UploadProgressSlider.value = 0;
        }
    }
}
