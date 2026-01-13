using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LiteDB;

namespace Insait_Video_Player;

/// <summary>
/// Represents a saved tab state for persistence
/// </summary>
public class SavedTabState
{
    public int Id { get; set; }
    public string? FilePath { get; set; }
    public string Title { get; set; } = "Новий таб";
    // Removed player state fields to prevent issues
    public int TabOrder { get; set; }
}

/// <summary>
/// Represents a session document in LiteDB
/// </summary>
public class SessionDocument
{
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();
    public string SessionId { get; set; } = "default";
    public List<SavedTabState> Tabs { get; set; } = new();
    public int ActiveTabOrder { get; set; }
    public int Volume { get; set; } = 80;
    public DateTime LastSaved { get; set; }
    public string? SelectedAudioDeviceId { get; set; }
    public string? SelectedLanguageCode { get; set; }
}

/// <summary>
/// Represents a history item in LiteDB
/// </summary>
public class HistoryItem
{
    public int Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime WatchedAt { get; set; }
    public float LastPosition { get; set; }
    public long LastTime { get; set; }
}

/// <summary>
/// Manages session persistence with DPAPI encryption using LiteDB
/// </summary>
public class SessionManager : IDisposable
{
    private LiteDatabase? _database;
    private ILiteCollection<SessionDocument>? _sessions;
    private ILiteCollection<HistoryItem>? _history;
    private readonly string _appFolder;
    private readonly string _databasePath;
    private readonly string _keyFilePath;
    private byte[]? _encryptionKey;
    private bool _disposed;
    
    // IV size for AES
    private const int IvSize = 16;
    private const int KeySize = 32; // AES-256
    private const int MaxHistoryItems = 100; // Maximum history entries

    public SessionManager()
    {
        // Get AppData Local folder path for Microsoft Store compatibility
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _appFolder = Path.Combine(appDataPath, "InsaitVideoPlayer");
        _databasePath = Path.Combine(_appFolder, "sessions.db");
        _keyFilePath = Path.Combine(_appFolder, "session.key");
        
        InitializeDatabase();
    }
    
    /// <summary>
    /// Initializes or reinitializes the database, handling corrupted files
    /// </summary>
    private void InitializeDatabase(bool isRetry = false)
    {
        try
        {
            // Create folder if not exists
            if (!Directory.Exists(_appFolder))
            {
                Directory.CreateDirectory(_appFolder);
            }
            
            // Get or create protected encryption key
            _encryptionKey = GetOrCreateProtectedKey();
            
            if (_encryptionKey == null)
            {
                throw new InvalidOperationException("Failed to get encryption key");
            }
            
            // Generate connection string with encryption password
            var password = Convert.ToBase64String(_encryptionKey);
            var connectionString = new ConnectionString
            {
                Filename = _databasePath,
                Password = password,
                Connection = ConnectionType.Shared
            };
            
            _database = new LiteDatabase(connectionString);
            _sessions = _database.GetCollection<SessionDocument>("sessions");
            _history = _database.GetCollection<HistoryItem>("history");
            
            // Create index on SessionId for faster queries
            _sessions.EnsureIndex(x => x.SessionId);
            _history.EnsureIndex(x => x.FilePath);
            _history.EnsureIndex(x => x.WatchedAt);
            
            // Test database access
            _ = _sessions.Count();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing SessionManager: {ex.Message}");
            
            // If this is not a retry, try to recreate the database
            if (!isRetry)
            {
                RecreateDatabase();
            }
            else
            {
                // Database initialization failed completely - session persistence will be disabled
                _database = null;
                _sessions = null;
                _history = null;
            }
        }
    }
    
