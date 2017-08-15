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
    
    private float scale { get { return settings.Scale; }}
    private float grid { get { return settings.Grid; }}
    private float mid { get { return settings.Mid; }}
    private float aniTime { get { return BoardController.aniTime; }}
    public Refiller(int size, Circle[,] board, BoardSettings settings)
    {
        this.size = size;
        this.board = board;
        this.settings = settings;
    }
    public bool Done { get; private set; }

    public IEnumerator DoRefill()
    {
        Done = true;
        
        var movedCircles = CollectMovingCircles();
        var newCircles = CollectingNewCircles();

        yield return DOTween.Sequence()
            .JoinAll(newCircles.Select(cwp => GenerateCircle(cwp)))
            .JoinAll(movedCircles.Select(cwp => FallCircle(cwp)))
            .WaitForCompletion();
            
        Done = !(movedCircles.Count == 0 && newCircles.Count == 0);
    }   
    class CircleWithPos
    {
        public Circle circle;
        public IntVector2 pos;
        public CircleWithPos(Circle circle, IntVector2 pos)
        {
            this.circle = circle;
            this.pos = pos;
        }
    }

    private List<CircleWithPos> CollectMovingCircles() 
    {
        var list = new List<CircleWithPos>();
        for (int j = 0; j < size - 1; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[i, j] == null)
                {
                    board[i, j] = board[i, j+1];
                    board[i, j+1] = null;
                    if (board[i, j] != null)
                    { 
                        list.Add(new CircleWithPos(board[i,j], new IntVector2(i,j)));
                    }
                }
            }
        }
        return list;
    }

    Tween FallCircle(CircleWithPos cwp)
    {
        var x = cwp.pos.x;
        var y = cwp.pos.y;
        var ct = cwp.circle.circleObject.transform;
        var toVec = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y - mid) * grid);
        return ct.DOMove(toVec, aniTime);
    }
    
    private List<CircleWithPos> CollectingNewCircles()
    {
        var list = new List<CircleWithPos>();
        
        for (int i = 0, j = size - 1; i < size; i ++)
        {
            if (board[i,j] == null)
            {
                board[i,j] = new Circle();
                var ct = board[i,j].circleObject.transform;
                ct.localScale = Vector3.zero;
                ct.position = new Vector3(grid / 2 + (i - mid) * grid, grid / 2 + (j - mid) * grid);
                list.Add(new CircleWithPos(board[i,j], new IntVector2(i,j)));
            }
        }
        return list;
    }

    Tween GenerateCircle(CircleWithPos cwp)
    {
        var x = cwp.pos.x;
        var y = cwp.pos.y;
        var ct = cwp.circle.circleObject.transform;
        var toVec = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y+1 - mid) * grid);
        //return ct.DOMove(toVec, aniTime).From();
        return ct.DOScale(new Vector3(scale, scale, 1), aniTime);
    }
}
