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

        // Constructor
        public PawnGhost(Pawn pawn, (int Row, int Pos) position, int player) : base(player)
        {
            Type = "PawnGhost";
            Pawn = pawn;
            Position = position;
            Player = player;
            Symbol = Reference.pieceSymbol["PawnGhost"];
            Value = 0;
        }

        public void UpdateStatus(GameManager gameManager)
        {
            gameManager.PawnGhosts.Remove(this);

            if (gameManager.Board[Position.Row][Position.Col].Type == "PawnGhost")
                gameManager.Board[Position.Row][Position.Col] = null;
        }
    }
}
