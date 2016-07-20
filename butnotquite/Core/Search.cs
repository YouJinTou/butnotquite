namespace butnotquite.Core
{
    using Defaults;
    using Models;
    using Utils;

    using System.Collections.Generic;
    using System;
    internal static class Search
    {
        private const int MaxDepth = 3;

        private static Color maximizingSide;

        internal static void Initialize(Chessboard position)
        {
            maximizingSide = position.SideToMove;
            position.MaximizingSideBestMove = new Move();

            DoAlphaBetaPruning(0, int.MinValue, int.MaxValue, position);
        }

        private static int DoAlphaBetaPruning(int depth, int alpha, int beta, Chessboard position)
        {
            if (depth == MaxDepth)
            {
                return Evaluator.EvaluatePosition(position);
            }

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            int gameStateScore = GetGameStateScore(position, availableMoves.Count);

            if (gameStateScore != -1)
            {
                return gameStateScore;
            }

            for (int i = 0; i < availableMoves.Count; i++)
            {
                Move currentMove = availableMoves[i];

                position.MakeMove(currentMove);

                int score = DoAlphaBetaPruning(depth + 1, alpha, beta, position);

                position.UndoMove(currentMove);

                if (position.SideToMove == maximizingSide)
                {
                    if (score > alpha)
                    {
                        alpha = score;

                        if (depth == 0)
                        {
                            position.MaximizingSideBestMove = currentMove;
                        }
                    }
                }
                else
                {
                    if (score <= beta)
                    {
                        beta = score;
                    }
                }

                if (alpha >= beta)
                {
                    return (position.SideToMove == maximizingSide) ? alpha : beta;
                }
            }

            return (position.SideToMove == maximizingSide) ? alpha : beta;
        }

        #region Helpers

        private static int GetGameStateScore(Chessboard position, int availalbeMovesCount)
        {
            if (ThreefoldRepetitionEnforcable(position) || position.FiftyMoveCounter >= 100)
            {
                return 0;
            }

            if (availalbeMovesCount != 0)
            {
                return -1;
            }

            if (position.SideToMove == Color.White)
            {
                if (position.WhiteInCheck)
                {
                    return int.MinValue;
                }
            }
            else
            {
                if (position.BlackInCheck)
                {
                    return int.MaxValue;
                }
            }

            return 0; // Stalemate
        }

        private static bool ThreefoldRepetitionEnforcable(Chessboard position)
        {
            // Will be rewritten
            Stack<GameHistoryEntry> gameHistoryEntries = Utils.MakeDeepCopy(position.GameHistory);
            
            if (gameHistoryEntries.Count < 8)
            {
                return false;
            }

            GameHistoryEntry lastEntry = gameHistoryEntries.Pop();
            GameHistoryEntry nextToLastEntry = gameHistoryEntries.Pop();
            bool drawEnforcable = true;

            for (int i = 1; i <= 4; i++)
            {
                GameHistoryEntry lastEntryPreviousTurn = gameHistoryEntries.Pop();
                GameHistoryEntry nextToLastEntryPreviousTurn = gameHistoryEntries.Pop();

                if (!
                    (
                        (lastEntry.Move.FromSquare == lastEntryPreviousTurn.Move.ToSquare &&
                        lastEntry.Move.ToSquare == lastEntryPreviousTurn.Move.FromSquare &&
                        lastEntry.MovingPiece.Type == lastEntryPreviousTurn.MovingPiece.Type &&
                        lastEntry.MovingPiece.Color == lastEntryPreviousTurn.MovingPiece.Color)

                        &&

                        (nextToLastEntry.Move.FromSquare == nextToLastEntryPreviousTurn.Move.ToSquare &&
                        nextToLastEntry.Move.ToSquare == nextToLastEntryPreviousTurn.Move.FromSquare &&
                        nextToLastEntry.MovingPiece.Type == nextToLastEntryPreviousTurn.MovingPiece.Type &&
                        nextToLastEntry.MovingPiece.Color == nextToLastEntryPreviousTurn.MovingPiece.Color))
                    )
                {
                    drawEnforcable = false;

                    break;
                }

                lastEntry = lastEntryPreviousTurn;
                nextToLastEntry = nextToLastEntryPreviousTurn;
            }

            return drawEnforcable;
        }

        #endregion
    }
}