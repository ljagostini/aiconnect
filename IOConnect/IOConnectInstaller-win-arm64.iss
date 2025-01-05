;--------------------------------------------------------------------------------------------
;DEFINIÇÕES
;--------------------------------------------------------------------------------------------

;Empresa
#define DefAppCompany = "Percolore"
#define DefAppPublisher = "Percolore Equipamentos Tintométricos"
#define DefAppURL = "https://www.percolore.com"
#define DefAppCopyright = "Copyright © 2003-2025"

;Aplicação
#define DefAppName = "IOConnect"
#define DefAppExeFile = "IOConnect.exe"
#define DefAppVersion = "5.0.0"
#define DefAppArchitecture = "(ARM64)"
#define DefAppDescription = "Utilitário de comunicação e execução de dispensa."

;Dependências
#define DefAppNetVersion = "8.0.11"

[ThirdParty]
UseRelativePaths=True

[Setup]
;O valor definido em AppId é o mesmo GUID de identificação definido no arquivo AssemblyInfo.cs
AppId=7f57aded-e441-4e3c-913b-af955258d361

AppName={#DefAppName}
AppVersion ={#DefAppVersion}
AppVerName ={#DefAppName} {#DefAppVersion}
AppPublisher ={#DefAppPublisher}
AppPublisherURL ={#DefAppURL}
AppSupportURL ={#DefAppURL}
AppUpdatesURL ={#DefAppURL}
AppCopyright ={#DefAppCopyright}
VersionInfoVersion ={#DefAppVersion}
VersionInfoCompany =Percolore Máquinas
VersionInfoDescription ={#DefAppDescription}
ArchitecturesAllowed =x64compatible
SolidCompression =yes
Compression =lzma
PrivilegesRequired=none
SetupIconFile =Files\ioconnect.ico
WizardImageStretch =False
WizardSmallImageFile =Files\WizardSmallImageFile.bmp
WizardImageFile =Files\PercoloreWizard-100.bmp,Files\PercoloreWizard-125.bmp,Files\PercoloreWizard-150.bmp,Files\PercoloreWizard-175.bmp,Files\PercoloreWizard-200.bmp,Files\PercoloreWizard-225.bmp,Files\PercoloreWizard-250.bmp
AllowUNCPath =False
DiskSpanning =no
DisableProgramGroupPage =yes
DefaultGroupName =Percolore
DisableDirPage =yes
ShowLanguageDialog =yes
UninstallDisplayIcon ={uninstallexe}
;AlwaysRestart = yes

;Determina que deve ser criado apenas um executável com todas as informações da instalação
UseSetupLdr=yes

;O caminho de início dos arquivos de instalação é considerado
;a partir do diretório do arquivo de script .iss
OutputDir=..\dist
OutputBaseFilename={#DefAppName} {#DefAppVersion} {#DefAppArchitecture}

;Esta direitva serve para informar que, se a Instalação achar uma versão anterior instalada,
;deve substituir o nome do diretório padrão pelo diretório selecionado anteriormente.
UsePreviousAppDir=no

;Caminho padrão de instalação.
;Obtém o nome da unidade lógica de instalação do windows seguido do nome da empresa e da aplicação
DefaultDirName={sd}\{#DefAppCompany}\{#DefAppName}

[Languages]
Name: brazilianportuguese; MessagesFile: compiler:Languages\BrazilianPortuguese.isl
Name: spanish; MessagesFile: compiler:Languages\Spanish.isl
Name: english; MessagesFile: compiler:Default.isl

[CustomMessages]
;msgTaskAdicionarInicializacao
brazilianportuguese.msgTaskAdicionarInicializacao =Adicionar software à inicialização do Windows.
spanish.msgTaskAdicionarInicializacao =Agregar software al inicio de Windows.
english.msgTaskAdicionarInicializacao =Add software to the Windows initialization.

;msgTaskAdicionarIcones
brazilianportuguese.TaskAdicionarIcones =Criar ícones na área de trabalho e no menu iniciar.
spanish.TaskAdicionarIcones =Crear iconos en el escritorio y menú de inicio.
english.TaskAdicionarIcones =Create shortcuts to the Desktop and Start menu.

;msgNetInstalando
brazilianportuguese.msgNetInstalando =Instalando o runtime .NET %1...
spanish.msgNetInstalando =La instalación de runtime .NET %1...
english.msgNetInstalando =Installing .NET %1 runtime...

;msgNetWinDesktopInstalando
brazilianportuguese.msgNetWinDesktopInstalando =Instalando o runtime .NET %1 para Windows desktop...
spanish.msgNetWinDesktopInstalando =La instalación de runtime .NET %1 para Windows desktop...
english.msgNetWinDesktopInstalando =Installing .NET %1 runtime for Windows desktop...

;msgVCRedistInstalando
brazilianportuguese.msgVCRedistInstalando =Instalando o pacote redistribuível do Visual C++...
spanish.msgVCRedistInstalando =Instalando el paquete redistribuible de Visual C++...
english.msgVCRedistInstalando =Installing Visual C++ Redistributable Package...

;msgNetSucesso
brazilianportuguese.msgNetSucesso =Runtime do .NET %1 instalado com sucesso.
spanish.msgNetSucesso =Runtime del .NET %1 se ha completado con éxito.
english.msgNetSucesso =.NET Runtime %1 installed successfully.

;msgNetWinDesktopSucesso
brazilianportuguese.msgNetWinDesktopSucesso =Runtime do .NET %1 para Windows desktop instalado com sucesso.
spanish.msgNetWinDesktopSucesso =Runtime del .NET %1 para Windows desktop se ha completado con éxito.
english.msgNetWinDesktopSucesso =.NET Runtime %1 for Windows desktop installed successfully.

;msgVCRedistSucesso
brazilianportuguese.msgVCRedistSucesso =Pacote redistribuível do Visual C++ instalado com sucesso.
spanish.msgVCRedistSucesso =Paquete redistribuible de Visual C++ instalado correctamente.
english.msgVCRedistSucesso =Visual C++ Redistributable Package installed successfully.

;msgNetCancelado
brazilianportuguese.msgNetCancelado =Instalação do runtime do .NET %1 foi cancelada.
spanish.msgNetCancelado =La instalación de runtime del .NET %1 ha sido cancelada.
english.msgNetCancelado =Installation of .NET runtime %1 has been cancelled.

;msgNetWinDesktopCancelado
brazilianportuguese.msgNetWinDesktopCancelado =Instalação do runtime do .NET %1 para Windows desktop foi cancelada.
spanish.msgNetWinDesktopCancelado =La instalación de runtime del .NET %1 para Windows desktop ha sido cancelada.
english.msgNetWinDesktopCancelado =Installation of .NET runtime %1 for Windows desktop has been cancelled.

;msgVCRedistCancelado
brazilianportuguese.msgVCRedistCancelado =A instalação do pacote redistribuível do Visual C++ foi cancelada.
spanish.msgVCRedistCancelado =Se canceló la instalación del paquete redistribuible de Visual C++.
english.msgVCRedistCancelado =Installation of the Visual C++ redistributable package has been canceled.

;msgNetFalha
brazilianportuguese.msgNetFalha =A instalação do runtime .NET falhou.%nCódigo da falha: %s
spanish.msgNetFalha =La instalación del runtime .NET falló.%nCódigo de error: %s
english.msgNetFalha =.NET runtime installation failed.%nFailure code: %s

;msgNetWinDesktopFalha
brazilianportuguese.msgNetWinDesktopFalha =A instalação do runtime .NET para Windows desktop falhou.%nCódigo da falha: %s
spanish.msgNetWinDesktopFalha =La instalación del runtime .NET para Windows desktop falló.%nCódigo de error: %s
english.msgNetWinDesktopFalha =.NET runtime for Windows desktop installation failed.%nFailure code: %s

;msgVCRedistFalha
brazilianportuguese.msgVCRedistFalha =A instalação do pacote redistribuível do Visual C++ falhou.%nCódigo da falha: %s
spanish.msgVCRedistFalha =Error al instalar el paquete redistribuible de Visual C++.%nCódigo de error: %s
english.msgVCRedistFalha =Installation of the Visual C++ redistributable package failed.%nFailure code: %s

;msgArquiteturaNaoSuportada
brazilianportuguese.msgArquiteturaNaoSuportada =Arquitetura do sistema não suportada.
spanish.msgArquiteturaNaoSuportada =Arquitectura del sistema no compatible.
english.msgArquiteturaNaoSuportada =Unsupported system architecture.

;msgInicializacaoEncerrarIoconnect
brazilianportuguese.msgInicializacaoEncerrarIoconnect =Para prosseguir com a instalação, é necessário encerrar o IOConnect.
spanish.msgInicializacaoEncerrarIoconnect =Para proseguir con la instalación, es necesario cerrar el IOConnect.
english.msgInicializacaoEncerrarIoconnect =To continue with the installation, you must close IOConnect.

;msgInicializacaoVersaoJaInstalada
brazilianportuguese.msgInicializacaoVersaoJaInstalada =Esta versão já foi instalada na máquina.
spanish.msgInicializacaoVersaoJaInstalada =Esta versión ha sido instalada en la máquina.
english.msgInicializacaoVersaoJaInstalada =This version has already been installed on the machine.

;msgInicializacaoVersaoMaisRecenteInstalada
brazilianportuguese.msgInicializacaoVersaoMaisRecenteInstalada =Há uma versão mais recente instalada na máquina.
spanish.msgInicializacaoVersaoMaisRecenteInstalada =Hay una versión más reciente instalada en la máquina.
english.msgInicializacaoVersaoMaisRecenteInstalada =There is a newer version installed on the machine.

;msgFinalizandoInstalacao
brazilianportuguese.msgFinalizandoInstalacao =Finalizando a instalação. Aguarde enquanto o processo é concluído...
spanish.msgFinalizandoInstalacao =Finalizando la instalación. Espere mientras se completa el proceso...
english.msgFinalizandoInstalacao =Finishing the installation. Please wait while the process is completed...

[Tasks]
Name: TaskAdicionarInicializacao; Description: {cm:msgTaskAdicionarInicializacao}; Flags: checkedonce
Name: TaskAdicionarIcones; Description: {cm:TaskAdicionarIcones}; Flags: checkedonce

; Calibrações
Name: AFFACOR_TD_ANNETTA_ST; Description: Affacor TD - ANNETTA - Annetta ST; Flags: exclusive;
Name: Affatin_X_ANNETTA_ST; Description: Affatin X - ANNETTA - Annetta ST; Flags: exclusive;
Name: Affaton_K_ANNETTA_ST; Description: Affaton K - ANNETTA - Annetta ST; Flags: exclusive;
Name: BASF_SUVINIL_SELFCOLOR; Description: BASF SUVINIL - Selfcolor; Flags: exclusive;
Name: BLASCOR_BBF_CORES; Description: BLASCOR - BBF-Cores; Flags: exclusive;
Name: BRASILUX_BRASIMIX_IMOB; Description: BRASILUX - Brasimix Imob; Flags: exclusive;
Name: CIACOLLOR_BASETINT; Description: CIACOLLOR - Basetint; Flags: exclusive;
Name: CORAL_SHOTCOLOR_KUBO; Description: CORAL - Shotcolor; Flags: exclusive;
Name: CTED_CC_BASETINT; Description: CTED CC - Basetint; Flags: exclusive;
Name: DECOR_COLORS_ANNETTA_ST; Description: DECOR COLORS - Annetta ST; Flags: exclusive;
Name: EUCATEX_E_COLORS; Description: EUCATEX - e-colors; Flags: exclusive;
Name: HYDRONORTH_BBF_CORES; Description: HYDRONORTH - BBF-Cores; Flags: exclusive;
Name: INKOR_ANNETTA_ST; Description: INKOR - Annetta ST8; Flags: exclusive;
Name: KILLING_COLORLINE; Description: KILLING - Colorline; Flags: exclusive;
Name: MAZA_COLOR_MAZA_DNAXIS; Description: MAZA COLOR - Maza Dnaxis; Flags: exclusive;
Name: NEW_TINTAS_DOSADORA; Description: NEW TINTAS - Dosadora; Flags: exclusive;
Name: PPG_RENNER_THE_VOICE_OF_COLOR; Description: PPG RENNER - The Voice of Color; Flags: exclusive;
Name: RESICOLOR_DOSADORA; Description: RESICOLOR - Dosadora; Flags: exclusive;
Name: SHERWIN_WILLIAMS_BBF_CORES; Description: SHERWIN-WILLIAMS - BBF-Cores; Flags: exclusive;
Name: SINTEQUIMICA_IMOB_DOSADORA; Description: SINTEQUÍMICA IMOB - Dosadora; Flags: exclusive;
Name: TEDOX_ANNETTA_ST; Description: TEDOX - Annetta ST; Flags: exclusive;
Name: TEXTURA_E_CIA_BBF_CORES; Description: TEXTURA & CIA - BBF-Cores; Flags: exclusive;
Name: TEXWHITE_BASETINT; Description: TEXWHITE - Basetint; Flags: exclusive;
Name: TINTAS_NOBRE_DOSADORA; Description: TINTAS NOBRE - Dosadora; Flags: exclusive;


[LangOptions]
;LanguageID=$0416
RightToLeft=False

[Dirs]
Name: {app}\sistema; Flags: uninsneveruninstall
Name: {app}\bkp; Flags: 
Name: {app}\es; Flags: 
;Name: {app}\pt-BR; Flags: 

;[InstallDelete]
; Arquivos que devem ser removidos após a instalação


[Files]
Source: ..\dist\Publish\win-arm64\es\IOConnect.resources.dll; DestDir: {app}\es
Source: ..\dist\Publish\win-arm64\es\Percolore.Core.resources.dll; DestDir: {app}\es
Source: ..\dist\Publish\win-arm64\es\Treinamento.resources.dll; DestDir: {app}\es

Source: ..\dist\Publish\win-arm64\appsettings.json; DestDir: {app}
Source: ..\dist\Publish\win-arm64\EntityFramework.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\EntityFramework.SqlServer.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\FluentFTP.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Fractions.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Instalacao.deps.json; DestDir: {app}; Flags:deleteafterinstall;
Source: ..\dist\Publish\win-arm64\Instalacao.dll; DestDir: {app}; Flags:deleteafterinstall;
Source: ..\dist\Publish\win-arm64\Instalacao.exe; DestDir: {app}; Flags:deleteafterinstall;
Source: ..\dist\Publish\win-arm64\Instalacao.runtimeconfig.json; DestDir: {app}; Flags:deleteafterinstall;
Source: ..\dist\Publish\win-arm64\InTheHand.Net.Bluetooth.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\IOConnect.deps.json; DestDir: {app}
Source: ..\dist\Publish\win-arm64\IOConnect.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\IOConnect.dll.config; DestDir: {app}
Source: ..\dist\Publish\win-arm64\IOConnect.exe; DestDir: {app}
Source: ..\dist\Publish\win-arm64\IOConnect.runtimeconfig.json; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Configuration.Abstractions.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Configuration.Binder.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Configuration.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Configuration.FileExtensions.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Configuration.Json.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.DependencyInjection.Abstractions.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.DependencyInjection.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.DependencyModel.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.FileProviders.Abstractions.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.FileProviders.Physical.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.FileSystemGlobbing.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Logging.Abstractions.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Logging.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Options.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Microsoft.Extensions.Primitives.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Newtonsoft.Json.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Percolore.Core.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Serilog.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Serilog.Extensions.Logging.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Serilog.Settings.Configuration.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Serilog.Sinks.Console.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Serilog.Sinks.File.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\sni.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.Data.OleDb.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.Data.SqlClient.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.Data.SQLite.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.Data.SQLite.EF6.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.Drawing.Common.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.IO.Ports.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\System.Management.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Treinamento.deps.json; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Treinamento.dll; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Treinamento.dll.config; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Treinamento.exe; DestDir: {app}
Source: ..\dist\Publish\win-arm64\Treinamento.runtimeconfig.json; DestDir: {app}
Source: ..\dist\Publish\win-arm64\WSMBS.dll; DestDir: {app}

; Instalador Visual C++ Redistributable
Source: Installer\VC_redist.arm64.exe; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: InstallVCRedist; AfterInstall: ExecuteVCRedist

;Instalador do .NET 8
Source: Installer\dotnet-runtime-8.0.11-win-arm64.exe; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: InstallDotNetArm64; AfterInstall: InstallDotNet
Source: Installer\windowsdesktop-runtime-8.0.11-win-arm64.exe; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: InstallDotNetWinArm64; AfterInstall: InstallDotNetWindowsDesktop


;***************************
;*** Banco de Calibração ***
;***************************

;Affacor TD - ANNETTA - Annetta ST - Percolore AD-D8
Source: Files\Affacor TD - ANNETTA - Annetta ST - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: AFFACOR_TD_ANNETTA_ST
Source: Files\Affacor TD - ANNETTA - Annetta ST - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: AFFACOR_TD_ANNETTA_ST
Source: Files\Affacor TD - ANNETTA - Annetta ST - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: AFFACOR_TD_ANNETTA_ST
Source: Files\Affacor TD - ANNETTA - Annetta ST - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: AFFACOR_TD_ANNETTA_ST
Source: Files\Affacor TD - ANNETTA - Annetta ST - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: AFFACOR_TD_ANNETTA_ST

;Affatin X - ANNETTA - Annetta ST - Percolore AD-D8
Source: Files\Affatin X - ANNETTA - Annetta ST - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affatin_X_ANNETTA_ST
Source: Files\Affatin X - ANNETTA - Annetta ST - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affatin_X_ANNETTA_ST
Source: Files\Affatin X - ANNETTA - Annetta ST - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affatin_X_ANNETTA_ST
Source: Files\Affatin X - ANNETTA - Annetta ST - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affatin_X_ANNETTA_ST
Source: Files\Affatin X - ANNETTA - Annetta ST - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affatin_X_ANNETTA_ST

;Affaton K - ANNETTA - Annetta ST - Percolore AD-D8
Source: Files\Affaton K - ANNETTA - Annetta ST - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affaton_K_ANNETTA_ST
Source: Files\Affaton K - ANNETTA - Annetta ST - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affaton_K_ANNETTA_ST
Source: Files\Affaton K - ANNETTA - Annetta ST - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affaton_K_ANNETTA_ST
Source: Files\Affaton K - ANNETTA - Annetta ST - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affaton_K_ANNETTA_ST
Source: Files\Affaton K - ANNETTA - Annetta ST - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: Affaton_K_ANNETTA_ST

;BASF SUVINIL - Selfcolor - Percolore AD-D8
Source: Files\BASF SUVINIL - Selfcolor - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BASF_SUVINIL_SELFCOLOR
Source: Files\BASF SUVINIL - Selfcolor - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BASF_SUVINIL_SELFCOLOR
Source: Files\BASF SUVINIL - Selfcolor - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BASF_SUVINIL_SELFCOLOR
Source: Files\BASF SUVINIL - Selfcolor - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BASF_SUVINIL_SELFCOLOR
Source: Files\BASF SUVINIL - Selfcolor - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BASF_SUVINIL_SELFCOLOR

;BLASCOR - BBF-Cores - Percolore AD-D8
Source: Files\BLASCOR - BBF-Cores - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BLASCOR_BBF_CORES
Source: Files\BLASCOR - BBF-Cores - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BLASCOR_BBF_CORES
Source: Files\BLASCOR - BBF-Cores - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BLASCOR_BBF_CORES
Source: Files\BLASCOR - BBF-Cores - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BLASCOR_BBF_CORES
Source: Files\BLASCOR - BBF-Cores - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BLASCOR_BBF_CORES

;BRASILUX - Brasimix Imob - Percolore AD-D8
Source: Files\BRASILUX - Brasimix Imob - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BRASILUX_BRASIMIX_IMOB
Source: Files\BRASILUX - Brasimix Imob - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BRASILUX_BRASIMIX_IMOB
Source: Files\BRASILUX - Brasimix Imob - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BRASILUX_BRASIMIX_IMOB
Source: Files\BRASILUX - Brasimix Imob - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BRASILUX_BRASIMIX_IMOB
Source: Files\BRASILUX - Brasimix Imob - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: BRASILUX_BRASIMIX_IMOB

;CIACOLLOR - Basetint - Percolore AD-D8
Source: Files\CIACOLLOR - Basetint - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CIACOLLOR_BASETINT
Source: Files\CIACOLLOR - Basetint - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CIACOLLOR_BASETINT
Source: Files\CIACOLLOR - Basetint - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CIACOLLOR_BASETINT
Source: Files\CIACOLLOR - Basetint - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CIACOLLOR_BASETINT
Source: Files\CIACOLLOR - Basetint - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CIACOLLOR_BASETINT

;CORAL - Shotcolor - Percolore KUBO
Source: Files\CORAL - Shotcolor - Percolore KUBO\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CORAL_SHOTCOLOR_KUBO
Source: Files\CORAL - Shotcolor - Percolore KUBO\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CORAL_SHOTCOLOR_KUBO
Source: Files\CORAL - Shotcolor - Percolore KUBO\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CORAL_SHOTCOLOR_KUBO
Source: Files\CORAL - Shotcolor - Percolore KUBO\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CORAL_SHOTCOLOR_KUBO
Source: Files\CORAL - Shotcolor - Percolore KUBO\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CORAL_SHOTCOLOR_KUBO

;CTED CC - Basetint - Percolore AD-D8
Source: Files\CTED CC - Basetint - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CTED_CC_BASETINT
Source: Files\CTED CC - Basetint - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CTED_CC_BASETINT
Source: Files\CTED CC - Basetint - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CTED_CC_BASETINT
Source: Files\CTED CC - Basetint - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CTED_CC_BASETINT
Source: Files\CTED CC - Basetint - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: CTED_CC_BASETINT

;DECOR COLORS - Annetta ST - Percolore AD-D8
Source: Files\DECOR COLORS - Annetta ST - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: DECOR_COLORS_ANNETTA_ST
Source: Files\DECOR COLORS - Annetta ST - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: DECOR_COLORS_ANNETTA_ST
Source: Files\DECOR COLORS - Annetta ST - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: DECOR_COLORS_ANNETTA_ST
Source: Files\DECOR COLORS - Annetta ST - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: DECOR_COLORS_ANNETTA_ST
Source: Files\DECOR COLORS - Annetta ST - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: DECOR_COLORS_ANNETTA_ST

;EUCATEX - e-colors - Percolore AD-D8
Source: Files\EUCATEX - e-colors - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: EUCATEX_E_COLORS
Source: Files\EUCATEX - e-colors - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: EUCATEX_E_COLORS
Source: Files\EUCATEX - e-colors - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: EUCATEX_E_COLORS
Source: Files\EUCATEX - e-colors - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: EUCATEX_E_COLORS
Source: Files\EUCATEX - e-colors - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: EUCATEX_E_COLORS

;HYDRONORTH - BBF-Cores - Percolore AD-D8
Source: Files\HYDRONORTH - BBF-Cores - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: HYDRONORTH_BBF_CORES
Source: Files\HYDRONORTH - BBF-Cores - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: HYDRONORTH_BBF_CORES
Source: Files\HYDRONORTH - BBF-Cores - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: HYDRONORTH_BBF_CORES
Source: Files\HYDRONORTH - BBF-Cores - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: HYDRONORTH_BBF_CORES
Source: Files\HYDRONORTH - BBF-Cores - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: HYDRONORTH_BBF_CORES

;INKOR - Annetta ST - Percolore AD-D8
Source: Files\INKOR - Annetta ST - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: INKOR_ANNETTA_ST
Source: Files\INKOR - Annetta ST - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: INKOR_ANNETTA_ST
Source: Files\INKOR - Annetta ST - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: INKOR_ANNETTA_ST
Source: Files\INKOR - Annetta ST - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: INKOR_ANNETTA_ST
Source: Files\INKOR - Annetta ST - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: INKOR_ANNETTA_ST

;KILLING - Colorline - Percolore AD-D8
Source: Files\KILLING - Colorline - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: KILLING_COLORLINE
Source: Files\KILLING - Colorline - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: KILLING_COLORLINE
Source: Files\KILLING - Colorline - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: KILLING_COLORLINE
Source: Files\KILLING - Colorline - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: KILLING_COLORLINE
Source: Files\KILLING - Colorline - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: KILLING_COLORLINE

;MAZA COLOR - Maza Dnaxis - Percolore AD-D8
Source: Files\MAZA COLOR - Maza Dnaxis - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: MAZA_COLOR_MAZA_DNAXIS
Source: Files\MAZA COLOR - Maza Dnaxis - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: MAZA_COLOR_MAZA_DNAXIS
Source: Files\MAZA COLOR - Maza Dnaxis - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: MAZA_COLOR_MAZA_DNAXIS
Source: Files\MAZA COLOR - Maza Dnaxis - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: MAZA_COLOR_MAZA_DNAXIS
Source: Files\MAZA COLOR - Maza Dnaxis - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: MAZA_COLOR_MAZA_DNAXIS

;NEW TINTAS - Dosadora - Percolore AD-D8
Source: Files\NEW TINTAS - Dosadora - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: NEW_TINTAS_DOSADORA
Source: Files\NEW TINTAS - Dosadora - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: NEW_TINTAS_DOSADORA
Source: Files\NEW TINTAS - Dosadora - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: NEW_TINTAS_DOSADORA
Source: Files\NEW TINTAS - Dosadora - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: NEW_TINTAS_DOSADORA
Source: Files\NEW TINTAS - Dosadora - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: NEW_TINTAS_DOSADORA

;PPG RENNER - The Voice of Color - Percolore AD-D8
Source: Files\PPG RENNER - The Voice of Color - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: PPG_RENNER_THE_VOICE_OF_COLOR
Source: Files\PPG RENNER - The Voice of Color - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: PPG_RENNER_THE_VOICE_OF_COLOR
Source: Files\PPG RENNER - The Voice of Color - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: PPG_RENNER_THE_VOICE_OF_COLOR
Source: Files\PPG RENNER - The Voice of Color - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: PPG_RENNER_THE_VOICE_OF_COLOR
Source: Files\PPG RENNER - The Voice of Color - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: PPG_RENNER_THE_VOICE_OF_COLOR

;RESICOLOR - Dosadora - Percolore AD-D8
Source: Files\RESICOLOR - Dosadora - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: RESICOLOR_DOSADORA
Source: Files\RESICOLOR - Dosadora - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: RESICOLOR_DOSADORA
Source: Files\RESICOLOR - Dosadora - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: RESICOLOR_DOSADORA
Source: Files\RESICOLOR - Dosadora - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: RESICOLOR_DOSADORA
Source: Files\RESICOLOR - Dosadora - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: RESICOLOR_DOSADORA

;SHERWIN-WILLIAMS - BBF-Cores - Percolore AD-D8
Source: Files\SHERWIN-WILLIAMS - BBF-Cores - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SHERWIN_WILLIAMS_BBF_CORES
Source: Files\SHERWIN-WILLIAMS - BBF-Cores - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SHERWIN_WILLIAMS_BBF_CORES
Source: Files\SHERWIN-WILLIAMS - BBF-Cores - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SHERWIN_WILLIAMS_BBF_CORES
Source: Files\SHERWIN-WILLIAMS - BBF-Cores - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SHERWIN_WILLIAMS_BBF_CORES
Source: Files\SHERWIN-WILLIAMS - BBF-Cores - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SHERWIN_WILLIAMS_BBF_CORES

;SINTEQUÍMICA IMOB - Dosadora - Percolore AD-D8
Source: Files\SINTEQUÍMICA IMOB - Dosadora - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SINTEQUIMICA_IMOB_DOSADORA
Source: Files\SINTEQUÍMICA IMOB - Dosadora - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SINTEQUIMICA_IMOB_DOSADORA
Source: Files\SINTEQUÍMICA IMOB - Dosadora - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SINTEQUIMICA_IMOB_DOSADORA
Source: Files\SINTEQUÍMICA IMOB - Dosadora - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SINTEQUIMICA_IMOB_DOSADORA
Source: Files\SINTEQUÍMICA IMOB - Dosadora - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: SINTEQUIMICA_IMOB_DOSADORA

;TEDOX - Annetta ST - Percolore AD-D8
Source: Files\TEDOX - Annetta ST - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEDOX_ANNETTA_ST
Source: Files\TEDOX - Annetta ST - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEDOX_ANNETTA_ST
Source: Files\TEDOX - Annetta ST - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEDOX_ANNETTA_ST
Source: Files\TEDOX - Annetta ST - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEDOX_ANNETTA_ST
Source: Files\TEDOX - Annetta ST - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEDOX_ANNETTA_ST

;TEXTURA & CIA - BBF-Cores - Percolore AD-D8
Source: Files\TEXTURA & CIA - BBF-Cores - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXTURA_E_CIA_BBF_CORES
Source: Files\TEXTURA & CIA - BBF-Cores - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXTURA_E_CIA_BBF_CORES
Source: Files\TEXTURA & CIA - BBF-Cores - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXTURA_E_CIA_BBF_CORES
Source: Files\TEXTURA & CIA - BBF-Cores - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXTURA_E_CIA_BBF_CORES
Source: Files\TEXTURA & CIA - BBF-Cores - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXTURA_E_CIA_BBF_CORES

;TEXWHITE - Basetint - Percolore AD-D8
Source: Files\TEXWHITE - Basetint - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXWHITE_BASETINT
Source: Files\TEXWHITE - Basetint - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXWHITE_BASETINT
Source: Files\TEXWHITE - Basetint - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXWHITE_BASETINT
Source: Files\TEXWHITE - Basetint - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXWHITE_BASETINT
Source: Files\TEXWHITE - Basetint - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TEXWHITE_BASETINT

;TINTAS NOBRE - Dosadora - Percolore AD-D8
Source: Files\TINTAS NOBRE - Dosadora - Percolore AD-D8\Calibragem.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TINTAS_NOBRE_DOSADORA
Source: Files\TINTAS NOBRE - Dosadora - Percolore AD-D8\Colorantes.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TINTAS_NOBRE_DOSADORA
Source: Files\TINTAS NOBRE - Dosadora - Percolore AD-D8\Formulas.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TINTAS_NOBRE_DOSADORA
Source: Files\TINTAS NOBRE - Dosadora - Percolore AD-D8\Parametros.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TINTAS_NOBRE_DOSADORA
Source: Files\TINTAS NOBRE - Dosadora - Percolore AD-D8\Recircular.xml; DestDir: {app}; Flags: onlyifdoesntexist; Tasks: TINTAS_NOBRE_DOSADORA


[Icons]
Name: {commondesktop}\{#DefAppName}; Filename: {app}\{#DefAppName}; Tasks: TaskAdicionarIcones
Name: {group}\{#DefAppName}; Filename: {app}\{#DefAppName}; Tasks: TaskAdicionarIcones
Name: {userstartup}\{#DefAppName}; Filename: {app}\{#DefAppName}; WorkingDir: {app}; IconFilename: {app}\{#DefAppName}; Tasks: TaskAdicionarInicializacao

[Registry]
;

[Run]
Filename: {app}\Instalacao.exe; Flags: waituntilterminated; StatusMsg: {cm:msgFinalizandoInstalacao}
Filename: {app}\{#DefAppExeFile}; Flags: nowait postinstall skipifsilent; Description: {cm:LaunchProgram,{#StringChange(DefAppExeFile, '&', '&&')}}


[Code]
#include "Installer\Scripts\stringVersion.iss"
#include "Installer\Scripts\fileReplaceString.iss"
#include "Installer\Scripts\getAppValueRegister.iss"
#include "Installer\Scripts\isAppRunning.iss"
#include "Installer\Scripts\updateStructure.iss"

(*--------------------------------------------------------------------------------------------
  Instalação do .NET
  --------------------------------------------------------------------------------------------
  O código das funções abaixo recupera a versão do .NET instalada na máquina e se for mais
  baixa que a verão do .NET alvo, instala o pacote do .NET correspondente.
  --------------------------------------------------------------------------------------------}*)
function GetSystemArchitecture: String;
var
  Arch: String;
begin
  // Detecta ARM64 com fallback
  if RegQueryStringValue(HKLM, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'PROCESSOR_ARCHITECTURE', Arch) then
  begin
    if Arch = 'ARM64' then
      Result := 'arm64'
    else if Arch = 'AMD64' then
      Result := 'x64'
    else
      Result := 'x86';
  end else
  begin
    // Fallback para sistemas legados
    if IsWin64 then
      Result := 'x64'
    else
      Result := 'x86';
  end;
end;

function InstallDotNet8(architecture: String; platform: String): Boolean;
var
  regAddress: String;
  isNet8Installed: Cardinal;
begin
  Result := true;  
  regAddress := 'Software\dotnet\Setup\InstalledVersions\' + architecture + '\sharedfx\' + platform
  
  if (RegQueryDWordValue(HKLM, regAddress, '{#DefAppNetVersion}', isNet8Installed)) then
  begin  
    if (isNet8Installed = 1) then
    begin
      Result := false;
    end
  end;
end;

function InstallDotNetArm64(): Boolean;
begin
  Result := InstallDotNet8('arm64', 'Microsoft.NETCore.App');
end;

function InstallDotNetWinArm64(): Boolean;
begin
  Result := InstallDotNet8('arm64', 'Microsoft.WindowsDesktop.App');
end;

function InstallVCRedist(): Boolean;
var
  InstallKey: String;
  IsInstalled: Cardinal;
begin
  Result := True;
  // Verifica a existência da chave para o VC_redist.arm64
  InstallKey := 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\arm64';

  // Verifica em HKLM se o redistributable está instalado (32 ou 64 bits)
  if RegQueryDWordValue(HKLM, InstallKey, 'Installed', IsInstalled) then
  begin
    if (IsInstalled = 1) then
    begin
      Result := False;
    end;
  end;
end;

procedure ExecuteVCRedist;
var
  StatusText: string;
  ResultCode: Integer;
  Installer: string;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := ExpandConstant('{cm:msgVCRedistInstalando}');
  WizardForm.ProgressGauge.Style := npbstMarquee;

  Installer := ExpandConstant('{tmp}\VC_redist.arm64.exe');

  // Executa o instalador
  try
    if Exec(Installer, '/quiet /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      if (ResultCode = 0) then
      begin
        StatusText := ExpandConstant('{cm:msgVCRedistSucesso}');
      end
      else if (ResultCode = 1602) then
      begin
        StatusText := ExpandConstant('{cm:msgVCRedistCancelado}');
        WizardForm.Close;
      end
      else
      begin
        StatusText := Format(ExpandConstant('{cm:msgVCRedistFalha}'), [IntToStr(ResultCode)]);
        MsgBox(StatusText, mbError, MB_OK);
        WizardForm.Close;
      end;
    end
    else
    begin
      StatusText := Format(ExpandConstant('{cm:msgVCRedistFalha}'), [IntToStr(ResultCode)]);
      MsgBox(StatusText, mbError, MB_OK);
      WizardForm.Close;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
  end;
end;

procedure InstallDotNetWindowsDesktop;
var
  StatusText: string;
  ResultCode: Integer;
  DotNetInstaller: string;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := ExpandConstant('{cm:msgNetWinDesktopInstalando,{#DefAppNetVersion}}');
  WizardForm.ProgressGauge.Style := npbstMarquee;

  DotNetInstaller := ExpandConstant('{tmp}\windowsdesktop-runtime-8.0.11-win-arm64.exe');

  // Executa o instalador
  try
    if Exec(DotNetInstaller, '/quiet /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      if (ResultCode = 0) then
      begin
        StatusText := ExpandConstant('{cm:msgNetWinDesktopSucesso,{#DefAppNetVersion}}');
      end
      else if (ResultCode = 1602) then
      begin
        StatusText := ExpandConstant('{cm:msgNetWinDesktopCancelado,{#DefAppNetVersion}}');
        WizardForm.Close;
      end
      else
      begin
        StatusText := Format(ExpandConstant('{cm:msgNetWinDesktopFalha}'), [IntToStr(ResultCode)]);
        MsgBox(StatusText, mbError, MB_OK);
        WizardForm.Close;
      end;
    end
    else
    begin
      StatusText := Format(ExpandConstant('{cm:msgNetWinDesktopFalha}'), [IntToStr(ResultCode)]);
      MsgBox(StatusText, mbError, MB_OK);
      WizardForm.Close;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
  end;
end;

procedure InstallDotNet;
var
  StatusText: string;
  ResultCode: Integer;
  DotNetInstaller: string;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := ExpandConstant('{cm:msgNetInstalando,{#DefAppNetVersion}}');
  WizardForm.ProgressGauge.Style := npbstMarquee;

  DotNetInstaller := ExpandConstant('{tmp}\dotnet-runtime-8.0.11-win-arm64.exe');

  // Executa o instalador
  try
    if Exec(DotNetInstaller, '/quiet /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      if (ResultCode = 0) then
      begin
        StatusText := ExpandConstant('{cm:msgNetSucesso,{#DefAppNetVersion}}');
      end
      else if (ResultCode = 1602) then
      begin
        StatusText := ExpandConstant('{cm:msgNetCancelado,{#DefAppNetVersion}}');
        WizardForm.Close;
      end
      else
      begin
        StatusText := Format(ExpandConstant('{cm:msgNetFalha}'), [IntToStr(ResultCode)]);
        MsgBox(StatusText, mbError, MB_OK);
        WizardForm.Close;
      end;
    end
    else
    begin
      StatusText := Format(ExpandConstant('{cm:msgNetFalha}'), [IntToStr(ResultCode)]);
      MsgBox(StatusText, mbError, MB_OK);
      WizardForm.Close;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
  end;
end;

function IsDefaultTaskSelected: Boolean;
begin
  Result := True; // Retorna True para marcar a task como selecionada por padrão
end;

(***********************************************************************************************
 Eventos disparados pelo Inno Setup
 ************************************************************************************************)
procedure CancelButtonClick(CurPageID: Integer; var Cancel, Confirm: Boolean);
begin
  if CurPageID = wpInstalling then
    Confirm := true;
    //Confirm := not CancelWithoutPrompt;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin

  //Quando instalação estiver concuída
  //if (CurStep = ssDone )then
  //begin

    //Se houver versão mais antiga instalada e se o caminho de instação
    //houver sido alterado, atualiza estrutura de instalação
   // if (bOldVersionExists and bOldInstallationPathChange) then
    //begin
   //   UpdateStructure(sOldInstallationPath, sNewInstallationPath, sOldUninstallationPath);
   // end;
 // end
end;

function InitializeSetup: boolean;
begin
  Result := False;

  //Se o executável do ioconnect estiver em execução, a instalação deve ser abortada
  if IsAppRunning('IOConnect.exe') then begin
    MsgBox(ExpandConstant('{cm:msgInicializacaoEncerrarIoconnect}'), mbError, MB_OK);
    exit;
  end;

  Result := True;
end;
