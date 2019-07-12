using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JulieDemonstrate : MonoBehaviour
{
    Animator anim;

    // Use this for initialization
    void Start()
    {
        TutorialManager.onTutorialStateChanged += HandleChange;
        anim = this.GetComponent<Animator>();
    }

    private void HandleChange(TutorialManager.TutorialState ts)
    {
        if (ts == TutorialManager.TutorialState.DEMONSTRATION && anim != null)
        {
            anim.SetTrigger("Burst");
        }
    }
}
