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
        public override bool IsValidMove((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if (newPos.Row - curPos.Row == 0 || newPos.Col - curPos.Col == 0)
            {
                if (GameManager.IsStraightBlocked(curPos, newPos))
                    return false;

                return true;
            }

            return false;
        }

        public override bool CanMove((int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in rookMoves)
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
