namespace butnotquite.Engine
{
    using Core;
    using Defaults;
    using Models;
    using Utils;

    using System;
    using System.Diagnostics;

    internal sealed class Engine
    {
        public static void Main()
        {
            Chessboard position = Utils.LoadPositionFromFenString("1q3n1r/P5P1/3k4/8/8/NnP1p1p1/PP3P11/5K2 w - - 0 50");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            MoveGenerator.GetAvailableMoves(position);
            position.MakeMove(52, 36, Direction.Vertical);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw.Reset();

            sw.Start();
            
            MoveGenerator.GetAvailableMoves(position);
            position.MakeMove(12, 28, Direction.Vertical);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw.Reset();

            sw.Start();

            MoveGenerator.GetAvailableMoves(position);
            position.MakeMove(12, 28, Direction.Vertical);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}
