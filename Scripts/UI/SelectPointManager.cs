using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPointManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PointOn;
    public GameObject PointOff;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnPointOn()
    {
        PointOn.SetActive(true);
        PointOff.SetActive(false);
    }
    public void TurnPointOff()
    {
        PointOn.SetActive(false);
        PointOff.SetActive(true);
    }
}
