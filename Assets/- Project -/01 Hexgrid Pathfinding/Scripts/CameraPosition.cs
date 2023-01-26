using UnityEngine;

public class CameraPosition : MonoBehaviour
{
	#region Variables
	[SerializeField] private float xOffset;
	[SerializeField] private float yOffset;
	#endregion
	
	#region Lifecycle
	private void Start()
    {
	    Transform transform1 = transform;
	    transform1.position = new Vector3(xOffset, yOffset, transform1.position.z);
    }
	#endregion
}
