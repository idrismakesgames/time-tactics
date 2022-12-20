using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class GameController : MonoBehaviour
{
	// ------------------------------------------------------- ##### --------------------------------------------------------\\
	#region ----------------------------------------- GameController Variables ----------------------------------------------
	
	public static GameController Instance { get; private set; } // Singleton to allow GameController access game wide
	
	public event EventHandler OnHoveredHexChange;
	public event EventHandler OnHoveredShipChange;
	public event EventHandler OnSelectedShipChange;
	
	private HexTile hoveredHexTile; // Hovered Hex stored here to show visual
	private HexTile selectedHexTile; // Selected Hex stored here to show visual
	private ShipUnit hoveredShip; // Hovered Ship stored here to show visual
	private ShipUnit selectedShip; // Selected Ship stored here to show visual
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ------------------------------------------------------- ##### --------------------------------------------------------\\
    #region ----------------------------------------- GameController Lifecycle-----------------------------------------------
    
	private void Awake() { Instance = this; }
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ------------------------------------------------------- ##### --------------------------------------------------------\\
	#region ---------------------------------------- GameController Accessors -----------------------------------------------
	
	// Selected Tile Methods
	public void SetSelectedHex(HexTile selectedHexTileObject) { selectedHexTile = selectedHexTileObject; }
	
	public HexTile GetSelectedHex() => selectedHexTile;
	
	public void SetHoveredHex(HexTile hoveredHextileObject) 
	{ 
		hoveredHexTile = hoveredHextileObject; 
		OnHoveredHexChange?.Invoke(hoveredHextileObject, EventArgs.Empty);
	}
	
	public HexTile GetHoveredHex() => hoveredHexTile;
	
	// Selected Ship Methods
	public void SetSelectedShip(ShipUnit selectedShipObject) 
	{
		selectedShip = selectedShipObject; 
		OnSelectedShipChange?.Invoke(selectedShipObject, EventArgs.Empty);
	}
	
	public ShipUnit GetSelectedShip() => selectedShip;
	
	public void SetHoveredShip(ShipUnit hoveredShipObject) 
	{
		hoveredShip = hoveredShipObject; 
		OnHoveredShipChange?.Invoke(hoveredShipObject, EventArgs.Empty);
	}
	
	public ShipUnit GetHoveredShip() => hoveredShip;
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
}
