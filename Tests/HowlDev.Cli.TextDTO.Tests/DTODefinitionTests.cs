using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;

public class DTODefinitionTests {
    [Test]
    public async Task BasicFileAsClass() {
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
        await Assert.That(def.Namespace).IsEqualTo("HowlDev.Cli.Tests");
        await Assert.That(def.Name).IsEqualTo("IdAndTitleDTO");
        await Assert.That(def.Type).IsEqualTo("Class");
        await Assert.That(def.IgnoreWarnings).IsEqualTo(false);
        await Assert.That(def.Properties.Length).IsEqualTo(1);
        await Assert.That(def.Properties[0].Name).IsEqualTo("Name");
        await Assert.That(def.Properties[0].Type).IsEqualTo("string");
        await Assert.That(def.Properties[0].Default).IsEqualTo("Default Name");
        await Assert.That(def.Properties[0].Nullable).IsEqualTo(true);
        await Assert.That(def.EnumValues.Length).IsEqualTo(0);
    }

    [Test]
    public async Task BasicFileAsEnum() {
        string json = """
        {
            "namespace": "HowlDev.Cli.Tests",
            "name": "IdAndTitleDTO", 
            "type": "Enum", 
            "enumValues": [
                "one", "two", "three"
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        DTODefinition def = config.As<DTODefinition>();
        await Assert.That(def.Namespace).IsEqualTo("HowlDev.Cli.Tests");
        await Assert.That(def.Name).IsEqualTo("IdAndTitleDTO");
        await Assert.That(def.Type).IsEqualTo("Enum");
        await Assert.That(def.Properties.Length).IsEqualTo(0);
        await Assert.That(def.EnumValues.Length).IsEqualTo(3);
        await Assert.That(def.EnumValues[0]).IsEqualTo("one");
        await Assert.That(def.EnumValues[1]).IsEqualTo("two");
        await Assert.That(def.EnumValues[2]).IsEqualTo("three");
    }

    [Test]
    public async Task LargeFileDTODefinitionTest() {
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
                    "name": "Amount",
                    "type": "double", 
                    "default": "25.1"
                }
            ]
        }
        """;
        TextConfigFile config = TextConfigFile.ReadTextAs(FileTypes.JSON, json);
        DTODefinition def = config.As<DTODefinition>();
        await Assert.That(def.Namespace).IsEqualTo("ProjectTracker.Classes");
        await Assert.That(def.Name).IsEqualTo("IdAndTitleDTO");
        await Assert.That(def.Type).IsEqualTo("Class");
        await Assert.That(def.IgnoreWarnings).IsEqualTo(true);
        await Assert.That(def.Properties.Length).IsEqualTo(4);
    }
}