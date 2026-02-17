using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;

public class TSZodClassTests {
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
        string result = ConfigToText.ToTSZodFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        import z from "zod"

        export const IdAndTitleDTOSchema = z.object({
            Name: z.string().default("Default Name").nullable(),
        });

        export type IdAndTitleDTOType = z.infer<typeof IdAndTitleDTOSchema>;

        """);
    }

    [Test]
    public async Task SimpleFileWithNamespaceAndArrayProperty() {
        string json = """
        {
            "namespace": "HowlDev.Cli.Tests",
            "name": "IdAndTitleDTO", 
            "type": "Class", 
            "properties": [
                {
                    "name": "Name",
                    "type": "string[]"
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToTSZodFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        import z from "zod"

        export const IdAndTitleDTOSchema = z.object({
            Name: z.string().array(),
        });

        export type IdAndTitleDTOType = z.infer<typeof IdAndTitleDTOSchema>;
        
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
        fileReference.AddKey("MyClass", "ClassFile", "HowlDev.Cli.Tests.Classes");
        string result = ConfigToText.ToTSZodFile(config, fileReference);
        await TestHelpers.NormalStringsAreEqual(result, """
        import { MyClassSchema } from "./ClassFile.ts";
        import z from "zod"

        export const IdAndTitleDTOSchema = z.object({
            Name: MyClassSchema,
        });

        export type IdAndTitleDTOType = z.infer<typeof IdAndTitleDTOSchema>;

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
        string result = ConfigToText.ToTSZodFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        /* eslint-disable */
        import z from "zod"

        export const IdAndTitleDTOSchema = z.object({
            Id: z.number().default(23),
            Sample: z.string().default("Unknown").nullable(),
            Bool: z.boolean().default(true),
            Amount: z.number().default(25.1),
        });

        export type IdAndTitleDTOType = z.infer<typeof IdAndTitleDTOSchema>;
        /* eslint-enable */

        """);
    }
}
public class TSZodEnumTests {
    ICrossFileReference n = new CrossFileReference();

    [Test]
    public async Task Enum1() {
        string json = """
        {
            "name": "Numbers", 
            "type": "Enum", 
            "properties": [
                "One", "Two", "Three", 
                "Four"
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToTSZodFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        import z from "zod"

        export const NumbersSchema = z.enum(["One", "Two", "Three", "Four"]);

        export type NumbersType = z.infer<typeof NumbersSchema>;

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
        string result = ConfigToText.ToTSZodFile(config, n);
        await TestHelpers.NormalStringsAreEqual(result, """
        import z from "zod"

        export const NumbersSchema = z.enum(["One", "Two", "Three"]);   

        export type NumbersType = z.infer<typeof NumbersSchema>;
        """);
    }
}
