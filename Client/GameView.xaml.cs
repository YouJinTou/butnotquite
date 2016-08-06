namespace Client
{
    using Engine.Core;
    using Engine.Core.Search;
    using Engine.Models;
    using Engine.Defaults;
    using ViewModel;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class GameView : Window
    {
        private Chessboard chessboard;
        private ObservableCollection<PieceViewModel> pieces;
        private Engine.Defaults.Color playerColor;
        private int computerStrength;
        private IDictionary<int, int[]> colBorders;
        private IDictionary<int, int[]> rowBorders;
        private bool squareSelected;
        private int fromSquare;
        private int toSquare;
        private Border lastSelectedSquare;
        private bool computerMoved;

        public GameView(Engine.Defaults.Color playerColor, int depth)
        {
            InitializeComponent();

            this.chessboard = new Chessboard(false);
            this.pieces = new ObservableCollection<PieceViewModel>();
            this.playerColor = playerColor;
            this.computerStrength = depth;
            this.computerMoved = (playerColor == Engine.Defaults.Color.White) ? true : false;

            this.InitializeSquareBorders();
            this.BindPieces(false);

            if (!this.computerMoved) // Player has chosen black
            {
                this.GetComputerResponse();
            }
        }

        #region Initialization

        private void InitializeSquareBorders()
        {
            this.colBorders = new Dictionary<int, int[]>();
            this.rowBorders = new Dictionary<int, int[]>();
            int rowFrom = 0;
            int rowTo = 53;
            int colFrom = 10;
            int colTo = 63;

            for (int row = 0; row < 8; row++)
            {
                if (!this.rowBorders.ContainsKey(row))
                {
                    this.rowBorders.Add(row, new int[] { rowFrom, rowTo });
                }

                rowFrom = rowTo;
                rowTo += 53;

                for (int col = 0; col < 8; col++)
                {
                    if (!this.colBorders.ContainsKey(col))
                    {
                        this.colBorders.Add(col, new int[] { colFrom, colTo });
                    }

                    colFrom = colTo;
                    colTo += 53;
                }
            }
        }
        
        #endregion

        #region Game State
                
        private bool UpdateGameState(bool differentThread)
        {
            int fromRow = this.fromSquare / 8;
            int toRow = this.toSquare / 8;
            int fromCol = this.fromSquare % 8;
            int toCol = this.toSquare % 8;
            Point fromPosition = new Point(fromCol, fromRow);
            Point toPosition = new Point(toCol, toRow);
            PieceViewModel movingPiece = this.pieces.FirstOrDefault(piece =>
                piece.Position.X == fromPosition.X &&
                piece.Position.Y == fromPosition.Y);
            movingPiece.Position = toPosition;

            this.BindPieces(differentThread);
            return this.IsGameOver();
        }

        private bool IsGameOver()
        {
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(this.chessboard);

            if (availableMoves.Count == 0)
            {
                if (this.chessboard.KingInCheck)
                {
                    string victor = this.chessboard.OppositeColor == Engine.Defaults.Color.White ? "White" : "Black";

                    MessageBox.Show(victor + " wins.");
                }
                else
                {
                    MessageBox.Show("Game drawn.");
                }

                return true;
            }

            return false;
        }

        private void BindPieces(bool differentThread)
        {
            if (differentThread)
            {
                App.Current.Dispatcher.Invoke(this.BindPieces);
            }
            else
            {
                this.BindPieces();
            }
        }

        private void BindPieces()
        {
            this.pieces.Clear();

            for (int square = 0; square < this.chessboard.Board.Length; square++)
            {
                int row = square / 8;
                int col = square % 8;
                int x = (this.playerColor == Engine.Defaults.Color.White) ? col : Math.Abs(col - 7);
                int y = (this.playerColor == Engine.Defaults.Color.White) ? row : Math.Abs(row - 7);
                Point position = new Point(x, y);
                PieceViewModel pieceModel = new PieceViewModel()
                {
                    PieceType = this.chessboard.Board[square].OccupiedBy.Type,
                    Color = this.chessboard.Board[square].OccupiedBy.Color,
                    Position = position
                };

                this.pieces.Add(pieceModel);
            }

            this.ChessboardUI.ItemsSource = this.pieces;
        }

        #endregion

        #region Handlers

        private void square_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.chessboard.SideToMove != this.playerColor || !this.computerMoved)
            {
                return;
            }

            Border currentSquare = (Border)sender;
            Point position = e.GetPosition(this);
            int row = this.rowBorders.FirstOrDefault(kvp => kvp.Value[0] <= position.Y && kvp.Value[1] >= position.Y).Key;
            int col = this.colBorders.FirstOrDefault(kvp => kvp.Value[0] <= position.X && kvp.Value[1] >= position.X).Key;
            int squareIndex = (this.playerColor == Engine.Defaults.Color.White) ? 
                ((row * 8) + col) : 
                ((Math.Abs((row - 7) * 8) + (Math.Abs(col - 7))));

            if (!this.squareSelected)
            {
                if (this.chessboard.Board[squareIndex].OccupiedBy.Type == PieceType.None)
                {
                    return;
                }

                if (this.chessboard.Board[squareIndex].OccupiedBy.Color != this.chessboard.SideToMove)
                {
                    return;
                }

                this.fromSquare = squareIndex;
                this.squareSelected = true;

                currentSquare.BorderBrush = Brushes.Gray;
                currentSquare.BorderThickness = new Thickness(0.05);
            }
            else
            {
                if (squareIndex == this.fromSquare || 
                    this.chessboard.Board[squareIndex].OccupiedBy.Color == this.chessboard.SideToMove)
                {
                    this.ResetSquare(currentSquare);

                    return;
                }

                Move playerMove = this.GetHumanMove(squareIndex);

                if (playerMove.Id == 0)
                {
                    this.ResetSquare(currentSquare);

                    return;
                }

                this.squareSelected = false;

                this.chessboard.MakeMove(playerMove);

                if (this.UpdateGameState(false))
                {
                    return;
                }

                this.GetComputerResponse();
            }

            this.lastSelectedSquare = currentSquare;
        }

        #endregion

        #region Helpers

        private Move GetHumanMove(int squareIndex)
        {
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(this.chessboard);

            if (availableMoves.Count == 0)
            {
                return new Move();
            }

            Piece movingPiece = this.chessboard.Board[fromSquare].OccupiedBy;
            this.toSquare = squareIndex;
            bool isKing = (movingPiece.Type == PieceType.King);
            bool isCastleAttempt = isKing ? (Math.Abs(this.fromSquare - this.toSquare) > 1) : false;
            bool canCastle = availableMoves.Any(m => m.Direction == Direction.Castle);
            bool isValidCastleAttempt = (isKing && isCastleAttempt && canCastle) ?
                availableMoves.Any(m => m.KingToSquare == this.toSquare) : false;
            bool isValidNonCastleMove = isValidCastleAttempt ? false :
                availableMoves.Any(m => m.FromSquare == this.fromSquare && m.ToSquare == this.toSquare);

            if (isValidNonCastleMove)
            {
                return availableMoves.FirstOrDefault(m => m.FromSquare == this.fromSquare && m.ToSquare == this.toSquare);
            }
            else if (isValidCastleAttempt)
            {
                return availableMoves.FirstOrDefault(m => m.KingFromSquare == this.fromSquare && m.KingToSquare == this.toSquare);
            }

            return new Move();
        }

        private void GetComputerResponse()
        {
            Thread searchThread = new Thread(this.SearchThread);

            searchThread.Start();
        }

        private void SearchThread()
        {
            this.computerMoved = false;

            Search.Initialize(this.chessboard, this.computerStrength);

            this.chessboard.MakeMove(this.chessboard.MaximizingSideBestMove);
            this.UpdateGameState(true);

            this.computerMoved = true;
        }

        private void ResetSquare(Border currentSquare)
        {
            this.squareSelected = false;
            this.lastSelectedSquare.BorderBrush = null;
            currentSquare.BorderBrush = null;
        }

        #endregion
    }
}