using ArabicSupport;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    public AudioSource BGMAudio;
    // Start is called before the first frame update
    public const int Res_1920_1080 = 0;
    public const int Res_1280_720 = 1;
    public const int Res_640_360 = 2;

    public const int Lang_Cn = 0;
    public const int Lang_En = 1;
    public const int Lang_Ru = 2;
    public const int Lang_Fra = 3;
    public const int Lang_De = 4;
    public const int Lang_Kor = 5;
    public const int Lang_Jp = 6;
    public const int Lang_Spa = 7;
    public const int Lang_Pt = 8;
    //public const int Lang_Ara = 9;

    public List<string> LangList_LowCase;

    public int CurrentResolution;
    public int CurrentLanguage;
    public float CurrentVolume;
    public float CurrentBGMVolume;
    public GameObject SettingsPanel;
    public Toggle isFullScreen;
    public Toggle isMute;
    public Dropdown ResolutionDropDown;
    public Dropdown LanguageDropDown;
    public Slider VolumeSlider;
    public Slider BGMVolumeSlider;
    public VideoPlayer videoPlayer;

    public const string ResolutionSetting = "ResolutionSet";
    public const string FullScreenSetting = "FullScreenSet";
    public const string LanguageSetting = "LanguageSet";
    public const string VolumeSetting = "VolumeSet";
    public const string MuteSetting = "MuteSet";
    public const string BGMVolumeSetting = "BGMVolumeSet";

    public delegate void SetLanguageDelegate(int langIndex);
    public SetLanguageDelegate setLanguageEvent;
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

    
    void Start()
    {
        //LanguageDropDown.options[9].text= ArabicFixer.Fix(LanguageDropDown.options[9].text, false, false);
        BGMAudio.Play();
        LoadPlayerSet();
        //ResolutionDropDown.onValueChanged.AddListener(SetResolution);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public string GetCurrentLang()
    {
        if (CurrentLanguage == Lang_Cn)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "chinese";
        }
        if (CurrentLanguage == Lang_En)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "english";
        }
        if (CurrentLanguage == Lang_Ru)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "russian";
        }
        if (CurrentLanguage == Lang_Fra)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "french";
        }
        if (CurrentLanguage == Lang_De)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "german";
        }
        if (CurrentLanguage == Lang_Kor)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "korean";
        }
        if (CurrentLanguage == Lang_Jp)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "japanese";
        }
        if (CurrentLanguage == Lang_Spa)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "spanish";
        }
        if (CurrentLanguage == Lang_Pt)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "portuguese";
        }
        /*if (CurrentLanguage == Lang_Ara)
        {
            Debug.Log("current lang " + CurrentLanguage);
            return "arabic";
        }*/
        return "";
    }
    public void SetLanguageBySteamLang(string steamLang)
    {
        if (steamLang == "schinese")
        {
            SettingsManager.instance.SetLanguageAPI(0);
        }
        else if (steamLang == "english")
        {
            SettingsManager.instance.SetLanguageAPI(1);
        }
        else if (steamLang == "russian")
        {
            SettingsManager.instance.SetLanguageAPI(2);
        }
        else if (steamLang == "french")
        {
            SettingsManager.instance.SetLanguageAPI(3);
        }
        else if (steamLang == "german")
        {
            SettingsManager.instance.SetLanguageAPI(4);
        }
        else if (steamLang == "koreana")
        {
            SettingsManager.instance.SetLanguageAPI(5);
        }
        else if (steamLang == "japanese")
        {
            SettingsManager.instance.SetLanguageAPI(6);
        }
        else if (steamLang == "spanish")
        {
            SettingsManager.instance.SetLanguageAPI(7);
        }
        else if (steamLang == "latam")
        {
            SettingsManager.instance.SetLanguageAPI(7);
        }
        else if (steamLang == "portuguese")
        {
            SettingsManager.instance.SetLanguageAPI(8);
        }
        else if (steamLang == "brazilian")
        {
            SettingsManager.instance.SetLanguageAPI(8);
        }
    }
    public void SetLanguageAPI(int index)
    {
        LanguageDropDown.value = index;
        SetLanguage();
    }
    public void LoadPlayerSet()
    {
        if (PlayerPrefs.HasKey(ResolutionSetting))
        {
            
            CurrentResolution = PlayerPrefs.GetInt(ResolutionSetting);
            Debug.Log("LoadInt  ResolutionSetting " + CurrentResolution);
            ResolutionDropDown.value = CurrentResolution;
        }
        else
        {
            CurrentResolution = 1;
        }
        if (PlayerPrefs.HasKey(LanguageSetting))
        {

            CurrentLanguage = PlayerPrefs.GetInt(LanguageSetting);
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            
            if (AnswerGameManager.instance)
            {
                AnswerGameManager.instance.QuestionLang = GetCurrentLang();
            }
            Debug.Log("LoadInt  LanguageSetting " + CurrentLanguage);
            LanguageDropDown.value = CurrentLanguage;
        }
        else
        {
            CurrentLanguage = 0;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            if (AnswerGameManager.instance)
            {
                AnswerGameManager.instance.QuestionLang = GetCurrentLang();
            }
        }
        if (PlayerPrefs.HasKey(FullScreenSetting))
        {
            int isOn = PlayerPrefs.GetInt(FullScreenSetting);
            Debug.Log("LoadInt  FullScreenSetting " + isOn);
            if (isOn == 1)
            {
                isFullScreen.isOn = true;
            }
            else
            {
                isFullScreen.isOn = false;
            }
        }
        else
        {
            isFullScreen.isOn = false;
        }
        if (PlayerPrefs.HasKey(VolumeSetting))
        {

            CurrentVolume = PlayerPrefs.GetFloat(VolumeSetting);
            Debug.Log("LoadFloat  VolumeSetting " + CurrentVolume);
            VolumeSlider.value = CurrentVolume;
        }
        else
        {
            CurrentVolume = 0.5f;
            VolumeSlider.value = CurrentVolume;
            AudioListener.volume = CurrentVolume;
            if (videoPlayer)
            {
                videoPlayer.SetDirectAudioVolume(0, CurrentVolume);
            }
        }
        if (PlayerPrefs.HasKey(BGMVolumeSetting))
        {

            CurrentBGMVolume = PlayerPrefs.GetFloat(BGMVolumeSetting);
            Debug.Log("LoadFloat  BGMVolumeSetting " + CurrentBGMVolume);
            BGMVolumeSlider.value = CurrentBGMVolume;
            BGMAudio.volume = CurrentBGMVolume;
        }
        else
        {
            CurrentBGMVolume = 0.15f;
            BGMVolumeSlider.value = CurrentBGMVolume;
            BGMAudio.volume = CurrentBGMVolume;
            
        }
        if (PlayerPrefs.HasKey(MuteSetting))
        {
            int isOn = PlayerPrefs.GetInt(MuteSetting);
            Debug.Log("LoadInt  MuteSetting " + isOn);
            if (isOn == 1)
            {
                isMute.isOn = true;
                VolumeSlider.value = 0;
                AudioListener.volume = 0;
                if (videoPlayer)
                {
                    videoPlayer.SetDirectAudioVolume(0, 0);
                }
            }
            else
            {
                isMute.isOn = false;
            }
        }
        else
        {
            isMute.isOn = false;
        }
    }
    public void SetResolution()
    {
        int choice = ResolutionDropDown.value;
        if (choice == Res_1920_1080)
        {
            CurrentResolution = choice;
            Screen.SetResolution(1920, 1080, isFullScreen.isOn);
            PlayerPrefs.SetInt(ResolutionSetting, choice);
            Debug.Log("SetInt  ResolutionSetting  0");
        }
        else if (choice == Res_1280_720)
        {
            CurrentResolution = choice;
            Screen.SetResolution(1280, 720, isFullScreen.isOn);
            PlayerPrefs.SetInt(ResolutionSetting, choice);
            Debug.Log("SetInt  ResolutionSetting  1");
        }
        else if (choice == Res_640_360)
        {
            CurrentResolution = choice;
            Screen.SetResolution(640, 360, isFullScreen.isOn);
            PlayerPrefs.SetInt(ResolutionSetting, choice);
            Debug.Log("SetInt  ResolutionSetting  2");
        }
    }
    public void SetLanguage()
    {
        int choice = LanguageDropDown.value;
        if (choice == Lang_Cn)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  0");
        }
        else if (choice == Lang_En)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  1");
        }
        else if (choice == Lang_Ru)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  2");
        }
        else if (choice == Lang_Fra)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  3");
        }
        else if (choice == Lang_De)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  4");
        }
        else if (choice == Lang_Kor)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  5");
        }
        else if (choice == Lang_Jp)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  6");
        }
        else if (choice == Lang_Spa)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  7");
        }
        else if (choice == Lang_Pt)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  8");
        }
        /*else if (choice == Lang_Ara)
        {
            CurrentLanguage = choice;
            //setLanguageEvent(choice);
            LanguageManager.instance.setLanguage(CurrentLanguage);
            TagManager.instance.SetDropDown();
            PlayerPrefs.SetInt(LanguageSetting, choice);
            Debug.Log("SetInt  LanguageSetting  9");
        }*/
        if (AnswerWorkshopManager.instance)
        {
            if (UIManager.instance.AnswerWorkshopCanvas.gameObject.activeSelf)
            {
                UIManager.instance.OnClick_Refresh();
            }
            if (UIManager.instance.UnderCoverWorkshopCanvas.gameObject.activeSelf)
            {
                UIManager.instance.OnClick_Refresh_UnderCover();
            }
            
        }
    }
    public void SetFullScreen()
    {
        if (CurrentResolution == Res_1920_1080)
        {
            Screen.SetResolution(1920, 1080, isFullScreen.isOn);
        }
        else if (CurrentResolution == Res_1280_720)
        {
            Screen.SetResolution(1280, 720, isFullScreen.isOn);
        }
        else if (CurrentResolution == Res_640_360)
        {
            Screen.SetResolution(640, 360, isFullScreen.isOn);
        }
        if (isFullScreen.isOn)
        {
            Debug.Log("SetInt  FullScreenSetting  1");
            PlayerPrefs.SetInt(FullScreenSetting, 1);
        }
        else
        {
            Debug.Log("SetInt  FullScreenSetting  0");
            PlayerPrefs.SetInt(FullScreenSetting, 0);
        }
    }
    public void SetVolume()
    {
        CurrentVolume = VolumeSlider.value;
        AudioListener.volume = VolumeSlider.value;
        PlayerPrefs.SetFloat(VolumeSetting, CurrentVolume);
        Debug.Log("SetFloat  VolumeSetting " + CurrentVolume);
        if (videoPlayer)
        {
            videoPlayer.SetDirectAudioVolume(0,CurrentVolume);
        }
        if (CurrentVolume == 0)
        {
            isMute.isOn = true;
            Debug.Log("SetInt  MuteSetting  1");
            PlayerPrefs.SetInt(MuteSetting, 1);
        }
        else
        {
            isMute.isOn = false;
            Debug.Log("SetInt  MuteSetting  0");
            PlayerPrefs.SetInt(MuteSetting, 0);
        }
    }
    public void SetBGMVolume()
    {
        CurrentBGMVolume = BGMVolumeSlider.value;
        BGMAudio.volume = BGMVolumeSlider.value;
        PlayerPrefs.SetFloat(BGMVolumeSetting, CurrentBGMVolume);
        Debug.Log("SetFloat  BGMVolumeSetting " + CurrentBGMVolume);
    }
    public void SetMute()
    {
        if (isMute.isOn)
        {
            Debug.Log("SetInt  MuteSetting  1");
            PlayerPrefs.SetInt(MuteSetting, 1);
            VolumeSlider.value = 0;
            AudioListener.volume = 0;
            if (videoPlayer)
            {
                videoPlayer.SetDirectAudioVolume(0, 0);
            }
        }
        else
        {
            Debug.Log("SetInt  MuteSetting  0");
            PlayerPrefs.SetInt(MuteSetting, 0);
            VolumeSlider.value = CurrentVolume;
            AudioListener.volume = CurrentVolume;
            if (videoPlayer)
            {
                videoPlayer.SetDirectAudioVolume(0, CurrentVolume);
            }
        }
    }
    public void ShowSettings()
    {
        SettingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }

    
}
