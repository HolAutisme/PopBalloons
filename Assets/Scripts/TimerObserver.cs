using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerObserver : MonoBehaviour {

    [SerializeField]
    private UnityEngine.UI.Text timerText;

    public bool TimeOverride
    { get; set; }

	// Use this for initialization
	void Start () {
		if(timerText == null)
        {
            timerText = this.GetComponent<UnityEngine.UI.Text>();
        }
        TimeOverride = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(timerText != null && !TimeOverride)
        {
            setTime(TimerManager.getTimeStamp());
        }
	}

    public void setTime(float time)
    {
        timerText.text = time.ToString("0.00") + "s";
    }
}
