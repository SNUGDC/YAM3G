using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arcball : MonoBehaviour //마우스 이동 방향 : 왼쪽 => rotation.Y 증가, 마우스 이동 방향 : 위쪽 => rotation.X 증가
{
    public void RotateArcball(MouseMovement.SwipeVector dir)
    {
        switch (dir)
        {
            case MouseMovement.SwipeVector.Left:
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, transform.rotation.eulerAngles.z);
                break;
            case MouseMovement.SwipeVector.Right:
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
                break;
            case MouseMovement.SwipeVector.Up:
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                break;
            case MouseMovement.SwipeVector.Down:
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                break;
        }
    }
}