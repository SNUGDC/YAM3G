using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Refiller
{
    private int size;
    private Circle[,] board;
    private BoardSettings settings;
    private int[] columnCount;
    
    private float scale { get { return settings.Scale; }}
    private float grid { get { return settings.Grid; }}
    private float mid { get { return settings.Mid; }}
    private float aniTime { get { return BoardController.aniTime; }}
    public Refiller(int size, Circle[,] board, BoardSettings settings)
    {
        this.size = size;
        this.board = board;
        this.settings = settings;
        columnCount = new int[size];
    }
    public bool Done { get; private set; }

    public IEnumerator DoRefill()
    {
        Done = true;
        
        var movedCircles = CollectMovingCircles();
        var newCircles = CollectNewCircles();

        yield return DOTween.Sequence()
            .JoinAll(newCircles.Select(cwp => GenerateCircle(cwp)))
            .JoinAll(movedCircles.Select(cwp => FallCircle(cwp)))
            .WaitForCompletion();
            
        Done = !(movedCircles.Count == 0 && newCircles.Count == 0);
    }   

    List<CircleWithPos> CollectMovingCircles() 
    {
        var list = new List<CircleWithPos>();
        for (int j = 0; j < size - 1; j++)
        {
            for (int i = 0; i < size; i++)
            {
                var upCwp = UpCwp(i,j);
                if (board[i, j] == null && upCwp != null)
                {
                    board[i, j] = upCwp.circle;
                    board[upCwp.pos.x, upCwp.pos.y] = null;

                    var cwp = new CircleWithPos(board[i,j], new IntVector2(i,j));
                    list.Add(cwp);
                }
            }
        }
        return list;
    }

    CircleWithPos UpCwp(int x, int y)
    {
        int count = 0;
        for (int i = x, j = y; j < size; j ++)
        {
            if (board[i,j] != null)
            {
                columnCount[i] = count;
                var cwp = new CircleWithPos(board[i,j], new IntVector2(i,j));
                return cwp;
            }
            count++;
        }
        return null;
    }

    Tween FallCircle(CircleWithPos cwp)
    {
        var x = cwp.pos.x;
        var y = cwp.pos.y;
        var ct = cwp.circle.circleObject.transform;
        var toVec = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y - mid) * grid);
        return ct.DOMove(toVec, aniTime);
    }
    
    private List<CircleWithPos> CollectNewCircles()
    {
        var list = new List<CircleWithPos>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i ++)
            {
                if (board[i,j] == null)
                {
                    board[i,j] = new Circle();
                    var ct = board[i,j].circleObject.transform;
                    ct.localScale = new Vector3(scale, scale, 1);
                    ct.position = new Vector3(grid / 2 + (i - mid) * grid, grid / 2 + (j - mid) * grid);
                    list.Add(new CircleWithPos(board[i,j], new IntVector2(i,j)));
                }
            }
        }
        return list;
    }

    Sequence GenerateCircle(CircleWithPos cwp)
    {
        var x = cwp.pos.x;
        var y = cwp.pos.y;
        var ct = cwp.circle.circleObject.transform;
        var toVec = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y+columnCount[x]+1 - mid) * grid);
        return DOTween.Sequence()
            .Join(ct.DOMove(toVec, aniTime).From())
            .Join(ct.DOScale(Vector3.zero, aniTime).From());
    }
}
