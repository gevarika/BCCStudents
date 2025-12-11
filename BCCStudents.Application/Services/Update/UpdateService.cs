using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

using BCCStudents.Application.Interfaces;

namespace BCCStudents.Application.Services.Update
{
    public class UpdateService : IUpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _manifestUrl;
        private readonly string _logPath;

        public UpdateService()
        {
            _httpClient = new HttpClient();
            _manifestUrl = System.Configuration.ConfigurationManager.AppSettings["UpdateManifestUrl"];
            try
            {
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
                Directory.CreateDirectory(logDir);
                _logPath = Path.Combine(logDir, "Update.txt");
            }
            catch { }
        }

        public Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public async Task<UpdateManifest> GetManifestAsync(CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_manifestUrl)) return null;
            try
            {
                Log($"GET manifest: {_manifestUrl}");
                using (var req = new HttpRequestMessage(HttpMethod.Get, _manifestUrl))
                using (var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        Log($"Manifest HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");
                        return null;
                    }
                    var json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Log($"Manifest OK, {json?.Length ?? 0} bytes");
                    return JsonConvert.DeserializeObject<UpdateManifest>(json);
                }
            }
            catch (Exception ex)
            {
                Log($"Manifest error: {ex.Message}");
                return null;
            }
        }

        public bool IsNewer(Version latest, Version current)
        {
            if (latest == null || current == null) return false;
            return latest > current;
        }

        public bool IsUpdateRequired(Version latest, Version current)
        {
            if (latest == null || current == null) return false;
            
            // áƒ›áƒáƒŸáƒáƒ  áƒ•áƒ”áƒ áƒ¡áƒ˜áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ˜áƒ¡áƒáƒ¡ áƒáƒ£áƒªáƒ˜áƒšáƒ”áƒ‘áƒ”áƒšáƒ˜áƒ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
            if (latest.Major > current.Major) return true;
            
            // áƒ›áƒ˜áƒœáƒáƒ  áƒ•áƒ”áƒ áƒ¡áƒ˜áƒ˜áƒ¡ áƒ¨áƒ”áƒªáƒ•áƒšáƒ˜áƒ¡áƒáƒ¡ áƒáƒ£áƒªáƒ˜áƒšáƒ”áƒ‘áƒ”áƒšáƒ˜áƒ áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ
            if (latest.Major == current.Major && latest.Minor > current.Minor) return true;
            
            // Build áƒ“áƒ Revision áƒªáƒ•áƒšáƒ˜áƒšáƒ”áƒ‘áƒ”áƒ‘áƒ˜ áƒáƒ  áƒáƒ áƒ˜áƒ¡ áƒáƒ£áƒªáƒ˜áƒšáƒ”áƒ‘áƒ”áƒšáƒ˜
            return false;
        }

        public async Task<string> DownloadAsync(UpdateManifest manifest, IProgress<(long current, long total)> progress, CancellationToken ct)
        {
            var url = manifest?.package?.url;
            if (string.IsNullOrWhiteSpace(url)) return null;

            var tempDir = Path.Combine(Path.GetTempPath(), "BCCStudents", "updates", manifest.latestVersion);
            Directory.CreateDirectory(tempDir);
            var zipPath = Path.Combine(tempDir, "update.zip");

            Log($"Download: {url}");
            using (var resp = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
            {
                if (!resp.IsSuccessStatusCode)
                {
                    Log($"Download HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");
                    return null;
                }
                var total = resp.Content.Headers.ContentLength ?? 0;
                using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var fs = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var buffer = new byte[81920];
                    int read;
                    long written = 0;
                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
                    {
                        await fs.WriteAsync(buffer, 0, read, ct).ConfigureAwait(false);
                        written += read;
                        progress?.Report((written, total));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(manifest.package.sha256))
            {
                var actual = ComputeSha256Hex(zipPath);
                if (!string.Equals(actual, manifest.package.sha256?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Log($"Hash mismatch for downloaded package. expected={manifest.package.sha256} actual={actual}");
                    throw new InvalidOperationException($"Update package hash mismatch (expected={manifest.package.sha256}, actual={actual})");
                }
            }

            return zipPath;
        }

        private string ComputeSha256Hex(string filePath)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            using (var fs = File.OpenRead(filePath))
            {
                var hash = sha.ComputeHash(fs);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public async Task ScheduleApplyAndRestartAsync(string zipPath, Form owner)
        {
            // Prepare PowerShell script to wait for current process, expand zip to target dir, relaunch
            var exePath = System.Windows.Forms.Application.ExecutablePath;
            var targetDir = Path.GetDirectoryName(exePath);
            var pid = System.Diagnostics.Process.GetCurrentProcess().Id;

            var scriptDir = Path.Combine(Path.GetTempPath(), "BCCStudents", "updates", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(scriptDir);
            var scriptPath = Path.Combine(scriptDir, "apply_update.ps1");

            var ps = new StringBuilder();

            // Log path - áƒ’áƒáƒ“áƒáƒ•áƒªáƒ”áƒ— áƒ áƒáƒ’áƒáƒ áƒª áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ˜ áƒ áƒáƒ› UAC-elevated PowerShell-áƒ›áƒ áƒ˜áƒªáƒáƒ“áƒ”áƒ¡ áƒ¡áƒáƒ“ áƒ©áƒáƒ¬áƒ”áƒ áƒáƒ¡
            var userLogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BCCStudents", "logs");
            Directory.CreateDirectory(userLogDir);

            // áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ”áƒ‘áƒ˜ (MUST be first line in PowerShell script!)
            ps.AppendLine("param([string]$zip, [string]$exeName, [int]$targetPid, [string]$userLogDir)");
            ps.AppendLine("");
            
            ps.AppendLine("# áƒ“áƒ”áƒ‘áƒáƒ’áƒ˜áƒœáƒ’áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡ - áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒ©áƒ•áƒ”áƒœáƒ”áƒ‘áƒ");
            ps.AppendLine("Write-Host '=== BCCStudents Update Script Started ===' -ForegroundColor Cyan");
            ps.AppendLine("Write-Host \"PID to wait for: $targetPid\"");
            ps.AppendLine("Write-Host \"ZIP file: $zip\"");
            ps.AppendLine("Write-Host \"Target exe name: $exeName\"");
            ps.AppendLine("Write-Host \"Log directory: $userLogDir\"");
            ps.AppendLine("");

            // áƒáƒ“áƒ›áƒ˜áƒœáƒ˜áƒ¡áƒ¢áƒ áƒáƒ¢áƒáƒ áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ
            ps.AppendLine("if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {");
            ps.AppendLine("    Write-Host 'ERROR: This script requires Administrator privileges!' -ForegroundColor Red");
            ps.AppendLine("    Write-Host 'Press Enter to close...'");
            ps.AppendLine("    $null = Read-Host");
            ps.AppendLine("    exit 1");
            ps.AppendLine("}");
            ps.AppendLine("Write-Host 'Running with Administrator privileges âœ“' -ForegroundColor Green");
            ps.AppendLine("");

            // áƒšáƒáƒ’áƒ˜ (áƒ’áƒáƒ›áƒáƒ•áƒ˜áƒ§áƒ”áƒœáƒáƒ— áƒ’áƒáƒ“áƒáƒªáƒ”áƒ›áƒ£áƒšáƒ˜ user log directory UAC-áƒ˜áƒ¡ áƒžáƒ áƒáƒ‘áƒšáƒ”áƒ›áƒ˜áƒ¡ áƒ’áƒáƒ“áƒáƒ¡áƒáƒ­áƒ áƒ”áƒšáƒáƒ“)
            ps.AppendLine("# áƒ’áƒáƒ›áƒáƒ•áƒ˜áƒ§áƒ”áƒœáƒáƒ— áƒ’áƒáƒ“áƒáƒªáƒ”áƒ›áƒ£áƒšáƒ˜ log directory (áƒáƒ áƒ $env:LOCALAPPDATA áƒ áƒáƒ›áƒ”áƒšáƒ˜áƒª UAC-áƒ¨áƒ˜ Administrator-áƒ˜áƒ¡ áƒ˜áƒ¥áƒœáƒ”áƒ‘áƒ!)");
            ps.AppendLine("if (!(Test-Path $userLogDir)) { ");
            ps.AppendLine("    try { New-Item -ItemType Directory -Path $userLogDir -Force | Out-Null } catch { }");
            ps.AppendLine("}");
            ps.AppendLine("$logFile = Join-Path $userLogDir ('Updater_' + (Get-Date -Format 'yyyyMMdd_HHmmss') + '.txt')");
            ps.AppendLine("");
            ps.AppendLine("# Manual logging function");
            ps.AppendLine("function Write-Log {");
            ps.AppendLine("    param([string]$Message, [string]$Color = 'White')");
            ps.AppendLine("    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'");
            ps.AppendLine("    $logLine = \"[$timestamp] $Message\"");
            ps.AppendLine("    Write-Host $Message -ForegroundColor $Color");
            ps.AppendLine("    try { Add-Content -Path $logFile -Value $logLine -ErrorAction Stop } catch { ");
            ps.AppendLine("        Write-Host \"[ERROR] Could not write to log: $($_.Exception.Message)\" -ForegroundColor Red");
            ps.AppendLine("    }");
            ps.AppendLine("}");
            ps.AppendLine("");
            ps.AppendLine("Write-Log 'Update script started' 'Green'");
            ps.AppendLine("Write-Log \"Log file: $logFile\" 'Cyan'");
            ps.AppendLine("");

            // áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜áƒ¡ áƒ“áƒáƒ®áƒ£áƒ áƒ•áƒ˜áƒ¡ áƒ›áƒáƒšáƒáƒ“áƒ˜áƒœáƒ˜
            ps.AppendLine("Write-Log \"Waiting for process (PID: $targetPid) to exit...\" 'Yellow'");
            ps.AppendLine("$maxWait = 30"); // 30 áƒ¬áƒáƒ›áƒ˜ áƒ›áƒáƒ¥áƒ¡áƒ˜áƒ›áƒ£áƒ›
            ps.AppendLine("$waited = 0");
            ps.AppendLine("while ((Get-Process -Id $targetPid -ErrorAction SilentlyContinue) -and ($waited -lt $maxWait)) {");
            ps.AppendLine("    Start-Sleep -Milliseconds 500");
            ps.AppendLine("    $waited++");
            ps.AppendLine("    if ($waited % 2 -eq 0) { Write-Host \".\" -NoNewline }");
            ps.AppendLine("}");
            ps.AppendLine("Write-Host ''");
            ps.AppendLine("if ($waited -ge $maxWait) {");
            ps.AppendLine("    Write-Log 'WARNING: Timeout waiting for process to exit!' 'Red'");
            ps.AppendLine("    Write-Log 'Attempting to force close...' 'Yellow'");
            ps.AppendLine("    try { Stop-Process -Id $targetPid -Force -ErrorAction SilentlyContinue } catch { }");
            ps.AppendLine("    Start-Sleep -Seconds 3");
            ps.AppendLine("} else {");
            ps.AppendLine("    Write-Log \"Process exited after $waited checks!\" 'Green'");
            ps.AppendLine("}");
            ps.AppendLine("Write-Log 'Waiting for file handles to release...' 'Yellow'");
            ps.AppendLine("Start-Sleep -Seconds 5"); // áƒ›áƒœáƒ˜áƒ¨áƒ•áƒœáƒ”áƒšáƒáƒ•áƒáƒœáƒ˜: Windows-áƒ¡ áƒ¡áƒ­áƒ˜áƒ áƒ“áƒ”áƒ‘áƒ áƒ“áƒ áƒ file handles-áƒ˜áƒ¡ áƒ’áƒáƒ¡áƒáƒ—áƒáƒ•áƒ˜áƒ¡áƒ£áƒ¤áƒšáƒ”áƒ‘áƒšáƒáƒ“

            // Target áƒ¤áƒáƒšáƒ“áƒ”áƒ áƒ˜ (áƒ“áƒ˜áƒœáƒáƒ›áƒ˜áƒ£áƒ áƒ˜ áƒžáƒáƒ—áƒ˜)
            ps.AppendLine($"$target = '{targetDir.Replace("\\", "\\\\")}'");
            
            ps.AppendLine("Write-Host \"Target: $target\"");
            ps.AppendLine("if (!(Test-Path $target)) { ");
            ps.AppendLine("    Write-Host 'Creating target directory...'");
            ps.AppendLine("    New-Item -ItemType Directory -Path $target -Force | Out-Null ");
            ps.AppendLine("}");

            // Staging folder
            ps.AppendLine("Write-Host ''");
            ps.AppendLine("Write-Host '=== Preparing Update ===' -ForegroundColor Cyan");
            ps.AppendLine("$staging = Join-Path ([IO.Path]::GetTempPath()) ('BCCStudents\\staging\\' + [Guid]::NewGuid().ToString('N'))");
            ps.AppendLine("Write-Host \"Creating staging folder: $staging\"");
            ps.AppendLine("try {");
            ps.AppendLine("    New-Item -ItemType Directory -Path $staging -Force | Out-Null");
            ps.AppendLine("    Write-Host 'Staging folder created âœ“' -ForegroundColor Green");
            ps.AppendLine("} catch {");
            ps.AppendLine("    Write-Host \"ERROR: Could not create staging folder: $($_.Exception.Message)\" -ForegroundColor Red");
            ps.AppendLine("    Write-Host 'Press Enter to close...'");
            ps.AppendLine("    $null = Read-Host");
            ps.AppendLine("    exit 1");
            ps.AppendLine("}");
            ps.AppendLine("");

            // ZIP áƒáƒ áƒ¡áƒ”áƒ‘áƒáƒ‘áƒ˜áƒ¡ áƒ¨áƒ”áƒ›áƒáƒ¬áƒ›áƒ”áƒ‘áƒ
            ps.AppendLine("Write-Log \"Checking ZIP file: $zip\" 'Cyan'");
            ps.AppendLine("if (!(Test-Path $zip)) {");
            ps.AppendLine("    Write-Log \"ERROR: ZIP file not found: $zip\" 'Red'");
            ps.AppendLine("    Write-Host 'Press Enter to close...'");
            ps.AppendLine("    $null = Read-Host");
            ps.AppendLine("    exit 1");
            ps.AppendLine("}");
            ps.AppendLine("Write-Log 'ZIP file found âœ“' 'Green'");
            ps.AppendLine("");

            // ZIP áƒ’áƒáƒ®áƒ¡áƒœáƒ
            ps.AppendLine("Write-Log 'Extracting ZIP archive...' 'Yellow'");
            ps.AppendLine("try { ");
            ps.AppendLine("    Expand-Archive -Path $zip -DestinationPath $staging -Force; ");
            ps.AppendLine("    $extractedFiles = Get-ChildItem -Path $staging -Recurse | Measure-Object");
            ps.AppendLine("    Write-Log \"Extraction completed! ($($extractedFiles.Count) files extracted)\" 'Green'");
            ps.AppendLine("} catch { ");
            ps.AppendLine("    Write-Log \"ERROR: Expand-Archive failed: $($_.Exception.Message)\" 'Red'");
            ps.AppendLine("    Write-Host 'Press Enter to close...'");
            ps.AppendLine("    $null = Read-Host");
            ps.AppendLine("    exit 1");
            ps.AppendLine("}");
            ps.AppendLine("");

            // Source path áƒ’áƒáƒœáƒ¡áƒáƒ–áƒ¦áƒ•áƒ áƒ
            ps.AppendLine("$entries = Get-ChildItem -Path $staging");
            ps.AppendLine("if ($entries.Count -eq 1 -and $entries[0].PSIsContainer) { ");
            ps.AppendLine("    $src = $entries[0].FullName ");
            ps.AppendLine("} else { ");
            ps.AppendLine("    $src = $staging ");
            ps.AppendLine("}");
            ps.AppendLine("Write-Host \"Source: $src\"");

            // Backup app.config before update (áƒ›áƒáƒ›áƒ®áƒ›áƒáƒ áƒ”áƒ‘áƒšáƒ˜áƒ¡ áƒ™áƒáƒœáƒ¤áƒ˜áƒ’áƒ£áƒ áƒáƒªáƒ˜áƒ”áƒ‘áƒ˜áƒ¡ áƒ¨áƒ”áƒ¡áƒáƒœáƒáƒ áƒ©áƒ£áƒœáƒ”áƒ‘áƒšáƒáƒ“)
            ps.AppendLine("$appConfigPath = Join-Path $target 'BCCStudents.exe.config'");
            ps.AppendLine("$appConfigBackup = Join-Path $target 'BCCStudents.exe.config.backup'");
            ps.AppendLine("if (Test-Path $appConfigPath) {");
            ps.AppendLine("    Write-Host 'Backing up App.config...' -ForegroundColor Yellow");
            ps.AppendLine("    Copy-Item -Path $appConfigPath -Destination $appConfigBackup -Force");
            ps.AppendLine("}");

            // Robocopy-áƒ˜áƒ— áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ™áƒáƒžáƒ˜áƒ áƒ”áƒ‘áƒ (retry logic-áƒ˜áƒ—)
            ps.AppendLine("Write-Host ''");
            ps.AppendLine("Write-Host '=== Copying Files ===' -ForegroundColor Cyan");
            ps.AppendLine("Write-Log 'Starting file copy with Robocopy...' 'Yellow'");
            ps.AppendLine("Write-Log \"Source: $src\" 'Cyan'");
            ps.AppendLine("Write-Log \"Target: $target\" 'Cyan'");
            
            ps.AppendLine("$maxRetries = 3");
            ps.AppendLine("$retryCount = 0");
            ps.AppendLine("$exitCode = 16"); // default error
            ps.AppendLine("");
            ps.AppendLine("while ($retryCount -lt $maxRetries -and $exitCode -ge 8) {");
            ps.AppendLine("    if ($retryCount -gt 0) {");
            ps.AppendLine("        Write-Host \"Retry attempt $retryCount of $maxRetries...\" -ForegroundColor Yellow");
            ps.AppendLine("        Start-Sleep -Seconds 3");
            ps.AppendLine("    }");
            ps.AppendLine("    ");
            // /E = copy subdirectories including empty
            // /XO = exclude older files (keep newer if exists)
            // /R:5 = retry 5 times per file
            // /W:2 = wait 2 seconds between retries
            // /NP = no progress (cleaner output)
            // /NFL = no file list (cleaner output)
            ps.AppendLine("    $robocopyArgs = @(\"$src\", \"$target\", '/E', '/XO', '/R:5', '/W:2', '/NP', '/NFL')");
            ps.AppendLine("    $process = Start-Process -FilePath 'robocopy.exe' -ArgumentList $robocopyArgs -Wait -PassThru -NoNewWindow");
            ps.AppendLine("    $exitCode = $process.ExitCode");
            ps.AppendLine("    $color = if ($exitCode -lt 8) {'Green'} else {'Red'}");
            ps.AppendLine("    Write-Log \"Robocopy exit code: $exitCode\" $color");
            ps.AppendLine("    $retryCount++");
            ps.AppendLine("}");
            ps.AppendLine("");
            ps.AppendLine("if ($exitCode -ge 8) {");
            ps.AppendLine("    Write-Log \"ERROR: Robocopy failed after $maxRetries attempts (exit code: $exitCode)\" 'Red'");
            ps.AppendLine("    Write-Log 'Files may still be in use. Please close all instances of BCCStudents and try again.' 'Yellow'");
            ps.AppendLine("}");

            // Robocopy exit code-áƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒ”áƒ¢áƒáƒšáƒ£áƒ áƒ˜ áƒáƒœáƒáƒšáƒ˜áƒ–áƒ˜
            ps.AppendLine("Write-Host ''");
            ps.AppendLine("Write-Host '=== Robocopy Result ===' -ForegroundColor Cyan");
            ps.AppendLine("switch ($exitCode) {");
            ps.AppendLine("    0 { Write-Host 'No files were copied (all files up to date)' -ForegroundColor Green }");
            ps.AppendLine("    1 { Write-Host 'Files copied successfully!' -ForegroundColor Green }");
            ps.AppendLine("    2 { Write-Host 'Extra files or directories detected (normal)' -ForegroundColor Green }");
            ps.AppendLine("    3 { Write-Host 'Files copied and extras found (normal)' -ForegroundColor Green }");
            ps.AppendLine("    4 { Write-Host 'Some mismatched files or directories' -ForegroundColor Yellow }");
            ps.AppendLine("    5 { Write-Host 'Some files copied, some mismatches' -ForegroundColor Yellow }");
            ps.AppendLine("    6 { Write-Host 'Extra files and mismatches' -ForegroundColor Yellow }");
            ps.AppendLine("    7 { Write-Host 'Files copied, extras and mismatches' -ForegroundColor Yellow }");
            ps.AppendLine("    8 { Write-Host 'ERROR: Some files or directories could not be copied!' -ForegroundColor Red }");
            ps.AppendLine("    16 { Write-Host 'ERROR: Serious error - files are still in use or locked!' -ForegroundColor Red }");
            ps.AppendLine("    default { Write-Host \"ERROR: Unexpected exit code $exitCode\" -ForegroundColor Red }");
            ps.AppendLine("}");
            ps.AppendLine("");
            
            // Success check
            ps.AppendLine("if ($exitCode -lt 8) {");
            ps.AppendLine("    Write-Log 'Update files copied successfully!' 'Green'");
            ps.AppendLine("    ");
            ps.AppendLine("    # Restore app.config backup (áƒ›áƒáƒ›áƒ®áƒ›áƒáƒ áƒ”áƒ‘áƒšáƒ˜áƒ¡ áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ”áƒ‘áƒ˜áƒ¡ áƒáƒ¦áƒ“áƒ’áƒ”áƒœáƒ)");
            ps.AppendLine("    if (Test-Path $appConfigBackup) {");
            ps.AppendLine("        Write-Host 'Restoring user App.config settings...' -ForegroundColor Yellow");
            ps.AppendLine("        try {");
            ps.AppendLine("            [xml]$oldConfig = Get-Content $appConfigBackup");
            ps.AppendLine("            [xml]$newConfig = Get-Content $appConfigPath");
            ps.AppendLine("            # Merge appSettings (AutoFileDetection áƒ“áƒ áƒ¡áƒ®áƒ•áƒ áƒžáƒáƒ áƒáƒ›áƒ”áƒ¢áƒ áƒ”áƒ‘áƒ˜)");
            ps.AppendLine("            if ($oldConfig.configuration.appSettings -and $newConfig.configuration.appSettings) {");
            ps.AppendLine("                $oldConfig.configuration.appSettings.add | ForEach-Object {");
            ps.AppendLine("                    $key = $_.key");
            ps.AppendLine("                    $existing = $newConfig.configuration.appSettings.add | Where-Object { $_.key -eq $key }");
            ps.AppendLine("                    if ($existing) { $existing.value = $_.value }");
            ps.AppendLine("                }");
            ps.AppendLine("            }");
            ps.AppendLine("            $newConfig.Save($appConfigPath)");
            ps.AppendLine("            Write-Host 'App.config settings restored!' -ForegroundColor Green");
            ps.AppendLine("        } catch {");
            ps.AppendLine("            Write-Host \"Warning: Could not merge config settings: $($_.Exception.Message)\" -ForegroundColor Yellow");
            ps.AppendLine("        }");
            ps.AppendLine("        Remove-Item $appConfigBackup -Force -ErrorAction SilentlyContinue");
            ps.AppendLine("    }");
            ps.AppendLine("} else {");
            ps.AppendLine("    Write-Host ''");
            ps.AppendLine("    Write-Host '=== UPDATE FAILED ===' -ForegroundColor Red");
            ps.AppendLine("    Write-Log '=== UPDATE FAILED ===' 'Red'");
            ps.AppendLine("    Write-Log \"Robocopy failed with exit code $exitCode\" 'Red'");
            ps.AppendLine("    Write-Host 'Please close all instances of BCCStudents and try again.' -ForegroundColor Yellow");
            ps.AppendLine("    Write-Host ''");
            ps.AppendLine("    Write-Host 'Press Enter to close...'");
            ps.AppendLine("    $null = Read-Host");
            ps.AppendLine("    Stop-Transcript -ErrorAction SilentlyContinue");
            ps.AppendLine("    exit 1");
            ps.AppendLine("}");
            ps.AppendLine("");

            // áƒáƒžáƒšáƒ˜áƒ™áƒáƒªáƒ˜áƒ˜áƒ¡ áƒ’áƒáƒ¨áƒ•áƒ”áƒ‘áƒ
            ps.AppendLine("Write-Host ''");
            ps.AppendLine("Write-Host '=== Restarting Application ===' -ForegroundColor Cyan");
            ps.AppendLine("if (-not [string]::IsNullOrEmpty($exeName)) {");
            ps.AppendLine("    $exePath = Join-Path $target $exeName");
            ps.AppendLine("    Write-Host \"Looking for: $exePath\"");
            ps.AppendLine("    ");
            ps.AppendLine("    if (Test-Path $exePath) {");
            ps.AppendLine("        Write-Log 'Executable found âœ“' 'Green'");
            ps.AppendLine("        try {");
            ps.AppendLine("            Write-Log 'Starting application...' 'Yellow'");
            ps.AppendLine("            Start-Process -FilePath `\"$exePath`\" -WorkingDirectory `\"$target`\"");
            ps.AppendLine("            Start-Sleep -Seconds 2"); // áƒ“áƒáƒ•áƒ áƒ¬áƒ›áƒ£áƒœáƒ“áƒ”áƒ— áƒ áƒáƒ› áƒžáƒ áƒáƒªáƒ”áƒ¡áƒ˜ áƒ©áƒáƒ˜áƒ¢áƒ•áƒ˜áƒ áƒ—áƒ
            ps.AppendLine("            Write-Log 'Application restarted successfully! âœ“' 'Green'");
            ps.AppendLine("        } catch {");
            ps.AppendLine("            Write-Log \"ERROR: Failed to start application: $($_.Exception.Message)\" 'Red'");
            ps.AppendLine("            Write-Host 'You can manually start the application from:' -ForegroundColor Yellow");
            ps.AppendLine("            Write-Host \"  $exePath\" -ForegroundColor White");
            ps.AppendLine("        }");
            ps.AppendLine("    } else {");
            ps.AppendLine("        Write-Log 'ERROR: Executable not found!' 'Red'");
            ps.AppendLine("        Write-Host \"Expected location: $exePath\" -ForegroundColor Yellow");
            ps.AppendLine("        Write-Host 'Files in target directory:' -ForegroundColor Yellow");
            ps.AppendLine("        Get-ChildItem -Path $target -Filter '*.exe' | ForEach-Object { Write-Host \"  - $($_.Name)\" }");
            ps.AppendLine("    }");
            ps.AppendLine("} else {");
            ps.AppendLine("    Write-Log 'WARNING: No executable name provided' 'Yellow'");
            ps.AppendLine("}");

            // Cleanup
            ps.AppendLine("Write-Host 'Cleaning up temporary files...'");
            ps.AppendLine("Remove-Item -LiteralPath $zip -Force -ErrorAction SilentlyContinue");
            ps.AppendLine("Remove-Item -LiteralPath $staging -Recurse -Force -ErrorAction SilentlyContinue");

            ps.AppendLine("Write-Host ''");
            ps.AppendLine("Write-Host '=== UPDATE COMPLETED SUCCESSFULLY ===' -ForegroundColor Green");
            ps.AppendLine("Write-Log '=== UPDATE COMPLETED SUCCESSFULLY ===' 'Green'");

            // áƒ™áƒáƒœáƒ¡áƒáƒšáƒ˜áƒ¡ áƒ“áƒáƒ§áƒáƒ•áƒœáƒ”áƒ‘áƒ
            ps.AppendLine("Write-Host ''");
            ps.AppendLine("Write-Host 'Press Enter to close this window...' -ForegroundColor Gray");
            ps.AppendLine("$null = Read-Host");

            ps.AppendLine("Stop-Transcript -ErrorAction SilentlyContinue");

            File.WriteAllText(scriptPath, ps.ToString(), new UTF8Encoding(true));



            var exeName = Path.GetFileName(exePath);
            var args = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" -zip \"{zipPath}\" -exeName \"{exeName}\" -targetPid {pid} -userLogDir \"{userLogDir}\"";
            var psExe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "WindowsPowerShell", "v1.0", "powershell.exe");
            var si = new System.Diagnostics.ProcessStartInfo(psExe, args)
            {
                UseShellExecute = true, // needed for Verb=runas
                Verb = "runas",         // prompt for elevation (UAC)
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal // áƒ®áƒ˜áƒšáƒ£áƒšáƒ˜ áƒ¤áƒáƒœáƒ¯áƒáƒ áƒ áƒ¢áƒ”áƒ¡áƒ¢áƒ˜áƒ¡áƒ—áƒ•áƒ˜áƒ¡
            };
            Log($"Apply: zip={zipPath} target={targetDir} pid={pid}");
            
            // áƒ›áƒáƒ™áƒšáƒ” áƒ¨áƒ”áƒ¢áƒ§áƒáƒ‘áƒ˜áƒœáƒ”áƒ‘áƒ áƒ›áƒáƒ›áƒ®áƒ›áƒáƒ áƒ”áƒ‘áƒ”áƒšáƒ¡ BEFORE PowerShell-áƒ˜áƒ¡ áƒ’áƒáƒ¨áƒ•áƒ”áƒ‘áƒáƒ›áƒ“áƒ”
            var result = MessageBox.Show(
                "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ áƒ›áƒ–áƒáƒ“ áƒáƒ áƒ˜áƒ¡ áƒ“áƒáƒ¡áƒáƒ¬áƒ§áƒ”áƒ‘áƒáƒ“!\n\n" +
                "áƒ“áƒáƒáƒ­áƒ˜áƒ áƒ”áƒ— OK áƒ áƒáƒ›:\n" +
                "1. áƒ’áƒáƒ˜áƒ®áƒ¡áƒœáƒáƒ¡ PowerShell Administrator áƒ áƒ”áƒŸáƒ˜áƒ›áƒ¨áƒ˜\n" +
                "2. áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ¤áƒáƒ˜áƒšáƒ”áƒ‘áƒ˜ áƒ“áƒáƒ™áƒáƒžáƒ˜áƒ áƒ“áƒ”áƒ¡\n" +
                "3. áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ áƒáƒ•áƒ¢áƒáƒ›áƒáƒ¢áƒ£áƒ áƒáƒ“ áƒ®áƒ”áƒšáƒáƒ®áƒšáƒ áƒ©áƒáƒ˜áƒ¢áƒ•áƒ˜áƒ áƒ—áƒáƒ¡\n\n" +
                "âš ï¸ áƒ›áƒœáƒ˜áƒ¨áƒ•áƒœáƒ”áƒšáƒáƒ•áƒáƒœáƒ˜: PowerShell áƒ¤áƒáƒœáƒ¯áƒáƒ áƒ áƒáƒ  áƒ“áƒáƒ®áƒ£áƒ áƒáƒ— áƒ®áƒ”áƒšáƒ˜áƒ—!",
                "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (result != DialogResult.OK)
            {
                Log("User cancelled update");
                return;
            }

            try
            {
                Log("Starting PowerShell updater with elevated privileges...");
                var psProcess = System.Diagnostics.Process.Start(si);
                Log($"PowerShell updater started successfully (PID: {psProcess?.Id})");
                
                // áƒ“áƒáƒ•áƒ”áƒšáƒáƒ“áƒáƒ— PowerShell-áƒ˜áƒ¡ áƒ¡áƒ áƒ£áƒšáƒáƒ“ áƒ’áƒáƒ¨áƒ•áƒ”áƒ‘áƒáƒ¡
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Log($"Failed to start PowerShell updater: {ex.Message}");
                MessageBox.Show(
                    $"áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ˜áƒ¡ áƒ“áƒáƒ¬áƒ§áƒ”áƒ‘áƒ áƒ•áƒ”áƒ  áƒ›áƒáƒ®áƒ”áƒ áƒ®áƒ“áƒ:\n\n{ex.Message}\n\n" +
                    "áƒ’áƒ—áƒ®áƒáƒ•áƒ— áƒ¡áƒªáƒáƒ“áƒ”áƒ—:\n" +
                    "1. áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ˜áƒ¡ Administrator-áƒ˜áƒ— áƒ’áƒáƒ¨áƒ•áƒ”áƒ‘áƒ\n" +
                    "2. Antivirus-áƒ˜áƒ¡ áƒ“áƒ áƒáƒ”áƒ‘áƒ˜áƒ— áƒ’áƒáƒ›áƒáƒ áƒ—áƒ•áƒ", 
                    "áƒ¨áƒ”áƒªáƒ“áƒáƒ›áƒ", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            // Close current app to allow updater to replace files
            Log("Closing application for update...");
            
            // áƒ’áƒáƒ›áƒáƒ•áƒáƒ©áƒ˜áƒœáƒáƒ— áƒ˜áƒœáƒ¤áƒáƒ áƒ›áƒáƒªáƒ˜áƒ áƒ áƒáƒ› áƒžáƒ áƒáƒ’áƒ áƒáƒ›áƒ áƒ˜áƒ®áƒ£áƒ áƒ”áƒ‘áƒ
            owner?.BeginInvoke(new Action(() =>
            {
                owner.Hide();
                owner.Text = "áƒ’áƒáƒœáƒáƒ®áƒšáƒ”áƒ‘áƒ áƒ›áƒ˜áƒ›áƒ“áƒ˜áƒœáƒáƒ áƒ”áƒáƒ‘áƒ¡...";
            }));
            
            await Task.Delay(1000); // 1 áƒ¬áƒáƒ›áƒ˜ PowerShell-áƒ¡ áƒ áƒáƒ› áƒ“áƒáƒáƒ¡áƒ¬áƒ áƒáƒ¡
            
            try { System.Windows.Forms.Application.Exit(); } catch { }
            await Task.Delay(500);
            try { Environment.Exit(0); } catch { }
            await Task.Delay(500);
            try { System.Diagnostics.Process.GetCurrentProcess().Kill(); } catch { }
        }

        private void Log(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_logPath)) return;
                File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\r\n");
            }
            catch { }
        }
    }

    public class UpdateManifest
    {
        public string latestVersion { get; set; }
        public string releaseDate { get; set; }
        public string notes { get; set; }
        public UpdatePackage package { get; set; }
    }

    public class UpdatePackage
    {
        public string url { get; set; }
        public long size { get; set; }
        public string sha256 { get; set; }
    }
}



