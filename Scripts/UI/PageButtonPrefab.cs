using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageButtonPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    public Text PageNumText;
    public int PageNum;
    public Button PageButton;

    public void Initialize(int PageNum,bool Selected)
    {
        this.PageNum = PageNum;
        PageNumText.text = ""+PageNum;
        if (Selected)
        {
            SetSelected();
        }
        else
        {
            SetUnselected();
        }
    }
    public void OnClick_Button()
    {
        if (UIManager.instance.AnswerWorkshopCanvas.gameObject.activeSelf)
        {
            UIManager.instance.JumpPage(PageNum);
        }
        else if (UIManager.instance.UnderCoverWorkshopCanvas.gameObject.activeSelf)
        {
            UIManager.instance.JumpPage_UnderCover(PageNum);
        }
        else
        {
            LeaderBoardUIManagger.instance.JumpToPage(PageNum);
        }
        
    }
    public void SetSelected()
    {
        PageButton.interactable = false;
    }
    public void SetUnselected()
    {
        PageButton.interactable = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
