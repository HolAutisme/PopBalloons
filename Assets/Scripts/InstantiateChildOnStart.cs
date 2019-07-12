using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateChildOnStart : MonoBehaviour {

    [Header("Settings")]
    [SerializeField]
    List<GameObject> prefabs;


    [Header("Scaling")]
    [SerializeField]
    bool needRescale = false;

    [SerializeField]
    float sizeFactor = 1 ;


    [SerializeField]
    bool disableOnStart = false;

    // Use this for initialization
    void Start()
    {
        if(needRescale)
        {
            this.transform.localScale *= sizeFactor;
        }
        
        foreach (GameObject g in prefabs)
        {
          var obj = Instantiate(g, this.transform);
          if (disableOnStart)
             obj.SetActive(false);
        }
    }
}
