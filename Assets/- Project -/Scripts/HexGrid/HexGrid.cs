using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
	public static HexGrid Instance { get; private set; } // Singleton to allow HexGrid access game wide
	
    #region HexGrid Editable Vars
	[SerializeField] private HexTile hexTileObject;
	[SerializeField] private float widthOffset; // Row spacing for hex
	[SerializeField] private float heightOffset; // Column spacing for hex
	[SerializeField] private int rowLength; // Number of rows in HexGrid
	[SerializeField] private int colHeight; // Number of Columns in HexGrid
	#endregion
	
    #region HexGrid Private Vars
	private HexGridMethods hexGridMethods; // Grid methods asset to help clean code
	private HexTile[,] hexTileArray; // Holds reference to every tile in the HexGrid
	
	private Sprite hexTileSprite; // Sprite for hexTile to get gaps
	private float widthGap; // Horizontal spacing based on Sprite
	private float heightGap; // Vertical spacing based on Sprite
	#endregion
	
    #region HexGrid Lifecycle
	private void Awake() 
	{ 
		Instance = this; 
		
		// Assign HexGrid Methods and Sprite from attached components
		hexGridMethods = GetComponent<HexGridMethods>();
		hexTileSprite = hexTileObject.GetComponent<SpriteRenderer>().sprite; 
	}

	private void Start()
	{
		// Set the sprite to get the width and height for grid generation
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
    #endregion
  
	#region HexGrid Accessors
	// Grid Gap Methods
	public float GetWidthGap() => widthGap;
	
	public float GetHeightGap() => heightGap;
	
	public int GetRowLength() => rowLength;
	
	public int GetColHeight() => colHeight;

	// Grid Position Methods
	public HexTile GetHexTileAtPosition(int x, int y)   => hexTileArray[x, y];
	
	public Vector2Int GetGridPositionFromWorld(Vector3 worldPos) => hexGridMethods.GetGridPositionFromWorld(worldPos);
	
	public Vector3 GetWorldPositionFromGrid(int x, int y) => hexGridMethods.GetWorldPositionFromGrid(x, y);
	
	public bool IsMousOffGrid(Vector2Int gridPosition) => hexGridMethods.IsMousOffGrid(gridPosition);
	#endregion
}
