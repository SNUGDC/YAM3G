using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Checker {
    private int size;
    private Circle[,] board;
    
    private bool[,] checkBoard;
    public Checker(int size, Circle[,] board) {
        this.size = size;
        this.board = board;
        
        this.checkBoard = new bool[size, size];
    }

    public bool Done { get; private set; }
    public int Score { get; private set; }

    public IEnumerator DoCheck()
    {
        var checkedList = new List<IntVector2>();
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                CheckOnce(checkedList, i, j, 0);
            }
        }
        
        var deadCircles = CleanUpDeadCircles();
        Done = deadCircles.Count == 0;

        yield return DOTween.Sequence()
            .JoinAll(deadCircles.Select(circleGO => circleGO.transform.DOScale(Vector3.zero, 1)))
            .OnComplete(() => { 
                deadCircles.ForEach(MonoBehaviour.Destroy);
            })
            .WaitForCompletion();
    }

    private List<GameObject> CleanUpDeadCircles() {
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
    
    private void CheckOnce(List<IntVector2> checkedList, int x, int y, int recursiveNum)
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
                CheckOnBoard(x0, y);
                CheckOnBoard(x1, y);
                CheckOnBoard(x2, y);
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
                CheckOnBoard(x, y0);
                CheckOnBoard(x, y1);
                CheckOnBoard(x, y2);
                if (recursiveNum != 0)
                {
                    checkedList.Add(presentPos);
                }
            }
            if (doForX)
            {
                CheckOnce(checkedList, x0, y, recursiveNum + 1);
                CheckOnce(checkedList, x1, y, recursiveNum + 1);
                CheckOnce(checkedList, x2, y, recursiveNum + 1);
            }
            if (doForY)
            {
                CheckOnce(checkedList, x, y0, recursiveNum + 1);
                CheckOnce(checkedList, x, y1, recursiveNum + 1);
                CheckOnce(checkedList, x, y2, recursiveNum + 1);
            }
            if ((doForX || doForY) && recursiveNum == 0)
            {
                Debug.Log("a mount of circles");
            }
        }
    }
    private void CheckOnBoard(int x, int y)
    {
        if (checkBoard[x, y] == false)
        {
            checkBoard[x, y] = true;
            Score += 1;
        }
    }
    private int[] ThreeNumber(int n)
    {
        if (n == 0) { return new int[] { 0, 1, 2 }; }
        else if (n == size - 1) { return new int[] { size - 3, size - 2, size - 1 }; }
        else { return new int[] { n - 1, n, n + 1 }; }
    }
}