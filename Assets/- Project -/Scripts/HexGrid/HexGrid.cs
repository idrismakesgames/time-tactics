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
				// Set the position of the tile based on Grid Coordinate. (Using calculated gaps)
				float xPosition = x * widthGap;
				float yPosition = y * heightGap;
				
				// If the row is odd we need to adjust the x gap by half
				if (y % 2 != 0) xPosition +=  widthGap / 2;
				
				// Create Vector3 to use when instantiating HexTile.
				Vector3 tilePosition = new Vector3(xPosition, yPosition, 0);
				
				// Instantiate HexTile Prefab using Ref, Set Values + Name, and Attach to Grid as Child
				HexTile hexTileInstance = Instantiate(hexTileObject, tilePosition,  Quaternion.identity);
				hexTileInstance.SetStartingValues(hexTileInstance.gameObject, new Vector2(xPosition, yPosition), new Vector2(x, y));
				hexTileInstance.name = $"HexTile {x}-{y}";
				hexTileInstance.transform.parent =  this.transform;
				
				// Add this instance to the two dimensional array for later reference
				hexTileArray[x, y] = hexTileInstance;
			}
		}
	}   
    #endregion
}
