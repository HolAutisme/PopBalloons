using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour {
    [Header("Paramètres :")]
    [SerializeField]
    private UnityEngine.UI.Text hintText;
    [SerializeField]
    private AudioClip hintMP3;



    public UnityEngine.UI.Text GetText()
    {
        return this.hintText;
    }

    public AudioClip GetAudioClip()
    {
        return this.hintMP3;
    }
}
