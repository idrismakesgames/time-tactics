using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUnit : MonoBehaviour
{
	public float speed;
    #region Ship Editable Vars
	[SerializeField] private Vector2Int shipGridPosition; // Starting grid position for this ship
	[SerializeField] private float topSpeed; // Top speed for this ship
	[SerializeField] private float shipAcceleration; // Acceleration for this ship
	[SerializeField] private float shipDecceleration; // Deceleration for this ship
	[SerializeField] private Transform shipTarget; // Deceleration for this ship
	
	[SerializeField] private Sprite shipSprite; // Sprite for Ship when not selected
	[SerializeField] private Sprite shipSpriteHovered; // Sprite for Ship when selected
	[SerializeField] private Sprite shipSpriteSelected; // Sprite for Ship when selected
	#endregion
	
    #region Ship Private Vars
	private Vector2 shipWorldPosition; // Position in the world for the Ship 
	private SpriteRenderer spriteRenderer; // Store sprite renderer for quick changeing.
	private Rigidbody2D rigidBody; // Store sprite renderer for quick changeing.
	private Vector2 velocity; // Store sprite renderer for quick changeing.
	#endregion
	
    #region Ship LifeCycle
	private void Awake() 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D>();
	}
	
	private void Start() 
	{
		// Set up components and starting Grid and World position.
		Vector3 worldPosition = HexGrid.Instance.GetWorldPositionFromGrid(shipGridPosition.x, shipGridPosition.y);
		shipWorldPosition.x = worldPosition.x;
		shipWorldPosition.y = worldPosition.y;
		
		// Get the Hextile from GridPosition and set its players
		HexTile hexTileInShipPosition = HexGrid.Instance.GetHexTileAtPosition(shipGridPosition.x, shipGridPosition.y);
		hexTileInShipPosition.SetShipUnit(this);
	}
	
	void Update()
	{
		ShipUnit selectedShip = GameController.Instance.GetSelectedShip();
		SetSpriteBasedOnHoverSelected(selectedShip);
	}
	
	void FixedUpdate() 
	{
		ShipUnit selectedShip = GameController.Instance.GetSelectedShip();
		if (selectedShip == this) {
			// Set target, calculate time it will take by checking distance and top speed
			// Time factor that takes in passed time and duration will be what we use as the lerp position.
			// Use Smoothstep or anim curve for accel decel.			
			
			// Do same for rotate, how many second to get to rotation. 
			// Use time to acount for movement as this is what this prototype is all about!!!
		}

	}
	#endregion
	
	#region ShipUnit Methods
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
	#endregion


}
