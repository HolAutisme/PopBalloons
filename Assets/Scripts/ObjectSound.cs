using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectSound : MonoBehaviour
{

    private AudioSource source;
    [SerializeField]
    private AudioClip sound;
    [SerializeField]
    private SoundMixManager.SoundType type;
    [SerializeField]
    private bool loop = false;
    [SerializeField]
    private bool toTrigger = false;
    [SerializeField]
    private bool onEnable = false;

    // Use this for initialization
    void Start()
    {
        if (!toTrigger)
            PlaySound();
    }

    public void TriggerSound()
    {
        PlaySound();
    }


    public void SetSound(AudioClip clip)
    {
        sound = clip;
    }

    public void OnEnable()
    {
        if (onEnable)
        {
            PlaySound();
            onEnable = false;
        }
    }

    private void PlaySound()
    {
        source = this.GetComponent<AudioSource>();
        if (loop)
        {
            source.loop = true;
            source.clip = sound;
            source.volume = SoundMixManager.getVolume(type);
            source.Play();
        }
        else
        {
            source.PlayOneShot(sound, SoundMixManager.getVolume(type));
        }
    }
}

