namespace Engine.Core.Search.Zobrist
{
    using System;
    using System.Security.Cryptography;

    internal sealed class ZobristRandomizer : RandomNumberGenerator
    {
        private RandomNumberGenerator rng;

        internal ZobristRandomizer()
        {
            this.rng = RandomNumberGenerator.Create();
        }

        public long NextLong()
        {
            byte[] bytes = new byte[8];

            rng.GetBytes(bytes);

            return BitConverter.ToInt64(bytes, 0);
        }

        public override void GetBytes(byte[] data)
        {
            rng.GetBytes(data);
        }
    }
}
