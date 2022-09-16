using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class View
    {
        // Methods
        public static void PrintBoard()
        {
            string file1 = "\t       A    B    C    D    E    F    G    H";
            string file2 = "\t       H    G    F    E    D    C    B    A";
            string rowBorder = "\t    +----+----+----+----+----+----+----+----+";

            if (GameManager.Turn % 2 == 1)
            {
                Console.WriteLine(file1 + "\n");
                Console.WriteLine(rowBorder);

                for (int i = 7; i >= 0; i--)
                {
                    Console.Write("\t " + (i + 1) + "  ");

                    for (int j = 0; j < 8; j++)
                    {
                        Piece piece = GameManager.Board[i][j];

                        if (j == 0)
                            Console.Write("|  ");

                        if (piece != null)
                        {
                            PrintPiece(piece);
                            Console.Write(" |  ");
                        }
                        else
                            Console.Write("  |  ");
                    }

                    Console.Write(" " + (i + 1) + "\n");
                    Console.WriteLine(rowBorder);
                }
                Console.WriteLine("\n" + file1 + "\n");
            }

            else if (GameManager.Turn % 2 == 0)
            {
                Console.WriteLine(file2 + "\n");
                Console.WriteLine(rowBorder);

                for (int i = 0; i < 8; i++)
                {
                    Console.Write("\t " + (i + 1) + "  ");

                    for (int j = 7; j >= 0; j--)
                    {
                        Piece piece = GameManager.Board[i][j];

                        if (j == 7)
                            Console.Write("|  ");

                        if (piece != null)
                        {
                            PrintPiece(piece);
                            Console.Write(" |  ");
                        }

                        else
                            Console.Write("  |  ");
                    }
                    Console.Write(" " + (i + 1) + "\n");
                    Console.WriteLine(rowBorder);
                }
                Console.WriteLine("\n" + file2 + "\n");
            }
        }

        public static void PrintPiece(Piece piece)
        {
            if (piece.Player == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (piece.Player == 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write(piece.Symbol);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintScorePlayer1()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nCaptured: ");
            int score = 0;

            foreach (Piece piece in GameManager.CapturedPieces1)
            {
                PrintPiece(piece);
                Console.Write(" ");
                score += piece.Value;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nScore: " + score + "\n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintScorePlayer2()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nCaptured: ");
            int score = 0;

            foreach (Piece piece in GameManager.CapturedPieces2)
            {
                PrintPiece(piece);
                Console.Write(" ");
                score += piece.Value;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nScore: " + score + "\n\n");
            Console.ForegroundColor= ConsoleColor.White;
        }
    }
}
