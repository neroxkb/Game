using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Runtime.InteropServices;
using UnityEditor;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class AnswerWorkshopManager : MonoBehaviour
{
    /* private CallResult<CreateItemResult_t> cr_CreateItem;
     private CallResult<SubmitItemUpdateResult_t> cr_SubmitItem;
     private CallResult<SteamUGCQueryCompleted_t> cr_QueryComplete;
     bool startUpdate;
     UGCUpdateHandle_t updateHandle;*/
    public static AnswerWorkshopManager instance;
    public string defaultPreviewPath = Application.streamingAssetsPath + "/CharacterStand.png";
    public Text openFilePath;
    private string objectFilePath="";
    private string previewImagePath = "";
    public RawImage previewImage;
    private string tempPath = Application.streamingAssetsPath+"/Temp";

    public InputField titleInputFiled;
    public InputField descriptionInputFiled;
    public InputField questionInputFiled;
    public InputField answerAInputFiled;
    public InputField answerBInputFiled;
    public InputField answerCInputFiled;
    public InputField answerDInputFiled;
    public Dropdown TagDropDown;
    public Dropdown UploadTagDropDown;
    public Toggle toggleA;
    public Toggle toggleB;
    public Toggle toggleC;
    public Toggle toggleD;
    public string questionObjectName="";
    public string previewName = "";
    public string otherInfo;

    public Text ErrorDisplay;
    public Text UploadProgressText;
    public Slider UploadProgressSlider;

    public GameObject WorkshopListEntry;
    public GameObject WorkshopListContent;
    public GameObject UploadProgressPanel;
    public GameObject ConfirmSubmitPanel;
    public GameObject UploadSuccessPanel;
    public GameObject UploadFailedPanel;
    public GameObject ErrorDisplayPanel;
    public GameObject ErrorDisplayPanel1;
    public GameObject ErrorDisplayPanel2;
    public GameObject ErrorDisplayPanel3;
    public GameObject ErrorDisplayPanel4;
    public GameObject ErrorDisplayPanel5;

    public List<Steamworks.Ugc.Item> InstalledAnswerItemList;
    public List<Steamworks.Ugc.Item> UninstalledAnswerItemList;
    public List<string> TagOptions;
    public const string QUESTION_TAG = "default";
    public static event Action<float> DownloadProgress;
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
        InstalledAnswerItemList = new List<Steamworks.Ugc.Item>();
        UninstalledAnswerItemList = new List<Steamworks.Ugc.Item>();
        toggleA.onValueChanged.AddListener((bool value) => OnClick_ToggleA(toggleA, value));
        toggleB.onValueChanged.AddListener((bool value) => OnClick_ToggleB(toggleB, value));
        toggleC.onValueChanged.AddListener((bool value) => OnClick_ToggleC(toggleC, value));
        toggleD.onValueChanged.AddListener((bool value) => OnClick_ToggleD(toggleD, value));
    }

    #region On_Click
    
    public void OnClick_OpenFile()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "All files (*.*)|*.*";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = " ";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetOpenFileName(openFileName))
        {
            Debug.Log(openFileName.file);
            Debug.Log(openFileName.structSize);
            Debug.Log(openFileName.fileTitle);
            openFilePath.text = openFileName.file;
            objectFilePath = openFileName.file;
            this.questionObjectName = openFileName.fileTitle;
            
        }
    }
    public void OnClick_OpenPreviewFile()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "image(*.jpg*.png)\0*.jpg;*.png";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = " ";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetOpenFileName(openFileName))
        {
            Debug.Log(openFileName.file);
            Debug.Log(openFileName.structSize);
            Debug.Log(openFileName.fileTitle);
            previewImagePath = openFileName.file;
            previewName = openFileName.fileTitle;
            StartCoroutine(LoadImageAssert(previewImagePath));
            
        }
    }
    public void OnClick_ParseFromDictory()
    {
        string path = "C:\\steam\\steamapps\\workshop\\content\\1758390";
        DirectoryInfo direction = new DirectoryInfo(path);

        
        DirectoryInfo[] folders = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);
        for (int i = 0; i < folders.Length; i++)
        {
            Debug.Log(folders[i].FullName);
            FileInfo[] files = folders[i].GetFiles("*", SearchOption.TopDirectoryOnly);
            for (int j = 0; j < files.Length; j++)
            {
                if (files[j].Name == "Question.json")//取脚本文件
                {
                    QuestionData questionData = AnswerFileManager.instance.readJsonFromFile(files[j].FullName);
                    Debug.Log(files[j].DirectoryName);
                    Debug.Log(files[j].FullName);
                    //ParseJsonToUI(questionData);
                }
            }
        }

        

    }
    public void OnClick_SubmitAsync()
    {
        
        if (CheckQuestionData())
        {
            StopAllCoroutines();
            ErrorDisplay.text = "";
            ErrorDisplayPanel.SetActive(false);
            ConfirmSubmitPanel.SetActive(true);
            
            UploadProgressPanel.SetActive(false);
            UploadSuccessPanel.SetActive(false);
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
        QuestionData questionData = GetQuestionData();
        CreateTempDir(questionData);
        var t = CreateItem();
    }
    public void OnClick_SubmitCancel()
    {
        ConfirmSubmitPanel.SetActive(false);
        UploadProgressPanel.SetActive(false);
        UploadSuccessPanel.SetActive(false);
    }
    public void OnClick_SuccessConfirm()
    {
        ConfirmSubmitPanel.SetActive(false);
        UploadProgressPanel.SetActive(false);
        UploadSuccessPanel.SetActive(false);
        UploadFailedPanel.SetActive(false);
        //ClearCanvas();
    }
    public void OnClick_FailedConfirm()
    {
        ConfirmSubmitPanel.SetActive(false);
        UploadProgressPanel.SetActive(false);
        UploadSuccessPanel.SetActive(false);
        UploadFailedPanel.SetActive(false);
        //ClearCanvas();
    }
    public void OnClick_Query(int QueryCode)
    {
        var t = QueryItem(QueryCode);
    }
    public void OnClick_ToggleA(Toggle toggle, bool value)
    {
        if (value)
        {
            toggleB.gameObject.SetActive(false);
            toggleC.gameObject.SetActive(false);
            toggleD.gameObject.SetActive(false);
        }
        else
        {
            toggleB.gameObject.SetActive(true);
            toggleC.gameObject.SetActive(true);
            toggleD.gameObject.SetActive(true);
        }
    }
    public void OnClick_ToggleB(Toggle toggle, bool value)
    {
        if (value)
        {
            toggleA.gameObject.SetActive(false);
            toggleC.gameObject.SetActive(false);
            toggleD.gameObject.SetActive(false);
        }
        else
        {
            toggleA.gameObject.SetActive(true);
            toggleC.gameObject.SetActive(true);
            toggleD.gameObject.SetActive(true);
        }
    }
    public void OnClick_ToggleC(Toggle toggle, bool value)
    {
        if (value)
        {
            toggleA.gameObject.SetActive(false);
            toggleB.gameObject.SetActive(false);
            toggleD.gameObject.SetActive(false);
        }
        else
        {
            toggleA.gameObject.SetActive(true);
            toggleB.gameObject.SetActive(true);
            toggleD.gameObject.SetActive(true);
        }
    }
    public void OnClick_ToggleD(Toggle toggle, bool value)
    {
        if (value)
        {
            toggleA.gameObject.SetActive(false);
            toggleB.gameObject.SetActive(false);
            toggleC.gameObject.SetActive(false);
        }
        else
        {
            toggleA.gameObject.SetActive(true);
            toggleB.gameObject.SetActive(true);
            toggleC.gameObject.SetActive(true);
        }
    }
    #endregion

    #region utils
    private IEnumerator LoadImageAssert(string path)
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

            previewImage.texture = tt;
        }
    }
    public void ParseJsonToUI(QuestionData questionData)
    {

        SetUploadTag(questionData.tag);
        string lang = SettingsManager.instance.GetCurrentLang();

        questionData.question = questionInputFiled.text;
        questionData.answerA = answerAInputFiled.text;
        questionData.answerB = answerBInputFiled.text;
        questionData.answerC = answerCInputFiled.text;
        questionData.answerD = answerDInputFiled.text;
        questionData.correctA = toggleA.isOn;
        questionData.correctB = toggleB.isOn;
        questionData.correctC = toggleC.isOn;
        questionData.correctD = toggleD.isOn;
        questionData.objectName = this.questionObjectName;
        questionData.tag = tag;
        questionData.lang = lang;
    }
    public void SetUploadTag(string tag)
    {
        List<string> Tags = TagManager.instance.TagOptions_English;
        int tagIndex = 0;
        for (int i = 0; i < Tags.Count; i++)
        {
            if (Tags[i] == tag)
            {
                tagIndex = i;
            }
        }
        UploadTagDropDown.value = tagIndex;
    }
    public void clearList()
    {
        for (int i = WorkshopListContent.transform.childCount-1; i >=0 ; i--)
        {
            Destroy(WorkshopListContent.transform.GetChild(i).gameObject);
        }
        
    }
    
    public bool CheckQuestionData()
    {
        int answerCount = 0;
        if (questionInputFiled.text.Equals(""))
        {
            ErrorDisplayPanel = ErrorDisplayPanel2;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("Question is necessary"));
            return false;
        }
        if (titleInputFiled.text.Equals(""))
        {
            ErrorDisplayPanel = ErrorDisplayPanel3;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("Title is necessary"));
            return false;
        }
        if (!answerAInputFiled.text.Equals("")) answerCount++;
        if (!answerBInputFiled.text.Equals("")) answerCount++;
        if (!answerCInputFiled.text.Equals("")) answerCount++;
        if (!answerDInputFiled.text.Equals("")) answerCount++;
        if (answerCount < 2)
        {
            ErrorDisplayPanel = ErrorDisplayPanel1;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("Two answers is necessary"));
            return false;
        }
        if (!toggleA.isOn && !toggleB.isOn && !toggleC.isOn && !toggleD.isOn)
        {
            ErrorDisplayPanel = ErrorDisplayPanel4;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("A correct answer is necessary"));
            return false;
        }
        if ((toggleA.isOn && answerAInputFiled.text.Equals("")) || (toggleB.isOn && answerBInputFiled.text.Equals(""))
            || (toggleC.isOn && answerCInputFiled.text.Equals("")) || (toggleD.isOn && answerDInputFiled.text.Equals("")))
        {
            ErrorDisplayPanel = ErrorDisplayPanel1;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("A correct answer should not be empty"));
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
    public QuestionData GetQuestionData()
    {
        int tagIndex = UploadTagDropDown.value;
        string tag = TagManager.instance.TagOptions_English[tagIndex];
        string lang = SettingsManager.instance.GetCurrentLang();

        QuestionData questionData = new QuestionData();
        questionData.question = questionInputFiled.text;
        questionData.answerA = answerAInputFiled.text;
        questionData.answerB = answerBInputFiled.text;
        questionData.answerC = answerCInputFiled.text;
        questionData.answerD = answerDInputFiled.text;
        questionData.correctA = toggleA.isOn;
        questionData.correctB = toggleB.isOn;
        questionData.correctC = toggleC.isOn;
        questionData.correctD = toggleD.isOn;
        questionData.objectName = this.questionObjectName;
        questionData.tag = tag;
        questionData.lang = lang;
        questionData.title = titleInputFiled.text;
        questionData.description = descriptionInputFiled.text;
        return questionData;
    }
    public void CreateTempDir(QuestionData questionData)
    {
        if (Directory.Exists(this.tempPath))
        {
            DeleteTempDir();
        }
        Directory.CreateDirectory(this.tempPath);
        
        if (!this.objectFilePath.Equals(""))
        {
            CopyObject();
        }
        if (!this.previewImagePath.Equals(""))
        {
            CopyPreview();
        }
        print(questionData.question);
        print(this.tempPath);
        AnswerFileManager.instance.writeJson(questionData, this.tempPath);
    }

    public void DeleteTempDir()
    {
        Directory.Delete(this.tempPath,true);
    }
    public void CopyObject()
    {
        File.Copy(objectFilePath, this.tempPath + "/" + this.questionObjectName);
    }
    public void CopyPreview()
    {
        Debug.Log("previewName:"+previewName);
        string[] sp = previewName.Split('.');
        if (sp.Length != 2)
        {
            ErrorDisplayPanel = ErrorDisplayPanel5;
            StartCoroutine(DisplayErrorPanel());
            //StartCoroutine(DisplayError("preview file error!"));
            //File.Copy(defaultPreviewPath, this.tempPath + "/Preview.png");
            return;
        }
        string name = sp[0];
        string extend = sp[1];
        if (!File.Exists(this.tempPath + "/Preview." + extend))
        {
            File.Copy(previewImagePath, this.tempPath + "/Preview." + extend);
        }
        else
        {
            File.Copy(previewImagePath, this.tempPath + "/Preview(1)." + extend);
        }
        
    }
    public void ClearCanvas()
    {
        previewImage.texture = null;
        questionInputFiled.text=string.Empty;
        answerAInputFiled.text = string.Empty;
        answerBInputFiled.text = string.Empty;
        answerCInputFiled.text = string.Empty;
        answerDInputFiled.text = string.Empty;
        titleInputFiled.text = string.Empty;
        descriptionInputFiled.text = string.Empty;
        toggleA.isOn=false;
        toggleB.isOn = false;
        toggleC.isOn = false;
        toggleD.isOn = false;
        questionObjectName = string.Empty;
        previewName = string.Empty;
        openFilePath.text = string.Empty;
        ErrorDisplay.text = string.Empty;
        ErrorDisplayPanel.SetActive(false);
        ErrorDisplayPanel1.SetActive(false);
        ErrorDisplayPanel2.SetActive(false);
        ErrorDisplayPanel3.SetActive(false);
        ErrorDisplayPanel4.SetActive(false);
        ErrorDisplayPanel5.SetActive(false);
        toggleA.gameObject.SetActive(true);
        toggleB.gameObject.SetActive(true);
        toggleC.gameObject.SetActive(true);
        toggleD.gameObject.SetActive(true);


        objectFilePath = "";
        previewImagePath = "";
        UploadTagDropDown.value = 0;

    }
    #endregion

    #region CreateWorkshopItem
    private async Task CreateItem()
    {
        int tagIndex = UploadTagDropDown.value;
        string tag = TagManager.instance.TagOptions_English[tagIndex];
        Debug.Log("Create item with tag:" + tag);
        string lang = SettingsManager.instance.GetCurrentLang();
        
        if (tag == QUESTION_TAG)
        {
            var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle(titleInputFiled.text)
                    .WithDescription(descriptionInputFiled.text)
                    .WithPublicVisibility()
                    .WithPreviewFile(this.previewImagePath)
                    .WithContent(this.tempPath)
                    .WithTag(QUESTION_TAG)
                    .WithTag(lang)
                    .SubmitAsync(new ProgressClass());
            Debug.Log(result.Success);
            if (!result.Success)
            {
                //StartCoroutine(DisplayError("submit failed"));
                UploadSuccessPanel.SetActive(false);
                UploadFailedPanel.SetActive(true);
            }
            DeleteTempDir();
        }
        else if (tag == "r-18+")
        {
            var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle(titleInputFiled.text)
                    .WithDescription(descriptionInputFiled.text)
                    .WithPublicVisibility()
                    .WithPreviewFile(this.previewImagePath)
                    .WithContent(this.tempPath)
                    .WithTag(QUESTION_TAG)
                    .WithTag(tag)
                    .WithTag("chinese").WithTag("english").WithTag("russian").WithTag("french")
                    .WithTag("german").WithTag("korean").WithTag("japanese").WithTag("spanish").WithTag("portuguese")
                    .SubmitAsync(new ProgressClass());
            Debug.Log(result.Success);
            if (!result.Success)
            {
                //StartCoroutine(DisplayError("submit failed"));
                UploadSuccessPanel.SetActive(false);
                UploadFailedPanel.SetActive(true);
            }
            DeleteTempDir();
        }
        else
        {
            var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle(titleInputFiled.text)
                    .WithDescription(descriptionInputFiled.text)
                    .WithPublicVisibility()
                    .WithPreviewFile(this.previewImagePath)
                    .WithContent(this.tempPath)
                    .WithTag(QUESTION_TAG)
                    .WithTag(lang)
                    .WithTag(tag)
                    .SubmitAsync(new ProgressClass());
            Debug.Log(result.Success);
            if (!result.Success)
            {
                Debug.Log(result.GetType());
                UploadSuccessPanel.SetActive(false);
                UploadFailedPanel.SetActive(true);
                //StartCoroutine(DisplayError("submit failed"));
            }
            DeleteTempDir();
        }
        
        
    }
    void CreateWorkshopItem()
    {
        /*SteamAPICall_t handle = SteamUGC.CreateItem((AppId_t)1758390, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        cr_CreateItem.Set(handle);*/
    }

    /*void CreateItemFinished(CreateItemResult_t result, bool bIOFailure)
    {
        print("CreateItemFinished " + result.m_eResult.ToString());
        if (result.m_eResult == EResult.k_EResultOK)
        {
            PublishedFileId_t fileID = result.m_nPublishedFileId;
            UGCUpdateHandle_t handle = SteamUGC.StartItemUpdate((AppId_t)1758390, fileID);
            updateHandle = handle;
            startUpdate = true;
            //set
            bool setItemTitleSuccess = SteamUGC.SetItemTitle(handle, "WorkShop Test");
            bool setItemDescriptionSuccess = SteamUGC.SetItemDescription(handle, "Test");
            bool setVisibilitySuccess = SteamUGC.SetItemVisibility(handle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
            //set content
            //bool setContentSuccess = SteamUGC.SetItemContent(handle, "J:/WorkShopTest");//content path
            bool setContentSuccess = SteamUGC.SetItemContent(handle, this.tempPath);//content path

            bool setItemPreviewSuccess = SteamUGC.SetItemPreview(handle, "J:/WorkShopTest/OnePair.jpg");
            print(setContentSuccess + " " + setItemPreviewSuccess + " " + setItemTitleSuccess
                + " " + setItemDescriptionSuccess + " " + setVisibilitySuccess);

            SteamAPICall_t submitHandle = SteamUGC.SubmitItemUpdate(handle, "change note test");
            cr_SubmitItem.Set(submitHandle);
        }
    }*/
    /*void SubmitItemFinished(SubmitItemUpdateResult_t result, bool bIOFailure)
    {
        print("SubmitItemFinished " + result.m_eResult.ToString());
        if (result.m_eResult == EResult.k_EResultOK)
        {
            print("Submit Item ok");
            DeleteTempDir();
        }
        
    }*/
    #endregion

    #region Query
    private async Task QueryItem(int QueryCode)
    {
        string lang = SettingsManager.instance.GetCurrentLang();
        InstalledAnswerItemList = new List<Steamworks.Ugc.Item>();
        UninstalledAnswerItemList = new List<Steamworks.Ugc.Item>();
        var q = Steamworks.Ugc.Query.Items.WithTag(QUESTION_TAG).WithTag(lang);
        
        //var q = Steamworks.Ugc.Query.Screenshots.CreatedByFollowedUsers();
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
               
                foreach (Steamworks.Ugc.Item entry in page.Value.Entries)
                {
                    //Debug.Log($"Entry: {entry.Title}");
                    //Debug.Log($"Entry: {entry.Id}");
                    if (entry.IsSubscribed && !entry.IsDownloading && !entry.IsDownloadPending)
                    {
                        
                        Debug.Log(entry.Title + " is IsSubscribed:" + entry.Directory);
                        //addNewItemEntry(entry.Title, entry.Directory,entry.IsInstalled);
                        InstalledAnswerItemList.Add(entry);
                        
                    }
                    else
                    {
                        /*Debug.Log(entry.Title + " is not installed");
                        var sub = await entry.Subscribe();

                        var download = await entry.DownloadAsync(
                            DownloadProgress = delegate (float progress)
                              {
                                  Debug.Log("DownLoading:" + progress);
                              });
                        //Debug.Log("Dwonload " + download);
                        Debug.Log("Downloaded:" + entry.Directory);*/
                        //addNewItemEntry(entry.Title, entry.Directory, entry.IsInstalled);
                        UninstalledAnswerItemList.Add(entry);
                        
                    }
                    //var del=await Steamworks.SteamUGC.DeleteFileAsync(entry.Id);
                }
                pagenum++;
            }
            else
            {
                break;
            }
        }
        if (QueryCode == UIManager.instance.QUERY_AND_SHOW)
        {
            UIManager.instance.ShowAnswerItemsWithTag(QUESTION_TAG);
        }
        
    }
    
    public List<Steamworks.Ugc.Item> GetInstalledAnswerItemListWithTag(string Tag)
    {
        Debug.Log(Tag);
        if (Tag == "r-18+")
        {
            List<Steamworks.Ugc.Item> installedAnswerItemList = new List<Steamworks.Ugc.Item>();
            foreach (Steamworks.Ugc.Item item in InstalledAnswerItemList)
            {
                string[] itemTags = item.Tags;
                List<string> TagList = new List<string>();
                foreach (string tag in itemTags)
                {
                    TagList.Add(tag);
                }
                if (TagList.Contains(Tag))
                {
                    installedAnswerItemList.Add(item);
                }
            }
            return installedAnswerItemList;
        }
        else
        {
            List<Steamworks.Ugc.Item> installedAnswerItemList = new List<Steamworks.Ugc.Item>();
            foreach (Steamworks.Ugc.Item item in InstalledAnswerItemList)
            {
                string[] itemTags = item.Tags;
                List<string> TagList = new List<string>();
                foreach (string tag in itemTags)
                {
                    TagList.Add(tag);
                }
                if (TagList.Contains(Tag) && !TagList.Contains("r-18+"))
                {
                    installedAnswerItemList.Add(item);
                }
            }
            return installedAnswerItemList;
        }
        
    }
    public List<Steamworks.Ugc.Item> GetUninstalledAnswerItemListWithTag(string Tag)
    {
        Debug.Log(Tag);
        if (Tag == "r-18+")
        {
            List<Steamworks.Ugc.Item> uninstalledAnswerItemList = new List<Steamworks.Ugc.Item>();
            foreach (Steamworks.Ugc.Item item in UninstalledAnswerItemList)
            {
                string[] itemTags = item.Tags;
                List<string> TagList = new List<string>();
                foreach (string tag in itemTags)
                {
                    TagList.Add(tag);
                }
                if (TagList.Contains(Tag))
                {
                    uninstalledAnswerItemList.Add(item);
                }
            }
            return uninstalledAnswerItemList;
        }
        else
        {
            List<Steamworks.Ugc.Item> uninstalledAnswerItemList = new List<Steamworks.Ugc.Item>();
            foreach (Steamworks.Ugc.Item item in UninstalledAnswerItemList)
            {
                string[] itemTags = item.Tags;
                List<string> TagList = new List<string>();
                foreach (string tag in itemTags)
                {
                    Debug.Log(tag);
                    TagList.Add(tag);
                }
                if (TagList.Contains(Tag) && !TagList.Contains("r-18+"))
                {
                    uninstalledAnswerItemList.Add(item);
                }
            }
            return uninstalledAnswerItemList;
        }
        
    }
    public string GetCurrentTag()
    {
        int tagIndex = TagDropDown.value;
        string tag = TagManager.instance.TagOptions_English[tagIndex];
        return tag;
    }
    public void FilterWithTag()
    {
        int tagIndex = TagDropDown.value;
        string tag = TagManager.instance.TagOptions_English[tagIndex];
        UIManager.instance.ShowAnswerItemsWithTag(tag);
    }
    void addNewItemEntry(Steamworks.Ugc.Item item)
    {
        GameObject itemEntry = Instantiate(WorkshopListEntry, WorkshopListContent.transform);
        itemEntry.GetComponent<WorkshopListEntry>().Initialize(item);


    }
    public bool ThisUserIsOwner(Steamworks.Ugc.Item item)
    {
        Steamworks.SteamId ItemOwner = item.Owner.Id;
        Steamworks.SteamId playerId = Steamworks.SteamClient.SteamId;
        return Steamworks.SteamId.Equals(ItemOwner, playerId);
    }

    #endregion
    // Update is called once per frame
    void Update()
    {
        

    }
}

public class ProgressClass :IProgress<float>
{
    float lastvalue = 0;
    public Text ErrorDisplay;
    public Text UploadProgressText;
    public Slider UploadProgressSlider;
    public ProgressClass()
    {
        ErrorDisplay = AnswerWorkshopManager.instance.ErrorDisplay;
        UploadProgressText = AnswerWorkshopManager.instance.UploadProgressText;
        UploadProgressSlider = AnswerWorkshopManager.instance.UploadProgressSlider;
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
        UploadProgressText.text = lastvalue * 100 + "%";
        UploadProgressSlider.value = lastvalue;
        if (lastvalue == 1)
        {
            AnswerWorkshopManager.instance.ConfirmSubmitPanel.SetActive(false);
            AnswerWorkshopManager.instance.UploadProgressPanel.SetActive(false);
            AnswerWorkshopManager.instance.UploadSuccessPanel.SetActive(true);
            UploadProgressText.text = 0 + "%";
            UploadProgressSlider.value = 0;
        }
        //Console.WriteLine(value);
    }
}


