// https://github.com/DomGries/InnoDependencyInstaller
#define AppName "Vajehdan"
#define AppExeName AppName+".exe"

//Return app version in SemVer (for example: 4.0.2.3 => 4.0.2)
#define AppVersion() \
   GetVersionComponents("..\artifacts\release\"+AppExeName, \
       Local[0], Local[1], Local[2], Local[3]), \
   Str(Local[0]) + "." + Str(Local[1]) + "." + Str(Local[2])

#define MyAppURL "https://kokabi1365.github.io/Vajehdan/"

[Setup]  
AppId={{57847BF6-6691-4CF6-98D7-4692205872F6}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\{#AppName}
OutputDir=..\artifacts
DefaultGroupName={#AppName}
OutputBaseFilename=Vajehdan.Setup.{#AppVersion}
SetupIconFile=setup.ico
PrivilegesRequired=lowest
DisableFinishedPage=yes 
DisableDirPage=yes
DisableReadyPage=yes
DisableReadyMemo=yes
DisableProgramGroupPage=yes


[Files]
Source: "..\artifacts\release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs;
Source: "netcorecheck_x64.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall
Source: "netcorecheck.exe"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall; AfterInstall : DownloadInstallDotNet;

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{userdesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"

[Run]
Filename: {app}\{#AppExeName}; Flags: nowait postinstall skipifsilent

[Code]
var
  ProgressPage: TOutputProgressWizardPage;
  DownloadPage: TDownloadWizardPage;

function OnDownloadProgress(const Url, FileName: String; const Progress, ProgressMax: Int64): Boolean;
begin
  if Progress = ProgressMax then
    Log(Format('Successfully downloaded file to {tmp}: %s', [FileName]));
  Result := True;
end;

procedure InitializeWizard;
begin
  ProgressPage := CreateOutputProgressPage('Finalization of installation','');
  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), @OnDownloadProgress);
end;

function GetArchitectureSuffix: String;
begin
  if IsWin64 then
    Result:= '_x64'
  else
    Result:= ''
end;

function IsDotNetInstalled(const Version: String): Boolean;
var
  ResultCode: Integer;
begin
  if not FileExists(ExpandConstant('{tmp}{\}') + 'netcorecheck' + GetArchitectureSuffix + '.exe') then begin
    ExtractTemporaryFile('netcorecheck' + GetArchitectureSuffix + '.exe');
  end;
  Result := ShellExec('', ExpandConstant('{tmp}{\}') + 'netcorecheck' + GetArchitectureSuffix + '.exe', Version, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) and (ResultCode = 0);
end;

//.Net 5 (.NET is completely independent of the .NET Framework.)
procedure DownloadDotNet;
begin
    if GetArchitectureSuffix = '_x64' then 
    begin          
      DownloadPage.Clear;           
      DownloadPage.Add('https://go.microsoft.com/fwlink/?linkid=2155258', 'dotnet503desktop.exe', '');
      DownloadPage.Show;
        try
          DownloadPage.Download;
          DownloadPage.Hide;
          Exit;
        except
          SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
          DownloadPage.Hide;
          Exit;
        end;                     
    end 
    else 
    begin          
      DownloadPage.Clear;           
      DownloadPage.Add('https://go.microsoft.com/fwlink/?linkid=2155347', 'dotnet503desktop.exe', '');
      DownloadPage.Show;
        try
          DownloadPage.Download;
          DownloadPage.Hide;
          Exit;
        except
          SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
          DownloadPage.Hide;
          Exit;
        end;            
    end;
end;

procedure InstallDotNet;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := 'Installing .NET Desktop Runtime 5.0.3';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    Exec(ExpandConstant('{tmp}\dotnet503desktop.exe'), '/install /quiet /norestart', '', SW_HIDE, ewWaitUntilTerminated, ResultCode)
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
  end;
end;

procedure DownloadInstallDotNet;
begin
   if (not IsDotNetInstalled('Microsoft.WindowsDesktop.App 5.0.0')) then 
   begin
      DownloadDotNet
      InstallDotNet
   end; 
end;






































