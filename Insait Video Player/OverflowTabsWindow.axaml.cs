using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Insait_Video_Player.Localization;

namespace Insait_Video_Player;

/// <summary>
/// Display item for overflow tabs
/// </summary>
public class OverflowTabDisplayItem
{
    public int TabId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public TabInstance? TabInstance { get; set; }
    
    // Localized tooltip strings for AXAML binding
    public string ActivateTooltip { get; set; } = string.Empty;
    public string MoveToMainTooltip { get; set; } = string.Empty;
    public string CloseTabTooltip { get; set; } = string.Empty;
}

public partial class OverflowTabsWindow : Window
{
    private ObservableCollection<OverflowTabDisplayItem> _overflowTabs = new();
    
    public const int MaxOverflowTabs = 4;
    
    /// <summary>
    /// Event raised when user wants to activate a tab from overflow
    /// </summary>
    public event EventHandler<TabInstance>? ActivateTabRequested;
    
    /// <summary>
    /// Event raised when user wants to move tab to main window
    /// </summary>
    public event EventHandler<TabInstance>? MoveToMainRequested;
    
    /// <summary>
    /// Event raised when user wants to delete a tab
    /// </summary>
    public event EventHandler<TabInstance>? DeleteTabRequested;
    
    /// <summary>
    /// Event raised when user wants to create a new tab in overflow
    /// </summary>
    public event EventHandler? CreateTabRequested;
    
    /// <summary>
    /// Current count of overflow tabs
    /// </summary>
    public int TabCount => _overflowTabs.Count;
    
    /// <summary>
    /// Whether overflow can accept more tabs (based on local limit)
    /// </summary>
    public bool CanAddTabLocally => _overflowTabs.Count < MaxOverflowTabs;
    
    /// <summary>
    /// Global flag set by PlayerWindow to indicate if total tab limit is reached
    /// </summary>
    public bool TotalLimitReached { get; set; }
    
    /// <summary>
    /// Whether overflow can accept more tabs (checks both local and global limits)
    /// </summary>
    public bool CanAddTab => CanAddTabLocally && !TotalLimitReached;
    
    public OverflowTabsWindow()
    {
        InitializeComponent();
        SetupWindow();
        UpdateLocalizedText();
        UpdateUI();
        
        // Subscribe to language changes
        LocalizationManager.Instance.LanguageChanged += (_, _) => UpdateLocalizedText();
    }
    
    private void SetupWindow()
    {
        // Enable window dragging from title bar
        TitleBar.PointerPressed += OnTitleBarPointerPressed;
        TitleBarDragArea.PointerPressed += OnTitleBarPointerPressed;
        
        // Close button - just hide, don't close
        CloseButton.Click += (_, _) => Hide();
        
        // Add tab button
        AddTabButton.Click += OnAddTabClick;
        
        // Set items source
        TabsList.ItemsSource = _overflowTabs;
    }
    
    private void UpdateLocalizedText()
    {
        var loc = LocalizationManager.Instance;
        
        TitleText.Text = loc["OverflowWindowTitle"];
        Title = loc["OverflowWindowTitle"];
        HeaderText.Text = loc["OverflowTabsHeader"];
        NewTabButtonText.Text = loc["NewTabButton"];
        EmptyStateText.Text = loc["NoOverflowTabs"];
        MaximumText.Text = string.Format(loc["MaximumTabs"], MaxOverflowTabs);
        
        // Add tab button tooltip
        ToolTip.SetTip(AddTabButton, loc["NewTab"]);
        
        // Update tooltip properties in all existing items
        foreach (var item in _overflowTabs)
        {
            item.ActivateTooltip = loc["ActivateTab"];
            item.MoveToMainTooltip = loc["MoveToMain"];
            item.CloseTabTooltip = loc["CloseTabTooltip"];
            if (item.TabInstance != null)
            {
                item.FilePath = item.TabInstance.FilePath ?? loc["NoFile"];
            }
        }
        
        // Force refresh of items to update bindings
        RefreshTabsList();
        
        UpdateUI();
    }
    
