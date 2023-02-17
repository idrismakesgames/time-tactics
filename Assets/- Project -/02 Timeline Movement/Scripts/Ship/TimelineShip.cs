using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
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
        // Based on frame rate per second and time to mac. calculate curve time factor
        int curveTimeFactor = Mathf.RoundToInt(TimelineController.Instance.FixedFramesPerSecond * timeToMaxSpeed);
        for (int i = 0; i < curveTimeFactor; i++)
        {
            if (i == 0)
            {
                shipActionsList.Add(new TimelineShipAction(linePath[0], 0, this));
            }
            else
            {
                // Then divide current frame to save by this factor, to give curve position
                float curvePosition = i / (float)curveTimeFactor;
                // Run this position through the animation curve;
                float finalCurvePosition = accelerationCurve.Evaluate(curvePosition);
                // Get final distance to move by multiplying this by the distance to max speed.
                float distanceMoved = finalCurvePosition * distanceToMaxSpeed;

                // TODO Deal with Curved Points on Smoothed Path
                // TODO Deal with after acceleration
                // TODO Deal with deceleration
                
                // Use final direction and move towards it
                Vector3 nextPathNode = linePath[1];
                Vector3 dir = nextPathNode - shipActionsList[i-1].Position;
                dir = Vector3.ClampMagnitude(dir, distanceMoved - shipActionsList[i - 1].DistanceFromStart);
                Vector3 newShipPosition = shipActionsList[i-1].Position + dir;
                shipActionsList.Add(new TimelineShipAction(
                    newShipPosition, distanceMoved, this));
            }
        }
        Debug.Log(shipActionsList.Count + " " + shipActionsList[^1].DistanceFromStart);
    }
    #endregion
    
    #region Events Subscribed Methods
    private void TimelineController_OnSelectedShipChange(object sender, EventArgs empty)
    {
        shipSelectionSprite.enabled = TimelineController.Instance.SelectedShip == this;
        if (linePath.Count == 0)
        {
            Vector3 position = transform.position;
            linePath.Add(new Vector3(position.x, position.y + 0.01f, -9));
            linePath.Add(new Vector3(position.x + 0.5f, position.y + 0.01f, -9));
        }
    }
    #endregion
}
