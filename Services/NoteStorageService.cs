using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ONE.Models;

namespace ONE.Services;

public class NoteStorageService
{
    private readonly string _documentsPath;
    private readonly string _appFolderName = "ONE";
    
    public NoteStorageService()
    {
        // Use standard Documents folder instead of iCloud Documents
        // This is more likely to trigger permission dialogs
        var documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _documentsPath = Path.Combine(documentsDir, _appFolderName);
        
        // Create directory if it doesn't exist
        try
        {
            Directory.CreateDirectory(_documentsPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied to Documents folder: {ex.Message}");
            // Fallback to a user-accessible location
            var fallbackPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appFolderName);
            _documentsPath = fallbackPath;
            Directory.CreateDirectory(_documentsPath);
        }
    }
    
    public async Task<List<NotePage>> LoadAllPagesAsync()
    {
        var pages = new List<NotePage>();
        
        if (!Directory.Exists(_documentsPath))
            return pages;
            
        var files = Directory.GetFiles(_documentsPath, "*.one");
        
        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var page = JsonSerializer.Deserialize<NotePage>(json);
                if (page != null) pages.Add(page);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading page {file}: {ex.Message}");
            }
        }
        
        return pages;
    }
    
    public async Task SavePageAsync(NotePage page)
    {
        page.ModifiedAt = DateTime.Now;
        var fileName = $"{page.Id}.one";
        var filePath = Path.Combine(_documentsPath, fileName);
        
        var json = JsonSerializer.Serialize(page, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await File.WriteAllTextAsync(filePath, json);
    }
    
    public Task DeletePageAsync(Guid pageId)
    {
        var fileName = $"{pageId}.one";
        var filePath = Path.Combine(_documentsPath, fileName);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        return Task.CompletedTask;
    }
}