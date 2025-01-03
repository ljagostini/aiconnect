(*--------------------------------------------------------------------------------------------
  Retorno
    1 = v1 é menor
    2 = versões iguais
    3 = v2 é maior
  --------------------------------------------------------------------------------------------*)
function CompareStringVersion(V1, V2: string): Integer;
var
  P, N1, N2: Integer;
begin
  Result := 2;

  while (Result = 2) and ((V1 <> '') or (V2 <> '')) do
  begin
    P := Pos('.', V1);
    if P > 0 then
    begin
      N1 := StrToInt(Copy(V1, 1, P - 1));
      Delete(V1, 1, P);
    end
      else
    if V1 <> '' then
    begin
      N1 := StrToInt(V1);
      V1 := '';
    end
      else
    begin
      N1 := 0;
    end;

    P := Pos('.', V2);
    if P > 0 then
    begin
      N2 := StrToInt(Copy(V2, 1, P - 1));
      Delete(V2, 1, P);
    end
      else
    if V2 <> '' then
    begin
      N2 := StrToInt(V2);
      V2 := '';
    end
      else
    begin
      N2 := 0;
    end;

    if N1 < N2 then Result := 1
      else
    if N1 > N2 then Result := 3;
  end;
end;