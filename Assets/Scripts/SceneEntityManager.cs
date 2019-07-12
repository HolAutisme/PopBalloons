using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityManager : Singleton<SceneEntityManager>
{
    [Tooltip("Defines the gameobjects to be placed on the scene with it's definitions, rules and constraints")]
    public EntityPlacementDefinition[] ObjectPlacementDefinitions;
    private List<SceneDesignManager.PlacementQuery> placementQueries;

    [Tooltip("Does scene need to be arranged")]
    public bool IsSceneArrangementRequired = true;
    // Use this for initialization
    void Start()
    {
        placementQueries = new List<SceneDesignManager.PlacementQuery>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneDesignManager.Instance.IsSolverInitialized && IsSceneArrangementRequired)
        {
            PlaceObjects(ObjectPlacementDefinitions);
            IsSceneArrangementRequired = false;
        }
    }

    SceneDesignManager.PlacementQuery CreatePlacementQueries(EntityPlacementDefinition ObjectPlacementDefinition)
    {
        Vector3 halfDimention = (ObjectPlacementDefinition.SceneContentPrefab.GetComponentInChildren<Renderer>().bounds.size) * 0.5f;

        SceneDesignManager.PlacementQuery query = new SceneDesignManager.PlacementQuery();
        query.Prefab = ObjectPlacementDefinition.SceneContentPrefab;

        query.PlacementDefinition = CreateObjectPlacementDefinition(ObjectPlacementDefinition.PlacementType, halfDimention, ObjectPlacementDefinition.Shape);

        query.PlacementRules = CreateObjectPlacementRule(ObjectPlacementDefinition.PlacementRules, ObjectPlacementDefinition.MinDistance, ObjectPlacementDefinition.Position);

        query.PlacementConstraints = CreateObjectPlacementConstraint(ObjectPlacementDefinition.PlacementConstraints, 
            ObjectPlacementDefinition.MinDistance, 
            ObjectPlacementDefinition.MaxDistance, 
            ObjectPlacementDefinition.Position);

        return query;
    }

    SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition CreateObjectPlacementDefinition(
        SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType PlacementType, Vector3 halfDimention, Shapes Shape)
    {
        switch (PlacementType)
        {
            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_OnFloor:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_OnWall:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(halfDimention, halfDimention.y, halfDimention.y * 10f);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_OnCeiling:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnCeiling(halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_OnEdge:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnEdge(halfDimention, halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_OnFloorAndCeiling:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloorAndCeiling(halfDimention, halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_RandomInAir:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_RandomInAir(halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_InMidAir:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_InMidAir(halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_UnderPlatformEdge:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_UnderPlatformEdge(halfDimention);

            case SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType.Place_OnShape:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnShape(halfDimention, Shape.ToString(), 0);

            default:
                return SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(halfDimention);
        }
    }

    List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> CreateObjectPlacementRule(
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.ObjectPlacementRuleType> ObjectPlacementRuleTypes, float MinDistance, Vector3 Position)
    {
        if (ObjectPlacementRuleTypes != null && ObjectPlacementRuleTypes.Count > 0)
        {
            List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> rules = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>();
            for (int i = 0; i < ObjectPlacementRuleTypes.Count; i++)
            {
                switch (ObjectPlacementRuleTypes[i])
                {
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.ObjectPlacementRuleType.Rule_AwayFromPosition:
                        rules.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromPosition(Position, MinDistance));
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.ObjectPlacementRuleType.Rule_AwayFromWalls:
                        rules.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromWalls(MinDistance));
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.ObjectPlacementRuleType.Rule_AwayFromOtherObjects:
                    default:
                        rules.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(MinDistance));
                        break;
                }
            }
            return rules;
        }
        return null;
    }

    List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> CreateObjectPlacementConstraint(
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType> ObjectPlacementConstraintTypes,
        float MinDistance,
        float MaxDistance,
        Vector3 Position)
    {
        if (ObjectPlacementConstraintTypes != null && ObjectPlacementConstraintTypes.Count > 0)
        {
            List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> constraints = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint>();
            for (int i = 0; i < ObjectPlacementConstraintTypes.Count; i++)
            {
                switch (ObjectPlacementConstraintTypes[i])
                {
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType.Constraint_NearPoint:
                        constraints.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_NearPoint(Position, MinDistance, MaxDistance));
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType.Constraint_NearWall:
                        constraints.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_NearWall(MinDistance, MaxDistance));
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType.Constraint_AwayFromWalls:
                        constraints.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_AwayFromWalls());
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType.Constraint_NearCenter:
                        constraints.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_NearCenter(MinDistance, MaxDistance));
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType.Constraint_AwayFromPoint:
                        constraints.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_AwayFromPoint(Position));
                        break;
                    case SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType.Constraint_AwayFromOtherObjects:
                    default:
                        constraints.Add(SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_AwayFromOtherObjects());
                        break;
                }
            }
            return constraints;
        }
        return null;
    }

    public void PlaceObjects(EntityPlacementDefinition[] ObjectPlacementDefinitions, bool clearCurrentScene = true)
    {
        for (int i = 0; i < ObjectPlacementDefinitions.Length; i++)
        {
            placementQueries.Add(CreatePlacementQueries(ObjectPlacementDefinitions[i]));
        }
        // need to place objects
        SceneDesignManager.Instance.PlaceObjectAsync("SceneDesign", placementQueries, clearCurrentScene);
    }

    /// <summary>
    /// Remove all objects from the scene and reposition it (random positioning) 
    /// </summary>
    /// <param name="ObjectPlacementDefinitions"></param>
    /// <param name="clearCurrentScene"></param>
    public void RefreshScene(EntityPlacementDefinition[] ObjectPlacementDefinitions = null, bool clearCurrentScene = true)
    {
        if (ObjectPlacementDefinitions != null)
        {
            placementQueries.Clear();
            PlaceObjects(ObjectPlacementDefinitions, clearCurrentScene);
        }
        else
        {
            SceneDesignManager.Instance.PlaceObjectAsync("SceneDesign", placementQueries, true);
        }

    }
}

/// <summary>
/// Defines the placement for an entity in the scene
/// </summary>
[Serializable]
public class EntityPlacementDefinition
{
    [Tooltip("The gameobject that needs to be placed in the scene")]
    public GameObject SceneContentPrefab;
    [Tooltip("Specifies where to place the gameobject")]
    public SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.PlacementType PlacementType;
    [Tooltip("The Shape to place the gameobject on. Required if placement type for this gameobject is onshape ")]
    public Shapes Shape;
    [Tooltip("Specifies rules for gameobject placement")]
    public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.ObjectPlacementRuleType> PlacementRules;
    [Tooltip("Position to keep away from. Required if rule is AwayFromPosition")]
    public Vector3 Position;
    [Tooltip("Minimum distance to be keep from other entities in meter.")]
    public float MinDistance;
    [Tooltip("Maximum distance to be keep from other entities in meter.")]
    public float MaxDistance;
    [Tooltip("Specifies constraints for gameobject placement")]
    public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.ObjectPlacementConstraintType> PlacementConstraints;
}

public enum Shapes
{
    Platform,
    Chair,
    Couch,
    EmptyTable
}