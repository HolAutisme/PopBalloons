using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;


public class SceneDesignManager : MonoBehaviour
{
    public static SceneDesignManager Instance;

    public enum QueryStates
    {
        None,
        Processing,
        Finished
    }

    private struct QueryStatus
    {
        public void Reset()
        {
            State = QueryStates.None;
            Name = "";
            CountFail = 0;
            CountSuccess = 0;
            QueryResults = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult>();
            ObjectsToPlace = new List<GameObject>();
        }

        public QueryStates State;
        public string Name;
        public int CountFail;
        public int CountSuccess;
        public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult> QueryResults;
        public List<GameObject> ObjectsToPlace;
    }

    public struct PlacementQuery
    {
        public PlacementQuery(
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition,
            List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null,
            List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null,
            GameObject prefab = null)
        {
            PlacementDefinition = placementDefinition;
            PlacementRules = placementRules;
            PlacementConstraints = placementConstraints;
            Prefab = prefab;
        }

        public SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition PlacementDefinition;
        public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> PlacementRules;
        public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> PlacementConstraints;
        public GameObject Prefab;
    }

    private class PlacementResult
    {
        public PlacementResult(SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult result, GameObject objectToPlace)
        {
            Result = result;
            ObjectToPlace = objectToPlace;
        }

        public SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult Result;
        public GameObject ObjectToPlace;
    }

    public bool IsSolverInitialized { get; private set; }

    // Privates
    private List<PlacementResult> placementResults = new List<PlacementResult>();
    private QueryStatus queryStatus = new QueryStatus();

    public void ClearGeometry(bool clearAll = true)
    {
        placementResults.Clear();
        if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            SpatialUnderstandingDllObjectPlacement.Solver_RemoveAllObjects();
        }
        CustomSpatialUnderstandingState.Instance.ObjectPlacementDescription = "";
    }

    public bool PlaceObjectAsync(string placementName,
            List<PlacementQuery> placementList,
            bool clearObjectsFirst = true)
    {
        // If we already mid-query, reject the request
        if (queryStatus.State != QueryStates.None)
        {
            return false;
        }

        // Clear geo
        if (clearObjectsFirst)
        {
            ClearGeometry();
        }

        // Mark it
        queryStatus.Reset();
        queryStatus.State = QueryStates.Processing;
        queryStatus.Name = placementName;

        // Tell user we are processing
        CustomSpatialUnderstandingState.Instance.ObjectPlacementDescription = placementName + " (processing)";

        // Kick off a thread to do process the queries
#if UNITY_EDITOR || !UNITY_WSA
        new System.Threading.Thread
#else
            System.Threading.Tasks.Task.Run
#endif
            (() =>
            {
                // Go through the queries in the list
                for (int i = 0; i < placementList.Count; ++i)
                {
                    // Do the query
                    bool success = PlaceObject(
                        placementName,
                        placementList[i].PlacementDefinition,
                        placementList[i].PlacementRules,
                        placementList[i].PlacementConstraints,
                        placementList[i].Prefab,
                        clearObjectsFirst,
                        true);

                    // Mark the result
                    queryStatus.CountSuccess = success ? (queryStatus.CountSuccess + 1) : queryStatus.CountSuccess;
                    queryStatus.CountFail = !success ? (queryStatus.CountFail + 1) : queryStatus.CountFail;
                }

                // Done
                queryStatus.State = QueryStates.Finished;
            }
        )
#if UNITY_EDITOR || !UNITY_WSA
            .Start()
#endif
            ;

        return true;
    }

    private bool PlaceObject(string placementName,
        SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null,
        GameObject ObjectToPlace = null,
        bool clearObjectsFirst = true,
        bool isASync = false)
    {
        // Clear objects (if requested)
        if (!isASync && clearObjectsFirst)
        {
            ClearGeometry();
        }
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return false;
        }

        // New query
        if (SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject(
                placementName,
                SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementDefinition),
                (placementRules != null) ? placementRules.Count : 0,
                ((placementRules != null) && (placementRules.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementRules.ToArray()) : IntPtr.Zero,
                (placementConstraints != null) ? placementConstraints.Count : 0,
                ((placementConstraints != null) && (placementConstraints.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementConstraints.ToArray()) : IntPtr.Zero,
                SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()) > 0)
        {
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();
            if (!isASync)
            {
                // If not running async, we can just add the results to the draw list right now
                CustomSpatialUnderstandingState.Instance.ObjectPlacementDescription = placementName + " (1)";
                placementResults.Add(new PlacementResult(placementResult.Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult, ObjectToPlace));
            }
            else
            {
                queryStatus.QueryResults.Add(placementResult.Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult);
                queryStatus.ObjectsToPlace.Add(ObjectToPlace);
            }
            return true;
        }
        if (!isASync)
        {
            CustomSpatialUnderstandingState.Instance.ObjectPlacementDescription = placementName + " (0)";
        }
        return false;
    }

    private void ProcessPlacementResults()
    {
        // Check it
        if (queryStatus.State != QueryStates.Finished)
        {
            return;
        }
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }

        // Clear results
        ClearGeometry();

        // We will reject any above or below the ceiling/floor
        SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
        SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();

        // Copy over the results
        for (int i = 0; i < queryStatus.QueryResults.Count; ++i)
        {
            if ((queryStatus.QueryResults[i].Position.y < alignment.CeilingYValue) &&
                (queryStatus.QueryResults[i].Position.y > alignment.FloorYValue))
            {
                placementResults.Add(new PlacementResult(queryStatus.QueryResults[i].Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult, queryStatus.ObjectsToPlace[i]));
            }
        }

        // Text
        //CustomSpatialUnderstandingState.Instance.ObjectPlacementDescription = queryStatus.Name + " (" + placementResults.Count + "/" + (queryStatus.CountSuccess + queryStatus.CountFail) + ")";

        // Mark done
        queryStatus.Reset();
    }

    public bool InitializeSolver()
    {
        if (IsSolverInitialized ||
            !SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return IsSolverInitialized;
        }

        if (SpatialUnderstandingDllObjectPlacement.Solver_Init() == 1)
        {
            IsSolverInitialized = true;
        }
        return IsSolverInitialized;
    }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Can't do any of this till we're done with the scanning phase
        if (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done)
        {
            return;
        }

        // Make sure the solver has been initialized
        if (!IsSolverInitialized &&
            SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            InitializeSolver();
        }

        // Constraint queries
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {

        }

        // Handle async query results
        ProcessPlacementResults();
        
        RenderSceneObjects();
    }

    bool IsRenderRequired = true;
    private void RenderSceneObjects()
    {
        if (placementResults.Count > 0 && IsRenderRequired)
        {
            for (int i = 0; i < placementResults.Count; i++)
            {
                GameObject objectToPlace = Instantiate(placementResults[i].ObjectToPlace,
                    placementResults[i].Result.Position,
                    Quaternion.LookRotation(placementResults[i].Result.Forward, placementResults[i].Result.Up)) as GameObject;

                if (objectToPlace != null)
                {
                    objectToPlace.transform.parent = gameObject.transform;
                }
            }
            IsRenderRequired = false;
        }
    }
    
}
