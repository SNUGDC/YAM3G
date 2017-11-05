
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
    int multiplier;

    static int _score;
    public static int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            scoreRenderer.text = "Score : " + _score; 
        }
    }
    
    public static int highestScore;
    public static int bestScore;
    int _turn;
    int turn 
    {
        get { 
            return _turn; 
        } 
        set { 
            _turn = value; 
            turnRenderer.text = "Turn : " + value; 
        }
    }
    int combo;
    private Circle[,] board;
    private Barrier[,] barrierH;
    private Barrier[,] barrierV;
    public Sprite[] barrierSprites;
    public GameObject standardBarrier;
    GameObject clickedObject;
    public Text standardScoreRenderer;
    static Text scoreRenderer;
    public Text turnRenderer;
    public int initialTurn;
    bool autoMode;
    bool isPlayingAnimation;
    
    void Awake()
    {
        ImportScores();
        ImportSettings();
        CreateBoard();
        Initiate();
    }
    void Start()
    {
        Screen.SetResolution(405, 720, false);
    }
    void ImportScores()
    {
        scoreRenderer = standardScoreRenderer;
        highestScore = 0;
        SaveLoad.Load();
        bestScore = SaveLoad.bestScore;
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
        isPlayingAnimation = false;
        PopupController.Initiate();
        SoundManager.Initiate();

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
        
        combo = 0;
        multiplier = 1;
        score = 0;
        turn = initialTurn;
        StartCoroutine(AutoProgress(2));
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
                    barrierH[i, j] = new Barrier(false, Instantiate(standardBarrier, gameObject.transform));
                    TransformPositionOfBarrier(barrierH, i, j);
                    DirectingBarrier(barrierH, i, j);
                }
                if (i != size - 1)
                {
                    barrierV[i, j] = new Barrier(false, Instantiate(standardBarrier, gameObject.transform));
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
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PopupController.SwitchPopup();
		}
	}

    bool CanInput()
    {
        return 
            !isPlayingAnimation
            && !PopupController.isActive;
    }
    public void Restart()
    {
        StopAllCoroutines();
        Initiate();
    }
    public void InputRotate(bool isClockwise)
    {
        if (CanInput() && turn > 0)
        {   
            if (isClockwise)
            {   
                StartCoroutine(RotateBoard(AngularDirection.Clockwise));
            }   
            else
            {
                StartCoroutine(RotateBoard(AngularDirection.AntiClockwise));
            }
        }
    }
    public void InputMouse(int deltaX, int deltaY)
    {
        if (clickedObject != null && CanInput() && turn > 0)
        {
            StartCoroutine(SwapObjects(deltaX, deltaY));
        }
    }
    IEnumerator SwapObjects(int deltaX, int deltaY)
    {
        isPlayingAnimation = true;
        IntVector2 posI = PositionOfCircleObject(clickedObject);
        IntVector2 posF = new IntVector2(posI.x + deltaX, posI.y + deltaY);
        if (IsInsideOfRange(posF))
        {
            combo = 1;
            multiplier = 1;
            var swaper = new Swaper(size, board, boardSettings);
            yield return swaper.DoSwap(posI, posF);
            clickedObject = null;

            var checker = new Checker(size, board, combo, multiplier);
            yield return checker.DoCheck();

            if (checker.Done)
            {
                yield return swaper.DoSwap(posI, posF);
                isPlayingAnimation = false;
            }
            else
            {
                turn--;
            }
            StartCoroutine(AutoProgress(1));
        }
        else
        {
            isPlayingAnimation = false;
            yield return null;
        }
    }
    
    IEnumerator RotateBoard(AngularDirection dir)
    {
        isPlayingAnimation = true;
        combo = 1;
        multiplier = 2;
        turn--;
        var rotater = new Rotater(dir,size,board,barrierH,barrierV,boardSettings);
        yield return rotater.DoRotate();
        StartCoroutine(AutoProgress(2));
    }

    IEnumerator AutoProgress(int Refill1_MoveBubbleAndStone2_Check3)
    {
        isPlayingAnimation = true;
        var magicNum = Refill1_MoveBubbleAndStone2_Check3;
        var loop = true;
        while (loop)
        {
            if (magicNum < 2)
            {
                yield return Refill();
            }

            if (magicNum < 3)
            {  
                yield return MoveBubbleAndStone();
                combo++;
            }
            
            if (magicNum < 4)
            {
                var checker = new Checker(size, board, combo, multiplier);
                yield return checker.DoCheck();
                loop = !checker.Done;            
            }
            magicNum = 1;
        }
        isPlayingAnimation = false;
        if (turn == 0)
        {
            var effector = new Effector(size, board, boardSettings);
            yield return effector.Finale();
            ForcePopup();
        }
    }
    IEnumerator Refill()
    {
        var refiller = new Refiller(size, board, boardSettings);
        yield return refiller.DoRefill();
    }
    IEnumerator MoveBubbleAndStone()
    {
        var bsMover = new BubbleStoneMover(size, board, barrierH, boardSettings);
        yield return bsMover.DoBSMove();
    }
    void ForcePopup()
    {
        CommitScore();
        var duration = size * size * aniTime / 4;
        var text = 
            "\n"
            +"Your score is\n" 
            + score + "\n"
            +"\n"
            +"Session Best : " + highestScore + "\n"
            +"All-time Best : " + bestScore;
        PopupController.ForcedPopup(text);
        PopupController.FinaleAction(duration);
    }
    public static void SetScore(int value)
    {
        scoreRenderer.text = "Score : " + score; 
    }
    void CommitScore()
    {
        if (score > highestScore)
        {
            highestScore = score;
        }
        if (score > bestScore)
        {
            bestScore = score;
        }
    }
}