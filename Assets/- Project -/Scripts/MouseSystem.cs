using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSystem : MonoBehaviour
{
	#region MouseSystem Variables 
	public static MouseSystem Instance { get; private set; } // Singleton to allow MouseSystem access game wide
	
	[SerializeField] private LayerMask mousePlaneLayerMask;
	#endregion 
	
	

	#region Private Variables 
	private Vector3 mousePosition;
	private Vector2Int gridPosition;
	private ShipUnit hoveredShip;
	private ShipUnit selectedShip;
	private HexTile hoveredHex;
	#endregion 
	
	
	
    #region MouseSystem Lifecycle 
	private void Awake() 
	{ 
		Instance = this; 
	}

	private void FixedUpdate() 
	{
		// Get mouse position on screen, get gridposition and then store selected ship if there is one
		mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		gridPosition = HexGrid.Instance.GetGridPositionFromWorld(mousePosition);
		selectedShip = GameController.Instance.GetSelectedShip();
		
		// cast a ray over current mouse position to see if we are hitting a ship
		if (!HexGrid.Instance.IsValidGridPosition(gridPosition)) {
			RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, float.MaxValue, mousePlaneLayerMask);
			
			// if so store the hit ship in the hoveredShip variable
			if(hit.collider != null)
			{
				hoveredShip = hit.collider.gameObject.TryGetComponent(out ShipUnit ship) ? ship : null; 
			} 
			else hoveredShip = null;
			
			// regardless of if there is a ship get the HexTile at this mouse position.
			hoveredHex = HexGrid.Instance.GetHexTileAtPosition(gridPosition.x, gridPosition.y);
		}
	}
	
	private void Update() 
	{
		// Then check is selectedHex has Ship Unit in it.
		HandleHover();
		HandClick();
	}
	#endregion
	
	
	
	#region MouseSystem Methods
	private void HandleHover() 
	{
		// If hovering over ship
		if (hoveredShip != null) 
		{
			// If not hovering over selected ship, set the hover graphic
			if (hoveredShip != selectedShip) { 
				GameController.Instance.SetHoveredShip(hoveredShip); 
			} else 
			{
				// Otherwise show the selected graphic
				GameController.Instance.SetHoveredShip(null); ;
			}
			// Dont show any hex as hovering as over ship right now.
			GameController.Instance.SetHoveredHex(null);
		} 
		else 
		{
			// Else hover hex
			GameController.Instance.SetHoveredHex(hoveredHex);
			GameController.Instance.SetHoveredShip(null);
		}
	}
	
	private void HandClick() 
	{
		// If mouse Click and Hovered ship = true Set selected ship.
		if (Input.GetMouseButtonDown(0)) 
		{
			if (hoveredShip != null) { 
				GameController.Instance.SetSelectedShip(hoveredShip != selectedShip ? hoveredShip : null);
			} else if (selectedShip != null)
			{
				// if clicking while not hovering a ship, mean you are clicking on tile. so set move target.
				selectedShip.GetMoveAction().SetShipMoveTarget(hoveredHex);
			}
			// Deselct ship hover after click not matter what.
			GameController.Instance.SetHoveredShip(null);
		}
	}
	#endregion
	
	
	
}
