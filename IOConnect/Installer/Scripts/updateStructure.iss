(*-------------------------------------------------------------------------------------------
  Aplica altera��es na estrutura de diret�rio e arquivos.
  Essas altera��es s�o necess�rias a partir de vers�o 1.5.11
  -------------------------------------------------------------------------------------------*)
procedure UpdateStructure(sOldInstallationPath : String; sNewInstallationPath : String; sOldUninstallPath : String);
var
  sFilename : String;
  sDirectoryPath : String;

begin
 
  (*Remove arquivos da instala��o anterior.
    Os arquivos ser�o exclu�dos manualmente pois n�o havia como contornar o problema de 
    exclus�o da dll compartilhada percolore.Core.dll haja vista que os desinstaladores 
    gerados antes da vers�o 1.5.11 n�o marcava a dll como SharedFile.     
    Se o arquivo Percolore.Core.dll for deletado, pode ser que Dosadora deixe de 
    funcionar corretamente. *)

    sFilename := '\IOConnect.exe.Config';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

    sFilename := '\IOConnect.exe.Manifest';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

    sFilename := '\IOConnect.exe';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

    sFilename := '\WSMBS.dll';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

    sFilename := sOldInstallationPath + '\FlatTabControl.dll';
    if(FileExists(sFilename)) then begin
      DelayDeleteFile(sFilename, 2);
    end;

    if(FileExists(sOldUninstallPath)) then begin
      DelayDeleteFile(sOldUninstallPath, 2);
    end;

  //Arquivos e diret�rios compartilhados
  if(FileExists(sOldInstallationPath + '\Dosadora.exe')) then
  begin    
      
    sFilename := '\es\IOConnect.resources.dll';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

    sFilename := '\es\Percolore.Core.resources.dll';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

  end
  else begin
    DelTree(sOldInstallationPath + '\es', True, True, True);

    sFilename := '\Percolore.Core.dll';
    if(FileExists(sOldInstallationPath + sFilename)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;

  end;
 
  (*Copia arquivos xml para novo diret�rio de instala��o e remove 
  do diret�rio de instala��o anterior. *)    
  sFilename := '\Parametros.xml';
  if(FileExists(sOldInstallationPath + sFilename)) then begin
    if (FileCopy(sOldInstallationPath + sFilename, sNewInstallationPath + sFileName, false)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;
  end;
  
  sFilename := '\Calibragem.xml';
  if(FileExists(sOldInstallationPath + sFilename)) then begin
    if (FileCopy(sOldInstallationPath + sFilename, sNewInstallationPath + sFileName, false)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;
  end;

  sFilename := '\Formulas.xml';
  if(FileExists(sOldInstallationPath + sFilename)) then begin
    if (FileCopy(sOldInstallationPath + sFilename, sNewInstallationPath + sFileName, false)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;
  end;
                         
  sFilename := '\Colorantes.xml';
  if(FileExists(sOldInstallationPath + sFilename)) then begin
    if (FileCopy(sOldInstallationPath + sFilename, sNewInstallationPath + sFileName, false)) then begin
      DelayDeleteFile(sOldInstallationPath + sFilename, 2);
    end;
  end;

  (*Verifica diret�rios possam estar armazenados na raiz do diret�rio de instala��o.
    Os dirdet�rios ter�o todos os arquivos e subpastas deletados.*)

  //sistema
  sDirectoryPath := sOldInstallationPath + '\sistema';
  if DirExists(sDirectoryPath) then 
  begin
    DelTree(sDirectoryPath, True, True, True);
  end;

  //dat_old
  sDirectoryPath := sOldInstallationPath + '\dat_old';
  if DirExists(sDirectoryPath) then 
  begin
    DelTree(sDirectoryPath, True, True, True);
  end;

  //log
  sDirectoryPath := sOldInstallationPath + '\log';
  if DirExists(sDirectoryPath) then 
  begin
    DelTree(sDirectoryPath, True, True, True);
  end;

  //Remove arquivos que possam estar armazenados na raiz do diret�rio de instala��o.    
  DelTree(sOldInstallationPath + '\*.err', False, True, False);  
  DelTree(sOldInstallationPath + '\*.dat', False, True, False);
  DelTree(sOldInstallationPath + '\*.log', False, True, False);


  //Atualiza caminho de instala��o no conte�do do arquivo xml
  sFilename := sNewInstallationPath + '\Parametros.xml'
  if(FileExists(sFilename)) then begin
    FileReplaceString(sFilename, 'C:\Percolore', sNewInstallationPath);  
  end;

end;