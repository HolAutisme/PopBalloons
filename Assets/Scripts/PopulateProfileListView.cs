using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateProfileListView : MonoBehaviour {

    /// <summary>
    /// Load and add profiles name inside UI scrollview.
    /// </summary>
    public GameObject buttonSample;
    private GameObject newButton;
    
    // Use this for initialization
    void Start ()
    {
        PopulateListView();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void PopulateListView()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerProfile playerProfile in GameControl.control.playerProfileList)
        {
            newButton = Instantiate(buttonSample, gameObject.transform);
            newButton.SetActive(true);
            newButton.GetComponentInChildren<Text>().text = playerProfile.username;
        }
    }
}
