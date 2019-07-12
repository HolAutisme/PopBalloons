using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintHandler : MonoBehaviour {

    /// <summary>
    /// BALLOON_TO_FAR : Le ballon est dans le champ de vision, mais l'enfant est trop loin
    /// HOW_TO_POP : Le ballon est à porté depuis quelques temps mais l'enfant ne l'éclate pas
    /// BALLOON_BEHIND : Le ballon est derrière l'enfant.
    /// Never used.
    /// </summary>
    private enum HintType {RANDOM, HOW_TO_POP, NOT_FOUND}

    [Header("Visual fields :")]
    [SerializeField]
    [Tooltip("Text value that will change.")]
    private UnityEngine.UI.Text Quote;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AudioSource audioSource;


    [Header("Hint settings :")]
    [Range(0,20)]
    [SerializeField]
    float hintFrequency = 5f;

    [Range(0,15)]
    [SerializeField]
    float considerHardTime = 7f;

    [Tooltip("Maximum distance required for triggering balloon can't pop advise.")]
    [Range(0,3)]
    [SerializeField]
    float distanceConsiderNear = 0.8f;

    [Tooltip("Value compare to a scalar product between child -> vector and camera -> forward")]
    [Range(0f,1f)]
    [SerializeField]
    float seableAngleTolerance = 0.8f;
    
    [Range(0f,20f)]
    [SerializeField]
    float HintDuration = 5f;

    [Header("Advise list : ")]
    [SerializeField]
    private GameObject randomAdvise;

    [SerializeField]
    private GameObject cantPop;

    [SerializeField]
    private GameObject balloonNotFound;



    List<Hint> randomHintList;
    List<Hint> cantPopHintList;
    List<Hint> balloonNotFoundHintList;

    GameObject player;
    Vector3 balloonPosition;
    float timeSinceLastHint;
    float distance;
    float time;
    bool inLevel = false;
    //Is set to true because we want a captcha which tell us we joined the game
    bool hintDisplayed = true;

    // Use this for initialization
    void Start ()
    {
        BalloonBehaviour.OnBalloonSpawned += balloonSpawned;
        randomHintList = new List<Hint>(randomAdvise.gameObject.GetComponentsInChildren<Hint>(true));
        cantPopHintList = new List<Hint>(cantPop.gameObject.GetComponentsInChildren<Hint>(true));
        balloonNotFoundHintList = new List<Hint>(balloonNotFound.gameObject.GetComponentsInChildren<Hint>(true));

        if (SharingManager.getHostPlayer() != null)
        {
            this.setPlayer(SharingManager.getHostPlayer().gameObject);
        }
        else
        {
            SharingManager.OnHostPlayerSet += setPlayer;
        }

        //We hide the indication that we successfully join the game
        Invoke("hideHint", HintDuration);

    }


    private void OnDestroy()
    {
        BalloonBehaviour.OnBalloonSpawned -= balloonSpawned;
        SharingManager.OnHostPlayerSet -= setPlayer;
    }

    private void setPlayer(GameObject g)
    {
        player = g;
    }

    private void balloonSpawned(float timeStamp, Vector3 position)
    {
        inLevel = true; //Bug lors du retour au menu
        balloonPosition = position;
        time = timeStamp;
    }


    private void displayHint(HintType type)
    {
        hintDisplayed = true;
        Hint hint;
        switch (type)
        {
            case HintType.HOW_TO_POP:
                hint = cantPopHintList[UnityEngine.Random.Range(0, cantPopHintList.Count)];
                break;
            case HintType.NOT_FOUND:
                hint = balloonNotFoundHintList[UnityEngine.Random.Range(0, balloonNotFoundHintList.Count)];
                break;
            case HintType.RANDOM:
            default:
                hint = randomHintList[UnityEngine.Random.Range(0, randomHintList.Count)];
                break;
        }
        if (hint != null && hint.GetText() != null)
            showHint(hint.GetText().text);
        if( hint != null)
            PlaySound(hint.GetAudioClip());

    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null) {
            audioSource.PlayOneShot(clip, SoundMixManager.getVolume(SoundMixManager.SoundType.HINT));
        }
    }



    private void showHint(string hint)
    {
        if(Quote != null)
            Quote.text = hint;

        if(animator != null)
        {
            animator.Play("iAppear");
            Invoke("hideHint", HintDuration);
        }
    }

    private void hideHint()
    {
        if(animator != null)
        {
            animator.Play("iDisappear");
        }
        timeSinceLastHint = 0;
        hintDisplayed = false;
    }


    // Update is called once per frame
    void Update ()
    {
        if (!hintDisplayed && inLevel)
        {
            //ON display un hint
            if(timeSinceLastHint > hintFrequency)
            {
                //Hard time exploding balloon
                if(TimerManager.getTimeStamp() - time > 7)
                {
                    //Show him
                    if (!balloonIsSeen())
                    {
                        displayHint(HintType.RANDOM);
                    }

                    //Teach him
                    if(balloonisNear())
                    {
                        displayHint(HintType.RANDOM);
                    }
                    
                    //The child sees the balloon, we don't have to display any hint.
                }
                else
                {
                    displayHint(HintType.RANDOM);
                }
            }
            timeSinceLastHint += Time.deltaTime;
        }
	}

    /// <summary>
    /// Check if balloon is near player
    /// </summary>
    /// <returns></returns>
    private bool balloonisNear()
    {
        return (Vector3.Distance(balloonPosition, player.transform.position) < this.distanceConsiderNear);
    }

    private bool balloonIsSeen()
    {
        return (Vector3.Dot((balloonPosition - player.transform.position).normalized, player.transform.forward.normalized) > seableAngleTolerance);
    }
}
