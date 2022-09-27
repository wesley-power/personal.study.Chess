using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Program
    {
        public static bool AppOn { get; private set; }
        public static bool MatchOn { get; private set; }
        public static string EndReason { get; private set; }
        public static bool IsTurnComplete { get; private set; }

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

                    // Checks for null and invalid inputs are included in GetMove()
                    ((int Row, int Col) MoveFrom, (int Row, int Col) MoveTo) = GetMove(main);

                    if (main.Board[MoveFrom.Row][MoveFrom.Col].IsValidMove(main, MoveFrom, MoveTo))
                    {
                        main.UpdateBoard(MoveFrom, MoveTo, false);

                        if (IsTurnComplete)
                        {
                            main.EvaluateCheck();
                            main.EvaluateStaleMate();

                            if (MatchOn)
                            {
                                record = new GameManager();
                                record.CopyGameManager(main);
                                View.PreviousTurns.Add(record);

                                main.NextTurn();
                                Console.Clear();
                                View.PrintDisplay(main, false, main.Turn);
                            }
                        }
                    }
                }
            }
        }

        public static ((int, int), (int, int)) GetMove(GameManager gameManager)
        {
            (int Left, int Top) = (Console.CursorLeft, Console.CursorTop);
            (int Row, int Col) moveFrom = (-1, -1);
            (int Row, int Col) moveTo = (-2, -2);

            while (moveTo == (-2, -2))
            {
                string fileRank = "";
                moveFrom = (-1, -1);

                while (moveFrom.Col == -1 || moveFrom.Row == -1)
                {
                    Console.Clear();
                    View.PrintDisplay(gameManager, false, gameManager.Turn);

                    View.UpdateRemarks(Top, "                                                                                           ");
                    View.UpdateRemarks(Top, "Enter the letter file and number rank of the piece you want to move. Example format: A2.");

                    int player = (gameManager.Turn % 2 == 1) ? 1 : 2;

                    Console.SetCursorPosition(Left, Top);
                    Console.Write("                                                                                           ");
                    Console.SetCursorPosition(Left, Top);
                    Console.Write("Move: ");
                    moveFrom = ConvertToCoordinates(gameManager, 1, ref fileRank);

                    if (moveFrom == (-1, -1))
                        continue;

                    else if (gameManager.Board[moveFrom.Row][moveFrom.Col] == null)
                        moveFrom = (-1, -1);

                    else if (gameManager.Board[moveFrom.Row][moveFrom.Col].Player != player)
                        moveFrom = (-1, -1);

                    if (moveFrom.Col == -1 || moveFrom.Row == -1)
                        View.UpdateRemarks(Top, "Invalid input. Input must be letter file and number rank of one of your pieces. Example format: E2");
                }

                string pieceType = gameManager.Board[moveFrom.Row][moveFrom.Col].Type;

                View.UpdateRemarks(Top, "Moving " + pieceType + " on " + fileRank + ". Enter new square to move to, or enter \"CANCEL\" to select a different piece.");

                while (moveTo.Col == -1 || moveTo.Row == -1 || moveTo == (-2, -2))
                { 
                    Console.SetCursorPosition(Left, Top);
                    Console.Write("Move " + pieceType + " on " + fileRank + " to: ");
                    moveTo = ConvertToCoordinates(gameManager, 2, ref fileRank);

                    if (moveTo == (-2, -2))
                        break;

                    else if (moveTo.Col == -1 || moveTo.Row == -1)
                        View.UpdateRemarks(Top, "Invalid input. Input must be letter file and number rank. Example format: E2");
                }
            }

            return (moveFrom, moveTo);
        }

        public static (int, int) ConvertToCoordinates(GameManager gameManager, int pass, ref string fileRank)
        {
            (int Left, int Top) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("                                                                                           ");
            Console.SetCursorPosition(Left, Top);

            string input = Console.ReadLine();
            input = input.ToUpper();

            if (input == "REVIEW")
            {
                View.PrintPreviousTurns(gameManager);
            }
            else if (input == "HELP")
            {
                Reference.OpenMenu();

                if (pass == 2)
                {
                    Console.Clear();
                    View.PrintDisplay(gameManager, false, gameManager.Turn);
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

            fileRank = input;
            string file = fileRank.Substring(0, 1);
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
                    return (-1, -1);
            }

            // default return if second digit of player input is invalid.
            int row = -1;

            // Converts number rank to index of subarray, the row in a jagged array.
            if (Char.IsNumber(fileRank, 1))
            {
                int rank = Convert.ToInt32(fileRank.Substring(1, 1));

                if (rank > 0 && rank <= 8)
                    row = rank - 1;
            }
            else
                return (-1, -1);

            return (row, col);
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

        public static void EndMatch(string endReason)
        {
            EndReason = endReason;
            MatchOn = false;
            Console.WriteLine(EndReason); // debug
        }
    }
}
