using UnityEngine;
using System;


public class TimelineController: MonoBehaviour
{
    #region Variables
    public static TimelineController Instance { get; private set; } // Singleton to allow GameController access game wide
    public Vector3 MousePosition { get; private set; } // where is the mouse position on screen, as seen through camera

    public TimelineShip SelectedShip { get; private set; }
    public TimelineShip HoveredShip { get; set; }
    public PivotPoint HoveredPivot { get; set; }
    public PivotPoint SelectedPivot { get; set; }

    public event EventHandler OnSelectedShipChange;
    #endregion
	
    #region Lifecycle
    private void Awake() 
    { 
        Instance = this; 
    }

    private void FixedUpdate()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        
        RaycastHit2D hit = Physics2D.Raycast(MousePosition, Vector2.zero, float.MaxValue);
        if(hit.collider)
        {
            HoveredShip = hit.collider.gameObject.TryGetComponent(out TimelineShip ship) ? ship : null; 
            HoveredPivot = hit.collider.gameObject.TryGetComponent(out PivotPoint pivot) ? pivot : null; 
        }
        else
        {
            HoveredShip = null;
            HoveredPivot = null;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Select ship if there is a hovered one and invoke event.
            if (HoveredShip)
            {
                SelectedShip = HoveredShip != SelectedShip ? HoveredShip : null;
                HoveredShip = null;
                // Call event for ships to update themselves.
                OnSelectedShipChange?.Invoke(SelectedShip, EventArgs.Empty);
            }
        }
    }
    #endregion
}
