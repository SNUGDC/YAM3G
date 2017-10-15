using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleController : MonoBehaviour
{
    private float aniTime { get { return BoardController.aniTime; }}
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
    public void Suicide()
    {
        var toX = gameObject.transform.position.x + UnityEngine.Random.value*2 - 1;
        var toY = gameObject.transform.position.y + UnityEngine.Random.value*2 + 8;
        
        DOTween.Sequence()
            .Join(mark.transform.DOMoveX(toX, aniTime*4).SetEase(Ease.Linear))
            .Join(mark.transform.DOMoveY(toY, aniTime*4).SetEase(Ease.InQuart))
            .Join(mark.GetComponent<SpriteRenderer>().DOFade(0, aniTime*4).SetEase(Ease.OutCirc))
            .Play()
            .OnComplete(()=>{
                MonoBehaviour.Destroy(gameObject);
            });
    }
    void OnMouseDown()
    {
        GetComponentInParent<BoardController>().SetClickedObject(gameObject);
    }
}
