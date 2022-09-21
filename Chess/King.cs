using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class King : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set;  }
        public override string Type { get; protected set; }
        public (int Row, int Col) Position { get; protected set; }
        public new int Player { get; protected set;  }
        public override bool HasNotMoved { get; protected set; }
        public bool IsInCheck { get; protected set; }

        // Fields 
        public static readonly (int RowMove, int ColMove)[] kingMoves = new (int RowMove, int ColMove)[]
        { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1), (0, -4), (0, 3) };


        // Constructor
        public King(int player) : base(player)
        {
            Type = "King";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["King"];
            Value = 100;
        }

        // Methods

        public override bool IsValidMove((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            // Check standard move
            if ((newPos.Row - curPos.Row == 0 && Math.Abs(newPos.Col - curPos.Col) == 1)
                || (newPos.Col - curPos.Col == 0 && Math.Abs(newPos.Row - curPos.Row) == 1)
                || (Math.Abs(newPos.Row - curPos.Row) == Math.Abs(newPos.Col - curPos.Col)
                && Math.Abs(newPos.Row - curPos.Row) == 1))
            {
                if (!GameManager.IsReachable(Player, newPos))
                    return true;
            }

            // Check if can castle
            if (!IsInCheck && (newPos.Col == 0 || newPos.Col == 7))
                if (GameManager.Board[newPos.Row][newPos.Col] != null)
                    if (GameManager.Board[newPos.Row][newPos.Col].Type == "Rook")
                        if (GameManager.Board[newPos.Row][newPos.Col].Player == this.Player)
                            if (this.HasNotMoved && GameManager.Board[newPos.Row][newPos.Col].HasNotMoved)
                            {
                                if (newPos.Col == 0)
                                {
                                    for (int i = 1; i < 4; i++)
                                        if (GameManager.Board[newPos.Row][i] != null)
                                            return false;

                                    if (GameManager.IsReachable(Player, (newPos.Row, 2)))
                                        return false;
                                }

                                else if (newPos.Col == 7)
                                {
                                    for (int i = 5; i < 7; i++)
                                        if (GameManager.Board[newPos.Row][i] != null)
                                            return false;

                                    if (GameManager.IsReachable(Player, (newPos.Row, 6)))
                                        return false;
                                }

                                return true;
                            }

            return false;
        }
        public override bool CanMove((int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in kingMoves)
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

        public void PutInCheck()
        {
            IsInCheck = true;
        }

        public void RemoveCheck()
        {
            IsInCheck = false;
        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }

        public void UpdatePosition((int, int) position)
        {
            Position = position;
        }
    }
}
