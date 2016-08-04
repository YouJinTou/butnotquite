namespace Client
{
    using butnotquite.Models;
    using butnotquite.Defaults;
    using ViewModel;

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class MainWindow : Window
    {
        private const double SquareWidth = 75;
        private const double SquareHeight = 75;

        private Chessboard chessboard;
        private ObservableCollection<PieceViewModel> pieces;
        private bool squareSelected;
        private int fromSquare;
        private int toSquare;

        public MainWindow()
        {
            InitializeComponent();
            this.InitializeGame();
        }

        private void InitializeGame()
        {
            this.chessboard = new Chessboard(false);
            this.pieces = new ObservableCollection<PieceViewModel>();

            for (int square = 0; square < this.chessboard.Board.Length; square++)
            {
                int x = square % 8;
                int y = square / 8;
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

        private void square_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            int row = (int)Math.Floor(position.Y / SquareHeight);
            int col = (int)Math.Floor(position.X / SquareWidth);
            int squareIndex = (row * 8) + col;

            if (!this.squareSelected)
            {
                if (this.chessboard.Board[squareIndex].OccupiedBy.Type == PieceType.None)
                {
                    return;
                }

                this.fromSquare = squareIndex;
                this.squareSelected = true;
            }
            else
            {
                this.toSquare = squareIndex;
                Move newMove = new Move(fromSquare, toSquare, Direction.SingleSquare);
                this.squareSelected = false;

                this.chessboard.MakeMove(newMove);
                this.UpdateGameState();
            }
        }

        private void UpdateGameState()
        {
            int fromRow = this.fromSquare / 8;
            int toRow = this.toSquare / 8;
            int toCol = this.toSquare % 8;
            int fromCol = this.fromSquare % 8;
            Point fromPosition = new Point(fromCol, fromRow);
            Point toPosition = new Point(toCol, toRow);
            PieceViewModel movingPiece = this.pieces.FirstOrDefault(piece => 
                piece.Position.X == fromPosition.X && 
                piece.Position.Y == fromPosition.Y);

            movingPiece.Position = toPosition;
        }
    }
}