using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerGameTag : MonoBehaviour
{
    // Start is called before the first frame update
    public static AnswerGameTag instance;
    public string QuestionTag;
    public Dropdown GameTagDropDown;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }
    public void SetQuestionTag()
    {
        int tagIndex = GameTagDropDown.value;
        Dropdown.OptionData optionData = GameTagDropDown.options[tagIndex];
        QuestionTag = optionData.text;
    }
    void Start()
    {
        QuestionTag = "default";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
