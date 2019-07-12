using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePanel : MonoBehaviour {

    [SerializeField]
    private FollowGazePoint _follow;

    [SerializeField]
    private GameObject[] _panels;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < _panels.Length; i++)
        {
            _panels[i].SetActive(!_follow.followEnable);
        }
        
	}
}
