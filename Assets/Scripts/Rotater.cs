using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public enum AngularDirection {Clockwise, AntiClockwise}
public class Rotater
{
    private int size;
    private bool isClockwise;
    private Circle[,] board;
    private Barrier[,] barrierH;
    private Barrier[,] barrierV;
    private Circle[,] tempBoard;
    private Barrier[,] tempBarrierH;
    private Barrier[,] tempBarrierV;
    private BoardSettings settings;

    private float grid { get { return settings.Grid; }}
    private float mid { get { return settings.Mid; }}
    private float aniTime { get { return BoardController.aniTime; }}

    public Rotater(AngularDirection dir, int size, Circle[,] board, Barrier[,] barrierH, Barrier[,] barrierV, BoardSettings settings)
    {
        this.size = size;
        this.board = board;
        this.barrierH = barrierH;
        this.barrierV = barrierV;
        this.settings = settings;
        isClockwise = dir == AngularDirection.Clockwise;
    }
    void GenerateTemps()
    {
        tempBoard = new Circle[size,size];
        tempBarrierH = new Barrier[size,size-1];
        tempBarrierV = new Barrier[size-1,size];
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                tempBoard[i, j] = board[i, j];
                if (j != size - 1)
                {
                    tempBarrierH[i, j] = barrierH[i, j];
                }
                if (i != size - 1)
                {
                    tempBarrierV[i, j] = barrierV[i, j];
                }
            }
        }
    }
    public IEnumerator DoRotate()
    {
        GenerateTemps();
        var circles = RotateCircles();
        var barrierHs = RotateBarrierHs();
        var barrierVs = RotateBarrierVs();

        yield return DOTween.Sequence()
            .JoinAll(circles.Select(cwp => MoveCircle(cwp)))
            .JoinAll(barrierHs.Select(bwpH => MoveBarrier(bwpH,true)))
            .JoinAll(barrierHs.Select(bwpV => MoveBarrier(bwpV,false)))
            .WaitForCompletion();
    }

    List<CircleWithPos> RotateCircles()
    {
        var list = new List<CircleWithPos>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (isClockwise)
                { 
                    board[i, j] = tempBoard[size - 1 - j, i]; 
                }
                else
                { 
                    board[i, j] = tempBoard[j, size - 1 - i]; 
                }
                var cwp = new CircleWithPos(board[i,j], new IntVector2(i,j));
                list.Add(cwp);
            }
        }
        return list;
    }

    List<BarrierWithPos> RotateBarrierHs()
    {
        var list = new List<BarrierWithPos>();
        for (int j = 0; j < size-1; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (isClockwise)
                { 
                    barrierH[i, j] = tempBarrierV[size - 2 - j, i]; 
                }
                else
                { 
                    barrierH[i, j] = tempBarrierV[j, size - 1 - i]; 
                }
                var bwpH = new BarrierWithPos(barrierH[i,j],new IntVector2(i,j));
                list.Add(bwpH);                
            }
        }
        return list;
    }
    List<BarrierWithPos> RotateBarrierVs()
    {
        var list = new List<BarrierWithPos>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size-1; i++)
            {
                if (isClockwise)
                { 
                    barrierV[i, j] = tempBarrierH[size - 1 - j, i]; 
                }
                else
                { 
                    barrierV[i, j] = tempBarrierH[j, size - 2 - i]; 
                }
                var bwpV = new BarrierWithPos(barrierV[i,j], new IntVector2(i,j));
                list.Add(bwpV);
            }
        }
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

    Sequence MoveBarrier(BarrierWithPos bwp, bool isBarrierH)
    {
        var x = bwp.pos.x;
        var y = bwp.pos.y;
        var bt = bwp.barrier.barrierObject.transform;
        var toVec = isBarrierH?
            new Vector3(grid / 2 + (x - mid) * grid, grid + (y - mid) * grid)
            : new Vector3(grid + (x - mid) * grid, grid / 2 + (y - mid) * grid);
        var dir = isClockwise? -1 : 1;
        return DOTween.Sequence()
            .Join(bt.DOMove(toVec, aniTime))
            .Join(bt.DORotate(new Vector3(0,0,dir*90), aniTime).SetRelative());
    }
}
