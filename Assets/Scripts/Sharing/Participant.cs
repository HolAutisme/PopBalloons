using HoloToolkit.Examples.SharingWithUNET;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Class permettant de gérer la différenciation de traitement entre enfant et médecin. La personne lançant l'application est un "enfant", tous les autres sont des "médecins"
/// </summary>
[NetworkSettings(sendInterval = 0.033f)]
public class Participant : NetworkBehaviour
{

    public delegate void successfullyInit(Participant p);
    public static event successfullyInit onSuccessfullyInit;
    public static event ScoreBoard.UIBoardChange OnBoardStatusChange;
    public static event LoadLevel.NextLevelRequested OnNextLevelRequested;
    public static event LoadLevel.RequestSceneLoad OnSceneRequested;

    #region Variables
    /// <summary>
    /// Enumération présentant la liste des type de participants possible. Pour l'instant enfant et médecin.
    /// </summary>
    public enum ParticipantType { CHILD, DOCTOR, NONE }

    /// <summary>
    /// Child transform, allow us to display specific data.
    /// </summary>
    private Participant child;

    /// <summary>
    /// Type de participant parmi l'énumération <seealso cref="ParticipantType"/>
    /// By default a new player is consider as a Doctor unless if it's the first to launch game.
    /// </summary>
    [SyncVar]
    public ParticipantType PType = ParticipantType.DOCTOR;

    /// <summary>
    /// The transform of the shared world anchor.
    /// </summary>
    private Transform sharedWorldAnchorTransform;

    /// <summary>
    /// Paramètre de lerp;
    /// </summary>
    [SerializeField]
    [Range(0.5f,50)]
    private float syncRate = 1;

    /// <summary>
    /// Data from heartRateMonitor, not shawn to child
    /// </summary>
    private HRMData heartData;


    private bool ptypeSet = false;


    /// <summary>
    /// The position relative to the shared world anchor.
    /// </summary>
    [SyncVar]
    private Vector3 localPosition;

    /// <summary>
    /// The rotation relative to the shared world anchor.
    /// </summary>
    [SyncVar]
    private Quaternion localRotation;

    #endregion


    /// <summary>
    /// Sets the localPosition and localRotation on clients.
    /// </summary>
    /// <param name="postion">the localPosition to set</param>
    /// <param name="rotation">the localRotation to set</param>
    [Command(channel = 1)]
    public void CmdTransform(Vector3 position, Quaternion rotation)
    {
        if (!isLocalPlayer)
        {
            localPosition = position;
            localRotation = rotation;
        }
    }


    /// <summary>
    /// Instancie un prefab passé en paramètre à la position souhaité. Attention celui-ci doit être déclaré dans la liste des prefabs instantiable du NetworkManager et la position sera recalculée par rapport à la SharedAnchor.
    /// </summary>
    /// <param name="prefab">Prefab à instancier</param>
    /// <param name="pos">Position souhaitée</param>
    /// <param name="rot">Rotation souhaitée</param>
    [Command]
    public void CmdInstantiateGameObject(GameObject prefab,Vector3 pos, Quaternion rot)
    {
        if(!isServer)
        {
            return;
        }

        // The object desired position needs to be transformed relative to the shared anchor.
        GameObject gameObject = (GameObject)Instantiate(prefab, SharingManager.getSharedCollection().InverseTransformPoint(pos), rot);
        //gameObject.transform.parent = this.transform.parent;
        NetworkServer.Spawn(gameObject);

    }


    /// <summary>
    /// Retourne le type du participant
    /// </summary>
    /// <returns>ParticipationType</returns>
    public ParticipantType getPType()
    {
        return this.PType;
    }

    /// <summary>
    /// Met à jour le type du participant chez sur l'ensemble des clients de l'application.
    /// </summary>
    /// <param name="type"></param>
    [Command]
    public void CmdSetPType(ParticipantType type)
    {
        PType = type;
    }


    /// <summary>
    /// On s'assure que l'instance est suprimée de l'ensemble des participants.
    /// </summary>
    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }
    }

    /// <summary>
    /// Appelé lorsqu'un joueur rejoint la scène. Permet d'attribuer un rôle (docteur / enfant)
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        //La première personne connectée est l'enfant (hôte)
        this.PType = (this.isServer)
         ? ParticipantType.CHILD
         : ParticipantType.DOCTOR;
        SharingManager.setLocalPlayer(this);
        if (isLocalPlayer)
        {
            CmdSetPType(this.PType);
        }
    }


    void Start ()
    {

        sharedWorldAnchorTransform = SharedCollection.Instance.gameObject.transform;
        transform.SetParent(sharedWorldAnchorTransform);
        if(heartData == null)
            heartData = this.GetComponent<HRMData>();
    }
	
	void Update ()
    {
        // If we aren't the local player, we just need to make sure that the position of this object is set properly
        // so that we properly render their avatar in our world.
        if (!isLocalPlayer)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, syncRate*Time.deltaTime);
            transform.localRotation = localRotation;
            return;
        }

        
        //CmdSetPType(this.PType);


        // if we are the remote player then we need to update our worldPosition and then set our 
        // local (to the shared world anchor) position for other clients to update our position in their world.
        transform.position = CameraCache.Main.transform.position;
        transform.rotation = CameraCache.Main.transform.rotation;

        // Depending on if you are host or client, either setting the SyncVar (client) 
        // or calling the Cmd (host) will update the other users in the session.
        // So we have to do both.
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        CmdTransform(localPosition, localRotation);
    }
    
    /// <summary>
    /// Retourne les données HRM Data.
    /// </summary>
    /// <returns></returns>
    public HRMData getHeartData()
    {
        if(heartData == null)
            heartData = this.GetComponent<HRMData>();
        return this.heartData;
    }


    [Command]
    public void CmdNextLevel()
    {
        if (OnNextLevelRequested != null)
        {
            OnNextLevelRequested.Invoke();
        }
    }


    [Command]
    public void CmdLoadScene(string sceneName)
    {
        if (OnSceneRequested != null)
        {
            OnSceneRequested.Invoke(sceneName);
        }
        
    }

    [Command]
    public void CmdUpdateSoundVolume(float f)
    {
        SoundMixManager.setChildVolume(f);
       
    }

    /// <summary>
    ///  We need to have authority on object to execute command on server
    /// </summary>
    [Command]
    public void CmdUpdateStatus(ScoreBoard.boardStatus current)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == SharingManager.lastLevel && current == ScoreBoard.boardStatus.FINAL)
            current = ScoreBoard.boardStatus.LAST_LEVEL_FINAL;
        if (OnBoardStatusChange != null)
            OnBoardStatusChange.Invoke(current);
        RpcUpdateStatus(current);
    }


    [ClientRpc]
    public void RpcUpdateStatus(ScoreBoard.boardStatus status)
    {
        if (OnBoardStatusChange != null)
        {
            OnBoardStatusChange.Invoke(status);
        }
    }

    public override int GetNetworkChannel()
    {
        return Channels.DefaultUnreliable;
    }

}
