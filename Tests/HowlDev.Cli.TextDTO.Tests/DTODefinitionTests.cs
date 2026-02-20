using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO.Tests;

public class DTODefinitionTests {
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
        await Assert.That(def.Namespace).IsEqualTo("HowlDev.Cli.Tests");
        await Assert.That(def.Name).IsEqualTo("IdAndTitleDTO");
        await Assert.That(def.Type).IsEqualTo("Class");
        await Assert.That(def.Properties.Length).IsEqualTo(1);
        await Assert.That(def.Properties[0].Name).IsEqualTo("Name");
        await Assert.That(def.Properties[0].Type).IsEqualTo("string");
        await Assert.That(def.Properties[0].Default).IsEqualTo("Default Name");
        await Assert.That(def.Properties[0].Nullable).IsEqualTo(true);
    }
}