using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BubbleStoneMover
{
    
    private int size;
    private Circle[,] board;
    private Barrier[,] barrierH;
    private BoardSettings settings;

    private float grid { get { return settings.Grid; }}
    private float mid { get { return settings.Mid; }}
    private float aniTime { get { return BoardController.aniTime; }}

    private List<Circle> pushedCircles;
    public BubbleStoneMover(int size, Circle[,] board, Barrier[,] barrierH, BoardSettings settings)
    {
        this.size = size;
        this.board = board;
        this.barrierH = barrierH;
        this.settings = settings;
    }
    public IEnumerator DoBSMove()
    {
        var movedBubbleStone = MoveBubbleStone();
        var movedCircle = CollectMovedCircles();
        bool done = (movedBubbleStone.Count == 0 && movedCircle.Count == 0);

        yield return DOTween.Sequence()
            .JoinAll(movedBubbleStone.Select(cwp => MoveCircle(cwp)))
            .JoinAll(movedCircle.Select(cwp => MoveCircle(cwp)))
            .OnStart(()=>{
                if (!done) SoundManager.PlaySound(SoundType.MoveBS);
            })
            .WaitForCompletion();
    }

    List<CircleWithPos> MoveBubbleStone()
    {
        var list = new List<CircleWithPos>();
        pushedCircles = new List<Circle>();
        for (int j = size - 1; j > -1; j--)
        {
            for (int i = 0; i < size; i ++)
            {
                if (board[i,j].att == Attribution.Bubble && canMoveBubble(i,j))
                {
                    var bwp = MoveUpBubble(i,j);
                    list.Add(bwp);
                }
            }
        }
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[i,j].att == Attribution.Stone && canMoveStone(i,j))
                {
                    var swp = MoveDownStone(i,j);
                    list.Add(swp);
                }
            }
        }
        return list;
    }

    CircleWithPos MoveUpBubble(int i, int j)
    {
        var x = i;
        var y = j;
        while (canMoveBubble(x,y)) 
        {
            var tempCircle = board[x,y+1];
            board[x,y+1] = board[x,y];
            board[x,y] = tempCircle;
            if (board[x,y].att == Attribution.None && !pushedCircles.Contains(board[x,y]))
            {
                pushedCircles.Add(board[x,y]);
            }
            y++;
        }
        var movedBubble = new CircleWithPos(board[x,y], new IntVector2(x,y));
        return movedBubble;
    }

    bool canMoveBubble(int x, int y)
    {
        return 
            !(y != size - 1 && barrierH[x, y].value)
            && IsInsideOfRange(x,y + 1)
            && board[x,y+1].att != Attribution.Bubble;
    }

    CircleWithPos MoveDownStone(int i, int j)
    {
        var x = i;
        var y = j;
        while (canMoveStone(x,y)) 
        {
            var tempCircle = board[x,y-1];
            board[x,y-1] = board[x,y];
            board[x,y] = tempCircle;
            if (board[x,y].att == Attribution.None && !pushedCircles.Contains(board[x,y]))
            {
                pushedCircles.Add(board[x,y]);
            }
            y--;
        }
        var movedBubble = new CircleWithPos(board[x,y], new IntVector2(x,y));
        return movedBubble;
    }
    bool canMoveStone(int x, int y)
    {
        return 
            !(y != 0 && barrierH[x, y - 1].value)
            && IsInsideOfRange(x, y - 1) 
            && board[x, y - 1].att != Attribution.Stone;
    }

    bool IsInsideOfRange(int x, int y)
    {
        if (x < 0) return false;
        if (x >= size) return false;
        if (y < 0) return false;
        if (y >= size) return false;
        return true;
    }

    List<CircleWithPos> CollectMovedCircles()
    {
        var list = new List<CircleWithPos>();
        foreach (var circle in pushedCircles)
        {
            var pos = PosOfCircleObject(circle.circleObject);
            list.Add(new CircleWithPos(circle,pos));
        }
        return list;
    }
    IntVector2 PosOfCircleObject(GameObject circleObject)
    {
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[i,j] != null && board[i, j].circleObject == circleObject)
                {
                    return new IntVector2(i, j);
                }
            }
        }
        return null;
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
