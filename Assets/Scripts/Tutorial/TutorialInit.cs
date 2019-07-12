using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInit : MonoBehaviour {

    [SerializeField]
    private GameObject tutorialPrefab;
    private GameObject tutorialInstance;


    // Use this for initialization
    void Start() {
        init();
    }

    private void OnDisable()
    {
        //tutorialInstance.SetActive(false);
    }


    private void init()
    {
        tutorialInstance = Instantiate(tutorialPrefab);
    }
}
