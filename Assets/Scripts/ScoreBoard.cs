using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ScoreBoard : NetworkBehaviour, IFocusable {

    /// <summary>
    /// SharedElement attribute attached to this object
    /// </summary>
    SharedElement sh;

    private int balloonDisplayed = 0;


    private GameObject normalState;

    /// <summary>
    /// Affichage du score
    /// </summary>
    [SerializeField]
    private UnityEngine.UI.Text scoreText;

    /// <summary>
    /// Affichage du nombre de points gagnés
    /// </summary>
    [SerializeField]
    private UnityEngine.UI.Text scorePointAdd;

    /// <summary>
    /// Animator du game object du score.
    /// </summary>
    [SerializeField]
    private Animator scoreAnim;
    


    /// <summary>
    /// Enumération définissant le statut actuel du tableau de score
    /// </summary>
    public enum boardStatus {INITIAL, LEVEL, END, FINAL, LAST_LEVEL_FINAL, TUTORIAL, FREE_PLAY, FREE_PLAY_FINAL }

    [SyncVar]
    private boardStatus currentState = boardStatus.INITIAL;

    public delegate void UIBoardChange(boardStatus status);
    public static event UIBoardChange OnBoardStatusChange;

    public delegate void UIBoardGaze();
    public static event UIBoardGaze OnBoardGaze;

    

    public static event ScoreManager.balloonWasPopped OnBalloonPopped;
    

    // Use this for initialization
    void Start ()
    {
        sh = this.GetComponent<SharedElement>();
        Participant.OnBoardStatusChange += UpdateStatus;
        SceneManager.sceneLoaded += sceneChanged;
        ScoreManager.onBalloonPopped += CmdBalloonPopped;
        ScoreManager.onScoreChange += CmdUpdateScore;
        ScoreManager.onBalloonPopped += handleBalloonPop;
        LoadLevel.OnLevelEnd += levelEnd;
    }

    [Command]
    private void CmdBalloonPopped(float time, int scoreGain, bool isBonus)
    {
        RpcBalloonPopped(time, scoreGain, isBonus);
    }

    [ClientRpc]
    private void RpcBalloonPopped(float time, int scoreGain, bool isBonus)
    {
        if (OnBalloonPopped != null)
        {
            OnBalloonPopped.Invoke(time, scoreGain, isBonus);
        }
    }

    private void OnDestroy()
    {
        Participant.OnBoardStatusChange -= UpdateStatus;
        SceneManager.sceneLoaded -= sceneChanged;
        ScoreManager.onScoreChange -= CmdUpdateScore;
        ScoreManager.onBalloonPopped -= handleBalloonPop;
        LoadLevel.OnLevelEnd -= levelEnd;
    }

    [Command]
    private void CmdUpdateStatus(boardStatus status)
    {
        currentState = status;
    }

    private void UpdateStatus(boardStatus status)
    {
        if(isServer)
        {
            CmdUpdateStatus(status);
        }

        if(OnBoardStatusChange != null )
        {
            OnBoardStatusChange.Invoke(status);
        }
    }

    public void levelEnd()
    {
        if (isServer)
        {
            Invoke("levelEndNoDelay", 8f);
        }
    }

    private void levelEndNoDelay()
    {
        if (isServer)
        {
            SharingManager.getLocalPlayer().CmdUpdateStatus((currentState == boardStatus.FREE_PLAY)? boardStatus.FREE_PLAY_FINAL : boardStatus.END);
        }
    }

    public void nextPanel()
    {

        SharingManager.getLocalPlayer().CmdUpdateStatus(boardStatus.FINAL);
        
    }


    public void nextLevel()
    {
        SharingManager.getLocalPlayer().CmdNextLevel();
    }

    public void returnToMenu(string sceneName)
    {
        SharingManager.getLocalPlayer().CmdLoadScene(sceneName);
    }

    private void handleBalloonPop(float time, int scoreGain, bool isBonus)
    {
        if (!isServer)
            return;
        if(currentState == boardStatus.LEVEL)
        {
            if(!isBonus)
                balloonDisplayed++;

            return;
        }
        //DO nothing, we are not in level
    }

    [Command]
    private void CmdUpdateScore(int score,int scoreToAdd)
    {
        RpcUpdateScore(score,scoreToAdd);
    }


    [ClientRpc]
    private void RpcUpdateScore(int score, int scoreToAdd)
    {
        if(scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        if(scoreToAdd != 0)
        {

            if(scorePointAdd != null)
            {
                scorePointAdd.text = "+"+scoreToAdd.ToString();
            }

            if(scoreAnim != null)
            {
                scoreAnim.Play("MainScored");
            }
        }
    }



    private void sceneChanged(Scene scene, LoadSceneMode mode)
    {
        if (isServer)
        {
            ScoreManager.initScore();
            TimerManager.initTimer();
            //TODO : Clear Board
            boardStatus newStatus = (scene.name.StartsWith("Level"))
            ? (scene.name == "LevelBonus")
                ? boardStatus.FREE_PLAY
                : boardStatus.LEVEL
            : (scene.name == "Tutorial")
                ? boardStatus.TUTORIAL
                : boardStatus.INITIAL;
            SharingManager.getLocalPlayer().CmdUpdateStatus(newStatus);
        }
    }

    public void removeInstance()
    {
        if(sh != null)
        {
            sh.removeInstance();
        }
    }


    public void move(Vector3 position, Quaternion rotation)
    {
        if(sh != null)
        {
            sh.CmdMove(position, rotation);
        }
    }

    public void OnFocusEnter()
    {
        if(isServer && currentState == boardStatus.END)
        {
            CmdBoardGaze();
        }
      
    }

    [ClientRpc]
    private void RpcBoardGaze()
    {
        if(OnBoardGaze != null)
        {
            OnBoardGaze();
        }
    }

    [Command]
    private void CmdBoardGaze()
    {
        RpcBoardGaze();
    }

    public void OnFocusExit()
    {
        //Nothing here
    }
}
