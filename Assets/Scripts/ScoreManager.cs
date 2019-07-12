using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreManager : NetworkBehaviour
{
    [SyncVar]
    public int score;


    public static int maxScoreByBalloon = 35;
    public static int scoreBonusBalloon = 10;


    public delegate void scoreChange(int score, int scoreGain);
    public static event scoreChange onScoreChange;

    public delegate void balloonWasPopped(float time, int scoreGain, bool isBonus);
    public static event balloonWasPopped onBalloonPopped;


    // Use this for initialization
    public static ScoreManager Instance
    {
        get; private set;
    }


    

    private void Awake()
    {
        if(Instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
    }

    public static void initScore()
    {
        if (Instance != null)
        {
            Instance.CmdSetScore(0);
        }
           
    }


    [Command]
    public void CmdSetScore(int sc)
    {
        Instance.score = sc;
        RpcSetScore(sc);
    }

    [ClientRpc]
    public void RpcSetScore(int sc)
    {
        //Update Score UI
        if (onScoreChange != null)
        {
            onScoreChange(sc, 0);
        }
    }


    private void OnEnable()
    {
        BalloonBehaviour.OnDestroyBalloon += ManageBalloonData;
        BonusBehaviour.OnDestroyBonus += ManageBalloonData;
    }
    private void OnDisable()
    {
        BalloonBehaviour.OnDestroyBalloon -= ManageBalloonData;
        BonusBehaviour.OnDestroyBonus -= ManageBalloonData;
    }

    public static float getScore(float duration)
    {
        return (duration < 7.0f)
                ? maxScoreByBalloon - 5 * (int)Mathf.Floor(duration)
                : 5;
    }

    void ManageBalloonData(float time,float duration, bool isBonus)
    {
        int scoreToAdd = (isBonus)
            ? scoreBonusBalloon
            : (duration < 7.0f)
                ? maxScoreByBalloon - 5 * (int) Mathf.Floor(duration)
                : 5;

        score += scoreToAdd;

        //Update Score UI
        if (onScoreChange != null)
        {
            onScoreChange(score,scoreToAdd);
        }

        if (onBalloonPopped != null)
        {
            onBalloonPopped(time, scoreToAdd, isBonus);
        }


    }


    /** OLD Incremente Score 
     void IncrementeScore()
     {
         StarScore.Play();
         if (BalloonBehaviour.highScore )
         {
             
             int i = (int)Mathf.Floor( BalloonBehaviour.timeOfCollision);
                 int scoreGiven = 35 - (5 * i);
               
                     score += scoreGiven;
             
             if (UIScoreAnim.isPlaying)
             {
                 UIScoreAnim.Stop();
                 UIScoreText.text = "+" + scoreGiven.ToString();
             }
             else
                 UIScoreText.text = "+"+ scoreGiven.ToString();
             UIScoreAnim.Play();
         }
         else if (BonusBehaviour.isBonus)
         {
             score += 10;

             if (UIScoreAnim.isPlaying)
             {
                 UIScoreAnim.Stop();
                 UIScoreText.text = "+" + (int.Parse(UIScoreText.text) + 10).ToString();
             }
             else
                 UIScoreText.text = "+10";
             UIScoreAnim.Play();
         }
         else
         {
             score += 5;
             if (UIScoreAnim.isPlaying)
             {
                 UIScoreAnim.Stop();
                 UIScoreText.text = "+" + (int.Parse(UIScoreText.text) + 5).ToString();
             }
             else
                 UIScoreText.text = "+5";
             UIScoreAnim.Play();
         }

         if(onScoreChange != null)
         {
             onScoreChange(score);
         }

         
     }
     **/
}
