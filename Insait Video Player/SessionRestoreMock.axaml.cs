using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Insait_Video_Player.Localization;

namespace Insait_Video_Player;

public partial class SessionRestoreMock : UserControl
{
    public event EventHandler? PlayClicked;
    
    public SessionRestoreMock()
    {
        InitializeComponent();
        UpdateLocalizedTexts();
        
        // Listen for language changes
        LocalizationManager.Instance.LanguageChanged += (_, _) => UpdateLocalizedTexts();
        
        // Make play icon clickable
        PlayIconBorder.PointerPressed += (_, e) =>
        {
            if (e.GetCurrentPoint(PlayIconBorder).Properties.IsLeftButtonPressed)
            {
                PlayClicked?.Invoke(this, EventArgs.Empty);
            }
        };
        
        // Hover effects for play icon
        PlayIconBorder.PointerEntered += (_, _) =>
        {
            PlayIconBorder.Background = new SolidColorBrush(Color.Parse("#508B5CF6"));
        };
        PlayIconBorder.PointerExited += (_, _) =>
        {
            PlayIconBorder.Background = new SolidColorBrush(Color.Parse("#308B5CF6"));
        };
    }
    
    public void SetFileName(string fileName)
    {
        FileNameText.Text = fileName;
    }
    
    private void UpdateLocalizedTexts()
    {
        try
        {
            TitleText.Text = LocalizationManager.Instance["SessionRestoredTitle"];
            HintText.Text = LocalizationManager.Instance["PressPlayToStart"];
            OrText.Text = LocalizationManager.Instance["Or"];
        }
        catch
        {
            // Ignore localization errors
        }
    }
}

