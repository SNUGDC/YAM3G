using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public enum SwipeVector
    {
        Right, Left, Up, Down
    }
    public static Vector2 pressPos;
    public static Vector2 nowPos;
    public static Vector2 swipeVector;
    private Arcball arcball;

    private SwipeVector GetSwipeVector()
    {
        swipeVector = nowPos - pressPos;

        if (Mathf.Abs(swipeVector.x) >= Mathf.Abs(swipeVector.y))
        {
            if (swipeVector.x >= 0)
            {
                Debug.Log("(" + swipeVector.x + ", " + swipeVector.y + ") => 오른쪽");
                return SwipeVector.Right;
            }
            else
            {
                Debug.Log("(" + swipeVector.x + ", " + swipeVector.y + ") => 왼쪽");
                return SwipeVector.Left;
            }
        }
        else
        {
            if (swipeVector.y >= 0)
            {
                Debug.Log("(" + swipeVector.x + ", " + swipeVector.y + ") => 위쪽");
                return SwipeVector.Up;
            }
            else
            {
                Debug.Log("(" + swipeVector.x + ", " + swipeVector.y + ") => 아래쪽");
                return SwipeVector.Down;
            }
        }
    }

    private void Awake()
    {
        arcball = GameObject.Find("Arcball").GetComponent<Arcball>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (SphereMover.isSphereMovingStart == true)
            {
                Debug.Log("공이 ");
                SphereMover.isSphereMovingStart = false;
                nowPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                GetSwipeVector();
            }
            else
            {
                nowPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                GetSwipeVector();
                arcball.RotateArcball(GetSwipeVector());
            }
        }
    }
}
