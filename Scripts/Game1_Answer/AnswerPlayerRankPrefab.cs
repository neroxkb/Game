using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPlayerRankPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage Avatar;
    public Text Name;
    public Text AnswerNumber;
    public Text BonusNum;

    public void Initialize(RawImage avatar,string name,int answerCorrectNumber)
    {
        Avatar.texture = avatar.texture;
        Name.text = name;
        AnswerNumber.text = answerCorrectNumber + "/" + AnswerGameManager.instance.QUESTION_NUMBER;
        int bonusNum = answerCorrectNumber * 1000;
        BonusNum.text = "" + bonusNum;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
