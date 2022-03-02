using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;             

    private Dictionary<string, string> languageText;       
    private bool isReady = false;                           
    private string missingTextString = "Localized text not found."; 
    public List<string> languageFileName;
    public int defaultIndex = 1;

    public delegate void ReSetLanguageDelegate();
    public ReSetLanguageDelegate reSetLanguageEvent;

    private void OnEnable()
    {
        /*GameObject settingsManager = GameObject.Find("SettingsManager");
        if (settingsManager != null)
        {
            SettingsManager sm = settingsManager.GetComponent<SettingsManager>();
            sm.setLanguageEvent += setLanguage;
        }*/
        //SettingsManager.instance.setLanguageEvent += setLanguage;
        
        reSetLanguageEvent = callReSetLanguageEvent;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);                     // when load another scene, not destroy this manager instance.
        /*if (languageFileName == null)
        {
            Debug.LogError("languageFileName is null");
            return;
        }
        
        string filename = languageFileName[defaultIndex];
        LoadLocalizedText(filename);
        
        isReady = true;*/
    }
    private void Start()
    {
                     
    }
    public void callReSetLanguageEvent()
    {

    }
    public void setLanguage(int langIndex)
    {
        Debug.Log("setting lang:"+languageFileName[langIndex]);
        LoadLocalizedText(languageFileName[langIndex]);
        
    }
    public void LoadLocalizedText(string fileName)          
    {
        Debug.Log(fileName);
        languageText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);     

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);    
            LanguageData loadedData = JsonUtility.FromJson<LanguageData>(dataAsJson);  

            for (int i = 0; i < loadedData.LanguageItems.Length; i++)
            {
                languageText.Add(loadedData.LanguageItems[i].key, loadedData.LanguageItems[i].value);   
            }

            Debug.Log("Data loaded, dictionary contains: " + languageText.Count + " entries.");    
        }
        else
        {
            Debug.LogError("Cannot find file!");  // show error.
        }
        reSetLanguageEvent();

    }

    public string GetLocalizedValue(string key)     // return value according the key.
    {
        string result = missingTextString;
        if (languageText.ContainsKey(key))
        {
            result = languageText[key];
        }

        return result;
    }

    public bool GetIsReady()        // return the status of json data loaded protcss.
    {
        return isReady;
    }
}
