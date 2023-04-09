using Bcr.DerpyJson;
using BenchmarkDotNet.Attributes;
using System.Text;
using System.Text.Json;

[MemoryDiagnoser]
public class BenchmarkParse
{
    public class CardTimeResponse
    {
        public int minutes { get; set; }
        public Decimal lat { get; set; }
        public Decimal lon { get; set; }
        public string area { get; set; }
        public string country { get; set; }
        public string zone { get; set; }
        public long time { get; set; }
    }

    private const string json = @"{""minutes"":-420,""lat"":123.994737500000022,""lon"":-111.01234500002,""area"":""Salem"",""country"":""US"",""zone"":""PDT,America/Los_Angeles"",""time"":1681068455}";

    [Benchmark]
    public CardTimeResponse MicrosoftSerializer() => JsonSerializer.Deserialize<CardTimeResponse>(json);

    [Benchmark]
    public CardTimeResponse DerpyParser() => Parser.Parse<CardTimeResponse>(Encoding.UTF8.GetBytes(json));
}
