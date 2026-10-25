using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Tokenize;
using opennlp.tools.tokenize;
using opennlp.tools.postag;
using opennlp.tools.chunker;
using opennlp.tools.namefind;

namespace TextToFields
{
    public class FormField
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Options { get; set; }
    }

    public class FormFields
    {
        private static TokenizerModel TokenModel { get; set; }
        private static EnglishMaximumEntropyPosTagger PosModel { get; set; }
        private static EnglishTreebankParser Parser { get; set; }
        private static EnglishNameFinder NameFinder { get; set; }

        static FormFields()
        {
            TokenModel = new TokenizerModel(typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-token.bin"));
            PosModel = new EnglishMaximumEntropyPosTagger(typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-pos-maxent.bin"));
            Parser = new EnglishTreebankParser(typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-parser-chunking.bin"));
            NameFinder = new EnglishNameFinder(typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-ner-person.bin"),
                typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-ner-location.bin"),
                typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-ner-organization.bin"),
                typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-ner-date.bin"),
                typeof(FormFields).Assembly.GetManifestResourceStream("OpenNLP.en-ner-time.bin"));
        }

        public static List<FormField> GenerateFormFields(string description)
        {
            List<FormField> fields = new List<FormField>();

            var tokens = TokenizeSentence(description);
            var tags = PosTagTokens(tokens);
            var chunks = ChunkTokens(tokens, tags);
            var names = FindNames(chunks);

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                string tag = tags[i];

                if (tag == "NN" || tag == "NNS")
                {
                    string fieldName = token.ToLower();
                    string fieldType = "text";

                    if (i < tokens.Length - 1 && (tokens[i + 1].ToLower() == "field" || tokens[i + 1].ToLower() == "input"))
                    {
                        i++;
                    }

                    if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "number")
                    {
                        fieldType = "number";
                        i++;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "dropdown")
                    {
                        fieldType = "dropdown";
                        i++;

                        List<string> options = new List<string>();
                        int j = i + 1;
                        while (j < tokens.Length && tags[j] == "NN")
                        {
                            options.Add(tokens[j]);
                            j++;
                        }

                        fields.Add(new FormField { Label = fieldName, Name = fieldName, Type = fieldType, Options = options });
                        i = j - 1;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "select")
                    {
                        fieldType = "select";
                        i++;

                        List<string> options = new List<string>();
                        int j = i + 1;
                        while (j < tokens.Length && tags[j] == "NN")
                        {
                            options.Add(tokens[j]);
                            j++;
                        }

                        fields.Add(new FormField { Label = fieldName, Name = fieldName, Type = fieldType, Options = options });
                        i = j - 1;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "radio")
                    {
                        fieldType = "radio";
                        i++;

                        List<string> options = new List<string>();
                        int j = i + 1;
                        while (j < tokens.Length && tags[j] == "NN")
                        {
                            options.Add(tokens[j]);
                            j++;
                        }

                        fields.Add(new FormField { Label = fieldName, Name = fieldName, Type = fieldType, Options = options });
                        i = j - 1;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "date")
                    {
                        fieldType = "date";
                        i++;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "datetime")
                    {
                        fieldType = "datetime";
                        i++;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "phone")
                    {
                        fieldType = "phone";
                        i++;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "email")
                    {
                        fieldType = "email";
                        i++;
                    }
                    else if (i < tokens.Length - 1 && tokens[i + 1].ToLower() == "password")
                    {
                        fieldType = "password";
                        i++;
                    }

                    fields.Add(new FormField { Label = fieldName, Name = fieldName, Type = fieldType });
                }
            }

            return fields;
        }

        private static string[] TokenizeSentence(string sentence)
        {
            var tokenizer = new TokenizerME(TokenModel);
            return tokenizer.tokenize(sentence);
        }

        private static string[] PosTagTokens(string[] tokens)
        {
            var posTagger = new POSTaggerME(PosModel);
            return posTagger.tag(tokens);
        }

        private static string[] ChunkTokens(string[] tokens, string[] tags)
        {
            var chunks = new List<string>();

            var chunker = new ChunkerME(Parser);
            var chunksArr = chunker.chunk(tokens, tags);

            foreach (var chunk in chunksArr)
            {
                if (chunk.StartsWith("NP"))
                {
                    chunks.AddRange(chunk.ToString().Split(' '));
                }
            }

            return chunks.ToArray();
        }

        private static string[] FindNames(string[] chunks)
        {
            var names = new List<string>();

            var nameFinder = new NameFinderME(NameFinder.PersonModel, NameFinder.LocationModel, NameFinder.OrganizationModel, NameFinder.DateModel, NameFinder.TimeModel);
            var nameSpans = nameFinder.find(chunks);

            foreach (var span in nameSpans)
            {
                for (int i = span.Start; i < span.End; i++)
                {
                    names.Add(chunks[i]);
                }
            }

            return names.ToArray();
        }


    }
}
