using Avalonia.Controls;
using Avalonia.Input;
using Insait_Video_Player.Localization;

namespace Insait_Video_Player;

public partial class UserAgreementWindow : Window
{
    public UserAgreementWindow()
    {
        InitializeComponent();
        SetupWindow();
        UpdateLocalizedText();
        
        // Subscribe to language changes
        LocalizationManager.Instance.LanguageChanged += (_, _) => UpdateLocalizedText();
    }
    
    private void SetupWindow()
    {
        // Enable window dragging from title bar
        TitleBar.PointerPressed += OnTitleBarPointerPressed;
        TitleBarDragArea.PointerPressed += OnTitleBarPointerPressed;
        
        // Close button
        CloseButton.Click += (_, _) => Close();
        AcceptButton.Click += (_, _) => Close();
    }
    
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
    
    private void UpdateLocalizedText()
    {
        var loc = LocalizationManager.Instance;
        
        Title = loc["UserAgreementWindowTitle"];
        TitleText.Text = loc["UserAgreementWindowTitle"];
        
        AgreementTitle.Text = loc["UserAgreementTitle"];
        LastUpdatedText.Text = loc["UserAgreementLastUpdated"];
        
        Section1Title.Text = loc["UserAgreementSection1Title"];
        Section1Text.Text = loc["UserAgreementSection1Text"];
        
        Section2Title.Text = loc["UserAgreementSection2Title"];
        Section2Text.Text = loc["UserAgreementSection2Text"];
        
        Section3Title.Text = loc["UserAgreementSection3Title"];
        Section3Text.Text = loc["UserAgreementSection3Text"];
        
        Section4Title.Text = loc["UserAgreementSection4Title"];
        Section4Text.Text = loc["UserAgreementSection4Text"];
        
        Section5Title.Text = loc["UserAgreementSection5Title"];
        Section5Text.Text = loc["UserAgreementSection5Text"];
        
        CopyrightText.Text = loc["UserAgreementCopyright"];
        AcceptButton.Content = loc["UserAgreementAcceptButton"];
    }
}

