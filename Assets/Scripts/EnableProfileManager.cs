using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableProfileManager : MonoBehaviour {

    /// <summary>
    /// This script auto enable the profile manager on start
    /// </summary>
    public GameObject profileManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update to check wich profile is active on game control script
	void Update () {
        if (GameControl.control.currentProfile != null && !profileManager.activeSelf)
            profileManager.SetActive(true);
        else if (GameControl.control.currentProfile == null && profileManager.activeSelf)
            profileManager.SetActive(false);
    }
}
