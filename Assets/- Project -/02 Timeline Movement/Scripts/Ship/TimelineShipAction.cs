using UnityEngine;

public class TimelineShipAction
{
    #region Variables
    public Vector3 Position { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float DistanceFromStart { get; private set; }
    public TimelineShip Ship { get; private set; }
    #endregion

    public TimelineShipAction(Vector3 position, Vector3 velocity, float distanceFromStart, TimelineShip ship)
    {
        Position = position;
        Velocity = velocity;
        DistanceFromStart = distanceFromStart;
        Ship = ship;
    }
}
