namespace butnotquite.Objects
{
    using Defaults;

    internal struct Square
    {
        internal Piece OccupiedBy;

        public bool IsOccupied
        {
            get
            {
                return this.OccupiedBy.Type == PieceType.None;
            }
        }
    }
}
