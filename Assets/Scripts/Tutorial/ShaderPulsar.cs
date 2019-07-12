using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPulsar : MonoBehaviour {

    [SerializeField]
    Material material;

    [SerializeField]
    [Range(0, 5f)]
    float offset;

    [SerializeField]
    [Range(0, 2f)]
    float propagationTime;


    [SerializeField]
    [Range(0, 4f)]
    float pulseFrequency = 2f;

    private float time = 0;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;

        if(time > pulseFrequency)
        {
            material.SetVector("_Center", Camera.main.transform.position);
            time = 0;
        }

        var pulseOffset = time / propagationTime * offset;
        material.SetFloat("_TransitionOffset", pulseOffset);
	}
}
