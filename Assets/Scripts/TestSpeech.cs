using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

[RequireComponent(typeof(TextToSpeech))]
public class TestSpeech : MonoBehaviour {

    private TextToSpeech t;
    [SerializeField]
    bool go = false;
    [SerializeField]
    string quote;
    
	// Use this for initialization
	void Start () {
        t = this.GetComponent<TextToSpeech>();
       
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (go)
        {
            go = false;
            t.StartSpeaking(string.Format(quote));
            Debug.Log(quote);
            
        }	
	}
}
