using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {


    /// <summary>
    /// Sound manager to handle and manage every sounds that has to be played in game.
    /// </summary>
    [SerializeField]
    private AudioClip idle;
    [SerializeField]
    private AudioClip pop1;
    [SerializeField]
    private AudioClip pop2;
    [SerializeField]
    private AudioClip pop3;
    [SerializeField]
    private List<AudioClip> confettis;

    [SerializeField]
    private AudioClip blow;
    [SerializeField]
    private AudioClip confetti;
    private AudioSource audioSource;
    [SerializeField]
    private bool bonusBalloon = false;

    // Use this for initialization
    void OnEnable ()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPop()
    {
        AudioClip pop;
        int soundSelection = Random.Range(0,3); //Random.Range(min[inclusive], max[exclusive]);
        switch(soundSelection){
            case 0 :
            pop = pop1;
                break;
            case 1 :
            pop = pop2;
                break;
            default :
            pop = pop3;
                break;
        }
        audioSource.clip = pop;
        audioSource.Play();
    }

    public void PlayConfetti()
    {
        audioSource.PlayOneShot(confetti, SoundMixManager.getVolume(SoundMixManager.SoundType.POP_BONUS));
    }

    public void PlayPopAndConfetti()
    {
        AudioClip pop;
        int soundSelection = Random.Range(0, confettis.Count); //Random.Range(min[inclusive], max[exclusive]);
        pop = confettis[soundSelection];
        if (!bonusBalloon)
        { 
            audioSource.PlayOneShot(pop, SoundMixManager.getVolume(SoundMixManager.SoundType.POP_BALLOON));
        } else
        {
            audioSource.PlayOneShot(pop, SoundMixManager.getVolume(SoundMixManager.SoundType.POP_BONUS));
        }
    }

    public void PlayIdle()
    {
        if (audioSource.isPlaying)
            return;
        audioSource.clip = idle;
        audioSource.Play();
    }

    public void PlayBlow()
    {
        if (audioSource.isPlaying)
            return;
        audioSource.clip = blow;
        audioSource.Play();
    }

    // Stop any current playing clip
    public void StopAudio()
    {
        audioSource.Stop();
    }
}
