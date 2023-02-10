using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class TimelineShip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shipSelectionSprite;
    [SerializeField] private PivotPoint pivotObject;
    
    private List<Vector3> linePath; // Path for ship to follow
    private List<Vector3> linePathSmoothed; // Path for ship to follow smoothed
    private LineRenderer lineRenderer; // Line that will show on screen

    #region Lifecycle Methods
    private void Start()
    {
        TimelineController.Instance.OnSelectedShipChange += TimelineController_OnSelectedShipChange;
        linePath = new List<Vector3>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TimelineController.Instance.SelectedShip == this && !TimelineController.Instance.HoveredShip && !TimelineController.Instance.HoveredPivot)
            {
                linePath.Add(TimelineController.Instance.MousePosition);
                PivotPoint pivotPointInstance = Instantiate(pivotObject, TimelineController.Instance.MousePosition, quaternion.identity);
                pivotPointInstance.SetStartingValues(this, linePath.Count-1, linePath);
                FeedPathToLineRenderer();
            }
        }
    }

    #endregion
    
    #region TimelineController Methods
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
