using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using static ExponentialGolombCoding.ExpGolombCoderCore;

namespace ExponentialGolombCoding
{
    public class ExpGolombCoder
    {
        public byte[] Encode(uint value, short order)
        {
            Span<NumberCodingInfo> numbersCoding = stackalloc NumberCodingInfo[1];
            CreateNumberCoding(value, order, out numbersCoding[0]);
            var totalBitCount = GetTotalBites(numbersCoding);

            var bytesLength = GetRequiredBytesLength(totalBitCount);
            Span<byte> buffer = stackalloc byte[bytesLength];

            ExpGolombCoderCore.Encode(numbersCoding, totalBitCount, buffer);

            return buffer.ToArray();
        }

        public byte[] Encode(IReadOnlyCollection<uint> values, short order)
        {
            Span<NumberCodingInfo> numbersCoding = stackalloc NumberCodingInfo[values.Count];
            var totalBitCount = CreateNumbersCoding(values, order, numbersCoding);

            var bytesLength = GetRequiredBytesLength(totalBitCount);
            Span<byte> buffer = stackalloc byte[bytesLength];

            ExpGolombCoderCore.Encode(numbersCoding, totalBitCount, buffer);
            return buffer.ToArray();
        }

        public string EncodeToBase64(IReadOnlyCollection<uint> values, short order)
        {
            Span<NumberCodingInfo> numbersCoding = stackalloc NumberCodingInfo[values.Count];
            var totalBitCount = CreateNumbersCoding(values, order, numbersCoding);

            var bytesLength = GetRequiredBytesLength(totalBitCount);
            Span<byte> buffer = stackalloc byte[bytesLength];

            ExpGolombCoderCore.Encode(numbersCoding, totalBitCount, buffer);

            var base64Len = Base64.GetMaxEncodedToUtf8Length(buffer.Length);
            Span<byte> base64 = stackalloc byte[base64Len];
            Base64.EncodeToUtf8(buffer, base64, out _, out int written);

            return Encoding.UTF8.GetString(base64[..written]);
        }

        public IEnumerable<uint> Decode(byte[] bytes, short order)
        {
            Span<bool> bits = stackalloc bool[bytes.Length * 8];
            BytesToBits(bytes, bits);

            var resultMaxLen = GetMaxDecodedLength(bits, order);
            var result = new uint[resultMaxLen];
            ExpGolombCoderCore.Decode(bits, order, result, out var written);

            return result[..written];
        }

        public IReadOnlyCollection<uint> DecodeFromBase64(string base64, short order)
        {
            var base64BytesLen = Encoding.UTF8.GetByteCount(base64);
            Span<byte> base64Bytes = stackalloc byte[base64BytesLen];
            Encoding.UTF8.GetBytes(base64, base64Bytes);

            var decodedBytesLen = Base64.GetMaxDecodedFromUtf8Length(base64Bytes.Length);
            Span<byte> decodedBytes = stackalloc byte[base64BytesLen];
            Base64.DecodeFromUtf8(base64Bytes, decodedBytes, out _, out int writtenBase64);

            Span<bool> bits = stackalloc bool[writtenBase64 * 8];
            BytesToBits(decodedBytes[..writtenBase64], bits);

            var resultMaxLen = GetMaxDecodedLength(bits, order);
            var result = new uint[resultMaxLen];
            ExpGolombCoderCore.Decode(bits, order, result, out var written);

            return result[..written];
        }

        private static int CreateNumbersCoding(
            IEnumerable<uint> values,
            short order,
            Span<NumberCodingInfo> codingInfo)
        {
            var totalBitCount = 0;
            var index = 0;
            foreach (var value in values)
            {
                CreateNumberCoding(value, order, out codingInfo[index]);
                totalBitCount += codingInfo[index].BitCount + codingInfo[index].ZeroCount;

                index++;
            }

            return totalBitCount;
        }
    }
}
