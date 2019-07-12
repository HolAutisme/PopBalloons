using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShapeFinder : Singleton<ShapeFinder>
{

    public bool HasCreatedShapes { get; private set; }

    private List<string> customShapeDefinitions = new List<string>();
    // Use this for initialization
    void Start()
    {
        if (SpatialUnderstanding.Instance != null)
        {
            SpatialUnderstanding.Instance.OnScanDone += FindShapes;
        }
    }

    /// <summary>
    /// Create shape definitions, add them and analyze the definitions to get the results to query later
    /// </summary>
    private void FindShapes()
    {
        if (HasCreatedShapes ||
                !SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }
        CreateShapeDefinitions();
        SpatialUnderstandingDllShapes.ActivateShapeAnalysis();
    }

    /// <summary>
    /// Creates definition for pre determined set of shapes
    /// </summary>
    private void CreateShapeDefinitions()
    {
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }

        List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponents;
        List<SpatialUnderstandingDllShapes.ShapeConstraint> shapeConstraints;

        //Platform
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.2f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.035f),
                }),
        };

        AddShape("Platform", shapeComponents);

        // Chair
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.2f, 0.7f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.035f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.4f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.1f, 0.7f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Between(0.1f, 0.6f),
                }),
        };
        AddShape("Chair", shapeComponents);

        // "Couch"
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    // Seat
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.2f, 0.7f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.3f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.4f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.4f, 3.0f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.2f),
                }),
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    // Back
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.2f, 1.0f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(0.3f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.4f, 3.0f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.05f),
                }),
        };
        shapeConstraints = new List<SpatialUnderstandingDllShapes.ShapeConstraint>()
        {
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesSameLength(0, 1, 0.5f),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesParallel(0, 1),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_RectanglesAligned(0, 1, 0.4f),
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_AtBackOf(1, 0),
        };
        AddShape("Couch", shapeComponents, shapeConstraints);

        // EmptyTable
        shapeComponents = new List<SpatialUnderstandingDllShapes.ShapeComponent>()
        {
            new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.3f, 0.75f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.35f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Min(0.75f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Min(0.5f),
                }),
        };
        shapeConstraints = new List<SpatialUnderstandingDllShapes.ShapeConstraint>()
        {
            SpatialUnderstandingDllShapes.ShapeConstraint.Create_NoOtherSurface(),
        };
        AddShape("EmptyTable", shapeComponents, shapeConstraints);

        // Mark it
        HasCreatedShapes = true;
    }

    private bool AddShape(string shapeName, List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponents)
    {
        return AddShape(shapeName, shapeComponents, null);
    }

    private bool AddShape(
        string shapeName,
        List<SpatialUnderstandingDllShapes.ShapeComponent> shapeComponents,
        List<SpatialUnderstandingDllShapes.ShapeConstraint> shapeConstraints = null)
    {
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return false;
        }
        IntPtr shapeComponentsPtr = (shapeComponents == null) ? IntPtr.Zero : SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeComponents.ToArray());
        IntPtr shapeConstraintsPtr = (shapeConstraints == null) ? IntPtr.Zero : SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeConstraints.ToArray());

        if (SpatialUnderstandingDllShapes.AddShape(
                    shapeName,
                    (shapeComponents == null) ? 0 : shapeComponents.Count,
                    shapeComponentsPtr,
                    (shapeConstraints == null) ? 0 : shapeConstraints.Count,
                    shapeConstraintsPtr) == 0)
        {
            Debug.LogError("Failed to create custom shape description");
            return false;
        }
        customShapeDefinitions.Add(shapeName);
        return true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SpatialUnderstanding.Instance != null)
        {
            SpatialUnderstanding.Instance.OnScanDone -= FindShapes;
        }
    }
}
