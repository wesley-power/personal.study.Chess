using System;
using System.Collections.Generic;
using System.Threading;

namespace Chess
{
    internal class Program
    {
        public static bool AppOn { get; private set; }
        public static bool MatchOn { get; private set; }
        public static string EndReason { get; private set; }
        public static int Loser { get; private set; }
        public static bool IsTurnComplete { get; private set; }
        public static string MoveFrom { get; private set; }
        public static string MoveTo { get; private set; }
        public static Piece MovingPiece { get; private set; }

        public static int inputRow = 38;

        static void Main(string[] args)
        {
            StartApp();

            while (AppOn)
            {
                StartMatch();
                View.PrintTitleScreen();

                GameManager main = new GameManager();
                main.SetBoard();

                GameManager record = new GameManager();
                record.SetBoard();
                View.PreviousTurns = new List<GameManager> { record };

                main.NextTurn();
                Console.Clear();
                View.PrintDisplay(main, false, main.Turn);

                while (MatchOn)
                {
                    SetTurnComplete(false);
                    MoveFrom = null;
                    MoveTo = null;

                    // Checks for null and invalid inputs are included in GetMove()
                    ((int Row, int Col) moveFrom, (int Row, int Col) moveTo) = GetMove(main);

                    if (!MatchOn)
                        continue;

                    if (!main.Board[moveFrom.Row][moveFrom.Col].IsValidMove(main, moveFrom, moveTo))
                        continue;

                    View.UpdateRemarks(null);
                    main.UpdateBoard(moveFrom, moveTo, false);

                    if (IsTurnComplete)
                    {
                        int player = (main.Turn % 2 == 1) ? 1 : 2;
                        main.UpdateLastMove(MoveFrom, MoveTo, MovingPiece, player);
                        record = new GameManager();
                        record.CopyGameManager(main);
                        View.PreviousTurns.Add(record);

                        main.EvaluateCheck();

                        if (!MatchOn)
                            break;

                        if (main.Turn % 2 == 1)
                            main.King1.RemoveCheck();
                        else
                            main.King2.RemoveCheck();

                        main.EvaluateStaleMate();

                        if (!MatchOn)
                            break;

                        main.NextTurn();
                        View.UpdateRemarks(null);
                        Console.Clear();
                        View.PrintDisplay(main, false, main.Turn);
                    }
                }

                Console.Clear();
                View.PrintDisplay(main, false, main.Turn);

                Console.SetCursorPosition(0, inputRow);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(EndReason + "! ");

                if (Loser == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("BLUE ");
                }
                else if (Loser == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("RED ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("No one ");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("wins!\n\n");


                while (true)
                {
                    Console.SetCursorPosition(0, 40);
                    Console.Write("                                                                                        ");
                    Console.SetCursorPosition(0, 40);
                    Console.Write("Play again? Enter Y or N: ");
                    string reply = Console.ReadLine();
                    reply = reply.ToUpper();

                    if (reply == "Y")
                    {
                        Console.Clear();
                        break;
                    }
                    else if (reply == "N")
                    {
                        Console.WriteLine("\n\nThank you for playing!");
                        Thread.Sleep(2500);
                        Environment.Exit(0);
                    }

                    Console.SetCursorPosition(0, 39);
                    Console.Write("Input not valid.");
                }
            }
        }

        public static ((int, int), (int, int)) GetMove(GameManager gameManager)
        {
            (int Row, int Col) moveFrom = (-1, -1);
            (int Row, int Col) moveTo = (-2, -2);
            bool repeat = false;

            while (moveTo == (-2, -2))
            {
                bool isValidText = false;
                moveFrom = (-1, -1);

                while (moveFrom.Col == -1 || moveFrom.Row == -1)
                {
                    isValidText = false;

                    if (repeat)
                    {
                        Console.Clear();
                        View.PrintDisplay(gameManager, false, gameManager.Turn);
                    }

                    repeat = true;
                    int player = (gameManager.Turn % 2 == 1) ? 1 : 2;

                    PrintRemarks();

                    PrintInstructions(1);

                    Console.SetCursorPosition(0, inputRow);
                    Console.Write("                                                                                           ");
                    Console.SetCursorPosition(0, inputRow);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Move piece on: ");
                    Console.ForegroundColor = ConsoleColor.White;

                    moveFrom = ConvertToCoordinates(gameManager, 1, ref isValidText);

                    // force exit due to game over
                    if (moveFrom == (-3, -3))
                        return ((-3, -3), (-3, -3));

                    if (isValidText)
                    {
                        continue;
                    }

                    else if (moveFrom == (-1, -1))
                    {
                        View.UpdateRemarks(Reference.error[1]);
                        continue;
                    }

                    else if (gameManager.Board[moveFrom.Row][moveFrom.Col] == null)
                    {
                        View.UpdateRemarks(Reference.error[2]);
                        moveFrom = (-1, -1);
                    }

                    else if (gameManager.Board[moveFrom.Row][moveFrom.Col].Player != player)
                    {
                        View.UpdateRemarks(Reference.error[2]);
                        moveFrom = (-1, -1);
                    }
                }

                View.UpdateRemarks(null);
                MovingPiece = gameManager.Board[moveFrom.Row][moveFrom.Col];

                while (moveTo.Col == -1 || moveTo.Row == -1 || moveTo == (-2, -2))
                {
                    PrintRemarks();

                    PrintInstructions(2);

                    Console.SetCursorPosition(0, inputRow);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Move " + MovingPiece.Type + " on " + MoveFrom + " to: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    moveTo = ConvertToCoordinates(gameManager, 2, ref isValidText);

                    // force exit due to game over
                    if (moveTo == (-3, -3))
                        return ((-3, -3), (-3, -3));

                    if (moveTo == (-2, -2))
                        break;
                }
            }

            return (moveFrom, moveTo);
        }

        public static (int, int) ConvertToCoordinates(GameManager gameManager, int pass, ref bool isValidText)
        {
            (int Left, int Top) = (Console.CursorLeft, inputRow);
            Console.Write("                                                                                           ");
            Console.SetCursorPosition(Left, Top);

            string input = Console.ReadLine();
            input = input.ToUpper();

            if (input == "REVIEW")
            {
                isValidText = true;
                View.PrintPreviousTurns(gameManager);
                Console.Clear();
                View.PrintDisplay(gameManager, false, gameManager.Turn);
            }
            else if (input == "HELP")
            {
                isValidText = true;
                Reference.OpenMenu();

                if (pass == 2)
                {
                    Console.Clear();
                    View.PrintDisplay(gameManager, false, gameManager.Turn);
                }
            }
            else if (input == "DRAW")
            {
                isValidText = true;
                if (IsDrawAccepted(gameManager.Turn))
                {
                    EndMatch("DRAW", 0);
                    return (-3, -3);
                }
            }

            else if (input == "RESIGN")
            {
                isValidText = true;
                if (IsResignConfirmed())
                {
                    int player = (gameManager.Turn % 2 == 1) ? 1 : 2;
                    EndMatch("RESIGNATION", player);
                    return (-3, -3);
                }
            }

            /* pass 1 is when member selects a piece to move.
             * pass 2 is selecting what space to move to.
             * The below statement allows the member to start
             * over and select a different piece to move.*/
            if (pass == 2 && input == "CANCEL")
                return (-2, -2);

            /* The below statement identifies an invalid input
             * of two many characters and asktehs player to
             * re-enter a valid input.*/
            else if (input.Length != 2)
                return (-1, -1);

            string file = input.Substring(0, 1);
            int col;

            //Converts letter "file" input to array index (the column of  a jagged array).
            switch (file)
            {
                case "A":
                    col = 0;
                    break;
                case "B":
                    col = 1;
                    break;
                case "C":
                    col = 2;
                    break;
                case "D":
                    col = 3;
                    break;
                case "E":
                    col = 4;
                    break;
                case "F":
                    col = 5;
                    break;
                case "G":
                    col = 6;
                    break;
                case "H":
                    col = 7;
                    break;
                default:
                    {
                        View.UpdateRemarks(Reference.error[4]);
                        return (-1, -1);
                    }
            }

            // default return if second digit of player input is invalid.
            int row = -1;

            // Converts number rank to index of subarray, the row in a jagged array.
            if (Char.IsNumber(input, 1))
            {
                int rank = Convert.ToInt32(input.Substring(1, 1));

                if (rank > 0 && rank <= 8)
                    row = rank - 1;

                else
                {
                    View.UpdateRemarks(Reference.error[4]);
                    return (-1, -1);
                }
            }

            if (MoveFrom == null && pass == 1)
                MoveFrom = input;

            if (MoveTo == null && pass == 2)
                MoveTo = input;

            return (row, col);
        }

        public static void PrintRemarks()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, inputRow - 1);
            if (View.Remarks == null)
                Console.Write("                                                                                        " +
                    "                                                                                                  ");
            else
                Console.Write("Remarks: " + View.Remarks);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintInstructions(int pass)
        {
            Console.SetCursorPosition(0, inputRow + 1);
            Console.WriteLine("                                                                                                                             ");
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, inputRow + 1);
            Console.WriteLine("Enter LetterNumber of square. Example: E2");

            if (pass == 1)
                Console.WriteLine("Enter HELP for menus, REVIEW to look at previous turns, DRAW to offer draw, RESIGN to resign.");
            else
                Console.WriteLine("Enter CANCEL to reselect piece, HELP for menus, REVIEW to look at previous turns, DRAW to offer draw, RESIGN to resign.");
        }

