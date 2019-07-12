using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

[NetworkSettings(sendInterval = 0.1f)]
public class BalloonBehaviour : NetworkBehaviour
{

    #region
    /// <summary>
    /// Animator of the balloon
    /// </summary>
    Animator animator;
    Rigidbody rigidbody;
    #endregion


    /// <summary>
    /// Paramètre de lerp;
    /// </summary>
    [SerializeField]
    [Range(1, 10)]
    private float syncRate = 1;


    static public float timeOfCollision;
    public float initializationTime;
    static public bool highScore;
    public delegate void destroyAction(float timeStamp,float duration,bool isBonus);
    public static event destroyAction OnDestroyBalloon;
    private float amplitude = 0.02f;
    public float frequency = 0.33f;
    public bool shouldFloat;
    [SerializeField]
    private GameObject particleBurst;
    private GameObject particleBurstClone;
    private Vector3 posOffset = new Vector3();
    private Vector3 tempPos = new Vector3();
    public delegate void EventHandler(BalloonBehaviour pRef);
    public event EventHandler OnTouchHand;
    private bool isOnFloor = false;
    private bool popOnce = false;
    private float balloonDuration = -1f;

    public delegate void balloonSpawned(float timeStamp, Vector3 position);
    public static event balloonSpawned OnBalloonSpawned;
    public delegate void balloonMissed(float timeStamp, float duration,bool timeout);
    public static event balloonMissed OnBalloonMissed;


    /// <summary>
    /// Booleen permettant de differencier les cause d'appel de la fonction ondestroy.
    /// </summary>
    private bool quittingApplication = false;

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

    

    /// <summary>
    /// Sets the localPosition and localRotation on clients.
    /// </summary>
    /// <param name="position">the localPosition to set</param>
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
    /// Fonction appellé lorsque l'on quitte l'application, permet de différencier les cas de destroy de ballons
    /// </summary>
    private void OnApplicationQuit()
    {
        quittingApplication = true;
    }

    public static void RaiseBalloonSpawn(float time,Vector3 position)
    {
        if(OnBalloonSpawned != null)
        {
            OnBalloonSpawned.Invoke(time, position);
        }
    }

    // Use this for initialization
    void Start()
    {
        sharedWorldAnchorTransform = HoloToolkit.Examples.SharingWithUNET.SharedCollection.Instance.gameObject.transform;
        transform.SetParent(sharedWorldAnchorTransform,false);

        initializationTime = TimerManager.getTimeStamp();
        posOffset = transform.position;

        animator = this.GetComponent<Animator>();
        rigidbody = this.GetComponent<Rigidbody>();

        if(OnBalloonSpawned != null)
        {
            OnBalloonSpawned.Invoke(initializationTime, this.transform.position);
        }

        //this.enabled = false;

        //TODO : AddToList(cloneBehavior); Static?
        if (isServer)
        {
            this.OnTouchHand += disposeBalloon;
        }
        adaptBehaviour(SceneManager.GetActiveScene().name);

        GameObject managers = GameObject.Find("Managers");
        particleBurst = managers.GetComponent<PopParticlesManager>().RandomConfetti();
        particleBurst.GetComponent<Renderer>().material = managers.GetComponent<PopParticlesManager>().RandomMat();
    }

