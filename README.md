# Insait Video Player

A modern, feature-rich video player for Windows built with Avalonia UI and LibVLC.

## Features

### Core Functionality
- **Multi-Format Support**: Play a wide variety of video formats powered by LibVLC
- **Tabbed Interface**: Open and manage multiple videos in tabs with overflow handling
- **Session Management**: Automatically saves and restores your playback sessions
- **Playback History**: Track recently watched videos with position memory
- **Audio Device Selection**: Choose your preferred audio output device
- **Multi-Language Support**: Available in Ukrainian, English, German, Russian, and Turkish

### Playback Controls
- **Advanced Timeline**: Precise seeking with thumbnail preview and time display
- **Speed Control**: Adjust playback speed from 0.25x to 2.0x
- **Volume Control**: Fine-grained volume adjustment with visual feedback
- **Subtitles**: Full subtitle support with multiple track selection
- **Audio Tracks**: Switch between multiple audio tracks
- **Loop Mode**: Repeat single video or entire playlist

### User Interface
- **Custom Window Controls**: Modern, minimalistic design with custom title bar
- **Fullscreen Mode**: Immersive viewing experience with automatic UI hiding
- **Drag and Drop**: Open videos by dragging files onto the player
- **Keyboard Shortcuts**: Extensive keyboard controls for efficient navigation
- **Settings Panel**: Customize language, audio output, and other preferences

### Data Management
- **Encrypted Storage**: Session data and history are encrypted using Windows DPAPI
- **LiteDB Database**: Fast and lightweight embedded database for persistence
- **Privacy Focused**: All data stored locally with encryption

## Technology Stack

- **Framework**: .NET 10.0
- **UI Framework**: Avalonia UI 11.3.x (Cross-platform XAML-based UI)
- **Media Engine**: LibVLCSharp 3.9.5 with VLC 3.0.21
- **Database**: LiteDB 6.0 (Embedded NoSQL database)
- **Encryption**: System.Security.Cryptography.ProtectedData (DPAPI)

## Requirements

- Windows 10 or later
- .NET 10.0 Runtime


## Project Structure

```
Insait Video Player/
├── PlayerWindow.axaml(.cs)          # Main player window and logic
├── SettingsWindow.axaml(.cs)        # Settings interface
├── HistoryWindow.axaml(.cs)         # Playback history viewer
├── SessionManager.cs                # Session persistence and encryption
├── Program.cs                       # Application entry point
├── App.axaml(.cs)                   # Application configuration
├── Localization/
│   └── LocalizationManager.cs       # Multi-language support
├── Sources/                         # UI resources and images
└── Icons/                           # Application icons
```

## Additional Features

- **Tab Management**:  Tabs with drag-to-reorder and overflow menu
- **Session Persistence**: Auto-saves sessions with encrypted storage
- **Error Handling**: User-friendly messages 

# #Download
Microsoft Store: (in work)

## Known Issues

- Some exotic video formats may not be supported
- Hardware acceleration depends on VLC capabilities
- First launch might take longer due to VLC initialization

## License
This project is licensed under the MIT License.

