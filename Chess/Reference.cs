using System;
using System.Collections.Generic;
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
            { "PawnGhost", " " }
        };
    }
}
