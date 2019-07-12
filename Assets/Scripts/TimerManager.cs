using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimerManager : NetworkBehaviour {



    [SyncVar]
    private float time = 0;


    private float displayedTime = 0;

    /// <summary>
    /// Paramètre de lerp;
    /// </summary>
    [SerializeField]
    [Range(1, 50)]
    private float syncRate = 1;


    private static bool timeRunning = false;
    private static bool countdown = false;

    private static float initialTime = 90f;


    /// <summary>
    /// Instance publique du timeManager
    /// </summary>
    public static TimerManager Instance
    {
        get;private set;
    }

    
    /// <summary>
    /// Fonction qui retourne la valeur de temps actuelle du niveau en cours.
    /// </summary>
    /// <returns></returns>
    public static float getTimeStamp()
    {
        //if(Instance != null && Instance.isServer)
        //    return Instance.time;
        if (Instance != null)
            return Instance.displayedTime;
        return 0;
    }

    public static float getTime()
    {
        //if(Instance != null && Instance.isServer)
        //    return Instance.time;
        if (Instance != null)
            return Instance.time;
        return 0;
    }

    /// <summary>
    /// Fonction à appeler au lancement d'un niveau
    /// </summary>
    public static void levelStart()
    {
        if (Instance != null)
        {
            timeRunning = true;
            countdown = false;
        }
        else
        {
            Debug.LogWarning("Instance of TimeManager is null, timer not started.");
        }
    }

    /// <summary>
    /// Fonction à appeler au lancement d'un niveau
    /// </summary>
    public static void levelBonusStart(float duration)
    {
        if (Instance != null)
        { 
            timeRunning = true;
            countdown = true;
            initialTime = duration;
        }
        else
        {
            Debug.LogWarning("Instance of TimeManager is null, timer not started.");
        }
    }

    public static void levelEnd()
    {
        timeRunning = false;
        countdown = false;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else{
            DestroyImmediate(this.gameObject);
        }
    }

	
	// Update is called once per frame
	void Update ()
    {
	    if(isServer && timeRunning)
        {
            time += Time.deltaTime;
            if(countdown && time > initialTime)
            {
                
                time = initialTime;

                //Should handle it best (Delegate?)
                levelEnd();
            }

            displayedTime = (countdown)
                ? initialTime - time
                : time;
            
        }

        if(!isServer)
        {
            displayedTime = Mathf.Lerp(displayedTime, time, Time.deltaTime * syncRate);
        }

	}

    public static void initTimer()
    {
        if(Instance != null)
            Instance.CmdSetTime(0f);
    }

    [Command]
    private void CmdSetTime(float t)
    {
        time = t;
    }
}
