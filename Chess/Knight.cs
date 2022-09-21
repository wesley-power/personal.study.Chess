using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override bool IsValidMove((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if ((Math.Abs(newPos.Row - curPos.Row) == 2 && Math.Abs(newPos.Col - curPos.Col) == 1)
                || (Math.Abs(newPos.Col - curPos.Col) == 2 && Math.Abs(newPos.Row - curPos.Row) == 1))
                    return true;

            return false;
        }

        public override bool CanMove((int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in knightMoves)
            {
                (int Row, int Col) newPos = (curPos.Row + RowMove, curPos.Col + ColMove);

                if (newPos.Row >= 0 && newPos.Row < 8 && newPos.Col >= 0 && newPos.Col < 8)
                    if (IsValidMove(curPos, newPos))
                    {
                        GameManager.UpdateBoard(curPos, newPos, true, out bool success);
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
    }
}
