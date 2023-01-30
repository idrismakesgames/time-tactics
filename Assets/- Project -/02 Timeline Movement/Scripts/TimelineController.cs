using UnityEngine;

public class TimelineController: MonoBehaviour
{
    #region Variables
    public static TimelineController Instance { get; private set; } // Singleton to allow GameController access game wide

    private TimelineShip hoveredShip; // Hovered Ship stored here to show visual
    private TimelineShip selectedShip; // Selected Ship stored here to show visual
    #endregion
	
    #region Lifecycle
    private void Awake() 
    { 
        Instance = this; 
    }
    #endregion
}
