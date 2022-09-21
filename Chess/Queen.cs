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

            else if (Math.Abs(newPos.Row - curPos.Row) == Math.Abs(newPos.Col - curPos.Col))
            {
                if (GameManager.IsDiagonalBlocked(curPos, newPos))
                    return false;

                return true;
            }
                
            return false;
        }

        public override bool CanMove((int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in queenMoves)
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
