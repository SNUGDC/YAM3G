using System;
using UnityEngine;

public enum Attribution { None, Bubble, Stone };

[Serializable]
public class CircleSettings {
    public int color;
    public int probOfBS;
    public Sprite[] circleNoneSprites;
    public Sprite[] circleBubbleSprites;
    public Sprite[] circleStoneSprites;
    public GameObject standardCircle;
}

public class Circle
{
    public int value;
    public GameObject circleObject;
    public Attribution att;

    public Circle(Transform parent, CircleSettings settings)
    {
        this.value = NewNumber(settings.color);
        this.att = NewAttribution(settings.probOfBS);
        this.circleObject = MonoBehaviour.Instantiate(settings.standardCircle, parent);
        circleObject.GetComponent<SpriteRenderer>().sprite = GetSprite(settings, value, att);
    }

    static Sprite GetSprite(CircleSettings settings, int value, Attribution att)
    {
        if (value == 0)
        {
            return null;
        }
        else
        {
            Sprite[] sprites;
            switch (att)
            {
                default:
                    {
                        sprites = settings.circleNoneSprites;
                        break;
                    }
                case Attribution.Bubble:
                    {
                        sprites = settings.circleBubbleSprites;
                        break;
                    }
                case Attribution.Stone:
                    {
                        sprites = settings.circleStoneSprites;
                        break;
                    }
            }
            return sprites[value - 1];
        }
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