using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DestroyOnSceneChange : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if(isServer)
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += disposeElement;
	}

    
    private void disposeElement(Scene sc, LoadSceneMode mode)
    {
        if(isServer)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= disposeElement;
    }
}
