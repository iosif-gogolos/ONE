using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace ONE.Services;

public class FileAccessService
{
    public async Task<string?> RequestDocumentsFolderAccessAsync(Window parentWindow)
    {
        var storageProvider = parentWindow.StorageProvider;
        
        if (storageProvider.CanPickFolder)
        {
            var options = new FolderPickerOpenOptions
            {
                Title = "Grant access to Documents folder",
                AllowMultiple = false
            };
            
            var folders = await storageProvider.OpenFolderPickerAsync(options);
            
            if (folders.Count > 0)
            {
                return folders[0].Path.LocalPath;
            }
        }
        
        return null;
    }
    
    public async Task<string?> SaveFileAsAsync(Window parentWindow, string defaultName, string content)
    {
        var storageProvider = parentWindow.StorageProvider;
        
        if (storageProvider.CanSave)
        {
            var options = new FilePickerSaveOptions
            {
                Title = "Save Note",
                DefaultExtension = "one",
                SuggestedFileName = defaultName,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("ONE Note")
                    {
                        Patterns = new[] { "*.one" }
                    }
                }
            };
            
            var file = await storageProvider.SaveFilePickerAsync(options);
            
            if (file != null)
            {
                await using var stream = await file.OpenWriteAsync();
                await using var writer = new System.IO.StreamWriter(stream);
                await writer.WriteAsync(content);
                return file.Path.LocalPath;
            }
        }
        
        return null;
    }
}
