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
    private Language _currentLanguage = Language.Ukrainian;
    
    public event EventHandler? LanguageChanged;
    
    public Language CurrentLanguage => _currentLanguage;
    
    public static string GetLanguageCode(Language language) => language switch
    {
        Language.Ukrainian => "uk-UA",
        Language.English => "en-US",
        Language.German => "de-DE",
        Language.Russian => "ru-RU",
        Language.Turkish => "tr-TR",
        _ => "uk-UA"
    };
    
    public static string GetLanguageDisplayName(Language language) => language switch
    {
        Language.Ukrainian => "Українська",
        Language.English => "English",
        Language.German => "Deutsch",
        Language.Russian => "Русский",
        Language.Turkish => "Türkçe",
        _ => "Українська"
    };
    
    public static Language GetLanguageFromCode(string code) => code switch
    {
        "uk-UA" or "uk" => Language.Ukrainian,
        "en-US" or "en" => Language.English,
        "de-DE" or "de" => Language.German,
        "ru-RU" or "ru" => Language.Russian,
        "tr-TR" or "tr" => Language.Turkish,
        _ => Language.Ukrainian
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
        ["Or"] = "або"
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
        
        // Tab
        ["NewTabTitle"] = "New tab",
        ["CloseTab"] = "Close tab",
        
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
        ["Or"] = "or"
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
        
        // Tab
        ["NewTabTitle"] = "Neuer Tab",
        ["CloseTab"] = "Tab schließen",
        
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
        ["Or"] = "oder"
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
        
        // Tab
        ["NewTabTitle"] = "Новая вкладка",
        ["CloseTab"] = "Закрыть вкладку",
        
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
        ["Or"] = "или"
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
        
        // Tab
        ["NewTabTitle"] = "Yeni sekme",
        ["CloseTab"] = "Sekmeyi kapat",
        
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
        ["Or"] = "veya"
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

