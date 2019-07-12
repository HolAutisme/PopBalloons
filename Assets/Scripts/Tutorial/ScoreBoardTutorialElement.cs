using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScoreBoardTutorialElement : MonoBehaviour
{



    [SerializeField]
    private List<TutorialManager.TutorialState> mask;

    private EnableButtonOnDemand[] enableList;

    private CanvasGroup c;

    // Use this for initialization
    void Start()
    {
        TutorialManager.onTutorialStateChanged += handleChange;
        if (c == null)
            c = this.GetComponent<CanvasGroup>();
        enableList = this.gameObject.GetComponentsInChildren<EnableButtonOnDemand>();
    }



    private void OnDestroy()
    {
        TutorialManager.onTutorialStateChanged -= handleChange;
    }


    private void handleChange(TutorialManager.TutorialState status)
    {
        if (mask.Contains(status))
        {
            //TODO : Animation?
            c.alpha = 1;
            c.interactable = true;
            c.blocksRaycasts = true;
            foreach (EnableButtonOnDemand en in enableList)
            {
                en.Active(true);
            }
        }
        else
        {
            //TODO : Animation?
            c.alpha = 0;
            c.interactable = false;
            c.blocksRaycasts = false;
            foreach (EnableButtonOnDemand en in enableList)
            {
                en.Active(false);
            }
        }
    }
}
