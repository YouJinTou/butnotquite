namespace Engine.Tests
{
    using Engine.Core.Search;
    using Engine.Defaults;
    using Engine.Models;
    using Engine.Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class SearchTests
    {
        [TestMethod]
        public void AlphaBeta_ShouldReturn6AsBestScoreWithWhiteToMoveFirst()
        {
            Chessboard position = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            List<Move> movesToMake = new List<Move>()
            {
                new Move(52, 36, Direction.Vertical), // e4
                new Move(12, 28, Direction.Vertical), // e5
                new Move(62, 45, Direction.L), // Nf3
                new Move(1, 18, Direction.L) // Nc6
            };

            //Search.Initialize(position);
        }
    }
}
