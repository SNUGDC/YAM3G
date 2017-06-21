using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private void Start()
	{
        transform.LookAt(new Vector3 (0,0,0));
	}
}