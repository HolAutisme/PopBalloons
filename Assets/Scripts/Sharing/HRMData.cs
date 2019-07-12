using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handle HRM Data send by the band
/// TODO:  Need the band to update data.
/// TODO:  Should Be static, we have only one instance of HRM Data.
/// </summary>
public class HRMData : NetworkBehaviour {


    public delegate void updateData(string s);
    public event updateData onUpdateScore;
    public event updateData onUpdateBpm;
    public event updateData onUpdateLevel;


    /// <summary>
    /// Score partagé sur le réseau. Devrait être le même que la variable static (qui ne devrait pas exister...) car SyncVar ne peut être static
    /// </summary>
    [SyncVar]
    public int sharedScore = 0;


    [SyncVar]
    public bool dataChanged = false;


    [SyncVar]
    private int bpm = 80;

    /// <summary>
    /// Retourne the heart beat per minute of the Child.
    /// </summary>
    /// <returns></returns>
	public float getBPM()
    {
        return this.bpm;
    }

    public int getScore()
    {
        return this.sharedScore;
    }


    private void Start()
    {
        if (isLocalPlayer)
        {
            //TODO: Régler le potentiel problème du chargement de scene.
            ScoreManager.onScoreChange += CmdUpdateScore;
        }
    }

    [ClientRpc]
    public void RpcUpdateScore(int score)
    {
        if (onUpdateScore != null)
        {
            onUpdateScore("Score : " + score.ToString());
        }
    }



    [Command]
    private void CmdUpdateScore(int score, int scoreToAdd)
    {
        if (score != this.sharedScore)
        {
            this.sharedScore = score;
            RpcUpdateScore(score);
        }
    }

    [Command]
    public void CmdUpdateBPM(int _bpm)
    {
        if (_bpm != this.bpm)
        {
            this.bpm = _bpm;
            RpcUpdateBPM(_bpm);
        }
    }

    [ClientRpc]
    public void RpcUpdateBPM(int _bpm)
    {
        if (onUpdateBpm != null)
        {
             onUpdateBpm(_bpm.ToString());   
        }
    }

}
