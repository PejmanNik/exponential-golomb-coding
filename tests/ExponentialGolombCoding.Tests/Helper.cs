using System.Collections;
using System.Text;

namespace ExponentialGolombCoding.Tests;

internal static class BytesHelper
{
    public static IEnumerable<bool> BitStringToBools(string bits)
    {
        for (var i = 0; i < bits.Length; i++)
        {
            yield return bits[i] == '1';
        }
    }

    public static string BytesToBitString(byte[] bytes)
    {
        var bits = new BitArray(bytes);
        var sb = new StringBuilder();

        for (var i = 0; i < bits.Length; i++)
        {
            sb.Append(bits[i] ? 1 : 0);
        }
        return sb.ToString();
    }
}
