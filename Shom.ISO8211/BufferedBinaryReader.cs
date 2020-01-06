namespace Shom.ISO8211
{
    using System;
    using System.IO;


    public class BufferedBinaryReader : IDisposable
    {
        private readonly Stream stream;
        private readonly byte[] buffer;
        private readonly int bufferSize;
        private int bufferOffset;
        private int numBufferedBytes;
        public int BytesRead;
        bool EOFreached;

        public BufferedBinaryReader(Stream stream, int bufferSize)
        {
            this.stream = stream;
            this.bufferSize = bufferSize;
            buffer = new byte[bufferSize];
            bufferOffset = bufferSize;
        }

        public int NumBytesAvailable { get { return Math.Max(0, numBufferedBytes - bufferOffset); } }

        public bool FillBuffer()
        {
            if (!EOFreached)
            {
                var numBytesUnread = bufferSize - bufferOffset;
                var numBytesToRead = bufferSize - numBytesUnread;
                bufferOffset = 0;
                numBufferedBytes = numBytesUnread;
                if (numBytesUnread > 0)
                {
                    Buffer.BlockCopy(buffer, numBytesToRead, buffer, 0, numBytesUnread);
                }
                while (numBytesToRead > 0)
                {
                    var numBytesRead = stream.Read(buffer, numBytesUnread, numBytesToRead);
                    if (numBytesRead == 0)
                    {
                        EOFreached = true;
                        return false;
                    }
                    numBufferedBytes += numBytesRead;
                    numBytesToRead -= numBytesRead;
                    numBytesUnread += numBytesRead;
                }
                return true;
            }
            else
                return false;
        }

        public ushort ReadUInt16()
        {
            var val = (ushort)((int)buffer[bufferOffset] | (int)buffer[bufferOffset + 1] << 8);
            bufferOffset += 2;
            return val;
        }

        public byte ReadByte()
        {
            if(NumBytesAvailable==0)
                FillBuffer();
            byte val = buffer[bufferOffset];
            bufferOffset++;
            BytesRead++;
            return val;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];         
            int numRead = 0;
            int transfer;
            if (NumBytesAvailable<count)
                FillBuffer();
            do
            {                
                if (NumBytesAvailable == 0)
                    break;
                transfer = Math.Min(NumBytesAvailable, count);
                Buffer.BlockCopy(buffer, bufferOffset, result, numRead, transfer);
                bufferOffset += transfer;
                numRead += transfer;
                count -= transfer;
                FillBuffer();
            } while (NumBytesAvailable>0 && count > 0);

            if (numRead != result.Length)
            {
                // Trim array.  This should happen on EOF & possibly net streams.
                byte[] copy = new byte[numRead];
                Buffer.BlockCopy(result, 0, copy, 0, numRead);
                result = copy;
            }
            BytesRead += numRead;
            return result;
        }               

        public void Dispose()
        {
            stream.Close();
        }
    }
}
