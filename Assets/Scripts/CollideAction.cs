using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideAction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Balloon")
            collision.gameObject.GetComponent<Renderer>().material.color = Color.blue;
    }
}
