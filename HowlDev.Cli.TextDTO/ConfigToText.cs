using HowlDev.IO.Text.ConfigFile;
using HowlDev.IO.Text.ConfigFile.Enums;

namespace HowlDev.Cli.TextDTO;

/// <summary>
/// Provides methods for turning an IBaseConfigOption to a given filetype. 
/// </summary>
public static class ConfigToText {
    /// <summary>
    /// To a standard C# DTO file.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToCSharpFile(TextConfigFile file) {
        if (file.Type != ConfigOptionType.Object) {
            throw new InvalidOperationException("Configuration must be of type Object.");
        }

        string output = "";

        bool ignoreWarnings = file.Contains("ignoreWarnings") && file["ignoreWarnings"].ToBoolean(null);
        if (ignoreWarnings) {
            output += "#pragma warning disable\n";
        }

        if (file.Contains("namespace"))
            output += "namespace " + file["namespace"] + ";\n\n";

        output += "public class " + file["name"] + " {\n";
        foreach (var option in file["properties"].Items) {
            string name = option["name"].ToString()!;
            string type = option["type"].ToString()!;

            string d = "\n";
            if (option.Contains("default")) {
                if (type == "string") {
                    d = '"' + option["default"].ToString() + '"';
                } else {
                    d = option["default"].ToString()!;
                }
                d = $"= {d};\n";
            }

            if (option.Contains("nullable") && option["nullable"].ToBoolean(null)) {
                type += "?";
            }

            output += $"    public {type} {name} {"{ get; set; }"} {d}";

        }
        output += "}\n";
        return output;
    }

    /// <summary>
    /// Returns a JS Type file with full exports. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToTSFile(TextConfigFile file) {
        if (file.Type != ConfigOptionType.Object) {
            throw new InvalidOperationException("Configuration must be of type Object.");
        }
        string output = "";

        bool ignoreWarnings = file.Contains("ignoreWarnings") && file["ignoreWarnings"].ToBoolean(null);
        if (ignoreWarnings) {
            output += "/* eslint-disable */\n";
        }

        output += "export type " + file["name"] + " = {\n";
        foreach (var option in file["properties"].Items) {
            string name = option["name"].ToString()!;
            string type = option["type"].ToString()!;

            string d = "\n";
            if (option.Contains("nullable") && option["nullable"].ToBoolean(null)) {
                d = "| undefined" + d;
            }

            output += $"    {name}: {ConvertCSharpToJS(type)} {d}";

        }
        output += "}\n";

        if (ignoreWarnings) {
            output += "/* eslint-enable */\n";
        }
        return output;
    }

    /// <summary>
    /// Returns a JS Zod file with the zod import and one-level objects. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string ToTSZodFile(TextConfigFile file) {
        if (file.Type != ConfigOptionType.Object) {
            throw new InvalidOperationException("Configuration must be of type Object.");
        }
        string output = "";

        bool ignoreWarnings = file.Contains("ignoreWarnings") && file["ignoreWarnings"].ToBoolean(null);
        if (ignoreWarnings) {
            output += "/* eslint-disable */\n";
        }

        output += "import z from \"zod\"\n\n";

        output += "export const " + file["name"] + "Schema = z.object({\n";
        foreach (var option in file["properties"].Items) {
            string name = option["name"].ToString()!;
            string type = option["type"].ToString()!;
            bool isArray = type.Contains("[]");
            type = type.Replace("[]", "");

            string d = "";
            if (option.Contains("default")) {
                if (type == "string") {
                    d = '"' + option["default"].ToString() + '"';
                } else {
                    d = option["default"].ToString()!;
                }
                d = $".default({d})";
            }
            if (option.Contains("nullable") && option["nullable"].ToBoolean(null)) {
                d += ".nullable()";
            }
            if (isArray) {
                d += ".array()";
            }

            output += $"    {name}: z.{ConvertCSharpToJS(type)}(){d},\n";

        }
        output += "});\n\n";

        output += $"export type {file["name"]}Type = z.infer<typeof {file["name"]}Schema>;\n";

        if (ignoreWarnings) {
            output += "/* eslint-enable */\n";
        }
        return output;
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
}