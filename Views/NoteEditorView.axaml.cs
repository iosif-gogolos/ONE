using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ONE.Views
{
    public partial class NoteEditorView : UserControl
    {
        public NoteEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
