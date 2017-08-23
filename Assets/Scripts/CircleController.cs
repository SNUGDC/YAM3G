using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CircleController : MonoBehaviour
{
    void ChangeMark(Sprite mark)
    {
        
    }
    void OnMouseDown()
    {
        GetComponentInParent<BoardController>().SetClickedObject(gameObject);
    }
}
