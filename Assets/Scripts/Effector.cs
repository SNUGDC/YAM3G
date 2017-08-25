using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Effector
{
    private int size;
    private Circle[,] board;
    private BoardSettings settings;
    
    private float scale { get { return settings.Scale; }}
    private float grid { get { return settings.Grid; }}
    private float mid { get { return settings.Mid; }}
    private float aniTime { get { return BoardController.aniTime; }}
    public Effector(int size, Circle[,] board, BoardSettings settings)
    {
        this.size = size;
        this.board = board;
        this.settings = settings;
    }

    public IEnumerator Finale()
    {
        var effectStack = new List<CircleWithPos>();
        var listOfPos = CollectAllPos();
        while(listOfPos.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, listOfPos.Count);
            var pos = listOfPos[rand];
            listOfPos.RemoveAt(rand);
            effectStack.Add(new CircleWithPos(board[pos.x,pos.y],pos));
        }
        yield return DOTween.Sequence()
            .JoinAllDelay(effectStack.Select(cwp => MoveCircle(cwp)), aniTime/8)
            .OnStart(()=>{
                SoundManager.PlayFinale();
            });
    }
    List<IntVector2> CollectAllPos()
    {
        var list = new List<IntVector2>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                var pos = new IntVector2(i,j);
                list.Add(pos);
            }
        }
        return list;
    }
    Sequence MoveCircle(CircleWithPos cwp)
    {
        var x = cwp.pos.x + UnityEngine.Random.value*2 - 1;
        var y = cwp.pos.y;
        var ct = cwp.circle.circleObject.transform;
        var toVec = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y + 1.5f*size - mid) * grid);
        return DOTween.Sequence()
            .Join(ct.DOMoveX(toVec.x, aniTime*6).SetEase(Ease.Linear))
            .Join(ct.DOMoveY(toVec.y, aniTime*6).SetEase(Ease.InQuart));
    }
}