using System;
using System.Threading;
using System.Diagnostics;

namespace Tetris
{


    class Program
    {
        const int Height = 22;
        const int Width = 12;
        static char[,] bg = new char[Height, Width];


        static int score = 0;

        const int holdSizeX = 8;
        const int holdSizeY = 5;

        const int upNextSize = 8;
        const int upNextRows = 5;
        const int upNextCols = 8;
        const int scoreYPosition = 8;

        static int holdIndex = -1;
        static char holdChar;

        static ConsoleKeyInfo input;


        static int currentX = 0;
        static int currentY = 0;
        static char currentChar = 'O';

        static int currentRotation = 0;

        static int selectedOption = 0;

        static int[]? Shape;
        static int[]? nextShape;

        static int ShapeIndex;
        static int currentIndex;


        static int maxSpeed = 20;
        static int Speed = 0;
        static int amount = 0;

        static bool pieceLanded = false;
        static int pauseSelectedOption = 0;
        static bool isPaused = false;
        static bool isPauseMenuDisplayed = false;


        const string LeaderboardFilePath = "leaderboard.txt";

        const int gameWidth = Width + holdSizeX + upNextCols + 10;
        const int gameHeight = Height + 10;


        readonly static string characters = "OILJSZT";
        readonly static int[,,,] positions =
        {
        {
        {{0,1},{1,1},{0,2},{1,2}},
        {{0,1},{1,1},{0,2},{1,2}},
        {{0,1},{1,1},{0,2},{1,2}},
        {{0,1},{1,1},{0,2},{1,2}}
        },

        {
        {{0,2},{1,2},{2,2},{3,2}},
        {{1,0},{1,1},{1,2},{1,3}},
        {{0,1},{1,1},{2,1},{3,1}},
        {{2,0},{2,1},{2,2},{2,3}},
        },
        {
        {{2,1},{2,2},{1,2},{0,2}},
        {{1,0},{1,1},{1,2},{2,2}},
        {{1,2},{1,1},{2,1},{3,1}},
        {{1,1},{2,1},{2,2},{2,3}},
        },

        {
        {{1,1},{1,2},{2,2},{3,2}},
        {{2,1},{1,1},{1,2},{1,3}},
        {{0,1},{1,1},{2,1},{2,2}},
        {{2,0},{2,1},{2,2},{1,2}},
        },

        {
        {{2,1},{1,1},{1,2},{0,2}},
        {{1,0},{1,1},{2,1},{2,2}},
        {{2,1},{1,1},{1,2},{0,2}},
        {{1,0},{1,1},{2,1},{2,2}},
        },

        {
        {{0,1},{1,1},{1,2},{2,2}},
        {{1,0},{1,1},{0,1},{0,2}},
        {{0,1},{1,1},{1,2},{2,2}},
        {{1,0},{1,1},{0,1},{0,2}},
        },

        {
        {{1,0},{0,1},{1,1},{2,1}},
        {{1,0},{1,1},{2,1},{1,2}},
        {{0,1},{1,1},{2,1},{1,2}},
        {{1,0},{0,1},{1,1},{1,2}}
        }
        };

        static void Main()
        {

            Console.SetWindowSize(gameWidth * 2, gameHeight + 2);

            Console.CursorVisible = false;
            Console.Title = "Tetris";

            DisplayMainMenu();

        }

