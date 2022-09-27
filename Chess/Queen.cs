using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Queen : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public override string Type { get; protected set; }
        public new int Player { get; protected set; }
        public override bool HasNotMoved { get; protected set; }
        
        // Fields
        public static readonly (int RowMove, int ColMove)[] queenMoves = new (int RowMove, int ColMove)[]
        { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) };

        // Constructor
        public Queen(int player) : base(player)
        {
            Type = "Queen";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["Queen"];
            Value = 9;
        }

        // Methods
        public override bool IsValidMove(GameManager gameManager, (int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (gameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if (newPos.Row - curPos.Row == 0 || newPos.Col - curPos.Col == 0)
            {
                if (gameManager.IsStraightBlocked(curPos, newPos))
                    return false;

                return true;
            }

            else if (Math.Abs(newPos.Row - curPos.Row) == Math.Abs(newPos.Col - curPos.Col))
            {
                if (gameManager.IsDiagonalBlocked(curPos, newPos))
                    return false;

                return true;
            }
                
            return false;
        }

        public override bool CanMove(GameManager gameManager, (int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in queenMoves)
            {
                (int Row, int Col) newPos = (curPos.Row + RowMove, curPos.Col + ColMove);

                if (newPos.Row >= 0 && newPos.Row < 8 && newPos.Col >= 0 && newPos.Col < 8)
                    if (IsValidMove(gameManager, curPos, newPos))
                    {
                        gameManager.UpdateBoard(curPos, newPos, true, out bool success);
                        if (success)
                            return true;
                    }
            }

            return false;
        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }

        public static void Define()
        {
            Console.WriteLine("HELP: Learn about the Queen\n\nSymbol: Q\tMaterial Value: 9\n\nThe Queen is one of the most versatile pieces on the board, " +
                "able to move in any straight or diagonal line for any unblocked distance. Each player starts the game with only one Queen." +
                "The Queen is unable to hop over pieces and so cannot move past friendly pieces if they are in its path, " +
                "but can capture the opponent's piece if it moves to the occupied square. The Queen cannot move past an opponent's " +
                "piece. The Queen is usually protected in the early game and brought into play in the mid game.\n\n" +
                "STARTING POSITIONS:\t\t\t\t\tPOSSIBLE MOVEMENT:\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----" +
                "+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  . |    |    |    |" +
                "  . |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    | " +
                "   |    |    |    |    |    |\t\t|  . |    |    |  . |    |    |  . |    |\r\n+----+----+----+----+----+----+--" +
                "--+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |  ." +
                " |    |  . |    |  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----" +
                "+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |  . |  . |  . |    |    |    |\r\n+----+" +
                "----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    | " +
                "   |    |    |\t\t|  . |  . |  . |  Q |  . |  . |  . |  . |\r\n+----+----+----+----+----+----+----+----+\t\t+--" +
                "--+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |  . |  . |  ." +
                " |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n" +
                "|    |    |    |    |    |    |    |    |\t\t|    |  . |    |  . |    |  . |    |    |\r\n+----+----+----+----+" +
                "----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |  Q |    |    |    |    |" +
                "\t\t|  . |    |    |  . |    |    |  . |    |\r\n+----+----+----+----+----+----+----+----+ \t\t+----+----+----+--" +
                "--+----+----+----+----+\r\n\nPress enter to exit.");
        }

    }
}
