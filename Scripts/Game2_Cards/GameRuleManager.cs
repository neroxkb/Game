using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleManager : MonoBehaviour
{
    public GameObject GameRuleCanvas;
    // Start is called before the first frame update
    public void OnClick_Show()
    {
        GameRuleCanvas.SetActive(true);
    }
    public void OnClick_Close()
    {
        GameRuleCanvas.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
