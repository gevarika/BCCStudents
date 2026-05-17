; --- პროექტის ძირითადი პარამეტრები ---
#define MyAppName "სტუდენტების მართვის სისტემა"
#define MyAppVersion "1.0.2.6"
#define MyAppPublisher "ბოლნისის კულტურის ცენტრი"
#define MyAppExeName "BCCStudents.Presentation.exe"
#define BuildOutput "BCCStudents.Presentation\bin\Release\net8.0-windows"

[Setup]
AppId={{6950628A-E139-409D-BF65-B5C2BEBC5D36}};
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf64}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir={#SourcePath}\Installer
OutputBaseFilename=BCCStudentsSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Dirs]
Name: "{commonappdata}\BCCStudents"; Permissions: users-modify

[Files]
; ყველა ფაილი bin\Release\net8.0-windows-დან
Source: "{#BuildOutput}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; გარე კონფიგურაცია MySQL პარამეტრებისთვის
Source: "BCCStudents.Presentation\Config\db.config.json"; DestDir: "{commonappdata}\BCCStudents"; Flags: onlyifdoesntexist; Permissions: users-modify

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// .NET Desktop Runtime 8-ის შემოწმება
function IsDotNetDesktopRuntimeInstalled(): Boolean;
var
  Installed: Cardinal;
begin
  Result := RegQueryDWordValue(
    HKLM,
    'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App',
    '8.0.0',
    Installed
  );
end;

const
  SERVICE_QUERY_STATUS = $0004;
  SERVICE_RUNNING = $00000004;
  SC_MANAGER_CONNECT = $0001;

type
  TServiceStatus = record
    dwServiceType: DWORD;
    dwCurrentState: DWORD;
    dwControlsAccepted: DWORD;
    dwWin32ExitCode: DWORD;
    dwServiceSpecificExitCode: DWORD;
    dwCheckPoint: DWORD;
    dwWaitHint: DWORD;
  end;

function OpenSCManager(lpMachineName, lpDatabaseName: string; dwDesiredAccess: DWORD): Integer;
  external 'OpenSCManagerW@advapi32.dll stdcall';
function OpenService(hSCManager: Integer; lpServiceName: string; dwDesiredAccess: DWORD): Integer;
  external 'OpenServiceW@advapi32.dll stdcall';
function QueryServiceStatus(hService: Integer; var lpServiceStatus: TServiceStatus): Boolean;
  external 'QueryServiceStatus@advapi32.dll stdcall';
function CloseServiceHandle(hSCObject: Integer): Boolean;
  external 'CloseServiceHandle@advapi32.dll stdcall';

function IsServiceRunning(const ServiceName: string): Boolean;
var
  scm: Integer;
  svc: Integer;
  status: TServiceStatus;
begin
  Result := False;
  scm := OpenSCManager('', '', SC_MANAGER_CONNECT);
  if scm = 0 then
    Exit;
  try
    svc := OpenService(scm, ServiceName, SERVICE_QUERY_STATUS);
    if svc = 0 then
      Exit;
    try
      if QueryServiceStatus(svc, status) then
        Result := (status.dwCurrentState = SERVICE_RUNNING);
    finally
      CloseServiceHandle(svc);
    end;
  finally
    CloseServiceHandle(scm);
  end;
end;

function IsAnyMySqlServiceRunning(): Boolean;
begin
  Result :=
    IsServiceRunning('MySQL80') or
    IsServiceRunning('MySQL') or
    IsServiceRunning('MySQL57') or
    IsServiceRunning('MariaDB');
end;

function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
begin
  if not IsDotNetDesktopRuntimeInstalled() then
  begin
    if MsgBox('პროგრამის მუშაობისთვის საჭიროა .NET Desktop Runtime 8. გსურთ გადმოწერა?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://dotnet.microsoft.com/en-us/download/dotnet/8.0', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
    Result := False; // ინსტალაცია წყდება
  end
  else if not IsAnyMySqlServiceRunning() then
  begin
    MsgBox('MySQL სერვისი არ მუშაობს ან არ არის დაინსტალირებული. გთხოვ, დააინსტალირე/გაუშვი MySQL და შემდეგ სცადე თავიდან.', mbError, MB_OK);
    Result := False;
  end
  else
    Result := True;
end;