using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanguageData
{
    public LanguageItem[] LanguageItems;        // store a localizationItem list.
}

[System.Serializable]
public class LanguageItem               // store a item
{
    public string key;                      // similar to json's key
    public string value;                    // similar to json's value
}
