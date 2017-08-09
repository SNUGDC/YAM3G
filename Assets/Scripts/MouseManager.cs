using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {

    Vector3 posI;
    Vector3 posF;
    public float standardDistance = 0.5f;

    void Update ()
    {
	    if (Input.GetMouseButtonDown(0))
        {
            posI = Input.mousePosition;
        }	
        if (Input.GetMouseButtonUp(0))
        {
            posF = Input.mousePosition;
            asdf();
        }
	}
    void asdf()
    {
        float d = DistanceOfTwoPoint(posI, posF);
        IntVector2 delta = Delta(posI, posF);
        Debug.Log("Standard Distance : "+d);
        Debug.Log("Delta Vector : " + delta.x + ", " + delta.y);
        if (d > standardDistance)
        {
            gameObject.GetComponent<BoardController>().InputMouse(delta.x, delta.y);
        }
    }
    float DistanceOfTwoPoint(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }
    IntVector2 Delta(Vector3 a, Vector3 b)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        if (dy >= dx && dy >= -1 * dx) { return new IntVector2(0, 1); }
        else if (dy >= dx && dy < -1*dx) { return new IntVector2(-1, 0); }
        else if (dy < dx && dy >= -1 * dx) { return new IntVector2(1, 0); }
        else { return new IntVector2(0, -1); }
    }
}
