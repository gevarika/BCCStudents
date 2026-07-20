; --- პროექტის ძირითადი პარამეტრები ---
#define MyAppName "სტუდენტების მართვის სისტემა"
; მოკლე ინგლისური დასახელება — გამოიყენება ინსტალაციის საქაღალდისა და მალსახმობებისთვის,
; რომ Program Files-ში და დესკტოპზე ქართული/გრძელი სახელი არ შეიქმნას.
#define MyAppShortName "BCCStudents"
#define MyAppVersion "1.0.2.7"
#define MyAppPublisher "ბოლნისის კულტურის ცენტრი"
#define MyAppExeName "BCCStudents.Presentation.exe"
#define BuildOutput "BCCStudents.Presentation\bin\Release\net8.0-windows10.0.17763.0"

[Setup]
AppId={{6950628A-E139-409D-BF65-B5C2BEBC5D36}};
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
; საქაღალდე იქმნება მოკლე ინგლისური სახელით: C:\Program Files\BCCStudents
DefaultDirName={autopf64}\{#MyAppShortName}
UsePreviousAppDir=no
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

[Icons]
; მალსახმობებს ენიჭება მოკლე ინგლისური სახელი (BCCStudents) Start მენიუსა და დესკტოპზე
Name: "{autoprograms}\{#MyAppShortName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppShortName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function HasDotNetDesktopRuntime8InRegistry(RootKey: Integer; const Architecture: string): Boolean;
var
  VersionNames: TArrayOfString;
  I: Integer;
begin
  Result := False;

  if not RegGetSubkeyNames(
    RootKey,
    'SOFTWARE\dotnet\Setup\InstalledVersions\' + Architecture + '\sharedfx\Microsoft.WindowsDesktop.App',
    VersionNames
  ) then
    Exit;

  for I := 0 to GetArrayLength(VersionNames) - 1 do
  begin
    if Copy(VersionNames[I], 1, 2) = '8.' then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function HasDotNetDesktopRuntime8InDirectory(const BaseDir: string): Boolean;
var
  FindRec: TFindRec;
  RuntimeDir: string;
begin
  Result := False;
  RuntimeDir := AddBackslash(BaseDir) + 'dotnet\shared\Microsoft.WindowsDesktop.App';

  if not DirExists(RuntimeDir) then
    Exit;

  if FindFirst(AddBackslash(RuntimeDir) + '8.*', FindRec) then
  begin
    try
      repeat
        if DirExists(AddBackslash(RuntimeDir) + FindRec.Name) then
        begin
          Result := True;
          Exit;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end;
end;

// .NET Desktop Runtime 8-ის შემოწმება
function IsDotNetDesktopRuntimeInstalled(): Boolean;
begin
  Result :=
    HasDotNetDesktopRuntime8InRegistry(HKLM64, 'x64') or
    HasDotNetDesktopRuntime8InRegistry(HKLM32, 'x86') or
    HasDotNetDesktopRuntime8InDirectory(ExpandConstant('{pf64}')) or
    HasDotNetDesktopRuntime8InDirectory(ExpandConstant('{pf32}'));
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
    IsServiceRunning('MySQL84') or
    IsServiceRunning('MySQL83') or
    IsServiceRunning('MySQL82') or
    IsServiceRunning('MySQL81') or
    IsServiceRunning('MySQL') or
    IsServiceRunning('MySQL57') or
    IsServiceRunning('MariaDB');
end;

function IsTcpPortListening(const Port: string): Boolean;
var
  ResultCode: Integer;
begin
  Result :=
    Exec(
      ExpandConstant('{sys}\cmd.exe'),
      '/C netstat -ano -p tcp | findstr /R /C:":' + Port + ' .*LISTENING" >nul',
      '',
      SW_HIDE,
      ewWaitUntilTerminated,
      ResultCode
    ) and (ResultCode = 0);
end;

function IsMySqlAvailable(): Boolean;
begin
  Result :=
    IsAnyMySqlServiceRunning() or
    IsTcpPortListening('3307') or
    IsTcpPortListening('3306');
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
  else if not IsMySqlAvailable() then
  begin
    Result := MsgBox(
      'MySQL სერვისი ვერ მოიძებნა ან არ მუშაობს.'#13#10#13#10 +
      'ინსტალაცია შესაძლებელია გაგრძელდეს, მაგრამ პროგრამა მონაცემთა ბაზას ვერ დაუკავშირდება, სანამ MySQL არ ჩაირთვება ან კონფიგურაციაში remote ბაზა არ მიეთითება.'#13#10#13#10 +
      'გსურთ ინსტალაციის გაგრძელება?',
      mbConfirmation,
      MB_YESNO
    ) = IDYES;
  end
  else
    Result := True;
end;