    // Update is called once per frame
    void Update()
    {
        //Seul l'hote instancie les ballons.
        if (!isServer)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, syncRate * Time.deltaTime);
            transform.localRotation = localRotation;
            return;
        }

        float timeSinceInstantiation = TimerManager.getTimeStamp() - initializationTime;

        highScore = (timeSinceInstantiation <= 7.0f);

        timeOfCollision = Mathf.FloorToInt(timeSinceInstantiation);
        if (timeSinceInstantiation >= 15.0f && !popOnce)
        {
            popOnce = true;
            //gameObject.GetComponent<SoundManager>().PlayPop();
            //particleBurstClone = Instantiate(particleBurst, gameObject.transform.position, Quaternion.identity);
            //particleBurstClone.GetComponent<SoundManager>().PlayAndPopConfetti();
            //Destroy(particleBurstClone, 3.0f);
            if (OnBalloonMissed != null) OnBalloonMissed(TimerManager.getTimeStamp(), TimerManager.getTimeStamp() - initializationTime, true);
            if (OnTouchHand != null) OnTouchHand.Invoke(this);
            timeSinceInstantiation = 0;
            
        }
        //floating behaviour
        if (shouldFloat)
        {

            tempPos = posOffset;
            tempPos.x += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

            transform.position = tempPos;
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
        //Seul l'hote instancie les ballons.
        if (!isServer)
        {
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponentInChildren<Collider>());
            return;
        }

        if (collision.gameObject.tag == "VirtualHand" || collision.gameObject.tag == "MainCamera")
        {
            balloonDuration = TimerManager.getTimeStamp() - initializationTime;
            if (OnDestroyBalloon != null) OnDestroyBalloon(TimerManager.getTimeStamp(),TimerManager.getTimeStamp() - initializationTime,false); //Incrémente le score
            if (OnTouchHand != null) OnTouchHand.Invoke(this);// Détruit les objets et fait apparaitre des particules
        }
        if (collision.gameObject.tag == "Balloon")
        {
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponentInChildren<Collider>());
        }
        else if (collision.gameObject.tag == "Spatial" || gameObject.transform.position.y <= -0.5f)
        {
            isOnFloor = true;
            if (OnBalloonMissed != null) OnBalloonMissed(TimerManager.getTimeStamp(), TimerManager.getTimeStamp() - initializationTime, false);
            if (OnTouchHand != null) OnTouchHand.Invoke(this);
        }

    }

    /// <summary>
    /// Fonction appellée lorsque l'on souhaite supprimer un ballon. Pourrait remplacer l'intégralité des Invoke de l'event OnTouchHand. Mais on préfère conserver le code existant.
    /// </summary>
    /// <param name="pRef">Pas forcément utile, mais doit correspondre à OnTouchHand</param>
    public void disposeBalloon(BalloonBehaviour pRef)
    {
        //Si ce n'est pas le joueur qui éclate le ballon, on ne le compte pas.
        if(!pRef.isOnFloor && !pRef.popOnce)
            CircleSpawner.balloonDestroyed++;
        if (isServer)
            SharingManager.instance.CmdDecrementBalloon();
        this.CmdPopIt();
    }


    [Command]
    public void CmdPopIt()
    {
        //gameObject.GetComponent<SoundManager>().PlayPop();
        particleBurstClone = Instantiate(particleBurst, sharedWorldAnchorTransform.InverseTransformPoint(gameObject.transform.position), Quaternion.identity);
        //particleBurstClone.transform.parent = sharedWorldAnchorTransform;
        particleBurstClone.GetComponent<SoundManager>().PlayPopAndConfetti();
        //Récupérer score et affecter au particle
        
        if (balloonDuration >= 0)
        {
            float score = ScoreManager.getScore(balloonDuration);
            particleBurstClone.GetComponent<ConfettiTextScore>().setTextScore(score);
        }
        NetworkServer.Spawn(particleBurstClone);
        Destroy(particleBurstClone, 3.0f);

        NetworkServer.Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (!quittingApplication)
        {
            //gameObject.GetComponent<SoundManager>().PlayPop();
        }
    }


    public void adaptBehaviour(string name)
    {
        if (rigidbody == null)
        {
            Debug.LogError("RigidBody should'nt be null");
            return;
        }

        switch (name)
        {
            case "Level1":
                this.shouldFloat = false;
                this.frequency = 0f;
                rigidbody.useGravity = false;
                //animator.enabled = true;
                break;
            case "Level2":
                this.shouldFloat = true;
                this.frequency = 0.3f;
                this.amplitude = 0.20f;
                rigidbody.useGravity = false;
                break;
            case "Level3":
                this.shouldFloat = false;
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.drag = 10.0f;
                break;
            case "Level4":
                this.shouldFloat = false;
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.drag = 5.0f;
                break;
            case "LevelBonus":
                this.shouldFloat = false;
                if (FreePlaySpawner.Instance != null)
                {
                    rigidbody.drag = Mathf.Lerp(5.0f, 0f, FreePlaySpawner.Instance.GetWeightingDifficultyFactor());
                    FreePlaySpawner.Instance.AdaptBehaviour(this);
                }
                break;
            default:
                this.shouldFloat = false;
                rigidbody.useGravity = false;
                rigidbody.isKinematic = false;
                break;
        }
    }

}