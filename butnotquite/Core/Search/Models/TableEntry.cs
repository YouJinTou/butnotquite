namespace butnotquite.Core.Search.Models
{
    internal struct TableEntry
    {
        internal int Depth;
        internal int Score;

        internal TableEntry(int depth, int score)
        {
            this.Depth = depth;
            this.Score = score;
        }
    }
}
