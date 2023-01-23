using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : MonoBehaviour
{
    #region MoveAction Editable Variables
	[SerializeField] private float topSpeed; // Top speed for this ship 
	[SerializeField] private float rotateSpeed; // Rotation speed for this ship 
	[SerializeField] private int moveHexRange = 3; // Movement range of action on grid
	#endregion
	
	

    #region MoveAction Private Variables
	private Rigidbody2D rigidBody; // Store sprite renderer for quick changeing
	private ShipUnit shipUnit; // The ship unit attached to the same gameobject as this action
	private ShipUnit selectedShip; // Selected Ship as dictated from the GameController
	private HexTile shipTarget; // Target for this ship to move towards
	private bool isActive; // Is this overall mov action active
	
	private Vector2 shipStart; // Location of ship start location when moving
	private Vector2 shipEnd; // Location of ship end location when moving
	private float moveDuration; // Time for move commands from ship to target
	private float moveTimePassed; // Timing for when move is in progress
	private bool isMoving; // Is the ship moving
	
	private Quaternion rotationStart; // Direction the ship before target selection
	private Quaternion rotationEnd; // Direction the ship should be face based on last target
	private float rotateDuration; // Time for move commands from ship to target
	private float rotateTimePassed; // Timing for when move is in progress
	private bool issRotating; // Is the ship rotating
	
	private List<Vector2Int> validHexPositionList; // The position list for this move action based on ship unit grid possition
	#endregion
	
	

    #region MoveAction Lifecycle
	private void Awake() 
	{
		shipUnit = GetComponent<ShipUnit>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	private void Start() 
	{
		// Set selected ship if there is one
		selectedShip = GameController.Instance.GetSelectedShip();
		
		shipUnit.OnSelectedShipChange += ShipUnit_OnSelectedShipChange;
		GenerateValidPositions();
	}
	
	void FixedUpdate()
	{
		if (!isActive) return;
		
	    HandleShipMovement(); // Handle Ship movement now that target is valid
		HandleShipRotation(); // Handle ship rotation towards target with speed
	}
    
	private void Update() 
	{
		if (Input.GetKeyUp(KeyCode.T))
		{
			List<Vector2Int> validGridPositionLsit = GetValidHexPositionList();
			foreach (Vector2Int gridPosition in validGridPositionLsit)
			{
				//Debug.Log(gridPosition);
			}
		}
	}
	#endregion
    
    

	#region MoveAction Methods
	public void SetShipMoveTarget(HexTile clickedHexTile) 
	{
		// Ship has just been assign target by MouseController
		shipTarget = clickedHexTile.SetHexIsTarget();
		isActive = true;
		SetMovementStart();
		SetRotationStart();
	}
	
	private void ShipUnit_OnSelectedShipChange(object sender, EventArgs empty) 
	{
		GenerateValidPositions();
	}
	
	private List<Vector2Int> GenerateValidPositions()
	{
		List<Vector2Int> validPositionList = new List<Vector2Int>();
		
		// get current grid position
		Vector2Int unitGridPosition = shipUnit.GetShipGridPosition();
		Vector2 unitWorldPosition = HexGrid.Instance.GetWorldPositionFromGrid(unitGridPosition.x, unitGridPosition.y);
		
		// Get max distance based on hex width and current position (add 0.1 buffer for the pixel edge cases)
		float maxDistance = (HexGrid.Instance.GetWidthGap() * moveHexRange) + 0.1f;

		// Go through the neighbours to this position with the radius of the moveHexRange
		for (int x = -moveHexRange; x <= moveHexRange; x++) 
		{
			for (int y = -moveHexRange; y <= moveHexRange; y++)
			{
				Vector2Int testGridPosition = unitGridPosition + new Vector2Int(x, y);
				Vector2 testWorldPosition = HexGrid.Instance.GetWorldPositionFromGrid(testGridPosition.x, testGridPosition.y);
				
				if (HexGrid.Instance.IsInvalidGridPosition(testGridPosition)) continue;
				
				if (unitGridPosition == testGridPosition) continue;
				
				//Check if hex tile has ship or is the target destination for a ship.
				HexTile testTile = HexGrid.Instance.GetHexTileAtPosition(testGridPosition.x, testGridPosition.y);
				if (testTile.GetIsTarget()) continue;
				if (testTile.GetShipUnit() != null) continue;
				// Get tiles that that aren't over the diagonal distance to make steps accurate.

				if (Vector2.Distance(unitWorldPosition, testWorldPosition) > maxDistance) continue;


				validPositionList.Add(testGridPosition);
			}
		}
		
		validHexPositionList = validPositionList;
		return validHexPositionList;
	}
	
	public List<Vector2Int> GetValidHexPositionList() => validHexPositionList;
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
		isMoving = true;
	}
	
	private void HandleShipMovement() 
	{
		if (moveTimePassed >= moveDuration) 
		{
			// If the ship has gone through its time period. snap position and remove target
			rigidBody.MovePosition(shipEnd);
			isMoving = false;
			
			// If rotating job is done then remove ship target
			if (issRotating == false) 
			{
				if (shipTarget) shipTarget = shipTarget.ClearHexIsTarget(); 
				isActive = false;
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
		issRotating = true;
	}
	
	private void HandleShipRotation() 
	{
		if (rotateTimePassed >= rotateDuration) 
		{
			// If the ship rotation has gone through its time period. snap rotation
			transform.rotation = rotationEnd;
			issRotating = false;
			
			// If moving job is done then remove ship target
			if (isMoving == false) 
			{ 
				if (shipTarget) shipTarget = shipTarget.ClearHexIsTarget(); 
				isActive =false;
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
