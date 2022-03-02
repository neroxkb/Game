using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public const int PLAYER_WAIT_FOR_BEGIN = 13;
    public const int PLAYER_WAIT_FOR_FIRST_CARD = 1;
    public const int PLAYER_WAIT_FOR_SECOND_CARD = 2;
    //public const int PLAYER_WAIT_FOR_ROUND_TWO = 3;
    public const int PLAYER_WAIT_FOR_ROUND_THREE = 4;
    public const int PLAYER_WAIT_FOR_ROUND_FOUR = 5;
    public const int PLAYER_FOLD = 6;
    public const int PLAYER_ADD_BET = 7;
    public const int PLAYER_SOHA = 8;
    public const int PLAYER_RAISE_BET = 9;
    public const int PLAYER_CALL = 10;
    public const int PLAYER_WAIT_FOR_MAKING_CHOICE = 11;
    public const int PLAYER_WAIT_FOR_MAKING_CHOICE_FOLD_OR_SOHA = 12;
    public const int PLAYER_WINNER = 14;
    public const int PLAYER_MAKING_CHOICE = 15;
    public const int PLAYER_MAKING_CHOICE_FOLD_OR_SOHA = 16;

    public const int ROOM_WAIT_FOR_CARD = 101;

    //花色，黑桃，红桃，梅花，方块
    public const int CARD_TYPE_ZILCH = 201;//散牌，顺序A,K,Q,J,10,9,8,7,6,5,4,3,2
    public const int CARD_TYPE_ONE_PAIR = 202;//一对
    public const int CARD_TYPE_TWO_PAIRS = 203;//两对，大的对子为大
    public const int CARD_TYPE_THREE_OF_A_KIND = 204;//三张点数一样的，先比三个，再比单
    public const int CARD_TYPE_STRAIGHT = 205;//顺子，五张连续的
    public const int CARD_TYPE_FLUSH = 206;//同花
    public const int CARD_TYPE_FULLHOUSE = 207;//三张点数一样的，加一个对子，先比三个，再比对子
    public const int CARD_TYPE_FOUR_OF_A_KIND = 208;//四张牌一样
    public const int CARD_TYPE_STRAIGHT_FLUSH = 209;//同花顺
    public const int CARD_TYPE_ROYAL_FLUSH = 210;//最高点为A的同花顺

    public static GameInfo instance;
    private void Awake()
    {
        instance = this;
    }
    public string getRoomStateString(int state)
    {
        if (state == PLAYER_WAIT_FOR_FIRST_CARD)
        {
            return "It's time to deliver card";
        }
        return "";
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
