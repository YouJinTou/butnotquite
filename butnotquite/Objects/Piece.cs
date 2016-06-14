namespace butnotquite.Objects
{
    using butnotquite.Defaults;

    using System.Collections.Generic;

    internal class Piece
    {
        internal Color Color;
        internal PieceType Type;
        internal short Value;
        internal HashSet<byte> Moves;
        internal bool Pinned;

        internal Piece()
        {
            this.Type = PieceType.None;
            this.Color = Color.None;
        }

        internal Piece(Color color, PieceType type)
        {
            this.Color = color;
            this.Type = type;
            this.Value = AssignInitialValue(type);
        }

        private short AssignInitialValue(PieceType type)
        {
            switch (type)
            {
                case PieceType.King:
                    return short.MaxValue;
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
    }
}
