

namespace Chess
{
    /* Chess allows for "en passant" captures. This occurs when a player advances
     * a pawn (p) to the opposite half of the board (rank 5 or 4), and the opposing player
     * moves a new pawn (b) two spaces forward, such that p and b are side by side. p may 
     * move forward and diagonally behind b to capture b "en passant". En passant captures 
     * must be executed immediately or the option has passed. Review Pawn.Define() for more 
     * details. This  class/object creates a marker (aka "PawnGhost") when appropriate so 
     * that en passant captures may be checked and executed.*/

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