        static void DisplayMainMenu()
        {
            Console.Clear();
 
            Console.ForegroundColor = ConsoleColor.Red;
            int headerX = (gameWidth - "████████╗███████╗████████╗██████╗░██╗░██████╗".Length) / 2;
            int headerY = gameHeight / 2 - 10;


            string[] menuOptions = { "Start Game", "Controls", "Leaderboard", "Exit" };
            int selectedOptionIndex = 0;

            int centerY = gameHeight / 2 + 1;
            int centerX = (gameWidth + 11 - "Start Game".Length) / 2;

            while (true)
            {
                for (int i = 0; i < menuOptions.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(headerX + 17, headerY);
                    Console.WriteLine("████████╗███████╗████████╗██████╗░██╗░██████╗");
                    Console.SetCursorPosition(headerX + 17, headerY + 1);
                    Console.WriteLine("╚══██╔══╝██╔════╝╚══██╔══╝██╔══██╗██║██╔════╝");
                    Console.SetCursorPosition(headerX + 17, headerY + 2);
                    Console.WriteLine("░░░██║░░░█████╗░░░░░██║░░░██████╔╝██║╚█████╗░");
                    Console.SetCursorPosition(headerX + 17, headerY + 3);
                    Console.WriteLine("░░░██║░░░██╔══╝░░░░░██║░░░██╔══██╗██║░╚═══██╗");
                    Console.SetCursorPosition(headerX + 17, headerY + 4);
                    Console.WriteLine("░░░██║░░░███████╗░░░██║░░░██║░░██║██║██████╔╝");
                    Console.SetCursorPosition(headerX + 17, headerY + 5);
                    Console.WriteLine("░░░╚═╝░░░╚══════╝░░░╚═╝░░░╚═╝░░╚═╝╚═╝╚═════╝░");

                    Console.SetCursorPosition((gameWidth - "Select an option:".Length) + 11 / 2, gameHeight / 2);
                    Console.WriteLine("Select an option:");
                    Console.ResetColor();

                    if (i == selectedOptionIndex)
                    {
                        Console.SetCursorPosition(centerX, centerY + i);
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("► ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($"{menuOptions[i]}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(centerX, centerY + i);
                        Console.WriteLine($"  {menuOptions[i]}");
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow && selectedOptionIndex > 0)
                {
                    selectedOptionIndex--;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow && selectedOptionIndex < menuOptions.Length - 1)
                {
                    selectedOptionIndex++;
                }
                else if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    HandleSelectedOption(selectedOptionIndex + 1);
                    break;
                }
            }
        }

        static void HandleSelectedOption(int option)
        {
            switch (option)
            {
                case 1:
                    ResetGame();
                    Console.Clear();
                    StartGame();
                    break;
                case 2:
                    Console.Clear();
                    Control();
                    break;
                case 3:
                    Console.Clear();
                    LoadLeaderboard();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }



        static void StartGame()
        {

            Thread inputThread = new Thread(Input);
            inputThread.Start();

            Shape = GenerateShape();
            nextShape = GenerateShape();
            NewBlock();

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    bg[y, x] = ' ';

            while (true)
            {
                if (!isPaused)
                {
                    int dropSpeed = GetDropSpeed();
                    if (Speed >= dropSpeed)
                    {

                        if (!Collision(currentIndex, bg, currentX, currentY + 1, currentRotation)) currentY++;
                        else BlockDownCollision();

                        Speed = 0;
                    }
                    Speed++;

                    InputHandler();
                    input = new ConsoleKeyInfo();


                    char[,] view = RenderView();

                    char[,] hold = RenderHold();

                    char[,] next = RenderUpNext();

                    Print(view, hold, next);

                }
                else
                {
                    DisplayPauseScreen();
                }

            }
        }
        static void Control()
        {

            Console.Clear();
            int ControlX = (gameWidth - "Tetris Controls".Length) / 2 + 6;
            int ControlY = gameHeight / 2 - 6;

            Console.SetCursorPosition(ControlX + 13, ControlY - 3);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Tetris Controls");


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(ControlX,ControlY);
            Console.WriteLine("Left/Right Arrows (A/D) - Move left/right");
            Console.SetCursorPosition(ControlX, ControlY + 1);
            Console.WriteLine("Up Arrow (W)            - Rotate");
            Console.SetCursorPosition(ControlX, ControlY + 2);
            Console.WriteLine("Down Arrow (S)          - Soft drop");
            Console.SetCursorPosition(ControlX,ControlY + 3);
            Console.WriteLine("Spacebar                - Hard drop");
            Console.SetCursorPosition(ControlX,ControlY + 4);
            Console.WriteLine("C                       - Hold piece");
            Console.SetCursorPosition(ControlX,ControlY + 5);
            Console.WriteLine("Escape                  - Pause Game");

            Console.ReadKey(true);
            DisplayMainMenu();
        }

        static void InputHandler()
        {
            if (!isPaused)
            {
                switch (input.Key)
                {

                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        if (!pieceLanded && !Collision(currentIndex, bg, currentX - 1, currentY, currentRotation))
                            currentX -= 1;
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        if (!pieceLanded && !Collision(currentIndex, bg, currentX + 1, currentY, currentRotation))
                            currentX += 1;
                        break;


                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        int newRot = currentRotation + 1;
                        if (newRot >= 4) newRot = 0;
                        if (!Collision(currentIndex, bg, currentX, currentY, newRot)) currentRotation = newRot;

                        break;

                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        Speed = maxSpeed;
                        break;

                    case ConsoleKey.Spacebar:
                        int i = 0;
                        while (true)
                        {
                            i++;
                            if (Collision(currentIndex, bg, currentX, currentY + i, currentRotation))
                            {
                                currentY += i - 1;
                                pieceLanded = true;
                                break;
                            }

                        }
                        break;

                    case ConsoleKey.C:

                        if (holdIndex == -1)
                        {
                            holdIndex = currentIndex;
                            holdChar = currentChar;
                            NewBlock();
                        }

                        else
                        {
                            if (!Collision(holdIndex, bg, currentX, currentY, 0))
                            {

                                int c = currentIndex;
                                char ch = currentChar;
                                currentIndex = holdIndex;
                                currentChar = holdChar;
                                holdIndex = c;
                                holdChar = ch;
                            }

                        }
                        break;

                    case ConsoleKey.Escape:
                        isPaused = !isPaused;
                        break;

                    case ConsoleKey.OemPlus:
                        score += 500;
                        break;


                    default:
                        break;
                }
            }
        }

        

        static int GetDropSpeed()
        {
            int maxSpeed = 20;
            int minSpeed = 1;

            int speedIncreaseInterval = 500;

            int adjustedSpeed = maxSpeed - (score / speedIncreaseInterval);

            adjustedSpeed = Math.Max(adjustedSpeed, minSpeed);

            return adjustedSpeed;
        }

        static void DisplayCountdown()
        {
            for (int i = 3; i > 0; i--)
            {
                Console.SetCursorPosition(Width / 2 + 24, Height / 2);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Resuming in {i}...");
                Thread.Sleep(1000);
                Console.SetCursorPosition(Width / 2, Height / 2);
                Console.Write(new string(' ', 12));
                Console.Clear();
            }
        }
        static void DisplayPauseScreen()
        {
            Console.Clear();

            string[] pauseOptions = { "Resume Game", "Exit Game" };
            int selectedOptionIndex = 0;

            int centerY = Height / 2;
            int centerX = (gameWidth + 11 - "Resume Game".Length) / 2;


            while (true)
            {
                for (int i = 0; i < pauseOptions.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(Width / 2 + 24, Height / 2 - 2);
                    Console.WriteLine("Game Paused");

                    if (i == selectedOptionIndex)
                    {
                        Console.SetCursorPosition(centerX, centerY + i);
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("► ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write($"{pauseOptions[i]}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(centerX, centerY + i);
                        Console.WriteLine($"  {pauseOptions[i]}");
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow && selectedOptionIndex > 0)
                {
                    selectedOptionIndex--;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow && selectedOptionIndex < pauseOptions.Length - 1)
                {
                    selectedOptionIndex++;
                }
                else if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    HandlePauseOptionSelection(selectedOptionIndex + 1);
                    break;
                }

            }
        }

            static void HandlePauseOptionSelection(int option)
        {
            switch (option)
            {
                case 1:
                    Console.Clear();
                    DisplayCountdown();
                    isPaused = false;
                    break;

                case 2:
                    Environment.Exit(0);
                    break;

                default:
                    break;
            }
        }

        static void BlockDownCollision()
        {
            for (int i = 0; i < positions.GetLength(2); i++)
            {
                bg[positions[currentIndex, currentRotation, i, 1] + currentY, positions[currentIndex, currentRotation, i, 0] + currentX] = currentChar;
            }

            int linesCleared = ClearLines();

            if (linesCleared > 0)
            {
                score += linesCleared switch
                {
                    1 => 100,
                    2 => 300,
                    3 => 500,
                    4 => 800,
                    _ => 0,
                };
            }

            NewBlock();
        }

        static int ClearLines()
        {
            int linesCleared = 0;

            for (int y = Height - 2; y > 0; y--)
            {
                bool isLineFull = true;

                for (int x = 1; x < Width - 1; x++)
                {
                    if (bg[y, x] == ' ')
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    linesCleared++;
                    for (int i = y; i > 0; i--)
                    {
                        for (int x = 1; x < Width - 1; x++)
                        {
                            bg[i, x] = bg[i - 1, x];
                        }
                    }
                    y++;
                }
            }

            return linesCleared;
        }

        static char[,] RenderView()
        {
            char[,] view = new char[Height, Width];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    view[y, x] = bg[y, x];
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (y == 0 || y == Height - 1 || x == 0 || x == Width - 1)
                    {
                        view[y, x] = '#';
                    }
                }
            }

            for (int i = 0; i < positions.GetLength(2); i++)
            {
                int pieceY = positions[currentIndex, currentRotation, i, 1] + currentY;
                int pieceX = positions[currentIndex, currentRotation, i, 0] + currentX;

                if (pieceY >= 0 && pieceY < Height && pieceX >= 0 && pieceX < Width)
                {
                    view[pieceY, pieceX] = currentChar;
                }
            }

            int ghostY = GetGhostYPosition(currentIndex, bg, currentX, currentY, currentRotation);
            for (int i = 0; i < positions.GetLength(2); i++)
            {
                int ghostPieceY = ghostY + positions[currentIndex, currentRotation, i, 1];
                int ghostPieceX = positions[currentIndex, currentRotation, i, 0] + currentX;

                if (ghostPieceY >= 0 && ghostPieceY < Height && ghostPieceX >= 0 && ghostPieceX < Width)
                {
                    if (view[ghostPieceY, ghostPieceX] != currentChar)
                    {
                        view[ghostPieceY, ghostPieceX] = '.';
                    }
                }
            }

            return view;
        }
        static int GetShapeWidth(int index, int rotation)
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;

            for (int i = 0; i < positions.GetLength(2); i++)
            {
                int posX = positions[index % 7, rotation, i, 0];

                if (posX < minX)
                {
                    minX = posX;
                }

                if (posX > maxX)
                {
                    maxX = posX;
                }
            }

            return maxX - minX + 2;
        }

