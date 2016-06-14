namespace butnotquite.Objects
{
    internal struct Move
    {
        internal byte FromSquare;
        internal byte ToSquare;

        internal Move(byte fromSquare, byte toSquare)
        {
            this.FromSquare = fromSquare;
            this.ToSquare = toSquare;
        }
    }
}
