using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepManager : MonoBehaviour {


    Vector2 previousPosition;
    Vector2 currentPosition;

    float sumDistance = 0;

    Participant player;

    public static FootStepManager instance;

	// Use this for initialization
	void Start ()
    {
        currentPosition = new Vector2();
        if (SharingManager.getLocalPlayer() != null)
            initPlayer();
        else
           SharingManager.OnLocalPlayerSet += initPlayer;
                
	}

    private void initPlayer()
    {
        player = SharingManager.getLocalPlayer();
        previousPosition = new Vector2(player.transform.position.x, player.transform.position.z);
    }

    public void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(player != null)
        {
            currentPosition.x = player.transform.position.x;
            currentPosition.y = player.transform.position.z;
            sumDistance += Vector2.Distance(previousPosition, currentPosition);
            previousPosition = currentPosition;


        }
    }


    public void initFootStep()
    {
        sumDistance = 0;
        previousPosition = new Vector2(player.transform.position.x, player.transform.position.z);
    }


    public float getDistance()
    {
        return sumDistance;
    }
}
