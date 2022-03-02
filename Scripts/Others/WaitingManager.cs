using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static WaitingManager instance;

    public GameObject WaitingPanel;
    public GameObject WaitingObject;
    public GameObject WaitingObject1;
    public GameObject WaitingObject2;
    public bool Waiting;
    public float speed = 2f;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        
    }
    public void ShowWaitingCanvas(int index)
    {
        WaitingPanel.SetActive(true);
        if (index == 1)
        {
            WaitingObject1.SetActive(true);
            WaitingObject2.SetActive(false);
            WaitingObject = WaitingObject1;
            
        }
        else if (index == 2)
        {
            WaitingObject1.SetActive(false);
            WaitingObject2.SetActive(true);
            WaitingObject = WaitingObject2;
            
        }

        Waiting = true;
    }
    public void CloseWaitingCanvas()
    {
        WaitingPanel.SetActive(false);
        WaitingObject.SetActive(false);
        WaitingObject1.SetActive(false);
        WaitingObject2.SetActive(false);
        Waiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Waiting)
        {
            WaitingObject.transform.Rotate(Vector3.forward * speed);
        }
    }
}
