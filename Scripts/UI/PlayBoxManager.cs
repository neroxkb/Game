using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBoxManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PlayBoxOn;
    public GameObject PlayBoxOn_Front;
    public GameObject PlayBoxOff;
    public GameObject PlayBoxOff_Front;

    public Vector3 OriginPosition;
    void Start()
    {
        OriginPosition = new Vector3(122f, -55, 0);
        this.transform.localPosition = OriginPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TurnPlayBoxOn()
    {
        PlayBoxOn.SetActive(true);
        PlayBoxOn_Front.SetActive(true);
        PlayBoxOff.SetActive(false);
        PlayBoxOff_Front.SetActive(false);
    }
    public void TurnPlayBoxOff()
    {
        PlayBoxOn.SetActive(false);
        PlayBoxOn_Front.SetActive(false);
        PlayBoxOff.SetActive(true);
        PlayBoxOff_Front.SetActive(true);
    }
}
