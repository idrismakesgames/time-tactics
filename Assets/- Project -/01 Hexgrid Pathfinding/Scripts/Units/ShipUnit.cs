using UnityEngine;
using System;

public class ShipUnit : MonoBehaviour
{
    #region Variables 
	[SerializeField] private Sprite shipSprite; // Sprite for Ship when not selected
	[SerializeField] private Sprite shipSpriteHovered; // Sprite for Ship when selected
	[SerializeField] private Sprite shipSpriteSelected; // Sprite for Ship when selected
	
	[SerializeField] private Vector2Int shipGridPosition; // Starting grid position for this ship

	public event EventHandler OnSelectedShipChange;
	private Vector2 shipWorldPosition; // Position in the world for the Ship 
	private SpriteRenderer spriteRenderer; // Store sprite renderer for quick changing
	private HexTile hexTileInShipPosition; // HexTile in grid that this ship is currently on or over
	
	private MoveAction moveAction; // Move to Hex action that is assigned to a Unit
	#endregion

	#region Lifecycle
	private void Awake() 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		moveAction = GetComponent<MoveAction>();
	}
	
	private void Start() 
	{
		// Set up components and starting Grid and World position.
		Vector3 worldPosition = HexGrid.Instance.GetWorldPositionFromGrid(shipGridPosition.x, shipGridPosition.y);
		transform.position = worldPosition;
		shipWorldPosition.x = worldPosition.x;
		shipWorldPosition.y = worldPosition.y;
		
		// Get the HexTile from GridPosition and set its players
		hexTileInShipPosition = HexGrid.Instance.GetHexTileAtPosition(shipGridPosition.x, shipGridPosition.y);
		hexTileInShipPosition.SetShipUnit(this);
		
		// Subscribe to events where Hovered and Selected ship changes in the GameController.
		GameController.Instance.OnHoveredShipChange += GameController_OnHoveredShipChange;
		GameController.Instance.OnSelectedShipChange += GameController_OnSelectedShipChange;
	}
	
	void FixedUpdate() 
	{
		// Method to check where ship is on the grid and then update which HexTile it belongs in. 
		Vector2Int latestGridPosition = HexGrid.Instance.GetGridPositionFromWorld(transform.position);
		if (latestGridPosition != shipGridPosition) 
		{
			// Grid Position of ship has changed. so I need to clear old h ex
			hexTileInShipPosition.ClearShipUnit();
			
			// And grab the new hex and update.
			hexTileInShipPosition = HexGrid.Instance.GetHexTileAtPosition(latestGridPosition.x, latestGridPosition.y);
			hexTileInShipPosition.SetShipUnit(this);
			
			
			// When ship does move from one hex to another, call an event that will assign to generate new valid positions.
			shipGridPosition = latestGridPosition;
			shipWorldPosition = HexGrid.Instance.GetWorldPositionFromGrid(shipGridPosition.x, shipGridPosition.y);
			OnSelectedShipChange?.Invoke(this, EventArgs.Empty);
		}
	}
	#endregion

	#region Ship Methods
	private void SetSpriteBasedOnHoverSelected() 
	{
		// Set Ship sprite based on hover or selected status in the Game Controller.
		if		(GameController.Instance.GetHoveredShip() == this) spriteRenderer.sprite = shipSpriteHovered; 
		else if (GameController.Instance.GetSelectedShip() == this) spriteRenderer.sprite = shipSpriteSelected; 
		else	spriteRenderer.sprite = shipSprite; 
	}
	#endregion

	#region Events
	private void GameController_OnHoveredShipChange(object sender, EventArgs empty)  
	{
		SetSpriteBasedOnHoverSelected();
	}
	
	private void GameController_OnSelectedShipChange(object sender, EventArgs empty)  
	{
		SetSpriteBasedOnHoverSelected();
	}
	#endregion
	
	#region Ship Accessors
	public MoveAction GetMoveAction() => moveAction;
	
	public Vector2Int GetShipGridPosition() => shipGridPosition;
	
	public Vector3 GetShipWorldPosition() => shipWorldPosition;
	#endregion
}
