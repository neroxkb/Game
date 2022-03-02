using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static TVManager instance;
    public GameObject TVOn;
    public GameObject TVOff;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void Start()
    {
        
    }

    public void TurnTVOn()
    {
        TVOn.SetActive(true);
        TVOff.SetActive(false);
    }
    public void TurnTVOff()
    {
        TVOn.SetActive(false);
        TVOff.SetActive(true);
    }
    public void OnClick_TVSwitch()
    {
        if (TVOn.activeSelf)
        {
            TVOn.SetActive(false);
            TVOff.SetActive(true);
        }
        else if(TVOff.activeSelf)
        {
            TVOn.SetActive(true);
            TVOff.SetActive(false);
        }
    }

   

}
