using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLink : MonoBehaviour {

    [SerializeField]
    private SoundMixManager.SoundType type;
    private AudioSource source;

    private void Start()
    {
        source = this.gameObject.GetComponent<AudioSource>();
        source.volume = SoundMixManager.getVolume(type);
    }
}
