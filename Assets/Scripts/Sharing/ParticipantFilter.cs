using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Classe qui désacitve des éléments de la scene si le participant n'est pas du type selectionné.
/// </summary>
public class ParticipantFilter : MonoBehaviour {



    [SerializeField]
    private bool activeOnServerStart = false;

    [SerializeField]
    private GameObject objectToFilter;

    [SerializeField]
    private Participant.ParticipantType t;
    
    // Use this for initialization
	void Start ()
    {

        if(SharingManager.getLocalPlayer() != null)
        {
            checkIfOK();
        }
        else
        {
            SharingManager.OnLocalPlayerSet += checkIfOK;
        }

	}
	

    private void checkIfOK()
    {
        if (SharingManager.getLocalPlayer().getPType() != t)
        {
            this.gameObject.SetActive(false);
        }
        else if (activeOnServerStart && objectToFilter != null)
        {
            objectToFilter.SetActive(true);
        }
    }
}