    /// <summary>
    /// Recreates the database and key files when they are corrupted
    /// </summary>
    private void RecreateDatabase()
    {
        System.Diagnostics.Debug.WriteLine("Attempting to recreate database and key...");
        
        try
        {
            // Dispose current database if any
            try
            {
                _database?.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }
            
            _database = null;
            _sessions = null;
            _history = null;
            
            // Delete corrupted files
            if (File.Exists(_databasePath))
            {
                try
                {
                    File.Delete(_databasePath);
                    System.Diagnostics.Debug.WriteLine("Deleted corrupted database file");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to delete database: {ex.Message}");
                }
            }
            
            // Also delete any LiteDB journal files
            var journalPath = _databasePath + "-journal";
            if (File.Exists(journalPath))
            {
                try
                {
                    File.Delete(journalPath);
                }
                catch
                {
                    // Ignore
                }
            }
            
            var logPath = _databasePath + "-log";
            if (File.Exists(logPath))
            {
                try
                {
                    File.Delete(logPath);
                }
                catch
                {
                    // Ignore
                }
            }
            
            if (File.Exists(_keyFilePath))
            {
                try
                {
                    File.Delete(_keyFilePath);
                    System.Diagnostics.Debug.WriteLine("Deleted corrupted key file");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to delete key: {ex.Message}");
                }
            }
            
            // Clear encryption key
            if (_encryptionKey != null)
            {
                Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
                _encryptionKey = null;
            }
            
            // Try to initialize again
            InitializeDatabase(isRetry: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to recreate database: {ex.Message}");
            _database = null;
            _sessions = null;
            _history = null;
        }
    }

    /// <summary>
    /// Gets or creates a protected encryption key using Windows DPAPI
    /// </summary>
    private byte[]? GetOrCreateProtectedKey()
    {
        try
        {
            // DPAPI is Windows-only, check platform
            if (!OperatingSystem.IsWindows())
            {
                return GetFallbackKey();
            }
            
            if (File.Exists(_keyFilePath))
            {
                try
                {
                    // Read and unprotect existing key
                    var protectedKey = File.ReadAllBytes(_keyFilePath);
                    
                    // Validate key file size
                    if (protectedKey.Length == 0)
                    {
                        throw new InvalidOperationException("Key file is empty");
                    }
                    
                    return ProtectedData.Unprotect(protectedKey, null, DataProtectionScope.CurrentUser);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to read key file: {ex.Message}");
                    
                    // Key file is corrupted, delete it and create new
                    try
                    {
                        File.Delete(_keyFilePath);
                    }
                    catch
                    {
                        // Ignore deletion error
                    }
                    
                    // Fall through to create new key
                }
            }
            
            // Generate new random key
            var newKey = new byte[KeySize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(newKey);
            }
            
            // Protect and save the key using DPAPI (tied to current Windows user)
            var protectedNewKey = ProtectedData.Protect(newKey, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(_keyFilePath, protectedNewKey);
            
            return newKey;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error with DPAPI key: {ex.Message}");
            return GetFallbackKey();
        }
    }
    
    /// <summary>
    /// Gets a fallback key based on machine-specific data (less secure but works on all platforms)
    /// </summary>
    private static byte[] GetFallbackKey()
    {
        var fallbackKey = new byte[KeySize];
        var machineData = Encoding.UTF8.GetBytes(
            Environment.MachineName + 
            Environment.UserName + 
            "InsaitVideoPlayer2026");
        
        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(machineData);
            Array.Copy(hash, fallbackKey, KeySize);
        }
        
        return fallbackKey;
    }

    /// <summary>
    /// Saves the current session state
    /// </summary>
    public void SaveSession(IEnumerable<TabInstance> tabs, TabInstance? activeTab, int volume, string? audioDeviceId, string? languageCode = null)
    {
        if (_sessions == null || _encryptionKey == null) return;

        try
        {
            var tabsList = tabs.ToList();
            var savedTabs = new List<SavedTabState>();
            int activeTabOrder = 0;

            for (int i = 0; i < tabsList.Count; i++)
            {
                var tab = tabsList[i];
                
                // Encrypt file path if exists
                var encryptedPath = !string.IsNullOrEmpty(tab.FilePath) 
                    ? EncryptString(tab.FilePath) 
                    : null;

                savedTabs.Add(new SavedTabState
                {
                    Id = tab.Id,
                    FilePath = encryptedPath,
                    Title = tab.Title,
                    TabOrder = i
                });

                if (tab == activeTab)
                {
                    activeTabOrder = i;
                }
            }

            var session = new SessionDocument
            {
                SessionId = "default",
                Tabs = savedTabs,
                ActiveTabOrder = activeTabOrder,
                Volume = volume,
                SelectedAudioDeviceId = audioDeviceId,
                SelectedLanguageCode = languageCode,
                LastSaved = DateTime.UtcNow
            };

            // Remove existing session and insert new one
            _sessions.DeleteMany(x => x.SessionId == "default");
            _sessions.Insert(session);
        }
        catch (LiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database error saving session: {ex.Message}");
            // Try to recreate database on corruption
            RecreateDatabase();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving session: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the saved session state
    /// </summary>
    public SessionDocument? LoadSession()
    {
        if (_sessions == null || _encryptionKey == null) return null;

        try
        {
            var session = _sessions.FindOne(x => x.SessionId == "default");
            
            if (session != null)
            {
                // Decrypt file paths
                foreach (var tab in session.Tabs)
                {
                    if (!string.IsNullOrEmpty(tab.FilePath))
                    {
                        tab.FilePath = DecryptString(tab.FilePath);
                    }
                }
            }

            return session;
        }
        catch (LiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database error loading session: {ex.Message}");
            // Try to recreate database on corruption
            RecreateDatabase();
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading session: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Clears all saved sessions
    /// </summary>
    public void ClearSession()
    {
        if (_sessions == null) return;

        try
        {
            _sessions.DeleteMany(x => x.SessionId == "default");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing session: {ex.Message}");
        }
    }

    #region History Management
    
    /// <summary>
    /// Adds or updates a history entry for the given file
    /// </summary>
    public void AddToHistory(string filePath, float position = 0, long time = 0)
    {
        if (_history == null || _encryptionKey == null || string.IsNullOrEmpty(filePath)) return;

        try
        {
            var encryptedPath = EncryptString(filePath);
            
            // Check if file already exists in history
            var existing = _history.FindOne(x => x.FilePath == encryptedPath);
            
            if (existing != null)
            {
                // Update existing entry
                existing.WatchedAt = DateTime.Now;
                existing.LastPosition = position;
                existing.LastTime = time;
                _history.Update(existing);
            }
            else
            {
                // Add new entry
                var item = new HistoryItem
                {
                    FilePath = encryptedPath,
                    WatchedAt = DateTime.Now,
                    LastPosition = position,
                    LastTime = time
                };
                _history.Insert(item);
                
                // Trim old entries if exceeded max
                TrimHistoryIfNeeded();
            }
        }
        catch (LiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database error adding to history: {ex.Message}");
            RecreateDatabase();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding to history: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Gets all history items
    /// </summary>
    public List<HistoryItem> GetHistory()
    {
        if (_history == null || _encryptionKey == null) return new List<HistoryItem>();

        try
        {
            var items = _history.FindAll().ToList();
            
            // Decrypt file paths
            foreach (var item in items)
            {
                item.FilePath = DecryptString(item.FilePath);
            }
            
            return items.Where(i => !string.IsNullOrEmpty(i.FilePath)).ToList();
        }
        catch (LiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database error getting history: {ex.Message}");
            RecreateDatabase();
            return new List<HistoryItem>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting history: {ex.Message}");
            return new List<HistoryItem>();
        }
    }
    
    /// <summary>
    /// Deletes a history item by ID
    /// </summary>
    public void DeleteHistoryItem(int id)
    {
        if (_history == null) return;

        try
        {
            _history.Delete(id);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting history item: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Clears all history
    /// </summary>
    public void ClearHistory()
    {
        if (_history == null) return;

        try
        {
            _history.DeleteAll();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing history: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Trims history to maximum allowed items
    /// </summary>
    private void TrimHistoryIfNeeded()
    {
        if (_history == null) return;

        try
        {
            var count = _history.Count();
            if (count > MaxHistoryItems)
            {
                // Get oldest items to delete
                var toDelete = _history.FindAll()
                    .OrderBy(x => x.WatchedAt)
                    .Take(count - MaxHistoryItems)
                    .Select(x => x.Id)
                    .ToList();
                
                foreach (var id in toDelete)
                {
                    _history.Delete(id);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error trimming history: {ex.Message}");
        }
    }
    
    #endregion

    /// <summary>
    /// Encrypts a string using AES-256 with DPAPI-protected key
    /// </summary>
    private string EncryptString(string plainText)
    {
        if (string.IsNullOrEmpty(plainText) || _encryptionKey == null)
            return string.Empty;

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Combine IV and encrypted data
        var result = new byte[IvSize + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, IvSize);
        Buffer.BlockCopy(encryptedBytes, 0, result, IvSize, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts a string using AES-256 with DPAPI-protected key
    /// </summary>
    private string DecryptString(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText) || _encryptionKey == null)
            return string.Empty;

        try
        {
            var fullCipher = Convert.FromBase64String(encryptedText);

            // Extract IV and cipher text
            var iv = new byte[IvSize];
            var cipher = new byte[fullCipher.Length - IvSize];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, IvSize);
            Buffer.BlockCopy(fullCipher, IvSize, cipher, 0, cipher.Length);

            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            // If decryption fails, return empty string (might be old unencrypted data)
            return string.Empty;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        // Clear sensitive data from memory
        if (_encryptionKey != null)
        {
            Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
        }
        
        _database?.Dispose();
    }
}

