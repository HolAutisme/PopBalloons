using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour {
    public ItemBlock blockPrefab;
	// Use this for initialization
	void Start () {
        Display();
	}
 public void Display()
    {foreach(ItemEntry intem in XMLManager.ins.itemDB.list)
        {
            
        ItemBlock newBlock = Instantiate(blockPrefab) as ItemBlock;
        newBlock.transform.SetParent(transform, false);
            //newBlock.Display(item);

        }
    }	
}
