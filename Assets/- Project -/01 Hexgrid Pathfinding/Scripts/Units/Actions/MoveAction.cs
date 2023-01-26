using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class MoveAction : MonoBehaviour
{
    #region Variables
	[SerializeField] private float topSpeed; // Top speed for this ship 
	[SerializeField] private int moveHexRange = 3; // Movement range of action on grid

	private Rigidbody2D rigidBody; // Store sprite renderer for quick changing
	private ShipUnit shipUnit; // The ship unit attached to the same GameObject as this action
	private HexTile shipTarget; // Target for this ship to move towards
	private bool isActive; // Is this overall mov action active
	
	private List<Vector3> pathVectors; // The path for the action on smoothed vector basis.
	private int currentPathStep;
	
	private Vector2 shipStart; // Location of ship start location when moving
	private Vector2 shipEnd; // Location of ship end location when moving
	private float moveDuration; // Time for move commands from ship to target
	private float moveTimePassed; // Timing for when move is in progress
	private bool isMoving; // Is the ship moving
	
	private Quaternion rotationStart; // Direction the ship before target selection
	private Quaternion rotationEnd; // Direction the ship should be face based on last target
	private float rotateDuration; // Time for move commands from ship to target
	private float rotateTimePassed; // Timing for when move is in progress
	private bool isRotating; // Is the ship rotating
	
	private List<Vector2Int> validHexPositionList; // The position list for this move action based on ship unit grid position
	
	private Seeker seeker; // Seeker for the shipUnit to allow for pathfinding to valid hexes from position.
	#endregion
	
    #region Lifecycle
	private void Awake() 
	{
		shipUnit = GetComponent<ShipUnit>();
		seeker = GetComponent<Seeker>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	private void Start() 
	{
		shipUnit.OnSelectedShipChange += ShipUnit_OnSelectedShipChange;
		GenerateValidPositions();
		PathToValidHexPositions();
	}
	
	void FixedUpdate()
	{
		if (!isActive) return;
		
		HandleShipMovement(); // Handle Ship movement now that target is valid
	}
	#endregion

	#region Methods
	public void SetShipMoveTarget(HexTile clickedHexTile, Path path) 
	{
		// Ship has just been assign target by MouseController
		shipTarget = clickedHexTile.SetHexIsTarget();
		isActive = true;
		
		// Assign paths to this action on movement start
		pathVectors = path.vectorPath;
		
		// Start the movement 
		SetMovementStart();
	}

	private void PathToValidHexPositions() 
	{
		// Generate multi-paths for all valid hex positions
		Vector3[] endPoints = new Vector3[validHexPositionList.Count];
		int indexCount = 0;
		
		foreach (Vector2Int hexGridPosition in validHexPositionList)
		{
			endPoints[indexCount] = HexGrid.Instance.GetWorldPositionFromGrid(hexGridPosition.x, hexGridPosition.y);
			indexCount++;
		}
		
		seeker.StartMultiTargetPath(transform.position, endPoints, true, OnPathComplete);
	}
	
	private void OnPathComplete (Path p) 
	{
		if (p is MultiTargetPath mp)
		{
			List<GraphNode>[] paths = mp.nodePaths;

			List<int> hexPositionsToRemove = new List<int>();
			List<Vector2Int> finalValidPositionList = new List<Vector2Int>();

			// On Complete remove the hexes that dont meet endpoint of have too many nodes.
			for (int i = 0; i < paths.Length; i++)
			{
				List<GraphNode> currentPath = paths[i];
				if (currentPath.Count > (moveHexRange + 1))
				{
					hexPositionsToRemove.Add(i);
				}
				else if (Vector3.Distance(
					         HexGrid.Instance.GetWorldPositionFromGrid(validHexPositionList[i].x,
						         validHexPositionList[i].y), (Vector3)currentPath[paths[i].Count - 1].position) > 0.1)
				{
					hexPositionsToRemove.Add(i);
				}
			}

			// Generate new Valid hex position grid based on the hexPositionsToRemove
			for (int j = 0; j < validHexPositionList.Count; j++)
			{
				Vector2Int validPosition = validHexPositionList[j];

				if (!hexPositionsToRemove.Contains(j))
				{
					finalValidPositionList.Add(validPosition);
				}
			}

			validHexPositionList = finalValidPositionList;
		}
	}
	
	private void GenerateValidPositions()
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
				if (testTile.GetIsObstacle()) continue;
				if (testTile.GetShipUnit() != null) continue;
				// Get tiles that that aren't over the diagonal distance to make steps accurate.
				if (Vector2.Distance(unitWorldPosition, testWorldPosition) > maxDistance) continue;

				validPositionList.Add(testGridPosition);
			}
		}
		
		validHexPositionList = validPositionList;
	}
	
	public List<Vector2Int> GetValidHexPositionList() => validHexPositionList;
	#endregion
	
	#region Movement Methods
	private void SetMovementStart() 
	{
		// Set up duration of move by testing distance against top speed
		shipEnd  = new Vector2(pathVectors[currentPathStep].x, pathVectors[currentPathStep].y);
		shipStart = transform.position;
		moveDuration = Vector2.Distance(shipStart, shipEnd) / topSpeed;
		
		// Initialize time passed that will start now that target has been assigned.
		moveTimePassed = 0f;
	}
	
	private void HandleShipMovement() 
	{
		if (moveTimePassed >= moveDuration) 
		{
			// If the current path step is reached and there is 
			if (currentPathStep < pathVectors.Count-1) 
			{
				// Set up move duration and values for next step in the path. 
				rigidBody.MovePosition(shipEnd);
				currentPathStep++;
				
				SetMovementStart();
				
				// Then process for this frame.
				ProcessMovement();
			}
			else 
			{
				// Movement reached end point so rest paths and starting values.
				ResetMovementValues();
			}
		} 
		else 
		{
			// process movement towards next vector in path normally.
			ProcessMovement();
		}
	}
	
	private void ProcessMovement() 
	{
		// Get movement step based on time passed since last frame, vs calculated duration from start.
		float movementAmount = moveTimePassed / moveDuration;
			
		// Move ship with towards top speed to do this.
		rigidBody.MovePosition(Vector2.Lerp(shipStart, shipEnd, movementAmount));
		
		// Handle rotation based on direction towards vector in path.
		Vector2 lookDirection = shipStart - shipEnd;
		transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
		
		// Add time to timePassed to progress movement of Ship
		moveTimePassed += Time.deltaTime;
	}
	
	private void ResetMovementValues() 
	{
		// If the ship has gone through its time period. snap position and remove target
		rigidBody.MovePosition(shipEnd);
		isActive = false;
		pathVectors = null;
		currentPathStep = 0;

		if (shipTarget) shipTarget = shipTarget.ClearHexIsTarget();
	}
	#endregion
	
	#region Events
	private void ShipUnit_OnSelectedShipChange(object sender, EventArgs empty) 
	{
		GenerateValidPositions();
		PathToValidHexPositions();
	}
	#endregion
}
