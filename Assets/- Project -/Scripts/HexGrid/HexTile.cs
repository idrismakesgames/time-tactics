using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexTile : MonoBehaviour
{
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ------------------------------------- HexTile Editable Variables ----------------------------------------------
    
	[SerializeField] private TextMeshPro textMeshPro; // Text to show Grid Coordinate
	[SerializeField] private Sprite hexSprite; // Sprite for HexTile when not selected
	[SerializeField] private Sprite hexSpriteSelected; // Sprite for HexTile when selected
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ------------------------------------- HexTile Private Variables -----------------------------------------------
    
	private GameObject hexTileObject; // GameObject that was instantiated by HexGrid 
	private SpriteRenderer spriteRenderer; // Store sprite renderer for qwuick changeing.
	private Vector2 hexTileWorldPosition; // Position in the world this was instantiated
	private Vector2 hexTileGridPosition; // Grid Coordinate as defined during HexGridGeneration.
	
	private ShipUnit shipUnitOnTile;
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//

	
	
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ----------------------------------------- HexTile Lifecycle ---------------------------------------------------
    
	private void Awake() 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	private void Update() 
	{
		// Check if this tile is the selected instance
		spriteRenderer.sprite = GameController.Instance.GetHoveredHex() == this ? hexSpriteSelected : hexSprite;
	}
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ------------------------------------------ HexTile Methods ----------------------------------------------------
    
	public void SetStartingValues(GameObject hexTileObj, Vector2 hexTileWorldPos, Vector2 hexTileGridPos) 
	{
		// Set the GameObject, Position, Coordinate, and Text.
		hexTileObject = hexTileObj;
		hexTileWorldPosition = hexTileWorldPos;
		hexTileGridPosition = hexTileGridPos;
		textMeshPro.text = $"{(int)Mathf.Round(hexTileGridPos.x)}-{(int)Mathf.Round(hexTileGridPos.y)}";
	}
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	

	    
	// ---------------------------------------------------- START ---------------------------------------------------------\\
    #region ----------------------------------------- HexTile Accessors ---------------------------------------------------
    
	// HexTile related accessors
	public GameObject GetHexTileObject() => this.hexTileObject;

	public Vector2 GetWorldPosition() => this.hexTileWorldPosition;
	
	public Vector2 GetGridPosition() => this.hexTileGridPosition;
	
	// ShipUnit releated Accessors
	public void SetShipUnit(ShipUnit shipUnit)  { shipUnitOnTile = shipUnit; }
	
	public void ClearShipUnit() { shipUnitOnTile = null; }
	
	public ShipUnit GetShipUnit() => shipUnitOnTile;
	
	#endregion ------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------//
	
	

}
