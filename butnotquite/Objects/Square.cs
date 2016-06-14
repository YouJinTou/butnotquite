namespace butnotquite.Objects
{
    using butnotquite.Defaults;

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
