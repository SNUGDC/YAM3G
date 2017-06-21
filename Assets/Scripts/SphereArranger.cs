using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereArranger : MonoBehaviour
{
	public GameObject[] sphere;
	public GameObject[] spherePrefab;

    private GameObject Arcball;
	private Vector3[] bundleToCheck;

	private void Start()
	{
        Arcball = GameObject.Find("Arcball");
        sphere = new GameObject[64];
		bundleToCheck = new Vector3[96];
		AssignBundleToCheck();

		for(int i = 0; i <= 63; i++)
		{
			CreateSphere(i);
		}
	}

	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.C))
		{
			foreach(Vector3 spheresToCheck in bundleToCheck)
			{
				Check3AndDestroy(spheresToCheck);
			}
		}
		if(Input.GetKeyUp(KeyCode.V))
		{
			ComeDown();
		}
		if(Input.GetKeyUp(KeyCode.B))
		{
			RefillSphere();
		}
	}

	private void CreateSphere(int num)
	{
		int rand = Random.Range(0, 4);
		int x;
		int y;
		int z;

		z = num / 16;
		y = (num % 16) / 4;
		x = (num % 16) % 4;

		sphere[num] = Instantiate(spherePrefab[rand], new Vector3(x - 1.5f, y - 1.5f, z - 1.5f), new Quaternion(0, 0, 0, 0), Arcball.transform);
	}

	private void Check3AndDestroy(Vector3 spheresToCheck) //3개 인접한 것이 있는지 확인하고 같으면 부순다!
	{
		if(sphere[(int)spheresToCheck.x].tag != sphere[(int)spheresToCheck.y].tag)
			return;
		if(sphere[(int)spheresToCheck.y].tag != sphere[(int)spheresToCheck.z].tag)
			return;
		
		Destroy(sphere[(int)spheresToCheck.x]);
		Destroy(sphere[(int)spheresToCheck.y]);
		Destroy(sphere[(int)spheresToCheck.z]);
	}

	private GameObject GetSphere(int x, int y, int z)
	{
		return sphere[16 * z + 4 * y + x];
	}

	private void ComeDown() //터진 자리를 위의 공들이 내려와서 채운다.
	{
		for(int i = 0; i <= 15; i++)
		{
			CheckForComeDown(i);
		}
	}

	private void RefillSphere()
	{
		for(int i = 0; i < sphere.Length; i++)
		{
			if(sphere[i] == null)
			{
				CreateSphere(i);
			}
		}
	}

	private void CheckForComeDown(int i)
	{
		if(i >= 16)
		{
			Debug.Log("CheckForComeDown 함수에 i가 16이상의 숫자가 들어왔어영");
			return;
		}

		int[] sphereState;
		int sphereStateInBinary;

		sphereState = new int[4] {0, 0, 0, 0};
		for(int stair = 0; stair <= 3; stair++)
		{
			if(sphere[i + stair * 16] != null)
			{
				sphereState[stair] = 1;
			}
		}
		sphereStateInBinary = sphereState[0] + sphereState[1] * 2 + sphereState[2] * 4 + sphereState[3] * 8;

		switch(sphereStateInBinary)
		{
			case 0:
			break;
			case 1:
			break;
			case 2:
				sphere[i] = Instantiate(sphere[i + 16], sphere[i + 16].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 16]); //2->1
			break;
			case 3:
			break;
			case 4:
				sphere[i] = Instantiate(sphere[i + 32], sphere[i + 32].transform.position - new Vector3(0, 0, 2), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 32]); //3->1
			break;
			case 5:
				sphere[i + 16] = Instantiate(sphere[i + 32], sphere[i + 32].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 32]); //3->2
			break;
			case 6:
				sphere[i] = Instantiate(sphere[i + 16], sphere[i + 16].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 16]); //2->1
				sphere[i + 16] = Instantiate(sphere[i + 32], sphere[i + 32].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 32]); //3->2
			break;
			case 7:
			break;
			case 8:
				sphere[i] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 3), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->1
			break;
			case 9:
				sphere[i + 16] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 2), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->2
			break;
			case 10:
				sphere[i] = Instantiate(sphere[i + 16], sphere[i + 16].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 16]); //2->1
				sphere[i + 16] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 2), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->2
			break;
			case 11:
				sphere[i + 32] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->3
			break;
			case 12:
				sphere[i] = Instantiate(sphere[i + 32], sphere[i + 32].transform.position - new Vector3(0, 0, 2), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 32]); //3->1
				sphere[i + 16] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 2), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->2
			break;
			case 13:
				sphere[i + 16] = Instantiate(sphere[i + 32], sphere[i + 32].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 32]); //3->2
				sphere[i + 32] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->3
			break;
			case 14:
				sphere[i] = Instantiate(sphere[i + 16], sphere[i + 16].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 16]); //2->1
				sphere[i + 16] = Instantiate(sphere[i + 32], sphere[i + 32].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 32]); //3->2
				sphere[i + 32] = Instantiate(sphere[i + 48], sphere[i + 48].transform.position - new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
				Destroy(sphere[i + 48]); //4->3
			break;
			case 15:
			break;
		}
	}

	private void AssignBundleToCheck()
	{
		bundleToCheck[0] = new Vector3(0, 1, 2);
		bundleToCheck[1] = new Vector3(1, 2, 3);
		bundleToCheck[2] = new Vector3(4, 5, 6);
		bundleToCheck[3] = new Vector3(5, 6, 7);
		bundleToCheck[4] = new Vector3(8, 9, 10);
		bundleToCheck[5] = new Vector3(9, 10, 11);
		bundleToCheck[6] = new Vector3(12, 13, 14);
		bundleToCheck[7] = new Vector3(13, 14, 15);

		bundleToCheck[8] = bundleToCheck[0] + new Vector3 (16, 16, 16);
		bundleToCheck[9] = bundleToCheck[1] + new Vector3 (16, 16, 16);
		bundleToCheck[10] = bundleToCheck[2] + new Vector3 (16, 16, 16);
		bundleToCheck[11] = bundleToCheck[3] + new Vector3 (16, 16, 16);
		bundleToCheck[12] = bundleToCheck[4] + new Vector3 (16, 16, 16);
		bundleToCheck[13] = bundleToCheck[5] + new Vector3 (16, 16, 16);
		bundleToCheck[14] = bundleToCheck[6] + new Vector3 (16, 16, 16);
		bundleToCheck[15] = bundleToCheck[7] + new Vector3 (16, 16, 16);

		bundleToCheck[16] = bundleToCheck[0] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[17] = bundleToCheck[1] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[18] = bundleToCheck[2] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[19] = bundleToCheck[3] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[20] = bundleToCheck[4] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[21] = bundleToCheck[5] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[22] = bundleToCheck[6] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[23] = bundleToCheck[7] + new Vector3 (16, 16, 16) * 2;

		bundleToCheck[24] = bundleToCheck[0] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[25] = bundleToCheck[1] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[26] = bundleToCheck[2] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[27] = bundleToCheck[3] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[28] = bundleToCheck[4] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[29] = bundleToCheck[5] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[30] = bundleToCheck[6] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[31] = bundleToCheck[7] + new Vector3 (16, 16, 16) * 3;

		bundleToCheck[32] = new Vector3 (0, 4, 8);
		bundleToCheck[33] = new Vector3 (4, 8, 12);
		bundleToCheck[34] = new Vector3 (1, 5, 9);
		bundleToCheck[35] = new Vector3 (5, 9, 13);
		bundleToCheck[36] = new Vector3 (2, 6, 10);
		bundleToCheck[37] = new Vector3 (6, 10, 14);
		bundleToCheck[38] = new Vector3 (3, 7, 11);
		bundleToCheck[39] = new Vector3 (7, 11, 15);

		bundleToCheck[40] = bundleToCheck[32] + new Vector3 (16, 16, 16);
		bundleToCheck[41] = bundleToCheck[33] + new Vector3 (16, 16, 16);
		bundleToCheck[42] = bundleToCheck[34] + new Vector3 (16, 16, 16);
		bundleToCheck[43] = bundleToCheck[35] + new Vector3 (16, 16, 16);
		bundleToCheck[44] = bundleToCheck[36] + new Vector3 (16, 16, 16);
		bundleToCheck[45] = bundleToCheck[37] + new Vector3 (16, 16, 16);
		bundleToCheck[46] = bundleToCheck[38] + new Vector3 (16, 16, 16);
		bundleToCheck[47] = bundleToCheck[39] + new Vector3 (16, 16, 16);

		bundleToCheck[48] = bundleToCheck[32] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[49] = bundleToCheck[33] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[50] = bundleToCheck[34] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[51] = bundleToCheck[35] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[52] = bundleToCheck[36] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[53] = bundleToCheck[37] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[54] = bundleToCheck[38] + new Vector3 (16, 16, 16) * 2;
		bundleToCheck[55] = bundleToCheck[39] + new Vector3 (16, 16, 16) * 2;

		bundleToCheck[56] = bundleToCheck[32] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[57] = bundleToCheck[33] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[58] = bundleToCheck[34] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[59] = bundleToCheck[35] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[60] = bundleToCheck[36] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[61] = bundleToCheck[37] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[62] = bundleToCheck[38] + new Vector3 (16, 16, 16) * 3;
		bundleToCheck[63] = bundleToCheck[39] + new Vector3 (16, 16, 16) * 3;

		bundleToCheck[64] = new Vector3 (0, 16, 32);
		bundleToCheck[65] = new Vector3 (16, 32, 48);
		bundleToCheck[66] = new Vector3 (1, 17, 33);
		bundleToCheck[67] = new Vector3 (17, 33, 49);
		bundleToCheck[68] = new Vector3 (2, 18, 34);
		bundleToCheck[69] = new Vector3 (18, 34, 50);
		bundleToCheck[70] = new Vector3 (3, 19, 35);
		bundleToCheck[71] = new Vector3 (19, 35, 51);
		
		bundleToCheck[72] = bundleToCheck[64] + new Vector3 (4, 4, 4);
		bundleToCheck[73] = bundleToCheck[65] + new Vector3 (4, 4, 4);
		bundleToCheck[74] = bundleToCheck[66] + new Vector3 (4, 4, 4);
		bundleToCheck[75] = bundleToCheck[67] + new Vector3 (4, 4, 4);
		bundleToCheck[76] = bundleToCheck[68] + new Vector3 (4, 4, 4);
		bundleToCheck[77] = bundleToCheck[69] + new Vector3 (4, 4, 4);
		bundleToCheck[78] = bundleToCheck[70] + new Vector3 (4, 4, 4);
		bundleToCheck[79] = bundleToCheck[71] + new Vector3 (4, 4, 4);

		bundleToCheck[80] = bundleToCheck[64] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[81] = bundleToCheck[65] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[82] = bundleToCheck[66] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[83] = bundleToCheck[67] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[84] = bundleToCheck[68] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[85] = bundleToCheck[69] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[86] = bundleToCheck[70] + new Vector3 (4, 4, 4) * 2;
		bundleToCheck[87] = bundleToCheck[71] + new Vector3 (4, 4, 4) * 2;

		bundleToCheck[88] = bundleToCheck[64] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[89] = bundleToCheck[65] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[90] = bundleToCheck[66] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[91] = bundleToCheck[67] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[92] = bundleToCheck[68] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[93] = bundleToCheck[69] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[94] = bundleToCheck[70] + new Vector3 (4, 4, 4) * 3;
		bundleToCheck[95] = bundleToCheck[71] + new Vector3 (4, 4, 4) * 3;
	}
}