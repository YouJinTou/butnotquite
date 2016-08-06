namespace Engine.Core.Search.Models
{
    using Engine.Models;

    internal struct TableEntry
    {
        internal int Depth;
        internal int Score;
        internal Move BestMove;

        internal TableEntry(int depth, int score, Move bestMove)
        {
            this.Depth = depth;
            this.Score = score;
            this.BestMove = bestMove;            
        }
    }
}
