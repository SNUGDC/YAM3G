using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMover : MonoBehaviour
{
    public static bool isSphereMovingStart;

    private void Start()
    {
        isSphereMovingStart = false;
    }

    private void Update()
    {
    }

    private void OnMouseDown()
    {
        isSphereMovingStart = true;
    }

    private void OnMouseUp()
    {
        //isSphereMovingStart = false;
    }
}