namespace butnotquite.Models
{
    using Utils;

    using System;

    [Serializable]
    internal sealed class GameHistoryEntry
    {
        internal Move Move;
        internal Piece MovingPiece;

        internal GameHistoryEntry(Move move, Piece movingPiece)
        {
            this.Move = Utils.MakeDeepCopy(move);

            if (movingPiece != null)
            {
                this.MovingPiece = Utils.MakeDeepCopy(movingPiece);
            }
        }
    }
}
