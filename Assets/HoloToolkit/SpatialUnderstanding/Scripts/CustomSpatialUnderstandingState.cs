using System;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using ProgressBar;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CustomSpatialUnderstandingState : Singleton<CustomSpatialUnderstandingState>, IInputClickHandler, ISourceStateHandler
{
    public float MinAreaForStats = 5.0f;
    public float MinAreaForComplete = 50.0f;
    public float MinHorizAreaForComplete = 25.0f;
    public float MinWallAreaForComplete = 10.0f;

    private uint trackedHandsCount = 0;
    [SerializeField]
    private ProgressRadialBehaviour m_progressBar;
    [SerializeField]
    private TextMesh DebugDisplay;
    [SerializeField]
    private TextMesh DebugSubDisplay;
    [SerializeField]
    private GameObject SpatialUnderstandingInstance;
    [SerializeField]
    private GameObject infoUI;
    [SerializeField]
    private GameObject limitZoneDrawer;

    private bool _triggered;
    public bool HideText = false;

    private bool ready = false;

    private List<GameObject> floors = new List<GameObject>();

    private string _spaceQueryDescription;
    private string _objectPlacementDescription;

    public string SpaceQueryDescription
    {
        get
        {
            return _spaceQueryDescription;
        }
        set
        {
            _spaceQueryDescription = value;
            _objectPlacementDescription = "";
        }
    }

    public string ObjectPlacementDescription
    {
        get
        {
            return _objectPlacementDescription;
        }
        set
        {
            _objectPlacementDescription = value;
            _spaceQueryDescription = "";
        }
    }

    public bool DoesScanMeetMinBarForCompletion
    {
        get
        {
            // Only allow this when we are actually scanning
            if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) ||
                (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                return false;
            }

            // Query the current playspace stats
            IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
            if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
            {
                return false;
            }
            SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
       
            int totalPercentage = Mathf.Clamp((int)(stats.TotalSurfaceArea / MinAreaForComplete * 100), 0, 100);
            int horizPercentage = Mathf.Clamp((int)(stats.HorizSurfaceArea / MinHorizAreaForComplete * 100), 0, 100);
            int wallPercentage = Mathf.Clamp((int)(stats.WallSurfaceArea / MinWallAreaForComplete * 100), 0, 100);

            m_progressBar.Value = (totalPercentage + horizPercentage + wallPercentage) / 3;
            // Check our preset requirements
            if ((stats.TotalSurfaceArea > MinAreaForComplete) &&
                (stats.HorizSurfaceArea > MinHorizAreaForComplete) &&
                (stats.WallSurfaceArea > MinWallAreaForComplete))
            {
                return true;
            }
            return false;
        }
    }

    public string PrimaryText
    {
        get
        {
            if (HideText)
                return string.Empty;

            // Display the space and object query results (has priority)
            if (!string.IsNullOrEmpty(SpaceQueryDescription))
            {
                return SpaceQueryDescription;
            }
            else if (!string.IsNullOrEmpty(ObjectPlacementDescription))
            {
                return ObjectPlacementDescription;
            }

            // Scan state
            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                switch (SpatialUnderstanding.Instance.ScanState)
                {
                    case SpatialUnderstanding.ScanStates.Scanning:
                        // Get the scan stats
                        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                        {
                            return "playspace stats query failed";
                        }

                        // The stats tell us if we could potentially finish
                        if (DoesScanMeetMinBarForCompletion)
                        {
                            return "Effectuez un Air Tap pour finaliser le scan.";
                        }
                        return "Scannez la pièce.";
                    case SpatialUnderstanding.ScanStates.Finishing:
                        return "Finalisation du scan en cours...";
                    case SpatialUnderstanding.ScanStates.Done:
                        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                        // TODO Enable to get limit zone and props 
                        infoUI.SetActive(true);
                        limitZoneDrawer.SetActive(true);
                        gameObject.SetActive(false);
                        return "Scan validé.";
                    default:
                        return "ScanState = " + SpatialUnderstanding.Instance.ScanState;
                }
            }
            return string.Empty;
        }
    }

    public Color PrimaryColor
    {
        get
        {
            ready = DoesScanMeetMinBarForCompletion;
            if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
            {
                if (trackedHandsCount > 0)
                {
                    return ready ? Color.green : Color.red;
                }
                return ready ? Color.yellow : Color.white;
            }

            // If we're looking at the menu, fade it out
            float alpha = 1.0f;

            // Special case processing & 
            return (!string.IsNullOrEmpty(SpaceQueryDescription) || !string.IsNullOrEmpty(ObjectPlacementDescription)) ?
                (PrimaryText.Contains("Traitement en cours") ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : new Color(1.0f, 0.7f, 0.1f, alpha)) :
                new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    public string DetailsText
    {
        get
        {
            if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.None)
            {
                return "";
            }

            // Scanning stats get second priority
            if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) &&
                (SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                {
                    return "Playspace stats query failed";
                }
                SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

                // Start showing the stats when they are no longer zero
                if (stats.TotalSurfaceArea > MinAreaForStats)
                {
                    SpatialMappingManager.Instance.DrawVisualMeshes = false;
                    string subDisplayText = string.Format("Sols: {0}/{1}   Murs: {2}/{3}   Surface totale: {4}/{5}",
                        (int)stats.HorizSurfaceArea, (int)MinHorizAreaForComplete,
                        (int)stats.WallSurfaceArea, (int)MinWallAreaForComplete,
                        (int)stats.TotalSurfaceArea, (int)MinAreaForComplete);
                    return subDisplayText;
                }
                return "";
            }
            return "";
        }
    }

    private void Update_DebugDisplay()
    {
        // Basic checks
        if (DebugDisplay == null)
        {
            return;
        }

        // Update display text
        DebugDisplay.text = PrimaryText;
        DebugDisplay.color = PrimaryColor;
        DebugSubDisplay.text = DetailsText;
    }

    private void Start()
    {
        InputManager.Instance.PushFallbackInputHandler(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        // Updates
        Update_DebugDisplay();

        if (!_triggered && SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {
            _triggered = true;
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (ready &&
            (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) &&
            !SpatialUnderstanding.Instance.ScanStatsReportStillWorking)
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
        }
    }

    void ISourceStateHandler.OnSourceDetected(SourceStateEventData eventData)
    {
        trackedHandsCount++;
    }

    void ISourceStateHandler.OnSourceLost(SourceStateEventData eventData)
    {
        trackedHandsCount--;
    }
}