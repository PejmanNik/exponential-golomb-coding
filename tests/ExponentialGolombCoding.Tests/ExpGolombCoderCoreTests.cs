namespace ExponentialGolombCoding.Tests;

public class ExpGolombCoderCoreTests
{
    [Fact]
    public void Decode_WithBitsAndOrder10_ReturnNumbers()
    {
        Span<bool> bits = stackalloc bool[6 * 8];
        ExpGolombCoderCore.BytesToBits([193, 109, 7, 48, 156, 25], bits);

        var resultMaxLen = ExpGolombCoderCore.GetMaxDecodedLength(bits, 10);
        Span<uint> result = stackalloc uint[resultMaxLen];

        ExpGolombCoderCore.Decode(bits, 10, result, out var written);
        Assert.Equal([29, 440, 99123], result[..written].ToArray());
    }

    [Fact]
    public void Encode_WithNumbersCodingOrder0_ReturnEncodedBytes()
    {
        Span<ExpGolombCoderCore.NumberCodingInfo> numbersCoding = stackalloc ExpGolombCoderCore.NumberCodingInfo[2];
        ExpGolombCoderCore.CreateNumberCoding(22, 0, out numbersCoding[0]);
        ExpGolombCoderCore.CreateNumberCoding(14, 0, out numbersCoding[1]);
        var totalBitCount = ExpGolombCoderCore.GetTotalBites(numbersCoding);

        var bytesLength = ExpGolombCoderCore.GetRequiredBytesLength(totalBitCount);
        Span<byte> result = stackalloc byte[bytesLength];

        ExpGolombCoderCore.Encode(numbersCoding, totalBitCount, result);
        Assert.Equal(208, result[0]);
        Assert.Equal(241, result[1]);
    }

    [Fact]
    public void BytesToBits_ReturnBits()
    {
        Span<bool> bits = stackalloc bool[2 * 8];
        ExpGolombCoderCore.BytesToBits([208, 241], bits);

        Assert.Equal(
            BytesHelper.BytesToBitString([208, 241]),
            string.Join("", bits.ToArray().Select(x => x ? "1" : "0"))
        );
    }

    [Fact]
    public void GetTotalBites_ReturnSumOfBitCountAndZeroCount()
    {
        var result = ExpGolombCoderCore.GetTotalBites([
            new ExpGolombCoderCore.NumberCodingInfo
            {
                BitCount = 3,
                ZeroCount = 2,
            },
            new ExpGolombCoderCore.NumberCodingInfo
            {
                BitCount = 5,
                ZeroCount = 6,
            },
            ]);

        Assert.Equal(16, result);
    }

    [Fact]
    public void GetRequiredBytesLength_WithAMultipleOfThree_ReturnBytesCount()
    {
        var result = ExpGolombCoderCore.GetRequiredBytesLength(16);
        Assert.Equal(2, result);
    }

    [Fact]
    public void GetRequiredBytesLength_WithANonMultipleOfThree_ReturnBytesCount()
    {
        var result = ExpGolombCoderCore.GetRequiredBytesLength(17);
        Assert.Equal(3, result);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("111", 3)]
    [InlineData("000011110", 1)]
    [InlineData("000011110000011110", 2)]
    [InlineData("010011010", 3)]
    public void GetMaxDecodedLength_WithOrder0_ReturnMaxPossibleEncodedNumber(string bits, int maxLen)
    {
        var result = ExpGolombCoderCore.GetMaxDecodedLength(
            BytesHelper.BitStringToBools(bits).ToArray(), 0);

        Assert.Equal(maxLen, result);
    }

    [Theory]
    [InlineData("1000", 1)]
    [InlineData("011110", 1)]
    [InlineData("010000010001", 2)]
    [InlineData("10001001", 2)]
    [InlineData("100010011111", 3)]
    public void GetMaxDecodedLength_WithOrder3_ReturnMaxPossibleEncodedNumber(string bits, int maxLen)
    {
        var result = ExpGolombCoderCore.GetMaxDecodedLength(
            BytesHelper.BitStringToBools(bits).ToArray(), 3);

        Assert.Equal(maxLen, result);
    }
}
