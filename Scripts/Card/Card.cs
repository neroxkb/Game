using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string suit;//»¨É«
    public string number;
    public int value = 0;
    public int weight = 0;

    void Awake()
    {
        this.transform.localScale= new Vector3(15,15, 1);
    }
     
    
}
