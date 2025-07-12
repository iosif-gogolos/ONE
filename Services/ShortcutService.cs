using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ONE.Services;

public static class ShortcutService
{
    public static (bool wasProcessed, string transformedText) ProcessShortcut(string text, int cursorPosition)
    {
        if (cursorPosition == 0) return (false, text);
        
        var lineStart = text.LastIndexOf('\n', cursorPosition - 1) + 1;
        var lineEnd = text.IndexOf('\n', cursorPosition);
        if (lineEnd == -1) lineEnd = text.Length;
        
        var currentLine = text.Substring(lineStart, lineEnd - lineStart);
        var beforeCursor = text.Substring(lineStart, cursorPosition - lineStart);
        
        // Check for heading shortcuts
        if (Regex.IsMatch(beforeCursor, @"^#{1,6}\s*$"))
        {
            var hashCount = beforeCursor.Where(c => c == '#').Count();
            var headingLevel = Math.Min(hashCount, 6);
            var headingPrefix = new string('#', headingLevel) + " ";
            
            var newText = text.Substring(0, lineStart) + headingPrefix + text.Substring(lineEnd);
            return (true, newText);
        }
        
        // Check for bullet points
        if (Regex.IsMatch(beforeCursor, @"^-\s*$"))
        {
            var newText = text.Substring(0, lineStart) + "• " + text.Substring(lineEnd);
            return (true, newText);
        }
        
        // Check for numbered lists
        if (Regex.IsMatch(beforeCursor, @"^\d+\.\s*$"))
        {
            var match = Regex.Match(beforeCursor, @"^(\d+)\.\s*$");
            var number = int.Parse(match.Groups[1].Value);
            var listItem = $"{number}. ";
            
            var newText = text.Substring(0, lineStart) + listItem + text.Substring(lineEnd);
            return (true, newText);
        }
        
        return (false, text);
    }
}