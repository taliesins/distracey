using System;

namespace Distracey.Common.Helpers.Reflection
{
    class ByteBuffer
    {
        internal readonly byte[] Buffer;
        internal int Position;

        public ByteBuffer(byte[] buffer)
        {
            Buffer = buffer;
        }

        public byte ReadByte()
        {
            CheckCanRead(1);
            return Buffer[Position++];
        }

        public byte[] ReadBytes(int length)
        {
            CheckCanRead(length);
            var value = new byte[length];
            System.Buffer.BlockCopy(Buffer, Position, value, 0, length);
            Position += length;
            return value;
        }

        public short ReadInt16()
        {
            CheckCanRead(2);
            short value = (short)(Buffer[Position]
                | (Buffer[Position + 1] << 8));
            Position += 2;
            return value;
        }

        public int ReadInt32()
        {
            CheckCanRead(4);
            int value = Buffer[Position]
                | (Buffer[Position + 1] << 8)
                | (Buffer[Position + 2] << 16)
                | (Buffer[Position + 3] << 24);
            Position += 4;
            return value;
        }

        public long ReadInt64()
        {
            CheckCanRead(8);
            uint low = (uint)(Buffer[Position]
                | (Buffer[Position + 1] << 8)
                | (Buffer[Position + 2] << 16)
                | (Buffer[Position + 3] << 24));

            uint high = (uint)(Buffer[Position + 4]
                | (Buffer[Position + 5] << 8)
                | (Buffer[Position + 6] << 16)
                | (Buffer[Position + 7] << 24));

            long value = (((long)high) << 32) | low;
            Position += 8;
            return value;
        }

        public float ReadSingle()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var bytes = ReadBytes(4);
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }

            CheckCanRead(4);
            float value = BitConverter.ToSingle(Buffer, Position);
            Position += 4;
            return value;
        }

        public double ReadDouble()
        {
            if (!BitConverter.IsLittleEndian)
            {
                var bytes = ReadBytes(8);
                Array.Reverse(bytes);
                return BitConverter.ToDouble(bytes, 0);
            }

            CheckCanRead(8);
            double value = BitConverter.ToDouble(Buffer, Position);
            Position += 8;
            return value;
        }

        void CheckCanRead(int count)
        {
            if (Position + count > Buffer.Length)
                throw new ArgumentOutOfRangeException();
        }
    }
}