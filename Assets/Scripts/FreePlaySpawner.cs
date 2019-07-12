using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class FreePlaySpawner : MonoBehaviour
{
    /// <summary>
    /// Collection d'hologramme partagée par tous les utilisateurs
    /// </summary>
    private Transform sharedCollection;

    [Header("Game Settings :")]

    [SerializeField]
    [Range(30f, 120f)]
    float levelDuration = 90f;

    [SerializeField]
    AnimationCurve difficulty;

    [SerializeField]
    float initialBalloonFrequency;

    [SerializeField]
    float maximumBalloonFrequency;

    [SerializeField]
    [Range(1,8f)]
    int maxElementOnScene;

    [SerializeField]
    int currentMaxElementOnScene;

    [SerializeField]
    private GameObject countdown;

    [Header("Scene settings :")]
    [SerializeField]
    private LoadLevel levelManager;

    public float maxRadius;
    public List<GameObject> balloonPrefabs;
    public List<GameObject> balloonWithoutIndicatorPrefabs;
    public List<GameObject> balloonBonus;
    public GameObject CenterObject;
    public float secondsBetweenBalloon;
    public bool isFirstBalloon;


    private float nbBalloonPopped = 0;


    Vector3 center;
    GameObject clone;
    GameObject cloneBonus;
    Transform posBonus;
    Vector3 axis;


    static public List<GameObject> balloonsArray;
    public static float balloonDestroyed;
    private static FreePlaySpawner instance;

    public float IntermediateMaxValue
    {
        get
        {
            return Mathf.Lerp(currentMaxElementOnScene, maxElementOnScene, GetDifficultyFactor());
        }
    }

    /// <summary>
    /// Vérifie qu'il n'y a pas déjà trop de ballon dans la scene.
    /// </summary>
    public bool TooMuch
    {
        get
        {
            return SharingManager.getBalloonCount() >= IntermediateMaxValue;
        }
    }

    public static FreePlaySpawner Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        isFirstBalloon = true;
        balloonDestroyed = 0;
    }



    void Start()
    {
        Physics.gravity = new Vector3(0, -0.5f, 0);

        ScoreManager.onBalloonPopped += UpdateDifficulty;

        if (ShapeCreator.Instance != null && ShapeCreator.Instance.GetCenter() != null)
        {
            CenterObject = ShapeCreator.Instance.GetCenter();
        }

        center = CenterObject.transform.position;
        center.y = Camera.main.transform.position.y;
        // We get our anchor, where balloon will be spawn.
        sharedCollection = SharingManager.getSharedCollection();
        if (SharingManager.getLocalPlayer() == null)
        {

            //Waiting for localPlayer to be set.
            SharingManager.OnLocalPlayerSet += launchSpawning;

        }
        else
        {
            //Level is ready to begin;
            launchSpawning();
        }

    }

    private void OnDestroy()
    {
        ScoreManager.onBalloonPopped -= UpdateDifficulty;
    }

    private void UpdateDifficulty(float time, int scoreGain, bool isBonus)
    {
        nbBalloonPopped++;
    }


    /// <summary>
    ///     Launch coroutine of spawning balloon if and only if local player if the Child.
    /// </summary>
    void launchSpawning()
    {
        Participant p = SharingManager.getLocalPlayer();
        if (p != null && p.getPType() == Participant.ParticipantType.CHILD)
        {
            StartCoroutine(LoopSpawn());
        }

    }

    private bool CreateBalloons()
    {
        Vector3 pos;
        Quaternion rot;
        int rand = 0;

        if (isFirstBalloon)
        {
            pos = Camera.main.transform.position + new Vector3(0, 0, 3);
            isFirstBalloon = false;
        }
        else
            pos = RandomCircle(center, maxRadius);
        if (ShapeCreator.Instance)
            if (!LimitArea.ContainsPoint(ShapeCreator.Instance.GetLandmarksPoint(), new Vector2(pos.x, pos.z)))
                return false;
        rot = Quaternion.Euler(-90, -90, 0);

        if (SharingManager.getBalloonCount() >= 1)
        {
            rand = UnityEngine.Random.Range(0, balloonWithoutIndicatorPrefabs.Count);
            SharingManager.getLocalPlayer().CmdInstantiateGameObject(balloonWithoutIndicatorPrefabs[rand], pos, rot);
        }
        else
        {
            rand = UnityEngine.Random.Range(0, balloonPrefabs.Count);
            SharingManager.getLocalPlayer().CmdInstantiateGameObject(balloonPrefabs[rand], pos, rot);
            
        }
        SharingManager.instance.CmdIncrementBalloon();

        return true;
    }

    IEnumerator LoopSpawn()
    {

        // Level intro delay
        yield return new WaitForSeconds(4f);
        if(countdown != null)
        {
            countdown.SetActive(true);
            // Level intro delay
            yield return new WaitForSeconds(3.2f);
            //countdown.SetActive(false);
        }

        nbBalloonPopped = 0;
        float t = 0;
        float spawnTime = 0;

        TimerManager.levelBonusStart(levelDuration);
        while (t < levelDuration)
        {

            if (!TooMuch)
            {

                if (spawnTime < 0)
                {
                    spawnTime = (CreateBalloons()) ? GetSpawnTime() : spawnTime; //Performance Issue? 
                }
                else
                {
                    spawnTime -= Time.deltaTime;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        levelManager.LevelEndRequest();
    }

    private float GetSpawnTime()
    {
        return Mathf.Lerp(initialBalloonFrequency, maximumBalloonFrequency,GetDifficultyFactor());
    }

    public float GetDifficultyFactor()
    {
       
        float nbBalloonFactor = 0;

        if(TimerManager.getTime() > 0)
        {
            nbBalloonFactor = nbBalloonPopped / (TimerManager.getTime() / (initialBalloonFrequency + 2f));

        }
        return difficulty.Evaluate(Mathf.Clamp01(nbBalloonFactor / 2f) * (TimerManager.getTime()/levelDuration));
    }

    public float GetWeightingDifficultyFactor()
    {

        float nbBalloonFactor = 0;

        if (TimerManager.getTime() > 0)
        {
            nbBalloonFactor = nbBalloonPopped / (TimerManager.getTime() / (initialBalloonFrequency + 3.5f));

        }
        return difficulty.Evaluate(Mathf.Clamp01(nbBalloonFactor / 2f) * (TimerManager.getTime() - (0.6f* levelDuration) )/ (levelDuration -0.6f * levelDuration));
    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = UnityEngine.Random.value * 360;
        Vector3 pos;
        
        pos.x = center.x + UnityEngine.Random.Range(0.8f,radius) * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = UnityEngine.Random.Range(center.y + 0.25f, (center.y - 0.4f));
        pos.z = center.z + UnityEngine.Random.Range(0.2f, radius) * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public void AdaptBehaviour(BalloonBehaviour balloonBehaviour)
    {
        var timeFactor = TimerManager.getTime() / levelDuration;
        if(timeFactor < 0.6f)
        {
            string lvlName = "";
            if(timeFactor < 0.2f)
            {
                lvlName = "Level1";
            }
            else if (timeFactor < 0.4f)
            {
                lvlName = "Level2";
            }
            else if (timeFactor < 0.5f)
            {
                lvlName = "Level3";
            }
            else
            {
                lvlName = "Level4";
            }

            balloonBehaviour.adaptBehaviour(lvlName);
        }
        else
        {
            
            //var d = GetDifficultyFactor();
            //balloonBehaviour.Rigidbody.useGravity = true;
            //balloonBehaviour.Rigidbody.isKinematic = false;
            //balloonBehaviour.Rigidbody.drag = Mathf.Lerp(5.0f,0.0f, d);
            //r.AddForce(Vector3.down * d * 10,ForceMode.Impulse);

        }


    }
}