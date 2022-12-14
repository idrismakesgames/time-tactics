using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	#region Variables
	public static GameController Instance { get; private set; } // Singleton to allow GameController access gamewide
	[SerializeField] HexGrid hexGrid;
	private HexTile selectedHexTile; // Selected hex stored here to show visual
	#endregion
	
	
    #region Lifecycle Methods
	private void Awake()
	{
		if (Instance!= null) 
		{
			Debug.LogError("More than on GameController! " + transform + " - " + Instance);
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}
	
	private void Update() 
	{
		// Get mouse position on grid and set selected hex tile
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2Int gridPosition = hexGrid.GetGridPositionFromWorld(mousePosition);
		GameController.Instance.SetSelectedHex(hexGrid.IsMousOffGrid(gridPosition) ? null : hexGrid.GetHexTileAtPosition(gridPosition.x, gridPosition.y));
	}
	#endregion
	
	
    #region Accessor Methods
	public void SetSelectedHex(HexTile selectedHex) 
	{
		selectedHexTile = selectedHex;
	}
	
	public HexTile GetSelectedHex() 
	{
		return selectedHexTile;
	}
	#endregion
}
