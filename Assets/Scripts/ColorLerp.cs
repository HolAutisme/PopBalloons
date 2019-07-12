using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLerp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator lerpColor(Color init, Color target)
    {
        float t = 0;


        //while
        while(t <= 5f)
        {

            t += Time.deltaTime;
            yield return null;

        }

        yield return null;
    }
}
