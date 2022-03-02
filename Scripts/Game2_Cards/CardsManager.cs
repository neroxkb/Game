using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static CardsManager instance;
    List<List<string>> allStraightNumber=new List<List<string>>();
    
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
        List<string> cardsNumber = new List<string>() { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        for (int i = 0; i < cardsNumber.Count - 5; i++)
        {
            List<string> temp = new List<string>();
            string debug = "";
            for (int j = 0; j < 5; j++)
            {
                temp.Add(cardsNumber[i + j]);
                debug += cardsNumber[i + j]+"  ";
            }
            Debug.Log("add straight:" + debug);
            allStraightNumber.Add(temp);
        }
        List<string> t = new List<string>() { "A","2", "3", "4", "5" };
        string s1 = "";
        for (int j = 0; j < 5; j++)
        {
            s1 += t[ j] + "  ";
        }
        Debug.Log("add straight:" + s1);
        allStraightNumber.Add(t);
        List<string> r = new List<string>() { "10", "J", "Q", "K", "A" };
        string s2 = "";
        for (int j = 0; j < 5; j++)
        {
            s2 += r[j] + "  ";
        }
        Debug.Log("add straight:" + s2);
        allStraightNumber.Add(r);

    }
    public int GetCardsType(List<Card> cards)
    {
        if (checkRoyalFlush(cards))
        {
            return GameInfo.CARD_TYPE_ROYAL_FLUSH;
        }
        else if (checkStraightFlush(cards))
        {
            return GameInfo.CARD_TYPE_STRAIGHT_FLUSH;
        }
        else if (checkFourOfAKind(cards))
        {
            return GameInfo.CARD_TYPE_FOUR_OF_A_KIND;
        }
        else if (checkFullHouse(cards))
        {
            return GameInfo.CARD_TYPE_FULLHOUSE;
        }
        else if (checkFlush(cards))
        {
            return GameInfo.CARD_TYPE_FLUSH;
        }
        else if (checkStraight(cards))
        {
            return GameInfo.CARD_TYPE_STRAIGHT;
        }
        else if (checkThreeOfAKind(cards))
        {
            return GameInfo.CARD_TYPE_THREE_OF_A_KIND;
        }
        else if (checkTwoPairs(cards))
        {
            return GameInfo.CARD_TYPE_TWO_PAIRS;
        }
        else if (checkOnePair(cards))
        {
            return GameInfo.CARD_TYPE_ONE_PAIR;
        }
        else 
        {
            return GameInfo.CARD_TYPE_ZILCH;
        }
       
    }
    public int compareCards(List<Card> a, List<Card> b)
    {
        if (a.Count != b.Count)
        {
            Debug.LogError("compare list length should be same. a is " + a.Count + ",b is " + b.Count);
            return -2;
        }
        int aType = GetCardsType(a);
        int bType = GetCardsType(b);
        if (aType > bType)
        {
            return 1;
        }
        else if (aType < bType)
        {
            return -1;
        }
        else//aType=bType
        {
            Debug.Log(cardsToString(a) + " -vs- " + cardsToString(b));
            if (aType == GameInfo.CARD_TYPE_STRAIGHT_FLUSH)
            {
                List<int> aWeights = GetCardsWeight(a);
                List<int> bWeights = GetCardsWeight(b);
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
            }
            else if (aType == GameInfo.CARD_TYPE_FOUR_OF_A_KIND)
            {
                List<int> aWeights = GetFourWeight(a);
                List<int> bWeights = GetFourWeight(b);
                if (aWeights.Count != 4 || bWeights.Count != 4)
                {
                    Debug.LogError("CARD_TYPE_FOUR_OF_A_KIND list length should be 4. a is " + a.Count + ",b is " + b.Count);
                }
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
                
            }
            else if (aType == GameInfo.CARD_TYPE_FULLHOUSE)
            {
                List<int> aWeights = GetThreeWeight(a);
                List<int> bWeights = GetThreeWeight(b);
                if (aWeights.Count != 3 || bWeights.Count != 3)
                {
                    Debug.LogError("CARD_TYPE_FULLHOUSE list length should be 3. a is " + a.Count + ",b is " + b.Count);
                }
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
            }
            else if (aType == GameInfo.CARD_TYPE_FLUSH)
            {
                //先比数字，再比花色
                List<int> aValue = GetCardsValue(a);
                List<int> bValue = GetCardsValue(b);
                aValue.Sort();
                bValue.Sort();
                if (aValue.Count != 5 || bValue.Count != 5)
                {
                    Debug.LogError("CARD_TYPE_FLUSH Value list length should be 5. a is " + a.Count + ",b is " + b.Count);
                }
                for (int i = 0; i < aValue.Count; i++)
                {
                    if (aValue[i] > bValue[i])
                    {
                        return 1;
                    }
                    else if (aValue[i] < bValue[i])
                    {
                        return -1;
                    }
                }
                List<int> aWeights = GetCardsWeight(a);
                List<int> bWeights = GetCardsWeight(b);
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
            }
            else if (aType == GameInfo.CARD_TYPE_STRAIGHT)
            {
                List<int> aWeights = GetCardsWeight(a);
                List<int> bWeights = GetCardsWeight(b);
                if (aWeights.Count != 5 || bWeights.Count != 5)
                {
                    Debug.LogError("CARD_TYPE_STRAIGHT list length should be 5. a is " + a.Count + ",b is " + b.Count);
                }
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
            }
            else if (aType == GameInfo.CARD_TYPE_THREE_OF_A_KIND)
            {
                List<int> aWeights = GetThreeWeight(a);
                List<int> bWeights = GetThreeWeight(b);
                if (aWeights.Count != 3 || bWeights.Count != 3)
                {
                    Debug.LogError("CARD_TYPE_THREE_OF_A_KIND list length should be 3. a is " + a.Count + ",b is " + b.Count);
                }
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
            }
            else if (aType == GameInfo.CARD_TYPE_TWO_PAIRS)
            {
                //先比大对子的数字，再比小对子的数字，再比花色
                List<int> aValues = GetPairsValue(a);
                List<int> bValues = GetPairsValue(b);
                List<int> aWeights = GetPairsWeight(a);
                List<int> bWeights = GetPairsWeight(b);
                if (aValues.Count != 2 || bValues.Count != 2)
                {
                    Debug.LogError("CARD_TYPE_TWO_PAIRS Values list length should be 2. a is " + a.Count + ",b is " + b.Count);
                }
                if (aWeights.Count != 4 || bWeights.Count != 4)
                {
                    Debug.LogError("CARD_TYPE_TWO_PAIRS Weights list length should be 4. a is " + a.Count + ",b is " + b.Count);
                }
                aValues.Sort();
                bValues.Sort();
                if (intCompare(aValues[0], bValues[0]) == 0)
                {
                    if (intCompare(aValues[1], bValues[1]) == 0)
                    {
                        int aMax = aWeights.Max();
                        int bMax = bWeights.Max();
                        return intCompare(aMax, bMax);
                    }
                    else
                    {
                        return intCompare(aValues[1], bValues[1]);
                    }
                }
                else
                {
                    return intCompare(aValues[0], bValues[0]);
                }
            }
            else if (aType == GameInfo.CARD_TYPE_ONE_PAIR)
            {
                //先比大对子的数字，再比小对子的数字，再比花色
                List<int> aValues = GetPairsValue(a);
                List<int> bValues = GetPairsValue(b);
                List<int> aWeights = GetPairsWeight(a);
                List<int> bWeights = GetPairsWeight(b);
                if (aValues.Count != 1 || bValues.Count != 1)
                {
                    Debug.LogError("CARD_TYPE_ONE_PAIR Values list length should be 1. a is " + a.Count + ",b is " + b.Count);
                }
                if (aWeights.Count != 2 || bWeights.Count != 2)
                {
                    Debug.LogError("CARD_TYPE_ONE_PAIR Weights list length should be 2. a is " + aWeights.Count + ",b is " + bWeights.Count);
                }
                aValues.Sort();
                bValues.Sort();
                if (intCompare(aValues[0], bValues[0]) == 0)
                {
                    int aMax = aWeights.Max();
                    int bMax = bWeights.Max();
                    return intCompare(aMax, bMax);
                }
                else
                {
                    return intCompare(aValues[0], bValues[0]);
                }
            }
            else if (aType == GameInfo.CARD_TYPE_ZILCH)
            {
                List<int> aWeights = GetCardsWeight(a);
                List<int> bWeights = GetCardsWeight(b);
                if (aWeights.Count <0 || bWeights.Count <0)
                {
                    Debug.LogError("CARD_TYPE_FOUR_OF_A_KIND list length should > 0. a is " + aWeights.Count + ",b is " + bWeights.Count);
                }
                int aMax = aWeights.Max();
                int bMax = bWeights.Max();
                return intCompare(aMax, bMax);
            }
        }
        Debug.LogError("compare failed ,return -3.");
        return -3;
    }
    public int intCompare(int a, int b)
    {
        if (a > b)
        {
            return 1;
        }
        else if (a < b)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    #region GetCardsInfo
    public List<int> GetFourWeight(List<Card> cards)
    {
        Hashtable valueFreqTable = new Hashtable();
        string fourSameNumber = "";
        List<int> weightList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (valueFreqTable.Contains(card.number))
            {
                valueFreqTable[card.number] = (int)valueFreqTable[card.number] + 1;
                if ((int)valueFreqTable[card.number] == 4)
                {
                    fourSameNumber = card.number;
                }
            }
            else
            {
                valueFreqTable.Add(card.number, 1);
            }
        }
        if (fourSameNumber != "")
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (fourSameNumber==card.number)
                {
                    weightList.Add(card.weight);
                }
            }
        }
        return weightList;
    }
    public List<int> GetThreeWeight(List<Card> cards)
    {
        Hashtable valueFreqTable = new Hashtable();
        string threeSameNumber = "";
        List<int> weightList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (valueFreqTable.Contains(card.number))
            {
                valueFreqTable[card.number] = (int)valueFreqTable[card.number] + 1;
                if ((int)valueFreqTable[card.number] == 3)
                {
                    threeSameNumber = card.number;
                }
            }
            else
            {
                valueFreqTable.Add(card.number, 1);
            }
        }
        if (threeSameNumber != "")
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (threeSameNumber == card.number)
                {
                    weightList.Add(card.weight);
                }
            }
        }
        return weightList;
    }
    public List<int> GetPairsValue(List<Card> cards)
    {
        Hashtable valueFreqTable = new Hashtable();
        List<int> valueList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (valueFreqTable.Contains(card.number))
            {
                valueFreqTable[card.number] = (int)valueFreqTable[card.number] + 1;
                if ((int)valueFreqTable[card.number] == 2)
                {
                    valueList.Add(card.value);
                }
            }
            else
            {
                valueFreqTable.Add(card.number, 1);
            }
        }
        return valueList;
    }
    public List<int> GetPairsWeight(List<Card> cards)
    {
        Hashtable valueFreqTable = new Hashtable();
        List<int> valueList = new List<int>();
        List<int> weightList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (valueFreqTable.Contains(card.number))
            {
                valueFreqTable[card.number] = (int)valueFreqTable[card.number] + 1;
                if ((int)valueFreqTable[card.number] == 2)
                {
                    valueList.Add(card.value);
                }
            }
            else
            {
                valueFreqTable.Add(card.number, 1);
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            for (int j = 0; j < valueList.Count; j++)
            {
                if (valueList[j] == card.value)
                {
                    weightList.Add(card.weight);
                }
            }
            
        }
        return weightList;
    }
    public List<int> GetValueFreqList(List<Card> cards)
    {
        Hashtable valueFreqTable = new Hashtable();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (valueFreqTable.Contains(card.value))
            {
                valueFreqTable[card.value] = (int)valueFreqTable[card.value] + 1;
            }
            else
            {
                valueFreqTable.Add(card.value, 1);
            }
        }
        //List<int> freqList = (List<int>)valueFreqTable.Values;
        List<int> freqList = new List<int>();
        foreach (var k in valueFreqTable.Values)
        {
            freqList.Add((int)k);
        }
        freqList.Sort();
        return freqList;
    }
    public List<int> GetSuitFreqList(List<Card> cards)
    {
        Hashtable suitFreqTable = new Hashtable();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (suitFreqTable.Contains(card.suit))
            {
                suitFreqTable[card.suit] = (int)suitFreqTable[card.suit] + 1;
            }
            else
            {
                suitFreqTable.Add(card.suit, 1);
            }
        }
        //List<int> freqList = (List<int>)suitFreqTable.Keys;
        List<int> freqList = new List<int>();
        foreach (var k in suitFreqTable.Values)
        {
            freqList.Add((int)k);
        }
        freqList.Sort();
        return freqList;
    }
    public List<string> GetCardsSuit(List<Card> cards)
    {
        List<string> suitList = new List<string>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            suitList.Add(card.suit);
        }

        return suitList;
    }
    public List<string> GetCardsNumber(List<Card> cards)
    {
        List<string> numberList = new List<string>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            numberList.Add(card.number);
        }

        return numberList;
    }
    public List<int> GetCardsValue(List<Card> cards)
    {
        List<int> valueList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            valueList.Add(card.value);
        }

        return valueList;
    }
    public List<int> GetCardsWeight(List<Card> cards)
    {
        List<int> weightList = new List<int>();
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            weightList.Add(card.weight);
        }

        return weightList;
    }
    #endregion
    #region CheckType
    bool checkOnePair(List<Card> cards)
    {
        List<int> freqList = GetValueFreqList(cards);
        List<int> OnePairFreqList = new List<int>() { 1, 1, 1, 2 };
        if (cards.Count == 2)
        {
            OnePairFreqList = new List<int>() { 2 };
        }
        else if (cards.Count == 3)
        {
            OnePairFreqList = new List<int>() { 1,2 };
        }
        else if (cards.Count == 4)
        {
            OnePairFreqList = new List<int>() { 1,1, 2 };
        }
        if (freqList.SequenceEqual(OnePairFreqList))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkTwoPairs(List<Card> cards)
    {
        List<int> freqList = GetValueFreqList(cards);
        List<int> TwoPairsFreqList = new List<int>() { 1, 2, 2 };

        if (cards.Count == 4)
        {
            TwoPairsFreqList = new List<int>() { 2, 2 };
        }

        if (freqList.SequenceEqual(TwoPairsFreqList))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkThreeOfAKind(List<Card> cards)
    {
        List<int> freqList = GetValueFreqList(cards);
        List<int> ThreeOfAKindFreqList = new List<int>() { 1, 1,3 };

        if (cards.Count == 3)
        {
            ThreeOfAKindFreqList = new List<int>() { 3};
        }
        else if(cards.Count == 4)
        {
            ThreeOfAKindFreqList = new List<int>() { 1,3 };
        }

        if (freqList.SequenceEqual(ThreeOfAKindFreqList))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkStraight(List<Card> cards)
    {
        List<string> cardsNumber = GetCardsNumber(cards);
        if (cardsNumber.Count != 5)
        {
            return false;
        }
        for (int i = 0; i < allStraightNumber.Count; i++)
        {
            List<string> sn = allStraightNumber[i];
            cardsNumber.Sort();
            sn.Sort();
            int equalCount = 0;
            //Debug.Log("checkStraight");
            for (int j = 0; j < cardsNumber.Count; j++)
            {
                //Debug.Log("cardsNumber:" + cardsNumber[j] + " sn:" + sn[j]);
                if (cardsNumber[j] == sn[j])
                {
                    equalCount += 1;
                }
                
            }
            if (equalCount == 5)
            {
                return true;
            }
        }
        return false;
    }
    bool checkFlush(List<Card> cards)
    {
        List<int> suitFreqList = GetSuitFreqList(cards);
        if (suitFreqList[0] == 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkFullHouse(List<Card> cards)
    {
        List<int> freqList = GetValueFreqList(cards);
        List<int> FullHouseFreqList = new List<int>() { 2, 3 };
        if (freqList.SequenceEqual(FullHouseFreqList))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkFourOfAKind(List<Card> cards)
    {
        List<int> freqList = GetValueFreqList(cards);
        List<int> FourOfAKindFreqList = new List<int>() { 1,4};
        if (cards.Count == 4)
        {
            FourOfAKindFreqList = new List<int>() { 4 };
        }

        if (freqList.SequenceEqual(FourOfAKindFreqList))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkStraightFlush(List<Card> cards)
    {
        if (checkStraight(cards) && checkFlush(cards))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool checkRoyalFlush(List<Card> cards)
    {
        if (!checkFlush(cards))
        {
            return false;
        }
        List<string> cardsNumber = GetCardsNumber(cards);
        List<string> sn = allStraightNumber[allStraightNumber.Count-1];
        cardsNumber.Sort();
        sn.Sort();
        
        if (cardsNumber.Count != 5)
        {
            return false;
        }
        int equalCount = 0;
        //Debug.Log("checkStraight");
        for (int j = 0; j < cardsNumber.Count; j++)
        {
            //Debug.Log("cardsNumber:" + cardsNumber[j] + " sn:" + sn[j]);
            if (cardsNumber[j] == sn[j])
            {
                equalCount += 1;
            }

        }
        if (equalCount == 5)
        {
            return true;
        }
        
        return false;
    }
    string cardsToString(List<Card> a)
    {
        string ret = "";
        for (int i = 0; i < a.Count; i++)
        {
            ret += a[i].number + " ";
        }
        return ret;
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        
    }
}