    /// <summary>
    /// Refreshes the tabs list to update bindings after language change
    /// </summary>
    private void RefreshTabsList()
    {
        if (_overflowTabs.Count == 0) return;
        
        var items = _overflowTabs.ToList();
        _overflowTabs.Clear();
        foreach (var item in items)
        {
            _overflowTabs.Add(item);
        }
    }
    
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
    
    /// <summary>
    /// Adds a tab to overflow window
    /// </summary>
    public bool AddTab(TabInstance tab)
    {
        if (!CanAddTabLocally) return false;
        
        var loc = LocalizationManager.Instance;
        _overflowTabs.Add(new OverflowTabDisplayItem
        {
            TabId = tab.Id,
            Title = tab.Title,
            FilePath = tab.FilePath ?? loc["NoFile"],
            TabInstance = tab,
            ActivateTooltip = loc["ActivateTab"],
            MoveToMainTooltip = loc["MoveToMain"],
            CloseTabTooltip = loc["CloseTabTooltip"]
        });
        
        UpdateUI();
        return true;
    }
    
    /// <summary>
    /// Removes a tab from overflow window
    /// </summary>
    public bool RemoveTab(TabInstance tab)
    {
        var item = _overflowTabs.FirstOrDefault(t => t.TabInstance == tab);
        if (item != null)
        {
            _overflowTabs.Remove(item);
            UpdateUI();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Gets all tabs in overflow
    /// </summary>
    public TabInstance[] GetAllTabs()
    {
        return _overflowTabs
            .Where(t => t.TabInstance != null)
            .Select(t => t.TabInstance!)
            .ToArray();
    }
    
    /// <summary>
    /// Clears all tabs from overflow
    /// </summary>
    public void ClearTabs()
    {
        _overflowTabs.Clear();
        UpdateUI();
    }
    
    /// <summary>
    /// Updates tab info in the list
    /// </summary>
    public void UpdateTabInfo(TabInstance tab)
    {
        var item = _overflowTabs.FirstOrDefault(t => t.TabInstance == tab);
        if (item != null)
        {
            var loc = LocalizationManager.Instance;
            item.Title = tab.Title;
            item.FilePath = tab.FilePath ?? loc["NoFile"];
            
            // Force refresh
            var index = _overflowTabs.IndexOf(item);
            if (index >= 0)
            {
                _overflowTabs.RemoveAt(index);
                _overflowTabs.Insert(index, item);
            }
        }
    }
    
    private void UpdateUI()
    {
        var loc = LocalizationManager.Instance;
        StatusText.Text = string.Format(loc["OverflowTabsCount"], _overflowTabs.Count, MaxOverflowTabs);
        
        if (_overflowTabs.Count == 0)
        {
            EmptyState.IsVisible = true;
            TabsList.IsVisible = false;
        }
        else
        {
            EmptyState.IsVisible = false;
            TabsList.IsVisible = true;
        }
        
        // Disable add button if at max (local or global)
        AddTabButton.IsEnabled = CanAddTab;
    }
    
    /// <summary>
    /// Updates the UI to reflect global limit changes
    /// </summary>
    public void RefreshLimitState()
    {
        UpdateUI();
    }
    
    private void OnAddTabClick(object? sender, RoutedEventArgs e)
    {
        if (CanAddTab)
        {
            CreateTabRequested?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private void OnActivateTab(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is OverflowTabDisplayItem item && item.TabInstance != null)
        {
            ActivateTabRequested?.Invoke(this, item.TabInstance);
        }
    }
    
    private void OnMoveToMain(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is OverflowTabDisplayItem item && item.TabInstance != null)
        {
            MoveToMainRequested?.Invoke(this, item.TabInstance);
        }
    }
    
    private void OnDeleteTab(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is OverflowTabDisplayItem item && item.TabInstance != null)
        {
            DeleteTabRequested?.Invoke(this, item.TabInstance);
            RemoveTab(item.TabInstance);
        }
    }
}

