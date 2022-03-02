using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Generator : MonoBehaviourPunCallbacks
{
    public GameObject room;
    [Range(1,10)]
    public int scale;
    public KeyCode reloadKey;
    public Queue<Vector3> queue;
    public List<Vector3> roomsPosition;
    public Vector3[] directionOffset;
    [Range(1, 1000)]
    public int num;
    public float generateDelay;

    GameObject overHeadCamera;
    GameObject mainCamera;
    //GameObject player;
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;//玩家
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<Vector3>();
        roomsPosition = new List<Vector3>();
        room.transform.localScale = Vector3.one * scale;

        overHeadCamera = GameObject.Find("OverHeadCamera");
        mainCamera = GameObject.Find("MainCamera");
        //player = GameObject.FindWithTag("Player");


        directionOffset = new Vector3[4];
        directionOffset[0]=new Vector3(0, 0, 2 * scale);
        directionOffset[1] = new Vector3(0, 0, -2 * scale);
        directionOffset[2] = new Vector3(2 * scale, 0, 0);
        directionOffset[3] = new Vector3(-2 * scale, 0, 0);
        //StartCoroutine(GenerateRoom());

        GenerateRoom();
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer . ");
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0.5f, 0f), Quaternion.identity, 0);//生成一个玩家
        }




    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    //IEnumerator GenerateRoom() {
    void GenerateRoom()
    {
        //overHeadCamera.SetActive(true);
        //mainCamera.SetActive(false);
        //player.SetActive(false);
        float startTime = Time.time;
        GameObject startRoom = Instantiate(room, transform.position, Quaternion.identity, transform) as GameObject;
        startRoom.name = "StartRoom";

        //Vector3 offset = new Vector3(3, 0, 0);


        Vector3 curRoomPosition = startRoom.transform.position;
        //queue.Enqueue(curRoomPosition);
        while (num>0) {
            //yield return new WaitForSeconds(generateDelay);
            //WaitForSeconds(generateDelay);
            //curRoomPosition=queue.Dequeue();
            bool generateSuccess=false;
            for (int i = 0; i < 4; i++)
            {
                if (toGenerate())
                {
                    Vector3 offset = directionOffset[i];
                    Vector3 nextPosition = curRoomPosition + offset;
                    if (!roomsPosition.Contains(nextPosition))
                    {
                        Debug.Log(num + ":" + nextPosition);
                        GameObject nextRoom = Instantiate(room, nextPosition, Quaternion.identity, transform) as GameObject;
                        curRoomPosition = nextPosition;
                        //.Enqueue(nextPosition);
                        num--;
                        generateSuccess = true;
                        roomsPosition.Add(nextPosition);
                    }
                }
            }
            if (!generateSuccess)
            {
                int positionIndex = Random.Range(0, roomsPosition.Count);
                Debug.Log("positionIndex:"+positionIndex);
                curRoomPosition = roomsPosition[positionIndex];
            }
        }
        float endTime = Time.time;
        Debug.Log("generate end.Time:"+(endTime-startTime));
        //overHeadCamera.SetActive(false);
        //mainCamera.SetActive(true);
        //player.SetActive(true);

    }
    bool toGenerate()
    {
        int direction = Random.Range(0, 5);
        if (direction > 2)
        {
            return true;
        }
        return false;
    }
    Vector3 getOffset() {
        Vector3 offset;
        int direction = Random.Range(0, 4);
        Debug.Log("direction:"+direction);
        if (direction == 0)
        {//up
            offset = new Vector3(0, 0, 2 * scale);
        }
        else if (direction ==1)
        {//down
            offset = new Vector3(0, 0, -2 * scale);
        }
        else if (direction == 2)
        {//left
            offset = new Vector3(-2 * scale, 0, 0);
        }
        else 
        {//right
            offset = new Vector3(2 * scale, 0, 0);
        }
        return offset;
    }
}
