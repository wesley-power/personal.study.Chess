using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new GameManager();
            bool appOn = true;

            while (appOn)
            {
                Console.WriteLine("Welcome to Chess! Press Enter to start a new game.");
                Console.ReadLine();

                bool matchOn = true;

                foreach (Piece[] rank in GameManager.Board)
                    Array.Clear(rank, 0, rank.Length);

                GameManager.SetBoard();

                while (matchOn)
                {
                    Console.Clear();

                    View.PrintDisplay();

                    ((int Row, int Col) MoveFrom, (int Row, int Col) MoveTo) = GetMove();

                    if (GameManager.Board[MoveFrom.Row][MoveFrom.Col] != null)
                        if (GameManager.Board[MoveFrom.Row][MoveFrom.Col].IsValidMove(MoveFrom, MoveTo) && GameManager.IsValidPiece(MoveFrom))
                        {
                            GameManager.UpdateBoard(MoveFrom, MoveTo, false);
                            GameManager.EvaluateCheck(); // Currently broken
                            if (GameManager.IsStalemate())
                                GameManager.StaleMate();
                            else
                                GameManager.NextTurn();
                        }
                }
            }
        }

        public static ((int, int), (int, int)) GetMove()
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
                    View.UpdateRemarks(Top, "                                                                                           ");
                    View.UpdateRemarks(Top, "Enter the letter file and number rank of the piece you want to move. Example format: A2.");

                    int player = (GameManager.Turn % 2 == 1) ? 1 : 2;

                    Console.SetCursorPosition(Left, Top);
                    Console.Write("                                                                                           ");
                    Console.SetCursorPosition(Left, Top);
                    Console.Write("Move: ");
                    moveFrom = ConvertToCoordinates(1, ref fileRank);

                    if (GameManager.Board[moveFrom.Row][moveFrom.Col] == null)
                        moveFrom = (-1, -1);

                    else if (GameManager.Board[moveFrom.Row][moveFrom.Col].Player != player)
                        moveFrom = (-1, -1);

                    if (moveFrom.Col == -1 || moveFrom.Row == -1)
                        View.UpdateRemarks(Top, "Invalid input. Input must be letter file and number rank of one of your pieces. Example format: E2");
                }

                string pieceType = GameManager.Board[moveFrom.Row][moveFrom.Col].Type;

                View.UpdateRemarks(Top, "Moving " + pieceType + " on " + fileRank + ". Enter new square to move to, or enter \"CANCEL\" to select a different piece.");

                while (moveTo.Col == -1 || moveTo.Row == -1 || moveTo == (-2, -2))
                {
                    Console.SetCursorPosition(Left, Top);
                    Console.Write("Move to: ");
                    moveTo = ConvertToCoordinates(2, ref fileRank);

                    if (moveTo == (-2, -2))
                        break;

                    else if (moveTo.Col == -1 || moveTo.Row == -1)
                        View.UpdateRemarks(Top, "Invalid input. Input must be letter file and number rank. Example format: E2");
                }
            }

            return (moveFrom, moveTo);
        }

        public static (int, int) ConvertToCoordinates(int pass, ref string fileRank)
        {
            (int Left, int Top) = (Console.CursorLeft, Console.CursorTop);
            Console.Write("                                                                                           ");
            Console.SetCursorPosition(Left, Top);

            fileRank = Console.ReadLine();
            fileRank = fileRank.ToUpper();

            if (pass == 2 && fileRank == "CANCEL")
                return (-2, -2);

            else if (fileRank.Length != 2)
                return (-1, -1);

            string file = fileRank.Substring(0, 1);
            int col;

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
                    col = -1;
                    break;
            }

            int row = -1;

            if (Char.IsNumber(fileRank, 1))
            {
                int rank = Convert.ToInt32(fileRank.Substring(1, 1));

                if (rank > 0 && rank <= 8)
                    row = rank - 1;
            }

            return (row, col);
        }
    }
}
