using UnityEngine;

public enum Attribution { None, Bubble, Stone };
public class Circle
{
    public int value;
    public GameObject circleObject;
    public Attribution att;
    // TODO: int value -> Color value
    public Circle(int value, GameObject circleObject, Attribution att)
    {
        this.value = value;
        this.circleObject = circleObject;
        this.att = att;
    }
}