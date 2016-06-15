namespace butnotquite.Models
{
    internal struct Move
    {
        internal int FromSquare;
        internal int ToSquare;

        internal Move(int fromSquare, int toSquare)
        {
            this.FromSquare = fromSquare;
            this.ToSquare = toSquare;
        }
    }
}
