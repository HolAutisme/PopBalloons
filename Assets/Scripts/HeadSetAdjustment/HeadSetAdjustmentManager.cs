using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSetAdjustmentManager : MonoBehaviour
{

    public static string nextScene;
    public static bool alreadySetUp = false;

    static HeadSetAdjustmentManager instance;

    public void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            instance = this;
        }
    }

}
