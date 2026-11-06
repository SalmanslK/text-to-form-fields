using System.Text.Json;
using System.Text.Json.Serialization;
using TextToFields;

// Reads one field per line until a blank line or the end of the input, so a file can be piped in too.
if (!Console.IsInputRedirected)
{
    Console.WriteLine("Describe the fields, one per line. Finish with a blank line.");
    Console.WriteLine("For example: Country dropdown [Pakistan, UAE]");
}

var lines = new List<string>();
while (Console.ReadLine() is { } line && (line.Trim().Length > 0 || Console.IsInputRedirected))
    lines.Add(line);

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};
Console.WriteLine(JsonSerializer.Serialize(FormFields.Generate(string.Join('\n', lines)), options));
