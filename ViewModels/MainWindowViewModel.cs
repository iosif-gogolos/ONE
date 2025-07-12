using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ONE.Models;
using ONE.Services;

namespace ONE.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly NoteStorageService _storageService;
        private NotePage? _selectedPage;
        private string _selectedPageContent = "";
        
        public ObservableCollection<NotePage> Pages { get; } = new();
        
        public NotePage? SelectedPage
        {
            get => _selectedPage;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _selectedPage, value);
                if (value != null)
                {
                    SelectedPageContent = string.Join("\n", value.Notes.ConvertAll(n => n.Content));
                }
            }
        }
        
        public string SelectedPageContent
        {
            get => _selectedPageContent;
            set 
            { 
                this.RaiseAndSetIfChanged(ref _selectedPageContent, value);
                if (SelectedPage != null)
                {
                    // Update the page content and save
                    UpdatePageContent(value);
                }
            }
        }
        
        public ReactiveCommand<Unit, Unit> AddPageCommand { get; }
        
        public MainWindowViewModel()
        {
            _storageService = new NoteStorageService();
            AddPageCommand = ReactiveCommand.Create(AddNewPage);
            
            // Load existing pages
            LoadPages();
        }

        
        
        private async void LoadPages()
        {
            try
            {
                var pages = await _storageService.LoadAllPagesAsync();
                Pages.Clear();
                foreach (var page in pages)
                {
                    Pages.Add(page);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pages: {ex.Message}");
            }
        }
        
        private void AddNewPage()
        {
            var newPage = new NotePage 
            { 
                Title = "New Page",
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            };
            
            Pages.Add(newPage);
            SelectedPage = newPage;
            
            // Save the new page
            _ = Task.Run(async () => await _storageService.SavePageAsync(newPage));
        }
        
        private void UpdatePageContent(string content)
        {
            if (SelectedPage == null) return;
            
            // Simple content update - in a real app, you'd parse this more sophisticatedly
            SelectedPage.Notes.Clear();
            if (!string.IsNullOrEmpty(content))
            {
                SelectedPage.Notes.Add(new Note 
                { 
                    Content = content,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                });
            }
            
            SelectedPage.ModifiedAt = DateTime.Now;
            
            // Auto-save
            _ = Task.Run(async () => await _storageService.SavePageAsync(SelectedPage));
        }
    }
}