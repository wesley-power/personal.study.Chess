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
            bool gameActive = true;
            new GameManager();
            GameManager.SetBoard();

            while (gameActive)
            {
                Console.Clear();

                (int First, int Second) = (GameManager.Turn % 2 == 1) ? (2, 1) : (1, 2);

                View.PrintCapturedDisplay(First);

                View.PrintBoard();

                View.PrintCapturedDisplay(Second);

                Console.WriteLine("TURN " + GameManager.Turn);

                if (GameManager.Turn % 2 == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("BLUE");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("RED");
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" to move!\n\n");


                Console.Write("Enter position of piece to move: ");
                (int Row, int Col) firstPos = ConvertToCoordinates();

                Console.Write("Enter the position to move the piece to: ");
                (int Row, int Col) secondPos = ConvertToCoordinates();

                if (GameManager.Board[firstPos.Row][firstPos.Col] != null)
                    if (GameManager.Board[firstPos.Row][firstPos.Col].IsValidMove(firstPos, secondPos) && GameManager.IsValidPiece(firstPos))
                    {
                        GameManager.UpdateBoard(firstPos, secondPos, false);
                        GameManager.EvaluateCheck();
                        if (GameManager.IsDraw())
                            GameManager.Draw();
                    }
            }
        }

        public static (int, int) ConvertToCoordinates()
        {
            string fileRank = Console.ReadLine();

            string file = fileRank.Substring(0, 1).ToUpper();
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

            int row = Convert.ToInt32(fileRank.Substring(1, 1)) - 1;

            return (row, col);
        }
    }
}
