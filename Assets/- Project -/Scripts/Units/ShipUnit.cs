using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUnit : MonoBehaviour
{
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ------------------------------------- Ship Editable Variables -------------------------------------------------
    
	[SerializeField] private Sprite shipSprite; // Sprite for Ship when not selected
	[SerializeField] private Sprite shipSpriteHovered; // Sprite for Ship when selected
	[SerializeField] private Sprite shipSpriteSelected; // Sprite for Ship when selected
	
	[SerializeField] private Vector2Int shipGridPosition; // Starting grid position for this ship
	[SerializeField] private HexTile shipTarget; // Target for this ship to move towards
	[SerializeField] private float topSpeed; // Top speed for this ship 
	[SerializeField] private AnimationCurve shipSpeedCurve; // Curve to ease in and out the ship speed
	[SerializeField] private float rotateSpeed; // Rotation speed for this ship 
	[SerializeField] private AnimationCurve rotateSpeedCurve; // Curve to ease in and out the ship speed
	
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region --------------------------------------- Ship Private Variables ------------------------------------------------
    
	private Vector2 shipWorldPosition; // Position in the world for the Ship 
	private SpriteRenderer spriteRenderer; // Store sprite renderer for quick changeing
	
	private Rigidbody2D rigidBody; // Store sprite renderer for quick changeing
	private ShipUnit selectedShip; // Selected Ship as dictated from the GameController
	private HexTile hexTileInShipPosition; // HexTile in grid that this ship is currently on or over
	private Vector2 velocity; // Store sprite renderer for quick changeing
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
	
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ------------------------------------------- Ship Lifecycle ----------------------------------------------------
    
	private void Awake() 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D>();
	}
	
	private void Start() 
	{
		// Set up components and starting Grid and World position.
		Vector3 worldPosition = HexGrid.Instance.GetWorldPositionFromGrid(shipGridPosition.x, shipGridPosition.y);
		transform.position = worldPosition;
		shipWorldPosition.x = worldPosition.x;
		shipWorldPosition.y = worldPosition.y;
		
		// Get the Hextile from GridPosition and set its players
		hexTileInShipPosition = HexGrid.Instance.GetHexTileAtPosition(shipGridPosition.x, shipGridPosition.y);
		hexTileInShipPosition.SetShipUnit(this);
		
		// Set selected ship if there is one.
		selectedShip = GameController.Instance.GetSelectedShip();
	}
	
	void Update()
	{
		selectedShip = GameController.Instance.GetSelectedShip();
		SetSpriteBasedOnHoverSelected(selectedShip);
	}
	
	void FixedUpdate() 
	{
		// Method to check where ship is on the grid and then update which hextile it belongs in. (remove from current, add to another.)
		Vector2Int latestGridPosition = HexGrid.Instance.GetGridPositionFromWorld(transform.position);
		if (latestGridPosition != shipGridPosition) 
		{
			// Grid Position of ship has changed. so I need to clear old h ex
			hexTileInShipPosition.ClearShipUnit();
			
			// And grab the new hex and update.
			hexTileInShipPosition = HexGrid.Instance.GetHexTileAtPosition(latestGridPosition.x, latestGridPosition.y);
			hexTileInShipPosition.SetShipUnit(this);
		}
		
		if (shipTarget)  {
			HandleShipMovement(); // Handle Ship movement now that target is valid.
			HandleShipRotation(); // Handle ship rotation towards target with speed.
		}
	}
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ---------------------------------------------------- START ---------------------------------------------------------\\
	#region ------------------------------------------- Ship Methods ------------------------------------------------------
	
	public void SetShipMoveTarget(HexTile clickedHexTile) 
	{
		// Ship has just been assign target by MouseController
		shipTarget = clickedHexTile;
		SetMovementStart();
	}
	
	private void SetMovementStart() 
	{
		// Set up duration of move by testing distance against top speed
		shipEnd  = new Vector2(shipTarget.transform.position.x, shipTarget.transform.position.y);
		shipStart = transform.position;
		moveDuration = Vector2.Distance(shipStart, shipEnd) / topSpeed;
		
		// PROBLEM Longer journeys are slower. Shorter are faster, do below to balance.
		if (moveDuration < 2) { moveDuration = 2; }
		if (moveDuration > 8) { moveDuration = 8; }
		
		// Initialize time passed that will start now that target has been assigned.
		moveTimePassed = 0f;
		shipIsMoving = true;
		
		SetRotationStart();
	}
	
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
	
	private void HandleShipMovement() 
	{
		if (moveTimePassed >= moveDuration) 
		{
			// If the ship has gone through its time period. snap position and remove target
			rigidBody.MovePosition(shipEnd);
			shipIsMoving = false;
			if (shipIsRotating == false) { shipTarget = null; }
		} 
		else 
		{
			// Get movement step based on time passed since last frame, vs calculated duration from start.
			float movementAmount = moveTimePassed / moveDuration;
			
			// Adjust movement Amount with curve if we want to ease in and out.
			movementAmount = shipSpeedCurve.Evaluate(movementAmount);
			
			// Move ship with towards top speed to do this.
			rigidBody.MovePosition(Vector2.Lerp(shipStart, shipEnd, movementAmount));
			
			// Add time to timePassed to progress movement of Ship
			moveTimePassed += Time.deltaTime;
		}
	}
	
	private void HandleShipRotation() 
	{
		if (rotateTimePassed >= rotateDuration) 
		{
			// If the ship rotation has gone through its time period. snap rotation
			transform.rotation = rotationEnd;
			shipIsRotating = false;
			if (shipIsMoving == false) { shipTarget = null; }
		} 
		else 
		{
			// Get movement step based on time passed since last frame, vs calculated duration from start.
			float rotateAmount = rotateTimePassed / rotateDuration;
			
			// Adjust movement Amount with curve if we want to ease in and out.
			rotateAmount = rotateSpeedCurve.Evaluate(rotateAmount);
			
			// Rotate object based on rotate amount
			transform.rotation = Quaternion.Slerp(transform.rotation, rotationEnd, rotateAmount);
			
			// Add time to timePassed to progress movement of Ship
			rotateTimePassed += Time.deltaTime;
		}
	}

	private void SetSpriteBasedOnHoverSelected(ShipUnit selectedShip) 
	{
		// Set Ship sprrite based on hover or selected status in the Game Controller.
		if (GameController.Instance.GetHoveredShip() == this) 
		{
			spriteRenderer.sprite = shipSpriteHovered; 
		}
		else if (selectedShip == this) 
		{
			spriteRenderer.sprite = shipSpriteSelected; 
		}
		else 
		{
			spriteRenderer.sprite = shipSprite; 
		}
	}
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	
	
}
