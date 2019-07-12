using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeadOverlay : NetworkBehaviour {

    /// <summary>
    /// Participant matching the overlay
    /// </summary>
    private Participant participant;


    /// <summary>
    /// Overlay prefab corresponding to child
    /// </summary>
    
    [Tooltip("Prefab correspondant à la fenêtre d'information survolant l'enfant.")]
    [SerializeField]
    private GameObject childOverlay;

    /// <summary>
    /// Overlay prefab corresponding to Doctor
    /// </summary>

    [Tooltip("Prefab correspondant à l'icone du médecin.")]
    [SerializeField]
    private GameObject doctorOverlay;


    /// <summary>
    ///  Élément instancié à partir des prefab doctorOverlay ou childOverlay.
    /// </summary>
    private GameObject overlay;

	// Use this for initialization
	void Start ()
    {
        this.participant = this.GetComponent<Participant>();

        if (this.participant.getPType() != Participant.ParticipantType.NONE)
        {
            instantiateOverlay();
        } //TODO : if NONE, we should wait and try to get a new value later. Boolean maybe, or delegate.

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// Instancie l'Overlay en fonction du type de participant.
    /// </summary>
    void instantiateOverlay()
    {
        //On ne souhaite pas afficher d'élément sur notre casque.
        if (!this.hasAuthority)
        {
            //On affiche soit une croix soit le panneau d'information de l'enfant.
            overlay = Instantiate((this.participant.getPType() == Participant.ParticipantType.CHILD) ? childOverlay : doctorOverlay, this.transform);
            DoctorUI ui = overlay.GetComponentInChildren<DoctorUI>();
            if(ui != null)
            {
                ui.setSource(participant.getHeartData());
            }
            
        }
    }
}
