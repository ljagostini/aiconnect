(*-------------------------------------------------------------------------------------------
  Recupera um valor na chave de registro criada pelo inno setup na instalação da aplicação
--------------------------------------------------------------------------------------------*)
function GetAppValueRegister(sValue : String): String;
var
  sAppKey: String;
  sReturn: String;
begin
  sReturn := '';
  sAppKey := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');;  
  
  if not RegQueryStringValue(HKLM, sAppKey, sValue, sReturn) then
    RegQueryStringValue(HKCU, sAppKey, sValue, sReturn);
  
  Result := RemoveQuotes(sReturn);
end;
