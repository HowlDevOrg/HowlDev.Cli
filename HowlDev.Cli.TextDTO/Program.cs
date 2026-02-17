using HowlDev.IO.Text.ConfigFile;
using System.CommandLine;

namespace HowlDev.Cli.TextDTO;

class Program {
    static int Main(string[] args) {
        RootCommand rootCommand = new("Parameter binding example") {
            new Argument<string>("file/folder") {
            Description = "Input file/folder to read for the type definitions.",
            Arity = ArgumentArity.ExactlyOne
            }
        };
        ParseResult result = rootCommand.Parse(args);
        string? folder = result.GetValue<string>("file/folder");
        if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentException("File or folder not provided. The first argument is the input file or folder.");

        ConfigFileCollector collector;

        if (Directory.Exists(folder)) {
            collector = new([.. Directory.EnumerateFiles(folder)]);
        } else {
            collector = new([folder]);
        }

        if (result.UnmatchedTokens.Count % 2 != 0) {
            throw new ArgumentException("Remaining tokens are unmatched. Make sure you have an even number of types and export paths.");
        }

        List<string> availableTypes = ["cs", "ts", "ts-z"];
        for (int i = 0; i < result.UnmatchedTokens.Count; i += 2) {
            string type = result.UnmatchedTokens[i];
            if (!availableTypes.Contains(type)) {
                throw new ArgumentException($"Type {type} is not supported. Supported types: {string.Join(", ", availableTypes)}.");
            }
            string outputFolder = result.UnmatchedTokens[i + 1];
            if (outputFolder.EndsWith('/')) outputFolder = outputFolder[0..^1];

            Directory.CreateDirectory(outputFolder);

            CrossFileReference fileLookup = new();

            foreach (string path in collector) {
                string file = Path.GetFileName(path);
                string fileWithoutExtension = Path.GetFileNameWithoutExtension(path);
                TextConfigFile config = collector.GetFile(file);
                fileLookup.AddKey(config["name"].ToString()!, fileWithoutExtension, config["namespace"].ToString()!);
            }

            foreach (string path in collector) {
                string file = Path.GetFileName(path);
                string fileWithoutExtension = Path.GetFileNameWithoutExtension(path);
                TextConfigFile config = collector.GetFile(file);
                string text = string.Empty;
                switch (type) {
                    case "cs":
                        text = ConfigToText.ToCSharpFile(config, fileLookup);
                        break;
                    case "ts":
                        text = ConfigToText.ToTSFile(config, fileLookup);
                        break;
                    case "ts-z":
                        text = ConfigToText.ToTSZodFile(config);
                        break;
                }
                string newPath = outputFolder + "/" + fileWithoutExtension + "." + type.Split('-')[0];
                Console.WriteLine($"Writing file to: {newPath}");
                File.WriteAllText(newPath, text);
            }
        }

        return 1;
    }
}
