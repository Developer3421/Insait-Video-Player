using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Insait_Video_Player.Localization;

namespace Insait_Video_Player;

public partial class SettingsWindow : Window
{
    private ObservableCollection<LanguageItem> _languages = new();
    private Language _selectedLanguage;
    
    public SettingsWindow()
    {
        InitializeComponent();
        SetupWindow();
        InitializeLanguages();
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
        
        // Navigation buttons
        GeneralSectionButton.Click += (_, _) => ShowSection("General");
        AboutSectionButton.Click += (_, _) => ShowSection("About");
    }
    
    private void InitializeLanguages()
    {
        _languages.Clear();
        
        _languages.Add(new LanguageItem 
        { 
            Language = Language.Ukrainian, 
            DisplayName = LocalizationManager.GetLanguageDisplayName(Language.Ukrainian),
            Code = LocalizationManager.GetLanguageCode(Language.Ukrainian)
        });
        _languages.Add(new LanguageItem 
        { 
            Language = Language.English, 
            DisplayName = LocalizationManager.GetLanguageDisplayName(Language.English),
            Code = LocalizationManager.GetLanguageCode(Language.English)
        });
        _languages.Add(new LanguageItem 
        { 
            Language = Language.German, 
            DisplayName = LocalizationManager.GetLanguageDisplayName(Language.German),
            Code = LocalizationManager.GetLanguageCode(Language.German)
        });
        _languages.Add(new LanguageItem 
        { 
            Language = Language.Russian, 
            DisplayName = LocalizationManager.GetLanguageDisplayName(Language.Russian),
            Code = LocalizationManager.GetLanguageCode(Language.Russian)
        });
        _languages.Add(new LanguageItem 
        { 
            Language = Language.Turkish, 
            DisplayName = LocalizationManager.GetLanguageDisplayName(Language.Turkish),
            Code = LocalizationManager.GetLanguageCode(Language.Turkish)
        });
        
        LanguageComboBox.ItemsSource = _languages;
        
        // Select current language
        _selectedLanguage = LocalizationManager.Instance.CurrentLanguage;
        for (int i = 0; i < _languages.Count; i++)
        {
            if (_languages[i].Language == _selectedLanguage)
            {
                LanguageComboBox.SelectedIndex = i;
                break;
            }
        }
        
        // Handle language selection change
        LanguageComboBox.SelectionChanged += OnLanguageSelectionChanged;
    }
    
    private void OnLanguageSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox.SelectedItem is LanguageItem selectedItem)
        {
            if (selectedItem.Language != _selectedLanguage)
            {
                _selectedLanguage = selectedItem.Language;
                LocalizationManager.Instance.SetLanguage(_selectedLanguage);
            }
        }
    }
    
    private void UpdateLocalizedText()
    {
        var loc = LocalizationManager.Instance;
        
        // Window title
        TitleText.Text = loc["SettingsWindowTitle"];
        Title = loc["SettingsWindowTitle"];
        
        // Navigation
        GeneralNavText.Text = loc["General"];
        AboutNavText.Text = loc["About"];
        
        // General panel
        LanguageSectionTitle.Text = loc["Language"];
        LanguageSectionDescription.Text = loc["SelectLanguage"];
        RestartNoticeText.Text = loc["RestartRequired"];
        
        // About panel
        AboutTitle.Text = loc["About"];
        VersionText.Text = $"{loc["Version"]} 1.0.0";
        AboutDescriptionText.Text = loc["AboutDescription"];
        CopyrightText.Text = loc["Copyright"];
        PoweredByText.Text = loc["PoweredBy"];
    }
    
    private void ShowSection(string section)
    {
        // Update button states
        GeneralSectionButton.Classes.Remove("active");
        AboutSectionButton.Classes.Remove("active");
        
        // Hide all panels
        GeneralPanel.IsVisible = false;
        AboutPanel.IsVisible = false;
        
        // Show selected section
        switch (section)
        {
            case "General":
                GeneralSectionButton.Classes.Add("active");
                GeneralPanel.IsVisible = true;
                break;
            case "About":
                AboutSectionButton.Classes.Add("active");
                AboutPanel.IsVisible = true;
                break;
        }
    }
    
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
}
    }

