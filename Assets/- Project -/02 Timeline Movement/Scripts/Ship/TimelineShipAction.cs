using UnityEngine;

public class TimelineShipAction
{
    #region Variables
    public Vector3 Position { get; private set; }
    public float DistanceFromStart { get; private set; }
    public TimelineShip Ship { get; private set; }
    #endregion

    public TimelineShipAction(Vector3 position, float distanceFromStart, TimelineShip ship)
    {
        Position = position;
        DistanceFromStart = distanceFromStart;
        Ship = ship;
    }
}
