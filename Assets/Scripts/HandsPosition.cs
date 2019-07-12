using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class HandsPosition : MonoBehaviour {

    private Vector3 initPos;
    public GameObject virtualHand;
    public GameObject virtualHand2;

    private bool registeringData = true;
    private bool coroutineIsRunning = false;
    private bool handIsDetected = false;
    private bool handIsDisplay = false;
    private bool balloonExist = false;

    [Header("Virtual hand settings :")]

    [Tooltip("Maximum distance for displaying virtual hand indicator.")]
    [Range(0, 3)]
    [SerializeField]
    float distanceConsiderNear = 1.2f;

    
    [Tooltip("Time required before considering user is struggling with hand recognition.")]
    [Range(0, 5f)]
    [SerializeField]
    float timeBeforeDisplayingHand = 3f;

    [Tooltip("Value compare to a scalar product between child -> vector and camera -> forward")]
    [Range(0f, 1f)]
    [SerializeField]
    float seableAngleTolerance = 0.8f;

    [Header("Vocal & Textual Helper :")]
    [SerializeField]
    bool needHelp = false;

    [SerializeField]
    HelperSystem helpDisplayer;

    private Vector3 currentBalloonPosition;
    private float balloonSpawnTime;
    private float timeWhenBalloonWasSeenAndNear;
    private float timeCurrentlyBalloonNotSeen;
    private float timeCurrentlyBalloonIsSeen;

    private int nbBalloon = 0;


    void Awake()
    {
        virtualHand2.SetActive(handIsDisplay);
        initPos = transform.position;
        InteractionManager.SourceUpdated += GetPosition;
        InteractionManager.SourceLost += SourceLost;
        InteractionManager.SourceDetected += SourceDetected;
    }

    private void Start()
    {
        LoadLevel.OnLevelEnd += levelEnd;
        BalloonBehaviour.OnBalloonSpawned += balloonSpawned;
        if (needHelp)
        {
            //BalloonBehaviour.OnBalloonMissed += ballonMissed;
            BalloonBehaviour.OnDestroyBalloon += balloonDestroy;
            helpDisplayer.Display(HelperSystem.HelpRequired.INTRODUCTION);
        }
    }

    private void ballonMissed(float timeStamp, float duration, bool timeout)
    {
        if (timeout)
        {
            //helpDisplayer.Display(HelperSystem.HelpRequired.TOO_LONG);
        }
    }

    private void balloonDestroy(float timeStamp, float duration, bool isBonus)
    {
        nbBalloon++;
        if(nbBalloon >= 2)
        {
            BalloonBehaviour.OnBalloonMissed -= ballonMissed;
            BalloonBehaviour.OnDestroyBalloon -= balloonDestroy;
            needHelp = false;
        }
    }

    private void balloonSpawned(float timeStamp, Vector3 position)
    {
        if(virtualHand2 == null)
        {
            BalloonBehaviour.OnBalloonSpawned -= balloonSpawned;
        }

        timeCurrentlyBalloonNotSeen = 0;
        balloonExist = true;
        currentBalloonPosition = position;
        balloonSpawnTime = timeStamp;
    }

    private void OnDestroy()
    {
        BalloonBehaviour.OnBalloonSpawned -= balloonSpawned;
        BalloonBehaviour.OnBalloonMissed -= ballonMissed;
        BalloonBehaviour.OnDestroyBalloon -= balloonDestroy;
        LoadLevel.OnLevelEnd -= levelEnd;
        InteractionManager.SourceUpdated -= GetPosition;
        InteractionManager.SourceLost -= SourceLost;
        InteractionManager.SourceDetected -= SourceDetected;
    }

    private void levelEnd()
    {
        registeringData = false;
    }


    private void Update()
    {
        if(!coroutineIsRunning)
            StartCoroutine(TrackGesture());
        
        if (balloonExist)
        {
            timeWhenBalloonWasSeenAndNear = (BalloonIsSeen() && BalloonIsNear()) ? timeWhenBalloonWasSeenAndNear + Time.deltaTime : 0;
            if (needHelp)
            {

                timeCurrentlyBalloonNotSeen += (BalloonIsSeen()) ? 0 : Time.deltaTime;
                if(timeCurrentlyBalloonNotSeen > 5f)
                {
                    helpDisplayer.Display(HelperSystem.HelpRequired.TOO_LONG);
                    timeCurrentlyBalloonNotSeen = 0;
                }
                    
            }
            ManageHandVisibility();
        }
        
       

    }


    private void ManageHandVisibility()
    {
        bool  shouldBeDisplayingHand =  !handIsDetected && (timeWhenBalloonWasSeenAndNear > timeBeforeDisplayingHand) ;

        if(shouldBeDisplayingHand != handIsDisplay)
        {
            if(shouldBeDisplayingHand && needHelp)
            {
                helpDisplayer.Display(HelperSystem.HelpRequired.HAND_PLACEMENT);
            }
            handIsDisplay = shouldBeDisplayingHand;
            virtualHand2.SetActive(shouldBeDisplayingHand);
        }
    }


    private bool BalloonIsNear()
    {
        return (Vector3.Distance(Camera.main.transform.position, currentBalloonPosition) < distanceConsiderNear);
    }

    private bool BalloonIsSeen()
    {
        return (Vector3.Dot((currentBalloonPosition - Camera.main.transform.position).normalized, Camera.main.transform.forward.normalized) > seableAngleTolerance);
    }

    private void GetPosition(InteractionSourceState state)
    {
        Vector3 pos;
        if (virtualHand && state.source.kind == InteractionSourceKind.Hand && state.properties.location.TryGetPosition(out pos))
        {
            virtualHand.transform.position = initPos + pos;
        }
    }

    private void SourceLost(InteractionSourceState state)
    {
        handIsDetected = false;
        if (virtualHand)
        {
            virtualHand.SetActive(false);
        }

    }

    private void SourceDetected(InteractionSourceState state)
    {
        handIsDetected = true;
        if (virtualHand)
        {
            virtualHand.SetActive(true);
        }
    }

    private IEnumerator TrackGesture()
    {
        //Il ne faut PAS lancer plusieurs fois la coroutine.
        coroutineIsRunning = true;
        yield return new WaitForSeconds(0.01f);
        if (registeringData)
        {    
            UserDatas data = new UserDatas();
            float bpm = 0;
            if (SharingManager.instance != null && SharingManager.getLocalPlayer() != null)
                bpm = SharingManager.getLocalPlayer().getHeartData().getBPM();

            data.headPos = Camera.main.transform.position;
            data.BPM = bpm;
            data.headRotationY = Camera.main.transform.rotation.eulerAngles.y;
            data.timeStamp = TimerManager.getTimeStamp();
            if (virtualHand != null && DataManager.instance != null) DataManager.instance.AddUsersDatas(data);
        }
        coroutineIsRunning = false;
    }
}
