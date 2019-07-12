using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Classe orientant un hologramme dans la direction du player local.
/// </summary>
public class LookAtLocalPlayer : MonoBehaviour {

    /// <summary>
    /// Local player transform reference, use for lookAt action.
    /// </summary>
    private Transform target;

    [SerializeField]
    private Vector3 offset;


    private void Awake()
    {
       Participant pLocal = SharingManager.getLocalPlayer();
       if (pLocal == null)
       {
            SharingManager.OnLocalPlayerSet += waitForLocalPlayer;
       }
       else
       {
            target = pLocal.GetComponent<Transform>();
            //transform.localPosition += offset;
        }
    }

    private void LateUpdate()
    {
        if(target != null && this.transform.parent != null)
        {
            // On applique le lookAt après toute les autres mise à jours.
            this.transform.LookAt(target, Vector3.up);
            this.transform.position = this.transform.parent.position + offset;

        }

    }

    private void waitForLocalPlayer()
    {
        Participant pLocal = SharingManager.getLocalPlayer();
        if(pLocal != null)
        {
            target = pLocal.GetComponent<Transform>();
            
        }
    }


}
