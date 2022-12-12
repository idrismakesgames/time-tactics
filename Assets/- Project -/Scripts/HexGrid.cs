using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
	[SerializeField] private GameObject hexTileObject;
	[SerializeField] private float widthOffset; // Row spacing for hex
	[SerializeField] private float heightOffset; // Column spacing for hex
	
	private Sprite hexTileSprite;
	private float widthGap; // Horizontal spacing based on Sprite
	private float heightGap; // Vertical spacing based on Sprite
	
	private List<GameObject> tileList;
	
    void Start()
	{
		hexTileSprite = hexTileObject.GetComponent<SpriteRenderer>().sprite; 
		widthGap = (hexTileSprite.bounds.extents.x * 2f) + widthOffset;
		heightGap = (hexTileSprite.bounds.extents.y * 1.5f) + heightOffset;
		
		Debug.Log($"widthGap: {widthGap} heightGap: {heightGap}");
		
		//private HexTile[,] hexTileArray; TODO: Make Object and Array
		int rowLength = 30;
		int colHeight = 20;	
	
		for (int y = 0; y < colHeight; y++) 
		{
			for (int x = 0; x < rowLength; x++) 
			{
				float xPosition = x * widthGap;
				float yPosition = y * heightGap;
				if (y % 2 != 0) 
				{
					xPosition +=  widthGap / 2;
				}
				Vector3 tilePosition = new Vector3(xPosition, yPosition, 0);
				Instantiate(hexTileObject, tilePosition,  Quaternion.identity);
			}
		}
    }
    
    void Update()
	{
    	
    }   
}
