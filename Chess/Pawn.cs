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
        
        // Fields
        public static readonly (int RowMove, int ColMove)[] pawnMoves = new (int RowMove, int ColMove)[]
        { (1, -1), (1, 0), (1, -1) };

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
        public override bool IsValidMove((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            int direction = (this.Player == 1) ? 1 : -1;

            if ((HasNotMoved && (direction * (newPos.Row - curPos.Row)) == 2 && newPos.Col == curPos.Col))
            {
                if (GameManager.Board[newPos.Row][newPos.Col] == null
                    && GameManager.Board[newPos.Row - (direction * 1)][newPos.Col] == null)
                {
                    return true;
                }
            }
            else if ((direction * (newPos.Row - curPos.Row)) == 1 && newPos.Col == curPos.Col)
            {
                if (GameManager.Board[newPos.Row][newPos.Col] == null)
                    return true;
            }

            else if ((direction * (newPos.Row - curPos.Row)) == 1 && Math.Abs(newPos.Col - curPos.Col) == 1)
                if (GameManager.Board[newPos.Row][newPos.Col] != null)
                    if (GameManager.Board[newPos.Row][newPos.Col].Player != this.Player)
                        return true;

            return false;
        }

        public override bool CanMove((int Row, int Col) curPos)
        {
            int dir = (GameManager.Turn % 2 == 1) ? 1 : -1;

            foreach ((int RowMove, int ColMove) in pawnMoves)
            {
                #pragma warning disable IDE0042 // Deconstruct variable declaration
                (int Row, int Col) newPos = (curPos.Row + (RowMove * dir), curPos.Col + ColMove);
                #pragma warning restore IDE0042 // Deconstruct variable declaration

                if (newPos.Row >= 0 && newPos.Row < 8 && newPos.Col >= 0 && newPos.Col < 8)
                {
                    if (IsValidMove(curPos, (curPos.Row + (RowMove * dir), curPos.Col + ColMove)))
                    {
                        GameManager.UpdateBoard(curPos, (curPos.Row + dir, curPos.Col), true, out bool success);
                        if (success)
                            return true;
                    }
                }
            }

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

        public void Promote((int Row, int Col) position)
        {
            string symbol = "";

            while (symbol != "Q" && symbol != "R" && symbol != "B" && symbol != "N")
            {
                Console.Write("Promote your pawn to Q, R, B N.\nEnter symbol: ");
                symbol = Console.ReadLine();
                symbol = symbol.ToUpper();
                
                if (symbol != "Q" && symbol != "R" && symbol != "B" && symbol != "N")
                {
                    Console.WriteLine("Invalid input.");
                }
            }

            if (symbol == "Q")
                GameManager.Board[position.Row][position.Col] = new Queen(this.Player);

            else if (symbol == "R")
                GameManager.Board[position.Row][position.Col] = new Rook(this.Player);

            else if (symbol == "B")
                GameManager.Board[position.Row][position.Col] = new Bishop(this.Player);

            else
                GameManager.Board[position.Row][position.Col] = new Knight(this.Player);
        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }
    }
}
