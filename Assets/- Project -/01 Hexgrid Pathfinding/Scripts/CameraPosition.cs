using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
	[SerializeField] private float xOffset;
	[SerializeField] private float yOffset;

    void Start()
    {
    	this.transform.position = new Vector3(xOffset, yOffset, this.transform.position.z);
    }
}
