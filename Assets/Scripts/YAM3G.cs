using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace YAM3G
{
    enum Direction { Up, Down, Left, Right };
    class Grid
    {
        private int size;
        private int color;
        private int score;
        private int movement;
        private int[,,] grid;
        public Grid() : this(4, 4) { }
        public Grid(int s, int c)
        {
            size = s;
            color = c;
            score = 0;
            movement = 0;
            grid = new int[size, size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        SetValue(0, x, y, z);
                    }
                }
            }
            Refill(false);
            Check(false);
            score = 0;
        }

        public int GetValue(int x, int y, int z) { return grid[x, y, z]; }
        public void SetValue(int value, int x, int y, int z) { grid[x, y, z] = value; }
        public void SetScore(int value) { score = value; }
        char Renderer(int i)
        {
            switch (i)
            {
                case 0: { return ' '; }
                case 1: { return 'A'; }
                case 2: { return 'B'; }
                case 3: { return 'C'; }
                case 4: { return 'D'; }
                case 5: { return 'E'; }
                case 6: { return 'F'; }
                case 7: { return 'G'; }
                case 8: { return 'H'; }
                case 9: { return 'I'; }
                default: { return '_'; }
            }
        }
        public void Print()
        {
            for (int y = size - 1; y > -1; y--)
            {
                for (int x = 0; x < size; x++)
                {
                    Console.Write("[");
                    for (int z = 0; z < size; z++)
                    {
                        Console.Write(Renderer(GetValue(x, y, z)));
                    }
                    Console.Write("]");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Score : {0}    Movement : {1}", score, movement);
            Console.WriteLine();
        }

        void AddScore(int value, bool display)
        {
            score += value;
            if (display) { Print(); }
        }

        void MoveX(int yI, int zI, int yF, int zF)
        {
            int[,,] tempGrid = new int[size, 1, 1];
            for (int x = 0; x < size; x++)
            {
                tempGrid[x, 0, 0] = GetValue(x, yF, zF);
                SetValue(GetValue(x, yI, zI), x, yF, zF);
                SetValue(tempGrid[x, 0, 0], x, yI, zI);
            }
        }
        void MoveY(int xI, int zI, int xF, int zF)
        {
            int[,,] tempGrid = new int[1, size, 1];
            for (int y = 0; y < size; y++)
            {
                tempGrid[0, y, 0] = GetValue(xF, y, zF);
                SetValue(GetValue(xI, y, zI), xF, y, zF);
                SetValue(tempGrid[0, y, 0], xI, y, zI);
            }
        }
        public void Rotate(Direction dir)
        {
            if (size % 2 == 0)
            {
                int layer = size / 2;
                for (int l = 0; l < layer; l++)
                {
                    int min = layer - l - 1;
                    int max = layer + l;
                    RotatingProgress(min, max, 2 * l + 1, dir);
                }
            }
            else
            {
                int layer = (size - 1) / 2;
                for (int l = 1; l < layer + 1; l++)
                {
                    int min = layer - l;
                    int max = layer + l;
                    RotatingProgress(min, max, 2 * l, dir);
                }
            }
            Print();
        }
        void RotatingProgress(int min, int max, int repeat, Direction dir)
        {
            for (int i = 0; i < repeat; i++)
            {
                int x = max;
                int y = max;
                int z = max;
                switch (dir)
                {
                    case Direction.Right:
                        {
                            while (x > min) { MoveY(x, z, x - 1, z); x--; }
                            while (z > min) { MoveY(x, z, x, z - 1); z--; }
                            while (x < max) { MoveY(x, z, x + 1, z); x++; }
                            while (z < max) { MoveY(x, z, x, z + 1); z++; }
                            MoveY(x, z, x, z - 1);
                            break;
                        }
                    case Direction.Left:
                        {
                            while (z > min) { MoveY(x, z, x, z - 1); z--; }
                            while (x > min) { MoveY(x, z, x - 1, z); x--; }
                            while (z < max) { MoveY(x, z, x, z + 1); z++; }
                            while (x < max) { MoveY(x, z, x + 1, z); x++; }
                            MoveY(x, z, x - 1, z);
                            break;
                        }
                    case Direction.Up:
                        {
                            while (y > min) { MoveX(y, z, y - 1, z); y--; }
                            while (z > min) { MoveX(y, z, y, z - 1); z--; }
                            while (y < max) { MoveX(y, z, y + 1, z); y++; }
                            while (z < max) { MoveX(y, z, y, z + 1); z++; }
                            MoveX(y, z, y, z - 1);
                            break;
                        }
                    case Direction.Down:
                        {
                            while (z > min) { MoveX(y, z, y, z - 1); z--; }
                            while (y > min) { MoveX(y, z, y - 1, z); y--; }
                            while (z < max) { MoveX(y, z, y, z + 1); z++; }
                            while (y < max) { MoveX(y, z, y + 1, z); y++; }
                            MoveX(y, z, y - 1, z);
                            break;
                        }
                }
            }
        }
        void MoveZ(int xI, int yI, int xF, int yF)
        {
            int[,,] tempGrid = new int[1, 1, size];
            for (int z = 0; z < size; z++)
            {
                tempGrid[0, 0, z] = GetValue(xF, yF, z);
                SetValue(GetValue(xI, yI, z), xF, yF, z);
                SetValue(tempGrid[0, 0, z], xI, yI, z);
            }
            movement++;
            Print();
        }
        public void Move(int x, int y, Direction dir)
        {
            switch (dir)
            {
                case Direction.Right:
                    {
                        if (x == size - 1) { Console.WriteLine("cannot move to there!"); break; }
                        else { MoveZ(x, y, x + 1, y); break; }
                    }
                case Direction.Left:
                    {
                        if (x == 0) { Console.WriteLine("cannot move to there!"); break; }
                        else { MoveZ(x, y, x - 1, y); break; }
                    }
                case Direction.Up:
                    {
                        if (y == size - 1) { Console.WriteLine("cannot move to there!"); break; }
                        else { MoveZ(x, y, x, y + 1); break; }
                    }
                case Direction.Down:
                    {
                        if (y == 0) { Console.WriteLine("cannot move to there!"); break; }
                        else { MoveZ(x, y, x, y - 1); break; }
                    }
            }
        }

        public void Check(bool display)
        {
            bool checkDone = true;
            while (checkDone)
            {
                checkDone = false;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        for (int z = 0; z < size; z++)
                        {
                            if (size >= 4)
                            {
                                int meanColor;
                                if (GetValue(FourNumber(x)[1], y, z) == GetValue(FourNumber(x)[2], y, z))
                                {
                                    meanColor = GetValue(FourNumber(x)[1], y, z);
                                    if (meanColor != 0 && GetValue(FourNumber(x)[0], y, z) == meanColor && GetValue(FourNumber(x)[3], y, z) == meanColor)
                                    {
                                        SetValue(0, FourNumber(x)[0], y, z);
                                        SetValue(0, FourNumber(x)[1], y, z);
                                        SetValue(0, FourNumber(x)[2], y, z);
                                        SetValue(0, FourNumber(x)[3], y, z);
                                        AddScore(4, display);
                                        checkDone = true;
                                    }
                                }
                                if (GetValue(x, FourNumber(y)[1], z) == GetValue(x, FourNumber(y)[2], z))
                                {
                                    meanColor = GetValue(x, FourNumber(y)[1], z);
                                    if (meanColor != 0 && GetValue(x, FourNumber(y)[0], z) == meanColor && GetValue(x, FourNumber(y)[3], z) == meanColor)
                                    {
                                        SetValue(0, x, FourNumber(y)[0], z);
                                        SetValue(0, x, FourNumber(y)[1], z);
                                        SetValue(0, x, FourNumber(y)[2], z);
                                        SetValue(0, x, FourNumber(y)[3], z);
                                        AddScore(4, display);
                                        checkDone = true;
                                    }
                                }
                                if (GetValue(x, y, FourNumber(z)[1]) == GetValue(x, y, FourNumber(z)[2]))
                                {
                                    meanColor = GetValue(x, y, FourNumber(z)[1]);
                                    if (meanColor != 0 && GetValue(x, y, FourNumber(z)[0]) == meanColor && GetValue(x, y, FourNumber(z)[3]) == meanColor)
                                    {
                                        SetValue(0, x, y, FourNumber(z)[0]);
                                        SetValue(0, x, y, FourNumber(z)[1]);
                                        SetValue(0, x, y, FourNumber(z)[2]);
                                        SetValue(0, x, y, FourNumber(z)[3]);
                                        AddScore(4, display);
                                        checkDone = true;
                                    }
                                }
                            }
                            if (size >= 3)
                            {
                                int midColor;
                                midColor = GetValue(ThreeNumber(x)[1], y, z);
                                if (midColor != 0 && GetValue(ThreeNumber(x)[0], y, z) == midColor && GetValue(ThreeNumber(x)[2], y, z) == midColor)
                                {
                                    SetValue(0, ThreeNumber(x)[0], y, z);
                                    SetValue(0, ThreeNumber(x)[1], y, z);
                                    SetValue(0, ThreeNumber(x)[2], y, z);
                                    AddScore(3, display);
                                    checkDone = true;
                                }
                                midColor = GetValue(x, ThreeNumber(y)[1], z);
                                if (midColor != 0 && GetValue(x, ThreeNumber(y)[0], z) == midColor && GetValue(x, ThreeNumber(y)[2], z) == midColor)
                                {
                                    SetValue(0, x, ThreeNumber(y)[0], z);
                                    SetValue(0, x, ThreeNumber(y)[1], z);
                                    SetValue(0, x, ThreeNumber(y)[2], z);
                                    AddScore(3, display);
                                    checkDone = true;
                                }
                                midColor = GetValue(x, y, ThreeNumber(z)[1]);
                                if (midColor != 0 && GetValue(x, y, ThreeNumber(z)[0]) == midColor && GetValue(x, y, ThreeNumber(z)[2]) == midColor)
                                {
                                    SetValue(0, x, y, ThreeNumber(z)[0]);
                                    SetValue(0, x, y, ThreeNumber(z)[1]);
                                    SetValue(0, x, y, ThreeNumber(z)[2]);
                                    AddScore(3, display);
                                    checkDone = true;
                                }
                            }
                        }
                    }
                }
                if (checkDone) { Refill(display); }
            }
        }
        int[] FourNumber(int i)
        {
            if (i == 0) { return new int[] { 0, 1, 2, 3 }; }
            else if (i == size - 1 || i == size - 2) { return new int[] { size - 4, size - 3, size - 2, size - 1 }; }
            else { return new int[] { i - 1, i, i + 1, i + 2 }; }
        }
        int[] ThreeNumber(int i)
        {
            if (i == 0) { return new int[] { 0, 1, 2 }; }
            else if (i == size - 1) { return new int[] { size - 3, size - 2, size - 1 }; }
            else { return new int[] { i - 1, i, i + 1 }; }
        }
        void Refill(bool display)
        {
            Random rand = new Random();
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (GetValue(x, y, z) == 0)
                        {
                            bool done = false;
                            for (int tempY = y; tempY < size; tempY++)
                            {
                                if (GetValue(x, tempY, z) != 0)
                                {
                                    SetValue(GetValue(x, tempY, z), x, y, z);
                                    SetValue(0, x, tempY, z);
                                    done = true;
                                    break;
                                }
                            }
                            if (!done)
                            {
                                SetValue(rand.Next(color) + 1, x, y, z);
                            }
                        }
                    }
                }
            }
            if (display) { Print(); }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input size of the cube...");
            int size = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Input number of colors...");
            int color = Convert.ToInt32(Console.ReadLine());

            Grid grid = new Grid(size, color);
            grid.Print();
            bool isInGame = true;
            while (isInGame)
            {
                char action = ' ';
                bool rightAction = false;
                while (!rightAction)
                {
                    Console.WriteLine("Choose your action : [Q]uit, mov[E], or [R]otate");
                    action = Convert.ToChar(Console.ReadLine());
                    if (action == 'q' || action == 'Q')
                    {
                        Console.WriteLine("Ok, good bye.");
                        isInGame = false;
                        rightAction = true;
                    }
                    else if (action == 'e' || action == 'E' || action == 'r' || action == 'R')
                    { rightAction = true; }
                    else
                    {
                        Console.WriteLine("You cannot choose that letter, try again with correct character.");
                        rightAction = false;
                    }
                }
                if (!isInGame) { break; }

                int inputColumn = 0;
                int inputRow = 0;
                char inputDir = ' ';
                if (action == 'r' || action == 'R')
                {
                    bool rightRotation = false;
                    while (!rightRotation)
                    {
                        Console.WriteLine("Select direction of rotation : [W], [A], [S], and [D] for up, left, down and right");
                        inputDir = Convert.ToChar(Console.ReadLine());
                        if (inputDir == 'w' || inputDir == 'W' || inputDir == 'a' || inputDir == 'A' || inputDir == 's' || inputDir == 'S' || inputDir == 'd' || inputDir == 'D')
                        { rightRotation = true; }
                        else
                        {
                            Console.WriteLine("You cannot choose that letter, try again with correct character.");
                            rightRotation = false;
                        }
                    }
                    switch (inputDir)
                    {
                        case 'w': { grid.Rotate(Direction.Up); break; }
                        case 'W': { grid.Rotate(Direction.Up); break; }
                        case 'a': { grid.Rotate(Direction.Left); break; }
                        case 'A': { grid.Rotate(Direction.Left); break; }
                        case 's': { grid.Rotate(Direction.Down); break; }
                        case 'S': { grid.Rotate(Direction.Down); break; }
                        case 'd': { grid.Rotate(Direction.Right); break; }
                        case 'D': { grid.Rotate(Direction.Right); break; }
                        default: { Console.WriteLine("아닛, 어떻게 여기에 온거지?!"); break; }
                    }
                }
                else if (action == 'e' || action == 'E')
                {
                    bool rightColumn = false;
                    while (!rightColumn)
                    {
                        Console.WriteLine("Select a column of the point you want to move...");
                        inputColumn = Convert.ToInt32(Console.ReadLine()) - 1;
                        if (inputColumn > -1 && inputColumn < size)
                        { rightColumn = true; }
                        else
                        {
                            Console.WriteLine("You cannot choose that number, try again with correct integer.");
                            rightColumn = false;
                        }
                    }
                    bool rightRow = false;
                    while (!rightRow)
                    {
                        Console.WriteLine("Select a row of the point you want to move...");
                        inputRow = Convert.ToInt32(Console.ReadLine()) - 1;
                        if (inputRow > -1 && inputRow < size)
                        { rightRow = true; }
                        else
                        {
                            Console.WriteLine("You cannot choose that number, try again with correct integer.");
                            rightRow = false;
                        }
                    }
                    bool rightMovement = false;
                    while (!rightMovement)
                    {
                        Console.WriteLine("Select direction of movement : [W], [A], [S], and [D] for up, left, down and right");
                        inputDir = Convert.ToChar(Console.ReadLine());
                        if (inputDir == 'w' || inputDir == 'W' || inputDir == 'a' || inputDir == 'A' || inputDir == 's' || inputDir == 'S' || inputDir == 'd' || inputDir == 'D')
                        { rightMovement = true; }
                        else
                        {
                            Console.WriteLine("You cannot choose that letter, try again with correct character.");
                            rightMovement = false;
                        }
                    }
                    switch (inputDir)
                    {
                        case 'w': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Up); break; }
                        case 'W': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Up); break; }
                        case 'a': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Left); break; }
                        case 'A': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Left); break; }
                        case 's': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Down); break; }
                        case 'S': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Down); break; }
                        case 'd': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Right); break; }
                        case 'D': { grid.Move(inputColumn, size - 1 - inputRow, Direction.Right); break; }
                        default: { Console.WriteLine("아닛, 어떻게 여기에 온거지?!"); break; }
                    }
                    grid.Check(true);
                }
            }
        }
    }
}