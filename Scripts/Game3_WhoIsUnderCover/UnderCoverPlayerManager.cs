using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderCoverPlayerManager : MonoBehaviour
{
    public static UnderCoverPlayerManager instance;
    public int PlayerState;
    public bool IsDead;
    public bool IsUnderCover;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        IsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
