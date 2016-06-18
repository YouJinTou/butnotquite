﻿namespace butnotquite.Models
{
    using Defaults;
    using Utils;

    using System.Collections.Generic;

    internal sealed class Chessboard
    {
        internal Square[] Board;
        internal int MoveCounter;
        internal Color SideToMove;
        internal Color OppositeColor;
        internal int Evaluation;

        internal IDictionary<Piece, HashSet<int>> OpponentControl;

        internal int WhiteKingPosition;
        internal int BlackKingPosition;
        internal bool WhiteCanCastle;
        internal bool BlackCanCastle;

        internal int RepetitionCounter;
        internal bool RepetitionEnforcable;
        internal int FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;
        internal bool Stalemate;

        internal Move LastMove;
        internal int EnPassantSquare; // To be implemented

        internal Chessboard(bool fromFen)
        {
            if (!fromFen)
            {
                this.InitializeStartingPosition();
            }

            this.SideToMove = Color.White;
            this.OppositeColor = Color.Black;

            this.WhiteKingPosition = 60;
            this.BlackKingPosition = 4;

            this.OpponentControl = new Dictionary<Piece, HashSet<int>>(30);
        }               

        internal void MakeMove(int fromSquare, int toSquare)
        {
            Piece movingPiece = Utils.MakeDeepCopy(this.Board[fromSquare].OccupiedBy);

            this.ResetSquare(fromSquare);
            this.Board[toSquare].OccupiedBy = movingPiece;
            movingPiece.Position = toSquare;

            this.LastMove = new Move(fromSquare, toSquare);

            this.SideToMove = (this.SideToMove == Color.White) ? Color.Black : Color.White;
            this.OppositeColor = (this.SideToMove == Color.White) ? Color.Black : Color.White;
        }

        #region Board Initialization

        private void InitializeStartingPosition()
        {
            Chessboard tempBoard = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            this.Board = tempBoard.Board;
        }

        #endregion

        #region Utils

        private void ResetSquare(int squareNumber)
        {
            Square square = this.Board[squareNumber];

            square.OccupiedBy.Type = PieceType.None;
            square.OccupiedBy.Color = Color.None;
            square.OccupiedBy.Value = 0;
            square.OccupiedBy.Position = -1;
            square.OccupiedBy.Moves = null;
        }

        #endregion
    }
}