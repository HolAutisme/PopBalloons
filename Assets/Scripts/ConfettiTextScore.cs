using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiTextScore : MonoBehaviour {

    [SerializeField]
    private UnityEngine.TextMesh textScore;
    
    public void setTextScore(float score)
    {
        this.textScore.text = "+" + score.ToString();
    }
}
