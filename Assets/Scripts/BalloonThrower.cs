using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BalloonThrower : BalloonBehaviour
{
    public GameObject balloon;
    public GameObject target;
    public float force;
    public ForceMode forceMode;
    private GameObject clone;
    private Vector3 dir;
 



    // Use this for initialization
    void Start()
    {
       StartCoroutine(CreateProjectil());
    }

    IEnumerator CreateProjectil()
    {
        while (true)
        {
            clone = Instantiate(balloon, transform) as GameObject;

            if (clone.GetComponent<Rigidbody>() != null)
            {
                dir = target.transform.position - clone.transform.position;
                Vector3 randomFactor = new Vector3(0, Random.Range(2f, 2f), 0);
                clone.transform.LookAt(target.transform);
                clone.GetComponent<Rigidbody>().AddForce((dir + randomFactor) * force);
            }
            //time between every instantiate
            yield return new WaitForSeconds(1.5f);
        }

    }

    IEnumerator OnBecameInvisible()
    {
        if(SceneManager.GetActiveScene().name=="Level6") Destroy(clone);

        yield return null;
    }
    

}
