using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;
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
    public static string ToCSharpFile(TextConfigFile file, ICrossFileReference reference) {
        if (file.Type != ConfigOptionType.Object) {
            throw new InvalidOperationException("Configuration must be of type Object.");
        }

        var output = new StringBuilder();

        bool ignoreWarnings = file.Contains("ignoreWarnings") && file["ignoreWarnings"].ToBoolean(null);
        if (ignoreWarnings) {
            output.AppendLine("#pragma warning disable");
        }

        if (file["type"].ToString() == "Class") {
            // Check for namespaces that we need to include from our properties
            var namespaces = file["properties"].Items
                .Where(a => reference.ContainsKey(a["type"].ToString()!.Replace("[]", "")) &&
                    reference.GetReference(a["type"].ToString()!.Replace("[]", "")).csharpNamespace != file["namespace"].ToString())
                .Select(a => reference.GetReference(a["type"].ToString()!.Replace("[]", "")).csharpNamespace);

            foreach (var item in namespaces) {
                output.AppendLine($"using {item};");
            }
        }

        output.AppendLine($"namespace {file["namespace"]};").AppendLine();

        switch (file["type"].ToString()) {
            case "Class":
                CSharpClassBuilder(file, output);
                break;
            case "Enum":
                CSharpEnumBuilder(file, output);
                break;
        }
        return output.ToString();
    }

    /// <summary>
    /// Returns a JS Type file with full exports. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToTSFile(TextConfigFile file, ICrossFileReference reference) {
        if (file.Type != ConfigOptionType.Object) {
            throw new InvalidOperationException("Configuration must be of type Object.");
        }
        var output = new StringBuilder();

        bool ignoreWarnings = file.Contains("ignoreWarnings") && file["ignoreWarnings"].ToBoolean(null);
        if (ignoreWarnings) {
            output.AppendLine("/* eslint-disable */");
        }

        if (file["type"].ToString() == "Class") {
            // Check for imports that we need to include from our properties
            var fileImports = file["properties"].Items
                .Where(a => reference.ContainsKey(a["type"].ToString()!.Replace("[]", "")))
                .Select(a => (a["type"].ToString()!, reference.GetReference(a["type"].ToString()!.Replace("[]", "")).file));

            foreach (var item in fileImports) {
                output.AppendLine($"import type {"{"} {item.Item1.Replace("[]", "")} {"}"} from './{item.file}.ts';");
            }
        }

        switch (file["type"].ToString()) {
            case "Class":
                JSClassBuilder(file, output);
                break;
            case "Enum":
                JSEnumBuilder(file, output);
                break;
        }

        if (ignoreWarnings) {
            output.AppendLine("/* eslint-enable */");
        }
        return output.ToString();
    }

    /// <summary>
    /// Returns a JS Zod file with the zod import and one-level objects. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToTSZodFile(TextConfigFile file, ICrossFileReference reference) {
        if (file.Type != ConfigOptionType.Object) {
            throw new InvalidOperationException("Configuration must be of type Object.");
        }
        var output = new StringBuilder();

        bool ignoreWarnings = file.Contains("ignoreWarnings") && file["ignoreWarnings"].ToBoolean(null);
        if (ignoreWarnings) {
            output.AppendLine("/* eslint-disable */");
        }

        if (file["type"].ToString() == "Class") {
            // Check for imports that we need to include from our properties
            var fileImports = file["properties"].Items
                .Where(a => reference.ContainsKey(a["type"].ToString()!.Replace("[]", "")))
                .Select(a => (a["type"].ToString()!, reference.GetReference(a["type"].ToString()!.Replace("[]", "")).file));

            foreach (var item in fileImports) {
                output.AppendLine($"import {"{"} {item.Item1.Replace("[]", "")}Schema {"}"} from \"./{item.file}.ts\";");
            }
        }

        output.AppendLine("import z from \"zod\"").AppendLine();

        switch (file["type"].ToString()) {
            case "Class":
                ZodClassBuilder(file, output, reference);
                break;
            case "Enum":
                ZodEnumBuilder(file, output);
                break;
        }

        if (ignoreWarnings) {
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

    private static void CSharpClassBuilder(TextConfigFile file, StringBuilder output) {
        output.AppendLine($"public class {file["name"]} {{");
        foreach (var option in file["properties"].Items) {
            string name = option["name"].ToString()!;
            string type = option["type"].ToString()!;

            string d = "";
            if (option.Contains("default")) {
                if (type == "string") {
                    d = '"' + option["default"].ToString() + '"';
                } else {
                    d = option["default"].ToString()!;
                }
                d = $" {{ get; set; }} = {d};";
            } else {
                d = " { get; set; } ";
            }

            if (option.Contains("nullable") && option["nullable"].ToBoolean(null)) {
                type += "?";
            }

            output.AppendLine($"    public {type} {name}{d}");

        }
        output.AppendLine("}");
    }

    private static void CSharpEnumBuilder(TextConfigFile file, StringBuilder output) {
        output.AppendLine($"public enum {file["name"]} {{");
        output.Append("    " + string.Join(",\n    ", file["properties"].AsEnumerable<string>())).AppendLine();
        output.AppendLine("}");
    }

    private static void JSClassBuilder(TextConfigFile file, StringBuilder output) {
        output.AppendLine($"export type {file["name"]} = {{");
        foreach (var option in file["properties"].Items) {
            string name = option["name"].ToString()!;
            string type = option["type"].ToString()!;

            string nullable = "";
            string trailingSpace = " ";
            if (option.Contains("nullable") && option["nullable"].ToBoolean(null)) {
                nullable = " | undefined";
                trailingSpace = "";
            }

            output.AppendLine($"    {name}: {ConvertCSharpToJS(type)}{nullable}{trailingSpace}");

        }
        output.AppendLine("}");
    }

    private static void JSEnumBuilder(TextConfigFile file, StringBuilder output) {
        output.AppendLine($"export type {file["name"]} = \""
            + string.Join("\" | \"", file["properties"].AsEnumerable<string>())
            + "\";");
    }

    private static void ZodClassBuilder(TextConfigFile file, StringBuilder output, ICrossFileReference reference) {
        output.AppendLine($"export const {file["name"]}Schema = z.object({{");
        foreach (var option in file["properties"].Items) {
            string name = option["name"].ToString()!;
            string type = option["type"].ToString()!;
            bool isArray = type.Contains("[]");
            type = type.Replace("[]", "");

            if (reference.ContainsKey(type)) {
                output.AppendLine($"    {name}: {type}Schema{(isArray ? ".array()" : "")},");
                continue;
            }

            string d = string.Empty;
            if (option.Contains("nullable") && option["nullable"].ToBoolean(null)) {
                d += ".nullable()";
            }
            if (isArray) {
                d += ".array()";
            }
            if (option.Contains("default")) {
                string local = string.Empty;
                if (type == "string") {
                    local = '"' + option["default"].ToString() + '"';
                } else {
                    local = option["default"].ToString()!;
                }
                d += $".default({local})";
            }

            output.AppendLine($"    {name}: z.{ConvertCSharpToJS(type)}(){d},");

        }
        output.AppendLine("});").AppendLine();

        output.AppendLine($"export type {file["name"]}Type = z.infer<typeof {file["name"]}Schema>;");
    }

    private static void ZodEnumBuilder(TextConfigFile file, StringBuilder output) {
        output.AppendLine($"export const {file["name"]}Schema = z.enum([\"{string.Join("\", \"", file["properties"].AsEnumerable<string>())}\"]);").AppendLine();
        output.AppendLine($"export type {file["name"]}Type = z.infer<typeof {file["name"]}Schema>;").AppendLine();
    }
}
