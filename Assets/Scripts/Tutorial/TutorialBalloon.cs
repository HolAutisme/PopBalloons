using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TutorialBalloon : NetworkBehaviour {

    public delegate void TutorialBalloonPopped();
    public static TutorialBalloonPopped onTutorialBalloonPopped;

    [SerializeField]
    private GameObject particleBurst;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "VirtualHand" || collision.gameObject.tag == "MainCamera")
        {
            if (onTutorialBalloonPopped != null) onTutorialBalloonPopped();// Détruit les objets et fait apparaitre des particules
            CmdPopIt();
        }
    }


    [Command]
    public void CmdPopIt()
    {
        //gameObject.GetComponent<SoundManager>().PlayPop();
        GameObject particleBurstClone = Instantiate(particleBurst, SharingManager.getSharedCollection().InverseTransformPoint(gameObject.transform.position), Quaternion.identity);
        //particleBurstClone.transform.parent = sharedWorldAnchorTransform;
        particleBurstClone.GetComponent<SoundManager>().PlayPopAndConfetti();
        //Récupérer score et affecter au particle

        NetworkServer.Spawn(particleBurstClone);
        Destroy(particleBurstClone, 3.0f);

        NetworkServer.Destroy(this.gameObject);
    }

    public void Start()
    {
        BalloonBehaviour.RaiseBalloonSpawn(0, this.transform.position);
    }
}
