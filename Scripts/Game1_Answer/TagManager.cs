using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;
public class TagManager : MonoBehaviour
{
    public static TagManager instance;
    public int CurrentGameTagValue;
    public int StoreGameTape=0;
    public Dropdown TagDropDown;
    public Dropdown UploadTagDropDown;
    public Dropdown GameTagDropDown;
    public List<string> TagOptions_Chinese;
    public List<string> TagOptions_English;
    public List<string> TagOptions_Russian;
    public List<string> TagOptions_French;
    public List<string> TagOptions_German;
    public List<string> TagOptions_Korean;
    public List<string> TagOptions_Japanese;
    public List<string> TagOptions_Spanish;
    public List<string> TagOptions_Portuguese;
    public List<string> TagOptions_Arabic;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;
    }
   
    void Start()
    {
        /*for (int i = 0; i < TagOptions_Arabic.Count; i++)
        {
            print(TagOptions_Arabic[i]);
            TagOptions_Arabic[i] = ArabicFixer.Fix(TagOptions_Arabic[i], false, false);
            print(TagOptions_Arabic[i]);
        }*/
        /*TagOptions_Arabic = new List<string>();
        TagOptions_Arabic.Add(ArabicFixer.Fix("افتراضي ", false, false));
        TagOptions_Arabic.Add(ArabicFixer.Fix("حيوان .", false, false));
        TagOptions_Arabic.Add(ArabicFixer.Fix("نباتات ", false, false));
        TagOptions_Arabic.Add(ArabicFixer.Fix("جغرافيا ", false, false));
        TagOptions_Arabic.Add(ArabicFixer.Fix("انترنت .", false, false));*/
    }
    public void SetDropDown()
    {
        Debug.Log("set tag");
        if (!TagDropDown)
        {
            return;
        }
        TagDropDown.ClearOptions();
        UploadTagDropDown.ClearOptions();
        GameTagDropDown.ClearOptions();
        if (SettingsManager.instance.GetCurrentLang() == "chinese")
        {
            Debug.Log("set tag chinese");
            TagDropDown.AddOptions(TagOptions_Chinese);
            UploadTagDropDown.AddOptions(TagOptions_Chinese);
            GameTagDropDown.AddOptions(TagOptions_Chinese);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "english")
        {
            Debug.Log("set tag english");
            TagDropDown.AddOptions(TagOptions_English);
            UploadTagDropDown.AddOptions(TagOptions_English);
            GameTagDropDown.AddOptions(TagOptions_English);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "russian")
        {
            Debug.Log("set tag russian");
            TagDropDown.AddOptions(TagOptions_Russian);
            UploadTagDropDown.AddOptions(TagOptions_Russian);
            GameTagDropDown.AddOptions(TagOptions_Russian);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "french")
        {
            Debug.Log("set tag french");
            TagDropDown.AddOptions(TagOptions_French);
            UploadTagDropDown.AddOptions(TagOptions_French);
            GameTagDropDown.AddOptions(TagOptions_French);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "german")
        {
            Debug.Log("set tag german");
            TagDropDown.AddOptions(TagOptions_German);
            UploadTagDropDown.AddOptions(TagOptions_German);
            GameTagDropDown.AddOptions(TagOptions_German);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "korean")
        {
            Debug.Log("set tag korean");
            TagDropDown.AddOptions(TagOptions_Korean);
            UploadTagDropDown.AddOptions(TagOptions_Korean);
            GameTagDropDown.AddOptions(TagOptions_Korean);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "japanese")
        {
            Debug.Log("set tag japanese");
            TagDropDown.AddOptions(TagOptions_Japanese);
            UploadTagDropDown.AddOptions(TagOptions_Japanese);
            GameTagDropDown.AddOptions(TagOptions_Japanese);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "spanish")
        {
            Debug.Log("set tag spanish");
            TagDropDown.AddOptions(TagOptions_Spanish);
            UploadTagDropDown.AddOptions(TagOptions_Spanish);
            GameTagDropDown.AddOptions(TagOptions_Spanish);
        }
        else if (SettingsManager.instance.GetCurrentLang() == "portuguese")
        {
            Debug.Log("set tag portuguese");
            TagDropDown.AddOptions(TagOptions_Portuguese);
            UploadTagDropDown.AddOptions(TagOptions_Portuguese);
            GameTagDropDown.AddOptions(TagOptions_Portuguese);
        }
        /*else if (SettingsManager.instance.GetCurrentLang() == "arabic")
        {
            Debug.Log("set tag arabic");
            for (int i = 0; i < TagOptions_Arabic.Count; i++)
            {
                print(TagOptions_Arabic[i]);
                TagOptions_Arabic[i]= ArabicFixer.Fix(TagOptions_Arabic[i], false, false);
                print(TagOptions_Arabic[i]);
            }
            TagDropDown.AddOptions(TagOptions_Arabic);
            UploadTagDropDown.AddOptions(TagOptions_Arabic);
            GameTagDropDown.AddOptions(TagOptions_Arabic);
        }*/
    }
    public void SetCurrentGameTag()
    {
        CurrentGameTagValue = GameTagDropDown.value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
