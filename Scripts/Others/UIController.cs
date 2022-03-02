using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject canvas;
    bool isPause;
    public KeyCode pauseKey=KeyCode.Escape;
    void Start()
    {
        isPause = false;
        canvas.SetActive(isPause);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            isPause = !isPause;
            canvas.SetActive(isPause);
            if (isPause)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                //Time.timeScale = 0;
            }
            else
            {
                Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
                //Time.timeScale = 1;
            }
        }
    }
}
