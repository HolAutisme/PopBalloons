using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVolumeValue : MonoBehaviour {

    public void ChangeVolume()
    {
        float childVolume = SoundMixManager.getChildVolume();

        if (childVolume == 0f)
        {
            childVolume = 0.25f;
        }
        else if (childVolume > 0f && childVolume <= 0.25f)
        {
            childVolume = 0.5f;
        }
        else if (childVolume > 0.25f && childVolume <= 0.5f)
        {
            childVolume = 0.75f;
        }
        else if (childVolume > 0.5f && childVolume <= 0.75f)
        {
            childVolume = 1f;
        }

        else if (childVolume > 0.75f && childVolume <= 1f)
        {
            childVolume = 0f;
        }

        SharingManager.getLocalPlayer().CmdUpdateSoundVolume(childVolume);
    }
}
