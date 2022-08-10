namespace PrintableString.Test;

public class PrintableStringTest_Hex
{
    class Converter : PrintableStringConverter
    {
        const string _chars = "0123456789ABCDEF";

        /// <inheritdoc/>
        protected override char[] GetChars() => _chars.ToArray();
    }

    [Fact]
    public void ToPrintableString()
    {
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
        var expect = "123456789ABCDEF0";

        var c = new Converter();
        var actual = c.ToPrintableString(data);

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void FromPrintableString()
    {
        var data = "123456789ABCDEF0";
        var expect = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

        var c = new Converter();
        var actual = c.FromPrintableString(data);

        Assert.Equal(expect, actual);
    }
}