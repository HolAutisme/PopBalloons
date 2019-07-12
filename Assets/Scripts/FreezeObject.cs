using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeObject : MonoBehaviour {

    private Vector3 initPosition;
    private Quaternion initRotation;
    [SerializeField]
    private bool freezeRotation;

    // Use this for initialization
    void Start () {
        this.initPosition = this.GetComponent<Transform>().position;
        this.initRotation = this.GetComponent<Transform>().rotation;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (freezeRotation)
        {
            this.transform.position = initPosition;
            this.transform.rotation = initRotation;
        }
        else
        {
            this.transform.position = initPosition;
            this.transform.LookAt(Camera.main.transform.position + 2*(initPosition - Camera.main.transform.position), Vector3.up);
        }
    }
}