        static char[,] RenderHold()
        {
            char[,] hold = new char[holdSizeY + 2, holdSizeX + 2];

            for (int y = 0; y < holdSizeY + 2; y++)
            {
                for (int x = 0; x < holdSizeX + 2; x++)
                {
                    if (y == 0 || y == holdSizeY + 1 || x == 0 || x == holdSizeX + 1)
                    {
                        hold[y, x] = '#';
                    }
                    else
                    {
                        hold[y, x] = ' ';
                    }
                }
            }

            if (holdIndex != -1)
            {
                int offsetX = (holdSizeX - GetShapeWidth(holdIndex, 0)) / 2 - 1;

                for (int i = 0; i < positions.GetLength(2); i++)
                {
                    int pieceY = positions[holdIndex, 0, i, 1] + 2;
                    int pieceX = positions[holdIndex, 0, i, 0] + 1 + offsetX;

                    if (pieceY >= 0 && pieceY < holdSizeY + 2 && pieceX >= 0 && pieceX < holdSizeX + 2)
                    {
                        hold[pieceY, pieceX] = holdChar;
                    }
                }
            }

            return hold;
        }
        static char[,] RenderUpNext()
        {

            char[,] next = new char[upNextRows + 2, upNextCols + 2];

            for (int y = 0; y < upNextRows + 2; y++)
            {
                for (int x = 0; x < upNextCols ; x++)
                {
                    if (y == 0 || y == upNextRows + 1 || x == -1 || x == upNextCols -1)
                    {
                        next[y, x] = '#'; 
                    }
                    else
                    {
                        next[y, x] = ' ';
                    }
                }
            }

            int nextShapeIndex = 0;

            for (int i = 0; i < 1; i++)
            {
                int offsetX = (upNextCols - GetShapeWidth(Shape[ShapeIndex + i], 0)) / 2 - 2;

                for (int j = 0; j < positions.GetLength(2); j++)
                {
                    if (i + ShapeIndex >= 7)
                    {
                        next[positions[nextShape[nextShapeIndex], 0, j, 1] + 2, positions[nextShape[nextShapeIndex], 0, j, 0] + 2 + offsetX] = characters[nextShape[nextShapeIndex]];
                    }
                    else
                    {
                        next[positions[Shape[ShapeIndex + i], 0, j, 1] + 2, positions[Shape[ShapeIndex + i], 0, j, 0] + 2 + offsetX] = characters[Shape[ShapeIndex + i]];
                    }
                }

                if (i + ShapeIndex >= 7) nextShapeIndex++;
            }

            return next;
        }
        static int GetGhostYPosition(int index, char[,] bg, int x, int y, int rotation)
        {
            int ghostY = y;
            while (!Collision(index, bg, x, ghostY + 1, rotation))
            {
                ghostY++;
            }
            return ghostY;
        }

