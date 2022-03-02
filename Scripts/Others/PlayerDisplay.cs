using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplay : MonoBehaviour
{
    public Text PlayerName;

    public void setPlayerName(string name)
    {
        PlayerName.text = name;
    }
    public void setPlayerColor(Color color)
    {
        PlayerName.color= color;
    }
}
