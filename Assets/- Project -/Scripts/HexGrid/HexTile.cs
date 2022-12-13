using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexTile : MonoBehaviour
{
    #region Editable Variables
	[SerializeField] private TextMeshPro textMeshPro; // Text to show Grid Coordinate
	#endregion
	
	
    #region Private Variables
	private GameObject hexTileObject; // GameObject that was instantiated by HexGrid 
	private Vector2 hexTileWorldPosition; // Position in the world this was instantiated
	private Vector2 hexTileGridPosition; // Grid Coordinate as defined during HexGridGeneration.
	#endregion
	
	
    #region HexTile Methods
	public void SetStartingValues(GameObject hexTileObj, Vector2 hexTileWorldPos, Vector2 hexTileGridPos) 
	{
		// Set the GameObject, Position, Coordinate, and Text.
		hexTileObject = hexTileObj;
		hexTileWorldPosition = hexTileWorldPos;
		hexTileGridPosition = hexTileGridPos;
		textMeshPro.text = $"{(int)Mathf.Round(hexTileGridPos.x)}-{(int)Mathf.Round(hexTileGridPos.y)}";
	}
    #endregion
    
	    
    #region HexTile Accessors
	public GameObject GetHexTileObject()
	{
		return this.hexTileObject;
	}
	
	public Vector2 GetWorldPosition()
	{
		return this.hexTileWorldPosition;
	}
	
	public Vector2 GetGridPosition()
	{
		return this.hexTileGridPosition;
	}
    #endregion
}
