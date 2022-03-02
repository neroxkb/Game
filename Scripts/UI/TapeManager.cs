using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject TapeOn;
    public GameObject TapeOff;
    public GameObject Tv;
    public GameObject PlayBox;

    public bool TapeConfirming;
    public bool TapeRemoving;
    public Vector3 OriginPosition;
    public Vector3 TargetPosition;
    public float Speed;

    
    void Start()
    {
        TapeConfirming = false;
        TapeRemoving = false;
        PlayBox = GameObject.Find("PlayBox").gameObject;
        if (PlayBox == null)
        {
            Debug.LogError("cannot find playbox");
        }

        Tv = GameObject.Find("TV").gameObject;
        if (Tv == null)
        {
            Debug.LogError("cannot find Tv");
        }

        OriginPosition = new Vector3(124.5f, 38, 0);
        TargetPosition = new Vector3(124.5f, -28.5f, 0);
        Speed = 0.1f;
        this.transform.localPosition = OriginPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (TapeConfirming)
        {
            TapeMove_Forward();
        }
        else if (TapeRemoving)
        {
            TapeMove_Back();
        }
    }
    public void TapeMove_Forward()
    {
        Speed = Speed + 100 *Time.deltaTime;
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, TargetPosition, Speed);
        if (this.transform.position == TargetPosition)
        {
            this.TapeConfirming = false;
            TurnTapeOn();
            TurnPlayBoxOn();
            TurnTVOn();
            UIManager.instance.ShowGameCanvas();
            Speed = 0.1f;
        }
    }
    public void TapeMove_Back()
    {
        Speed = Speed + 100 * Time.deltaTime;
        this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, OriginPosition, Speed);
        if (this.transform.position == OriginPosition)
        {
            this.TapeRemoving = false;
            Speed = 0.1f;
        }
    }
    public void TurnTapeOn()
    {
        TapeOn.SetActive(true);
        TapeOff.SetActive(false);
    }
    public void TurnTapeOff()
    {
        TapeOn.SetActive(false);
        TapeOff.SetActive(true);
    }
    public void TurnPlayBoxOn()
    {
        PlayBox.GetComponent<PlayBoxManager>().TurnPlayBoxOn();
    }
    public void TurnPlayBoxOff()
    {
        PlayBox.GetComponent<PlayBoxManager>().TurnPlayBoxOff();
    }

    public void TurnTVOn()
    {
        Tv.GetComponent<TVManager>().TurnTVOn();
    }
    public void TurnTVOff()
    {
        Tv.GetComponent<TVManager>().TurnTVOff();
    }

    public void TapeConfirm()
    {
        this.TapeConfirming = true;
    }

    public void TapeRemove()
    {
        TurnTapeOff();
        TurnPlayBoxOff();
        TurnTVOff();
        this.TapeRemoving = true;
    }
}
