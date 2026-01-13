using Avalonia.Controls;
using Avalonia.Input;

namespace Insait_Video_Player;

public enum ErrorType
{
    FileNotFound,
    UnsupportedFormat,
    AudioOutputError
}

public partial class ErrorWindow : Window
{
    public ErrorWindow()
    {
        InitializeComponent();
        SetupWindow();
    }

    public ErrorWindow(ErrorType errorType, string? details = null)
    {
        InitializeComponent();
        SetupWindow();
        SetErrorContent(errorType, details);
    }

    private void SetupWindow()
    {
        // Enable window dragging from title bar
        TitleBar.PointerPressed += OnTitleBarPointerPressed;
        TitleBarDragArea.PointerPressed += OnTitleBarPointerPressed;

        // Close button
        CloseButton.Click += (_, _) => Close();
    }

    private void SetErrorContent(ErrorType errorType, string? details)
    {
        switch (errorType)
        {
            case ErrorType.FileNotFound:
                TitleText.Text = "Файл не знайдено";
                ErrorContent.Content = new FileNotFoundErrorControl(details);
                break;

            case ErrorType.UnsupportedFormat:
                TitleText.Text = "Непідтримуваний формат";
                ErrorContent.Content = new UnsupportedFormatErrorControl(details);
                break;

            case ErrorType.AudioOutputError:
                TitleText.Text = "Помилка аудіо";
                ErrorContent.Content = new AudioOutputErrorControl(details);
                break;
        }
    }

    public static ErrorWindow CreateFileNotFoundError(string? filePath = null)
    {
        return new ErrorWindow(ErrorType.FileNotFound, filePath);
    }

    public static ErrorWindow CreateUnsupportedFormatError(string? formatOrCodec = null)
    {
        return new ErrorWindow(ErrorType.UnsupportedFormat, formatOrCodec);
    }

    public static ErrorWindow CreateAudioOutputError(string? deviceInfo = null)
    {
        return new ErrorWindow(ErrorType.AudioOutputError, deviceInfo);
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }
}
