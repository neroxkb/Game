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

public class UploadDiffLanguage : MonoBehaviour
{
    // Start is called before the first frame update
    public string path = "H:\\UnityProject\\WorkshopLanguages\\english";
    public string currentLang = "english";
    public const string QUESTION_TAG = "default";
    public const string WORD_PAIR_TAG = "WordPairItem";
    public string defaultPreviewPath = "H:\\UnityProject\\PSAssert\\UI\\CharacterStand.png";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick_ParseFromDictory(int StartIndex)
    {
        DirectoryInfo direction = new DirectoryInfo(path);
        int type = -1;

        DirectoryInfo[] folders = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);
        for (int i = StartIndex; i < folders.Length; i++)
        {
            int index = i + 1;
            //Debug.Log(folders[i].FullName);
            FileInfo[] files = folders[i].GetFiles("*", SearchOption.TopDirectoryOnly);
            for (int j = 0; j < files.Length; j++)
            {
                if (files[j].Name == "Question.json")
                {
                    type = 1;
                    QuestionData questionData = AnswerFileManager.instance.readJsonFromFile(files[j].FullName);
                    //Debug.Log(files[j].DirectoryName);
                    //Debug.Log(files[j].FullName);
                    Debug.Log("uploading " + index + "/" + folders.Length);
                    var t = CreateAnswerItem(questionData.tag, questionData.lang, questionData.title,questionData.description, folders[i].FullName, index);

                }
                else if (files[j].Name == "WordPair.json")
                {
                    Debug.Log("uploading " + index + "/" + folders.Length);
                    WordPairData wordPairData = UnderCoverWorkshopManager.instance.readJsonFromFile(files[j].FullName);
                    var t = CreateWordItem(wordPairData.wordA, wordPairData.wordB, currentLang, folders[i].FullName, index);
                }
            }
        }



    }
    public string GetPreviewPath(string contentPath)
    {
        string previewPath1 = contentPath + "\\Preview.png";
        string previewPath2 = contentPath + "\\Preview.PNG";
        string previewPath3 = contentPath + "\\Preview.jpg";
        string previewPath4 = contentPath + "\\Preview.JPG";
        if (File.Exists(previewPath1))
        {
            Debug.Log("preview1");
            return previewPath1;
        }
        if (File.Exists(previewPath2))
        {
            Debug.Log("preview2");
            return previewPath2;
        }
        if (File.Exists(previewPath3))
        {
            Debug.Log("preview3");
            return previewPath3;
        }
        if (File.Exists(previewPath4))
        {
            Debug.Log("preview4");
            return previewPath4;
        }
        return defaultPreviewPath;

    }
    private async Task CreateAnswerItem(string tag,string lang,string title,string description, string contentPath,int index)
    {

        string previewPath = GetPreviewPath(contentPath);
        if (tag == QUESTION_TAG)
        {
            var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle(title)
                    .WithDescription(description)
                    .WithPublicVisibility()
                    .WithPreviewFile(previewPath)
                    .WithContent(contentPath)
                    .WithTag(QUESTION_TAG)
                    .WithTag(lang)
                    .SubmitAsync();
            if (result.Success)
            {
                Debug.LogWarning(index + " success: " + result.Success + " " + result.Result.ToString());
            }
            else
            {
                Debug.LogError(index + " success: " + result.Success + " " + result.Result.ToString());
            }
            
            
        }
        else
        {
            var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle(title)
                    .WithDescription(description)
                    .WithPublicVisibility()
                    .WithPreviewFile(previewPath)
                    .WithContent(contentPath)
                    .WithTag(QUESTION_TAG)
                    .WithTag(lang)
                    .WithTag(tag)
                    .SubmitAsync();
            if (result.Success)
            {
                Debug.LogWarning(index + " success: " + result.Success + " " + result.Result.ToString());
            }
            else
            {
                Debug.LogError(index + " success: " + result.Success + " "+result.Result.ToString());
            }

        }


    }
    private async Task CreateWordItem(string wordA,string wordB,string lang, string contentPath, int index)
    {
        var result = await Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle("WordPair")
                    .WithDescription(wordA + " --- " + wordB)
                    .WithPublicVisibility()
                    .WithTag(WORD_PAIR_TAG)
                    .WithTag(lang)
                    .WithContent(contentPath)
                    .SubmitAsync();
        if (result.Success)
        {
            Debug.LogWarning(index + " success: " + result.Success + " " + result.Result.ToString());
        }
        else
        {
            Debug.LogError(index + " success: " + result.Success + " " + result.Result.ToString());
        }

    }

}
