using System;

namespace Chess
{
    internal class Knight : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public override string Type { get; protected set; }
        public new int Player { get; protected set; }
        public override bool HasNotMoved { get; protected set; }

        // Fields
        public static readonly (int Row, int Col)[] knightMoves = new (int Row, int Col)[]
        { (2, -1), (2, 1), (1, 2), (-1, 2), (-2, 1), (-2, -1), (-1, -2), (1, -2) };

        // Constructor
        public Knight(int player) : base(player)
        {
            Type = "Knight";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["Knight"];
            Value = 3;
        }

        // Methods
        public override bool IsValidMove(GameManager gameManager, (int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (gameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if ((Math.Abs(newPos.Row - curPos.Row) == 2 && Math.Abs(newPos.Col - curPos.Col) == 1)
                || (Math.Abs(newPos.Col - curPos.Col) == 2 && Math.Abs(newPos.Row - curPos.Row) == 1))
                    return true;

            View.UpdateRemarks(Reference.error[7]);
            return false;
        }

        public override bool CanMove(GameManager gameManager, (int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in knightMoves)
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
            Console.WriteLine("HELP: Learn about the Knight\n\nSymbol: N\tMaterial Value: 3\n\nThe Knight is unique as it is the only piece able to hop over all other pieces. " +
                "The Knight's movement is also uniquely \"L\" shaped. Its move is always two straight, non-diagonal squares in any direction, " +
                "followed by a one square movement that is perpendicular to the previous movement. If it lands on a square occupied by an " +
                "a piece of the opposing player, the Knight captures that piece. Each player starts the game with two Knights on the back rank, " +
                "one square in from the outermost edge. It is frequently moved in the early game as it is quickly able to control a large " +
                "number of squares in the center of the board.\n\n" +
                "STARTING POSITIONS:\t\t\t\t\tPOSSIBLE MOVEMENT:\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+" +
                "----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+--" +
                "--+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t" +
                "|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+-" +
                "---+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |  . |    |  . |    |    |    |\r\n+----+----+----+---" +
                "-+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |" +
                "  . |    |    |    |  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+--" +
                "--+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  N |    |    |    |    |\r\n+----+----+----+----+----" +
                "+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |  . | " +
                "   |    |    |  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n" +
                "|    |    |    |    |    |    |    |    |\t\t|    |    |  . |    |  . |    |    |    |\r\n+----+----+----+----+----+----+-" +
                "---+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |  N |    |    |    |    |  N |    |\t\t|    |    |    |   " +
                " |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+ \t\t+----+----+----+----+----+----+----+----+\n\nPress " +
                "enter to exit.");
        }
    }
}
