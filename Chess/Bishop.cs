using System;

namespace Chess
{
    internal class Bishop : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public override string Type { get; protected set; }
        public new int Player { get; protected set; }
        public override bool HasNotMoved { get; protected set; }

        // Fields
        public static readonly (int RowMove, int ColMove)[] bishopMoves = new (int RowMove, int ColMove)[]
        { (1, 1), (-1, 1), (-1, -1), (1, -1) };

        // Constructor
        public Bishop(int player) : base(player)
        {
            Type = "Bishop";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["Bishop"];
            Value = 3;
        }

        // Methods
        public override bool IsValidMove(GameManager gameManager, (int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (gameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if (Math.Abs(newPos.Row - curPos.Row) == Math.Abs(newPos.Col - curPos.Col))
            {
                if (gameManager.IsDiagonalBlocked(curPos, newPos))
                    return false;

                return true;
            }

            return false;
        }

        public override bool CanMove(GameManager gameManager, (int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in bishopMoves)
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
            Console.WriteLine("HELP: Learn about the Bishop\n\nSymbol: B\tMaterial Value: 3\n\nThe Bishop may move in any straight, diagonal line as far as the player wishes, " +
                "until it reaches either the edge of the board or another piece. The Bishop may not hope over friendly or opposing pieces, but may " +
                "capture an opposing piece by stopping on its square. Each player has two Bishops, whose starting positions on the back rank flank " +
                "the center squares occupied by the King and Queen. The Bishops are usually activated in the early phases of the game, when pieces" +
                "are moved to control important squares in the center of the board.\n\n" +
                "STARTING POSITIONS:\t\t\t\t\tPOSSIBLE MOVEMENT:\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+--" +
                "--+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |  . |\r\n+----+----+----+----+----+-" +
                "---+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|  . |    |    |    |" +
                "    |    |  . |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    " +
                "|    |    |    |    |    |\t\t|    |  . |    |    |    |  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+---" +
                "-+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |  . |    |  . |    |    |    |\r\n+--" +
                "--+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t" +
                "\t|    |    |    |  B |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+-" +
                "---+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |  . |    |  . |    |    |    |\r\n+----+----+----+----+----+----+" +
                "----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |  . |    |    |    " +
                "|  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |  B |   " +
                " |    |  B |    |    |\t\t|  . |    |    |    |    |    |  . |    |\r\n+----+----+----+----+----+----+----+----+ \t\t+----+----+---" +
                "-+----+----+----+----+----+\n\nPress enter to exit.");
        }
    }
}
