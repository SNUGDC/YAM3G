﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    public CircleSettings circleSettings;
    public int size = 8;
    public float edgeOfBoard = 2.5f;
    public float aniTime = 0.5f;
    float mid;
    float grid;
    float scale;
    int score;
    private Circle[,] board;
    private Barrier[,] barrierH;
    private Barrier[,] barrierV;
    public Sprite[] barrierSprites;
    public GameObject standardBarrier;
    GameObject clickedObject;
    public Text scoreRenderer;
    bool autoMode;
    bool canInput;
    
    void Awake()
    {
        CreateBoard();
        Initiate();
    }
    void Start()
    {
        Screen.SetResolution(360, 640, false);
    }

    void CreateBoard()
    {
        board = new Circle[size, size];
        barrierH = new Barrier[size, size - 1];
        barrierV = new Barrier[size - 1, size];
    }
    void Initiate()
    {
        clickedObject = null;
        autoMode = true;
        canInput = true;
        mid = size / 2f;
        grid = 2 * edgeOfBoard / size;
        scale = (8f / size) * edgeOfBoard / 2f;
        
        ClearBoard();
        CreateBoard();
        ImportBarrier();

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                NewCircle(i, j);
            }
        }
        AutoProgress();
        score = 0;
    }
    void ClearBoard()
    {
        foreach (Circle circle in board)
        {
            if (circle != null && circle.circleObject != null)
                Destroy(circle.circleObject);
        }
        foreach (Barrier barrier in barrierH)
        {
            if (barrier != null && barrier.barrierObject != null)
            {
                Destroy(barrier.barrierObject);
            }
        }
        foreach (Barrier barrier in barrierV)
        {
            if (barrier != null && barrier.barrierObject != null)
            {
                Destroy(barrier.barrierObject);
            }
        }
    }
    void ImportBarrier()
    {
        int mid = size / 2 - 1;
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (j != size - 1)
                {
                    barrierH[i, j] = new Barrier(/*j == mid ? true : */false, Instantiate(standardBarrier, gameObject.transform));
                    TransformPositionOfBarrier(barrierH, i, j);
                    DirectingBarrier(barrierH, i, j);
                }
                if (i != size - 1)
                {
                    barrierV[i, j] = new Barrier(/*i == mid ? true : */false, Instantiate(standardBarrier, gameObject.transform));
                    TransformPositionOfBarrier(barrierV, i, j);
                    DirectingBarrier(barrierV, i, j);
                }
            }
        }
    }

    Circle NewCircle(int x, int y)
    {
        var circle = new Circle(gameObject.transform, circleSettings);
        board[x, y] = circle;
        TransformPositionOfCircle(x, y);
        return circle;
    }
    
    void DirectingBarrier(Barrier[,] b, int x, int y)
    {
        Barrier barrier = b[x, y];
        GameObject barrierObject = barrier.barrierObject;
        bool value = barrier.value;
        int sprite = -1;
        SpriteRenderer spriteR = barrierObject.GetComponent<SpriteRenderer>();
        barrierObject.transform.localScale = new Vector3(scale, scale, 1);
        if (!value)
        {
            spriteR.sprite = null;
            return;
        }
        if (b == barrierH) { sprite = 0; }
        else if (b == barrierV) { sprite = 1; }
        spriteR.sprite = barrierSprites[sprite];
    }

    void TransformPositionOfCircle(int x, int y)
    {
        if (board[x, y] != null)
            board[x, y].circleObject.transform.position = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y - mid) * grid);
    }
    void TransformPositionOfBarrier(Barrier[,] b, int x, int y)
    {
        if (b == barrierH)
        {
            barrierH[x, y].barrierObject.transform.position = new Vector3(grid / 2 + (x - mid) * grid, grid + (y - mid) * grid);
            return;
        }
        if (b == barrierV)
        {
            barrierV[x, y].barrierObject.transform.position = new Vector3(grid + (x - mid) * grid, grid / 2 + (y - mid) * grid);
            return;
        }
    }
    void MoveCircle(int xi, int yi, int xf, int yf)
    {
        Circle tempCircle = board[xf, yf];
        board[xf, yf] = board[xi, yi];
        board[xi, yi] = tempCircle;
        TransformPositionOfCircle(xi, yi);
        TransformPositionOfCircle(xf, yf);
    }
    void MoveCircle(IntVector2 vectori, IntVector2 vectorf)
    {
        MoveCircle(vectori.x, vectori.y, vectorf.x, vectorf.y);
    }
    void MoveBarrier(Barrier[,] bi, int xi, int yi, Barrier[,] bf, int xf, int yf)
    {
        Barrier tempBarrier = bf[xf, yf];
        bf[xf, yf] = bi[xi, yi];
        bi[xi, yi] = tempBarrier;

        TransformPositionOfBarrier(bi, xi, yi);
        TransformPositionOfBarrier(bf, xf, yf);
    }
    void MoveBarrier(Barrier[,] bi, IntVector2 vectori, Barrier[,] bf, IntVector2 vectorf)
    {
        MoveBarrier(bi, vectori.x, vectori.y, bf, vectorf.x, vectorf.y);
    }

    void Refill()
    {
        bool loop = true;
        while (loop)
        {
            loop = RefillOnce();
        }
    }
    bool RefillOnce()
    {
        bool done = false;
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[i, j] == null)
                {
                    if (j != size - 1)
                    {
                        MoveCircle(i, j, i, j + 1);
                        done = true;
                    }
                    else
                    {
                        NewCircle(i, j);
                    }
                }
            }
        }
        return done;
    }
    Circle CircleOfPresentPos(IntVector2 vector)
    {
        return board[vector.x, vector.y];
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

    void RotateBoard(bool isClockwise)
    {
        Circle[,] tempBoard = new Circle[size, size];
        Barrier[,] tempBarrierH = new Barrier[size, size - 1];
        Barrier[,] tempBarrierV = new Barrier[size - 1, size];
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
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (isClockwise)
                { board[i, j] = tempBoard[size - 1 - j, i]; }
                else
                { board[i, j] = tempBoard[j, size - 1 - i]; }

                TransformPositionOfCircle(i, j);

                if (j != size - 1)
                {
                    if (isClockwise)
                    { barrierH[i, j] = tempBarrierV[size - 2 - j, i]; }
                    else
                    { barrierH[i, j] = tempBarrierV[j, size - 1 - i]; }

                    TransformPositionOfBarrier(barrierH, i, j);
                    DirectingBarrier(barrierH, i, j);
                }

                if (i != size - 1)
                {

                    if (isClockwise)
                    { barrierV[i, j] = tempBarrierH[size - 1 - j, i]; }
                    else
                    { barrierV[i, j] = tempBarrierH[j, size - 2 - i]; }

                    TransformPositionOfBarrier(barrierV, i, j);
                    DirectingBarrier(barrierV, i, j);
                }
            }
        }

        MoveBubbleAndStone();
        AutoProgress();
    }
    void MoveBubbleAndStone()
    {
        bool loop = true;
        while (loop)
        {
            loop = MoveBubble() || MoveStone();
        }
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

    public void SetClickedObject(GameObject clicked)
    {
        clickedObject = clicked;
    }
    IntVector2 PositionOfCircleObject(GameObject circleObject)
    {
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[i, j].circleObject == circleObject)
                {
                    return new IntVector2(i, j);
                }
            }
        }
        return null;
    }

    public int GetScore()
    {
        return score;
    }
    void AddScore(int s)
    {
        score += s;
    }
    void RefreshScore()
    {
        string newScoreText = "Score : " + score;
        scoreRenderer.text = newScoreText;
    }

    void Update()
    {
        KeyInput();
    }
    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Initiate();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            autoMode = !autoMode;
            Debug.Log("Auto Mode : " + autoMode);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateBoard(true);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateBoard(false);
            return;
        }

        if (!autoMode)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
            {
                Refill();
                return;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                RefillOnce();
                return;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                var checker = new Checker(size, board) {
                    RefreshScore = RefreshScore,
                    AddScore = AddScore,
                    NewCircle = NewCircle
                };
                checker.Check();
                return;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                MoveBubbleAndStone();
                return;
            }
        }

        if (clickedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SwapObjects(0, 1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                SwapObjects(-1, 0);
                return;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                SwapObjects(0, -1);
                return;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                SwapObjects(1, 0);
                return;
            }
        }
    }
    public void InputMouse(int deltaX, int deltaY)
    {
        if (clickedObject != null)
        {
            SwapObjects(deltaX, deltaY);
        }
    }
    void SwapObjects(int deltaX, int deltaY)
    {
        IntVector2 posI = PositionOfCircleObject(clickedObject);
        IntVector2 posF = new IntVector2(posI.x + deltaX, posI.y + deltaY);
        if (IsInsideOfRange(posF))
        {
            MoveCircle(posI, posF);
        }
        clickedObject = null;
        AutoProgress();
    }
    void AutoProgress()
    {
        bool check;
        while (true)
        {
            var checker = new Checker(size, board) {
                RefreshScore = RefreshScore,
                AddScore = AddScore,
                NewCircle = NewCircle
            };
            check = checker.Check();

            Refill();
            MoveBubbleAndStone();
            if (check) { break; }
        }
    }
}