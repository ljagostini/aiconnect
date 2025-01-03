function FileReplaceString(const FilePath, SearchString, ReplaceString: string):boolean;
var
  MyFile : TStrings;
  MyText : string;
begin
  MyFile := TStringList.Create;

  try
    result := true;

    try
      MyFile.LoadFromFile(FilePath);
      MyText := MyFile.Text;

      //Only save if text has been changed.
      if StringChangeEx(MyText, SearchString, ReplaceString, True) > 0 then
      begin;
        MyFile.Text := MyText;
        MyFile.SaveToFile(FilePath);
      end;
    except
      result := false;
    end;
  finally
    MyFile.Free;
  end;
end;