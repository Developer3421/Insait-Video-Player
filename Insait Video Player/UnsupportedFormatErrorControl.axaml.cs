using Avalonia.Controls;

namespace Insait_Video_Player;

public partial class UnsupportedFormatErrorControl : UserControl
{
    public UnsupportedFormatErrorControl()
    {
        InitializeComponent();
    }

    public UnsupportedFormatErrorControl(string? formatOrCodec)
    {
        InitializeComponent();
        
        if (!string.IsNullOrWhiteSpace(formatOrCodec))
        {
            FormatDetailsText.Text = formatOrCodec;
        }
    }
}

