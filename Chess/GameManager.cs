using System;
using System.Collections.Generic;

namespace Chess
{
    internal class GameManager
    {
        // Properties
        public Piece[][] Board { get; private set; }
        public List<Piece> CapturedPieces { get; private set; }
        public List<PawnGhost> PawnGhosts { get; private set; }
        public King King1 { get; private set; }
        public King King2 { get; private set; }
        public int Turn { get; private set; }
        public int MaterialAdvantage { get; set; }
        public (int Player, string Move) LastMove { get; private set; }

        // Constructor
        public GameManager()
        {
            Board = new Piece[8][];
            for (int i = 0; i < 8; i++)
                Board[i] = new Piece[8];

            CapturedPieces = new List<Piece>();
            PawnGhosts = new List<PawnGhost>();

            Turn = 0;
            MaterialAdvantage = 0;
        }

        public void SetBoard()
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
            King1.UpdatePosition((0, 4));
            King2.UpdatePosition((7, 4));
        }


        public void UpdateBoard((int Row, int Col) curPos, (int Row, int Col) newPos, bool onlyTest, out bool success)
        {
            Piece oldSquare = Board[curPos.Row][curPos.Col];
            Piece newSquare = Board[newPos.Row][newPos.Col];
            Piece saveOld = oldSquare;
            Piece saveNew = newSquare;
            Piece capturedPiece = null;
            bool castleKingside = false;
            bool castleQueenside = false;
            King king = (oldSquare.Player == 1) ? King1 : King2;

            if (newSquare != null)
            {
                // Castle K and R
                if (newSquare.Player == oldSquare.Player && oldSquare.Type == "King" && newSquare.Type == "Rook")
                {
                    if (newPos.Col == 0)
                    {
                        Board[curPos.Row][2] = oldSquare;
                        Board[curPos.Row][3] = newSquare;
                        castleQueenside = true;
                        king.UpdatePosition((curPos.Row, 2));
                    }
                    else if (newPos.Col == 7)
                    {
                        Board[curPos.Row][6] = oldSquare;
                        Board[curPos.Row][5] = newSquare;
                        castleKingside = true;
                        king.UpdatePosition((curPos.Row, 6));
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

                        capturedPiece = (pawnGhost.Pawn);
                    }

                    // All other captures
                    else
                    {
                        capturedPiece = newSquare;
                    }

                }
            }

            if (oldSquare != null)
                if (oldSquare.Type == "King")
                    king.UpdatePosition(newPos);

            Board[newPos.Row][newPos.Col] = oldSquare;
            Board[curPos.Row][curPos.Col] = null;

            bool check = IsReachable(king.Player, king.Position);

            if (check)
            {
                View.UpdateRemarks(Reference.error[8]);
                success = false;
            }
            else
                success = true;

            // If player put their own king in check, undo the move.
            if (check || onlyTest)
                UndoMove(curPos, newPos, king, saveOld, saveNew, castleKingside, castleQueenside);

            else
            {
                Console.Clear();
                View.PrintDisplay(this, false, this.Turn);
                bool pendingReview = true;

                while (pendingReview)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nType Y and press enter to approve. N to undo.");
                    Console.ForegroundColor = ConsoleColor.White;
                    string reply = Console.ReadLine();
                    reply = reply.ToUpper();

                    if (reply == null)
                        continue;

                    if (reply != "Y" && reply != "N")
                        continue;

                    pendingReview = false;

                    if (reply == "N")
                    {
                        UndoMove(curPos, newPos, king, saveOld, saveNew, castleKingside, castleQueenside);
                        Console.Clear();
                        View.PrintDisplay(this, false, this.Turn);
                        return;
                    }

                    else
                    {
                        Program.SetTurnComplete(true);

                        // King and Rook may only castle on first move. Pawns may only move two spaces on first move.
                        if (saveOld.HasNotMoved)
                            saveOld.FalsifyHasNotMoved();

                        // Create "PawnGhost" as en passant marker behind pawn after moving two spaces.
                        if (saveOld.Type == "Pawn" && Math.Abs(newPos.Row - curPos.Row) == 2)
                        {
                            if (IsEnPassant(saveOld.Player, newPos))
                            {
                                Pawn pawn = (Pawn)saveOld;
                                int rank = (pawn.Player == 1) ? 2 : 5;
                                Board[rank][curPos.Col] = new PawnGhost(pawn, (rank, curPos.Col), pawn.Player);
                                pawn.AddPawnGhost(Board[rank][curPos.Col]);
                                PawnGhosts.Add((PawnGhost)Board[rank][curPos.Col]);
                            }
                        }

                        if (capturedPiece != null)
                        {
                            if (saveNew.Type == "PawnGhost")
                            {
                                int dir = (Turn % 2 == 1) ? 1 : -1;
                                Board[newPos.Row - dir][newPos.Col] = null;
                            }

                            CapturedPieces.Add(capturedPiece);
                            CapturedPieces.Sort();

                            int advantageSwing = (capturedPiece.Player == 1) ? -capturedPiece.Value : capturedPiece.Value;

                            MaterialAdvantage += advantageSwing;
                        }

                        if ((newPos.Row == 7 || newPos.Row == 0) && Board[newPos.Row][newPos.Col] != null)
                            if (Board[newPos.Row][newPos.Col].Type == "Pawn")
                            {
                                Pawn pawn = (Pawn)Board[newPos.Row][newPos.Col];
                                pawn.Promote(this, newPos);
                            }

                        // Remove "PawnGhost" en passant marker on player's next turn
                        if (PawnGhosts.Count > 0)
                        {
                            int player = (Turn % 2 == 1) ? 1 : 2;

                            for (int i = 0; i < PawnGhosts.Count; i++)
                            {
                                if (PawnGhosts[i].Player != player)
                                    PawnGhosts[i].RemovePawnGhost(this);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateBoard((int Row, int Col) curPos, (int Row, int Col) newPos, bool onlyTest)
        {
            UpdateBoard(curPos, newPos, onlyTest, out _);
        }

        public void UndoMove((int Row, int Col) curPos, (int Row, int Col) newPos, 
            King king, Piece saveOld, Piece saveNew, bool castleKingside, bool castleQueenside)
        {
            Board[curPos.Row][curPos.Col] = saveOld;
            Board[newPos.Row][newPos.Col] = saveNew;

            if (Board[curPos.Row][curPos.Col].Type == "Pawn" && Math.Abs(newPos.Row - curPos.Row) == 2)
            {
                int rank = (Board[curPos.Row][curPos.Col].Player == 1) ? 2 : 5;
                if (Board[rank][curPos.Col] != null)
                    if (Board[rank][curPos.Col].Type == "PawnGhost")
                        Board[rank][curPos.Col] = null;
            }

            if (castleKingside)
            {
                Board[curPos.Row][2] = null;
                Board[curPos.Row][3] = null;
                king.UpdatePosition((curPos.Row, 4));
            }
            else if (castleQueenside)
            {
                Board[curPos.Row][6] = null;
                Board[curPos.Row][5] = null;
                king.UpdatePosition((curPos.Row, 4));
            }

            Console.Clear();
            View.PrintDisplay(this, false, this.Turn);
        }

        public void EvaluateCheck()
        {
            King king = (Turn % 2 == 1) ? King2 : King1;

            king.RemoveCheck();

            // Is the king in check?
            if (IsReachable(king.Player, king.Position, out (int Row, int Col) enemyPosition))
            {
                king.PutInCheck();

                (int Row, int Col) enemyPos = enemyPosition;

                /* Is it impossible to remove the piece inflicting check next turn with a piece that isn't the checked king?*/
                if (!IsReachable(Board[enemyPos.Row][enemyPos.Col].Player, enemyPos))
                {
                    // Can the king move to a safe square?
                    (int RowMove, int ColMove)[] adjSquares = new (int Row, int Col)[]
                    {
                            (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1)
                    };

                    int row = king.Position.Row;
                    int col = king.Position.Col;
                    bool canMove = false;

                    foreach ((int RowMove, int ColMove) in adjSquares)
                    {
                        if ((row + RowMove) >= 0 && (row + RowMove) < 8 && (col + ColMove) >= 0 && (col + ColMove) < 8)
                        {
                            if (Board[row + RowMove][col + ColMove] == null)
                            {
                                if (!IsReachable(king.Player, ((row + RowMove), (col + ColMove))))
                                {
                                    canMove = true;
                                    break;
                                }
                            }

                            else if (Board[row + RowMove][col + ColMove].Player != king.Player)
                            {
                                if (!IsReachable(king.Player, ((row + RowMove), (col + ColMove))))
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
                        if (Board[enemyPos.Row][enemyPos.Col].Type == "Knight" || Board[enemyPos.Row][enemyPos.Col].Type == "Pawn")
                        {
                            Program.EndMatch("CHECKMATE", king.Player);
                        }

                        bool canBlock = false;

                        // Can the player shield the king with another piece?
                        while (king.Position.Row != enemyPos.Row && king.Position.Col != enemyPos.Col && canBlock == false)
                        {
                            int rowShift = 0;

                            if (king.Position.Row != enemyPos.Row)
                                rowShift = (king.Position.Row > enemyPos.Row) ? 1 : -1;

                            int colShift = 0;

                            if (king.Position.Col != enemyPos.Col)
                                colShift = (king.Position.Col > enemyPos.Col) ? 1 : -1;

                            if (king.Position.Row != (enemyPos.Row + rowShift) && king.Position.Col != (enemyPos.Col + colShift))
                            {
                                // Is opposing player Player 1 or Player 2?
                                int oppPlayer = (king.Player == 1) ? 2 : 1;

                                if (IsReachable(oppPlayer, ((enemyPos.Row + rowShift), (enemyPos.Col + colShift))))
                                {
                                    canBlock = true;
                                }
                            }

                            enemyPos.Row += rowShift;
                            enemyPos.Col += colShift;
                        }

                        if (canBlock == false)
                            Program.EndMatch("CHECKMATE", king.Player);
                    }
                }
            }
        }

        public void EvaluateStaleMate()
        {
            int player = (Turn % 2 == 1) ? 2 : 1;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i][j] == null)
                        continue;

                    else if (Board[i][j].Player != player)
                        continue;

                    else
                    {
                        if (Board[i][j].CanMove(this, (i, j)))
                            return;
                    }
                }
            }

            Program.EndMatch("STALEMATE", 0);
        }
        

        public bool IsUniversalInvalidMove((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            // Requested move has no change.
            if (newPos == curPos)
            {
                View.UpdateRemarks(Reference.error[3]);
                return true;
            }

            // Requested move is out of bounds.
            if (newPos.Row < 0 || newPos.Row > 7 || newPos.Col < 0 || newPos.Col > 7)
            {
                View.UpdateRemarks(Reference.error[4]);
                return true;
            }

            // Requested move on friendly occupied square, except when castling.
            if (Board[newPos.Row][newPos.Col] != null)
                if (Board[newPos.Row][newPos.Col].Player == Board[curPos.Row][curPos.Col].Player)
                {
                    // If not a King and Rook castling
                    if (!(Board[curPos.Row][curPos.Col].Symbol == Reference.pieceSymbol["King"] && Board[curPos.Row][curPos.Col].HasNotMoved
                        && Board[newPos.Row][newPos.Col].Symbol == Reference.pieceSymbol["Rook"] && Board[newPos.Row][newPos.Col].HasNotMoved))
                    {
                        View.UpdateRemarks(Reference.error[5]);
                        return true;
                    }
                }

            return false;
        }

        public bool IsDiagonalBlocked((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            int directionHor = (newPos.Row > curPos.Row) ? 1 : -1;
            int directionVer = (newPos.Col > curPos.Col) ? 1 : -1;

            for (int i = 1; i < Math.Abs(newPos.Row - curPos.Row); i++)
                if (Board[curPos.Row + (i * directionHor)][curPos.Col + (i * directionVer)] != null)
                    if (Board[curPos.Row + (i * directionHor)][curPos.Col + (i * directionVer)].Type != "PawnGhost")
                    {
                        View.UpdateRemarks(Reference.error[6]);
                        return true;
                    }

            return false;
        }

        public bool IsStraightBlocked((int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (newPos.Row != curPos.Row)
            {
                int direction = (newPos.Row > curPos.Row) ? 1 : -1;
                int gap = Math.Abs(newPos.Row - curPos.Row);
                for (int i = 1; i < gap; i++)
                    if (Board[curPos.Row + (i * direction)][curPos.Col] != null)
                        if (Board[curPos.Row + (i * direction)][curPos.Col].Type != "PawnGhost")
                        {
                            View.UpdateRemarks(Reference.error[6]);
                            return true;
                        }
            }
            else
            {
                int direction = (newPos.Col > curPos.Col) ? 1 : -1;
                int gap = Math.Abs(newPos.Col - curPos.Col);
                for (int i = 1; i < gap; i++)
                    if (Board[curPos.Row][curPos.Col + (i * direction)] != null)
                        if (Board[curPos.Row][curPos.Col + (i * direction)].Type != "PawnGhost")
                        {
                            View.UpdateRemarks(Reference.error[6]);
                            return true;
                        }
            }

            return false;
        }

        public bool IsEnPassant(int player, (int Row, int Col) newPos)
        {
            int rank = (player == 1) ? 3 : 4;

            if (newPos.Col - 1 >= 0)
            {
                if (Board[rank][newPos.Col - 1] != null)
                    if (Board[rank][newPos.Col - 1].Type == "Pawn" && Board[rank][newPos.Col - 1].Player != player)
                        return true;
            }

            if (newPos.Col + 1 < 8)
            {
                if (Board[rank][newPos.Col + 1] != null)
                    if (Board[rank][newPos.Col + 1].Type == "Pawn" && Board[rank][newPos.Col + 1].Player != player)
                        return true;
            }

            return false;
        }

        // Can the other player move to this square?
        // Has overload method that does not out enemyPosition.
        public bool IsReachable(int player, (int Row, int Col) position, out (int Row, int Col) enemyPosition)
        {
            int row = position.Row;
            int col = position.Col;

            foreach ((int RowMove, int ColMove) in Knight.knightMoves)
            {
                if (IsKnight((row + RowMove), (col + ColMove), player))
                {
                    enemyPosition = ((row + RowMove), (col + ColMove));
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
                        if (IsQueenOrRook(row + i, col))
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
                        if (IsQueenOrRook(row, col + i))
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
                        if (IsQueenOrRook(row - i, col))
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
                        if (IsQueenOrRook(row, col - i))
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
                        if (IsQueenOrBishop(row + i, col + i) || IsPawn(row + i, col + i, player, 1, i))
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
                        if (IsQueenOrBishop(row - i, col + i) || IsPawn(row - i, col + i, player, 2, i))
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
                        if (IsQueenOrBishop(row - i, col - i) || IsPawn(row - i, col - i, player, 2, i))
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
                        if ((IsQueenOrBishop(row + i, col - i)) || IsPawn(row + i, col - i, player, 1, i))
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

        public bool IsReachable(int player, (int Row, int Col) position)
        {
            return IsReachable(player, position, out _);
        }

        public bool IsKnight(int row, int col, int player)
        {
            if (row >= 0 && row < 8 && col >= 0 && col < 8)
                if (Board[row][col] != null)
                    if (Board[row][col].Player != player && Board[row][col].Type == "Knight")
                        return true;

            return false;
        }

        public bool CheckSquare(int row, int col, int player, ref bool isEnemy)
        {
            if (Board[row][col] != null)
            {
                if (Board[row][col].Type != "PawnGhost")
                {
                    if (Board[row][col].Player != player)
                    {
                        isEnemy = true;
                        return true;

                    }
                    else if (Board[row][col].Player == player)
                    {
                        isEnemy = false;
                        return true; // friendly piece blocks way, end search
                    }
                }
            }

            isEnemy = false;
            return false;
        }

        public bool IsQueenOrBishop(int row, int col)
        {
            if ((Board[row][col].Type == "Bishop" || Board[row][col].Type == "Queen"))
                return true;

            return false;
        }

        public bool IsPawn(int row, int col, int player, int reqPlayer, int i)
        {
            if (i == 1)
            {
                /*if ((Board[row][col].Type == "King"))
                    return true;*/

                if (player == reqPlayer)
                    if (Board[row][col].Type == "Pawn")
                        return true;
            }

            return false;
        }

        public bool IsQueenOrRook(int row, int col)
        {
            if (Board[row][col].Type == "Rook" || Board[row][col].Type == "Queen")
                return true;

            return false;
        }

        public void NextTurn()
        {
            Turn++;
        }

        public void CopyGameManager(GameManager other)
        {
            this.Turn = other.Turn;
            this.MaterialAdvantage = other.MaterialAdvantage;
            this.LastMove = other.LastMove;

            foreach (var piece in other.CapturedPieces)
                this.CapturedPieces.Add(piece);

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (other.Board[i][j] == null)
                    {
                        this.Board[i][j] = null;
                        continue;                            
                    }

                    int player = (other.Board[i][j].Player == 1) ? 1 : 2;

                    if (other.Board[i][j].Type == "Pawn")
                        this.Board[i][j] = new Pawn(player);

                    else if (other.Board[i][j].Type == "Rook")
                        this.Board[i][j] = new Rook(player);

                    else if (other.Board[i][j].Type == "Knight")
                        this.Board[i][j] = new Knight(player);

                    else if (other.Board[i][j].Type == "Bishop")
                        this.Board[i][j] = new Bishop(player);

                    else if (other.Board[i][j].Type == "Queen")
                        this.Board[i][j] = new Queen(player);

                    else if (other.Board[i][j].Type == "King")
                        this.Board[i][j] = new King(player);

                    else
                        this.Board[i][j] = null;                 
                }
        }

        public void UpdateLastMove(string moveFrom, string moveTo, Piece piece, int player)
        {
            LastMove = (player, piece.Type + " from " + moveFrom + " to " + moveTo + ".");
        }
    }
}
