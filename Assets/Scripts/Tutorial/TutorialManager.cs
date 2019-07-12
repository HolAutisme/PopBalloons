using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TutorialManager : NetworkBehaviour {


    public enum TutorialState {INITIALISATION, POSITIONNING, WAITING_FOR_DEMONSTRATION, DEMONSTRATION, DIY, FINISH }
    public delegate void TutorialStateChanged(TutorialState ts);
    public static TutorialStateChanged onTutorialStateChanged;

    [Header("Settings :")]
    [SerializeField]
    private GameObject juliePrefab;
    [SerializeField]
    private GameObject tutorialBalloonPrefab;

    [SyncVar]
    public TutorialState currentState = TutorialState.INITIALISATION;
    private GameObject julie;
    private bool doctorSide = false;

    [Header("Focus :")]
    [SerializeField]
    [Range(0,5)]
    private float focusTimeRequired;
    [SerializeField]
    [Range(0.5f, 1)]
    private float focusTolerance;
    private float focusTime;


    Animator julieAnimator;
    Camera player;


	// Use this for initialization
	void Start ()
    {
        if (NetworkServer.active)
            NetworkServer.Spawn(this.gameObject);



        doctorSide =(SharingManager.getLocalPlayer().getPType() == Participant.ParticipantType.DOCTOR);
        if(!doctorSide)
            currentState = TutorialState.INITIALISATION;
        if (!doctorSide && currentState == TutorialState.INITIALISATION)
        {
            player = Camera.main;
            julie = Instantiate(juliePrefab);
            //Spawn julie near child in zone
            if (ShapeCreator.Instance)
            {
                GameObject l1, l2;
                l1 = GameObject.Find("Landmark1");
                l2 = GameObject.Find("Landmark2");

                julie.transform.position = (l1 != null && l2 != null)
                    ? l1.transform.position + (l2.transform.position - l1.transform.position) * 0.5f
                    : player.transform.position + new Vector3(0, -1, 4);
                GameObject target = ShapeCreator.Instance.GetCenter();
                julie.transform.LookAt(new Vector3(target.transform.position.x,julie.transform.position.y,target.transform.position.z),Vector3.up);
            }
            julieAnimator = julie.GetComponent<Animator>();

        }
	}
	
    [Command]
    void CmdChangeCurrentState(TutorialState ts)
    {
        currentState = ts;
        RpcChangeCurrentState(ts);
    }

    [ClientRpc]
    void RpcChangeCurrentState(TutorialState ts)
    {
        if (onTutorialStateChanged != null)
            onTutorialStateChanged(ts);
    }

    void initState(TutorialState nextState)
    {

        switch (nextState)
        {
            case TutorialState.POSITIONNING:
                //Event positionning
                break;

            case TutorialState.DEMONSTRATION:
                //RpcLaunchJulie();
                Invoke("goToDIY", 8);
                break;
            case TutorialState.DIY:
                Vector3 pos = player.transform.position + player.transform.forward * 2;
                pos.y = player.transform.position.y;
                Quaternion rot = Quaternion.Euler(-90, -90, 0);
                Instantiate(tutorialBalloonPrefab, SharingManager.getSharedCollection().InverseTransformPoint(pos),rot);
                TutorialBalloon.onTutorialBalloonPopped += goToFinish;
                break;
            case TutorialState.FINISH:
                TutorialBalloon.onTutorialBalloonPopped -= goToFinish;
                break;
        }

        CmdChangeCurrentState(nextState);
    }

    void goToDIY()
    {
        initState(TutorialState.DIY);
    }

    void goToFinish()
    {
        initState(TutorialState.FINISH);
    }


    void updateState(TutorialState ts)
    {
        switch (ts)
        {
            case TutorialState.INITIALISATION:
                if (ShapeCreator.Instance && CheckLimitArea.isOut)
                {
                    initState(TutorialState.POSITIONNING);
                    return;
                }
                initState(TutorialState.WAITING_FOR_DEMONSTRATION);
                break;

            case TutorialState.POSITIONNING:
                if (ShapeCreator.Instance && CheckLimitArea.isOut)
                    return;
                initState(TutorialState.WAITING_FOR_DEMONSTRATION);
                break;

            case TutorialState.WAITING_FOR_DEMONSTRATION:

                if (Vector3.Dot(player.transform.forward.normalized, (julie.transform.position - player.transform.position).normalized) > focusTolerance)
                {
                    focusTime += Time.deltaTime;
                }

                if (Vector3.Dot(player.transform.forward.normalized, (julie.transform.position - player.transform.position).normalized) < 0.5)
                {
                    focusTime =0;
                }

                if (focusTime > focusTimeRequired)
                    initState(TutorialState.DEMONSTRATION);
                break;
            default:
                //Nothing
                break;
        }
    }
        


    // Update is called once per frame
    void Update ()
    {
        if (!doctorSide)
        {
            updateState(currentState);
        }
    }
}
