using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    #region Editable Variables
	[SerializeField] private HexTile hexTileObject;
	[SerializeField] private float widthOffset; // Row spacing for hex
	[SerializeField] private float heightOffset; // Column spacing for hex
	[SerializeField] private int rowLength; // Number of rows in HexGrid
	[SerializeField] private int colHeight; // Number of Columns in HexGrid
	#endregion
	
	
    #region Private Variables
	private Sprite hexTileSprite;
	private float widthGap; // Horizontal spacing based on Sprite
	private float heightGap; // Vertical spacing based on Sprite
	private HexTile[,] hexTileArray; // Holds reference to every tile in the HexGrid
	#endregion
	
	
    #region Lifecycle Methods
    void Start()
	{
		// Set the sprite to get the width and height for grid generation
		hexTileSprite = hexTileObject.GetComponent<SpriteRenderer>().sprite; 
		widthGap = (hexTileSprite.bounds.extents.x * 2f) + widthOffset;
		heightGap = (hexTileSprite.bounds.extents.y * 1.5f) + heightOffset;
	
		// Instance the array that will hold all the HexGrid Tiles
		hexTileArray = new HexTile[rowLength, colHeight];
		
		// Generate the HexGrid
		GenerateHexGrid();
	}
    #endregion
    
    
    #region HexGrid Methods
	private void GenerateHexGrid()
	{
		// Loop throught set Row and Column amount to generate Grid
		for (int x = 0; x < rowLength; x++) 
		{
			for (int y = 0; y < colHeight; y++) 
			{
				Vector3 tilePosition = GetWorldPositionFromGrid(x, y);
				
				// Instantiate HexTile Prefab using Ref, Set Values + Name, and Attach to Grid as Child
				HexTile hexTileInstance = Instantiate(hexTileObject, tilePosition,  Quaternion.identity);
				hexTileInstance.SetStartingValues(hexTileInstance.gameObject, new Vector2(tilePosition.x, tilePosition.y), new Vector2(x, y));
				hexTileInstance.name = $"HexTile {x}-{y}";
				hexTileInstance.transform.parent =  this.transform;
				
				// Add this instance to the two dimensional array for later reference
				hexTileArray[x, y] = hexTileInstance;
			}
		}
	}   
	
	public Vector3 GetWorldPositionFromGrid(int x, int y)  
	{
		// Set the position of the tile based on Grid Coordinate. (Using calculated gaps)
		float xPosition = x * widthGap;
		float yPosition = y * heightGap;
				
		// If the row is odd we need to adjust the x gap by half
		if (y % 2 != 0) xPosition +=  widthGap / 2;
				
		// Create Vector3 to use when instantiating HexTile.
		return new Vector3(xPosition, yPosition, 0);
	}
	
	public Vector2Int GetGridPositionFromWorld(Vector3 worldPos)  
	{
		// Create Vector2 for Grid Coordinates, Get Y Coordinate
		Vector2Int gridPosition = new Vector2Int(0,0);
		gridPosition.y = Mathf.RoundToInt(worldPos.y / heightGap);
		
		// If Y is an odd row make sure to adjust the X back to account
		if (gridPosition.y % 2 == 0) 
		{ gridPosition.x = Mathf.RoundToInt(worldPos.x / widthGap); }
		else 
		{ gridPosition.x = Mathf.RoundToInt((worldPos.x / widthGap) - (widthGap / 2)); }

		// Test six neighbours of gridPosition (with oddRow check) in case its inaccurate due to hex
		bool oddRow = gridPosition.y % 2 ==1;
		List<Vector2Int> neighbourList = new List<Vector2Int> 
		{
			// Neighbours left and right
			gridPosition + new Vector2Int(-1, 0), gridPosition + new Vector2Int(+1, 0),
			// Neighbours above
			gridPosition + new Vector2Int(oddRow ? +1 : -1, +1), gridPosition + new Vector2Int(+0, +1),
			// Neighbours below
			gridPosition + new Vector2Int(oddRow ? +1 : -1, -1), gridPosition + new Vector2Int(+0, -1),
		};
		
		// Check distance is lowest to ensure correct grid position is set
		foreach (Vector2Int neighbour in neighbourList) 
		{
			// If distance from world to neighbour is lower than current closestPosition then set that as closestPosition.
			if (Vector2.Distance(new Vector2(worldPos.x, worldPos.y), GetWorldPositionFromGrid(neighbour.x, neighbour.y)) < 
				Vector2.Distance(new Vector2(worldPos.x, worldPos.y), GetWorldPositionFromGrid(gridPosition.x, gridPosition.y))) 
			{
				gridPosition = neighbour;
			}
		}
		
		return gridPosition;
	}
    #endregion
  
  
	#region Accessor Methods
	public HexTile GetHexTileAtPosition(int x, int y)  
	{
		return hexTileArray[x, y];
	}
	#endregion
  
    
    #region Helper Methods
	public bool IsMousOffGrid(Vector2Int gridPosition)  
	{
		// Safety check the x and y to not allow less than 0 or more than Max
		if (gridPosition.x < 0) return true;
		if (gridPosition.x >= rowLength) return true;
		if (gridPosition.y < 0) return true;
		if (gridPosition.y >= colHeight) return true;
		return false;
	}
    #endregion
}
