using Avalonia.Controls;

namespace Insait_Video_Player;

public partial class FileNotFoundErrorControl : UserControl
{
    public FileNotFoundErrorControl()
    {
        InitializeComponent();
    }

    public FileNotFoundErrorControl(string? filePath)
    {
        InitializeComponent();
        
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            FilePathText.Text = filePath;
        }
    }
}
