using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundSound : MonoBehaviour {

    private AudioSource source;
    [SerializeField]
    private AudioClip musicEnfant;
    [SerializeField]
    private AudioClip musicEspace;
    [SerializeField]
    private AudioClip musicDino;


    // Use this for initialization
    void Start () {
        source = this.GetComponent<AudioSource>();

        switch(ThemeManager.getCurrentTheme())
        {
            case ThemeManager.ThemeType.CHILDROOM:
                source.clip = musicEnfant;
                source.volume = SoundMixManager.getVolume(SoundMixManager.SoundType.BACKGROUNDCHILD);
                break;
            case ThemeManager.ThemeType.SPACE:
                source.clip = musicEspace;
                source.volume = SoundMixManager.getVolume(SoundMixManager.SoundType.BACKGROUNDSPACE);
                break;

            case ThemeManager.ThemeType.DINOSAUR:
                source.clip = musicDino;
                source.volume = SoundMixManager.getVolume(SoundMixManager.SoundType.BACKGROUNDDINO);
                break;
        }
        source.loop = true;
        
        source.Play();
    }

}
