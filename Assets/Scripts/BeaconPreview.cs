using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconPreview : MonoBehaviour
{

    public enum orientationMode
    {
        Fixed,
        Free,
        Y_Axis,
    }

    public bool followEnable;
    public bool scalingByDistance;
    public orientationMode orientation = orientationMode.Y_Axis;
    private RaycastHit _hitInfo;

    private const float _lerpingCoef = 0.5f;

    // Use this for initialization
    void Start()
    {
        if (SpatialMappingManager.Instance == null)
        {
            Debug.LogError("This script expects that you have a SpatialMappingManager component in your scene.");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (followEnable)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hitInfo, 30.0f, SpatialMappingManager.Instance.LayerMask))
            {
                transform.position = Vector3.Lerp(transform.position, _hitInfo.point, _lerpingCoef);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.forward * Vector3.Distance(transform.position, Camera.main.transform.position), _lerpingCoef);
            }
            switch (orientation)
            {
                case orientationMode.Free:
                    transform.LookAt(Camera.main.transform.position);
                    break;
                case orientationMode.Y_Axis:
                    transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
                    break;
            }
            if (scalingByDistance)
            {
                transform.localScale = Vector3.one * Vector3.Distance(Camera.main.transform.position, transform.position) / 5;
            }
        }

    }

    public void SetFollowEnable(bool state)
    {
        followEnable = state;
    }
}
