using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Swaper
{
    private int size;
    private Circle[,] board;
    private BoardSettings settings;
    private IntVector2 posI;
    private IntVector2 posF;
    private float grid { get { return settings.Grid; }}
    private float mid { get { return settings.Mid; }}
    private float aniTime { get { return BoardController.aniTime; }}
    public Swaper(int size, Circle[,] board, BoardSettings settings)
    {
        this.size = size;
        this.board = board;
        this.settings = settings;
    }

    public IEnumerator DoSwap(IntVector2 posI, IntVector2 posF)
    {
        this.posI = posI;
        this.posF = posF;
        var swapedCircles = SwapCircles();

        yield return DOTween.Sequence()
            .JoinAll(swapedCircles.Select(cwp => MoveCircle(cwp)))
            .OnStart(()=>{
                SoundManager.PlaySound(SoundType.Swap);
            })
            .WaitForCompletion();
    }

    List<CircleWithPos> SwapCircles()
    {
        var list = new List<CircleWithPos>();
        int xi = posI.x, yi = posI.y, xf = posF.x, yf = posF.y;
        var tempCircle = board[xf, yf];
        board[xf,yf] = board[xi,yi];
        board[xi,yi] = tempCircle;
        list.Add(new CircleWithPos(board[xi,yi], new IntVector2(xi,yi)));
        list.Add(new CircleWithPos(board[xf,yf], new IntVector2(xf,yf)));
        return list;
    }

    Tween MoveCircle(CircleWithPos cwp)
    {
        var x = cwp.pos.x;
        var y = cwp.pos.y;
        var ct = cwp.circle.circleObject.transform;
        var toVec = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y - mid) * grid);
        return ct.DOMove(toVec, aniTime);
    }
}