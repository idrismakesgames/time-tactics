using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridMethods : MonoBehaviour
{
	public Vector3 GetWorldPositionFromGrid(int x, int y)  
	{
		// Set the position of the tile based on Grid Coordinate. (Using calculated gaps)
		float xPosition = x * HexGrid.Instance.GetWidthGap();
		float yPosition = y * HexGrid.Instance.GetHeightGap();
				
		// If the row is odd we need to adjust the x gap by half
		if (y % 2 != 0) xPosition +=  HexGrid.Instance.GetWidthGap() / 2;
				
		// Create Vector3 to use when instantiating HexTile.
		return new Vector3(xPosition, yPosition, 0);
	}
	
	public Vector2Int GetGridPositionFromWorld(Vector3 worldPos)  
	{
		// Create Vector2 for Grid Coordinates, Get Y Coordinate
		Vector2Int gridPosition = new Vector2Int(0,0);
		gridPosition.y = Mathf.RoundToInt(worldPos.y / HexGrid.Instance.GetHeightGap());
		
		// If Y is an odd row make sure to adjust the X back to account
		if (gridPosition.y % 2 == 0) 
		{ 
			gridPosition.x = Mathf.RoundToInt(worldPos.x / HexGrid.Instance.GetWidthGap()); 
		}
		else 
		{ 
			gridPosition.x = Mathf.RoundToInt((worldPos.x / HexGrid.Instance.GetWidthGap()) - (HexGrid.Instance.GetWidthGap() / 2)); 
		}

		// Test six neighbours of gridPosition (with oddRow check) in case its inaccurate due to hex
		bool oddRow = gridPosition.y % 2 ==1;
		List<Vector2Int> neighbourList = new List<Vector2Int> 
		{
			// Neighbours left and right
			gridPosition + new Vector2Int(-1, 0), 
			gridPosition + new Vector2Int(+1, 0),
			// Neighbours above
			gridPosition + new Vector2Int(oddRow ? +1 : -1, +1), 
			gridPosition + new Vector2Int(+0, +1),
			// Neighbours below
			gridPosition + new Vector2Int(oddRow ? +1 : -1, -1), 
			gridPosition + new Vector2Int(+0, -1),
		};
		
		// Check distance is lowest to ensure correct grid position is set
		foreach (Vector2Int neighbour in neighbourList) 
		{
			// If distance from world to neighbour is lower than current closestPosition then set that as closestPosition.
			if (Vector2.Distance(new Vector2(worldPos.x, worldPos.y), GetWorldPositionFromGrid(neighbour.x, neighbour.y)) < 
				Vector2.Distance(new Vector2(worldPos.x, worldPos.y), GetWorldPositionFromGrid(gridPosition.x, gridPosition.y))) 
			{
				gridPosition = neighbour;
			}
		}
		
		return gridPosition;
	}
	
	public bool IsInvalidGridPosition(Vector2Int gridPosition)  
	{
		// Safety check the x and y to not allow less than 0 or more than Max
		if (gridPosition.x < 0) return true;
		if (gridPosition.x >= HexGrid.Instance.GetRowLength()) return true;
		if (gridPosition.y < 0) return true;
		if (gridPosition.y >= HexGrid.Instance.GetColHeight()) return true;
		return false;
	}
}
