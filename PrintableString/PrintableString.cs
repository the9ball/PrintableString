using System.Diagnostics.CodeAnalysis;

namespace PrintableString;

/// <summary>
/// byte列を出力可能かつ複合可能な文字列に変換する
/// </summary>
public class PrintableStringConverter
{
    /// <summary>
    /// 出力可能なASCII文字一覧
    /// </summary>
    public const string AsciiChars = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

    /// <summary>
    /// 出力可能な文字一覧
    /// </summary>
    /// <remarks>
    /// 一覧のうち、2のべき乗個に収まる範囲のみ使用される
    /// </remarks>
    public char[] Chars
    {
        get
        {
            if (_chars is null) Initialize();
            return _chars;
        }
    }
    private char[]? _chars;

    /// <summary>
    /// <see cref="Chars"/>のインデックスのうち使う範囲のbit数
    /// </summary>
    private int _charsBits;

    /// <summary>
    /// 出力可能な文字一覧を取得する
    /// </summary>
    /// <remarks>
    /// 一覧のうち、2のべき乗個に収まる範囲のみ使用される
    /// </remarks>
    protected virtual char[] GetChars() => AsciiChars.ToArray();

    /// <summary>
    /// 初期化
    /// </summary>
    [MemberNotNull(nameof(_chars))]
    private void Initialize()
    {
        var chars = _chars = GetChars();
        var length = chars.Length;
        if (length >= int.MaxValue) throw new NotSupportedException($"{nameof(GetChars)}().Length <= {int.MaxValue}");

        var log2 = (int)Math.Log2(length);
        _charsBits = log2;
    }

    /// <summary>
    /// 出力可能な文字列に変換する
    /// </summary>
    public static string ToPrintableString(ReadOnlySpan<byte> bytes, char[] chars, int useBits)
    {
        var useBytes = (useBits + 8 - 1) / 8;
        var mask = (1 << useBits) - 1;

        var bits = 8 * bytes.Length;
        var length = (bits + useBits - 1) / useBits;
        Span<char> buffer = stackalloc char[length];

        int sourceIndex = 0;
        int bit = useBits;
        for (int destinationIndex = 0; destinationIndex < length; destinationIndex++)
        {
            int b = 0;

            // 必要なbit数を超えるまで詰める
            int setBytes = 0;
            for (; setBytes * 8 < bit; setBytes++)
            {
                var i = sourceIndex + setBytes;
                var s = i < bytes.Length ? bytes[i] : 0;
                b = (b << 8) | s;
            }

            // 詰めすぎたbitを消す
            b >>= setBytes * 8 - bit;
            b &= mask;

            // 文字を取得
            buffer[destinationIndex] = chars[b];

            // 8bit以上使ったなら次のbyteを読みに行く
            if (bit >= 8)
            {
                bit -= 8;
                sourceIndex++;
            }

            // 次に必要なbit数
            bit += useBits;
        }

        return new(buffer);
    }

    /// <inheritdoc cref="ToPrintableString"/>
    public string ToPrintableString(ReadOnlySpan<byte> bytes) => ToPrintableString(bytes, Chars, _charsBits);

    /// <inheritdoc cref="ToPrintableString"/>
    public string ToPrintableString(byte[] bytes) => ToPrintableString(bytes.AsSpan());

    /// <summary>
    /// 変換した文字列からデータを復元する
    /// </summary>
    public static byte[] FromPrintableString(string data, char[] chars, int useBits)
    {
        // 1文字毎にuseBits数のbitを使っている
        // 端数は切り捨てでいいはず
        var length = useBits * data.Length / 8;
        var buffer = new byte[length];

        var useChars = chars.AsSpan(0, 1 << useBits);
        int b = 0;
        int bits = 0;
        var destinationIndex = 0;
        foreach (var source in data)
        {
            var index = useChars.IndexOf(source);
            if (index < 0) throw new InvalidDataException($"Unknown char : {source}");

            b = b << useBits | index;
            bits += useBits;

            if (bits >= 8)
            {
                var d = (b >> (bits - 8)) & 0xFF;
                buffer[destinationIndex++] = (byte)d;
                bits -= 8;
            }
        }

        return buffer;
    }

    /// <inheritdoc cref="FromPrintableString"/>
    public byte[] FromPrintableString(string data) => FromPrintableString(data, Chars, _charsBits);
}

