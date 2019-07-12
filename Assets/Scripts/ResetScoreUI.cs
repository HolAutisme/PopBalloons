using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScoreUI : MonoBehaviour {

    /// <summary>
    /// Reset Score UI in game.
    /// </summary>
    public List<GameObject> levelsBtn;
    // Use this for initialization
    
    public void ResetScore()
    {
        foreach (GameObject btn in levelsBtn)
        {
            btn.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
