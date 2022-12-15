using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	#region GameController Variables
	public static GameController Instance { get; private set; } // Singleton to allow GameController access game wide
	private HexTile selectedHexTile; // Selected hex stored here to show visual
	private ShipUnit hoveredShip; // Selected hex stored here to show visual
	private ShipUnit selectedShip; // Selected hex stored here to show visual
	#endregion
	
	
    #region GameController Lifecycle
	private void Awake() { Instance = this; }
	#endregion
	
	
	#region GameController Accessors
	// Selected Tile Methods
	public void SetSelectedHex(HexTile selectedHexTileObject) { selectedHexTile = selectedHexTileObject; }
	
	public HexTile GetSelectedHex() => selectedHexTile;
	
	public void SetSelectedShip(ShipUnit selectedShipObject) { selectedShip = selectedShipObject; }
	
	public ShipUnit GetSelectedShip() => selectedShip;
	
	public void SetHoveredShip(ShipUnit hoveredShipObject) { hoveredShip = hoveredShipObject; }
	
	public ShipUnit GetHoveredShip() => hoveredShip;
	#endregion
}
