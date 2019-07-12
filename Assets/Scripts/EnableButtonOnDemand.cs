using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableButtonOnDemand : MonoBehaviour {

    //public List<GameObject> childrenList;

	// Use this for initialization
	void Start ()
    {
        /*
        int children = this.transform.childCount;
        for (int i = 0; i < children; ++i)
            childrenList.Add(this.transform.GetChild(i).gameObject);
        */
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Active(bool condition)
    {
        this.gameObject.SetActive(condition);
    }
}
