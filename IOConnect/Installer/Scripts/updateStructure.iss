(*-------------------------------------------------------------------------------------------
  Aplica alterações na estrutura de diretório e arquivos.
  Essas alterações são necessárias a partir de versão 1.5.11
  -------------------------------------------------------------------------------------------*)
procedure UpdateStructure(sOldInstallationPath : String; sNewInstallationPath : String; sOldUninstallPath : String);
var
  sFilename : String;
  sDirectoryPath : String;

begin
 
  (*Remove arquivos da instalação anterior.
    Os arquivos serão excluídos manualmente pois não havia como contornar o problema de 
    exclusão da dll compartilhada percolore.Core.dll haja vista que os desinstaladores 
    gerados antes da versão 1.5.11 não marcava a dll como SharedFile.     
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

  //Arquivos e diretórios compartilhados
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
 
  (*Copia arquivos xml para novo diretório de instalação e remove 
  do diretório de instalação anterior. *)    
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

  (*Verifica diretórios possam estar armazenados na raiz do diretório de instalação.
    Os dirdetórios terão todos os arquivos e subpastas deletados.*)

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

  //Remove arquivos que possam estar armazenados na raiz do diretório de instalação.    
  DelTree(sOldInstallationPath + '\*.err', False, True, False);  
  DelTree(sOldInstallationPath + '\*.dat', False, True, False);
  DelTree(sOldInstallationPath + '\*.log', False, True, False);


  //Atualiza caminho de instalação no conteúdo do arquivo xml
  sFilename := sNewInstallationPath + '\Parametros.xml'
  if(FileExists(sFilename)) then begin
    FileReplaceString(sFilename, 'C:\Percolore', sNewInstallationPath);  
  end;

end;