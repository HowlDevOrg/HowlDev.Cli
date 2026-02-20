using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;

public class TSClassTests {
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
        DTODefinition def = config.As<DTODefinition>();
        string result = ConfigToText.ToTSFile(def, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        export type IdAndTitleDTO = {
            Name: string | undefined
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
        DTODefinition def = config.As<DTODefinition>();
        CrossFileReference fileReference = new();
        fileReference.AddKey("MyClass", "ClassFile", "HowlDev.Cli.Tests.Classes");
        string result = ConfigToText.ToTSFile(def, fileReference);
        await TestHelpers.NormalStringsAreEqual(result, """
        import type { MyClass } from './ClassFile.ts';
        export type IdAndTitleDTO = {
            Name: MyClass
        }

        """);
    }

    [Test]
    public async Task SimpleFileWithArrayAndNamespaceReferencingAnotherFile() {
        string json = """
        {
            "namespace": "HowlDev.Cli.Tests",
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "properties": [
                {
                    "name": "Name",
                    "type": "MyClass[]",
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        DTODefinition def = config.As<DTODefinition>();
        CrossFileReference fileReference = new();
        fileReference.AddKey("MyClass", "ClassFile", "HowlDev.Cli.Tests.Classes");
        string result = ConfigToText.ToTSFile(def, fileReference);
        await TestHelpers.NormalStringsAreEqual(result, """
        import type { MyClass } from './ClassFile.ts';
        export type IdAndTitleDTO = {
            Name: MyClass[]
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
                    "default": "Unknown", 
                    "nullable": true
                },
                {
                    "name": "Bool",
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
        DTODefinition def = config.As<DTODefinition>();
        string result = ConfigToText.ToTSFile(def, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        /* eslint-disable */
        export type IdAndTitleDTO = {
            Id: number 
            Sample: string | undefined
            Bool: boolean 
            Amount: number 
        }
        /* eslint-enable */

        """);
    }
}
public class TSEnumTests {
    ICrossFileReference n = new CrossFileReference();

    [Test]
    public async Task Enum1() {
        string json = """
        {
            "name": "Numbers", 
            "type": "Enum", 
            "enumValues": [
                "One", "Two", "Three", 
                "Four"
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        DTODefinition def = config.As<DTODefinition>();
        string result = ConfigToText.ToTSFile(def, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        export type Numbers = "One" | "Two" | "Three" | "Four";

        """);
    }

    [Test]
    public async Task Enum2() {
        string json = """
        {
            "name": "Numbers", 
            "namespace": "HowlDev.Cli.Tests", 
            "type": "Enum", 
            "enumValues": [
                "One", "Two", "Three"
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        DTODefinition def = config.As<DTODefinition>();
        string result = ConfigToText.ToTSFile(def, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        export type Numbers = "One" | "Two" | "Three";

        """);
    }
}
