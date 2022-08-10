namespace PrintableString.Test;

public class PrintableStringTest_Decimal
{
    class Converter : PrintableStringConverter
    {
        const string _chars = "0123456789";

        /// <inheritdoc/>
        protected override char[] GetChars() => _chars.ToArray();
    }

    [Fact]
    public void ToPrintableString()
    {
        // 0-7までが有効な数値なので3bit毎にデータが入るはず
        var data = new byte[] { 0b001_010_01, 0b1_100_101_1, 0b10_111_000 };
        var expect = "12345670";

        var c = new Converter();
        var actual = c.ToPrintableString(data);

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void FromPrintableString()
    {
        var data = "12345670";
        var expect = new byte[] { 0b001_010_01, 0b1_100_101_1, 0b10_111_000 };

        var c = new Converter();
        var actual = c.FromPrintableString(data);

        Assert.Equal(expect, actual);
    }
}