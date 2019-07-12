using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBehaviourLeve6 : MonoBehaviour {
    public delegate void destroyAction();
    public static event destroyAction OnDestroyBalloonLevel6;
    Color[] colors = { Color.green, Color.red, Color.blue, Color.cyan, Color.yellow, Color.magenta };

    private void Awake()
    {
        gameObject.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
    }
   

    IEnumerator OnBecameInvisible()
    {
        Destroy(gameObject);
       
        yield return null;
    }

    
    public void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "VirtualHand")
        {

            Destroy(gameObject);
            if (OnDestroyBalloonLevel6 != null) OnDestroyBalloonLevel6();

        }
    }
}
