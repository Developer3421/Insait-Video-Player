using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Insait_Video_Player.Localization;

/// <summary>
/// Supported languages in the application
/// </summary>
public enum Language
{
    Ukrainian,  // uk-UA
    English,    // en-US
    German,     // de-DE
    Russian,    // ru-RU
    Turkish     // tr-TR
}

/// <summary>
/// Manages localization/internationalization for the application
/// </summary>
public class LocalizationManager
{
    private static LocalizationManager? _instance;
    public static LocalizationManager Instance => _instance ??= new LocalizationManager();
    
    private Dictionary<string, string> _currentStrings = new();
    private Language _currentLanguage = Language.English;
    
    public event EventHandler? LanguageChanged;
    
    public Language CurrentLanguage => _currentLanguage;
    
    public static string GetLanguageCode(Language language) => language switch
    {
        Language.Ukrainian => "uk-UA",
        Language.English => "en-US",
        Language.German => "de-DE",
        Language.Russian => "ru-RU",
        Language.Turkish => "tr-TR",
        _ => "en-US"
    };
    
    public static string GetLanguageDisplayName(Language language) => language switch
    {
        Language.Ukrainian => "Українська",
        Language.English => "English",
        Language.German => "Deutsch",
        Language.Russian => "Русский",
        Language.Turkish => "Türkçe",
        _ => "English"
    };
    
    public static Language GetLanguageFromCode(string code) => code switch
    {
        "uk-UA" or "uk" => Language.Ukrainian,
        "en-US" or "en" => Language.English,
        "de-DE" or "de" => Language.German,
        "ru-RU" or "ru" => Language.Russian,
        "tr-TR" or "tr" => Language.Turkish,
        _ => Language.English
    };
    
    public LocalizationManager()
    {
        LoadLanguage(_currentLanguage);
    }
    
