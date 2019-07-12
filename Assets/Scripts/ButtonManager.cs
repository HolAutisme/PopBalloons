using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour {

    #region ////////////////////////////////////////////////// VARIABLES //////////////////////////////////////////////////

    public static ButtonManager instance;

    private GameObject scoreboardDisplayer;
    #endregion

    #region ////////////////////////////////////////////////// FUNCTIONS //////////////////////////////////////////////////

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void SetScoreBoardDisplayer(GameObject g)
    {
        scoreboardDisplayer = g;
    }

    public void EnableDisplayer()
    {
        if(scoreboardDisplayer != null)
        {
            scoreboardDisplayer.SetActive(true);
        }
    }

    #endregion
}
