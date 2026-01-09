# Скрипт для реєстрації Insait Video Player як додатку за замовчуванням для відео файлів
# ВАЖЛИВО: Запустіть цей скрипт від імені Адміністратора!

param(
    [string]$ExePath = "",
    [switch]$Unregister
)

# Якщо шлях не вказано, спробуємо знайти exe автоматично
if ([string]::IsNullOrEmpty($ExePath)) {
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    $possiblePaths = @(
        "$scriptDir\Insait Video Player\bin\Debug\net10.0\Insait Video Player.exe",
        "$scriptDir\Insait Video Player\bin\Release\net10.0\Insait Video Player.exe",
        "$scriptDir\Insait Video Player\bin\Debug\net9.0\Insait Video Player.exe",
        "$scriptDir\Insait Video Player\bin\Release\net9.0\Insait Video Player.exe"
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $ExePath = $path
            break
        }
    }
}

if ([string]::IsNullOrEmpty($ExePath) -or -not (Test-Path $ExePath)) {
    Write-Host "ПОМИЛКА: Не знайдено виконуваний файл Insait Video Player.exe" -ForegroundColor Red
    Write-Host "Вкажіть шлях до exe файлу параметром -ExePath" -ForegroundColor Yellow
    Write-Host "Приклад: .\RegisterFileAssociations.ps1 -ExePath 'C:\path\to\Insait Video Player.exe'" -ForegroundColor Yellow
    exit 1
}

$ExePath = (Resolve-Path $ExePath).Path
Write-Host "Використовується: $ExePath" -ForegroundColor Cyan

# Список відео розширень для реєстрації
$videoExtensions = @(
    ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".webm", ".flv", ".m4v", ".mpeg", ".mpg",
    ".3gp", ".3g2", ".ts", ".mts", ".m2ts", ".vob", ".ogv", ".divx", ".xvid"
)

$appName = "InsaitVideoPlayer"
$appDescription = "Insait Video Player"
$progId = "InsaitVideoPlayer.VideoFile"

