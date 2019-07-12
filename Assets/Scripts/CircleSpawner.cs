using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CircleSpawner : MonoBehaviour
{
    /// <summary>
    /// Collection d'hologramme partagée par tous les utilisateurs
    /// </summary>
    private Transform sharedCollection;


    [SerializeField]
    private GameObject countdown;
    public int maxElementOnScene;
    public float radius;
    public List<GameObject> balloonPrefabs;
    public List<GameObject> balloonBonus;
    public GameObject CenterObject;
    public float secondsBetweenBalloon;
    public bool isFirstBalloon;
    Vector3 center;
    GameObject clone;
    GameObject cloneBonus;
    Transform posBonus;
    Vector3 axis;


    static public List<GameObject> balloonsArray;
    public static float balloonDestroyed;


    /// <summary>
    /// Vérifie qu'il n'y a pas déjà trop de ballon dans la scene.
    /// </summary>
    public bool TooMuch
    {
        get
        {
            return SharingManager.getBalloonCount() >= maxElementOnScene;
        }
    }

    private void Awake()
    {

        isFirstBalloon = true;
        balloonDestroyed = 0;
    }



    void Start()
    {
        Physics.gravity = new Vector3(0, -0.5f, 0);

        
        //center = CenterObject.transform.position;

        if(ShapeCreator.Instance != null && ShapeCreator.Instance.GetCenter() != null)
        {
            CenterObject = ShapeCreator.Instance.GetCenter();
        }

        center = CenterObject.transform.position;

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



    IEnumerator CreateBalloons()
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
            pos = RandomCircle(center, radius);
        if (ShapeCreator.Instance)
            if (!LimitArea.ContainsPoint(ShapeCreator.Instance.GetLandmarksPoint(), new Vector2(pos.x, pos.z)))
                yield break;
        rot = Quaternion.Euler(-90, -90, 0);
        rand = Random.Range(0, balloonPrefabs.Count);
        SharingManager.getLocalPlayer().CmdInstantiateGameObject(balloonPrefabs[rand], pos, rot);
        SharingManager.instance.CmdIncrementBalloon();
        yield return new WaitForSeconds(secondsBetweenBalloon);
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

        TimerManager.levelStart();
        while (true)
        {

            while (!TooMuch && balloonDestroyed < 5)
            {

                yield return CreateBalloons();
            }

            yield return new WaitForSeconds(2f);
        }
    }


    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        var y = Camera.main.transform.position.y;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = Random.Range(y + 0.1f, (y - 0.5f));
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}