using System.Text.Json;
using System.Text.Json.Serialization;
using MissionTelemetry.Core.Models;

public sealed class JsonDictionaryLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    static JsonDictionaryLoader()
    {
        Options.Converters.Add(new JsonStringEnumConverter()); // <-- wichtig
    }

    public TelemetryDictionary LoadFromFile(string path)
        => LoadFromString(File.ReadAllText(path));

    public TelemetryDictionary LoadFromString(string json)
        => JsonSerializer.Deserialize<TelemetryDictionary>(json, Options) ?? new TelemetryDictionary();
}

