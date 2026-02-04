using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;

public class CSharpFileTests {
    [Test]
    public async Task SimpleFileNoNamespaceAnd1Property() {
        string json = """
        {
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "properties": [
                {
                    "name": "Id",
                    "type": "int"
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToCSharpFile(config);
        await Assert.That(result).IsEqualTo("""
        public class IdAndTitleDTO {
            public int Id { get; set; } 
        }

        """);
    }

    [Test]
    public async Task SimpleFileNoNamespaceAnd1FullProperty() {
        string json = """
        {
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "properties": [
                {
                    "name": "Name",
                    "type": "string",
                    "default": "Default Name",
                    "nullable": true
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToCSharpFile(config);
        await Assert.That(result).IsEqualTo("""
        public class IdAndTitleDTO {
            public string? Name { get; set; } = "Default Name";
        }

        """);
    }

    [Test]
    public async Task SimpleFileWithNamespaceAnd1FullProperty() {
        string json = """
        {
            "namespace": "HowlDev.Cli.Tests",
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "properties": [
                {
                    "name": "Name",
                    "type": "string",
                    "default": "Default Name",
                    "nullable": true
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToCSharpFile(config);
        await Assert.That(result).IsEqualTo("""
        namespace HowlDev.Cli.Tests;
        
        public class IdAndTitleDTO {
            public string? Name { get; set; } = "Default Name";
        }

        """);
    }

    [Test]
    public async Task FullFileWithNamespaceAndFullProperties() {
        string json = """
        {
            "namespace": "ProjectTracker.Classes",
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "ignoreWarnings": true,
            "properties": [
                {
                    "name": "Id",
                    "type": "int", 
                    "default": 23
                },
                {
                    "name": "Sample",
                    "type": "string", 
                    "default": "Unknown"
                },
                {
                    "name": "Indexes",
                    "type": "Calculator[]",
                    "default": "new()"
                },
                {
                    "name": "Boolean",
                    "type": "bool", 
                    "default": "true"
                },
                {
                    "name": "Amount",
                    "type": "double", 
                    "default": "25.1"
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToCSharpFile(config);
        await Assert.That(result).IsEqualTo("""
        #pragma warning disable
        namespace ProjectTracker.Classes;
        
        public class IdAndTitleDTO {
            public int Id { get; set; } = 23;
            public string Sample { get; set; } = "Unknown";
            public Calculator[] Indexes { get; set; } = new();
            public bool Boolean { get; set; } = true;
            public double Amount { get; set; } = 25.1;
        }

        """);
    }
}
