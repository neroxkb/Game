using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public static GuideManager instance;

    public int CurrentPageIndex;
    public GameObject GuideCanvas;
    public GameObject[] PageList = new GameObject[4];
    public GameObject[] SelectPointList = new GameObject[4];
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SelectPointList[0].GetComponent<SelectPointManager>().TurnPointOn();
        SetPageActive(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowGuideCancas()
    {
        GuideCanvas.SetActive(true);
    }
    public void CloseGuideCancas()
    {
        GuideCanvas.SetActive(false);
    }
    public void SetPageActive(int index)
    {
        for (int i = 0; i < PageList.Length; i++)
        {
            if (i == index)
            {
                PageList[i].SetActive(true);
            }
            else
            {
                PageList[i].SetActive(false);
            }
        }
    }
    public void OnClick_NextPage()
    {
        SelectPointList[CurrentPageIndex].GetComponent<SelectPointManager>().TurnPointOff();
        if (CurrentPageIndex == PageList.Length - 1)
        {
            SetPageActive(0);
            CurrentPageIndex = 0;
            SelectPointList[CurrentPageIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        else
        {
            SetPageActive(CurrentPageIndex + 1);
            CurrentPageIndex += 1;
            SelectPointList[CurrentPageIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        
    }
    public void OnClick_PrevGame()
    {
        SelectPointList[CurrentPageIndex].GetComponent<SelectPointManager>().TurnPointOff();
        if (CurrentPageIndex == 0)
        {
            SetPageActive(PageList.Length - 1);
            CurrentPageIndex = PageList.Length - 1;
            SelectPointList[CurrentPageIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
        else
        {
            SetPageActive(CurrentPageIndex - 1);
            CurrentPageIndex -= 1;
            SelectPointList[CurrentPageIndex].GetComponent<SelectPointManager>().TurnPointOn();
        }
       
    }
}
