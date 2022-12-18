using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	// ------------------------------------------------------- START --------------------------------------------------------\\
	#region ----------------------------------------- GameController Variables ----------------------------------------------
	
	public static GameController Instance { get; private set; } // Singleton to allow GameController access game wide
	private HexTile hoveredHexTile; // Hovered Hex stored here to show visual
	private HexTile selectedHexTile; // Selected Hex stored here to show visual
	private ShipUnit hoveredShip; // Hovered Ship stored here to show visual
	private ShipUnit selectedShip; // Selected Ship stored here to show visual
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ------------------------------------------------------- START --------------------------------------------------------\\
    #region ----------------------------------------- GameController Lifecycle-----------------------------------------------
    
	private void Awake() { Instance = this; }
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
	// ------------------------------------------------------- START --------------------------------------------------------\\
	#region ---------------------------------------- GameController Accessors -----------------------------------------------
	
	// Selected Tile Methods
	public void SetSelectedHex(HexTile selectedHexTileObject) { selectedHexTile = selectedHexTileObject; }
	
	public HexTile GetSelectedHex() => selectedHexTile;
	
	public void SetHoveredHex(HexTile hoveredHextileObject) { hoveredHexTile = hoveredHextileObject; }
	
	public HexTile GetHoveredHex() => hoveredHexTile;
	
	// Selected Ship Methods
	public void SetSelectedShip(ShipUnit selectedShipObject) { selectedShip = selectedShipObject; }
	
	public ShipUnit GetSelectedShip() => selectedShip;
	
	public void SetHoveredShip(ShipUnit hoveredShipObject) { hoveredShip = hoveredShipObject; }
	
	public ShipUnit GetHoveredShip() => hoveredShip;
	
	#endregion --------------------------------------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------------------------------------------//
	
	
	
}
