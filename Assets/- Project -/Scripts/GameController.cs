using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	#region Variables
	public static GameController Instance { get; private set; } // Singleton to allow GameController access gamewide
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
