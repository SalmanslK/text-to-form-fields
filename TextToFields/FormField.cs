using System.Text.RegularExpressions;

namespace TextToFields
{
    public class FormField
    {
        public string Label { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "text";
        public List<string>? Options { get; set; }
    }

    public static class FormFields
    {
        // Word in the description -> field type. The first match in this order wins, so "Phone number" is a
        // phone field rather than a number, and "datetime" isn't read as "date".
        private static readonly (string Word, string Type)[] TypeWords =
        {
            ("datetime", "datetime"), ("password", "password"), ("email", "email"), ("phone", "tel"), ("date", "date"),
            ("dropdown", "select"), ("select", "select"), ("radio", "radio"), ("checkbox", "checkbox"),
            ("number", "number"), ("text", "text"),
        };

        // These only name the kind of control, so they aren't part of the label: "Colour dropdown" -> "Colour"
        private static readonly HashSet<string> ControlWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "dropdown", "select", "radio", "checkbox", "text", "field", "input",
        };

        private static readonly HashSet<string> TypesWithOptions = new() { "select", "radio", "checkbox" };

        /// <summary>Turns a description with one field per line into field definitions; blank lines are skipped.</summary>
        public static List<FormField> Generate(string description) =>
            description.Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .Select(ParseLine)
                .Where(field => field.Name.Length > 0)
                .ToList();

        private static FormField ParseLine(string line)
        {
            var options = Regex.Matches(line, @"\[(.*?)\]")
                .SelectMany(match => match.Groups[1].Value.Split(','))
                .Select(option => option.Trim())
                .Where(option => option.Length > 0)
                .ToList();
            var text = Regex.Replace(line, @"\[.*?\]", " ").Trim();

            // "Age: number" names the type explicitly
            var parts = text.Split(':', 2);
            var label = parts.Length == 2 ? parts[0].Trim() : text;
            var type = FindType(parts.Length == 2 ? parts[1] : text);

            // Otherwise drop trailing control words, keeping the label if that's all there is: "Text" -> "Text"
            var words = Regex.Split(label, @"\s+").ToList();
            while (words.Count > 1 && ControlWords.Contains(words[^1]))
                words.RemoveAt(words.Count - 1);
            label = string.Join(' ', words).Trim();

            return new FormField
            {
                Label = label,
                Name = Regex.Replace(label.ToLowerInvariant(), @"[^a-z0-9]+", "_").Trim('_'),
                Type = type,
                Options = TypesWithOptions.Contains(type) && options.Count > 0 ? options : null,
            };
        }

        private static string FindType(string text)
        {
            foreach (var (word, type) in TypeWords)
            {
                if (Regex.IsMatch(text, $@"\b{word}\b", RegexOptions.IgnoreCase))
                    return type;
            }
            return "text";
        }
    }
}
