using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class HexTile : MonoBehaviour
{
    #region HexTile Editable Variables
	[SerializeField] private TextMeshPro textMeshPro; // Text to show Grid Coordinate
	[SerializeField] private Sprite hexSprite; // Sprite for HexTile when not selected
	[SerializeField] private Sprite hexSpriteHovered; // Sprite for HexTile when selected
	[SerializeField] private Vector2Int hexTileGridPosition; // Grid Coordinate as defined during HexGridGeneration.
	#endregion
	
	

    #region HexTile Private Variables
	private SpriteRenderer spriteRenderer; // Store sprite renderer for qwuick changeing.
	private Vector2 hexTileWorldPosition; // Position in the world this was instantiated
	
	public ShipUnit shipUnitOnTile; // Store the ship that is over this tile if there is one.
	private bool isTarget; // Is this hex tile currently a ships target
	
	private Color movableColour;
	private Color defaultColour;
	#endregion

	

    #region HexTile Lifecycle
	private void Awake() 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		defaultColour = spriteRenderer.color;
		movableColour = defaultColour;
		movableColour.a = 255f;
	}
	
	private void Start() 
	{
		// Get Hextile World Position
		hexTileWorldPosition = HexGrid.Instance.GetWorldPositionFromGrid(hexTileGridPosition.x, hexTileGridPosition.y);
		
		// Set hextile default name for gameobject
		textMeshPro.text = $"{(int)Mathf.Round(hexTileGridPosition.x)}-{(int)Mathf.Round(hexTileGridPosition.y)}";
		
		// Subscribe to the event when a hovered hex changes from GameController
		GameController.Instance.OnHoveredHexChange += GameController_OnHoveredHexChange;
	}
	#endregion
	
	
	
    #region HexTile Methods
	private void GameController_OnHoveredHexChange(object sender, EventArgs empty) 
	{
		// When hovered tile is changed call this subscribed event
		spriteRenderer.sprite = GameController.Instance.GetHoveredHex() == this ? hexSpriteHovered : hexSprite;
	}
    
	public void SetStartingValues(Vector2 hexTileWorldPos, Vector2Int hexTileGridPos) 
	{
		// Set the GameObject, Position, Coordinate, and Text.
		hexTileWorldPosition = hexTileWorldPos;
		hexTileGridPosition = hexTileGridPos;
		textMeshPro.text = $"{(int)Mathf.Round(hexTileGridPos.x)}-{(int)Mathf.Round(hexTileGridPos.y)}";
	}
	#endregion
	

	    
    #region HexTile Accessors
	// HexTile related accessors
	public Vector2 GetWorldPosition() => this.hexTileWorldPosition;
	
	public Vector2 GetGridPosition() => this.hexTileGridPosition;
	
	// ShipUnit releated Accessors
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
	
	// Highlight Moveable Hexes
	public void ShowValidHex() 
	{
		spriteRenderer.color = movableColour;
	}
	
	public void HideValidHex() 
	{
		spriteRenderer.color = defaultColour;
	}
	#endregion
	
	

}
