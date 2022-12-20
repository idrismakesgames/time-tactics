using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    #region MoveAction Editable Variables
	[SerializeField] private float topSpeed; // Top speed for this ship 
	[SerializeField] private float rotateSpeed; // Rotation speed for this ship 
	#endregion
	
	

    #region MoveAction Private Variables
	private Rigidbody2D rigidBody; // Store sprite renderer for quick changeing
	private ShipUnit selectedShip; // Selected Ship as dictated from the GameController
	private HexTile shipTarget; // Target for this ship to move towards
	
	private Vector2 shipStart; // Location of ship start location when moving
	private Vector2 shipEnd; // Location of ship end location when moving.
	private float moveDuration; // Time for move commands from ship to target
	private float moveTimePassed; // Timing for when move is in progress
	private bool shipIsMoving; // Is the ship moving
	
	private Quaternion rotationStart; // Direction the ship before target selection
	private Quaternion rotationEnd; // Direction the ship should be face based on last target.
	private float rotateDuration; // Time for move commands from ship to target
	private float rotateTimePassed; // Timing for when move is in progress
	private bool shipIsRotating; // Is the ship rotating
	#endregion
	
	

    #region MoveAction Lifecycle
	private void Awake() 
	{
		rigidBody = GetComponent<Rigidbody2D>();
	}

	private void Start() 
	{
		// Set selected ship if there is one.
		selectedShip = GameController.Instance.GetSelectedShip();
	}
	
	void FixedUpdate()
    {
	    if (shipTarget)  {
		    HandleShipMovement(); // Handle Ship movement now that target is valid.
		    HandleShipRotation(); // Handle ship rotation towards target with speed.
	    }
    }
	#endregion
    
    

	#region MoveAction Methods
	public void SetShipMoveTarget(HexTile clickedHexTile) 
	{
		// Ship has just been assign target by MouseController
		shipTarget = clickedHexTile;
		SetMovementStart();
	}
	#endregion
	
	
	
	#region MoveAction Movement Methods
	private void SetMovementStart() 
	{
		// Set up duration of move by testing distance against top speed
		shipEnd  = new Vector2(shipTarget.transform.position.x, shipTarget.transform.position.y);
		shipStart = transform.position;
		moveDuration = Vector2.Distance(shipStart, shipEnd) / topSpeed;
		
		// Initialize time passed that will start now that target has been assigned.
		moveTimePassed = 0f;
		shipIsMoving = true;
		
		SetRotationStart();
	}
	
	private void HandleShipMovement() 
	{
		if (moveTimePassed >= moveDuration) 
		{
			// If the ship has gone through its time period. snap position and remove target
			rigidBody.MovePosition(shipEnd);
			shipIsMoving = false;
			
			// If rotating job is done then remove ship target
			if (shipIsRotating == false) 
			{
				shipTarget = null; 
			}
		} 
		else 
		{
			// Get movement step based on time passed since last frame, vs calculated duration from start.
			float movementAmount = moveTimePassed / moveDuration;
			
			// IDRIS-TODO Implement Acceleration and deceleration on fixed time rather than ratio of distance
			
			// Move ship with towards top speed to do this.
			rigidBody.MovePosition(Vector2.Lerp(shipStart, shipEnd, movementAmount));
			
			// Add time to timePassed to progress movement of Ship
			moveTimePassed += Time.deltaTime;
		}
	}
	#endregion
	
	
	
	#region MoveAction Rotation Methods	
	private void SetRotationStart() 
	{
		// Set up duration of rotation by testing angle between start and target
		rotationStart = transform.rotation;
		Vector2 lookDirection = shipStart - shipEnd;
		rotationEnd = Quaternion.LookRotation(Vector3.forward, lookDirection);
		rotateDuration = Quaternion.Angle(rotationStart, rotationEnd) / rotateSpeed;
		
		// Initialize rotation time passed that will start now that target has been assigned.
		rotateTimePassed = 0f;
		shipIsRotating = true;
	}
	
	private void HandleShipRotation() 
	{
		if (rotateTimePassed >= rotateDuration) 
		{
			// If the ship rotation has gone through its time period. snap rotation
			transform.rotation = rotationEnd;
			shipIsRotating = false;
			
			// If moving job is done then remove ship target
			if (shipIsMoving == false) 
			{ 
				shipTarget = null; 
			}
		} 
		else 
		{
			// Get movement step based on time passed since last frame, vs calculated duration from start.
			float rotateAmount = rotateTimePassed / rotateDuration;
			
			// IDRIS-TODO Implement Acceleration and deceleration on fixed time rather than ratio of distance
			
			// Rotate object based on rotate amount
			transform.rotation = Quaternion.Slerp(transform.rotation, rotationEnd, rotateAmount);
			
			// Add time to timePassed to progress movement of Ship
			rotateTimePassed += Time.deltaTime;
		}
	}
	#endregion
}
