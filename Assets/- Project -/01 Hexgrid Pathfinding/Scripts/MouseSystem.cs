using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class MouseSystem : MonoBehaviour
{
	#region Variables 
	// ReSharper disable once UnusedAutoPropertyAccessor.Local
	private static MouseSystem Instance { get; set; } // Singleton to allow MouseSystem access game wide
	private Path path; // Current path to show based on hovering
	
	[SerializeField] private LayerMask mousePlaneLayerMask; // Layer that the ships are on for collision detection

	private Vector3 mousePosition; // Mouse position on screen
	private Vector2Int gridPosition; // Grid position of mouse if valid
	private ShipUnit hoveredShip; // If hovering over a ship store here otherwise set to null (stored in game controller too)
	private ShipUnit selectedShip; // If clicked on a ship store this here (stored in GameController too)
	private HexTile hoveredHex; // If hovering over a hex set to here otherwise null (stored in game controller too)
	
	private List<Vector2Int> validHexMovePositionList; // Where can a ship move to based on its range will be stored here for use
	
	private Seeker seeker; // Pathfinding helper that will generate path between targets for us.
	#endregion

	#region Lifecycle 
	private void Awake() 
	{ 
		Instance = this; 
		validHexMovePositionList = new List<Vector2Int>();
	}
	
	private void Start() 
	{
		// Seeker that will activate the pathfinding from the grid object in the scene.
		seeker = GetComponent<Seeker>();
		// Subscribe to the event when a hovered hex changes from GameController
		GameController.Instance.OnHoveredHexChange += GameController_OnHoveredHexChange;
	}

	private void FixedUpdate() 
	{
		// Get mouse position on screen, get GridPosition and then store selected ship if there is one
		if (Camera.main != null) mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		gridPosition = HexGrid.Instance.GetGridPositionFromWorld(mousePosition);
		selectedShip = GameController.Instance.GetSelectedShip();
		
		// Get Valid Grid position list for selected Unit
		HexGrid.Instance.HideAllSelectedHexes();
		if(selectedShip != null) 
		{
			validHexMovePositionList = selectedShip.GetMoveAction().GetValidHexPositionList();
			HexGrid.Instance.ShowSelectedHexPositions(validHexMovePositionList);
		}
		
		// cast a ray over current mouse position to see if we are hitting a ship
		if (!HexGrid.Instance.IsInvalidGridPosition(gridPosition)) {
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
				if (GameController.Instance.GetHoveredShip() != hoveredShip) GameController.Instance.SetHoveredShip(hoveredShip); 
			} else 
			{
				// Otherwise show the selected graphic
				GameController.Instance.SetHoveredShip(null); 
			}
			// Dont show any hex as hovering as over ship right now.
			GameController.Instance.SetHoveredHex(null);
		} 
		else 
		{
			// Else hover hex
			if (GameController.Instance.GetHoveredHex() != hoveredHex && !hoveredHex.GetIsObstacle()) 
			{
				GameController.Instance.SetHoveredHex(hoveredHex);
			}
			
			if (GameController.Instance.GetHoveredShip() != null) 
			{
				GameController.Instance.SetHoveredShip(null);
			}
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
				if (validHexMovePositionList.Contains(gridPosition)) 
				{
					// Only set if clicking within valid position list.
					selectedShip.GetMoveAction().SetShipMoveTarget(hoveredHex, path);
				}
			}
			// Deselect ship hover after click not matter what.
			GameController.Instance.SetHoveredShip(null);
		}
	}
	#endregion
	
	
	
	#region MouseSystem Events	
	private void GameController_OnHoveredHexChange(object sender, EventArgs empty) 
	{
		// If hovering over valid hex position list entry, generate path.
		HexTile senderTile = (HexTile) sender;
		if (senderTile) {
			// if there is a selected ship and a valid hex position is present generate path.
			if (selectedShip != null && validHexMovePositionList.Contains(new Vector2Int((int)senderTile.GetGridPosition().x, (int)senderTile.GetGridPosition().y))) 
			{
				// Generate path using seeker and store
				seeker.StartPath(selectedShip.GetShipWorldPosition(), senderTile.GetWorldPosition(), OnPathComplete);
			}
		}
	}
	
	private void OnPathComplete(Path p)
	{
		if (!p.error) 
		{
			path = p;
		}
	}
	#endregion
	
}
