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
        int currentWaypoint = 0;
        for (int i = 0; i < curveTimeFactor; i++)
        {
            if (i == 0)
            {
                shipActionsList.Add(new TimelineShipAction(linePathSmoothed[currentWaypoint], 0, this));
                currentWaypoint++;
            }
            else
            {
                // GET DISTANCE TRAVELLED FOR FRAME
                // Then divide current frame to save by this factor, to give curve position
                float curvePosition = i / (float)curveTimeFactor;
                // Run this position through the animation curve;
                float finalCurvePosition = accelerationCurve.Evaluate(curvePosition);
                // Get final distance to move by multiplying this by the distance to max speed.
                float distanceMoved = finalCurvePosition * distanceToMaxSpeed;
                
                // WORK OUT DIFFERENCE AND CHECK CURVED AND WAYPOINTS
                // Get distance that will be travelled in this frame.
                float distanceMovedThisFrame = distanceMoved - shipActionsList[i - 1].DistanceFromStart;
                // Get distance between last frame position and current waypoint
                float distanceToNextWaypoint =
                    Vector3.Distance(shipActionsList[i - 1].Position, linePathSmoothed[currentWaypoint]);

                // LOOP TO MAKE SURE If WAYPOINT IS BETWEEN MOVEMENT FRAMES
                // For now store new position as last, then go into loop to add to it.
                Vector3 thisFrameShipPosition = shipActionsList[i - 1].Position;
                float distanceRemainingToGo = distanceMovedThisFrame;
                while (distanceRemainingToGo >= distanceToNextWaypoint)
                {
                    // Distance will go past the way point
                    // So we will want to move the ship part way of the way there
                    if (currentWaypoint >= 0)
                    {
                        Vector3 nextPathNode = linePathSmoothed[currentWaypoint];
                        Vector3 dir = nextPathNode - thisFrameShipPosition;


                        // Get the remainder distance that is after the next waypoint
                        float nextLoopDistance = distanceRemainingToGo - distanceToNextWaypoint;
                        // Only move the amount minus the above distance past waypoint
                        dir = Vector3.ClampMagnitude(dir, distanceRemainingToGo - nextLoopDistance);
                        // Add this to the ship position
                        thisFrameShipPosition += dir;

                        // Then update the new distance remaining to go for the while loop condition to remainder
                        distanceRemainingToGo = nextLoopDistance;
                        // And set to next waypoint  
                        currentWaypoint++;
                    }
                }
                
                // APPLY AND ADD TO TIMELINE FOR FRAME
                // If there is a loop then this position and distance remaining to go will have been updated.
                // If not it will just be normal from past frame.
                Vector3 finalPathNode = linePathSmoothed[currentWaypoint];
                Vector3 finalDir = finalPathNode - thisFrameShipPosition;
                finalDir = Vector3.ClampMagnitude(finalDir, distanceRemainingToGo);
                
                // Add the movement into this frame.
                shipActionsList.Add(new TimelineShipAction(
                    thisFrameShipPosition + finalDir, distanceMoved, this));
            }
            // TODO Visualise the action list on the screen for debug purposes.
            Debug.Log("ACTION LIST COUNT " + shipActionsList.Count + " DIST:  " + shipActionsList[^1].DistanceFromStart + " WAYPOINT:  " + currentWaypoint + " LINE PATH COUNT: " + linePathSmoothed.Count);
        }
  
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
