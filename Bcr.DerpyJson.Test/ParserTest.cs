using System.Text;

namespace Bcr.DerpyJson.Test;

public class UnitTest1
{
    class DummyClass
    {
        public int foo { get; set; }
        public string? bar { get; set; }
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
}
