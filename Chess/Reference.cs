using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class Reference
    {
        public static readonly Dictionary<string, string> pieceSymbol = new Dictionary<string, string>
        {
            { "King", "K" },
            { "Queen", "Q" },
            { "Bishop", "B" },
            { "Knight", "N" },
            { "Rook", "R" },
            { "Pawn", "p" },
            { "PawnGhost", "." }
        };

        public static void OpenMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            bool isExit = false;

            while (!isExit)
            {
                Console.Clear();
                Console.Write("Welcome to HELP. Enter the number of what you wish to learn about.\n\n" +
                    "1. What is Chess?\n" +
                    "2. Basic Chess Tips\n" +
                    "3. Learn about the King\n" +
                    "4. Learn about the Queen\n" +
                    "5. Learn about the Rook\n" +
                    "6. Learn about the Bishop\n" +
                    "7. Learn about the Knight\n" +
                    "8. Learn about the Pawn\n" +
                    "9. Exit HELP\n\nSelect: ");

                while (true)
                {
                    string select = Console.ReadLine();
                    if (!Int32.TryParse(select, out int result))
                        break;

                    if (result <= 0 || result > 9)
                        break;

                    if (result == 9)
                    {
                        isExit = true;
                        break;
                    }

                    Console.Clear();

                    switch (result)
                    {
                        case 1:
                            DefineChess();
                            break;

                        case 2:
                            DefineTips();
                            break;

                        case 3:
                            King.Define();
                            break;

                        case 4:
                            Queen.Define();
                            break;

                        case 5:
                            Rook.Define();
                            break;

                        case 6:
                            Bishop.Define();
                            break;

                        case 7:
                            Knight.Define();
                            break;

                        default:
                            Pawn.Define();
                            break;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DefineChess()
        {
            Console.Write("HELP: What is Chess?\n\n" +
                "Chess is a two-player game played on a board of eight squares by eight squares. Each player has one king, one queen, two bishops, two knights, " +
                "two rooks, and eight pawns. Refer to the Learn about... pages for more info on each. The players start on opposite sides of the board, each with two " +
                "rows of pieces. Each kind of piece has its own rules as to how it moves. In Chess, moving one of your pieces to a square occupied by your opponent's " +
                "piece allows you to capture that piece, which means you can remove that piece from the board. Your ultimate goal is to corner your opponent's king, such " +
                "that the King can be captured on your next turn and the King cannot escape being captured on your next turn. This is called Checkmate, and determines " +
                "which player wins the game. The game also can end in a stalemate with no winner if the king is not currently in the line of attack but a player has no " +
                "legal moves left.\n\nPress Enter to exit.");
        }

        public static void DefineTips()
        {
            Console.Write("HELP: Basic Chess Tips\n\n" +
                "Opening Principles: \n" +
                "\t1. Control the center. In the early game, move your pieces into positions that control the center squares for an advantage. Think of the board " +
                "like a hill to control, where the four center squares are the highest point, and the edge squares being the lowest.\n\n" +
                "\t2. Castle early. As a general principle, castle your King with a Rook early to protect your King. Learn about the King for an explanation of castling.\n\n" +
                "\t3. Connect your Rooks. When your Rooks control your back rank with no other pieces between them, they become very powerful defensively.\n\n" +
                "A chess match can be divided into three phases. The early game, the mid game, and the end game. The early game is about developing your pieces " +
                "to put them in positions to control important spaces on the board, usually the center. Knights and Bishops should be made active quickly. The mid game " +
                "phase roughly begins with the exchange of pieces, and actively trying to outmaneuver the opponent and gain material. The end game occurs when many of " +
                "the back rank pieces (Queen, Rook, Knight, Bishop) have been captured, especially the Queen. These markers are rough approximations, not exact " +
                "points of change.\n\nPress Enter to exit.");
        }
    }
}
