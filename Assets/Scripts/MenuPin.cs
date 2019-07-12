using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPin : MonoBehaviour {

    [SerializeField]
    Animator animator;

    public void toggleAnimation(bool b)
    {
        if (animator != null)
            animator.SetBool("Bool", b);
    }
}
