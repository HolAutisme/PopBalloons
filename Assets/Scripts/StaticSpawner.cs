using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticSpawner : MonoBehaviour
{

    public int score=0;
    public int maxBalloons;
    //public bool create;
    public float minPosition;
    public float maxPosition;
    public List <GameObject> balloons= new List<GameObject>();
    public GameObject balloon;
    GameObject clone;
    Quaternion rot;
    //public GameObject particles;
    public bool TooMuch
    {
        get
        {
            return balloons.Count >= maxBalloons;
        }
    }

    // Use this for initialization
    void Start()
    {
        
        StartCoroutine(LoopSpawn());
    }

    IEnumerator LoopSpawn()
    {
      
        while (true)
        {
            while (!TooMuch)
            {
                
                yield return CreateBalloons();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CreateBalloons()
    {
        rot = Quaternion.Euler(-90, -90, 0);
        balloons.Add(Instantiate(balloon, new Vector3(Random.Range(minPosition, maxPosition), Camera.main.transform.position.y, Random.Range(minPosition, maxPosition)), rot));


            balloon.GetComponentInChildren<BalloonBehaviour>().shouldFloat = true;
            if (SceneManager.GetActiveScene().name == "Level1" || SceneManager.GetActiveScene().name == "Level2") balloon.GetComponentInChildren<BalloonBehaviour>().frequency = 0;
            else balloon.GetComponentInChildren<BalloonBehaviour>().frequency = 0.5f;
           
        yield return null;
    }

    
}
