using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Piece
    {
        // Properties
        public virtual int Value { get; protected set; }
        public virtual string Symbol { get; protected set; }
        public virtual string Type { get; protected set; }
        public int Player { get; protected set; }
        public virtual bool HasNotMoved { get; protected set; }

        // Constructor
        public Piece(int player)
        {
            Type = "Piece";
            HasNotMoved = true;
            Player = player;
            Symbol = "T";
            Value = 999;
        }

        // Methods
        public virtual bool IsValidMove((int, int) curPos, (int, int) newPos)
        {
            return false; // default value, overridden by superclass
        }

        public virtual void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }
    }
}
