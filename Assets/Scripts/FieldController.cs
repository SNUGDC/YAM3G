using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
	public enum SwipeVector
	{
		Right, Left, Up, Down
	}

	public static bool isMovingState;
	public GameObject[] CirclePrefabs;

	private GameObject[] Circles;
	private int?[,,] Field;

	private void Start()
	{
		Field = new int?[4, 4, 4];
		Circles = new GameObject[64];
		isMovingState = false;

		InitializeField();
		Print();
	}

	private void Update()
	{
		if(isMovingState == false)
		{
			if(Input.GetKeyUp(KeyCode.RightArrow))
			{
				Rotate(SwipeVector.Right);
			}
			else if(Input.GetKeyUp(KeyCode.LeftArrow))
			{
				Rotate(SwipeVector.Left);
			}
			else if(Input.GetKeyUp(KeyCode.UpArrow))
			{
				Rotate(SwipeVector.Up);
			}
			else if(Input.GetKeyUp(KeyCode.DownArrow))
			{
				Rotate(SwipeVector.Down);
			}
		}
		else
		{
			if(Input.GetKeyUp(KeyCode.RightArrow))
			{
				Move(SwipeVector.Right);
			}
			else if(Input.GetKeyUp(KeyCode.LeftArrow))
			{
				Move(SwipeVector.Left);
			}
			else if(Input.GetKeyUp(KeyCode.UpArrow))
			{
				Move(SwipeVector.Up);
			}
			else if(Input.GetKeyUp(KeyCode.DownArrow))
			{
				Move(SwipeVector.Down);
			}
		}

		if(Input.GetKeyDown(KeyCode.B))
		{
			CheckAndBoom();
		}
		else if(Input.GetKeyDown(KeyCode.D))
		{
			ComeDown();
		}
		else if(Input.GetKeyDown(KeyCode.R))
		{
			Refill();
		}
	}
	private void InitializeField()
	{
		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					Field[x, y, z] = Random.Range(0, 4);
				}
			}
		}
	}

	private void Print()
	{
		foreach(GameObject circle in Circles)
		{
			Destroy(circle);
		}

		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					if(Field[x,y,z].HasValue)
						Circles[x + 4 * y + 16 * z] = Instantiate(CirclePrefabs[Field[x, y, z].Value], new Vector3 (6*x-9, 6*y-9, 6*z-9), new Quaternion(0, 0, 0, 0));
				}
			}
		}

		foreach(GameObject circle in Circles)
		{
			if(circle != null)
				circle.GetComponent<CircleController>().ScaleAdjustment();
		}
	}

	private void Rotate(SwipeVector dir)
	{
		int[,,] TempField = new int[4,4,4];
		
		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					TempField[x,y,z] = Field[x,y,z].Value;
				}
			}
		}

		switch(dir)
		{
			case SwipeVector.Right:
				for(int x = 0; x < 4; x++)
				{
					for(int y = 0; y < 4; y++)
					{
						for(int z = 0; z < 4; z++)
						{
							Field[3-z, y, x] = TempField[x, y, z];
						}
					}
				}
			break;
			case SwipeVector.Left:
				for(int x = 0; x < 4; x++)
				{
					for(int y = 0; y < 4; y++)
					{
						for(int z = 0; z < 4; z++)
						{
							Field[z, y, 3-x] = TempField[x, y, z];
						}
					}
				}
			break;
			case SwipeVector.Up:
				for(int x = 0; x < 4; x++)
				{
					for(int y = 0; y < 4; y++)
					{
						for(int z = 0; z < 4; z++)
						{
							Field[x, 3-z, y] = TempField[x, y, z];
						}
					}
				}
			break;
			case SwipeVector.Down:
				for(int x = 0; x < 4; x++)
				{
					for(int y = 0; y < 4; y++)
					{
						for(int z = 0; z < 4; z++)
						{
							Field[x, z, 3-y] = TempField[x, y, z];
						}
					}
				}
			break;
		}

		Print();
		TempField = new int [4, 4, 4];
	}

	private void Move(SwipeVector dir)
	{
		int[,,] TempField = new int[4,4,4];
		int[] movePos = new int[2] {Mathf.RoundToInt(CircleController.movePos.x), Mathf.RoundToInt(CircleController.movePos.y)};
		
		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					TempField[x,y,z] = Field[x,y,z].Value;
				}
			}
		}

		switch(dir)
		{
			case SwipeVector.Right:
				if(movePos[0] >= 3)
					return;
				for(int z = 0; z < 4; z++)
				{
					Field[movePos[0] + 1, movePos[1], z] = TempField[movePos[0], movePos[1], z];
					Field[movePos[0], movePos[1], z] = TempField[movePos[0] + 1, movePos[1], z];
				}
			break;
			case SwipeVector.Left:
				if(movePos[0] <= 0)
					return;
				for(int z = 0; z < 4; z++)
				{
					Field[movePos[0] - 1, movePos[1], z] = TempField[movePos[0], movePos[1], z];
					Field[movePos[0], movePos[1], z] = TempField[movePos[0] - 1, movePos[1], z];
				}
			break;
			case SwipeVector.Up:
				if(movePos[1] >= 3)
					return;
				for(int z = 0; z < 4; z++)
				{
					Field[movePos[0], movePos[1] + 1, z] = TempField[movePos[0], movePos[1], z];
					Field[movePos[0], movePos[1], z] = TempField[movePos[0], movePos[1] + 1, z];
				}
			break;
			case SwipeVector.Down:
				if(movePos[1] <= 0)
					return;
				for(int z = 0; z < 4; z++)
				{
					Field[movePos[0], movePos[1] - 1, z] = TempField[movePos[0], movePos[1], z];
					Field[movePos[0], movePos[1], z] = TempField[movePos[0], movePos[1] - 1, z];
				}
			break;
		}

		Print();
		TempField = new int [4, 4, 4];
		isMovingState = false;
	}

	enum BoomResult
	{
		Boom,
		UnBoom
	}

	private void CheckAndBoom()
	{
		BoomResult[,,] boomField = new BoomResult[4,4,4];
		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					boomField[x,y,z] = BoomResult.UnBoom;
				}
			}
		}

		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					CheckXAxis(x, y, z, boomField);
					CheckYAxis(x, y, z, boomField);
					CheckZAxis(x, y, z, boomField);
				}
			}
		}

		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					if(boomField[x, y, z] == 0)
					{  
						Field[x,y,z] = null;
						Print();
					}
				}
			}
		}
	}

	private void CheckXAxis(int x, int y, int z, BoomResult [,,] boomField) //x축으로 확인
	{
		//오른오른쪽의 공이 필드에 속해 있는지 확인
		if(x + 2 >= Field.GetLength(0))
			return;

		//오른쪽에 있는 애랑 색이 같지 않다면 끝낸다.
		if(Field[x, y, z] != Field[x+1, y, z])
		{
			if(boomField[x,y,z] == BoomResult.Boom)
				return;
			boomField[x,y,z] = BoomResult.UnBoom;
			return;
		}
		//오른쪽에 있는 애랑 색이 같지만 오른오른쪽에 있는 애랑 색이 다르면
		if(Field[x, y, z] != Field[x+2, y, z])
		{
			if(boomField[x, y, z] == BoomResult.Boom)
				return;
			boomField[x, y, z] = BoomResult.UnBoom;
			return;
		}
		//오른쪽과 오른오른쪽에 있는애랑 색이 모두 같다면
		boomField[x, y, z] = BoomResult.Boom;
		boomField[x+1, y, z] = BoomResult.Boom;
		boomField[x+2, y, z] = BoomResult.Boom;
	}

	private void CheckYAxis(int x, int y, int z, BoomResult [,,] boomField) //y축으로 확인
	{
		//위위쪽의 공이 필드에 속해 있는지 확인
		if(y + 2 >= Field.GetLength(1))
			return;

		//위쪽에 있는 애랑 색이 같지 않다면 끝낸다.
		if(Field[x, y, z] != Field[x, y+1, z])
		{
			if(boomField[x,y,z] == BoomResult.Boom)
				return;
			boomField[x,y,z] = BoomResult.UnBoom;
			return;
		}
		//위쪽에 있는 애랑 색이 같지만 위위쪽에 있는 애랑 색이 다르면
		if(Field[x, y, z] != Field[x, y+2, z])
		{
			if(boomField[x, y, z] == BoomResult.Boom)
				return;
			boomField[x, y, z] = BoomResult.UnBoom;
			return;
		}
		//위쪽과 위위쪽에 있는애랑 색이 모두 같다면
		boomField[x, y, z] = BoomResult.Boom;
		boomField[x, y+1, z] = BoomResult.Boom;
		boomField[x, y+2, z] = BoomResult.Boom;
	}

	private void CheckZAxis(int x, int y, int z, BoomResult [,,] boomField) //z축으로 확인
	{
		//뒤뒤쪽의 공이 필드에 속해 있는지 확인
		if(z + 2 >= Field.GetLength(2))
			return;

		//뒤쪽에 있는 애랑 색이 같지 않다면 끝낸다.
		if(Field[x, y, z] != Field[x, y, z+1])
		{
			if(boomField[x,y,z] == BoomResult.Boom)
				return;
			boomField[x,y,z] = BoomResult.UnBoom;
			return;
		}
		//뒤쪽에 있는 애랑 색이 같지만 뒤뒤쪽에 있는 애랑 색이 다르면
		if(Field[x, y, z] != Field[x, y, z+2])
		{
			if(boomField[x, y, z] == BoomResult.Boom)
				return;
			boomField[x, y, z] = BoomResult.UnBoom;
			return;
		}
		//뒤쪽과 뒤뒤쪽에 있는애랑 색이 모두 같다면
		boomField[x, y, z] = BoomResult.Boom;
		boomField[x, y, z+1] = BoomResult.Boom;
		boomField[x, y, z+2] = BoomResult.Boom;
	}

	private void ComeDown()
	{
		for(int x = 0; x < 4; x++)
		{
			for(int z = 0; z < 4; z++)
			{
				ComeDownOneAxis(x, z);
			}
		}

		Print();
	}

	private void ComeDownOneAxis(int x, int z)
	{
		for(int y = 0; y < Field.GetLength(1); y++)
		{
			if(Field[x, y, z] == null)
			{
				for(int above = 1; y + above < Field.GetLength(1); above++)
				{
					if(Field[x, y + above, z].HasValue)
					{
						Field[x, y, z] = Field[x, y + above, z];
						Field[x, y+above, z] = null;
						break;
					}
				}
			}
		}
	}

	private void Refill()
	{
		for(int x = 0; x < 4; x++)
		{
			for(int y = 0; y < 4; y++)
			{
				for(int z = 0; z < 4; z++)
				{
					if(Field[x, y, z].HasValue == false)
					{
						Field[x, y, z] = Random.Range(0, 4);
					}
				}
			}
		}

		Print();
	}
}