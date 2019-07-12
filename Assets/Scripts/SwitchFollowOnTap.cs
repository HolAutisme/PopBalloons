using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

[RequireComponent(typeof(FollowGazePoint))]

public class SwitchFollowOnTap : MonoBehaviour /*, IFocusable*/, IInputClickHandler {

    private FollowGazePoint _follow;
    public bool canEnable;
    public bool canDisable;
    public bool switchCollider = true;

    private Collider _collider;

    private void Awake()
    {
        _follow = GetComponent<FollowGazePoint>();
        _collider = GetComponent<Collider>();
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("Click");
        if(_follow.followEnable && canDisable)
        {
            _follow.followEnable = false;
            if(_collider != null && switchCollider)
            {
                _collider.enabled = false;
            }
        }
        else if (canEnable)
        {
            _follow.followEnable = true;
            if (_collider != null && switchCollider)
            {
                _collider.enabled = true;
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //void IFocusable.OnFocusEnter()
    //{
    //    //throw new NotImplementedException();
    //}

    //void IFocusable.OnFocusExit()
    //{
    //    //throw new NotImplementedException();
    //}
}
