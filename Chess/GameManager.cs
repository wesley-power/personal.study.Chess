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
        public static List<Piece> CapturedPieces { get; private set; }
        public static List<PawnGhost> PawnGhosts { get; private set; }
        public static King King1 { get; private set; }
        public static King King2 { get; private set; }
        public static int Turn { get; private set; }
        public static int MaterialAdvantage { get; private set; }

        // Constructor
        public GameManager()
        {
            Board = new Piece[8][];
            for (int i = 0; i < 8; i++)
                Board[i] = new Piece[8];

            CapturedPieces = new List<Piece>();
            PawnGhosts = new List<PawnGhost>();

            Turn = 1;
        }

        public static void SetBoard()
        {
            // Player 1 back rank
            Board[0][0] = new Rook(1);
            Board[0][1] = new Knight(1);
            Board[0][2] = new Bishop(1);
            Board[0][3] = new Queen(1);
            Board[0][4] = new King(1);
            Board[0][5] = new Bishop(1);
            Board[0][6] = new Knight(1);
            Board[0][7] = new Rook(1);

            // Player 2 back rank
            Board[7][0] = new Rook(2);
            Board[7][1] = new Knight(2);
            Board[7][2] = new Bishop(2);
            Board[7][3] = new Queen(2);
            Board[7][4] = new King(2);
            Board[7][5] = new Bishop(2);
            Board[7][6] = new Knight(2);
            Board[7][7] = new Rook(2);

            // All pawns
            for (int i = 0; i < 8; i++)
            {
                Board[1][i] = new Pawn(1);
                Board[6][i] = new Pawn(2);
            }

            King1 = (King)Board[0][4];
            King2 = (King)Board[7][4];
        }


        public static void UpdateBoard((int, int) curPos, (int, int) newPos)
        {
            Piece oldSquare = Board[curPos.Item1][curPos.Item2];
            Piece newSquare = Board[newPos.Item1][newPos.Item2];
            King king = (oldSquare.Player == 1) ? King1 : King2;

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
                        king.UpdatePosition((curPos.Item1, 2));
                    }
                    else if (newPos.Item2 == 7)
                    {
                        Board[curPos.Item1][6] = oldSquare;
                        Board[curPos.Item1][5] = newSquare;
                        king.UpdatePosition((curPos.Item1, 6));
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

                        CapturedPieces.Add(pawnGhost.Pawn);
                    }

                    // All other captures
                    else
                    {
                        CapturedPieces.Add(newSquare);
                    }

                }
            }

            if (oldSquare != null)
                if (oldSquare.Type == "King")
                    king.UpdatePosition(newPos);

            Board[newPos.Item1][newPos.Item2] = oldSquare;
            Board[curPos.Item1][curPos.Item2] = null;

            // Evaluate Check and Checkmate conditions. Change reference to opposing king.
            king = (Turn % 2 == 1) ? King2 : King1;
            {
                king.RemoveCheck();

                // Is the king in check?
                if (IsReachable(king.Player, king.Position, out (int, int) enemyPosition))
                {
                    king.PutInCheck();

                    (int, int) enemyPos = enemyPosition;

                    /* Is it impossible to remove the piece inflicting check next turn with a piece that isn't the checked king?*/
                    if (!IsReachable(Board[enemyPos.Item1][enemyPos.Item2].Player, enemyPos))
                    {
                        // Can the king move to a safe square?
                        (int, int)[] adjSquares = new (int, int)[]
                        {
                            (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1)
                        };

                        int row = king.Position.Item1;
                        int col = king.Position.Item2;
                        bool canMove = false;

                        foreach ((int, int) square in adjSquares)
                        {
                            if ((row + square.Item1) >= 0 && (row + square.Item1) < 8 && (col + square.Item2) >= 0 && (col + square.Item2) < 8)
                            {
                                if (Board[row + square.Item1][col + square.Item2] == null)
                                {
                                    if (!IsReachable(king.Player, ((row + square.Item1), (col + square.Item2))))
                                    {
                                        canMove = true;
                                        break;
                                    }
                                }

                                else if (Board[row + square.Item1][col + square.Item2].Player != king.Player)
                                {
                                    if (!IsReachable(king.Player, ((row + square.Item1), (col + square.Item2))))
                                    {
                                        canMove = true;
                                        break;
                                    }
                                }

                            }
                        }

                        // If the king cannot move to a safe square and the checking piece cannot be removed.
                        if (canMove == false)
                        {
                            if (Board[enemyPos.Item1][enemyPos.Item2].Type == "Knight" || Board[enemyPos.Item1][enemyPos.Item2].Type == "Pawn")
                            {
                                CheckMate();
                            }

                            bool canBlock = false;

                            // Can the player shield the king with another piece?
                            while (king.Position.Item1 != enemyPos.Item1 && king.Position.Item2 != enemyPos.Item2 && canBlock == false)
                            {
                                int rowShift = 0;

                                if (king.Position.Item1 != enemyPos.Item1)
                                    rowShift = (king.Position.Item1 > enemyPos.Item1) ? 1 : -1;

                                int colShift = 0;

                                if (king.Position.Item2 != enemyPos.Item2)
                                    colShift = (king.Position.Item2 > enemyPos.Item2) ? 1 : -1;

                                if (king.Position.Item1 != (enemyPos.Item1 + rowShift) && king.Position.Item2 != (enemyPos.Item2 + colShift))
                                {
                                    // Is opposing player Player 1 or Player 2?
                                    int oppPlayer = (king.Player == 1) ? 2 : 1;

                                    if (IsReachable(oppPlayer, ((enemyPos.Item1 + rowShift), (enemyPos.Item2 + colShift))))
                                    {
                                        canBlock = true;
                                    }
                                }

                                enemyPos.Item1 += rowShift;
                                enemyPos.Item2 += colShift;
                            }

                            if (canBlock == false)
                                CheckMate();
                        }
                    }
                }
            }

            Turn++;
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

        // Can the other player move to this square?
        // Has overload method that does not out enemyPosition.
        public static bool IsReachable(int player, (int, int) position, out (int, int) enemyPosition)
        {
            int row = position.Item1;
            int col = position.Item2;

            (int, int)[] knightMoves = new (int, int)[]
            {
                (2, -1), (2, 1), (1, 2), (-1, 2), (-2, 1), (-2, -1), (-1, -2), (1, -2)
            };

            foreach ((int, int) move in knightMoves)
            {
                if (IsKnight((row + move.Item1), (col + move.Item2), player))
                {
                    enemyPosition = ((row + move.Item1), (col + move.Item2));
                    return true;
                }
            }

            bool isUp = false;
            bool isUpright = false;
            bool isRight = false;
            bool isDownright = false;
            bool isDown = false;
            bool isDownleft = false;
            bool isLeft = false;
            bool isUpleft = false;

            // Check straight and diagonal paths
            for (int i = 1; i < 8; i++)
            {
                if (isUp == false && row + i < 8)
                {
                    bool isEnemy = false;
                    isUp = CheckSquare((row + i), col, player, ref isEnemy);

                    if (isEnemy)
                    {
                        if (IsQueenRook(row + i, col))
                        {
                            enemyPosition = ((row + i), col);
                            return true;
                        }
                    }
                }

                if (isRight == false && col + i < 8)
                {
                    bool isEnemy = false;
                    isRight = CheckSquare(row, (col + i), player, ref isEnemy);

                    if (isEnemy)
                    {
                        if (IsQueenRook(row, col + i))
                        {
                            enemyPosition = (row, (col + i));
                            return true;
                        }
                    }
                }

                if (isDown == false && row - i >= 0)
                {
                    bool isEnemy = false;
                    isDown = CheckSquare((row - i), col, player, ref isEnemy);

                    if (isEnemy)
                    {
                        if (IsQueenRook(row - i, col))
                        {
                            enemyPosition = ((row - i), col);
                            return true;
                        }
                    }
                }

                if (isLeft == false && col - i >= 0)
                {
                    bool isEnemy = false;
                    isLeft = CheckSquare(row, (col - i), player, ref isEnemy);

                    if (isEnemy)
                    {
                        if (IsQueenRook(row, col - i))
                        {
                            enemyPosition = (row, (col - i));
                            return true;
                        }
                    }
                }

                if (isUpright == false && (row + i) < 8 && (col + i) < 8)
                {
                    bool isEnemy = false;
                    isUpright = CheckSquare((row + i), (col + i), player, ref isEnemy);

                    if (isEnemy)
                    {
                        if ((IsQueenBishop(row + i, col + i)) || (player == 1 && i == 1 && IsPawn(row + i, col + i)))
                        {
                            enemyPosition = ((row + i), (col + i));
                            return true;
                        }
                    }
                }

                if (isDownright == false && (row - i) >= 0 && (col + i) < 8)
                {
                    bool isEnemy = false;
                    isDownright = CheckSquare((row - i), (col + i), player, ref isEnemy);

                    if (isEnemy)
                    {
                        if ((IsQueenBishop(row - i, col + i)) || (player == 2 && i == 1 && IsPawn(row - i, col + i)))
                        {
                            enemyPosition = ((row - i), (col + i));
                            return true;
                        }
                    }

                }

                if (isDownleft == false && (row - i) >= 0 && (col - i) >= 0)
                {
                    bool isEnemy = false;
                    isDownleft = CheckSquare((row - i), (col - i), player, ref isEnemy);

                    if (isEnemy)
                    {
                        if ((IsQueenBishop(row - i, col - i)) || (player == 2 && i == 1 && IsPawn(row - i, col - i)))
                        {
                            enemyPosition = ((row - i), (col - i));
                            return true;
                        }
                    }
                }

                if (isUpleft == false && (row + i) < 8 && (col - i) >= 0)
                {
                    bool isEnemy = false;
                    isUpleft = CheckSquare((row + i), (col - i), player, ref isEnemy);

                    if (isEnemy)
                    {
                        if ((IsQueenBishop(row + i, col - i)) || (player == 1 && i == 1 && IsPawn(row + i, col - i)))
                        {
                            enemyPosition = ((row + i), (col - i));
                            return true;
                        }
                    }
                }
            }

            enemyPosition = (-1, -1); // default value when no enemy is found
            return false;
        }

        public static bool IsReachable(int player, (int, int) position)
        {
            return IsReachable(player, position, out _);
        }

        public static bool IsKnight(int row, int col, int player)
        {
            if (row >= 0 && row < 8 && col >= 0 && col < 8)
                if (Board[row][col] != null)
                    if (Board[row][col].Player != player && Board[row][col].Type == "Knight")
                        return true;

            return false;
        }

        public static bool CheckSquare(int row, int col, int player, ref bool isEnemy)
        {
            if (Board[row][col] != null)
            {
                if (Board[row][col].Player != player)
                {
                    isEnemy = true;
                    return true;

                }
                else if (Board[row][col].Player == player)
                {
                    return true; // friendly piece blocks way, end search
                }
            }

            return false;
        }

        public static bool IsQueenBishop(int row, int col)
        {
            if ((Board[row][col].Type == "Bishop" || Board[row][col].Type == "Queen"))
                return true;

            return false;
        }

        public static bool IsPawn(int row, int col)
        {
            if ((Board[row][col].Type == "Pawn"))
                return true;

            return false;
        }

        public static bool IsQueenRook(int row, int col)
        {
            if (Board[row][col].Type == "Rook" || Board[row][col].Type == "Queen")
                return true;

            return false;
        }

        public static void CheckMate()
        {}
    }
}
