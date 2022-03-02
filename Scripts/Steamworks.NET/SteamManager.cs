using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    public static SteamManager instance;
    private void OnEnable()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        try
        {
       
            SteamClient.Init(1758390, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }
    private void OnDisable()
    {
        Steamworks.SteamClient.Shutdown();
    }
    
}