function Test-Admin {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Register-Application {
    Write-Host "Реєстрація додатку..." -ForegroundColor Green
    
    # Реєструємо додаток в App Paths
    $appPathKey = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Insait Video Player.exe"
    New-Item -Path $appPathKey -Force | Out-Null
    Set-ItemProperty -Path $appPathKey -Name "(Default)" -Value $ExePath
    Set-ItemProperty -Path $appPathKey -Name "Path" -Value (Split-Path $ExePath)
    
    # Реєструємо ProgId
    $progIdKey = "HKLM:\SOFTWARE\Classes\$progId"
    New-Item -Path $progIdKey -Force | Out-Null
    Set-ItemProperty -Path $progIdKey -Name "(Default)" -Value $appDescription
    
    # Іконка
    $iconKey = "$progIdKey\DefaultIcon"
    New-Item -Path $iconKey -Force | Out-Null
    Set-ItemProperty -Path $iconKey -Name "(Default)" -Value "`"$ExePath`",0"
    
    # Команда відкриття
    $commandKey = "$progIdKey\shell\open\command"
    New-Item -Path $commandKey -Force | Out-Null
    Set-ItemProperty -Path $commandKey -Name "(Default)" -Value "`"$ExePath`" `"%1`""
    
    # Дружня назва для shell
    $shellKey = "$progIdKey\shell\open"
    Set-ItemProperty -Path $shellKey -Name "FriendlyAppName" -Value $appDescription
    
    Write-Host "Реєстрація розширень файлів..." -ForegroundColor Green
    
    foreach ($ext in $videoExtensions) {
        $extKey = "HKLM:\SOFTWARE\Classes\$ext"
        
        # Створюємо ключ розширення якщо не існує
        if (-not (Test-Path $extKey)) {
            New-Item -Path $extKey -Force | Out-Null
        }
        
        # Додаємо OpenWithProgids
        $openWithKey = "$extKey\OpenWithProgids"
        New-Item -Path $openWithKey -Force | Out-Null
        Set-ItemProperty -Path $openWithKey -Name $progId -Value ([byte[]]@())
        
        Write-Host "  Зареєстровано: $ext" -ForegroundColor Gray
    }
    
    # Реєструємо в RegisteredApplications
    $regAppsKey = "HKLM:\SOFTWARE\RegisteredApplications"
    Set-ItemProperty -Path $regAppsKey -Name $appName -Value "SOFTWARE\Clients\Media\$appName\Capabilities"
    
    # Створюємо Capabilities
    $capabilitiesKey = "HKLM:\SOFTWARE\Clients\Media\$appName\Capabilities"
    New-Item -Path $capabilitiesKey -Force | Out-Null
    Set-ItemProperty -Path $capabilitiesKey -Name "ApplicationName" -Value $appDescription
    Set-ItemProperty -Path $capabilitiesKey -Name "ApplicationDescription" -Value "Сучасний відео плеєр для Windows"
    
    # FileAssociations
    $fileAssocKey = "$capabilitiesKey\FileAssociations"
    New-Item -Path $fileAssocKey -Force | Out-Null
    
    foreach ($ext in $videoExtensions) {
        Set-ItemProperty -Path $fileAssocKey -Name $ext -Value $progId
    }
    
    Write-Host "`nУСПІХ! Insait Video Player зареєстровано." -ForegroundColor Green
    Write-Host "Тепер ви можете встановити його як додаток за замовчуванням:" -ForegroundColor Yellow
    Write-Host "  1. Відкрийте Параметри Windows (Win+I)" -ForegroundColor White
    Write-Host "  2. Перейдіть: Додатки -> Додатки за замовчуванням" -ForegroundColor White
    Write-Host "  3. Знайдіть 'Insait Video Player' і виберіть потрібні розширення" -ForegroundColor White
}

function Unregister-Application {
    Write-Host "Видалення реєстрації додатку..." -ForegroundColor Yellow
    
    # Видаляємо App Paths
    $appPathKey = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Insait Video Player.exe"
    if (Test-Path $appPathKey) {
        Remove-Item -Path $appPathKey -Recurse -Force
    }
    
    # Видаляємо ProgId
    $progIdKey = "HKLM:\SOFTWARE\Classes\$progId"
    if (Test-Path $progIdKey) {
        Remove-Item -Path $progIdKey -Recurse -Force
    }
    
    # Видаляємо OpenWithProgids з розширень
    foreach ($ext in $videoExtensions) {
        $openWithKey = "HKLM:\SOFTWARE\Classes\$ext\OpenWithProgids"
        if (Test-Path $openWithKey) {
            Remove-ItemProperty -Path $openWithKey -Name $progId -ErrorAction SilentlyContinue
        }
    }
    
    # Видаляємо з RegisteredApplications
    $regAppsKey = "HKLM:\SOFTWARE\RegisteredApplications"
    Remove-ItemProperty -Path $regAppsKey -Name $appName -ErrorAction SilentlyContinue
    
    # Видаляємо Capabilities
    $clientKey = "HKLM:\SOFTWARE\Clients\Media\$appName"
    if (Test-Path $clientKey) {
        Remove-Item -Path $clientKey -Recurse -Force
    }
    
    Write-Host "Реєстрацію видалено." -ForegroundColor Green
}

# Перевіряємо права адміністратора
if (-not (Test-Admin)) {
    Write-Host "ПОМИЛКА: Цей скрипт потребує прав адміністратора!" -ForegroundColor Red
    Write-Host "Клікніть правою кнопкою на PowerShell і виберіть 'Запустити від імені адміністратора'" -ForegroundColor Yellow
    exit 1
}

# Виконуємо реєстрацію або видалення
if ($Unregister) {
    Unregister-Application
} else {
    Register-Application
}

# Оновлюємо системний кеш
Write-Host "`nОновлення системного кешу..." -ForegroundColor Cyan
$code = @"
[System.Runtime.InteropServices.DllImport("shell32.dll")]
public static extern void SHChangeNotify(int eventId, int flags, System.IntPtr item1, System.IntPtr item2);
"@
Add-Type -MemberDefinition $code -Namespace Win32 -Name Shell32
[Win32.Shell32]::SHChangeNotify(0x08000000, 0, [System.IntPtr]::Zero, [System.IntPtr]::Zero)

Write-Host "Готово!" -ForegroundColor Green

