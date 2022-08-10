namespace PrintableString.Test;

public class PrintableStringTest
{
    [Fact]
    public void ToPrintableString()
    {
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
        var expect = @"$C16>)J\WO!";

        var c = new PrintableStringConverter();
        var actual = c.ToPrintableString(data);

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void FromPrintableString()
    {
        var data = @"$C16>)J\WO!";
        var expect = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };

        var c = new PrintableStringConverter();
        var actual = c.FromPrintableString(data);

        Assert.Equal(expect, actual);
    }
}