        static ConsoleColor GetColor(char character)
        {
            switch (character)
            {
                case 'O':
                    return ConsoleColor.Red;
                case 'I':
                    return ConsoleColor.Blue;
                case 'T':
                    return ConsoleColor.Cyan;
                case 'S':
                    return ConsoleColor.DarkMagenta;
                case 'Z':
                    return ConsoleColor.DarkCyan;
                case 'L':
                    return ConsoleColor.Green;
                case 'J':
                    return ConsoleColor.DarkBlue;
                case '.':
                    return ConsoleColor.Gray;
                case '#':
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.Black;
            }
        }
        static void Print(char[,] view, char[,] hold, char[,] next)
        {
            int renderHeight = Math.Min(Height, Math.Max(view.GetLength(0), Math.Max(hold.GetLength(0), next.GetLength(0))));

            int startX = (gameWidth - (Width + holdSizeX + upNextSize - 8)) / 2;
            int startY = (gameHeight - renderHeight) / 2;

            string holdLabel = "HOLD";
            int holdLabelStartX = startX + (holdSizeX - holdLabel.Length) / 2 + 4;
            Console.SetCursorPosition(holdLabelStartX, startY - 1); 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(holdLabel);

            string nextLabel = "NEXT";
            int nextLabelStartX = startX + Width + holdSizeX + (upNextSize - nextLabel.Length) / 2 + 24;
            Console.SetCursorPosition(nextLabelStartX, startY - 1); 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(nextLabel);


            Console.SetCursorPosition(startX, startY);
            for (int y = 0; y < renderHeight; y++)
            {
                Console.SetCursorPosition(startX, startY + y);

                for (int x = 0; x < holdSizeX; x++)
                {
                    if (y < hold.GetLength(0) && x < holdSizeX)
                    {
                        Console.ForegroundColor = GetColor(hold[y, x]);
                        Console.Write(hold[y, x] == ' ' ? "  " : "██");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }


                for (int x = 0; x < Width; x++)
                {
                    if (y < view.GetLength(0) && x < view.GetLength(1))
                    {
                        char i = view[y, x];
                        Console.ForegroundColor = GetColor(i);
                        Console.Write(i == ' ' ? "  " : "██");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }


                for (int x = 0; x < upNextSize + 4; x++)
                {
                    if (y < next.GetLength(0) && x < next.GetLength(1))
                    {
                        Console.ForegroundColor = GetColor(next[y, x]);
                        Console.Write(next[y, x] == ' ' ? "  " : "██");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }

                if (y == scoreYPosition)
                {
                    Console.SetCursorPosition(startX + Width + holdSizeX + upNextSize + 16, startY + y);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Score: {score}");
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, Console.CursorTop - renderHeight);
        }

        static int[] GenerateShape()
        {

            Random random = new Random();
            int n = 7;
            int[] ret = { 0, 1, 2, 3, 4, 5, 6, 7 };
            while (n > 1)
            {
                int k = random.Next(n--);
                int temp = ret[n];
                ret[n] = ret[k];
                ret[k] = temp;

            }
            return ret;

        }
        static bool Collision(int index, char[,] bg, int x, int y, int rot)
        {
            for (int i = 0; i < positions.GetLength(2); i++)
            {
                int posX = positions[index, rot, i, 0] + x;
                int posY = positions[index, rot, i, 1] + y;


                if (posY >= Height - 1 || posX < 1 || posX >= Width - 1)
                {
                    return true;
                }

                if (bg[posY, posX] != ' ')
                {
                    return true;
                }
            }

            return false;
        }


        static int Line(char[,] bg)
        {
            for (int y = 0; y < Height; y++)
            {
                bool i = true;
                for (int x = 0; x < Width; x++)
                {
                    if (bg[y, x] == ' ')
                    {
                        i = false;
                    }
                }
                if (i) return y;
            }


            return -1;
        }

        static void NewBlock()
        {
            pieceLanded = false;

            if (ShapeIndex >= 7)
            {
                ShapeIndex = 0;
                Shape = nextShape;
                nextShape = GenerateShape();
            }

            currentY = 1;
            currentX = 4;
            currentChar = characters[Shape[ShapeIndex]];
            currentIndex = Shape[ShapeIndex];

            if (Collision(currentIndex, bg, currentX, currentY, currentRotation) && amount > 0)
            {
                GameOver();
            }
            ShapeIndex++;
            amount++;
        }

        static void LoadLeaderboard()
        {
            if (File.Exists(LeaderboardFilePath))
            {
                string[] lines = File.ReadAllLines(LeaderboardFilePath);

                Console.Clear();

                int leaderboardHeight = lines.Length + 4;

                int paddingY = (gameHeight - leaderboardHeight) / 4;
                int paddingX = (gameWidth - leaderboardHeight) / 2 + 16;
                Console.SetCursorPosition(paddingX, paddingY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Leaderboard");
                
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string line in lines)
                {
                    int spacesToAdd = ((gameWidth - 3) / 2) + 12;

                    string[] parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        string playerName = parts[0].ToUpper();
                        string playerScore = parts[1];

                        string formattedEntry = $"{playerName.PadRight(7)} : {playerScore.PadLeft(playerScore.Length+3)}";

                        Console.WriteLine(new string(' ', spacesToAdd) + formattedEntry);
                    }
                }
            }
            else
            {
                Console.WriteLine("Leaderboard file not found.");
            }
            Console.ReadKey(true);
            DisplayMainMenu();
        }


        static void UpdateLeaderboard(int playerScore)
        {
            const int MaxEntries = 10;

            int paddingY = (gameHeight) / 2 - 6;
            int paddingX = (gameWidth - "Congratulations! You made it to the leaderboard!".Length) / 2 + 16;

            List<string> leaderboardEntries = File.Exists(LeaderboardFilePath) ? File.ReadAllLines(LeaderboardFilePath).ToList() : new List<string>();

            leaderboardEntries = leaderboardEntries
                .OrderByDescending(entry => int.Parse(entry.Split(':')[1].Trim()))
                .Take(MaxEntries)
                .Select(entry => entry.ToUpper())
                .ToList();

            if (leaderboardEntries.Count == MaxEntries)
            {
                int lowestScore = int.Parse(leaderboardEntries.Last().Split(':')[1].Trim());

                if (playerScore <= lowestScore)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(paddingX, paddingY);
                    Console.WriteLine("Sorry, you didn't make it to the leaderboard.");
                    Console.SetCursorPosition(paddingX + 10, paddingY + 2);
                    Console.WriteLine("Better luck next time!");
                    Console.ReadKey(true);

                    Console.Clear();
                    DisplayMainMenu();
                    return;
                }
            }

            Console.Clear();
            Console.SetCursorPosition(paddingX, paddingY);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Congratulations! You made it to the leaderboard!");
            Console.SetCursorPosition(paddingX, paddingY + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your name (3 letters): ");
            string playerName = Console.ReadLine();

            playerName = playerName.Length > 3 ? playerName.Substring(0, 3) : playerName;
            playerName = playerName.ToUpper();

            string leaderboardEntry = $"{playerName}:{playerScore}";
            leaderboardEntries.Add(leaderboardEntry.ToUpper());

            leaderboardEntries = leaderboardEntries
                .OrderByDescending(entry => int.Parse(entry.Split(':')[1].Trim()))
                .Take(MaxEntries)
                .Select(entry => entry.ToUpper())
                .ToList();

            File.WriteAllLines(LeaderboardFilePath, leaderboardEntries);

            Console.ReadKey(true);
            Console.Clear();
            ResetGame();
            LoadLeaderboard();
        }



        static void GameOver()
        {
            UpdateLeaderboard(score);
        }
        static void ResetGame()
        {
            score = 0;
            ShapeIndex = 0;
            amount = 0;
            isPaused = false;
            isPauseMenuDisplayed = false;
        }
        static void Input()
        {
            while (true)
            {
                input = Console.ReadKey(true);
            }
        }
    }
}