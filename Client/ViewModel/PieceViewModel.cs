namespace Client.ViewModel
{
    using butnotquite.Defaults;
    using butnotquite.Models;

    using GalaSoft.MvvmLight;
    using System.Windows;

    public class PieceViewModel : ViewModelBase
    {
        private PieceType pieceType;
        private Color color;
        private Point position;

        public PieceType PieceType
        {
            get
            {
                return this.pieceType;
            }

            set
            {
                this.pieceType = value;
                base.RaisePropertyChanged(() => this.PieceType);
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }

            set
            {
                this.color = value;
                base.RaisePropertyChanged(() => this.Color);
            }
        }

        public Point Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
                base.RaisePropertyChanged(() => this.Position);
            }
        }
    }
}
