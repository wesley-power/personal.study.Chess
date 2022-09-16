using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Pawn : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public override string Type { get; protected set; }
        public new int Player { get; protected set; }
        public override bool HasNotMoved { get; protected set; }
        public Piece PawnGhost { get; protected set; }


        // Constructor
        public Pawn(int player) : base(player)
        {
            Type = "Pawn";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["Pawn"];
            Value = 1;
        }

        // Methods
        public override bool IsValidMove((int, int) curPos, (int, int) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            int direction = (this.Player == 1) ? 1 : -1;

            if ((HasNotMoved && (direction * (newPos.Item1 - curPos.Item1)) == 2 && newPos.Item2 == curPos.Item2))
            {
                if (GameManager.Board[newPos.Item1][newPos.Item2] == null
                    && GameManager.Board[newPos.Item1 - (direction * 1)][newPos.Item2] == null)
                {
                    return true;
                }
            }
            else if ((direction * (newPos.Item1 - curPos.Item1)) == 1 && newPos.Item2 == curPos.Item2)
            {
                if (GameManager.Board[newPos.Item1][newPos.Item2] == null)
                    return true;
            }

            else if ((direction * (newPos.Item1 - curPos.Item1)) == 1 && Math.Abs(newPos.Item2 - curPos.Item2) == 1)
                if (GameManager.Board[newPos.Item1][newPos.Item2].Player != this.Player)
                    return true;

            return false;
        }

        public void AddPawnGhost(Piece pawnGhost)
        {
            PawnGhost = pawnGhost;
        }

        public void DeletePawnGhost()
        {
            PawnGhost = null;
        }

        public void Promote()
        {

        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }
    }
}
