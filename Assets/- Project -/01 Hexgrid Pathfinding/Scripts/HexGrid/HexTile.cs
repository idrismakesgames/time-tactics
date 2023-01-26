using UnityEngine;
using System;
using TMPro;

public class HexTile : MonoBehaviour
{
    #region Variables
	[SerializeField] private TextMeshPro textMeshPro; // Text to show Grid Coordinate
	[SerializeField] private Sprite hexSprite; // Sprite for HexTile when not selected
	[SerializeField] private Sprite hexSpriteHovered; // Sprite for HexTile when selected
	[SerializeField] private Vector2Int hexTileGridPosition; // Grid Coordinate as defined during HexGridGeneration.
	[SerializeField] private bool isObstacle; // Is this hexTile an obstacle.

	private SpriteRenderer spriteRenderer; // Store sprite renderer for quick changing.
	private Vector2 hexTileWorldPosition; // Position in the world this was instantiated
	
	public ShipUnit shipUnitOnTile; // Store the ship that is over this tile if there is one.
	private bool isTarget; // Is this hex tile currently a ships target
	
	private Color movableColour;
	private Color defaultColour;
	#endregion

	#region Lifecycle
	private void Awake() 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		defaultColour = spriteRenderer.color;
		movableColour = defaultColour;
		movableColour.a = 255f;
	}
	
	private void Start() 
	{
		// Get HexTile World Position
		hexTileWorldPosition = HexGrid.Instance.GetWorldPositionFromGrid(hexTileGridPosition.x, hexTileGridPosition.y);
		
		// Set HexTile default name for GameObject
		textMeshPro.text = $"{(int)Mathf.Round(hexTileGridPosition.x)}-{(int)Mathf.Round(hexTileGridPosition.y)}";
		
		// Subscribe to the event when a hovered hex changes from GameController
		GameController.Instance.OnHoveredHexChange += GameController_OnHoveredHexChange;
	}
	#endregion
	
    #region Methods
    public void SetStartingValues(Vector2 hexTileWorldPos, Vector2Int hexTileGridPos) 
	{
		// Set the GameObject, Position, Coordinate, and Text.
		hexTileWorldPosition = hexTileWorldPos;
		hexTileGridPosition = hexTileGridPos;
		textMeshPro.text = $"{(int)Mathf.Round(hexTileGridPos.x)}-{(int)Mathf.Round(hexTileGridPos.y)}";
	}
	#endregion
	
	#region Events
	private void GameController_OnHoveredHexChange(object sender, EventArgs empty) 
	{
		// When hovered tile is changed call this subscribed event
		spriteRenderer.sprite = GameController.Instance.GetHoveredHex() == this ? hexSpriteHovered : hexSprite;
	}
	#endregion

	#region Accessors
	// HexTile related accessors
	public Vector2 GetWorldPosition() => this.hexTileWorldPosition;
	
	public Vector2 GetGridPosition() => this.hexTileGridPosition;
	
	// ShipUnit related Accessors
	public void SetShipUnit(ShipUnit shipUnit)  { shipUnitOnTile = shipUnit; }
	
	public void ClearShipUnit() { shipUnitOnTile = null; }
	
	public ShipUnit GetShipUnit() => shipUnitOnTile;
	
	// Target Variables
	public bool GetIsTarget() => isTarget;
	
	public HexTile SetHexIsTarget() 
	{
		isTarget = true;
		return this;
	}
	
	public HexTile ClearHexIsTarget() 
	{
		isTarget = false;
		return null;
	}
	
	// Highlight Movable Hexes
	public void ShowValidHex() 
	{
		spriteRenderer.color = movableColour;
	}
	
	public void HideValidHex() 
	{
		spriteRenderer.color = defaultColour;
	}
	
	public bool GetIsObstacle() => isObstacle;
	#endregion
}
