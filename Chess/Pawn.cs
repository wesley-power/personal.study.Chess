using System;

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
        public override bool IsValidMove(GameManager gameManager, (int Row, int Col) curPos, (int Row, int Col) newPos)
        {
            if (gameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            int direction = (this.Player == 1) ? 1 : -1;

            if ((HasNotMoved && (direction * (newPos.Row - curPos.Row)) == 2 && newPos.Col == curPos.Col))
            {
                if (gameManager.Board[newPos.Row][newPos.Col] == null
                    && gameManager.Board[newPos.Row - (direction * 1)][newPos.Col] == null)
                {
                    return true;
                }
            }
            else if ((direction * (newPos.Row - curPos.Row)) == 1 && newPos.Col == curPos.Col)
            {
                if (gameManager.Board[newPos.Row][newPos.Col] == null)
                    return true;
            }

            else if ((direction * (newPos.Row - curPos.Row)) == 1 && Math.Abs(newPos.Col - curPos.Col) == 1)
                if (gameManager.Board[newPos.Row][newPos.Col] != null)
                    if (gameManager.Board[newPos.Row][newPos.Col].Player != this.Player)
                        return true;

            View.UpdateRemarks(Reference.error[7]);
            return false;
        }

        public override bool CanMove(GameManager gameManager, (int Row, int Col) curPos)
        {
            int dir = (gameManager.Turn % 2 == 1) ? 1 : -1;

            foreach ((int RowMove, int ColMove) in pawnMoves)
            {
                #pragma warning disable IDE0042 // Deconstruct variable declaration
                (int Row, int Col) newPos = (curPos.Row + (RowMove * dir), curPos.Col + ColMove);
                #pragma warning restore IDE0042 // Deconstruct variable declaration

                if (newPos.Row >= 0 && newPos.Row < 8 && newPos.Col >= 0 && newPos.Col < 8)
                {
                    if (IsValidMove(gameManager, curPos, (curPos.Row + (RowMove * dir), curPos.Col + ColMove)))
                    {
                        gameManager.UpdateBoard(curPos, (curPos.Row + dir, curPos.Col), true, out bool success);
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

        public void Promote(GameManager gameManager, (int Row, int Col) position)
        {
            string symbol = "";
            Console.SetCursorPosition(0, Program.InputRow - 1);
            Console.WriteLine("                                                                                       ");

            while (symbol != "Q" && symbol != "R" && symbol != "B" && symbol != "N")
            {
                Program.PrintRemarks();
                Console.SetCursorPosition(0, Program.InputRow);
                Console.WriteLine("                                                                                                                                \n" +
                    "                                                                                                                                              \n" +
                    "                                                                                                                                              \n" +
                    "                                                                                                                                                ");
                Console.SetCursorPosition(0, Program.InputRow);                
                Console.Write("Promote your pawn to Q, R, B N.\nEnter symbol: ");
                symbol = Console.ReadLine();
                symbol = symbol.ToUpper();
                
                if (symbol != "Q" && symbol != "R" && symbol != "B" && symbol != "N")
                {
                    View.UpdateRemarks(Reference.error[1]);
                }
            }

            int advantageSwing = (this.Player == 1) ? this.Value : -this.Value;

            gameManager.UpdateMaterialAdvantage(-1, advantageSwing);

            if (symbol == "Q")
                gameManager.Board[position.Row][position.Col] = new Queen(this.Player);

            else if (symbol == "R")
                gameManager.Board[position.Row][position.Col] = new Rook(this.Player);

            else if (symbol == "B")
                gameManager.Board[position.Row][position.Col] = new Bishop(this.Player);

            else
                gameManager.Board[position.Row][position.Col] = new Knight(this.Player);

            Piece newPiece = gameManager.Board[position.Row][position.Col];

            advantageSwing = (newPiece.Player == 1) ? newPiece.Value : -newPiece.Value;

            gameManager.UpdateMaterialAdvantage(1, advantageSwing);
        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }

        public static void Define()
        {
            Console.WriteLine("HELP: Learn about the Pawn\n\nSymbol: p\tMaterial Value: 1\n\nThe humble Pawn is weak alone, but working in supporting formations with other pawns becomes an essential piece " +
                "in exerting control over the board. The Pawn is unique in that its move when capturing an opponent's piece is different than " +
                "it's standard move. It may also move differently on its first move than on any other move, and it has an extra way to capture " +
                "an opponent's piece.\n\nThe pawn's standard move is to move forward one square. It may not capture a piece with this move. Its " +
                "standard move to capture a piece, is to move diagonally forward one square. The pawn may never move backwards or simply to the side. " +
                "It must always advance closer to the opponent's back rank. If the pawn is still in its starting position, having not yet moved, it may " +
                "move forward two spaces instead of one. It cannot capture a piece with this move. One must be wary when making this move, as it may " +
                "open up your pawn to an En Passant capture. If one Pawn has advanced into the opponents half of the board, and the opposing player moves " +
                "his own pawn forward two spaces into a position next to your pawn, you have the opportunity to capture this passing pawn by moving forward and " +
                "diagonally to the space behind this passed pawn. If the opportunity is not taken immediately, the passing pawn may not be captured this way " +
                "on your next turn. There is also another special rule for pawns. If a pawn reaches the back rank on the opposite side of the board, the pawn " +
                "may be promoted to the player's choice of Queen, Rook, Knight, or Bishop.\n\n" +
                "STARTING POSITIONS:\t\t\t\t\tPOSSIBLE MOVEMENT: \'.\' for standard move,\r\n\t\t\t\t\t\t\t\'x\' for capture of opponent's piece" +
                "\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |   " +
                " |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+-" +
                "---+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |  x |    |  x |    |    |\r\n+----+----+----+----+----+----+----" +
                "+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |  p |    |  " +
                "  |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |" +
                "    |    |\t\t|    |    |  . |    |    |    |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+---" +
                "-+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |  p |    |    |  . |    |    |\r\n+----+----+----+----+----+-" +
                "---+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    " +
                "|  . |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|  p |  p |  p |  p |  " +
                "p |  p |  p |  p |\t\t|    |    |    |    |    |  p |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+" +
                "----+----+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+----+----+---" +
                "-+----+----+----+----+ \t\t+----+----+----+----+----+----+----+----+ \r\n\r\nEn Passant\t\t\t\t\t\tb moves forward two spaces\t\t\t\r" +
                "\nYour pawn \'p\' and opposing pawn \'b\'\t\t\tx marks where p can move to capture\t\t\tp has captured b en passant\r\n+----+----+-" +
                "---+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |    " +
                "|    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+----+--" +
                "--+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n|    |" +
                "    |  b |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\r\n+---" +
                "-+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r\n| " +
                "   |    |    |    |    |    |    |    |\t\t|    |    |  x |    |    |    |    |    |\t\t|    |    |  p |    |    |    |    |    |\r\n" +
                "+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\r" +
                "\n|    |    |    |  p |    |    |    |    |\t\t|    |    |  b |  p |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |" +
                "\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+---" +
                "-+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    | " +
                "   |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----" +
                "+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |  " +
                "  |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+" +
                "----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |   " +
                " |    |    |\r\n+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+----+----+----+\t\t+----+----+----+----+----+-" +
                "---+----+----+\r\n|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    |    |    |    |\t\t|    |    |    |    |    " +
                "|    |    |    |\r\n+----+----+----+----+----+----+----+----+ \t\t+----+----+----+----+----+----+----+----+ \t\t+----+----+----+----+----" +
                "+----+----+----+\n\nPress enter to exit.");
        }
    }
}
