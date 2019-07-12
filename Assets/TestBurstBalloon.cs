using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBurstBalloon : MonoBehaviour {
    public GameObject confetti;
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "VirtualHand")
        {

            
            Instantiate(confetti,transform.localPosition,Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
