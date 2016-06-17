namespace butnotquite.Models
{
    using Defaults;

    using System;
    using System.Collections.Generic;

    [Serializable]
    internal sealed class Piece : IComparable<Piece>
    {
        internal Color Color;
        internal PieceType Type;
        internal int Value;
        internal int Position;
        internal HashSet<int> Moves;
        internal bool Pinned;

        internal Piece()
        {
            this.Type = PieceType.None;
            this.Color = Color.None;
            this.Position = -1;
        }

        internal Piece(Color color, PieceType type, int position)
        {
            this.Color = color;
            this.Type = type;
            this.Value = AssignInitialValue(type);
            this.Position = position;
        }

        private int AssignInitialValue(PieceType type)
        {
            switch (type)
            {
                case PieceType.King:
                    return int.MaxValue;
                case PieceType.Queen:
                    return 900;
                case PieceType.Rook:
                    return 500;
                case PieceType.Bishop:
                    return 325;
                case PieceType.Knight:
                    return 300;
                case PieceType.Pawn:
                    return 100;
                default:
                    return 0;
            }
        }

        public int CompareTo(Piece other)
        {
            return this.Position.CompareTo(other.Position);
        }
    }
}
