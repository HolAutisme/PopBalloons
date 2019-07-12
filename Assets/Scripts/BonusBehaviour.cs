using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BonusBehaviour : NetworkBehaviour
{

    [SerializeField]
    private float moveFactor;
    public float amplitudeX = 0.10f;
    public float amplitudeY = 0.05f;
    public float omegaX = 1.0f;
    public float omegaY = 5.0f;
    public static event BalloonBehaviour.destroyAction OnDestroyBonus;
    public GameObject particleBurst;
    public GameObject particleBurstClone;
    public event EventHandler OnTouchHand2;
    public static bool isBonus;
    Color rend;
    float index;



    /// <summary>
    /// The transform of the shared world anchor.
    /// </summary>
    private Transform sharedWorldAnchorTransform;

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

    [Header("Network :")]
    /// <summary>
    /// Paramètre de lerp;
    /// </summary>
    [SerializeField]
    [Range(1, 50)]
    private float syncRate = 1;

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
    /// <summary>
    /// Fonction appellée lorsque l'on souhaite supprimer un ballon. Pourrait remplacer l'itégralité des Invoke de l'event OnTouchHand. Mais on préfère conserver le code existant.
    /// </summary>
    /// <param name="pRef">Pas forcément utile, mais doit correspondre à OnTouchHand</param>
    public void disposeBalloon(BonusBehaviour pRef)
    {
        this.CmdPopIt();
    }

    [Command]
    public void CmdPopIt()
    {
        particleBurstClone = Instantiate(particleBurst, SharingManager.getSharedCollection().transform.InverseTransformPoint(gameObject.transform.position), Quaternion.identity);
        particleBurstClone.GetComponent<SoundManager>().PlayConfetti();
        NetworkServer.Spawn(particleBurstClone);
        Destroy(particleBurstClone, 3.0f);

        NetworkServer.Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= onSceneChange;
    }

    private void Start()
    {
        sharedWorldAnchorTransform = HoloToolkit.Examples.SharingWithUNET.SharedCollection.Instance.gameObject.transform;
        transform.SetParent(sharedWorldAnchorTransform,false);
        isBonus = true;
        rend = gameObject.GetComponent<Renderer>().material.color;
        if (isServer)
        {
            SceneManager.sceneLoaded += onSceneChange;
        }
    }


    private void onSceneChange(Scene s, LoadSceneMode mode)
    {
        //On se désabonne
        SceneManager.sceneLoaded -= onSceneChange;
        if (isServer)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    public void Update()
    {
        //Seul l'hote instancie les ballons.
        if (!isServer)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition,syncRate * Time.deltaTime);
            transform.localRotation = localRotation;
            return;
        }


        index += Time.deltaTime;
        float x =  amplitudeX * Mathf.Cos(omegaX * index);

        float y = amplitudeY * Mathf.Cos(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor);
        float z = amplitudeY * Mathf.Cos(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor) * Mathf.Sin(omegaY * index * moveFactor);
        transform.position = new Vector3(x, y, z);
        //transform.position = new Vector3(x, y, LoadLevel.pos.z);
        gameObject.GetComponent<Renderer>().material.color = rend;
        rend.a -= Time.deltaTime * 0.125f;

        if (isServer && rend.a <= 0)
        {
            NetworkServer.Destroy(this.gameObject);
        }

        // Depending on if you are host or client, either setting the SyncVar (client) 
        // or calling the Cmd (host) will update the other users in the session.
        // So we have to do both.
        localPosition = transform.localPosition;
        localRotation = transform.localRotation;
        CmdTransform(localPosition, localRotation);

    }


    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "VirtualHand" || collision.gameObject.tag == "MainCamera")
        {
            if (OnDestroyBonus != null) OnDestroyBonus(-1,-1,isBonus);

            //gameObject.GetComponent<SoundManager>().PlayPop();
            disposeBalloon(this);
            //if (OnTouchHand != null) OnTouchHand.Invoke(this,);

        }
        if (collision.gameObject.tag == "Balloon")
        {

            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }

    }




}
