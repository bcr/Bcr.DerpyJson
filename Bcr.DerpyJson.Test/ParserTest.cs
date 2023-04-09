using System.Text;

namespace Bcr.DerpyJson.Test;

public class UnitTest1
{
    class DummyClass
    {
        public int foo { get; set; }
        public string? bar { get; set; }
        public Decimal baz { get; set; }
    }

    [Fact]
    public void Parse_ReturnsObject()
    {
        object o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{}"));

        Assert.IsType<DummyClass>(o);
    }

    [Fact]
    public void Parse_ObjectInteger()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"foo\":420}"));

        Assert.Equal(420, o.foo);
    }

    [Fact]
    public void Parse_ObjectString()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"bar\":\"baz\"}"));

        Assert.Equal("baz", o.bar);
    }

    [Fact]
    public void Parse_ObjectMultipleValues()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"bar\":\"baz\",\"foo\":420}"));

        Assert.Equal("baz", o.bar);
        Assert.Equal(420, o.foo);
    }

    [Fact]
    public void Parse_ObjectDecimal()
    {
        DummyClass? o = Parser.Parse<DummyClass>(Encoding.UTF8.GetBytes("{\"baz\":420.69}"));

        Assert.Equal((decimal) 420.69, o.baz);
    }
}
