using System;
using UnityEngine;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine.Rendering.UI;
using Vector3 = UnityEngine.Vector3;

public class TimelineShip : MonoBehaviour
{
    #region Editable Variables
    [SerializeField] private SpriteRenderer shipSelectionSprite;
    [SerializeField] private PivotPoint pivotObject;
    [SerializeField] private float distanceToMaxSpeed;
    [SerializeField] private float timeToMaxSpeed;
    [SerializeField] private AnimationCurve accelerationCurve;
    #endregion 
    
    #region  Variables
    private List<Vector3> linePath; // Path for ship to follow
    private List<Vector3> linePathSmoothed; // Path for ship to follow smoothed
    private LineRenderer lineRenderer; // Line that will show on screen
    
    private List<TimelineShipAction> shipActionsList; // Path for ship to follow smoothed
    #endregion
    
    #region Lifecycle Methods
    private void Start()
    {
        TimelineController.Instance.OnSelectedShipChange += TimelineController_OnSelectedShipChange;
        linePath = new List<Vector3>();
        shipActionsList = new List<TimelineShipAction>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TimelineController.Instance.SelectedShip == this && !TimelineController.Instance.HoveredShip && !TimelineController.Instance.HoveredPivot)
            {
                AddToPath();
                BuildShipActionList();
            }
        }
    }

    #endregion
    
    #region TimelineShip Path Methods
    private void AddToPath()
    {
        linePath.Add(TimelineController.Instance.MousePosition);
        PivotPoint pivotPointInstance = Instantiate(pivotObject, TimelineController.Instance.MousePosition, quaternion.identity);
        pivotPointInstance.SetStartingValues(this, linePath.Count-1, linePath);
        FeedPathToLineRenderer();
    }
    private void FeedPathToLineRenderer()
    {
        // Use smoothing from A* to then create curvature
        linePathSmoothed = TimelineHelperMethods.Instance.LineSmoothBezier(linePath);
            
        // Pass these points to line renderer.
        lineRenderer.positionCount = linePathSmoothed.Count;
        lineRenderer.SetPositions(linePathSmoothed.ToArray());
    }

    public void UpdateShipLinePath(List<Vector3> newLinePath)
    {
        linePath = newLinePath;
        FeedPathToLineRenderer();
    }
    #endregion
    
    #region TimelineShip Action Methods
    private void BuildShipActionList()
    {
        // TODO create Timeline Array frame by frame based on path.
        
        // TODO loop through each linePath entry
        
        // Example of frame 1
        // Based on frame rate per second and time to mac. calculate curve time factor
        float curveTimeFactor = TimelineController.Instance.FixedFramesPerSecond * timeToMaxSpeed;
        // Then divide current frame to save by this factor, to give curve position
        float curvePosition = 2 / curveTimeFactor;
        // Run this position through the animation curve;
        float finalCurvePosition = accelerationCurve.Evaluate(curvePosition);
        // Get final distance to move by multiplying this by the distance to max speed.
        float distanceMovedOnThisFrame = finalCurvePosition * distanceToMaxSpeed;
       

        float minDistance = 99;
        for (int i = 1; i < linePathSmoothed.Count; i++)
        {
            float distanceBetweenThisAndLast = Vector3.Distance(linePathSmoothed[i], linePathSmoothed[i - 1]);
            if (distanceBetweenThisAndLast < minDistance)
                minDistance = distanceBetweenThisAndLast;
        }
        
        // TODO go through smooth path and move towards each point per frame based on distance,
        // TODO THEN check that if a point if lower than distance, average position out between 2/3/4 points
        // TODO FINALLY and move towards that position 

    }
    #endregion
    
    #region Events Subscribed Methods
    private void TimelineController_OnSelectedShipChange(object sender, EventArgs empty)
    {
        shipSelectionSprite.enabled = TimelineController.Instance.SelectedShip == this;
        if (linePath.Count == 0)
        {
            Vector3 position = transform.position;
            linePath.Add(new Vector3(position.x + 0.5f, position.y + 0.01f, -9));
        }
    }
    #endregion
}
