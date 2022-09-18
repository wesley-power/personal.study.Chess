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
        public override bool IsValidMove((int, int) curPos, (int, int) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if (newPos.Item1 - curPos.Item1 == 0 || newPos.Item2 - curPos.Item2 == 0)
            {
                if (GameManager.IsStraightBlocked(curPos, newPos))
                    return false;

                return true;
            }

            else if (Math.Abs(newPos.Item1 - curPos.Item1) == Math.Abs(newPos.Item2 - curPos.Item2))
            {
                if (GameManager.IsDiagonalBlocked(curPos, newPos))
                    return false;

                return true;
            }
                
            return false;
        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }

    }
}
