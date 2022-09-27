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

        public override bool IsValidMove(GameManager gameManager, (int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (gameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            // Check standard move
            if ((newPos.Row - curPos.Row == 0 && Math.Abs(newPos.Col - curPos.Col) == 1)
                || (newPos.Col - curPos.Col == 0 && Math.Abs(newPos.Row - curPos.Row) == 1)
                || (Math.Abs(newPos.Row - curPos.Row) == Math.Abs(newPos.Col - curPos.Col)
                && Math.Abs(newPos.Row - curPos.Row) == 1))
            {
                if (!gameManager.IsReachable(Player, newPos) && !IsKingAdjacent(gameManager, newPos))
                    return true;
            }

            // Check if can castle
            if (!IsInCheck && (newPos.Col == 0 || newPos.Col == 7))
                if (gameManager.Board[newPos.Row][newPos.Col] != null)
                    if (gameManager.Board[newPos.Row][newPos.Col].Type == "Rook")
                        if (gameManager.Board[newPos.Row][newPos.Col].Player == this.Player)
                            if (this.HasNotMoved && gameManager.Board[newPos.Row][newPos.Col].HasNotMoved)
                            {
                                if (newPos.Col == 0)
                                {
                                    for (int i = 1; i < 4; i++)
                                        if (gameManager.Board[newPos.Row][i] != null)
                                            return false;

                                    if (gameManager.IsReachable(Player, (newPos.Row, 2)) && !IsKingAdjacent(gameManager, (newPos.Row, 2)))
                                        return false;
                                }

                                else if (newPos.Col == 7)
                                {
                                    for (int i = 5; i < 7; i++)
                                        if (gameManager.Board[newPos.Row][i] != null)
                                            return false;

                                    if (gameManager.IsReachable(Player, (newPos.Row, 6)) && !IsKingAdjacent(gameManager, (newPos.Row, 6)))
                                        return false;
                                }

                                return true;
                            }

            return false;
        }
        public override bool CanMove(GameManager gameManager, (int Row, int Col) curPos)
        {
            foreach ((int RowMove, int ColMove) in kingMoves)
            {
                (int Row, int Col) newPos = (curPos.Row + RowMove, curPos.Col + ColMove);

                if (newPos.Row >= 0 && newPos.Row < 8 && newPos.Col >= 0 && newPos.Col < 8)
                    if (IsValidMove(gameManager, curPos, newPos))
                    {
                        gameManager.UpdateBoard(curPos, newPos, true, out bool success);
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

        public bool IsKingAdjacent(GameManager gameManager, (int Row, int Col) newPos)
        {
            foreach ((int RowMove, int ColMove) in kingMoves)
            {
                if (newPos.Row + RowMove >= 0 && newPos.Row + RowMove < 8 && newPos.Col + ColMove >= 0 && newPos.Col + ColMove < 8)
                    if (gameManager.Board[newPos.Row + RowMove][newPos.Col + ColMove] != null)
                        if (gameManager.Board[newPos.Row + RowMove][newPos.Col + ColMove].Type == "King"
                            && gameManager.Board[newPos.Row + RowMove][newPos.Col + ColMove].Player != Player)
                            return true;
            }

            return false;
        }

        public static void Define()
        {
            Console.WriteLine("HELP: Learn about the King\n\nSymbol: K\tMaterial Value: Invaluable\n\nIf the King is captured, the game is lost. The very goal of chess " +
                "is to corner the opponent's king. The King is therefore the piece that must be protected in the early to mid game. However, " +
                "if enough material is removed from the board, the King becomes a vital attacking and defending piece in the end game. The King " +
                "is limited by its move. Its standard move lets it move one space in any direction, straight or diagonally. The King also has " +
                "a unique move with the Rook called a \"castle\". If the King and the chosen Rook have not moved, there are no pieces between them, " +
                "and the King is not in check, the King and Rook may castle. The King moves two spaces towards the chosen Rook. The Rook then moves " +
                "to the square next to the king closer to the middle of the row. If the King takes up position behind unmoved Pawns, it becomes " +
                "very well defended.\n\nThere are some terms that must be understood regarding the King. To put a king in Check is to put it in a position " +
                "where it can be attacked next turn if it does not move. If your King is put in check, you must attempt to remove the King from check " +
                "on your next turn, either by moving the king, eliminating the threatening piece, or blocking the threatening piece from reaching your King. " +
                "You may never move your King into a position where it is in check, and you may not move one of your pieces if that move puts the King in check. " +
                "If you are in a position where your King cannot be removed from check, you lose the game and the opponent wins. This is called checkmate, " +
                "and your goal is to put the opponent's piece in checkmate. Checkmate ends the game. Another way to end the game is a stalemate, which means " +
                "neither player wins. A stalemate occurs when it is your turn and you have no legal move, or when it is impossible for either King to be put in check. " +
                "For example, your King may not be in check, and the only possible moves you have will put your King in check. None of those moves are legal, " +
                "and a stalemate occurs. Be wary of putting your opponent in a non-check position with no legal moves. If you are in a position where you cannot win, " +
                "you may be able to avoid losing by forcing a stalemate.\n\n" +
                "STARTING POSITIONS:\t\t\t\t\tPOSSIBLE MOVEMENT: \r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n" +
                "|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+-" +
                "---+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+---" +
                "-+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |" +
                "    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |  " +
                "  |    |    |\t\t|    |    |    |  . |  . |  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----" +
                "+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  . |  K |  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t" +
                "\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  . |  . |  . |    |    |\r\n+----+" +
                "----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |   " +
                " |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    | " +
                " K |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+ \t\t+----+----+----+----+----+----+" +
                "----+----+ \r\n\r\nCASTLE\t\t\t\t\t\t\t\t\t\t\r\nThe King may castle with a Rook\t\t\tCastling with the right Rook\t\t\t\tCastling with the left" +
                " Rook\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n" +
                "|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+---" +
                "-+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |  " +
                "  |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+-" +
                "---+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |" +
                "    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t" +
                "+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |   " +
                " |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+--" +
                "--+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    | " +
                "   |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+" +
                "----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t" +
                "|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+---" +
                "-+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |  " +
                "  |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+-" +
                "---+----+----+\r\n|  R |    |    |    |  K |    |    |  R |\t\t|  R |    |    |    |    |  R |  K |    |\t\t|    |    |  K |  R |    |    |    |" +
                "  R |\r\n+----+----+----+----+----+----+----+----+ \t\t+----+----+----+----+----+----+----+----+ \t\t+----+----+----+----+----+----+----+----+" +
                "\n\nPress enter to exit.");
        }
    }
}
