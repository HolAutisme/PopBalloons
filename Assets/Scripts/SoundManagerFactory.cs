using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerFactory : MonoBehaviour {


    static SoundManagerFactory instance;

    [SerializeField]
    private SoundManager source;


    private static Stack<SoundManager> pool;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public static SoundManager getSource()
    {
        if (pool == null)
            pool = new Stack<SoundManager>();

        if (pool.Count == 0)
        {
            return Instantiate(instance.source);
        }
        else
        {
            return pool.Pop();
        }  
    }

    public static void returnSource(SoundManager s)
    {
        if(s != null)
        {
            if (pool == null)
                pool = new Stack<SoundManager>();
            s.StopAudio();
            s.gameObject.SetActive(false);
            pool.Push(s);
        }
    }


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
