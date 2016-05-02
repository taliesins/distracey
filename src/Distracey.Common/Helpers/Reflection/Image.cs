using System;
using System.IO;

namespace Distracey.Common.Helpers.Reflection
{
    public sealed class Image : IDisposable
    {
        readonly long _position;
        readonly Stream _stream;

        Image(Stream stream)
        {
            _stream = stream;
            _position = stream.Position;
            _stream.Position = 0;
        }

        bool Advance(int length)
        {
            if (_stream.Position + length >= _stream.Length)
                return false;

            _stream.Seek(length, SeekOrigin.Current);
            return true;
        }

        bool MoveTo(uint position)
        {
            if (position >= _stream.Length)
                return false;

            _stream.Position = position;
            return true;
        }

        void IDisposable.Dispose()
        {
            _stream.Position = _position;
        }

        ushort ReadUInt16()
        {
            return (ushort)(_stream.ReadByte()
                | (_stream.ReadByte() << 8));
        }

        uint ReadUInt32()
        {
            return (uint)(_stream.ReadByte()
                | (_stream.ReadByte() << 8)
                | (_stream.ReadByte() << 16)
                | (_stream.ReadByte() << 24));
        }

        bool IsManagedAssembly()
        {
            if (_stream.Length < 318)
                return false;
            if (ReadUInt16() != 0x5a4d)
                return false;
            if (!Advance(58))
                return false;
            if (!MoveTo(ReadUInt32()))
                return false;
            if (ReadUInt32() != 0x00004550)
                return false;
            if (!Advance(20))
                return false;
            if (!Advance(ReadUInt16() == 0x20b ? 222 : 206))
                return false;

            return ReadUInt32() != 0;
        }

        public static bool IsAssembly(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                return IsAssembly(stream);
        }

        public static bool IsAssembly(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Can not read from stream");
            if (!stream.CanSeek)
                throw new ArgumentException("Can not seek in stream");

            using (var image = new Image(stream))
                return image.IsManagedAssembly();
        }
    }
}