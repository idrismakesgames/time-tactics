using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSystem : MonoBehaviour
{
	// --------------------------------------------------------- START ------------------------------------------------------\\
	#region -------------------------------------------- MouseSystem Variables ----------------------------------------------
	
	public static MouseSystem Instance { get; private set; } // Singleton to allow MouseSystem access game wide
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
	// --------------------------------------------------------- START ------------------------------------------------------\\
    #region -------------------------------------------- MouseSystem Lifecycle ----------------------------------------------
    
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
		
			HandleHover(hoveredHex, hoveredShipUnit, selectedShipUnit);
			HandClick(hoveredHex, hoveredShipUnit, selectedShipUnit);
		}
	}
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
	// --------------------------------------------------------- START ------------------------------------------------------\\
	#region ---------------------------------------------- MouseSystem Methods ----------------------------------------------
	
	private void HandleHover(HexTile hoveredHex, ShipUnit hoveredShipUnit, ShipUnit selectedShipUnit) 
	{
		if (hoveredShipUnit != null) 
		{
			// If so set hovered Ship and not selected Hex.
			if (hoveredShipUnit != selectedShipUnit) { GameController.Instance.SetHoveredShip(hoveredShipUnit); }
			GameController.Instance.SetHoveredHex(null);
		} 
		else 
		{
			// Else hover hex
			GameController.Instance.SetHoveredHex(hoveredHex);
			GameController.Instance.SetHoveredShip(null);
		}
	}
	
	private void HandClick(HexTile hoveredHex, ShipUnit hoveredShipUnit, ShipUnit selectedShipUnit) 
	{
		// If mouse Click and Hovered ship = true Set selected ship.
		if (Input.GetMouseButtonDown(0)) 
		{
			if (hoveredShipUnit != null) { 
				GameController.Instance.SetSelectedShip(hoveredShipUnit != selectedShipUnit ? hoveredShipUnit : null);
			} else if (selectedShipUnit != null)
			{
				selectedShipUnit.SetShipMoveTarget(hoveredHex);
			}
			
			GameController.Instance.SetHoveredShip(null);
		}
	}
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
}
