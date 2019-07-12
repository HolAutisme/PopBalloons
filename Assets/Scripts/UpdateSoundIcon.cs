using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSoundIcon : MonoBehaviour {

    [SerializeField]
    private float min;

    [SerializeField]
    private float max;
    
    private UnityEngine.UI.Image img;

    // Use this for initialization
    void Start () {
        img = this.gameObject.GetComponent<UnityEngine.UI.Image>();
        HandleChange(SoundMixManager.getChildVolume());
        SoundMixManager.onChildVolumeChanged += HandleChange;
	}

    private void HandleChange(float f)
    {
       img.enabled = (f > min && f <= max);
    }

    private void OnDestroy()
    {
        SoundMixManager.onChildVolumeChanged -= HandleChange;
    }
}
