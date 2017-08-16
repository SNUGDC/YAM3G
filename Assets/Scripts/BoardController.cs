
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BoardSettings 
{
    public int size;
    public float edgeOfBoard;
    public float Mid { get; private set;}
    public float Grid { get; private set;}
    public float Scale { get; private set;}
    public void Init() 
    {
        Mid = size / 2f;
        Grid = 2 * edgeOfBoard / size;
        Scale = (8f / size) * edgeOfBoard / 2f;
    }
}

public class BoardController : MonoBehaviour
{
    public CircleSettings circleSettings;
    public BoardSettings boardSettings;
    public static float aniTime = 0.125f;
    int size;
    float mid;
    float grid;
    float scale;

    int _score;
    int score 
    {
        get { 
            return _score; 
        } 
        set { 
            _score = value; 
            scoreRenderer.text = "Score : " + value; 
        }
    }
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
        ImportSettings();
        CreateBoard();
        Initiate();
    }
    void Start()
    {
        Screen.SetResolution(360, 640, false);
    }
    void ImportSettings()
    {
        Circle.parent = gameObject.transform;
        Circle.settings = circleSettings;
        boardSettings.Init();
    }
    void CreateBoard()
    {
        size = boardSettings.size;
        board = new Circle[size, size];
        barrierH = new Barrier[size, size - 1];
        barrierV = new Barrier[size - 1, size];
    }
    void Initiate()
    {
        clickedObject = null;
        autoMode = true;
        canInput = true;

        ImportSettings();
        mid = boardSettings.Mid;
        grid = boardSettings.Grid;
        scale = boardSettings.Scale;

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
        
        StartCoroutine(AutoProgress());
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

    public Circle NewCircle(int x, int y)
    {
        var circle = new Circle();
        circle.circleObject.transform.localScale = new Vector3(scale, scale,1);
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
    IEnumerator Refill()
    {
        bool loop = true;
        while (loop)
        {
            var refiller = new Refiller(size, board, boardSettings);
            yield return refiller.DoRefill();
            loop = refiller.Done;
        }
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
        StartCoroutine(AutoProgress());
    }
    IEnumerator MoveBubbleAndStone()
    {
        bool loop = true;
        while (loop)
        {
            var bsMover = new BubbleStoneMover(size, board, barrierH);
            yield return bsMover.DoBSMove();
            loop = bsMover.Done;
        }
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
                //TODO : 
                if (board[i,j] != null && board[i, j].circleObject == circleObject)
                {
                    return new IntVector2(i, j);
                }
            }
        }
        return null;
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
                var refiller = new Refiller(size, board, boardSettings);
                StartCoroutine(refiller.DoRefill());
                return;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                var checker = new Checker(size, board);
                StartCoroutine(checker.DoCheck());
                score += checker.Score;
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
        StartCoroutine(AutoProgress());
    }

    IEnumerator AutoProgress()
    {
        while (true)
        {
            var checker = new Checker(size, board);
            yield return checker.DoCheck();
            score += checker.Score;
            if (checker.Done) { break; }
            
            yield return Refill();

            yield return MoveBubbleAndStone();
        }
    }
}