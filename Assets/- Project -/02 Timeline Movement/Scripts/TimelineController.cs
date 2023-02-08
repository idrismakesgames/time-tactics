using System.Collections.Generic;
using UnityEngine;


public class TimelineController: MonoBehaviour
{
    #region Variables
    public static TimelineController Instance { get; private set; } // Singleton to allow GameController access game wide

    private TimelineShip hoveredShip; // Hovered Ship stored here to show visual
    private TimelineShip selectedShip; // Selected Ship stored here to show visual

    private Vector3 mousePosition; // where is the mouse position on screen, as seen through camera
    private List<Vector3> linePath; // Path for ship to follow
    private List<Vector3> linePathSmoothed; // Path for ship to follow smoothed
    private LineRenderer lineRenderer; // Line that will show on screen
    #endregion
	
    #region Lifecycle
    private void Awake() 
    { 
        Instance = this; 
    }

    private void Start()
    {
        linePath = new List<Vector3>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            linePath.Add(mousePosition);

            // Use smoothing from A* to then create curvature
            linePathSmoothed = TimelineHelperMethods.Instance.LineSmoothBezier(linePath);
            //linePathSmoothed = TimelineHelperMethods.Instance.LineSmoothSimple()(linePath);
            
            // Pass these points to line renderer.
            lineRenderer.positionCount = linePathSmoothed.Count;
            lineRenderer.SetPositions(linePathSmoothed.ToArray());
        }
    }

    #endregion
}
