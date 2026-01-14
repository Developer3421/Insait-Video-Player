using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Insait_Video_Player.Localization;

namespace Insait_Video_Player;

/// <summary>
/// Represents a history item for display in UI
/// </summary>
public class HistoryDisplayItem
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime WatchedAt { get; set; }
    public string DateText => WatchedAt.ToString("dd.MM.yyyy");
    public string TimeText => WatchedAt.ToString("HH:mm");
    
    public override string ToString() => FileName;
}

public partial class HistoryWindow : Window
{
    private readonly SessionManager? _sessionManager;
    private ObservableCollection<HistoryDisplayItem> _historyItems = new();
    private ObservableCollection<HistoryDisplayItem> _filteredItems = new();
    
    public HistoryWindow() : this(null)
    {
    }
    
    public HistoryWindow(SessionManager? sessionManager)
    {
        InitializeComponent();
        _sessionManager = sessionManager;
        
        SetupWindow();
        UpdateLocalizedText();
        LoadHistory();
        
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
        
        // Clear history button
        ClearHistoryButton.Click += OnClearHistoryClick;
        
        // Refresh button
        RefreshButton.Click += (_, _) => LoadHistory();
        
        // Search box
        SearchBox.TextChanged += OnSearchTextChanged;
    }
    
    private void UpdateLocalizedText()
    {
        var loc = LocalizationManager.Instance;
        
        TitleText.Text = loc["HistoryWindowTitle"];
        Title = loc["HistoryWindowTitle"];
        SearchBox.Watermark = loc["Search"];
        ClearButtonText.Text = loc["ClearHistory"];
        EmptyStateText.Text = loc["HistoryEmpty"];
        
        UpdateStatusText();
    }
    
    private void UpdateStatusText()
    {
        var loc = LocalizationManager.Instance;
        var count = _filteredItems.Count;
        StatusText.Text = string.Format(loc["HistoryCount"], count);
    }
    
    private void LoadHistory()
    {
        if (_sessionManager == null)
        {
            ShowEmptyState();
            return;
        }
        
        try
        {
            var history = _sessionManager.GetHistory();
            _historyItems.Clear();
            
            foreach (var item in history.OrderByDescending(h => h.WatchedAt))
            {
                _historyItems.Add(new HistoryDisplayItem
                {
                    Id = item.Id,
                    FileName = Path.GetFileName(item.FilePath as string) ?? item.FilePath,
                    FilePath = item.FilePath,
                    WatchedAt = item.WatchedAt
                });
            }
            
            ApplyFilter();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            ShowEmptyState();
        }
    }
    
    private void ApplyFilter()
    {
        var searchText = SearchBox.Text?.Trim().ToLowerInvariant() ?? string.Empty;
        
        _filteredItems.Clear();
        
        var filtered = string.IsNullOrEmpty(searchText)
            ? _historyItems
            : _historyItems.Where(h => 
                h.FileName.ToLowerInvariant().Contains(searchText) ||
                h.FilePath.ToLowerInvariant().Contains(searchText));
        
        foreach (var item in filtered)
        {
            _filteredItems.Add(item);
        }
        
        HistoryList.ItemsSource = _filteredItems;
        
        if (_filteredItems.Count == 0)
        {
            ShowEmptyState();
        }
        else
        {
            HideEmptyState();
        }
        
        UpdateStatusText();
    }
    
    private void ShowEmptyState()
    {
        EmptyState.IsVisible = true;
        HistoryList.IsVisible = false;
    }
    
    private void HideEmptyState()
    {
        EmptyState.IsVisible = false;
        HistoryList.IsVisible = true;
    }
    
    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }
    
    private void OnClearHistoryClick(object? sender, RoutedEventArgs e)
    {
        if (_sessionManager == null) return;
        
        try
        {
            _sessionManager.ClearHistory();
            _historyItems.Clear();
            _filteredItems.Clear();
            HistoryList.ItemsSource = _filteredItems;
            ShowEmptyState();
            UpdateStatusText();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing history: {ex.Message}");
        }
    }
    
    private void OnDeleteHistoryItem(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is HistoryDisplayItem item && _sessionManager != null)
        {
            try
            {
                _sessionManager.DeleteHistoryItem(item.Id);
                _historyItems.Remove(item);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting history item: {ex.Message}");
            }
        }
    }
    
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
}

