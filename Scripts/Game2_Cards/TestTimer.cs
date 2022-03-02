using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
public class TestTimer : MonoBehaviourPunCallbacks
{
    /// <summary>
    ///     OnCountdownTimerHasExpired delegate.
    /// </summary>
    public delegate void CountdownTimerEnd();

    public const string CountdownStartTime = "StartTime";
    public const string RoomRound = "RoomRound";

    [Header("Countdown time in seconds")]
    public float Countdown = 5.0f;

    private bool isTimerRunning;

    private int startTime;
    private int round;

    [Header("Reference to a Text component for visualizing the countdown")]
    public Text Text;

    public bool countOver = false;
    /// <summary>
    ///     Called when the timer has expired.
    /// </summary>
    public static event CountdownTimerEnd OnCountdownTimerEnd;


    public void Start()
    {
        if (this.Text == null) Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
    }

    public override void OnEnable()
    {
        Debug.Log("OnEnable TestTimer");
        base.OnEnable();

        // the starttime may already be in the props. look it up.
        Initialize();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Debug.Log("OnDisable TestTimer");
    }


    public void Update()
    {

        if (!this.isTimerRunning) return;

        float countdown = TimeRemaining();
        //this.Text.text = string.Format("Game starts in {0} seconds", countdown.ToString("n0"));
        this.Text.text = string.Format("Round  {0}", round.ToString("n0"));
        if (countdown > 0.0f) return;

        OnTimerEnds();
        
        
    }


    private void OnTimerRuns()
    {
        this.isTimerRunning = true;
        this.enabled = true;
    }

    private void OnTimerEnds()
    {
        this.isTimerRunning = false;
        this.enabled = false;

        Debug.Log("Emptying info text.", this.Text);
        this.Text.text = string.Empty;

        if (OnCountdownTimerEnd != null) OnCountdownTimerEnd();
        countOver = true;
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("TestTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
        Initialize();
    }


    private void Initialize()
    {
        int propStartTime;
        int roundOut;
        if (TryGetStartTime(out propStartTime,out roundOut))
        {
            this.startTime = propStartTime;
            this.round = roundOut;
            Debug.Log("Initialize sets StartTime " + this.startTime + " server time now: " + PhotonNetwork.ServerTimestamp + " remain: " + TimeRemaining());


            this.isTimerRunning = TimeRemaining() > 0;

            if (this.isTimerRunning)
                OnTimerRuns();
            else
                OnTimerEnds();
        }
    }


    private float TimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.startTime;
        return this.Countdown - timer / 1000f;
    }


    public static bool TryGetStartTime(out int startTimestamp, out int roundOut)
    {
        startTimestamp = PhotonNetwork.ServerTimestamp;
        roundOut = 1;
        object startTimeFromProps;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
        {
            startTimestamp = (int)startTimeFromProps;
            return true;
        }
        object roundFromProps;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomRound, out roundFromProps))
        {
            roundOut = (int)roundFromProps;
        }

        return false;
    }


    public static void SetStartTime()
    {
        int startTime = 0;
        int roundOut;
        bool wasSet = TryGetStartTime(out startTime,out roundOut);

        Hashtable props = new Hashtable
            {
                {TestTimer.CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
            };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);


        Debug.Log("Set Custom Props for Time: " + props.ToStringFull() + " wasSet: " + wasSet);
    }
}


