using System.Text;

namespace HowlDev.Cli.TextDTO;

/// <summary>
/// Provides methods for turning an IBaseConfigOption to a given filetype. 
/// </summary>
public static class ConfigToText {
    /// <summary>
    /// To a standard C# DTO file.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToCSharpFile(DTODefinition def, ICrossFileReference reference) {
        var output = new StringBuilder();

        if (def.IgnoreWarnings) {
            output.AppendLine("#pragma warning disable");
        }

        if (def.Type == "Class") {
            // Check for namespaces that we need to include from our properties
            var namespaces = def.Properties
                .Where(a => reference.ContainsKey(a.Type.Replace("[]", "")) &&
                    reference.GetReference(a.Type.Replace("[]", "")).csharpNamespace != def.Namespace)
                .Select(a => reference.GetReference(a.Type.Replace("[]", "")).csharpNamespace);

            foreach (var item in namespaces) {
                output.AppendLine($"using {item};");
            }
        }

        output.AppendLine($"namespace {def.Namespace};").AppendLine();

        switch (def.Type) {
            case "Class":
                CSharpClassBuilder(def, output);
                break;
            case "Enum":
                CSharpEnumBuilder(def, output);
                break;
        }

        return output.ToString();
    }

    /// <summary>
    /// Returns a JS Type file with full exports. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToTSFile(DTODefinition def, ICrossFileReference reference) {
        var output = new StringBuilder();

        if (def.IgnoreWarnings) {
            output.AppendLine("/* eslint-disable */");
        }

        if (def.Type == "Class") {
            // Check for imports that we need to include from our properties
            var fileImports = def.Properties
                .Where(a => reference.ContainsKey(a.Type.Replace("[]", "")))
                .Select(a => (a.Type, reference.GetReference(a.Type.Replace("[]", "")).file));

            foreach (var item in fileImports) {
                output.AppendLine($"import type {"{"} {item.Type.Replace("[]", "")} {"}"} from './{item.file}.ts';");
            }
        }

        switch (def.Type) {
            case "Class":
                JSClassBuilder(def, output);
                break;
            case "Enum":
                JSEnumBuilder(def, output);
                break;
        }

        if (def.IgnoreWarnings) {
            output.AppendLine("/* eslint-enable */");
        }

        return output.ToString();
    }

    /// <summary>
    /// Returns a JS Zod file with the zod import and one-level objects. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToTSZodFile(DTODefinition def, ICrossFileReference reference) {
        var output = new StringBuilder();

        if (def.IgnoreWarnings) {
            output.AppendLine("/* eslint-disable */");
        }

        if (def.Type == "Class") {
            // Check for imports that we need to include from our properties
            var fileImports = def.Properties
                .Where(a => reference.ContainsKey(a.Type.Replace("[]", "")))
                .Select(a => (a.Type, reference.GetReference(a.Type.Replace("[]", "")).file));

            foreach (var item in fileImports) {
                output.AppendLine($"import {"{"} {item.Type.Replace("[]", "")}Schema {"}"} from \"./{item.file}.ts\";");
            }
        }

        output.AppendLine("import z from \"zod\"").AppendLine();

        switch (def.Type) {
            case "Class":
                ZodClassBuilder(def, output, reference);
                break;
            case "Enum":
                ZodEnumBuilder(def, output);
                break;
        }

        if (def.IgnoreWarnings) {
            output.AppendLine("/* eslint-enable */");
        }

        return output.ToString();
    }

    private static string ConvertCSharpToJS(string val) {
        return val switch {
            "string" => "string",
            "bool" => "boolean",
            "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" or "float" or "double" or "decimal" => "number",
            "object" or "dynamic" => "any",
            "DateTime" or "DateTimeOffset" => "Date",
            "Guid" => "string",
            _ => val,
        };
    }

    private static void CSharpClassBuilder(DTODefinition def, StringBuilder output) {
        output.AppendLine($"public class {def.Name} {{");
        foreach (var option in def.Properties) {
            string name = option.Name;
            string type = option.Type;

            string d = "";
            if (option.Default != null) {
                string opt = option.Default;
                if (type == "string" && opt != "null") {
                    d = '"' + opt + '"';
                } else {
                    d = opt;
                }

                d = $" {{ get; set; }} = {d};";
            } else {
                d = " { get; set; } ";
            }

            if (option.Nullable) {
                type += "?";
            }

            output.AppendLine($"    public {type} {name}{d}");

        }

        output.AppendLine("}");
    }

    private static void CSharpEnumBuilder(DTODefinition def, StringBuilder output) {
        output.AppendLine($"public enum {def.Name} {{");
        output.Append("    " + string.Join(",\n    ", def.EnumValues)).AppendLine();
        output.AppendLine("}");
    }

    private static void JSClassBuilder(DTODefinition def, StringBuilder output) {
        output.AppendLine($"export type {def.Name} = {{");
        foreach (var option in def.Properties) {
            string name = option.Name;
            string type = option.Type;

            string nullable = "";
            string trailingSpace = " ";
            if (option.Nullable) {
                nullable = " | undefined";
                trailingSpace = "";
            }

            output.AppendLine($"    {name}: {ConvertCSharpToJS(type)}{nullable}{trailingSpace}");

        }

        output.AppendLine("}");
    }

    private static void JSEnumBuilder(DTODefinition def, StringBuilder output) {
        output.AppendLine($"export type {def.Name} = \""
            + string.Join("\" | \"", def.EnumValues)
            + "\";");
    }

    private static void ZodClassBuilder(DTODefinition def, StringBuilder output, ICrossFileReference reference) {
        output.AppendLine($"export const {def.Name}Schema = z.object({{");
        foreach (var option in def.Properties) {
            string name = option.Name;
            string type = option.Type;
            bool isArray = type.Contains("[]");
            type = type.Replace("[]", "");

            if (reference.ContainsKey(type)) {
                output.AppendLine($"    {name}: {type}Schema{(isArray ? ".array()" : "")},");
                continue;
            }

            string d = string.Empty;
            if (isArray) {
                d += ".array()";
            }

            if (option.Nullable) {
                d += ".nullable()";
            }

            if (option.Default != null) {
                string local;
                string opt = option.Default;
                if (type == "string" && opt != "null" && !isArray) {
                    local = '"' + opt + '"';
                } else {
                    local = opt;
                }

                d += $".default({local})";
            }

            output.AppendLine($"    {name}: z.{ConvertCSharpToJS(type)}(){d},");

        }

        output.AppendLine("});").AppendLine();

        output.AppendLine($"export type {def.Name}Type = z.infer<typeof {def.Name}Schema>;");
    }

    private static void ZodEnumBuilder(DTODefinition def, StringBuilder output) {
        output.AppendLine($"export const {def.Name}Schema = z.enum([\"{string.Join("\", \"", def.EnumValues)}\"]);").AppendLine();
        output.AppendLine($"export type {def.Name}Type = z.infer<typeof {def.Name}Schema>;").AppendLine();
    }
}
