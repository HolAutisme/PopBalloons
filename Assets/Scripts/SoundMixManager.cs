using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundMixManager : NetworkBehaviour {

    public static SoundMixManager instance;

    [SyncVar(hook = "ChildVolumeChange")]
    private float childVolume;


    public delegate void childVolumeChanged(float f);
    public static event childVolumeChanged onChildVolumeChanged;


    #region ///////////////////// FX VOLUMES /////////////////////
    [Header("FX volumes :")]
    [Range(0, 1)]
    [SerializeField]
    public float fxVolume;

    [SerializeField]
    private FXSettings fxSettings;

    [System.Serializable]
    public class FXSettings
    {
        [Header("UI volumes :")]
        [Range(0, 1)]
        [SerializeField]
        public float buttonVolume = 0.4f;

        [Header("Inflate volumes :")]
        [Range(0, 1)]
        [SerializeField]
        public float inflateBalloonVolume = 0.3f;

        [Header("Pop volumes :")]
        [Range(0, 1)]
        [SerializeField]
        public float popBalloonVolume = 0.3f;

        [Range(0, 1)]
        [SerializeField]
        public float popBonusVolume = 0.5f;

        [Header("Beacon volumes :")]
        [Range(0, 1)]
        [SerializeField]
        public float beaconVolume = 1f;

        [Header("Props volumes :")]
        [Range(0, 1)]
        [SerializeField]
        public float spaceWindowVolume = 0.2f;

        [Header("Scoreboard volumes :")]
        [Range(0, 1)]
        [SerializeField]
        public float scoreboardTimerVolume = 0.2f;

        [Range(0, 1)]
        [SerializeField]
        public float scoreboardBalloonsVolume = 0.3f;

        [Range(0, 1)]
        [SerializeField]
        public float scoreboardBonusVolume = 0.6f;

        [Range(0, 1)]
        [SerializeField]
        public float scoreboardFocusVolume = 1f;
    }
    #endregion
    #region ///////////////////// BACKGROUND VOLUMES /////////////////////
    [Header("Background volumes :")]
    [Range(0, 1)]
    [SerializeField]
    public float backgroundVolume = 1f;

    [SerializeField]
    private BGSettings bgSettings;

    [System.Serializable]
    public class BGSettings
    {
        [Range(0, 1)]
        [SerializeField]
        public float bgEnfant = 0.2f;

        [Range(0, 1)]
        [SerializeField]
        public float bgEspace = 0.2f;

        [Range(0, 1)]
        [SerializeField]
        public float bgDino = 0.3f;        
    }

    #endregion
    #region ///////////////////// VOICES VOLUMES /////////////////////
    [Header("Voices volumes :")]
    [Range(0, 1)]
    [SerializeField]
    public float voiceVolume = 0.5f;
    #endregion

    [Header("Global volume :")]
    [Range(0, 1)]
    [SerializeField]
    private float globalVolume = 1f;

    public enum SoundType
    {
        //UI
        BUTTON,
        HINT,
        //Inflate sound
        INFLATE,
        //Pop sounds
        POP_BALLOON,
        POP_BONUS,
        //Background musics
        BACKGROUNDCHILD,
        BACKGROUNDSPACE,
        BACKGROUNDDINO,
        //Beacon sounds
        BEACON,
        //Prop sounds
        SPACE_WINDOW,
        //Scoreboard sounds
        SB_TIMER,
        SB_BALLOONS,
        SB_BONUS,
        SB_FOCUS
    }

    public static void setGlobalVolume(float volume)
    {
        if (instance != null)
        {
            if (instance.globalVolume != volume)
            {
                instance.globalVolume = volume;
            }
        }
    }

    public static void setChildVolume(float vol)
    {
        if(instance != null && instance.isServer)
        {
            instance.childVolume = vol;
            instance.globalVolume = vol;
        }
            
    }

    public void ChildVolumeChange(float vol)
    {
        childVolume = vol;
        if (onChildVolumeChanged != null)
            onChildVolumeChanged(vol);
    }

    public static float getGlobalVolume()
    {
        return instance.globalVolume;
    }

    public static float getChildVolume()
    {
        return instance.childVolume;
    }

    public void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            instance = this;
        }
    }


    public void Start()
    {
        if(isServer)
        {
            childVolume = globalVolume;
        }
    }

    public void Update()
    {
        AudioListener.volume = globalVolume;
    }

    public static float getVolume(SoundType type)
    {
        if (instance != null) { 
            switch (type)
            {
                case SoundType.BUTTON:
                    return instance.fxSettings.buttonVolume * instance.fxVolume;
                case SoundType.INFLATE:
                    return instance.fxSettings.inflateBalloonVolume * instance.fxVolume;
                case SoundType.POP_BALLOON:
                    return instance.fxSettings.popBalloonVolume * instance.fxVolume;
                case SoundType.POP_BONUS:
                    return instance.fxSettings.popBonusVolume * instance.fxVolume;
                case SoundType.BEACON:
                    return instance.fxSettings.beaconVolume * instance.fxVolume;
                case SoundType.SPACE_WINDOW:
                    return instance.fxSettings.spaceWindowVolume * instance.fxVolume;
                case SoundType.SB_TIMER:
                    return instance.fxSettings.scoreboardTimerVolume * instance.fxVolume;
                case SoundType.SB_BALLOONS:
                    return instance.fxSettings.scoreboardBalloonsVolume * instance.fxVolume;
                case SoundType.SB_BONUS:
                    return instance.fxSettings.scoreboardBonusVolume * instance.fxVolume;
                case SoundType.SB_FOCUS:
                    return instance.fxSettings.scoreboardFocusVolume * instance.fxVolume;

                case SoundType.HINT:
                    return instance.voiceVolume;

                case SoundType.BACKGROUNDCHILD:
                    return instance.bgSettings.bgEnfant * instance.backgroundVolume;
                case SoundType.BACKGROUNDSPACE:
                    return instance.bgSettings.bgEspace * instance.backgroundVolume;
                case SoundType.BACKGROUNDDINO:
                    return instance.bgSettings.bgDino * instance.backgroundVolume;
            }
        }
        return 0.5f;
    }
}
