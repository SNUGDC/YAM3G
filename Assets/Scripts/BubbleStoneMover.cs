using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class BubbleStoneMover
{
    
    private int size;
    private Circle[,] board;
    private Barrier[,] barrierH;
    private List<CircleWithPos> movedCircles;
    public bool Done { get; private set;}
    public BubbleStoneMover(int size, Circle[,] board, Barrier[,] barrierH)
    {
        this.size = size;
        this.board = board;
        this.barrierH = barrierH;
    }
    public IEnumerator DoBSMove()
    {
        var movedBubbleStone = MoveBubbleStone();
        
        yield return null;
    }

    List<CircleWithPos> MoveBubbleStone()
    {
        var list = new List<CircleWithPos>();
        for (int j = size - 1; j > -1; j--)
        {
            for (int i = 0; i < size; i ++)
            {
                if (canMoveBubble(i,j))
                {
                    var cwp = new CircleWithPos(board[i,j], new IntVector2(i,j));
                    list.Add(cwp);
                }
            }
        }
        return list;
    }

    bool canMoveBubble(int x, int y)
    {
        return 
            !(y != size - 1 && barrierH[x, y].value == true)
            && board[x,y].att == Attribution.Bubble
            && IsInsideOfRange(x,y)
            && board[x,y+1].att != Attribution.Bubble;
    }
    bool MoveBubble()
    {
        bool done = false;
        for (int j = size - 1; j > -1; j--)
        {
            for (int i = 0; i < size; i++)
            {
                bool canMove = true;
                if (j != size - 1 && barrierH[i, j].value == true)
                {
                    canMove = false;
                }
                if (canMove &&
                    board[i, j].att == Attribution.Bubble &&
                    IsInsideOfRange(i, j + 1) &&
                    board[i, j + 1].att != Attribution.Bubble)
                {
                    MoveCircle(i, j, i, j + 1);
                    done = true;
                }
            }
        }
        return done;
    }
    bool MoveStone()
    {
        bool done = false;
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                bool canMove = true;
                if (j != 0 && barrierH[i, j - 1].value == true)
                {
                    canMove = false;
                }
                if (canMove &&
                    board[i, j].att == Attribution.Stone &&
                    IsInsideOfRange(i, j - 1) &&
                    board[i, j - 1].att != Attribution.Stone)
                {
                    MoveCircle(i, j, i, j - 1);
                    done = true;
                }
            }
        }
        return done;
    }

    bool IsInsideOfRange(int x, int y)
    {
        if (x < 0) return false;
        if (x >= size) return false;
        if (y < 0) return false;
        if (y >= size) return false;
        return true;
    }
    bool IsInsideOfRange(IntVector2 vector)
    {
        return IsInsideOfRange(vector.x, vector.y);
    }
}
