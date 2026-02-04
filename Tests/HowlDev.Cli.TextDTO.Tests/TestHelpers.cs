namespace HowlDev.Cli.TextDTO.Tests;

public static class TestHelpers {
    public static string NormalizeWhitespace(string input) {
        return string.Join("\n", 
            input.Split('\n')
                 .Select(line => line.Trim())
                 .Where(line => !string.IsNullOrEmpty(line)));
    }
    
    public static async Task NormalStringsAreEqual(string actual, string expected) {
        var normalizedActual = NormalizeWhitespace(actual);
        var normalizedExpected = NormalizeWhitespace(expected);
        
        await Assert.That(normalizedActual).IsEqualTo(normalizedExpected);
    }
}
