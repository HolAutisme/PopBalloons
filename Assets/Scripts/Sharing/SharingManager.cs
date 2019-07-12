using HoloToolkit.Examples.SharingWithUNET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// Handle all knowledge about the scene status among all participant
/// </summary>
public class SharingManager : NetworkBehaviour {


    public delegate void LocalPlayerSet();
    public static event LocalPlayerSet OnLocalPlayerSet;

    public delegate void HostPlayerSet(GameObject g);
    public static event HostPlayerSet OnHostPlayerSet;

    /// <summary>
    /// Variable indiquant le nombre de ballons présents dans la scene
    /// </summary>
    [SyncVar]
    private int balloonCount;

    /// <summary>
    /// Variable indiquant le nombre de ballons présents dans la scene
    /// </summary>
    [SyncVar]
    private float childVolume;

    /// <summary>
    /// Variable that allow us to get the localPlayer coordinate anyTime on each hololens.
    /// </summary>
    private static Participant localPlayer;

    /// <summary>
    /// Variable that allow us to get the hostPlayer coordinate anyTime on each hololens.
    /// </summary>
    private static Participant hostPlayer;


    /// <summary>
    /// Instance du sharing Manager, doit être unique dans chaque scene.
    /// </summary>
    public static SharingManager instance;

    /// <summary>
    /// Instance du sharedCollection où sont instanciés les éléments partagés.
    /// </summary>
    private static Transform sharedCollection;

    /// <summary>
    /// String corresponding of the last possible level.
    /// </summary>
    public static string lastLevel = "Level4";

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            DontDestroyOnLoad(this);
            instance = this;
           
        }
    }


    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += sceneChanged;
    }

    private void sceneChanged(Scene sc, LoadSceneMode md)
    {
        if (isServer)
            this.balloonCount = 0;
    }

    /// <summary>
    /// Retourne le nombre de ballons présente dans la scene.
    /// </summary>
    /// <returns>Un int contenant le nombre de ballon</returns>
    public static int getBalloonCount()
    {
        return instance.balloonCount;
    }

    /// <summary>
    ///  Retourne le transform de la sharedCollection présente dans la scene
    /// </summary>
    /// <returns>Le transform de la sharedCollection</returns>
    public static Transform getSharedCollection()
    {
        if (sharedCollection == null)
        {
            SharedCollection s = GameObject.FindObjectOfType<SharedCollection>();
            sharedCollection = (s!=null)? s.GetComponent<Transform>():null;

        }
        return sharedCollection;
    }

    [Command]
    public void CmdIncrementBalloon()
    {
        this.balloonCount++;
    }


    [Command]
    public void CmdDecrementBalloon()
    {
        this.balloonCount--;
    }
    



    /// <summary>
    /// Retourne l'instance du player local,  différent dans chaque instance de la partie.
    /// </summary>
    /// <returns>Le player "local"</returns>
    public static Participant getLocalPlayer()
    {
        return localPlayer;
    }

    /// <summary>
    /// Affecte un nouveau player "Local" vers qui les UI seront orientées
    /// </summary>
    /// <param name="p">Le participant (docteur ou enfant)</param>
    public static void setLocalPlayer(Participant p)
    {
        localPlayer = p;
        if(p != null && OnLocalPlayerSet != null)
        {
            OnLocalPlayerSet();
        }
    }


    /// <summary>
    /// Retourne l'instance du player hote,  commun à chaque instance de la partie.
    /// </summary>
    /// <returns>Le player "local"</returns>
    public static Participant getHostPlayer()
    {
        return localPlayer;
    }

    /// <summary>
    /// Set a new player as "Host" to whom doctor UI will be oriented. 
    /// </summary>
    /// <param name="p">The participant (child) <see cref="Participant"/></param>
    public static void setHostPlayer(Participant p)
    {
        hostPlayer = p;
        if (p != null && OnHostPlayerSet != null)
        {
            OnHostPlayerSet(hostPlayer.gameObject);
        }
    }

}
