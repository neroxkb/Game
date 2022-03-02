using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BGMManager : MonoBehaviour
{
    public AudioSource BGMAudio;
    public string BGMAudioPath = Application.streamingAssetsPath + "/Audio/BGM.wav";
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        StartCoroutine(SetBGMAudio());
        //BGMAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator SetBGMAudio()
    {

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(BGMAudioPath, AudioType.WAV);
        yield return request.SendWebRequest();
        // AssetBundle ab = DownloadHandlerAssetBundle.GetContent (request );

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            DownloadHandlerAudioClip.GetContent(request);
            BGMAudio.clip = DownloadHandlerAudioClip.GetContent(request);
            Debug.Log("set BGM");
            BGMAudio.Play();
        }

    }
}
