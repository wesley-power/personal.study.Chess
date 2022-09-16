using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class GameManager
    {
        // Properties
        public static Piece[][] Board { get; private set; }
        public static List<Piece> CapturedPieces1 { get; private set; }
        public static List<Piece> CapturedPieces2 { get; private set;  }
        public static List<PawnGhost> PawnGhosts { get; private set; }
        public static int Turn { get; private set; }

        // Constructor
        public GameManager()
        {
            Board = new Piece[8][];
            for (int i = 0; i < 8; i++)
                Board[i] = new Piece[8];

            CapturedPieces1 = new List<Piece>();
            CapturedPieces2 = new List<Piece>();
            PawnGhosts = new List<PawnGhost>();

            Turn = 1;
        }

        public static void SetBoard()
        {
            // Player 1
            Board[0][0] = new Rook(1);
            Board[0][1] = new Knight(1);
            Board[0][2] = new Bishop(1);
            Board[0][3] = new Queen(1);
            Board[0][4] = new King(1);
            Board[0][5] = new Bishop(1);
            Board[0][6] = new Knight(1);
            Board[0][7] = new Rook(1);

            for (int i = 0; i < 8; i++)
                Board[1][i] = new Pawn(1);

            // Player 2
            Board[7][0] = new Rook(2);
            Board[7][1] = new Knight(2);
            Board[7][2] = new Bishop(2);
            Board[7][3] = new Queen(2);
            Board[7][4] = new King(2);
            Board[7][5] = new Bishop(2);
            Board[7][6] = new Knight(2);
            Board[7][7] = new Rook(2);

            for (int i = 0; i < 8; i++)
                Board[6][i] = new Pawn(2);
        }         

        public static void UpdateBoard((int, int) curPos, (int, int) newPos)
        {
            Piece oldSquare = Board[curPos.Item1][curPos.Item2];
            Piece newSquare = Board[newPos.Item1][newPos.Item2];

            // Remove "PawnGhost" en passant marker on player's next turn
            if (PawnGhosts.Count > 0)
            {
                foreach (PawnGhost pawnGhost in PawnGhosts)
                {
                    pawnGhost.UpdateStatus();
                }
            }

            // Indicate piece has moved. For: King, Rook castling, and Pawn first move. 
            if (oldSquare.HasNotMoved)
                oldSquare.FalsifyHasNotMoved();

            // Create "PawnGhost" as en passant marker behind pawn after moving two spaces.
            if (oldSquare.Type == "Pawn" && Math.Abs(newPos.Item1 - curPos.Item1) == 2)
            {
                if (IsEnPassant(oldSquare.Player, newPos))
                {
                    Pawn pawn = (Pawn)oldSquare;
                    int rank = (pawn.Player == 1) ? 2 : 5;
                    Board[rank][curPos.Item2] = new PawnGhost(pawn, (rank, curPos.Item2), pawn.Player);
                    pawn.AddPawnGhost(Board[rank][curPos.Item2]);
                    PawnGhosts.Add((PawnGhost)Board[rank][curPos.Item2]);
                }
            }

            if (newSquare != null)
            {
                // Castle K and R
                if (newSquare.Player == oldSquare.Player && oldSquare.Type == "King" && newSquare.Type == "Rook")
                {
                    if (newPos.Item2 == 0)
                    {
                        Board[curPos.Item1][2] = oldSquare;
                        Board[curPos.Item1][3] = newSquare;
                    }
                    else if (newPos.Item2 == 7)
                    {
                        Board[curPos.Item1][6] = oldSquare;
                        Board[curPos.Item1][5] = newSquare;
                    }

                    oldSquare = null;
                }

                // Capture piece
                else if (newSquare.Player != oldSquare.Player)
                {
                    // En passant captures
                    if (newSquare.Type == "PawnGhost" && oldSquare.Type == "Pawn")
                    {
                        PawnGhost pawnGhost = (PawnGhost)newSquare;

                        if (oldSquare.Player == 1 && newSquare.Player == 2 && Board[4][newPos.Item2].Type == "Pawn")
                        {
                            CapturedPieces1.Add(pawnGhost.Pawn);
                            Board[4][newPos.Item2] = null;
                        }
                        else if (oldSquare.Player == 2 && newSquare.Player == 1 && Board[3][newPos.Item2].Type == "Pawn")
                        {
                            CapturedPieces2.Add(pawnGhost.Pawn);
                            Board[3][newPos.Item2] = null;
                        }
                    }

                    // All other captures
                    else
                    {
                        if (oldSquare.Player == 1)
                            CapturedPieces1.Add(newSquare);

                        else if (oldSquare.Player == 2)
                            CapturedPieces2.Add(newSquare);
                    }
                }
            }

            Board[newPos.Item1][newPos.Item2] = oldSquare;
            Board[curPos.Item1][curPos.Item2] = null;
            Turn++;
        }

        public static bool IsEnPassant(int player, (int, int) newPos)
        {
            int rank = (player == 1) ? 3 : 4;

            if (newPos.Item2 - 1 >= 0)
            {
                if (Board[rank][newPos.Item2 - 1] != null)
                    if (Board[rank][newPos.Item2 - 1].Type == "Pawn" && Board[rank][newPos.Item2 - 1].Player != player)
                        return true;
            }

            if (newPos.Item2 + 1 < 8)
            {
                if (Board[rank][newPos.Item2 + 1] != null)
                    if (Board[rank][newPos.Item2 + 1].Type == "Pawn" && Board[rank][newPos.Item2 + 1].Player != player)
                        return true;
            }

            return false;
        }

        public static bool IsUniversalInvalidMove((int, int) curPos, (int, int) newPos)
        {
            // These are separate IF statements because I intend to add error messages

            // Requested move has no change.
            if (newPos == curPos)
                return true;

            // Requested move is out of bounds.
            if (newPos.Item1 < 0 || newPos.Item1 > 7 || newPos.Item2 < 0 || newPos.Item2 > 7)
                return true;

            // Requested move of other player's piece.
            if ((Board[curPos.Item1][curPos.Item2].Player == 1 && Turn % 2 == 0)
                || (Board[curPos.Item1][curPos.Item2].Player == 2 && Turn % 2 == 1))
                return true;

            // Requested move on friendly occupied square, except when castling.
            if (Board[newPos.Item1][newPos.Item2] != null)
                if (Board[newPos.Item1][newPos.Item2].Player == Board[curPos.Item1][curPos.Item2].Player)
                {
                    // If not a King and Rook castling
                    if (!(Board[curPos.Item1][curPos.Item2].Symbol == Reference.pieceSymbol["King"] && Board[curPos.Item1][curPos.Item2].HasNotMoved
                        && Board[newPos.Item1][newPos.Item2].Symbol == Reference.pieceSymbol["Rook"] && Board[newPos.Item1][newPos.Item2].HasNotMoved))
                        return true;
                }

            return false;
        }

        public static bool IsDiagonalBlocked((int, int) curPos, (int, int) newPos)
        {
            int directionHor = (newPos.Item1 > curPos.Item1) ? 1 : -1;
            int directionVer = (newPos.Item2 > curPos.Item2) ? 1 : -1;

            for (int i = 1; i < Math.Abs(newPos.Item1 - curPos.Item1); i++)
                if (Board[curPos.Item1 + (i * directionHor)][curPos.Item2 + (i * directionVer)] != null)
                    return true;

            return false;
        }

        public static bool IsStraightBlocked((int, int) curPos, (int, int) newPos)
        {
            if (newPos.Item1 != curPos.Item1)
            {
                int direction = (newPos.Item1 > curPos.Item1) ? 1 : -1;
                int gap = Math.Abs(newPos.Item1 - curPos.Item1);
                for (int i = 1; i < gap; i++)
                    if (Board[curPos.Item1 + (i * direction)][curPos.Item2] != null)
                        return true;
            }
            else
            {
                int direction = (newPos.Item2 > curPos.Item2) ? 1 : -1;
                int gap = Math.Abs(newPos.Item2 - curPos.Item2);
                for (int i = 1; i < gap; i++)
                    if (Board[curPos.Item1][curPos.Item2 + (i * direction)] != null)
                        return true;
            }

            return false;
        }
    }
}
