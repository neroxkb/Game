using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMessageEntry : MonoBehaviour
{
    // Start is called before the first frame update
    public Text PlayerName;
    public Text PlayerMessageText;
    public void Initialize(string name, string message)
    {
        PlayerName.text = name;
        PlayerMessageText.text = ""+message;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
