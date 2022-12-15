using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSystem : MonoBehaviour
{
	#region MouseSystem Variables
	public static MouseSystem Instance { get; private set; } // Singleton to allow MouseSystem access game wide
	#endregion
	
	
    #region MouseSystem Lifecycle
	private void Awake() { Instance = this; }

	private void Update() 
	{
		// Get mouse position on grid and set selected hex tile
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2Int gridPosition = HexGrid.Instance.GetGridPositionFromWorld(mousePosition);
		
		// Check if off grid.
		if (!HexGrid.Instance.IsMousOffGrid(gridPosition)) 
		{
			// Then check is selectedHex has Ship Unit in it.
			HexTile hoveredHex = HexGrid.Instance.GetHexTileAtPosition(gridPosition.x, gridPosition.y);
			ShipUnit hoveredShipUnit = hoveredHex.GetShipUnit();
			ShipUnit selectedShipUnit = GameController.Instance.GetSelectedShip();
		
			if (hoveredShipUnit != null) 
			{
				// If so set Hovered Ship and not selected Hex.
				if (hoveredShipUnit != selectedShipUnit) {
					GameController.Instance.SetHoveredShip(hoveredShipUnit);
				}
				GameController.Instance.SetSelectedHex(null);
			} 
			else 
			{
				// Else select hex
				GameController.Instance.SetSelectedHex(hoveredHex);
				GameController.Instance.SetHoveredShip(null);
			}
			
			// If mouse Click and Hovered ship = true Set selected ship.
			if (Input.GetMouseButtonDown(0)) 
			{
				if (hoveredShipUnit != null) 
				{
					if (hoveredShipUnit != selectedShipUnit) {
						GameController.Instance.SetSelectedShip(hoveredShipUnit);
					} else  
					{
						GameController.Instance.SetSelectedShip(null);
					}
				}
					
				GameController.Instance.SetHoveredShip(null);
			}
	
		}
	}
	#endregion
}
