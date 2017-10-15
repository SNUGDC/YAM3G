using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CircleController : MonoBehaviour
{
    public GameObject edge;
    public GameObject mark;

    SpriteRenderer GetBubbleSpriteRenderer()
    {
        return edge.GetComponent<SpriteRenderer>();
    }
    SpriteRenderer GetMarkSpriteRenderer()
    {
        return mark.GetComponent<SpriteRenderer>();
    }
    public void GetSprites(CircleSettings settings, int value, Attribution att)
    {
        Sprite sprite;
        switch (att)
        {
            default:
                {
                    sprite = settings.edgeNoneSprite;
                    break;
                }
            case Attribution.Bubble:
                {
                    sprite = settings.edgeBubbleSprite;
                    break;
                }
            case Attribution.Stone:
                {
                    sprite = settings.edgeStoneSprite;
                    break;
                }
        }
        GetBubbleSpriteRenderer().sprite = sprite;
        GetMarkSpriteRenderer().sprite = settings.markSprites[value-1];
    }
    void ChangeMark(Sprite mark)
    {
        
    }
    void OnMouseDown()
    {
        GetComponentInParent<BoardController>().SetClickedObject(gameObject);
    }
}
