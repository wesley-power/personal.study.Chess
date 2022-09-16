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
        public new int Player { get; protected set;  }
        public override bool HasNotMoved { get; protected set; }
        public bool IsInCheck { get; protected set; }


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

        public override bool IsValidMove((int, int) curPos, (int, int) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            // Check standard move
            if ((newPos.Item1 - curPos.Item1 == 0 && Math.Abs(newPos.Item2 - curPos.Item2) == 1)
                || (newPos.Item2 - curPos.Item2 == 0 && Math.Abs(newPos.Item1 - curPos.Item1) == 1)
                || (Math.Abs(newPos.Item1 - curPos.Item1) == Math.Abs(newPos.Item2 - curPos.Item2)
                && Math.Abs(newPos.Item1 - curPos.Item1) == 1))
            {
                    return true;
            }

            // Check if can castle
            if (!IsInCheck && (newPos.Item2 == 0 || newPos.Item2 == 7))
                if (GameManager.Board[newPos.Item1][newPos.Item2] != null)
                    if (GameManager.Board[newPos.Item1][newPos.Item2].Type == "Rook")
                        if (GameManager.Board[newPos.Item1][newPos.Item2].Player == this.Player)
                            if (this.HasNotMoved && GameManager.Board[newPos.Item1][newPos.Item2].HasNotMoved)
                            {
                                if (newPos.Item2 == 0)
                                {
                                    for (int i = 1; i < 4; i++)
                                        if (GameManager.Board[newPos.Item1][i] != null)
                                            return false;
                                }
                                else if (newPos.Item2 == 7)
                                {
                                    for (int i = 5; i < 7; i++)
                                        if (GameManager.Board[newPos.Item1][i] != null)
                                            return false;
                                }

                                return true;
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
    }
}
