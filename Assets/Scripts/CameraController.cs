using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector2 firstPressPos;
	public Vector2 secondPressPos;

	private void Start()
	{
		transform.LookAt(new Vector3 (0,0,0));
	}

	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{

		}
	}
}