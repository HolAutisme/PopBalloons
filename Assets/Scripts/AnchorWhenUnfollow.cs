using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowGazePoint))]

public class AnchorWhenUnfollow : MonoBehaviour {

    [SerializeField]
    private string _anchorName;

    private FollowGazePoint _follow;
    private bool _oldEnable;

    private bool _anchorExist = false;

    private void Awake()
    {
        _follow = GetComponent<FollowGazePoint>();
    }

    // Use this for initialization
    void Start () {

        if (WorldAnchorManager.Instance == null)
        {
            Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
            Destroy(this);
        }
        _oldEnable = !_follow.followEnable;
        if(_anchorName == "")
        {
            _anchorName = gameObject.name;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(WorldAnchorManager.Instance.AnchorStore == null)
        {
            return;
        }

		if(_follow.followEnable != _oldEnable)
        {
            if (_follow.followEnable && _anchorExist)
            {
                Debug.Log("Remove");
                WorldAnchorManager.Instance.RemoveAnchor(gameObject);
            }
            else if(!_follow.followEnable)
            {
                Debug.Log("Attach");
                WorldAnchorManager.Instance.AttachAnchor(gameObject, _anchorName);
                _anchorExist = true;
            }
            _oldEnable = _follow.followEnable;
        }
	}
}
