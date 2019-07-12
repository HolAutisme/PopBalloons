using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBaseScale : MonoBehaviour {

    [SerializeField]
    [Range(0.1f, 3f)]
    float baseDistance;

    Vector3 initialScale;
    Camera player;

	// Use this for initialization
	void Start ()
    {
        player = Camera.main;
        initialScale = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.localScale = initialScale * GetCurrentScaleFactor();
	}


    private float GetCurrentScaleFactor()
    {
        return (Vector3.Distance(this.transform.position, player.transform.position) / baseDistance);
    }
}
