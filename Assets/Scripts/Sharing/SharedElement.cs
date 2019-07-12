using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class SharedElement : NetworkBehaviour {

    /// <summary>
    /// If true, object will spawn on server on start.
    /// </summary>
    [SerializeField]
    private bool autoSpawn = true;

    /// <summary>
    /// If true, object will spawn on server on start.
    /// </summary>
    [SerializeField]
    private bool interpolateValue = true;

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

    [SerializeField]
    private bool keepPosition = false;

    /// <summary>
    /// Paramètre de lerp;
    /// </summary>
    [SerializeField]
    [Range(1, 50)]
    private float syncRate = 10;

    // Use this for initialization
    void Start ()
    {
        if(autoSpawn && NetworkServer.active)
            NetworkServer.Spawn(this.gameObject);
        this.transform.SetParent(SharingManager.getSharedCollection(), keepPosition);
	}

    // Called every frame
    void Update()
    {
        if (!isServer)
        {
            transform.localPosition = (interpolateValue)
                ?Vector3.Lerp(transform.localPosition, localPosition,syncRate * Time.deltaTime)
                :localPosition;
            transform.localRotation = localRotation;
            return;
        }


        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        CmdTransform(localPosition, localRotation);
        
    }

    /// <summary>
    /// Delete instance on server
    /// </summary>
    public void removeInstance()
    {
        if(hasAuthority)
            NetworkServer.Destroy(this.gameObject);
    }

    /// <summary>
    /// Move object instance and position on server>)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    [Command]
    public void CmdMove(Vector3 position, Quaternion rotation)
    {
        if(isServer)
        {
            this.transform.position = position;
            this.transform.rotation = rotation;
        }

    }


    /// <summary>
    /// Sets the localPosition and localRotation on clients.
    /// </summary>
    /// <param name="postion">the localPosition to set</param>
    /// <param name="rotation">the localRotation to set</param>
    [Command]
    public void CmdTransform(Vector3 position, Quaternion rotation)
    {
        if (!isLocalPlayer)
        {
            localPosition = position;
            localRotation = rotation;
        }
    }

    public override int GetNetworkChannel()
    {
        return Channels.DefaultUnreliable;
    }
}
