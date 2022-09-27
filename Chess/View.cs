using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class View
    {
        // Properties
        public static string Remarks { get; private set; }

        public static List<GameManager> PreviousTurns { get; set; }

        // Methods
        public static void PrintDisplay(GameManager gameManager, bool isPreview, int currentTurn)
        {
            (int First, int Second) order; // default value 

            if (isPreview)
            {
                (order.First, order.Second) = (currentTurn % 2 == 1) ? (2, 1) : (1, 2);
            }

            else
            {
                (order.First, order.Second) = (gameManager.Turn % 2 == 1) ? (2, 1) : (1, 2);
            }

            Console.WriteLine("\n\n\n");

            PrintCapturedDisplay(gameManager, order.First);

            PrintBoard(gameManager, isPreview, currentTurn);

            PrintCapturedDisplay(gameManager, order.Second);

            Console.WriteLine("TURN " + gameManager.Turn);

            if (order.Second == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("BLUE");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("RED");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" to move!\n\n\n");
        }

        public static void PrintBoard(GameManager gameManager, bool isPreview, int currentTurn)
        {
            string file1 = "\t       A    B    C    D    E    F    G    H";
            string file2 = "\t       H    G    F    E    D    C    B    A";
            string rowBorder = "\t    +----+----+----+----+----+----+----+----+";
            int player;

            if (isPreview)
                player = (currentTurn % 2 == 1) ? 1 : 2;
            else
                player = (gameManager.Turn % 2 == 1) ? 1 : 2;


            if (player == 1)
            {
                Console.WriteLine(file1 + "\n");
                Console.WriteLine(rowBorder);

                for (int i = 7; i >= 0; i--)
                {
                    Console.Write("\t " + (i + 1) + "  ");

                    for (int j = 0; j < 8; j++)
                    {
                        Piece piece = gameManager.Board[i][j];

                        if (j == 0)
                            Console.Write("|  ");

                        if (piece != null)
                        {
                            PrintPiece(piece);
                            Console.Write(" |  ");
                        }
                        else
                            Console.Write("  |  ");
                    }

                    Console.Write(" " + (i + 1) + "\n");
                    Console.WriteLine(rowBorder);
                }
                Console.WriteLine("\n" + file1 + "\n");
            }

            else if (player == 2)
            {
                Console.WriteLine(file2 + "\n");
                Console.WriteLine(rowBorder);

                for (int i = 0; i < 8; i++)
                {
                    Console.Write("\t " + (i + 1) + "  ");

                    for (int j = 7; j >= 0; j--)
                    {
                        Piece piece = gameManager.Board[i][j];

                        if (j == 7)
                            Console.Write("|  ");

                        if (piece != null)
                        {
                            PrintPiece(piece);
                            Console.Write(" |  ");
                        }

                        else
                            Console.Write("  |  ");
                    }
                    Console.Write(" " + (i + 1) + "\n");
                    Console.WriteLine(rowBorder);
                }
                Console.WriteLine("\n" + file2 + "\n");
            }
        }

        public static void PrintCapturedDisplay(GameManager gameManager, int player)
        {
            if (player == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Captured: ");
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (Piece piece in gameManager.CapturedPieces)
                    if (piece.Player == 2)
                        Console.Write(piece.Symbol + " ");

                Console.Write("\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Captured: ");
                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (Piece piece in gameManager.CapturedPieces)
                    if (piece.Player == 1)
                        Console.Write(piece.Symbol + " ");

                Console.Write("\n\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void PrintPiece(Piece piece)
        {
            if (piece.Player == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (piece.Player == 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write(piece.Symbol);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void UpdateRemarks(int top, string remark)
        {
            Console.SetCursorPosition(0, top - 1);
            Console.Write("                                                                                                                      ");
            Console.SetCursorPosition(0, top - 1);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Remarks: " + remark);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintTitleScreen()
        {
            Console.WriteLine("\n\n\n                                                   _:_\r\n" +
                "                                                  '-.-'\r\n             " +
                "                            ()      __.'.__\r\n                         " +
                "             .-:--:-.  |_______|\r\n                               ()   " +
                "   \\____/    \\=====/\r\n                               /\\      {====}" +
                "     )___(\r\n                    (\\=,      //\\\\      )__(     /_____" +
                "\\\r\n    __    |'-'-'|  //  .\\    (    )    /____\\     |   |\r\n   / " +
                " \\   |_____| (( \\_  \\    )__(      |  |      |   |\r\n   \\__/    |==" +
                "=|   ))  `\\_)  /____\\     |  |      |   |\r\n  /____\\   |   |  (/    " +
                " \\    |  |      |  |      |   |\r\n   |  |    |   |   | _.-'|    |  |  " +
                "    |  |      |   |\r\n   |__|    )___(    )___(    /____\\    /____\\  " +
                "  /_____\\\r\n  (====)  (=====)  (=====)  (======)  (======)  (=======)\r\n" +
                "  }===={  }====={  }====={  }======{  }======{  }======={\r\n (______)(_" +
                "______)(_______)(________)(________)(_________)");

            Console.Write("\n\nWelcome to Chess!\n\n" +
                "1. Play\n" +
                "2. Help\n");

            int top = Console.CursorTop;

            while (true)
            {
                Console.Write("Select and enter a number option: ");
                string select = Console.ReadLine();

                if (Int32.TryParse(select, out int result))
                {
                    if (result == 1)
                        break;

                    else if (result == 2)
                    {
                        Reference.OpenMenu();
                        break;
                    }
                }

                Console.SetCursorPosition(0, top);
                Console.Write("                                                                                                                    ");
                Console.SetCursorPosition(0, top);
            }
        }

        public static void PrintPreviousTurns(GameManager gameManager)
        {
            bool underReview = true;
            int currentItem = PreviousTurns.Count - 1;

            while (underReview)
            {
                Console.Clear();
                PrintDisplay(PreviousTurns[currentItem], true, gameManager.Turn);

                Console.Write("Enter 1 to view previous. Enter 2 to view next. Enter \"EXIT\" to exit:  ");
                string command = Console.ReadLine();
                command = command.ToUpper();

                if (command == "EXIT")
                    underReview = false;

                if (command != "1" && command != "2")
                    continue;

                if (command == "1" && currentItem - 1 >= 0)
                    currentItem--;

                else if (command == "2" && currentItem + 1 < PreviousTurns.Count)
                    currentItem++;
            }
        }
    }
}
