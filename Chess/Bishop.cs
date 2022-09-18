﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Bishop : Piece
    {
        // Properties
        public override int Value { get; protected set; }
        public override string Symbol { get; protected set; }
        public override string Type { get; protected set; }
        public new int Player { get; protected set; }
        public override bool HasNotMoved { get; protected set; }

        // Constructor
        public Bishop(int player) : base(player)
        {
            Type = "Bishop";
            HasNotMoved = true;
            Player = player;
            Symbol = Reference.pieceSymbol["Bishop"];
            Value = 3;
        }

        // Methods
        public override bool IsValidMove((int, int) curPos, (int, int) newPos)
        {
            if (GameManager.IsUniversalInvalidMove(curPos, newPos))
                return false;

            if (Math.Abs(newPos.Item1 - curPos.Item1) == Math.Abs(newPos.Item2 - curPos.Item2))
            {
                if (GameManager.IsDiagonalBlocked(curPos, newPos))
                    return false;

                return true;
            }

            return false;
        }

        public override void FalsifyHasNotMoved()
        {
            HasNotMoved = false;
        }
    }
}