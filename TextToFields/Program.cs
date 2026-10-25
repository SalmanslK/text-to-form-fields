// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Welcome to Text to Field Converter!");

Console.WriteLine("Please provide a description of the fields required!");
var input = Console.ReadLine();

var output = generate_form_fields(input);
Console.WriteLine($"result: {output}");

//static dynamic generate_form_fields(string description)
//{
//    var fields = new List<dynamic>();

//    var inputTypes = new Dictionary<string, string>()
//    {
//        {"text", "text"},
//        {"number", "number"},
//        {"dropdown", "dropdown"},
//        {"radio", "radio"},
//        {"select", "select"},
//        {"date", "date"},
//        {"datetime", "datetime"},
//        {"phone", "tel"},
//        {"email", "email"},
//        {"password", "password"},
//        {"checkbox", "checkbox"}
//    };

//    var inputRegex = @"(text|number|dropdown|radio|select|date|datetime|phone|email|password|checkbox)";

//    var lines = description.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//    foreach (var line in lines)
//    {
//        var inputType = Regex.Match(line, inputRegex, RegexOptions.IgnoreCase);
//        if (inputType.Success)
//        {
//            var label = Regex.Replace(line, inputRegex, "").Trim();
//            var name = Regex.Replace(label.ToLower(), @"\s+", "_");
//            var type = inputTypes[inputType.Groups[1].Value.ToLower()];

//            var optionsRegex = @"(?:\[(.*?)\])";
//            var optionsMatches = Regex.Matches(label, optionsRegex);
//            if (optionsMatches.Count > 0)
//            {
//                var options = new List<dynamic>();
//                foreach (Match match in optionsMatches)
//                {
//                    var option = match.Groups[1].Value;
//                    if (inputType.Groups[1].Value.ToLower() == "select")
//                    {
//                        options.Add(new { label = option, value = option });
//                    }
//                    else if (inputType.Groups[1].Value.ToLower() == "radio" || inputType.Groups[1].Value.ToLower() == "checkbox")
//                    {
//                        options.Add(new { label = option, name = name, value = option });
//                    }
//                }
//                fields.Add(new { label = label, name = name, type = type, options = options });
//            }
//            else
//            {
//                fields.Add(new { label = label, name = name, type = type });
//            }
//        }
//    }

//    return new { fields = fields };
//}

//static List<Dictionary<string, object>> GenerateFormFields(string description)
//{
//    var fields = new List<Dictionary<string, object>>();

//    // Load the English tokenizer, tagger, parser, and entity recognizer.
//    var pipeline = new EnglishPipeline();

//    // Process the input text.
//    var doc = pipeline.Process(description);

//    // Iterate through the tokens and identify the form fields.
//    foreach (var token in doc)
//    {
//        var label = token.Text.Trim(':');
//        var name = label.ToLower().Replace(' ', '_');
//        var fieldType = GetFieldType(token);

//        if (fieldType != null)
//        {
//            var field = new Dictionary<string, object>();
//            field.Add("label", label);
//            field.Add("name", name);
//            field.Add("type", fieldType);

//            if (fieldType == "select" || fieldType == "checkbox" || fieldType == "radio")
//            {
//                var options = GetOptions(doc, token);
//                if (options != null && options.Count > 0)
//                {
//                    field.Add("options", options);
//                }
//            }

//            fields.Add(field);
//        }
//    }

//    return fields;
//}

//static string GetFieldType(Token token)
//{
//    if (token.Pos == PartOfSpeech.NOUN)
//    {
//        var nounText = token.Text.ToLower();

//        if (nounText.Contains("text"))
//        {
//            return "text";
//        }
//        else if (nounText.Contains("number"))
//        {
//            return "number";
//        }
//        else if (nounText.Contains("dropdown"))
//        {
//            return "dropdown";
//        }
//        else if (nounText.Contains("select"))
//        {
//            return "select";
//        }
//        else if (nounText.Contains("radio"))
//        {
//            return "radio";
//        }
//        else if (nounText.Contains("checkbox"))
//        {
//            return "checkbox";
//        }
//        else if (nounText.Contains("date"))
//        {
//            return "date";
//        }
//        else if (nounText.Contains("time"))
//        {
//            return "time";
//        }
//        else if (nounText.Contains("phone"))
//        {
//            return "phone";
//        }
//        else if (nounText.Contains("email"))
//        {
//            return "email";
//        }
//        else if (nounText.Contains("password"))
//        {
//            return "password";
//        }
//    }

//    return null;
//}

//static List<Dictionary<string, object>> GetOptions(Document doc, Token token)
//{
//    var options = new List<Dictionary<string, object>>();
//    var optionToken = token.Next;

//    while (optionToken != null && optionToken.Pos == PartOfSpeech.ADJ)
//    {
//        var optionLabel = optionToken.Text.Trim('[', ']', ',');
//        var optionValue = optionLabel.ToLower().Replace(' ', '_');

//        var option = new Dictionary<string, object>();
//        option.Add("label", optionLabel);
//        option.Add("value", optionValue);
//        option.Add("name", token.Text.ToLower().Replace(' ', '_'));

//        options.Add(option);

//        optionToken = optionToken.Next;
//    }

//    if (options.Count > 0)
//    {
//        return options;
//    }
//    else
//    {
//        return null;
//    }
//}