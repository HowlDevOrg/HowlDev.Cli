using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;

public class TSZodFileTests {
    [Test]
    public async Task SimpleFileNoNamespaceAnd1Property() {
        string json = """
        {
            "name": "IdAndTitleDTO", 
            "properties": [
                {
                    "name": "Id",
                    "type": "int"
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToTSZodFile(config);
        await Assert.That(result).IsEqualTo("""
        import z from "zod"

        export const IdAndTitleDTOSchema = z.object({
            Id: z.number(),
        });

        export type IdAndTitleDTOType = z.infer<typeof IdAndTitleDTOSchema>;

        """);
    }

    [Test]
    public async Task SimpleFileNoNamespaceAnd1FullProperty() {
        string json = """
        {
            "name": "SomethingElse", 
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
        string result = ConfigToText.ToTSZodFile(config);
        await Assert.That(result).IsEqualTo("""
        import z from "zod"

        export const SomethingElseSchema = z.object({
            Name: z.string().default("Default Name").nullable(),
        });

        export type SomethingElseType = z.infer<typeof SomethingElseSchema>;

        """);
    }

    [Test]
    public async Task SimpleFileWithNamespaceAnd1FullProperty() {
        string json = """
        {
            "namespace": "HowlDev.Cli.Tests",
            "name": "IdAndTitleDTO", 
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
        string result = ConfigToText.ToTSZodFile(config);
        await Assert.That(result).IsEqualTo("""
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
            "properties": [
                {
                    "name": "Name",
                    "type": "string[]"
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        string result = ConfigToText.ToTSZodFile(config);
        await Assert.That(result).IsEqualTo("""
        import z from "zod"

        export const IdAndTitleDTOSchema = z.object({
            Name: z.string().array(),
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
        string result = ConfigToText.ToTSZodFile(config);
        await Assert.That(result).IsEqualTo("""
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
