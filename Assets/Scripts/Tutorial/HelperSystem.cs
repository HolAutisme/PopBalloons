using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperSystem : MonoBehaviour {

    public enum HelpRequired { INTRODUCTION, TOO_LONG, HAND_PLACEMENT}

    [SerializeField]
    private TextMesh displayer;

    [SerializeField]
    private TextMesh introduction;

    [SerializeField]
    private TextMesh tooLongToPop;

    [SerializeField]
    private TextMesh struggling;

    [SerializeField]
    private AudioClip introductionClip;

    [SerializeField]
    private AudioClip tooLongToPopClip;

    [SerializeField]
    private AudioClip strugglingClip;

    [SerializeField]
    [Range(1f,10f)]
    private float helperDuration = 4f;


    private ObjectSound objectSound;
    private float currentTime;
    private bool helpIsDisplay = false;

    private void Awake()
    {
        objectSound = this.GetComponent<ObjectSound>();
    }

    private void Update()
    {
        if (helpIsDisplay)
        {
            if(currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                ClearDisplay();
            }
        }
    }

    private void ClearDisplay()
    {
        currentTime = 0;
        helpIsDisplay = false;
        displayer.text = "";
    }

    public void Display(HelpRequired help)
    {
        if (helpIsDisplay)
            return;

        switch (help)
        {
            case HelpRequired.INTRODUCTION:
                displayer.text = introduction.text;
                objectSound.SetSound(introductionClip); // explanation
                objectSound.TriggerSound();
                break;
            case HelpRequired.TOO_LONG:
                displayer.text = tooLongToPop.text;
                objectSound.SetSound(tooLongToPopClip); //Follow arrow
                objectSound.TriggerSound();
                break;
            case HelpRequired.HAND_PLACEMENT:
                displayer.text = struggling.text;
                objectSound.SetSound(strugglingClip); // How to pop
                objectSound.TriggerSound();
                break;
        }
        currentTime = helperDuration;
        helpIsDisplay = true;
    }

}
