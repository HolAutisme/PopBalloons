using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonDataRegistrer : MonoBehaviour {


    BalloonDatas currentBalloonData;


	// Use this for initialization
	void Start ()
    {
        BalloonBehaviour.OnBalloonSpawned += balloonSpawned;
        BalloonBehaviour.OnDestroyBalloon += balloonDestroyed;
        BalloonBehaviour.OnBalloonMissed += balloonMissed;
	}

    void InitBalloonData()
    {
        currentBalloonData = new BalloonDatas();
    }

    private void OnDestroy()
    {
        BalloonBehaviour.OnBalloonSpawned -= balloonSpawned;
        BalloonBehaviour.OnDestroyBalloon -= balloonDestroyed;
        BalloonBehaviour.OnBalloonMissed -= balloonMissed;
    }

    private void balloonMissed(float timeStamp, float duration, bool timeout)
    {
        currentBalloonData.lifeTime = duration;
        currentBalloonData.balloonWasDestroyByUser = false;
        currentBalloonData.balloonTimout = timeout;
        currentBalloonData.balloonPointGain = 0;
        currentBalloonData.timeOfDestroy = timeStamp;
        currentBalloonData.distance = (FootStepManager.instance != null)
        ? FootStepManager.instance.getDistance()
        : 0;
        AddDatas(currentBalloonData);
    }

    private void balloonDestroyed(float timeStamp, float duration, bool isBonus)
    {
        currentBalloonData.lifeTime = duration;
        currentBalloonData.balloonWasDestroyByUser = true;
        currentBalloonData.balloonTimout = false;
        currentBalloonData.balloonPointGain = ScoreManager.getScore(duration);
        currentBalloonData.timeOfDestroy = timeStamp;
        currentBalloonData.distance = (FootStepManager.instance != null)
            ? FootStepManager.instance.getDistance()
            : 0;
        AddDatas(currentBalloonData);
    }

    private void balloonSpawned(float timeStamp, Vector3 position)
    {
        InitBalloonData();
        if(FootStepManager.instance != null)
            FootStepManager.instance.initFootStep();
        currentBalloonData.balloonInitialPosition = position;
        currentBalloonData.timeOfSpawn = timeStamp;
    }


    private void AddDatas(BalloonDatas data)
    {
        if (DataManager.instance != null)
        {
            DataManager.instance.AddBalloonsDatas(data);
        }
    }


}
