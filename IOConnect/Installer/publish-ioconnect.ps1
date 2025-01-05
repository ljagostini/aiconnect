param (
    [string]$target = "win-x86" # Arquitetura padrão, caso não seja fornecida
)

# Lista de arquiteturas válidas
$validArchitectures = @("win-x86", "win-x64", "win-arm64")

# Validação da arquitetura
if (-not ($target -in $validArchitectures)) {
    Write-Host "Erro: O target '$target' não é válido." -ForegroundColor Red
    Write-Host "Escolha uma das arquiteturas disponíveis: win-x86, win-x64 ou win-arm64." -ForegroundColor Yellow
    exit 1 # Finaliza o script com código de erro
}

# Caminho do arquivo .csproj
$projectFileTreinamento = Join-Path $PSScriptRoot "..\..\Treinamento\Treinamento.csproj"
$projectFileInstalacao = Join-Path $PSScriptRoot "..\..\Instalacao\Instalacao.csproj"
$projectFileIOConnect = Join-Path $PSScriptRoot "..\IOConnect.csproj"

# Caminho de saída do publish
$outputDir = Join-Path $PSScriptRoot "..\..\dist\Publish\$target"

# Script do instalador
$installerScript = Join-Path $PSScriptRoot "..\IOConnectInstallScript.iss"

# Caminho do executavel Inno Setup
$innoSetupDir = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

# Limpa a pasta de publish
if (Test-Path $outputDir) {
	Remove-Item $outputDir"\*" -Recurse -Force
}

# Função para executar o publish
function Publish {
    param (
        [string]$project
    )

    dotnet publish $project --configuration Release --self-contained false --runtime $target --output $outputDir -v diag
}

# Comandos para publish
Publish -project $projectFileTreinamento
Publish -project $projectFileInstalacao
Publish -project $projectFileIOConnect