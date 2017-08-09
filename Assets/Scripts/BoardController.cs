using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using System;
public class IntVector2
{
    public int x;
    public int y;
    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
public class Circle
{
    public int value;
    public GameObject circleObject;
    public Attribution att;
    public Circle() : this(0, null, Attribution.None) { }
    public Circle(int value, GameObject circleObject) : this(value, circleObject, Attribution.None) { }
    public Circle(int value, GameObject circleObject, Attribution att)
    {
        this.value = value;
        this.circleObject = circleObject;
        this.att = att;
    }
}
public class Barrier
{
    public bool value;
    public GameObject barrierObject;
    public Barrier() : this(false, null) { }
    public Barrier(bool value, GameObject barrierObject)
    {
        this.value = value;
        this.barrierObject = barrierObject;
    }
}
public enum Attribution { None, Bubble, Stone };
public class BoardController : MonoBehaviour
{
    public int size = 8;
    public int color = 6;
    public int probOfBS = 32;
    public float edgeOfBoard = 2.5f;
    public float aniTime = 0.5f;
    float mid;
    float grid;
    float scale;
    int score;
    private Circle[,] board;
    private bool[,] checkBoard;
    private Barrier[,] barrierH;
    private Barrier[,] barrierV;
    private List<IntVector2> checkedList;
    public Sprite[] circleNoneSprites;
    public Sprite[] circleBubbleSprites;
    public Sprite[] circleStoneSprites;
    public Sprite[] barrierSprites;
    public GameObject standardCircle;
    public GameObject standardBarrier;
    GameObject clickedObject;
    public Text scoreRenderer;
    bool autoMode;
    bool autoAnimation;
    bool canInput;
    Sequence animation;
    List<Sequence> animationList;

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
        autoAnimation = false;
        canInput = true;
        animationList = new List<Sequence>();
        mid = size / 2f;
        grid = 2 * edgeOfBoard / size;
        scale = (8f / size) * edgeOfBoard / 2f;
        animation = DOTween.Sequence();

