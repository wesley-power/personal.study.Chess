using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Piece : IComparable<Piece>
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
        public virtual bool IsValidMove((int Row, int Col) curPos, (int Row, int Col) newPos)
        { return false; } // default value, overridden by superclass

        public virtual bool CanMove((int Row, int Col) curPos)
        { return false; } // default value, overridden by superclass

        public virtual void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }

        public int CompareTo(Piece other)
        {
            if (this == null)
                return 1;

            else if (this.Value < other.Value)
                return -1;

            else if (this.Value > other.Value)
                return 1;

            else
            {
                if (this.Type == "Knight" && other.Type == "Bishop")
                    return -1;

                else if (other.Type == "Knight" && this.Type == "Bishop")
                    return 1;

                else
                    return 0;
            }
        }
    }
}
