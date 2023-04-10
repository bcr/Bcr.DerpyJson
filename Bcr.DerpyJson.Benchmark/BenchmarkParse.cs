using Bcr.DerpyJson;
using BenchmarkDotNet.Attributes;
using System.Text;
using System.Text.Json;
using TinyJson;

[MemoryDiagnoser]
public class BenchmarkParse
{
    public record CardTimeResponse(int minutes, Decimal lat, Decimal lon, string area, string country, string zone, long time);

    private const string json = @"{""minutes"":-420,""lat"":123.994737500000022,""lon"":-111.01234500002,""area"":""Salem"",""country"":""US"",""zone"":""PDT,America/Los_Angeles"",""time"":1681068455}";

    [Benchmark]
    public CardTimeResponse MicrosoftSerializer() => JsonSerializer.Deserialize<CardTimeResponse>(json);

    [Benchmark]
    public CardTimeResponse DerpyParser() => Parser.Parse<CardTimeResponse>(Encoding.UTF8.GetBytes(json));

    [Benchmark]
    public CardTimeResponse TinyJsonParser() => json.FromJson<CardTimeResponse>();

    [Benchmark]
    public CardTimeResponse NewtonsoftJsonParser() => Newtonsoft.Json.JsonConvert.DeserializeObject<CardTimeResponse>(json);
}
