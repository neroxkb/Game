using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;
using UnityEngine.UI;
public class AnswerFileManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    public static AnswerFileManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        //readJson(Application.streamingAssetsPath+"/Faucet-88028");
    }
    public void readCsv()
    {

    }
    public QuestionData readJson(string assertPath)
    {
        string questionDataPath = assertPath + "/Question.json";
        if (File.Exists(questionDataPath))
        {
            print(questionDataPath);
        }
        string textData = "";
        try
        {
            textData = File.ReadAllText(questionDataPath);
        }
        catch (System.Exception e)
        {
            AnswerGameManager.instance.LeaveRoom();
        }
        print(questionDataPath);
        print(textData);
        QuestionData questionData = JsonUtility.FromJson<QuestionData>(textData);
        return questionData;
        
    }
    public QuestionData readJsonFromFile(string filePath)
    {
        string questionDataPath = filePath;
        if (File.Exists(questionDataPath))
        {
            print(questionDataPath);
        }
        string textData = File.ReadAllText(questionDataPath);
        print(textData);
        QuestionData questionData = JsonUtility.FromJson<QuestionData>(textData);
        return questionData;

    }
    public void writeJson(QuestionData questionData,string path)
    {
        string text=JsonUtility.ToJson(questionData);
        File.Create(path + "/Question.json").Dispose();
        File.WriteAllText(path + "/Question.json", text);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
