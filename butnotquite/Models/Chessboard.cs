namespace butnotquite.Models
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
        internal Move MaximizingSideBestMove;
        internal Piece LastMoveCapturedPiece;

        internal IDictionary<Piece, HashSet<int>> OpponentActivity;

        internal bool WhiteInCheck;
        internal bool BlackInCheck;
        internal bool KingInCheck;
        internal int WhiteKingPosition;
        internal int BlackKingPosition;
        internal bool WhiteCanCastle;
        internal bool BlackCanCastle;

        internal int RepetitionCounter;
        internal bool RepetitionEnforcable;
        internal int FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;

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

            this.OpponentActivity = new Dictionary<Piece, HashSet<int>>(30);
        }        

        internal void MakeMove(int fromSquare, int toSquare, Direction direction)
        {
            Piece movingPiece = Utils.MakeDeepCopy(this.Board[fromSquare].OccupiedBy);
            this.LastMoveCapturedPiece = Utils.MakeDeepCopy(this.Board[toSquare].OccupiedBy);

            this.ResetSquare(fromSquare);
            this.Board[toSquare].OccupiedBy = movingPiece;
            movingPiece.Position = toSquare;

            this.LastMove = new Move(fromSquare, toSquare, direction);
            bool isPawn = (movingPiece.Type == PieceType.Pawn);
            bool isCapture = (LastMoveCapturedPiece.Type != PieceType.None);
            this.FiftyMoveCounter = (isPawn || isCapture) ? 0 : (this.FiftyMoveCounter + 1);

            this.SwapSides();
        }

        internal void UndoMove(Move move)
        {
            Piece movingPiece = Utils.MakeDeepCopy(this.Board[move.ToSquare].OccupiedBy);
            
            this.Board[move.ToSquare].OccupiedBy = this.LastMoveCapturedPiece;
            this.Board[move.FromSquare].OccupiedBy = movingPiece;
            movingPiece.Position = move.FromSquare;

            this.SwapSides();
        }

        #region Board Initialization

        private void InitializeStartingPosition()
        {
            Chessboard tempPosition = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            this.Board = tempPosition.Board;
        }

        #endregion

        #region Utils

        internal void SwapSides()
        {
            this.SideToMove = (this.SideToMove == Color.White) ? Color.Black : Color.White;
            this.OppositeColor = (this.SideToMove == Color.White) ? Color.Black : Color.White;
        }

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