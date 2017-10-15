using System;
using UnityEngine;

public enum Attribution { None, Bubble, Stone };

[Serializable]
public class CircleSettings 
{
    public int color;
    public int probOfBS;
    public Sprite edgeNoneSprite;
    public Sprite edgeBubbleSprite;
    public Sprite edgeStoneSprite;
    public Sprite[] markSprites;
    public GameObject standardCircle;
}

public class Circle
{
    public int value;
    public GameObject circleObject;
    public Attribution att;
    public static Transform parent;
    public static CircleSettings settings;

    public Circle()
    {
        this.value = NewNumber(settings.color);
        this.att = NewAttribution(settings.probOfBS);
        this.circleObject = MonoBehaviour.Instantiate(settings.standardCircle, parent);
        circleObject.GetComponent<CircleController>().GetSprites(settings, value, att);
    }

    static int NewNumber(int color)
    {
        return ((int)(UnityEngine.Random.value * color)) + 1;
    }

    static Attribution NewAttribution(int probOfBS)
    {
        int probNum = (int)(UnityEngine.Random.value * probOfBS);
        if (probNum == 0)
        {
            return Attribution.Bubble;
        }
        else if (probNum == 1)
        {
            return Attribution.Stone;
        }
        else
        {
            return Attribution.None;
        }
    }
}

public class CircleWithPos
{
    public Circle circle;
    public IntVector2 pos;
    public CircleWithPos(Circle circle, IntVector2 pos)
    {
        this.circle = circle;
        this.pos = pos;
    }
    public static Vector2 ToVector(IntVector2 pos)
    {
        return new Vector2(pos.x, pos.y);
    }
}