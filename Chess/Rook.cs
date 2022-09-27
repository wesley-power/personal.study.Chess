using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Rook : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public override string Type { get; protected set; }
        public new int Player { get; protected set; }
        public override bool HasNotMoved { get; protected set; }

        // Fields
        public static readonly (int RowMove, int ColMove)[] rookMoves = new (int RowMove, int ColMove)[]
        { (1, 0), (-1, 0), (0, 1), (0, -1) };

        // Constructor
        public Rook(int player) : base(player)
        {
            Type = "Rook";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["Rook"];
            Value = 5;
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

            return false;
        }

        public override bool CanMove(GameManager gameManager, (int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in rookMoves)
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
            Console.WriteLine("HELP: Learn about the Rook\n\nSymbol: R\tMaterial Value: 5\n\nThe Rook may move in any straight, non-diagonal line as far as the" +
                "player wishes, until it reaches either the edge of the board or another piece. The Rook may not hope over friendly or " +
                "opposing pieces, but may capture an opposing piece by stopping on its square. Each player has two Rooks on either end" +
                "of the back rank. The Rook can be moved as part of a \"castle\" move initiated by the King. See King for more details. " +
                "When castling, the rook always takes up an inside position beside the king. Given its starting position, the rook is " +
                "usually not moved in the early game except to castle with the king, but becomes a powerful piece in the mid-game.\n\n" +
                "STARTING POSITIONS:\t\t\t\t\tPOSSIBLE MOVEMENT:\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+" +
                "----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  . |    |    |    |    |\r\n+--" +
                "--+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    " +
                "|    |    |\t\t|    |    |    |  . |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+-" +
                "---+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  . |    |    |    |   " +
                " |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |" +
                "    |    |    |    |\t\t|    |    |    |  . |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+--" +
                "--+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|  . |  . |  . |  R |  . |  . " +
                "|  . |  . |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    | " +
                "   |    |    |    |    |    |\t\t|    |    |    |  . |    |    |    |    |\r\n+----+----+----+----+----+----+----+---" +
                "-+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  . |" +
                "    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|  " +
                "R |    |    |    |    |    |    |  R |\t\t|    |    |    |  . |    |    |    |    |\r\n+----+----+----+----+----+----" +
                "+----+----+ \t\t+----+----+----+----+----+----+----+----+\n\nPress enter to exit.");
        }
    }
}
