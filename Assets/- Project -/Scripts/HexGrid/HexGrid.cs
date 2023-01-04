using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    #region HexGrid Variables
	public static HexGrid Instance { get; private set; } // Singleton to allow HexGrid access game wide
	
	public bool GenerateTheGrid;
	
	[SerializeField] private HexTile hexTileObject;  // HexTile object to get sprite info from
	[SerializeField] private float widthOffset; // Row spacing for hex
	[SerializeField] private float heightOffset; // Column spacing for hex
	[SerializeField] private int rowLength; // Number of rows in HexGrid
	[SerializeField] private int colHeight; // Number of Columns in HexGrid
	#endregion
	
	
    #region HexGrid Private Variables
	private HexGridMethods hexGridMethods; // Grid methods asset to help clean code
	public HexTile[,] hexTileArray; // Holds reference to every tile in the HexGrid
	
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
		
		Debug.Log("xBounds: " + hexTileSprite.bounds.extents.x + " yBounds: " + hexTileSprite.bounds.extents.y + " widthOffset: " + widthOffset + " heightOffset: " + heightOffset);
	
		// Instance the array that will hold all the HexGrid Tiles
		hexTileArray = new HexTile[rowLength, colHeight];
		
		if (GenerateTheGrid) GenerateHexGrid();
		else				 ReadInGrid();
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
				hexTileInstance.SetStartingValues(new Vector2(tilePosition.x, tilePosition.y), new Vector2Int(x, y));
				hexTileInstance.name = $"HexTile {x}-{y}";
				hexTileInstance.transform.parent =  this.transform;
				
				// Add this instance to the two dimensional array for later reference
				hexTileArray[x, y] = hexTileInstance;
			}
		}
	}   
	
	private void ReadInGrid() 
	{
		// READ In Grid already Spawned
		for (int x = 0; x < rowLength; x++) 
		{
			for (int y = 0; y < colHeight; y++) 
			{
				// Store hexTile that is currently a child in each correct slot.
				HexTile[] childTiles = this.GetComponentsInChildren<HexTile>();
				hexTileArray[x, y] = childTiles[x*20 + y];
			}
		}
	}
	
	public void ShowSelectedHexPositions(List<Vector2Int> validHexPositionList)
	{
		// Go through list of valid positions andset the alpha on the HexTile Sprite to full
		foreach (Vector2Int validGridPosition in validHexPositionList)
		{
			hexTileArray[validGridPosition.x, validGridPosition.y].ShowValidHex();
		}
	}
	
	public void HideAllSelectedHexes() 
	{
		// Go through list of all Hexes set the alpha on the HexTile Sprite to default
		for (int x = 0; x < GetRowLength(); x++) 
		{
			for (int y = 0; y < GetColHeight(); y++) {
				hexTileArray[x, y].HideValidHex();
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
	
	public bool IsInvalidGridPosition(Vector2Int gridPosition) => hexGridMethods.IsInvalidGridPosition(gridPosition);
	#endregion 
	
	
	
} 
