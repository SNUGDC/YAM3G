using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Checker {
    private int size;
    private Circle[,] board;
    private int combo;
    private int multiplier;
    
    private bool[,] checkBoard;
    private float aniTime { get { return BoardController.aniTime; }}
    public Checker(int size, Circle[,] board, int combo, int multiplier) 
    {
        this.size = size;
        this.board = board;
        this.combo = combo;
        this.multiplier = multiplier;
        
        this.checkBoard = new bool[size, size];
    }

    public bool Done { get; private set; }
    public int Score { get; private set; }

    public IEnumerator DoCheck()
    {
        var count = new CheckCount();
        var checkedList = new List<IntVector2>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                CheckOnce(checkedList, i, j, 0, count);
            }
        }
        
        AddScore(count);
        var deadCircles = CleanUpDeadCircles();
        Done = deadCircles.Count == 0;

        yield return DOTween.Sequence()
            .JoinAll(deadCircles.Select(circleGO => circleGO.transform.DOScale(Vector3.zero, aniTime)))
            .OnStart(()=>{
                if(!Done) SoundManager.PlaySound(SoundType.Check);
            })
            .OnComplete(() => { 
                deadCircles.ForEach(MonoBehaviour.Destroy);
            })
            .WaitForCompletion();
    }
    private void AddScore(CheckCount count)
    {
        int value = count.count * combo * multiplier;
        Debug.Log("count, combo, multiplier, value : " + count.count + ", " + combo + ", " + multiplier + ", " + value);
        Score += value;
    }
    private List<GameObject> CleanUpDeadCircles() 
    {
        var list = new List<GameObject>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (checkBoard[i, j])
                {
                    list.Add(board[i, j].circleObject);
                    board[i, j] = null;
                }
            }
        }

        return list;
    }
    
    List<List<CircleWithPos>> CheckLines()
    {
        var lists = new List<List<CircleWithPos>>();
        var checkedPos = new List<IntVector2>();

        for (int j = 0; j < size; j ++)
        {
            for (int i = 0; i < size; i++)
            {
                if (!checkedPos.Contains(new IntVector2(i,j)))
                {
                    var line = new List<CircleWithPos>();
                    CheckCircles(checkedPos, i, j, true, 1, 0);
                    CheckCircles(checkedPos, i, j, false, 1, 0);
                    lists.Add (line);
                }
            }
        }
        return lists;
    }
    List<IntVector2> CheckCircles(List<IntVector2> checkedPos, int x, int y, bool isX, int delta, int recursiveNum)
    {
        var line = new List<IntVector2>();
        if (delta == 0)
        {
            
        }
        else
        {
            if (isX)
            {
                var tempX = x;
                var color = board[x,y].value;
                while(IsInsideOfSize(tempX) && board[tempX,y].value == color) 
                {
                    tempX += delta;
                }
                var count = delta*(tempX - x) + 1;

                if (count >= 3)
                {
                    var tempLine = new List<IntVector2>();
                    for (; delta * tempX >= delta * x; tempX -= delta)
                    {
                        var pos = new IntVector2(tempX,y);
                        if (!checkedPos.Contains(pos))
                        {
                            checkedPos.Add(pos);
                            tempLine.Add(pos);
                        }
                    }
                    
                    foreach(var pos in tempLine)
                    {
                        line.Add(pos);
                    }
                }
            }
        }
        return line;
    }
    List<IntVector2> CheckACircle()
    {
        var list = new List<IntVector2>();
        return list;
    }
    class CheckCount
    {
        public int count;
        public CheckCount()
        {
            this.count = 0;
        }
    }
    private void CheckOnce(List<IntVector2> checkedList, int x, int y, int recursiveNum, CheckCount count)
    {
        IntVector2 presentPos = new IntVector2(x, y);
        if (!checkedList.Contains(presentPos))
        {
            int x0 = ThreeNumber(x)[0], x1 = ThreeNumber(x)[1], x2 = ThreeNumber(x)[2];
            int y0 = ThreeNumber(y)[0], y1 = ThreeNumber(y)[1], y2 = ThreeNumber(y)[2];
            bool doForX = false;
            bool doForY = false;

            if (!(checkBoard[x0, y] && checkBoard[x1, y] && checkBoard[x2, y]) &&
            board[x1, y].value == board[x0, y].value &&
            board[x1, y].value == board[x2, y].value &&
            board[x1, y].value != 0)
            {
                doForX = true;
                CheckOnBoard(x0, y, count);
                CheckOnBoard(x1, y, count);
                CheckOnBoard(x2, y, count);
                if (recursiveNum != 0)
                {
                    checkedList.Add(presentPos);
                }
            }
            if (!(checkBoard[x, y0] && checkBoard[x, y1] && checkBoard[x, y2]) &&
                board[x, y1].value == board[x, y0].value &&
                board[x, y1].value == board[x, y2].value &&
                board[x, y1].value != 0)
            {
                doForY = true;
                CheckOnBoard(x, y0, count);
                CheckOnBoard(x, y1, count);
                CheckOnBoard(x, y2, count);
                if (recursiveNum != 0)
                {
                    checkedList.Add(presentPos);
                }
            }
            if (doForX)
            {
                CheckOnce(checkedList, x0, y, recursiveNum + 1, count);
                CheckOnce(checkedList, x1, y, recursiveNum + 1, count);
                CheckOnce(checkedList, x2, y, recursiveNum + 1, count);
            }
            if (doForY)
            {
                CheckOnce(checkedList, x, y0, recursiveNum + 1, count);
                CheckOnce(checkedList, x, y1, recursiveNum + 1, count);
                CheckOnce(checkedList, x, y2, recursiveNum + 1, count);
            }
        }
    }
    private void CheckOnBoard(int x, int y, CheckCount count)
    {
        if (checkBoard[x, y] == false)
        {
            checkBoard[x, y] = true;
            count.count++;
        }
    }
    private int[] ThreeNumber(int n)
    {
        if (n == 0) { return new int[] { 0, 1, 2 }; }
        else if (n == size - 1) { return new int[] { size - 3, size - 2, size - 1 }; }
        else { return new int[] { n - 1, n, n + 1 }; }
    }
    bool IsInsideOfSize(int value)
    {
        if (-1 < value && value < size) { return true; }
        else { return false; }
    }

    int IntAbs(int value)
    {
        if (value < 0) { return -1 * value;}
        else { return value; }
    }
}