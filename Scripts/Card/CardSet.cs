using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
public class CardSet : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public List<Card> cardSet;
    public Vector3 scale = new Vector3(10, 10, 1);
    public Text infoText;
    public Text nextCardText;
    private PhotonView photonView;
    public List<Card> allCards;
    public GameObject cardBack;
    private int seed;
    public int currentCardIndex = 0;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    public List<Card> DeepCopy(List<Card> source)
    {
        List<Card> copy = new List<Card>();
        for (int i = 0; i < source.Count; i++)
        {
            copy.Add(source[i]);
        }
        return copy;
    }
    public Card getCard(int index)
    {
        Card ret = cardSet[index];
        ret.transform.localScale = scale;
        return ret;
    }
    public GameObject getCardBack()
    {
        GameObject ret = cardBack;
        ret.transform.localScale = scale;
        return ret;
    }
    public void setInfoText(Text info)
    {
        infoText = info;
    }

    public List<Card> getRandomCarSet(int randomSeed)
    {
        List<Card> cardSetCopy = DeepCopy(cardSet);
        System.Random ran = new System.Random(randomSeed);
        Card temp;
        int index = 0;
        List<Card> ret = new List<Card>();
        for (int i = 0; i < cardSetCopy.Count; i++)
        {
            index = ran.Next(0, cardSet.Count - 1);
            if (index != i)
            {
                temp = cardSetCopy[index];
                cardSetCopy[index] = cardSetCopy[i];
                cardSetCopy[i] = temp;
            }
        }

        return cardSetCopy;
    }
    public List<Card> getRandomCardSet_neq(List<Card> inputCardSet, int randomSeed)
    {
        List<Card> cardSetCopy = DeepCopy(inputCardSet);
        System.Random ran = new System.Random(randomSeed);
        Card temp;
        int index = 0;
        List<Card> ret = new List<Card>();
        for (int i = 0; i < cardSetCopy.Count; i++)
        {
            index = ran.Next(0, cardSetCopy.Count - 1);
            if (index != i)
            {
                temp = cardSetCopy[index];
                cardSetCopy[index] = cardSetCopy[i];
                cardSetCopy[i] = temp;
            }
        }
        
        return cardSetCopy;
    }
    public List<Card> getRandomCardSet_eq(List<Card> inputCardSet, int randomSeed)
    {
        List<Card> cardSetCopy = DeepCopy(inputCardSet);
        System.Random ran = new System.Random(randomSeed);
        Card temp;
        int index = 0;
        List<Card> ret = new List<Card>();
        for (int i = cardSetCopy.Count - 1; i > 0; --i)
        {
            index = ran.Next(0, i+1);
            if (index != i)
            {
                temp = cardSetCopy[index];
                cardSetCopy[index] = cardSetCopy[i];
                cardSetCopy[i] = temp;
            }
            
        }
        
        return cardSetCopy;
    }
    [PunRPC]
    void initSoHaCards(int seed)
    {
        allCards = new List<Card>();
        List<Card> cardSetCopy = DeepCopy(cardSet);
        for (int i = 24; i < 52; i++)
        {
            allCards.Add(cardSetCopy[i]);
            Debug.Log("add card:" + cardSetCopy[i].name);
        }
        Debug.Log("init Card Stack, card number:" + allCards.Count);

        allCards = getRandomCarSet(allCards, seed);
        nextCardText.text = "next card " + currentCardIndex + ":" + allCards[currentCardIndex].name;
    }
    [PunRPC]
    void initAllCards(int seed)
    {
        allCards = new List<Card>();
        for (int i = 0; i < 2; i++)
        {
            List<Card> randomCardSet = getRandomCarSet(seed);
            allCards.AddRange(randomCardSet);
        }
        Debug.Log("init Card Stack, card number:" + allCards.Count);

        allCards = getRandomCarSet(allCards, seed);
        nextCardText.text = "next card " + currentCardIndex + ":" + allCards[currentCardIndex].name;
    }
    [PunRPC]
    void deliverNextCard(int playerNumber,int state,bool canBeSeen)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
            int playerCount = playerList.Length;
            for (int i = 0; i < playerCount; i++)
            {
                GamePlayer gp = playerList[i];
                //infoText.text = "playerNumber" + playerNumber;
                Debug.Log("playerNumber:" + playerNumber);
                Debug.Log("gp.photonView.Owner.ActorNumber:" + gp.photonView.Owner.ActorNumber);
                if (gp.State == state && gp.photonView.Owner.ActorNumber == playerNumber)
                {
                    Debug.Log("deliver  card to Player " + playerNumber);
                    infoText.text = "deliver  card to Player " + playerNumber;
                    gp.photonView.RPC("addNewCard", RpcTarget.All, currentCardIndex, canBeSeen);
                    photonView.RPC("cardIndexAdd", RpcTarget.All);
                    break;
                }
            }
        }
    }
    [PunRPC]
    void deliverACardToAll(int playerNumber, bool canBeSeen)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
            int playerCount = playerList.Length;
            for (int i = 0; i < playerCount; i++)
            {
                GamePlayer gp = playerList[i];
                //infoText.text = "playerNumber" + playerNumber;
                Debug.Log("playerNumber:" + playerNumber);
                Debug.Log("gp.photonView.Owner.ActorNumber:" + gp.photonView.Owner.ActorNumber);
                if (gp.State !=GameInfo.PLAYER_FOLD && gp.photonView.Owner.ActorNumber == playerNumber)
                {
                    Debug.Log("deliver  card to Player " + playerNumber);
                    infoText.text = "deliver  card to Player " + playerNumber;
                    gp.photonView.RPC("addNewCard", RpcTarget.All, currentCardIndex, canBeSeen);
                    photonView.RPC("cardIndexAdd", RpcTarget.All);
                }
            }
        }
    }
    /*[PunRPC]
    void nextPlayerMakeChoice(int nextPlayerId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GamePlayer[] playerList = GameObject.FindObjectsOfType<GamePlayer>();
            int playerCount = playerList.Length;
            for (int i = 0; i < playerCount; i++)
            {
                GamePlayer gp = playerList[i];
                if (gp.State == GameInfo.PLAYER_WAIT_FOR_MAKING_CHOICE && gp.photonView.Owner.ActorNumber == nextPlayerId)
                {
                    Debug.Log("Player " + nextPlayerId + " make choice");
                    infoText.text = "Player " + nextPlayerId+" make choice";
                    gp.photonView.RPC("makeChoiceStart", RpcTarget.All);
                }
            }
        }
    }*/
    [PunRPC]
    void cardIndexAdd()
    {
        currentCardIndex++;
        nextCardText.text = "next card " + currentCardIndex + ":" + allCards[currentCardIndex].name;
    }
    public int getCurrentIndex()
    {
        return currentCardIndex;
    }
}