    public void SetLanguage(Language language)
    {
        if (_currentLanguage == language) return;
        
        _currentLanguage = language;
        LoadLanguage(language);
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void LoadLanguage(Language language)
    {
        _currentStrings = language switch
        {
            Language.Ukrainian => GetUkrainianStrings(),
            Language.English => GetEnglishStrings(),
            Language.German => GetGermanStrings(),
            Language.Russian => GetRussianStrings(),
            Language.Turkish => GetTurkishStrings(),
            _ => GetUkrainianStrings()
        };
    }
    
    public string this[string key] => GetString(key);
    
    public string GetString(string key)
    {
        return _currentStrings.TryGetValue(key, out var value) ? value : key;
    }
    
    #region Ukrainian Strings
    private static Dictionary<string, string> GetUkrainianStrings() => new()
    {
        // Window titles
        ["WindowTitle"] = "Insait Video Player",
        ["SettingsWindowTitle"] = "Налаштування",
        ["ErrorWindowTitle"] = "Помилка",
        
        // Title bar buttons
        ["Settings"] = "Налаштування",
        ["Tools"] = "Інструменти",
        ["OpenFile"] = "Відкрити файл",
        ["OpenFolder"] = "Відкрити папку",
        ["NewTab"] = "Новий таб",
        ["NewWindow"] = "Нове вікно",
        
        // Empty state
        ["OpenFileOrDragVideo"] = "Відкрийте файл або перетягніть відео сюди",
        ["SupportedFormats"] = "Підтримувані формати:",
        ["OpenFileAction"] = "Відкрити файл",
        ["PlayPause"] = "Play/Pause",
        ["Fullscreen"] = "Повноекранний режим",
        
        // Transport controls
        ["Play"] = "Відтворити",
        ["Pause"] = "Пауза",
        ["Stop"] = "Стоп",
        ["MuteSound"] = "Вимкнути звук",
        ["UnmuteSound"] = "Увімкнути звук",
        ["SelectAudioDevice"] = "Вибір аудіо пристрою",
        ["RefreshDevices"] = "Оновити список пристроїв",
        ["DefaultDevice"] = "За замовчуванням",
        
        // File picker
        ["PickVideoOrAudio"] = "Виберіть відео або аудіо",
        ["MediaFiles"] = "Медіа файли",
        ["AllFiles"] = "Всі файли",
        ["PickFolderWithMedia"] = "Виберіть папку з медіа",
        
        // Errors
        ["FileNotFound"] = "Файл не знайдено",
        ["UnsupportedFormat"] = "Непідтримуваний формат",
        ["AudioError"] = "Помилка аудіо",
        ["AccessDenied"] = "Доступ заборонено",
        ["NoPermission"] = "Немає прав доступу",
        ["PlaybackError"] = "Помилка відтворення",
        ["CannotPlayMedia"] = "Не вдалося відтворити медіа файл. Можливо, формат не підтримується або файл пошкоджено.",
        ["CannotChangeAudioDevice"] = "Не вдалося змінити аудіо пристрій.",
        ["FileExtension"] = "Розширення файлу",
        ["SupportedFormatsDetails"] = "Підтримувані формати: MP4, MKV, AVI, MOV, MP3, WAV, FLAC, WEBM, WMV",
        ["Device"] = "Пристрій",
        ["Error"] = "Помилка",
        
        // Settings
        ["Language"] = "Мова",
        ["Appearance"] = "Вигляд",
        ["About"] = "Про програму",
        ["Version"] = "Версія",
        ["Save"] = "Зберегти",
        ["Cancel"] = "Скасувати",
        ["Apply"] = "Застосувати",
        ["General"] = "Загальні",
        ["SelectLanguage"] = "Виберіть мову інтерфейсу",
        ["RestartRequired"] = "Для застосування деяких змін може знадобитися перезапуск програми",
        
        // About page
        ["AboutDescription"] = "Сучасний відео плеєр з підтримкою багатьох форматів",
        ["Copyright"] = "© 2026 Insait",
        ["PoweredBy"] = "Відтворення відео на базі LibVLC",
        
        // Tab
        ["NewTabTitle"] = "Новий таб",
        ["CloseTab"] = "Закрити таб",
        
        // History
        ["HistoryWindowTitle"] = "Історія перегляду",
        ["Search"] = "Пошук...",
        ["ClearHistory"] = "Очистити",
        ["HistoryEmpty"] = "Історія порожня",
        ["HistoryCount"] = "{0} записів",
        ["DeleteFromHistory"] = "Видалити з історії",
        ["Refresh"] = "Оновити",
        
        // Session restore mock
        ["SessionRestoredTitle"] = "Сесію відновлено",
        ["PressPlayToStart"] = "Натисніть кнопку Play, щоб продовжити перегляд",
        ["Or"] = "або",
        
        // Overflow tabs window
        ["OverflowWindowTitle"] = "Додаткові вкладки",
        ["OverflowTabsHeader"] = "Вкладки в overflow",
        ["NewTabButton"] = "Нова вкладка",
        ["ActivateTab"] = "Активувати вкладку",
        ["MoveToMain"] = "Перемістити в головне вікно",
        ["CloseTabTooltip"] = "Закрити вкладку",
        ["NoFile"] = "Немає файлу",
        ["NoOverflowTabs"] = "Немає додаткових вкладок",
        ["OverflowTabsCount"] = "{0} / {1} вкладок",
        ["MaximumTabs"] = "Максимум: {0}",
        ["OverflowTabs"] = "Додаткові вкладки",
        ["TabLimitReached"] = "Досягнуто ліміт вкладок"
    };
    #endregion
    
    #region English Strings
    private static Dictionary<string, string> GetEnglishStrings() => new()
    {
        // Window titles
        ["WindowTitle"] = "Insait Video Player",
        ["SettingsWindowTitle"] = "Settings",
        ["ErrorWindowTitle"] = "Error",
        
        // Title bar buttons
        ["Settings"] = "Settings",
        ["Tools"] = "Tools",
        ["OpenFile"] = "Open file",
        ["OpenFolder"] = "Open folder",
        ["NewTab"] = "New tab",
        ["NewWindow"] = "New window",
        
        // Empty state
        ["OpenFileOrDragVideo"] = "Open a file or drag video here",
        ["SupportedFormats"] = "Supported formats:",
        ["OpenFileAction"] = "Open file",
        ["PlayPause"] = "Play/Pause",
        ["Fullscreen"] = "Fullscreen",
        
        // Transport controls
        ["Play"] = "Play",
        ["Pause"] = "Pause",
        ["Stop"] = "Stop",
        ["MuteSound"] = "Mute",
        ["UnmuteSound"] = "Unmute",
        ["SelectAudioDevice"] = "Select audio device",
        ["RefreshDevices"] = "Refresh device list",
        ["DefaultDevice"] = "Default",
        
        // File picker
        ["PickVideoOrAudio"] = "Select video or audio",
        ["MediaFiles"] = "Media files",
        ["AllFiles"] = "All files",
        ["PickFolderWithMedia"] = "Select folder with media",
        
        // Errors
        ["FileNotFound"] = "File not found",
        ["UnsupportedFormat"] = "Unsupported format",
        ["AudioError"] = "Audio error",
        ["AccessDenied"] = "Access denied",
        ["NoPermission"] = "No permission",
        ["PlaybackError"] = "Playback error",
        ["CannotPlayMedia"] = "Cannot play media file. The format may not be supported or the file is corrupted.",
        ["CannotChangeAudioDevice"] = "Cannot change audio device.",
        ["FileExtension"] = "File extension",
        ["SupportedFormatsDetails"] = "Supported formats: MP4, MKV, AVI, MOV, MP3, WAV, FLAC, WEBM, WMV",
        ["Device"] = "Device",
        ["Error"] = "Error",
        
        // Settings
        ["Language"] = "Language",
        ["Appearance"] = "Appearance",
        ["About"] = "About",
        ["Version"] = "Version",
        ["Save"] = "Save",
        ["Cancel"] = "Cancel",
        ["Apply"] = "Apply",
        ["General"] = "General",
        ["SelectLanguage"] = "Select interface language",
        ["RestartRequired"] = "A restart may be required for some changes to take effect",
        
        // About page
        ["AboutDescription"] = "Modern video player with support for many formats",
        ["Copyright"] = "© 2026 Insait",
        ["PoweredBy"] = "Video playback powered by LibVLC",
        
        // Tab
        ["NewTabTitle"] = "New tab",
        ["CloseTab"] = "Close tab",
        
        // User Agreement
        ["UserAgreement"] = "User Agreement",
        ["UserAgreementWindowTitle"] = "User Agreement",
        ["UserAgreementTitle"] = "Insait Video Player User Agreement",
        ["UserAgreementLastUpdated"] = "Last updated: January 14, 2026",
        ["UserAgreementSection1Title"] = "1. Disclaimer of Warranties",
        ["UserAgreementSection1Text"] = "This application is provided \"as is\", without any warranties, express or implied. The developer is not liable for any direct, indirect, incidental, or special damages arising from the use or inability to use this application.",
        ["UserAgreementSection2Title"] = "2. Privacy and Data Collection",
        ["UserAgreementSection2Text"] = "We value your privacy. This application does NOT collect, transmit, or store any personal data on external servers. All information (watch history, settings, playback positions) is stored exclusively locally on your device.",
        ["UserAgreementSection3Title"] = "3. Local Data Storage",
        ["UserAgreementSection3Text"] = "The application may store the following data on your computer:\n• Watch history\n• Playback positions for session restoration\n• Interface settings (language, volume, etc.)\n\nYou can delete this data at any time through the application settings or manually.",
        ["UserAgreementSection4Title"] = "4. Intellectual Property",
        ["UserAgreementSection4Text"] = "Insait Video Player uses the LibVLC library for media playback. LibVLC is free software distributed under the LGPL license.",
        ["UserAgreementSection5Title"] = "5. Acceptance of Terms",
        ["UserAgreementSection5Text"] = "By using this application, you agree to the terms of this agreement. If you do not agree with any terms, please stop using the application.",
        ["UserAgreementCopyright"] = "© 2026 Insait. All rights reserved.",
        ["UserAgreementAcceptButton"] = "Understood",
        
        // History
        ["HistoryWindowTitle"] = "Watch History",
        ["Search"] = "Search...",
        ["ClearHistory"] = "Clear",
        ["HistoryEmpty"] = "History is empty",
        ["HistoryCount"] = "{0} entries",
        ["DeleteFromHistory"] = "Delete from history",
        ["Refresh"] = "Refresh",
        
        // Session restore mock
        ["SessionRestoredTitle"] = "Session restored",
        ["PressPlayToStart"] = "Press the Play button to continue watching",
        ["Or"] = "or",
        
        // Overflow tabs window
        ["OverflowWindowTitle"] = "Additional tabs",
        ["OverflowTabsHeader"] = "Overflow tabs",
        ["NewTabButton"] = "New tab",
        ["ActivateTab"] = "Activate tab",
        ["MoveToMain"] = "Move to main window",
        ["CloseTabTooltip"] = "Close tab",
        ["NoFile"] = "No file",
        ["NoOverflowTabs"] = "No additional tabs",
        ["OverflowTabsCount"] = "{0} / {1} tabs",
        ["MaximumTabs"] = "Maximum: {0}",
        ["OverflowTabs"] = "Additional tabs",
        ["TabLimitReached"] = "Tab limit reached"
    };
    #endregion
    
    #region German Strings
    private static Dictionary<string, string> GetGermanStrings() => new()
    {
        // Window titles
        ["WindowTitle"] = "Insait Video Player",
        ["SettingsWindowTitle"] = "Einstellungen",
        ["ErrorWindowTitle"] = "Fehler",
        
        // Title bar buttons
        ["Settings"] = "Einstellungen",
        ["Tools"] = "Werkzeuge",
        ["OpenFile"] = "Datei öffnen",
        ["OpenFolder"] = "Ordner öffnen",
        ["NewTab"] = "Neuer Tab",
        ["NewWindow"] = "Neues Fenster",
        
        // Empty state
        ["OpenFileOrDragVideo"] = "Öffnen Sie eine Datei oder ziehen Sie ein Video hierher",
        ["SupportedFormats"] = "Unterstützte Formate:",
        ["OpenFileAction"] = "Datei öffnen",
        ["PlayPause"] = "Abspielen/Pause",
        ["Fullscreen"] = "Vollbild",
        
        // Transport controls
        ["Play"] = "Abspielen",
        ["Pause"] = "Pause",
        ["Stop"] = "Stopp",
        ["MuteSound"] = "Ton aus",
        ["UnmuteSound"] = "Ton ein",
        ["SelectAudioDevice"] = "Audiogerät auswählen",
        ["RefreshDevices"] = "Geräteliste aktualisieren",
        ["DefaultDevice"] = "Standard",
        
        // File picker
        ["PickVideoOrAudio"] = "Video oder Audio auswählen",
        ["MediaFiles"] = "Mediendateien",
        ["AllFiles"] = "Alle Dateien",
        ["PickFolderWithMedia"] = "Ordner mit Medien auswählen",
        
        // Errors
        ["FileNotFound"] = "Datei nicht gefunden",
        ["UnsupportedFormat"] = "Nicht unterstütztes Format",
        ["AudioError"] = "Audiofehler",
        ["AccessDenied"] = "Zugriff verweigert",
        ["NoPermission"] = "Keine Berechtigung",
        ["PlaybackError"] = "Wiedergabefehler",
        ["CannotPlayMedia"] = "Mediendatei kann nicht abgespielt werden. Das Format wird möglicherweise nicht unterstützt oder die Datei ist beschädigt.",
        ["CannotChangeAudioDevice"] = "Audiogerät kann nicht geändert werden.",
        ["FileExtension"] = "Dateierweiterung",
        ["SupportedFormatsDetails"] = "Unterstützte Formate: MP4, MKV, AVI, MOV, MP3, WAV, FLAC, WEBM, WMV",
        ["Device"] = "Gerät",
        ["Error"] = "Fehler",
        
        // Settings
        ["Language"] = "Sprache",
        ["Appearance"] = "Aussehen",
        ["About"] = "Über",
        ["Version"] = "Version",
        ["Save"] = "Speichern",
        ["Cancel"] = "Abbrechen",
        ["Apply"] = "Anwenden",
        ["General"] = "Allgemein",
        ["SelectLanguage"] = "Wählen Sie die Sprache der Benutzeroberfläche",
        ["RestartRequired"] = "Für einige Änderungen ist möglicherweise ein Neustart erforderlich",
        
        // About page
        ["AboutDescription"] = "Moderner Videoplayer mit Unterstützung für viele Formate",
        ["Copyright"] = "© 2026 Insait",
        ["PoweredBy"] = "Videowiedergabe powered by LibVLC",
        
        // Tab
        ["NewTabTitle"] = "Neuer Tab",
        ["CloseTab"] = "Tab schließen",
        
        // User Agreement
        ["UserAgreement"] = "Nutzungsvereinbarung",
        ["UserAgreementWindowTitle"] = "Nutzungsvereinbarung",
        ["UserAgreementTitle"] = "Insait Video Player Nutzungsvereinbarung",
        ["UserAgreementLastUpdated"] = "Letzte Aktualisierung: 14. Januar 2026",
        ["UserAgreementSection1Title"] = "1. Haftungsausschluss",
        ["UserAgreementSection1Text"] = "Diese Anwendung wird \"wie besehen\" bereitgestellt, ohne jegliche Garantien, weder ausdrücklich noch stillschweigend. Der Entwickler haftet nicht für direkte, indirekte, zufällige oder besondere Schäden, die aus der Nutzung oder der Unmöglichkeit der Nutzung dieser Anwendung entstehen.",
        ["UserAgreementSection2Title"] = "2. Datenschutz und Datenerfassung",
        ["UserAgreementSection2Text"] = "Wir schätzen Ihre Privatsphäre. Diese Anwendung sammelt, überträgt oder speichert KEINE persönlichen Daten auf externen Servern. Alle Informationen (Wiedergabeverlauf, Einstellungen, Wiedergabepositionen) werden ausschließlich lokal auf Ihrem Gerät gespeichert.",
        ["UserAgreementSection3Title"] = "3. Lokale Datenspeicherung",
        ["UserAgreementSection3Text"] = "Die Anwendung kann folgende Daten auf Ihrem Computer speichern:\n• Wiedergabeverlauf\n• Wiedergabepositionen für die Sitzungswiederherstellung\n• Oberflächeneinstellungen (Sprache, Lautstärke usw.)\n\nSie können diese Daten jederzeit über die Anwendungseinstellungen oder manuell löschen.",
        ["UserAgreementSection4Title"] = "4. Geistiges Eigentum",
        ["UserAgreementSection4Text"] = "Insait Video Player verwendet die LibVLC-Bibliothek für die Medienwiedergabe. LibVLC ist freie Software, die unter der LGPL-Lizenz vertrieben wird.",
        ["UserAgreementSection5Title"] = "5. Annahme der Bedingungen",
        ["UserAgreementSection5Text"] = "Durch die Nutzung dieser Anwendung stimmen Sie den Bedingungen dieser Vereinbarung zu. Wenn Sie mit irgendwelchen Bedingungen nicht einverstanden sind, beenden Sie bitte die Nutzung der Anwendung.",
        ["UserAgreementCopyright"] = "© 2026 Insait. Alle Rechte vorbehalten.",
        ["UserAgreementAcceptButton"] = "Verstanden",
        
        // History
        ["HistoryWindowTitle"] = "Wiedergabeverlauf",
        ["Search"] = "Suchen...",
        ["ClearHistory"] = "Löschen",
        ["HistoryEmpty"] = "Verlauf ist leer",
        ["HistoryCount"] = "{0} Einträge",
        ["DeleteFromHistory"] = "Aus Verlauf löschen",
        ["Refresh"] = "Aktualisieren",
        
        // Session restore mock
        ["SessionRestoredTitle"] = "Sitzung wiederhergestellt",
        ["PressPlayToStart"] = "Drücken Sie Play, um fortzufahren",
        ["Or"] = "oder",
        
        // Overflow tabs window
        ["OverflowWindowTitle"] = "Zusätzliche Tabs",
        ["OverflowTabsHeader"] = "Overflow-Tabs",
        ["NewTabButton"] = "Neuer Tab",
        ["ActivateTab"] = "Tab aktivieren",
        ["MoveToMain"] = "Zum Hauptfenster verschieben",
        ["CloseTabTooltip"] = "Tab schließen",
        ["NoFile"] = "Keine Datei",
        ["NoOverflowTabs"] = "Keine zusätzlichen Tabs",
        ["OverflowTabsCount"] = "{0} / {1} Tabs",
        ["MaximumTabs"] = "Maximum: {0}",
        ["OverflowTabs"] = "Zusätzliche Tabs",
        ["TabLimitReached"] = "Tab-Limit erreicht"
    };
    #endregion
    
    #region Russian Strings
    private static Dictionary<string, string> GetRussianStrings() => new()
    {
        // Window titles
        ["WindowTitle"] = "Insait Video Player",
        ["SettingsWindowTitle"] = "Настройки",
        ["ErrorWindowTitle"] = "Ошибка",
        
        // Title bar buttons
        ["Settings"] = "Настройки",
        ["Tools"] = "Инструменты",
        ["OpenFile"] = "Открыть файл",
        ["OpenFolder"] = "Открыть папку",
        ["NewTab"] = "Новая вкладка",
        ["NewWindow"] = "Новое окно",
        
        // Empty state
        ["OpenFileOrDragVideo"] = "Откройте файл или перетащите видео сюда",
        ["SupportedFormats"] = "Поддерживаемые форматы:",
        ["OpenFileAction"] = "Открыть файл",
        ["PlayPause"] = "Воспроизведение/Пауза",
        ["Fullscreen"] = "Полноэкранный режим",
        
        // Transport controls
        ["Play"] = "Воспроизвести",
        ["Pause"] = "Пауза",
        ["Stop"] = "Стоп",
        ["MuteSound"] = "Выключить звук",
        ["UnmuteSound"] = "Включить звук",
        ["SelectAudioDevice"] = "Выбор аудиоустройства",
        ["RefreshDevices"] = "Обновить список устройств",
        ["DefaultDevice"] = "По умолчанию",
        
        // File picker
        ["PickVideoOrAudio"] = "Выберите видео или аудио",
        ["MediaFiles"] = "Медиа файлы",
        ["AllFiles"] = "Все файлы",
        ["PickFolderWithMedia"] = "Выберите папку с медиа",
        
        // Errors
        ["FileNotFound"] = "Файл не найден",
        ["UnsupportedFormat"] = "Неподдерживаемый формат",
        ["AudioError"] = "Ошибка аудио",
        ["AccessDenied"] = "Доступ запрещён",
        ["NoPermission"] = "Нет прав доступа",
        ["PlaybackError"] = "Ошибка воспроизведения",
        ["CannotPlayMedia"] = "Не удалось воспроизвести медиа файл. Возможно, формат не поддерживается или файл повреждён.",
        ["CannotChangeAudioDevice"] = "Не удалось изменить аудиоустройство.",
        ["FileExtension"] = "Расширение файла",
        ["SupportedFormatsDetails"] = "Поддерживаемые форматы: MP4, MKV, AVI, MOV, MP3, WAV, FLAC, WEBM, WMV",
        ["Device"] = "Устройство",
        ["Error"] = "Ошибка",
        
        // Settings
        ["Language"] = "Язык",
        ["Appearance"] = "Внешний вид",
        ["About"] = "О программе",
        ["Version"] = "Версия",
        ["Save"] = "Сохранить",
        ["Cancel"] = "Отмена",
        ["Apply"] = "Применить",
        ["General"] = "Общие",
        ["SelectLanguage"] = "Выберите язык интерфейса",
        ["RestartRequired"] = "Для применения некоторых изменений может потребоваться перезапуск программы",
        
        // About page
        ["AboutDescription"] = "Современный видеоплеер с поддержкой множества форматов",
        ["Copyright"] = "© 2026 Insait",
        ["PoweredBy"] = "Воспроизведение видео на базе LibVLC",
        
        // Tab
        ["NewTabTitle"] = "Новая вкладка",
        ["CloseTab"] = "Закрыть вкладку",
        
        // User Agreement
        ["UserAgreement"] = "Пользовательское соглашение",
        ["UserAgreementWindowTitle"] = "Пользовательское соглашение",
        ["UserAgreementTitle"] = "Пользовательское соглашение Insait Video Player",
        ["UserAgreementLastUpdated"] = "Последнее обновление: 14 января 2026",
        ["UserAgreementSection1Title"] = "1. Отказ от гарантий",
        ["UserAgreementSection1Text"] = "Это приложение предоставляется «как есть» (as is), без каких-либо гарантий, явных или подразумеваемых. Разработчик не несёт ответственности за любые прямые, косвенные, случайные или особые убытки, возникающие в результате использования или невозможности использования этого приложения.",
        ["UserAgreementSection2Title"] = "2. Конфиденциальность и сбор данных",
        ["UserAgreementSection2Text"] = "Мы ценим вашу конфиденциальность. Это приложение НЕ собирает, НЕ передаёт и НЕ хранит никаких персональных данных на внешних серверах. Вся информация (история просмотра, настройки, позиции воспроизведения) хранится исключительно локально на вашем устройстве.",
        ["UserAgreementSection3Title"] = "3. Локальное хранение данных",
        ["UserAgreementSection3Text"] = "Приложение может хранить на вашем компьютере следующие данные:\n• История просмотренных файлов\n• Позиции воспроизведения для восстановления сессии\n• Настройки интерфейса (язык, громкость и т.д.)\n\nВы можете в любой момент удалить эти данные через настройки программы или вручную.",
        ["UserAgreementSection4Title"] = "4. Интеллектуальная собственность",
        ["UserAgreementSection4Text"] = "Insait Video Player использует библиотеку LibVLC для воспроизведения медиа. LibVLC является свободным программным обеспечением, распространяемым под лицензией LGPL.",
        ["UserAgreementSection5Title"] = "5. Принятие условий",
        ["UserAgreementSection5Text"] = "Используя это приложение, вы соглашаетесь с условиями этого соглашения. Если вы не согласны с какими-либо условиями, пожалуйста, прекратите использование приложения.",
        ["UserAgreementCopyright"] = "© 2026 Insait. Все права защищены.",
        ["UserAgreementAcceptButton"] = "Понятно",
        
        // History
        ["HistoryWindowTitle"] = "История просмотра",
        ["Search"] = "Поиск...",
        ["ClearHistory"] = "Очистить",
        ["HistoryEmpty"] = "История пуста",
        ["HistoryCount"] = "{0} записей",
        ["DeleteFromHistory"] = "Удалить из истории",
        ["Refresh"] = "Обновить",
        
        // Session restore mock
        ["SessionRestoredTitle"] = "Сессия восстановлена",
        ["PressPlayToStart"] = "Нажмите Play, чтобы продолжить просмотр",
        ["Or"] = "или",
        
        // Overflow tabs window
        ["OverflowWindowTitle"] = "Дополнительные вкладки",
        ["OverflowTabsHeader"] = "Вкладки overflow",
        ["NewTabButton"] = "Новая вкладка",
        ["ActivateTab"] = "Активировать вкладку",
        ["MoveToMain"] = "Переместить в главное окно",
        ["CloseTabTooltip"] = "Закрыть вкладку",
        ["NoFile"] = "Нет файла",
        ["NoOverflowTabs"] = "Нет дополнительных вкладок",
        ["OverflowTabsCount"] = "{0} / {1} вкладок",
        ["MaximumTabs"] = "Максимум: {0}",
        ["OverflowTabs"] = "Дополнительные вкладки",
        ["TabLimitReached"] = "Достигнут лимит вкладок"
    };
    #endregion
    
    #region Turkish Strings
    private static Dictionary<string, string> GetTurkishStrings() => new()
    {
        // Window titles
        ["WindowTitle"] = "Insait Video Player",
        ["SettingsWindowTitle"] = "Ayarlar",
        ["ErrorWindowTitle"] = "Hata",
        
        // Title bar buttons
        ["Settings"] = "Ayarlar",
        ["Tools"] = "Araçlar",
        ["OpenFile"] = "Dosya aç",
        ["OpenFolder"] = "Klasör aç",
        ["NewTab"] = "Yeni sekme",
        ["NewWindow"] = "Yeni pencere",
        
        // Empty state
        ["OpenFileOrDragVideo"] = "Bir dosya açın veya videoyu buraya sürükleyin",
        ["SupportedFormats"] = "Desteklenen formatlar:",
        ["OpenFileAction"] = "Dosya aç",
        ["PlayPause"] = "Oynat/Duraklat",
        ["Fullscreen"] = "Tam ekran",
        
        // Transport controls
        ["Play"] = "Oynat",
        ["Pause"] = "Duraklat",
        ["Stop"] = "Durdur",
        ["MuteSound"] = "Sesi kapat",
        ["UnmuteSound"] = "Sesi aç",
        ["SelectAudioDevice"] = "Ses cihazı seç",
        ["RefreshDevices"] = "Cihaz listesini yenile",
        ["DefaultDevice"] = "Varsayılan",
        
        // File picker
        ["PickVideoOrAudio"] = "Video veya ses dosyası seçin",
        ["MediaFiles"] = "Medya dosyaları",
        ["AllFiles"] = "Tüm dosyalar",
        ["PickFolderWithMedia"] = "Medya içeren klasör seçin",
        
        // Errors
        ["FileNotFound"] = "Dosya bulunamadı",
        ["UnsupportedFormat"] = "Desteklenmeyen format",
        ["AudioError"] = "Ses hatası",
        ["AccessDenied"] = "Erişim engellendi",
        ["NoPermission"] = "İzin yok",
        ["PlaybackError"] = "Oynatma hatası",
        ["CannotPlayMedia"] = "Medya dosyası oynatılamıyor. Format desteklenmiyor olabilir veya dosya bozuk.",
        ["CannotChangeAudioDevice"] = "Ses cihazı değiştirilemiyor.",
        ["FileExtension"] = "Dosya uzantısı",
        ["SupportedFormatsDetails"] = "Desteklenen formatlar: MP4, MKV, AVI, MOV, MP3, WAV, FLAC, WEBM, WMV",
        ["Device"] = "Cihaz",
        ["Error"] = "Hata",
        
        // Settings
        ["Language"] = "Dil",
        ["Appearance"] = "Görünüm",
        ["About"] = "Hakkında",
        ["Version"] = "Sürüm",
        ["Save"] = "Kaydet",
        ["Cancel"] = "İptal",
        ["Apply"] = "Uygula",
        ["General"] = "Genel",
        ["SelectLanguage"] = "Arayüz dilini seçin",
        ["RestartRequired"] = "Bazı değişikliklerin uygulanması için yeniden başlatma gerekebilir",
        
        // About page
        ["AboutDescription"] = "Birçok formatı destekleyen modern video oynatıcı",
        ["Copyright"] = "© 2026 Insait",
        ["PoweredBy"] = "LibVLC ile video oynatma",
        
        // Tab
        ["NewTabTitle"] = "Yeni sekme",
        ["CloseTab"] = "Sekmeyi kapat",
        
        // User Agreement
        ["UserAgreement"] = "Kullanıcı Sözleşmesi",
        ["UserAgreementWindowTitle"] = "Kullanıcı Sözleşmesi",
        ["UserAgreementTitle"] = "Insait Video Player Kullanıcı Sözleşmesi",
        ["UserAgreementLastUpdated"] = "Son güncelleme: 14 Ocak 2026",
        ["UserAgreementSection1Title"] = "1. Garanti Reddi",
        ["UserAgreementSection1Text"] = "Bu uygulama, açık veya zımni hiçbir garanti olmaksızın \"olduğu gibi\" sağlanmaktadır. Geliştirici, bu uygulamanın kullanımından veya kullanılamamasından kaynaklanan doğrudan, dolaylı, tesadüfi veya özel zararlardan sorumlu değildir.",
        ["UserAgreementSection2Title"] = "2. Gizlilik ve Veri Toplama",
        ["UserAgreementSection2Text"] = "Gizliliğinize değer veriyoruz. Bu uygulama harici sunucularda hiçbir kişisel veri toplamaz, iletmez veya saklamaz. Tüm bilgiler (izleme geçmişi, ayarlar, oynatma konumları) yalnızca cihazınızda yerel olarak saklanır.",
        ["UserAgreementSection3Title"] = "3. Yerel Veri Depolama",
        ["UserAgreementSection3Text"] = "Uygulama bilgisayarınızda aşağıdaki verileri saklayabilir:\n• İzleme geçmişi\n• Oturum geri yükleme için oynatma konumları\n• Arayüz ayarları (dil, ses seviyesi vb.)\n\nBu verileri istediğiniz zaman uygulama ayarlarından veya manuel olarak silebilirsiniz.",
        ["UserAgreementSection4Title"] = "4. Fikri Mülkiyet",
        ["UserAgreementSection4Text"] = "Insait Video Player, medya oynatma için LibVLC kütüphanesini kullanır. LibVLC, LGPL lisansı altında dağıtılan özgür bir yazılımdır.",
        ["UserAgreementSection5Title"] = "5. Koşulların Kabulü",
        ["UserAgreementSection5Text"] = "Bu uygulamayı kullanarak, bu sözleşmenin koşullarını kabul etmiş olursunuz. Herhangi bir koşulu kabul etmiyorsanız, lütfen uygulamayı kullanmayı bırakın.",
        ["UserAgreementCopyright"] = "© 2026 Insait. Tüm hakları saklıdır.",
        ["UserAgreementAcceptButton"] = "Anladım",
        
        // History
        ["HistoryWindowTitle"] = "İzleme Geçmişi",
        ["Search"] = "Ara...",
        ["ClearHistory"] = "Temizle",
        ["HistoryEmpty"] = "Geçmiş boş",
        ["HistoryCount"] = "{0} kayıt",
        ["DeleteFromHistory"] = "Geçmişten sil",
        ["Refresh"] = "Yenile",
        
        // Session restore mock
        ["SessionRestoredTitle"] = "Oturum geri yüklendi",
        ["PressPlayToStart"] = "Devam etmek için Play düğmesine basın",
        ["Or"] = "veya",
        
        // Overflow tabs window
        ["OverflowWindowTitle"] = "Ek sekmeler",
        ["OverflowTabsHeader"] = "Overflow sekmeleri",
        ["NewTabButton"] = "Yeni sekme",
        ["ActivateTab"] = "Sekmeyi etkinleştir",
        ["MoveToMain"] = "Ana pencereye taşı",
        ["CloseTabTooltip"] = "Sekmeyi kapat",
        ["NoFile"] = "Dosya yok",
        ["NoOverflowTabs"] = "Ek sekme yok",
        ["OverflowTabsCount"] = "{0} / {1} sekme",
        ["MaximumTabs"] = "Maksimum: {0}",
        ["OverflowTabs"] = "Ek sekmeler",
        ["TabLimitReached"] = "Sekme limitine ulaşıldı"
    };
    #endregion
}

/// <summary>
/// Represents a language item for display in UI
/// </summary>
public class LanguageItem
{
    public Language Language { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    
    public override string ToString() => DisplayName;
}

