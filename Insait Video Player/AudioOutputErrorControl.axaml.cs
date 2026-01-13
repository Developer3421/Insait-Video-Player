using Avalonia.Controls;

namespace Insait_Video_Player;

public partial class AudioOutputErrorControl : UserControl
{
    public AudioOutputErrorControl()
    {
        InitializeComponent();
    }

    public AudioOutputErrorControl(string? deviceInfo)
    {
        InitializeComponent();
        
        if (!string.IsNullOrWhiteSpace(deviceInfo))
        {
            DeviceInfoText.Text = deviceInfo;
        }
    }
}

