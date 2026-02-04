namespace HowlDev.Cli.TextDTO.Tests;

public static class TestHelpers {
    public static string NormalizeWhitespace(string input) {
        return string.Join("\n", 
            input.Split('\n')
                 .Select(line => line.Trim())
                 .Where(line => !string.IsNullOrEmpty(line)));
    }
    
    public static async Task AssertCodeEquals(string actual, string expected) {
        var normalizedActual = NormalizeWhitespace(actual);
        var normalizedExpected = NormalizeWhitespace(expected);
        
        if (normalizedActual != normalizedExpected) {
            Console.WriteLine("EXPECTED:");
            Console.WriteLine(expected);
            Console.WriteLine("\nACTUAL:");
            Console.WriteLine(actual);
            Console.WriteLine("\nNORMALIZED EXPECTED:");
            Console.WriteLine(normalizedExpected);
            Console.WriteLine("\nNORMALIZED ACTUAL:");
            Console.WriteLine(normalizedActual);
        }
        
        await Assert.That(normalizedActual).IsEqualTo(normalizedExpected);
    }
    
    public static async Task AssertCodeEqualsLineByLineAsync(string actual, string expected) {
        var actualLines = actual.Split('\n').Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToArray();
        var expectedLines = expected.Split('\n').Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToArray();
        
        if (actualLines.Length != expectedLines.Length) {
            Console.WriteLine($"Line count mismatch: Expected {expectedLines.Length}, got {actualLines.Length}");
        }
        
        var maxLines = Math.Max(actualLines.Length, expectedLines.Length);
        for (int i = 0; i < maxLines; i++) {
            var actualLine = i < actualLines.Length ? actualLines[i] : "<MISSING>";
            var expectedLine = i < expectedLines.Length ? expectedLines[i] : "<MISSING>";
            
            if (actualLine != expectedLine) {
                Console.WriteLine($"Line {i + 1} differs:");
                Console.WriteLine($"  Expected: '{expectedLine}'");
                Console.WriteLine($"  Actual:   '{actualLine}'");
            }
        }
        
        await Assert.That(actualLines).IsEqualTo(expectedLines);
    }
}
