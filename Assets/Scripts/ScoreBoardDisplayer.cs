using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardDisplayer : MonoBehaviour, IInputClickHandler
{

    private enum PositioningState {PLACING, VALIDATING}

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


    private void OnEnable()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
        if (info != null)
            info.SetActive(true);
        if (preview != null)
            preview.SetActive(true);

    }

    

    private void OnDisable()
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        pos = Vector3.zero;
        rot = Quaternion.identity;
        if(ButtonManager.instance != null)
        {
            ButtonManager.instance.SetScoreBoardDisplayer(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePreview();
        if (buttonMoveScoreBoard)
            popup.SetActive(true);
	}

    public void UpdatePreview()
    {
        RaycastHit hitInfo;
        //We check if gaze to the end point and close the area if enough landmarks marker
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f, SpatialMappingManager.Instance.LayerMask))
        {
            if (hitInfo.collider != null)
            {
                //On teste si on se trouve sur un mur ou non. Si le produit scalaire retourne une valeur proche de 0 c'est un mur.
                bool onWall = (Mathf.Abs(Vector3.Dot(hitInfo.normal, Vector3.up)) > 0.35);
                Vector3 p = (onWall)
                    ? SharingManager.getSharedCollection().InverseTransformPoint(hitInfo.point + offset)
                    : SharingManager.getSharedCollection().InverseTransformPoint(hitInfo.point);
                Quaternion r = (onWall)
                    ? Quaternion.LookRotation(SharingManager.getSharedCollection().InverseTransformDirection(hitInfo.point + offset - Camera.main.transform.position))
                    : Quaternion.LookRotation(SharingManager.getSharedCollection().InverseTransformDirection(Vector3.zero - hitInfo.normal));


                pos = Vector3.Lerp(pos, p, lerpFactor);
                rot = Quaternion.Lerp(rot, r, lerpFactor);
            }

        }
        else
        {
            pos = Vector3.Lerp(pos, Camera.main.transform.position + Camera.main.transform.forward * Vector3.Distance(pos, Camera.main.transform.position), lerpFactor);
        }

        //On a déja placé un score board, on souhaite le déplacer.
        if (preview != null)
        {
            preview.transform.position = pos;
            preview.transform.rotation = rot;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //On a déja placé un score board, on souhaite le déplacer.
        if ( sc != null)
        {
            sc.move(pos, rot);
            sc.gameObject.SetActive(true);
        }
        else
        {
            //On instancie le premier scoreBoard
            GameObject g = Instantiate(ScoreBoard, pos, rot);
            sc = g.GetComponent<ScoreBoard>();
        }

        if(popup != null)
            popup.SetActive(true);
        if(info != null)
            info.SetActive(false);
        this.gameObject.SetActive(false);
        preview.gameObject.SetActive(false);

    }
}
