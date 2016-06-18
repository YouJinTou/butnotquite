namespace butnotquite.Models
{
    internal struct Move
    {
        internal int FromSquare;
        internal int ToSquare;

        internal Piece PromotionPiece;

        internal int KingFromSquare;
        internal int KingToSquare;
        internal int RookFromSquare;
        internal int RookToSquare;

        internal Move(int fromSquare, int toSquare)
        {
            this.FromSquare = fromSquare;
            this.ToSquare = toSquare;

            this.PromotionPiece = new Piece();
            this.KingFromSquare = -1;
            this.KingToSquare = -1;
            this.RookFromSquare = -1;
            this.RookToSquare = -1;
        }

        // Pawn promotion constructor
        internal Move(int fromSquare, int toSquare, Piece promotionPiece)
            : this(fromSquare, toSquare)
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

            this.FromSquare = -1;
            this.ToSquare = -1;
            this.PromotionPiece = new Piece();
        }
    }
}