        ClearBoard();
        CreateBoard();
        ImportBarrier();
        checkBoard = new bool[size, size];
        checkedList = new List<IntVector2>();

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                board[i, j] = new Circle(NewNumber(), Instantiate(standardCircle, gameObject.transform), NewAttribution());
                TransformPositionOfCircle(i, j);
                ColoringCircle(board[i, j], true);
                checkBoard[i, j] = false;
            }
        }
        AutoProgress(false);
        score = 0;
        Print(false);
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
    int NewNumber()
    {
        return ((int)(UnityEngine.Random.value * color)) + 1;
    }
    Attribution NewAttribution()
    {
        int value = (int)(UnityEngine.Random.value * probOfBS);
        if (value == 0)
        {
            return Attribution.Bubble;
        }
        else if (value == 1)
        {
            return Attribution.Stone;
        }
        else
        {
            return Attribution.None;
        }
    }
    void NewCircle(int x, int y, Sequence seq, bool animate)
    {
        board[x, y].value = NewNumber();
        board[x, y].att = NewAttribution();
        ColoringCircle(board[x, y], !animate);
        if (animate)
        {
            Transform ct = board[x, y].circleObject.transform;
            seq.Join(ct.DOScale(new Vector3(0, 0, 0), 0));
            seq.Join(ct.DOScale(new Vector3(scale, scale, 1), aniTime));
        }
    }
    void Print(bool animate)
    {
        Debug.Log("AnimationList.Count : " + animationList.Count);
        RefreshScore();
        foreach (var anim in animationList)
        {
            animation.Append(anim);
            //animation.Insert(animation.Duration(), animationList[i]);
        }
        Debug.Log("Animation Duration : " + animation.Duration());
        animation.Play();
        animationList = new List<Sequence>();
    }
    void ColoringCircle(Circle circle, bool setScale)
    {
        GameObject circleObject = circle.circleObject;
        int value = circle.value;
        SpriteRenderer spriteR = circleObject.GetComponent<SpriteRenderer>();
        if (setScale)
        {
            circleObject.transform.localScale = new Vector3(scale, scale, 1);
        }
        if (value == 0)
        {
            spriteR.sprite = null;
            return;
        }
        else
        {
            Sprite[] sprites;
            switch (circle.att)
            {
                default:
                    {
                        sprites = circleNoneSprites;
                        break;
                    }
                case Attribution.Bubble:
                    {
                        sprites = circleBubbleSprites;
                        break;
                    }
                case Attribution.Stone:
                    {
                        sprites = circleStoneSprites;
                        break;
                    }
            }
            spriteR.sprite = sprites[value - 1];
            return;
        }
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
        else if (b == barrierV) { sprite = 1; barrierObject.transform.DORotate(new Vector3(0, 0, 90),0); }
        sprite = 0;
        spriteR.sprite = barrierSprites[sprite];
    }

    void TransformPositionOfCircle(int x, int y)
    {
        board[x, y].circleObject.transform.position = new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y - mid) * grid);
    }
    Tween DOMovePositionOfCircle(int x, int y)
    {
        return board[x, y].circleObject.transform.DOMove(new Vector3(grid / 2 + (x - mid) * grid, grid / 2 + (y - mid) * grid), aniTime).OnPlay(() =>
        {
            Debug.Log("DOMovePos : " + Time.time);
        });
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
    Tween DOMovePositionOfBarrierH(int x, int y)
    {
        return barrierH[x, y].barrierObject.transform.DOMove(new Vector3(grid / 2 + (x - mid) * grid, grid + (y - mid) * grid), aniTime);
    }
    Tween DOMovePositionOfBarrierV(int x, int y)
    {
        return barrierV[x, y].barrierObject.transform.DOMove(new Vector3(grid + (x - mid) * grid, grid / 2 + (y - mid) * grid), aniTime);
    }
    void MoveCircle(int xi, int yi, int xf, int yf, Sequence seq, bool animate)
    {
        Circle tempCircle = board[xf, yf];
        board[xf, yf] = board[xi, yi];
        board[xi, yi] = tempCircle;

        if (animate)
        {
            seq.Join(DOMovePositionOfCircle(xi, yi)).OnPlay(() =>
            {
                Debug.Log("MoveCircle seqJoin : " + Time.time);
            });
            seq.Join(DOMovePositionOfCircle(xf, yf)).OnPlay(() =>
            {
                Debug.Log("MoveCircle seqJoin : " + Time.time);
            });
        }
        else
        {
            TransformPositionOfCircle(xi, yi);
            TransformPositionOfCircle(xf, yf);
        }
    }
    void MoveCircle(IntVector2 vectori, IntVector2 vectorf, Sequence sequence, bool animate)
    {
        MoveCircle(vectori.x, vectori.y, vectorf.x, vectorf.y, sequence, animate);
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

    void Refill(bool animate)
    {
        Sequence fallAnimation = DOTween.Sequence();
        Sequence createAnimation = DOTween.Sequence();
        bool loop = true;
        while (loop)
        {
            Sequence fallingSeq = DOTween.Sequence();
            Sequence creatingSeq = DOTween.Sequence();
            loop = RefillOnce(fallingSeq, creatingSeq, animate);
            fallAnimation.Insert(fallAnimation.Duration(), fallingSeq).OnPlay(() =>
            {
                Debug.Log("FallAnim Append : " + Time.time);
            });
            createAnimation.Insert(createAnimation.Duration(), creatingSeq).OnPlay(() =>
            {
                Debug.Log("CreateAnim Append : " + Time.time);
            });
        }
        animationList.Add(fallAnimation);
        //animation.Insert(animation.Duration(), fallAnimation);
        animationList.Add(createAnimation);
        //animation.Insert(animation.Duration(), createAnimation);
    }
    bool RefillOnce(Sequence moveSeq, Sequence newSeq, bool animate)
    {
        bool done = false;
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[i, j].value == 0)
                {
                    if (j != size - 1)
                    {
                        MoveCircle(i, j, i, j + 1, moveSeq, animate);
                        done = true;
                    }
                    else
                    {
                        NewCircle(i, j, newSeq, animate);
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

    void RotateBoard(bool isClockwise, bool animate)
    {
        Sequence rotateAnimation = DOTween.Sequence();
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
                if (animate)
                {
                    rotateAnimation.Insert(0, DOMovePositionOfCircle(i, j)).OnPlay(() =>
                    {
                        Debug.Log("RotateAnim CircleJoin : " + Time.time);
                    });
                }
                else
                {
                    TransformPositionOfCircle(i, j);
                }

                if (j != size - 1)
                {
                    if (isClockwise)
                    { barrierH[i, j] = tempBarrierV[size - 2 - j, i]; }
                    else
                    { barrierH[i, j] = tempBarrierV[j, size - 1 - i]; }
                    if (animate)
                    {
                        rotateAnimation.Insert(0, DOMovePositionOfBarrierH(i, j)).OnPlay(() =>
                        {
                            Debug.Log("RotateAnim BarrierHJoin : " + Time.time);
                        });
                        rotateAnimation.Insert(0, DORotateBarrierH(i, j, isClockwise)).OnPlay(() =>
                        {
                            Debug.Log("RotateAnim BarrierHJoin : " + Time.time);
                        });
                    }
                    else
                    {
                        TransformPositionOfBarrier(barrierH, i, j);
                        DirectingBarrier(barrierH, i, j);
                    }
                }

                if (i != size - 1)
                {

                    if (isClockwise)
                    { barrierV[i, j] = tempBarrierH[size - 1 - j, i]; }
                    else
                    { barrierV[i, j] = tempBarrierH[j, size - 2 - i]; }
                    if (animate)
                    {
                        rotateAnimation.Insert(0, DOMovePositionOfBarrierV(i, j)).OnPlay(() =>
                        {
                            Debug.Log("RotateAnim BarrierVJoin : " + Time.time);
                        });
                        rotateAnimation.Insert(0, DORotateBarrierV(i, j, isClockwise)).OnPlay(() =>
                        {
                            Debug.Log("RotateAnim BarrierVJoin : " + Time.time);
                        });
                    }
                    else
                    {
                        TransformPositionOfBarrier(barrierV, i, j);
                        DirectingBarrier(barrierV, i, j);
                    }
                }
            }
        }
        animationList.Add(rotateAnimation);
        //animation.Insert(animation.Duration(), rotateAnimation);
        MoveBubbleAndStone(autoAnimation);
        AutoProgress(autoAnimation);
    }
    void MoveBubbleAndStone(bool animate)
    {
        Sequence moveBnSAnimation = DOTween.Sequence();
        bool loop = true;
        while (loop)
        {
            Sequence movingSeq = DOTween.Sequence();
            loop = MoveBubble(movingSeq, animate) || MoveStone(movingSeq, animate);
            moveBnSAnimation.Insert(moveBnSAnimation.Duration(), movingSeq).OnPlay(() =>
            {
                Debug.Log("MoveBnS PlayTime : " + Time.time);
            });
        }
        animationList.Add(moveBnSAnimation);
        //animation.Insert(animation.Duration(), moveBnSAnimation);
    }
    bool MoveBubble(Sequence seq, bool animate)
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
                    MoveCircle(i, j, i, j + 1, seq, animate);
                    done = true;
                }
            }
        }
        return done;
    }
    bool MoveStone(Sequence seq, bool animate)
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
                    MoveCircle(i, j, i, j - 1, seq, animate);
                    done = true;
                }
            }
        }
        return done;
    }
    Tween DORotateBarrierH(int x, int y, bool isClockwise)
    {
        int clock = -1;
        if (!isClockwise) { clock = 1; }
        return barrierH[x, y].barrierObject.transform.DORotate(new Vector3(0, 0, 0), aniTime).OnPlay(() =>
        {
            Debug.Log("DORotate BarrierH : " + Time.time);
        });
    }
    Tween DORotateBarrierV(int x, int y, bool isClockwise)
    {
        int clock = -1;
        if (!isClockwise) { clock = 1; }
        return barrierV[x, y].barrierObject.transform.DORotate(new Vector3(0, 0, 90), aniTime).OnPlay(() =>
        {
            Debug.Log("DORotate BarrierV : " + Time.time);
        });
    }

    bool Check(bool animate)
    {
        Sequence deleteAnimation = DOTween.Sequence();
        checkedList.Clear();
        bool done = true;
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                CheckOnce(i, j, 0, animate);
            }
        }
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                if (checkBoard[i, j])
                {
                    DeleteCircle(i, j, deleteAnimation, animate);
                    done = false;
                }
                checkBoard[i, j] = false;
            }
        }
        animationList.Add(deleteAnimation);
        //animation.Insert(animation.Duration(), deleteAnimation);
        return done;
    }
    void DeleteCircle(int x, int y, Sequence seq, bool animate)
    {
        board[x, y].value = 0;
        Transform ct = board[x, y].circleObject.transform;
        RefreshScore();
        if (animate)
        {
            seq.Join(ct.DOScale(new Vector3(0, 0, 0), aniTime)).OnPlay(() =>
            {
                Debug.Log("DeleteAnim Join : " + Time.time);
            });
        }
        else
        {
            ct.localScale = new Vector3(0, 0, 0);
        }
    }
    void CheckOnce(int x, int y, int recursiveNum, bool animate)
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
                CheckOnce(x0, y, recursiveNum + 1, animate);
                CheckOnce(x1, y, recursiveNum + 1, animate);
                CheckOnce(x2, y, recursiveNum + 1, animate);
            }
            if (doForY)
            {
                CheckOnce(x, y0, recursiveNum + 1, animate);
                CheckOnce(x, y1, recursiveNum + 1, animate);
                CheckOnce(x, y2, recursiveNum + 1, animate);
            }
            if ((doForX || doForY) && recursiveNum == 0)
            {
                Debug.Log("a mount of circles");
            }
        }
    }
    void CheckOnBoard(int x, int y)
    {
        if (checkBoard[x, y] == false)
        {
            checkBoard[x, y] = true;
            AddScore(1);
        }
    }
    int[] ThreeNumber(int n)
    {
        if (n == 0) { return new int[] { 0, 1, 2 }; }
        else if (n == size - 1) { return new int[] { size - 3, size - 2, size - 1 }; }
        else { return new int[] { n - 1, n, n + 1 }; }
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            autoAnimation = !autoAnimation;
            Debug.Log("Auto Animation : " + autoAnimation);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateBoard(true, autoAnimation);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateBoard(false, autoAnimation);
            return;
        }

        if (!autoMode)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
            {
                Refill(autoAnimation);
                return;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                RefillOnce(animation, animation, autoAnimation);
                Print(autoAnimation);
                return;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Check(autoAnimation);
                return;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                MoveBubbleAndStone(autoAnimation);
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
        Sequence swapAnimation = DOTween.Sequence();
        IntVector2 posI = PositionOfCircleObject(clickedObject);
        IntVector2 posF = new IntVector2(posI.x + deltaX, posI.y + deltaY);
        if (IsInsideOfRange(posF))
        {
            MoveCircle(posI, posF, swapAnimation, autoAnimation);
        }
        clickedObject = null;
        //animationList.Add(swapAnimation);
        animation.Insert(animation.Duration(), swapAnimation).OnPlay(() =>
        {
            Debug.Log("SwapAnim Append : " + Time.time);
        });
        AutoProgress(autoAnimation);
        /*
        if (autoMode)
        {
            AutoProgress(autoAnimation);
        }
        */
    }
    void AutoProgress(bool animate)
    {
        bool check;
        while (true)
        {
            check = Check(animate);
            Refill(animate);
            MoveBubbleAndStone(animate);
            if (check) { break; }
        }
        Print(animate);
    }
}