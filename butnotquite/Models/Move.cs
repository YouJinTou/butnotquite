namespace butnotquite.Models
{
    using Defaults;

    using System;
    using System.Threading;

    internal struct Move : IEquatable<Move>
    {
        internal long Id;
        internal int FromSquare;
        internal int ToSquare;
        internal Direction Direction;
        internal int Score;

        internal Piece PromotionPiece;

        internal int KingFromSquare;
        internal int KingToSquare;
        internal int RookFromSquare;
        internal int RookToSquare;

        private static long instanceCount;

        internal Move(int fromSquare, int toSquare, Direction direction)
        {
            this.Id = Interlocked.Increment(ref instanceCount);
            this.FromSquare = fromSquare;
            this.ToSquare = toSquare;
            this.Direction = direction;

            this.Score = 0;
            this.PromotionPiece = new Piece();
            this.KingFromSquare = -1;
            this.KingToSquare = -1;
            this.RookFromSquare = -1;
            this.RookToSquare = -1;
        }

        // Pawn promotion constructor
        internal Move(int fromSquare, int toSquare, Direction direction, Piece promotionPiece)
            : this(fromSquare, toSquare, direction)
        {
            this.PromotionPiece = promotionPiece;
        }

        // Castling constructor
        internal Move(int kingFromSquare, int kingToSquare, int rookFromSquare, int rookToSquare)
        {
            this.KingFromSquare = kingFromSquare;
            this.KingToSquare = kingToSquare;
            this.RookFromSquare = rookFromSquare;
            this.RookToSquare = rookToSquare;
            this.Direction = Direction.Castle;

            this.Id = Interlocked.Increment(ref instanceCount);
            this.FromSquare = -1; // The initial move is the king's, not the rook's
            this.ToSquare = -1;
            this.Score = 0;
            this.PromotionPiece = new Piece();
        }

        public bool Equals(Move other)
        {
            return (this.Id == other.Id);
        }
    }
}
