using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour {

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private string sliderType;

    [SerializeField]
    private GameObject fill;

    public void Start()
    {
        switch (sliderType)
        {
            case "Music":
                slider.value = SoundMixManager.instance.backgroundVolume;
                return;
            case "Sound effects" :
                slider.value = SoundMixManager.instance.fxVolume;
                return;
            case "Voices":
                slider.value = SoundMixManager.instance.voiceVolume;
                return;
        }
    }


    public void Increment()
    {

        fill.SetActive(true);
        if (slider.value <= 0.8f)
        {
            slider.value += 0.2f;
        }
        else
        {
            slider.value = 1f;
        }
        switch (sliderType)
        {
            case "Music":
                SoundMixManager.instance.backgroundVolume = slider.value;
                return;
            case "Sound effects":
                SoundMixManager.instance.fxVolume = slider.value;
                return;
            case "Voices":
                SoundMixManager.instance.voiceVolume = slider.value;
                return;
        }
    }

    public void Decrement()
    {
        if (slider.value >= 0.2f)
        {
            slider.value -= 0.2f;

        }
        else
        {
            slider.value = 0f;
            fill.SetActive(false);
        }
        switch (sliderType)
        {
            case "Music":
                SoundMixManager.instance.backgroundVolume = slider.value;
                return;
            case "Sound effects":
                SoundMixManager.instance.fxVolume = slider.value;
                return;
            case "Voices":
                SoundMixManager.instance.voiceVolume = slider.value;
                return;
        }
    }


}
