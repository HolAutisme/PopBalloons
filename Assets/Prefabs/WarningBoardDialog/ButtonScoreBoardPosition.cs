using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScoreBoardPosition : MonoBehaviour {

    private enum PositioningState { PLACING, VALIDATING }

    [SerializeField]
    [Tooltip("ScoreBoard prefab to instantiate if needs")]
    private GameObject ScoreBoard;

    private ScoreBoard sc;

    [SerializeField]
    [Tooltip("Preview prefab to instantiate if needs")]
    private GameObject preview;
    [SerializeField]
    [Tooltip("Validation menu to fix score board position")]
    private GameObject popup;

    [SerializeField]
    [Tooltip("Text indicating what to do.")]
    private GameObject info;

    [SerializeField]
    private Vector3 offset;

    [Range(0.1f, 1f)]
    [SerializeField]
    private float lerpFactor = 0.5f;
    private Vector3 pos;
    private Quaternion rot;

    public bool buttonMoveScoreBoard = false;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (buttonMoveScoreBoard)
            popup.SetActive(true);
    }

}
