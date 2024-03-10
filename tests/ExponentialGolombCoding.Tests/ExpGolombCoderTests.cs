namespace ExponentialGolombCoding.Tests;

public class ExpGolombCoderTests
{
    private readonly ExpGolombCoder _exponentialGolombCoding = new();

    [Theory]
    [InlineData(0, "10000000")]
    [InlineData(1, "01000000")]
    [InlineData(2, "01100000")]
    [InlineData(3, "00100000")]
    [InlineData(15, "0000100000000000")]
    [InlineData(29, "0000111100000000")]
    public void Encode_WithSingleValueAndOrder0_ReturnEncodedBytes(uint value, string result)
    {
        var bytes = _exponentialGolombCoding.Encode(value, 0);
        Assert.Equal(result, BytesHelper.BytesToBitString(bytes));
    }

    [Theory]
    [InlineData(0, "10000000")]
    [InlineData(1, "11000000")]
    [InlineData(2, "01000000")]
    [InlineData(3, "01010000")]
    [InlineData(15, "00010001")]
    [InlineData(29, "00011111")]
    public void Encode_WithSingleValueAndOrder1_ReturnEncodedBytes(uint value, string result)
    {
        var bytes = _exponentialGolombCoding.Encode(value, 1);
        Assert.Equal(result, BytesHelper.BytesToBitString(bytes));
    }

    [Theory]
    [InlineData(0, "10000000")]
    [InlineData(1, "10010000")]
    [InlineData(2, "10100000")]
    [InlineData(3, "10110000")]
    [InlineData(15, "01011100")]
    [InlineData(29, "00100101")]
    public void Encode_WithSingleValueAndOrder3_ReturnEncodedBytes(uint value, string result)
    {
        var bytes = _exponentialGolombCoding.Encode(value, 3);
        Assert.Equal(result, BytesHelper.BytesToBitString(bytes));
    }

    [Fact]
    public void Encode_WithMultipleValuesAndOrder0_ReturnEncodedBytes()
    {
        var bytes = _exponentialGolombCoding.Encode([12, 11, 334], 0);
        Assert.Equal([88, 12, 64, 121], bytes);
    }

    [Fact]
    public void Encode_WithMultipleValuesAndOrder10_ReturnEncodedBytes()
    {
        var bytes = _exponentialGolombCoding.Encode([29, 440, 99123], 10);
        Assert.Equal([193, 109, 7, 48, 156, 25], bytes);
    }

    [Fact]
    public void EncodeToBase64_WithOrder0_ReturnEncodedString()
    {
        var bytes = _exponentialGolombCoding.EncodeToBase64([3, 5, 99], 0);
        Assert.Equal("hAET", bytes);
    }

    [Fact]
    public void EncodeToBase64_WithOrder10_ReturnEncodedString()
    {
        var bytes = _exponentialGolombCoding.EncodeToBase64([29, 440, 99123], 10);
        Assert.Equal("wW0HMJwZ", bytes);
    }

    [Fact]
    public void DecodeBase64_WithOrder0_ReturnNumbers()
    {
        var numbers = _exponentialGolombCoding.DecodeFromBase64("hAET", 0);
        Assert.Equal([3, 5, 99], numbers);
    }

    [Fact]
    public void DecodeBase64_WithOrder10_ReturnNumbers()
    {
        var numbers = _exponentialGolombCoding.DecodeFromBase64("wW0HMJwZ", 10);
        Assert.Equal([29, 440, 99123], numbers);
    }

    [Fact]
    public void Decode_WithSingleValueOrder0_ReturnNumbers()
    {
        //11110000 = 240
        var numbers = _exponentialGolombCoding.Decode([240, 0], 0);
        Assert.Equal([29], numbers);
    }

    [Fact]
    public void Decode_WithMultipleValueOrder0_ReturnNumbers()
    {
        var numbers = _exponentialGolombCoding.Decode([88, 12, 64, 121], 0);
        Assert.Equal([12, 11, 334], numbers);
    }

    [Fact]
    public void Decode_WithMultipleValuesAndOrder10_ReturnEncodedBytes()
    {
        var numbers = _exponentialGolombCoding.Decode([193, 109, 7, 48, 156, 25], 10);
        Assert.Equal([29, 440, 99123], numbers);
    }
}