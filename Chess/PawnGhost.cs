using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class PawnGhost : Pawn
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public new int Player { get; protected set; }
        public (int Row, int Col) Position { get; protected set; }
        public Pawn Pawn { get; protected set; }
        public int Counter { get; protected set; }

        // Constructor
        public PawnGhost(Pawn pawn, (int Row, int Pos) position, int player) : base(player)
        {
            Type = "PawnGhost";
            Pawn = pawn;
            Position = position;
            Player = player;
            Symbol = Reference.pieceSymbol["PawnGhost"];
            Value = 0;
            Counter = 1;
        }

        public void UpdateStatus()
        {
            Counter -= 1;

            if (Counter == 0)
            {
                GameManager.Board[Position.Row][Position.Col] = null;
            }
        }
    }
}
