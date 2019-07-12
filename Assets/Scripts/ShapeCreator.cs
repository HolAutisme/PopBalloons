using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShapeCreator : NetworkBehaviour, IInputClickHandler
{
    public GameObject landmarkCornerPrefab;
    public GameObject landmarkLimitPrefab;
    public GameObject landmarkMarkerPrefab;
    public GameObject finalLandmarkMarkerPrefab;
    public GameObject info;
    public GameObject popup;
    public List<GameObject> landmarks;
    public Material material;

    public bool playedOnced = false;

    [SerializeField]
    private GameObject preview;
    [SerializeField]
    private GameObject CenterPrefab;
    private GameObject tmpLandmark1;
    private GameObject tmpLandmark2;
    private GameObject tmpLandmarkMarker1;
    private GameObject tmpLandmarkMarker2;
    private Vector3 mousePos;
    private RaycastHit hitInfo;
    private int clickValue = 0;
    private GameObject center;
    Vector3 instantiatePosition;
    float distance;
    float lerpValue;
    int segmentsToCreate;

    Vector3 moy;
    int nbBeacon;

    private static ShapeCreator shapeCreator;

    public static ShapeCreator Instance
    {
        get
        {
            if (shapeCreator == null)
            {
                //TODO: attribut privé 
                shapeCreator = FindObjectOfType<ShapeCreator>();
            }

            return shapeCreator;
        }
    }

    private void Awake()
    {
        landmarks = new List<GameObject>();
        InputManager.Instance.AddGlobalListener(gameObject);
        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {
        if (preview != null)
        {
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f);
            if (hitInfo.collider != null)
            {
                //TODO Change preview color to indicate that user can close aera

                if (hitInfo.collider.name.StartsWith("Landmark"))
                {
                    preview.SetActive(false);
                    return;
                }
                else
                {
                    /*
                    if (hitInfo.collider.name == "Landmark1" && tmpLandmarkMarker1 != null)
                    {
                        tmpLandmarkMarker1.SetActive(true);
                        tmpLandmarkMarker1.GetComponent<Renderer>().material.color = Color.red;
                    }
                    */
                    if (!preview.activeSelf)
                        preview.SetActive(true);
                    return;
                }
            }
        }


    }
    //Empty previous landmarks to avoid any limit area issue
    private void OnEnable()
    {
        if (!playedOnced)
        {
            //TODO : playAudioClip
            playedOnced = true;
        }

        clickValue = 0;
        if (preview != null)
            preview.SetActive(true);
        Destroy(center);
        nbBeacon = 0;
        moy = Vector3.zero;
        foreach (GameObject landmark in landmarks)
        {
            NetworkServer.UnSpawn(landmark);
            Destroy(landmark);
        }
        landmarks.Clear();
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDisable()
    {
        if (preview != null)
            preview.SetActive(false);
    }


    public Vector2[] GetLandmarksPoint()
    {
        Vector2[] landmarksPointArray = new Vector2[landmarks.Count];
        Vector2 landmarkPoint = new Vector2();
        int occ = 0;

        foreach (GameObject landmark in landmarks)
        {
            landmarkPoint.x = landmark.transform.position.x;
            landmarkPoint.y = landmark.transform.position.z;
            landmarksPointArray[occ] = landmarkPoint;
            occ++;
        }

        return landmarksPointArray;
    }

    public Vector3[] GetLandmarksPoint3D()
    {
        Vector3[] landmarksPointArray = new Vector3[landmarks.Count];
        Vector3 landmarkPoint = new Vector3();
        int occ = 0;

        foreach (GameObject landmark in landmarks)
        {
            landmarkPoint.x = landmark.transform.position.x;
            landmarkPoint.y = landmark.transform.position.y;
            landmarkPoint.z = landmark.transform.position.z;
            landmarksPointArray[occ] = landmarkPoint;
            occ++;
        }

        return landmarksPointArray;
    }

    public GameObject GetCenter()
    {
        return this.center;
        /*
        Vector3 sum = Vector3.zero;
        float nb = 0;
        foreach(Vector3 v in GetLandmarksPoint3D())
        {
            sum += v;
            nb++;
        }
        if(nb > 0)
            sum /=  nb;
        return sum;
        */
    }


    [Command]
    void CmdHandleClick(Vector3 point)
    {
        clickValue++;


        if (clickValue == 1)
        {
            //First landmark marker
            info.SetActive(false);
            tmpLandmarkMarker1 = Instantiate(finalLandmarkMarkerPrefab, SharingManager.getSharedCollection().InverseTransformPoint(point), landmarkMarkerPrefab.transform.rotation);
            tmpLandmarkMarker2 = Instantiate(landmarkMarkerPrefab, SharingManager.getSharedCollection().InverseTransformPoint(point), landmarkMarkerPrefab.transform.rotation);
            tmpLandmarkMarker1.transform.parent = gameObject.transform;
            tmpLandmarkMarker2.transform.parent = gameObject.transform;
            tmpLandmarkMarker1.gameObject.SetActive(false);
            //tmpLandmarkMarker2.gameObject.SetActive(false);
            //TODO: inverseTransformPoint
            tmpLandmark1 = Instantiate(landmarkCornerPrefab, SharingManager.getSharedCollection().InverseTransformPoint(point), Quaternion.identity);
            NetworkServer.Spawn(tmpLandmark1);
            //tmpLandmark1.transform.parent = gameObject.transform;
            landmarks.Add(tmpLandmark1);
            tmpLandmark1.name = "Landmark" + clickValue;
        }
        else
        {
            //Others landmark markers
            tmpLandmarkMarker2.transform.position = point;
            tmpLandmark2 = Instantiate(landmarkCornerPrefab, SharingManager.getSharedCollection().InverseTransformPoint(point), Quaternion.identity);
            //TODO: inverseTransformPoint
            NetworkServer.Spawn(tmpLandmark2);
            tmpLandmark2.transform.parent = gameObject.transform;
            landmarks.Add(tmpLandmark2);
            tmpLandmark2.name = "Landmark" + clickValue;


            //TODO : Handle afterWard connection
            RpcDrawLine(tmpLandmark1.transform.position, tmpLandmark2.transform.position);
            lerpValue = 0;
            tmpLandmark1 = tmpLandmark2;
            tmpLandmark2 = null;
        }
        moy += point;
        nbBeacon++;

        if (clickValue > 3)
        {
            if (tmpLandmarkMarker1)
                tmpLandmarkMarker1.SetActive(true);
        }

    }

    [ClientRpc]
    void RpcDrawLine(Vector3 pointA, Vector3 pointB)
    {
        segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(pointA, pointB) / 0.1f);
        distance = 1f / segmentsToCreate;
        for (int i = 1; i < segmentsToCreate; i++)
        {
            //We increase our lerpValue
            lerpValue += distance;
            //Get the position
            instantiatePosition = Vector3.Lerp(SharingManager.getSharedCollection().InverseTransformPoint(pointA), SharingManager.getSharedCollection().InverseTransformPoint(pointB), lerpValue);
            //Instantiate the landmarks limit
            GameObject tmpLimit = Instantiate(landmarkLimitPrefab, instantiatePosition, transform.rotation);
            tmpLimit.transform.parent = gameObject.transform;

            //if(isServer)
            //  NetworkServer.Spawn(tmpLimit);
        }
    }


    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {

        //We check if gaze to the end point and close the area if enough landmarks marker
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f);
        if (hitInfo.collider != null)
        {
            if (hitInfo.transform.name == "Landmark1" && landmarks.Count < 3)
                return;
            else if (hitInfo.transform.name == "Landmark1" && landmarks.Count >= 3)
            {
                CmdLastDraw();
                //landmarks.Clear();
                if (nbBeacon != 0)
                {
                    center = Instantiate(CenterPrefab, SharingManager.getSharedCollection().InverseTransformPoint(moy / nbBeacon), Quaternion.identity);
                    center.transform.SetParent(SharingManager.getSharedCollection().transform, false);
                }

                gameObject.GetComponent<ShapeCreator>().enabled = false;
                popup.SetActive(true);
                Vector2[] test = GetLandmarksPoint();
                return;
            }

            //Second raycast to target spatial mapping only
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20.0f, SpatialMappingManager.Instance.LayerMask);
            CmdHandleClick(hitInfo.point);

        }
    }

    [Command]
    //Last draw to the last landmark marker of the area
    private void CmdLastDraw()
    {
        tmpLandmarkMarker2.transform.position = hitInfo.collider.gameObject.transform.position;
        RpcDrawLine(tmpLandmark1.transform.position, tmpLandmarkMarker2.transform.position);
        /*
         * segmentsToCreate = Mathf.RoundToInt(Vector3.Distance(tmpLandmark1.transform.position, hitInfo.transform.position) / 0.1f);
        distance = 1f / segmentsToCreate;
        for (int i = 1; i < segmentsToCreate; i++)
        {
            lerpValue += distance;
            instantiatePosition = Vector3.Lerp(tmpLandmark1.transform.position, hitInfo.transform.position, lerpValue);
            GameObject tmpLimit = Instantiate(landmarkLimitPrefab, instantiatePosition, transform.rotation);
            tmpLimit.transform.parent = gameObject.transform;
        }
        */
        lerpValue = 0;
        tmpLandmark1 = tmpLandmark2;
        tmpLandmark2 = null;
        Destroy(tmpLandmarkMarker1);
        Destroy(tmpLandmarkMarker2);
    }
}