        public static void SetTurnComplete(bool value)
        {
            IsTurnComplete = value;
        }

        public static void StartApp()
        {
            AppOn = true;
        }

        public static void StartMatch()
        {
            MatchOn = true;
        }

        public static bool IsDrawAccepted(int turn)
        {
            Console.SetCursorPosition(0, inputRow - 1);
            Console.WriteLine("                                                                                                                                \n" +
                "                                                                                                                                              \n" +
                "                                                                                                                                              \n" +
                "                                                                                                                                                ");

            string reply; 

            while (true)
            {
                PrintRemarks();

                Console.SetCursorPosition(0, inputRow);
                Console.Write("\"                                                                                                                                       ");
                Console.SetCursorPosition(0, inputRow);

                if (turn % 2 == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("BLUE ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("RED ");
                }

                Console.ForegroundColor= ConsoleColor.White;
                Console.Write("offers a draw. Does ");
                
                if (turn % 2 == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("RED ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("BLUE ");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("accept? Enter Y or N: ");

                reply = Console.ReadLine();
                reply = reply.ToUpper();

                if (reply == "Y" || reply == "N")
                    break;

                View.UpdateRemarks(Reference.error[1]);
            }

            if (reply == "Y")
            {
                return true;
            }
            else
            {
                View.UpdateRemarks(Reference.error[9]);
                return false;
            }
        }

        public static bool IsResignConfirmed()
        {
            Console.SetCursorPosition(0, inputRow - 1);
            Console.WriteLine("                                                                                                                                \n" +
                "                                                                                                                                              \n" +
                "                                                                                                                                              \n" +
                "                                                                                                                                                ");

            while (true)
            {
                PrintRemarks();

                Console.SetCursorPosition(0, inputRow);
                Console.Write("\"                                                                                                                                       ");
                Console.SetCursorPosition(0, inputRow);

                Console.Write("Are you sure you wish to resign? Enter Y or N: ");
                string reply = Console.ReadLine();
                reply = reply.ToUpper();

                if (reply == "Y")
                    return true;
                else if (reply == "N")
                    return false;

                View.UpdateRemarks(Reference.error[1]);
            }
        }


        public static void EndMatch(string endReason, int loser)
        {
            Loser = loser;
            EndReason = endReason;
            MatchOn = false;
        }
    }
}
