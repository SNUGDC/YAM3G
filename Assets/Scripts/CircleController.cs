using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CircleController : MonoBehaviour
{
    void OnMouseDown()
    {
        GetComponentInParent<BoardController>().SetClickedObject(gameObject);
    }
}
