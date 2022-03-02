using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;
public class LanguageText : MonoBehaviour
{
    public string key;          // the key of UI.
    LanguageManager lm;

    public void OnEnable()
    {
        GameObject languageManager = GameObject.Find("LanguageManager");
        if (languageManager != null)
        {
            lm = languageManager.GetComponent<LanguageManager>();
            lm.reSetLanguageEvent += reSetLanguageText;
        }
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = LanguageManager.instance.GetLocalizedValue(key);
            /*if (SettingsManager.instance.GetCurrentLang() == "arabic")
            {
                text.text = ArabicFixer.Fix(text.text, false, false);
            }*/
            
        }
        
        TextMeshProUGUI textPro = GetComponent<TextMeshProUGUI>();
        if (textPro != null)
        {
            textPro.text = LanguageManager.instance.GetLocalizedValue(key);
            /*if (SettingsManager.instance.GetCurrentLang() == "arabic")
            {
                textPro.text = ArabicFixer.Fix(textPro.text, false, false);
            }*/
        }
        
    }
    private void OnDisable()
    {
        if (lm != null)
        {
            lm.reSetLanguageEvent -= reSetLanguageText;
        }
        
    }
    void Start()
    {
        
    }

    public void reSetLanguageText()
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = LanguageManager.instance.GetLocalizedValue(key);
        }

        TextMeshProUGUI textPro = GetComponent<TextMeshProUGUI>();
        if (textPro != null)
        {
            textPro.text = LanguageManager.instance.GetLocalizedValue(key);
        }
    }
}
