using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;


public class CSharpClassTests {
    ICrossFileReference n = new CrossFileReference();

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
        string result = ConfigToText.ToCSharpFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        namespace HowlDev.Cli.Tests;
        
        public class IdAndTitleDTO {
            public string? Name { get; set; } = "Default Name";
        }

        """);
    }

    [Test]
    public async Task SimpleFileWithNamespaceReferencingAnotherFile() {
        string json = """
        {
            "namespace": "HowlDev.Cli.Tests",
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "properties": [
                {
                    "name": "Name",
                    "type": "MyClass",
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        CrossFileReference fileReference = new();
        fileReference.AddKey("MyClass", "MyClass", "HowlDev.Cli.Tests.Classes");
        string result = ConfigToText.ToCSharpFile(config, fileReference);
        await TestHelpers.NormalStringsAreEqual(result, """
        using HowlDev.Cli.Tests.Classes;
        namespace HowlDev.Cli.Tests;
        
        public class IdAndTitleDTO {
            public MyClass Name { get; set; }
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
        string result = ConfigToText.ToCSharpFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
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
public class CSharpEnumTests {
    ICrossFileReference n = new CrossFileReference();

    [Test]
    public async Task Enum1() {
        string json = """
        {
            "namespace": "ProjectTracker.Classes",
            "name": "Numbers", 
            "type": "Enum", 
            "properties": [
                "One", "Two", "Three", 
                "Four"
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToCSharpFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        namespace ProjectTracker.Classes;

        public enum Numbers {
            One,
            Two,
            Three,
            Four
        }
        
        """);
    }

    [Test]
    public async Task Enum2() {
        string json = """
        {
            "name": "Numbers", 
            "namespace": "HowlDev.Cli.Tests", 
            "type": "Enum", 
            "properties": [
                "One", "Two", "Three"
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToCSharpFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        namespace HowlDev.Cli.Tests;
        
        public enum Numbers {
            One,
            Two,
            Three
        }
        
        """);
    }
}
