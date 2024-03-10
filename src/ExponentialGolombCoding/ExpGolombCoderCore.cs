using System;

namespace ExponentialGolombCoding
{
    public static class ExpGolombCoderCore
    {
        public struct NumberCodingInfo
        {
            public uint Code { get; set; }
            public int BitCount { get; set; }
            public int ZeroCount { get; set; }
        }


        public static void Encode(
           ReadOnlySpan<NumberCodingInfo> numbersCoding,
           int totalBitCount,
           Span<byte> result)
        {
            Span<bool> bitsBuffer = stackalloc bool[totalBitCount];
            int offset = 0;
            for (int i = 0; i < numbersCoding.Length; i++)
            {
                var info = numbersCoding[i];
                offset += info.ZeroCount;
                NumberCodingToBites(info.Code, info.BitCount, bitsBuffer[offset..]);
                offset += info.BitCount;
            }

            BitsToBytes(bitsBuffer, result);
        }

        public static int GetTotalBites(ReadOnlySpan<NumberCodingInfo> numbersCoding)
        {
            int sum = 0;
            for (int i = 0; i < numbersCoding.Length; i++) sum += numbersCoding[i].BitCount + numbersCoding[i].ZeroCount;
            return sum;
        }

        public static int GetRequiredBytesLength(int totalBitCount)
        {
            // calculate the number of padding bits required for efficient byte alignment
            var excessBits = totalBitCount % 8;
            var paddingBytes = excessBits == 0 ? 0 : 8 - excessBits;

            return (totalBitCount + paddingBytes) / 8;
        }

        private static void BitsToBytes(Span<bool> bitsBuffer, Span<byte> result)
        {
            var resultIndex = 0;
            for (var i = 0; i < bitsBuffer.Length; i += 8)
            {
                for (int offset = 0; offset < 8 && i + offset < bitsBuffer.Length; offset++)
                {
                    if (bitsBuffer[i + offset])
                    {
                        result[resultIndex] |= (byte)(1 << offset); // Set the corresponding bit to 1
                    }
                }
                resultIndex++;
            }
        }

        private static void NumberCodingToBites(uint code, int bitCount, Span<bool> buffer)
        {
            for (int i = 0; i < bitCount; i++)
            {
                // 1 << (bitCount - 1 - i) is a mask that has a single bit set to 1 at the i-th position.
                // if value i-th bit is 1, then the result of the AND operation will be one.
                buffer[i] = (code & (1 << (bitCount - 1 - i))) != 0;
            }
        }

        public static void CreateNumberCoding(
            uint value,
            short order,
            out NumberCodingInfo codingInfo)
        {
            // calculate x+2k
            var code = value + (uint)Math.Pow(2, order);

            // calculate the min number of bits required to represent the value.
            var bitCount = (int)Math.Ceiling(Math.Log(code + 1, 2));
            var zeroCount = bitCount - 1 - order;

            codingInfo = new NumberCodingInfo
            {
                Code = code,
                BitCount = bitCount,
                ZeroCount = zeroCount
            };
        }

        public static void Decode(
            ReadOnlySpan<bool> bits,
            short order,
            Span<uint> result,
            out int written)
        {
            written = 0;
            var readOffset = 0;
            while (readOffset < bits.Length)
            {
                if (TryDecodeBitToNumber(bits[readOffset..], order, out var number, out var bytesConsumed))
                {
                    result[written] = number;
                    written++;
                }
                readOffset += bytesConsumed;
            }
        }

        public static int GetMaxDecodedLength(ReadOnlySpan<bool> bits, short order)
        {
            var index = 0;
            var result = 0;
            while (index < bits.Length)
            {
                // Read the leading zeros
                var i = 0;
                for (; i + index < bits.Length && !bits[i + index]; i++) ;
                result++;
                index += (i * 2) + order + 1;
            }

            return result;
        }

        private static bool TryDecodeBitToNumber(ReadOnlySpan<bool> bitsBuffer, short order, out uint result, out int bytesConsumed)
        {
            // Read the leading zeros
            var i = 0;
            for (; i < bitsBuffer.Length && !bitsBuffer[i]; i++) ;
            int len = i + (i + order + 1);

            if (len > bitsBuffer.Length)
            {
                result = 0;
                bytesConsumed = bitsBuffer.Length;
                return false;
            }

            // Extract the payload.
            int value = 0;
            for (; i < len; i++)
            {
                value = (value << 1) | (bitsBuffer[i] ? 1 : 0);
            }

            result = (uint)(value - Math.Pow(2, order));
            bytesConsumed = i;
            return true;
        }

        public static void BytesToBits(ReadOnlySpan<byte> bytes, Span<bool> bitsBuffer)
        {
            var index = 0;
            foreach (var b in bytes)
            {
                for (int offset = 0; offset < 8; offset++)
                {
                    if ((b & (1 << offset)) != 0)
                    {
                        bitsBuffer[index * 8 + offset] = true;
                    }
                }

                index++;
            }
        }
    }
}
