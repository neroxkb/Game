using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnderCoverItemPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    public Steamworks.Ugc.Item item;
    public Text WordA;
    public Text WordB;
    public void Initialize(Steamworks.Ugc.Item item)
    {
        this.item = item;
        WordPairData wordPairData = UnderCoverWorkshopManager.instance.readJson(item.Directory);
        WordA.text = wordPairData.wordA;
        WordB.text = wordPairData.wordB;
        Debug.Log("add word:" + wordPairData.wordA + "," + wordPairData.wordB);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
