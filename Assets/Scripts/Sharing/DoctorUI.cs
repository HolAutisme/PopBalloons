using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorUI : MonoBehaviour {

    [SerializeField]
    private UnityEngine.UI.Text bpm;

    [SerializeField]
    private UnityEngine.UI.Text score;

    [SerializeField]
    private UnityEngine.UI.Text level;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private int mediumThreshold = 90;

    [SerializeField]
    private int highThreshold = 105;

    private HRMData source;

    bool animationPlayed = false;

    public void setSource(HRMData data)
    {
        source = data;
        if (source != null)
        {
            source.onUpdateBpm += updateBpm;
            source.onUpdateLevel += updateLevel;
            source.onUpdateScore += updateScore;
            updateBpm(data.getBPM().ToString());
        };
    }

    private void OnDestroy()
    {
       if(source != null)
        {
            source.onUpdateBpm -= updateBpm;
            source.onUpdateLevel -= updateLevel;
            source.onUpdateScore -= updateScore;
        }
    }

    public void updateBpm(string b)
    {
        
        int bpmInt = int.Parse(b);
        
        switch (bpmInt)
        {
            case 0:
                bpm.text = "BPM : Scanning devices";
                break;
            case 1:
                bpm.text = "BPM : Device not available";
                break;
            case 2:
                bpm.text = "BPM : No device detected";
                break;
            default:
                bpm.text = "BPM : " + b;

                //ANIMATION
                if (bpmInt < mediumThreshold)
                {
                    if (!animationPlayed)
                    {
                        animator.Play("BPMnormal");
                        animationPlayed = true;
                    }
                    //animator.Play("BPMnormal");
                    animator.ResetTrigger("NormalToMedium");
                    animator.ResetTrigger("HighToMedium");
                    animator.ResetTrigger("MediumToHigh");
                    animator.ResetTrigger("NormalToHigh");
                    animator.ResetTrigger("MediumToNormal");
                    animator.ResetTrigger("HighToNormal");

                    animator.SetTrigger("MediumToNormal");
                    animator.SetTrigger("HighToNormal");
                }
                else if(bpmInt > mediumThreshold && bpmInt < highThreshold)
                {
                    if (!animationPlayed)
                    {
                        animator.Play("BPMmedium");
                        animationPlayed = true;
                    }
                    
                    animator.ResetTrigger("NormalToMedium");
                    animator.ResetTrigger("HighToMedium");
                    animator.ResetTrigger("MediumToHigh");
                    animator.ResetTrigger("NormalToHigh");
                    animator.ResetTrigger("MediumToNormal");
                    animator.ResetTrigger("HighToNormal");

                    animator.SetTrigger("NormalToMedium");
                    animator.SetTrigger("HighToMedium");
                }
                else if (bpmInt > highThreshold)
                {
                    if (!animationPlayed)
                    {
                        animator.Play("BPMhigh");
                        animationPlayed = true;
                    }
                    animator.ResetTrigger("NormalToMedium");
                    animator.ResetTrigger("HighToMedium");
                    animator.ResetTrigger("MediumToHigh");
                    animator.ResetTrigger("NormalToHigh");
                    animator.ResetTrigger("MediumToNormal");
                    animator.ResetTrigger("HighToNormal");

                    animator.SetTrigger("MediumToHigh");
                    animator.SetTrigger("NormalToHigh");
                }
                break;
        }
    }

    public void updateScore(string sc)
    {
        if(score != null )
            score.text =sc;
    }
    public void updateLevel(string l)
    {
        level.text = l;
    }


}
