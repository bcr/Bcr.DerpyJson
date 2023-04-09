using System.Text;

namespace Bcr.DerpyJson.Test;

public class ParserTest
{
    class DummyClass
    {
        public int foo { get; set; }
        public string? bar { get; set; }
        public decimal baz { get; set; }
        public DummyClass? blort { get; set; }
        public long fooo { get; set; }
    }

    [Fact]
    public void Parse_ReturnsObject()
    {
        object? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{}"));

        Assert.IsType<DummyClass>(o);
    }

    [Fact]
    public void Parse_ObjectInteger()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"foo\":420}"));

        Assert.Equal(420, o?.foo);
    }

    [Fact]
    public void Parse_ObjectString()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"bar\":\"baz\"}"));

        Assert.Equal("baz", o?.bar);
    }

    [Fact]
    public void Parse_ObjectMultipleValues()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"bar\":\"baz\",\"foo\":420}"));

        Assert.Equal("baz", o?.bar);
        Assert.Equal(420, o?.foo);
    }

    [Fact]
    public void Parse_ObjectDecimal()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"baz\":420.69}"));

        Assert.Equal((decimal) 420.69, o?.baz);
    }

    [Fact]
    public void Parse_ObjectNegativeDecimal()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"baz\":-420.69}"));

        Assert.Equal((decimal) -420.69, o?.baz);
    }

    [Fact]
    public void Parse_NestedObjects()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"bar\":\"baz\",\"foo\":420,\"blort\":{\"bar\":\"bay\",\"foo\":421}}"));

        Assert.Equal("bay", o?.blort?.bar);
        Assert.Equal(421, o?.blort?.foo);
    }

    [Fact]
    public void Parse_ObjectLong()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"fooo\":420}"));

        Assert.Equal(420, o?.fooo);
    }

    [Fact]
    public void Parse_Long()
    {
        long actual = Parser.Parse<long>(Encoding.UTF8.GetBytes("420"));

        Assert.Equal(420, actual);
    }

    [Fact]
    public void Parse_BooleanTrue()
    {
        bool actual = Parser.Parse<bool>(Encoding.UTF8.GetBytes("true"));

        Assert.True(actual);
    }

    [Fact]
    public void Parse_BooleanFalse()
    {
        bool actual = Parser.Parse<bool>(Encoding.UTF8.GetBytes("false"));

        Assert.False(actual);
    }

    [Fact]
    public void Parse_StringWithEscapes()
    {
        // In the string below, I am using a verbatim string, and then in the
        // assertion I'm using a normal C# string. Note the escaping
        // conventions are different between the two. Also note the escaping
        // conventions are different in a JSON string. You have been warned.
        string? actual = Parser.Parse<string>(Encoding.UTF8.GetBytes(@"""\""\\\/\b\f\n\r\t\u20ac"""));

        Assert.Equal("\"\\/\b\f\n\r\t\u20ac", actual);
    }
}
