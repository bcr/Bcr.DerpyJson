using System.Text;

namespace Bcr.DerpyJson.Test;

public class ParserTest
{
    class DummyClass
    {
        public int foo { get; set; }
        public string? bar { get; set; }
        public Decimal baz { get; set; }
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
}
