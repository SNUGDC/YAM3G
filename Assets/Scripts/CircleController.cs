using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour
{
	public Sprite[] CircleSprite;
	public static Vector2 movePos;


	private void Update()
	{
		//ScaleAdjustment();
	}

	public void ScaleAdjustment()
	{
		switch((int)transform.position.z)
		{
			case 9:
				transform.localScale = new Vector3(1, 1, 1);
				break;
			case 3:
				transform.localScale = new Vector3(0.75f, 0.75f, 1);
				break;
			case -3:
				transform.localScale = new Vector3(0.5f, 0.5f, 1);
				break;
			case -9:
				transform.localScale = new Vector3(0.25f, 0.25f, 1);
				break;
		}
	}

	private void OnMouseDown()
	{
		FieldController.isMovingState = true;
		movePos = new Vector2 ((transform.position.x + 9) / 6, (transform.position.y + 9) / 6);
		Debug.Log(((transform.position.x + 9) / 6) + ", " + ((transform.position.y + 9) / 6));
	}
}