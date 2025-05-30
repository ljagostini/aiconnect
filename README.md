
# Introdução

A solução *Tintometria.NET8.sln* contém o código-fonte dos projetos de automação da Percolore, atualizados para a plataforma .NET 8. Os seguintes projetos estão contidos na solução:

 - Core
 - DataHoraPercolore
 - Gerador
 - Instalacao
 - IOConnect
 - Treinamento
 - UpdateConfig
 
# Começando
Para a compilação e execução da aplicação recomenda-se utilizar o Visual Studio 2022. A versão Community do Visual Studio é gratuita e pode ser obtido em https://visualstudio.microsoft.com/pt-br/vs/community/

### Processo de Instalação

Após baixar o instalador do Visual Studio 2022, clique na aba "Componentes Individuais" (Individual Components) e marque as seguintes cargas de trabalho:

**.NET**
 
 - .NET 8 Runtime (Long Term Support)
 - SDK do .NET

**Atividades de Desenvolvimento**

 - C# e Visual Basic
 
 **SDKs, bibliotecas e estruturas**
 
 - Ferramentas do Entity Framework 6
 - SDK do Visual Studio
 - SDK do Windows 10 (10.0.18362,0)
 - SDK do Windows 11 (10.0.22621.0)

Após selecionar todas as cargas de trabalho acima, prossiga normalmente com a instalação.
Quando o processo de instalação do Visual Studio 2022 estiver concluído, reinicie o computador.

# Compilação e Teste

Para compilar e testar a solução *Tintometria.NET8.sln*,  abra-a com o Visual Studio 2022.
As bibliotecas de terceiros utilizadas pelo projeto estão na pasta "*libs*", na raíz do repositório. Elas já estão referenciadas dentro dos respectivos projetos a partir dessa pasta.

Para incluir novas bibliotecas de terceiros, elas devem ser salvas na pasta "libs" e referenciadas no projeto a partir desse local. Isso evita o download e referência em locais diferentes sempre que a solução for baixada do repositório para um novo computador.

**Dependências de Software**

Todos os projetos migrados para o .NET 8 possuem suas dependências resolvidas via pacotes Nuget. Ao compilar o projeto pela primeira vez, todas as dependências serão baixadas automaticamente, bastando apenas ter conexão com a internet.

Para a correta execução do *IOConnect*, se faz necessário alguns arquivos de configuração em formato xml para a primeira execução do programa. Esses arquivos são responsáveis por criar os bancos de dados SQLite com os parâmetros de inicialização e execução do software. Esses arquivos são:

 - Calibragem.xml
 - Colorantes.xml
 - Formulas.xml
 - Parametros.xml

Esses arquivos são necessários apenas na primeira execução, sendo excluídos pelo próprio programa após criar os bancos de dados SQLite, sendo desnecessários nas próximas execuções.

Para assegurar a correta execução em ambiente local, esses arquivos estão versionados junto ao código-fonte e são copiados para o diretório final onde o software será compilado. Dessa forma, sempre que for executado localmente via Debug ou em modo Release, o software sempre executará de forma correta, sem erros.

Caso os parâmetros iniciais devam ser alterados, edite os arquivos xml necessários e os versione novamente no repositório.

As bibliotecas do SQLite foram atualizadas para a versão 1.0.118, não sendo mais necessário incluir uma senha para criptografia do banco de dados. Por esse motivo a aplicação passou a utilizar o componente `Util.SQLite` para gerenciamento da string de conexão, que não utiliza mais a senha para conexão ao banco de dados.

## Compilação
Para compilar a Solução *Tintometria.NET8.sln*, siga os passos a seguir:

### Passo 1: Abrir a Solução no Visual Studio

1. Abra o Visual Studio.
2. No menu principal, clique em **File** (Arquivo) > **Open** (Abrir) > **Project/Solution** (Projeto/Solução).
3. Navegue até o diretório onde a solução `Tintometria.NET8.sln` está localizada e selecione o arquivo.
4. Clique em **Open** (Abrir).

### Passo 2: Selecionar o Modo de Compilação

1. Com a solução aberta, observe a barra superior do Visual Studio.
2. No lado esquerdo, você verá um menu suspenso que pode estar com a opção **Debug** selecionada. Este é o **Modo de Compilação**.
   - **Debug**: Usado para desenvolvimento e testes. Gera binários com informações adicionais para facilitar a depuração.
   - **Release**: Usado para a versão final do software. Gera binários otimizados, sem informações de depuração.
3. Clique no menu suspenso e selecione **Debug** ou **Release**, conforme necessário.

### Passo 3: Selecionar a Arquitetura do Processador

1. Ao lado do modo de compilação, você verá outro menu suspenso que pode estar marcado como **Any CPU**.
   - **Any CPU**: Permite que o software seja executado em qualquer arquitetura de processador (x86, x64).
   - **x86**: Compila o software especificamente para processadores de 32 bits.
   - **x64**: Compila o software especificamente para processadores de 64 bits.
2. Recomenda-se deixar a opção **Any CPU** selecionada, a menos que haja uma necessidade específica para compilar para uma arquitetura diferente.

### Passo 4: Compilar a Solução

1. Com o modo de compilação e a arquitetura selecionados, vá para o menu principal do Visual Studio.
2. Clique em **Build** (Compilar) > **Build Solution** (Compilar Solução).
3. O Visual Studio começará a compilar a solução. Esse processo pode levar alguns minutos, dependendo do tamanho da solução.

### Passo 5: Localizar os Binários Compilados

1. Após a compilação ser concluída, você encontrará os arquivos binários (executáveis e bibliotecas) no diretório `/dist/` dentro da pasta raiz do projeto.
   - Se você compilou em modo **Debug**, os arquivos estarão em `/dist/Debug/net8.0-windows`.
   - Se você compilou em modo **Release**, os arquivos estarão em `/dist/Release/net8.0-windows`.
2. Navegue até o diretório correspondente para acessar os binários compilados.

### Considerações Finais

Se você precisar alterar algo ou recompilar a solução, repita os passos acima conforme necessário. Lembre-se de sempre verificar se está no modo de compilação correto para o propósito desejado (Debug para testes, Release para produção).

### Referências

 - [Tutorial: Compilar um aplicativo no Visual Studio 2022](https://learn.microsoft.com/pt-br/visualstudio/ide/walkthrough-building-an-application?view=vs-2022)

## Configuração do Inno Setup no Processo de Build
O instalador para o IOConnect é gerado automaticamente através de um script powershell, criado exclusivamente para essa finalidade, utilizando o **Inno Setup**. Para configurar ou modificar esse processo:

### Passo 1: Instalação do Inno Setup
1. Certifique-se de que o **Inno Setup Compiler (ISCC)** está instalado no caminho: `C:\Program Files (x86)\Inno Setup 6\ISCC.exe`. Se não estiver, utilize o instalador localizado no caminho `IOConnect\Installer` no repositório da solução `Tintometria.NET8.sln`.
2. Os scripts de instalação (x86, x64 e ARM64) estão localizados na pasta do projeto IOConnect.

### Passo 2: Selecione o modo de compilação
1. No Visual Studio, selecione a arquitetura do processador (x86, x64 ou ARM64). É recomendado selecionar a opção *AnyCPU*, que permite gerar os binários do sistema de forma compatível com qualquer arquitetura.

**Nota importante:** *Atualmente o IOConnect utiliza uma biblioteca externa para comunicação ModBus chamada WSMBS.dll, que é compatível apenas com arquiteturas x86. Compilar o IOCOnnect com esse componente em arquiteturas x64 ou AnyCPU vai gerar warnings na compilação e pode gerar incompatibilidade e mal funcionamento em sistemas diferentes de x86 (32-Bits)*. 

### Passo 3: Compilação do Instalador
1. Acesse a pasta `Installer`, no projeto IOConnect, e copie o endereço completo da pasta.
2. Execute o Windows Powershell.
3. Digite o comando `cd [caminho do installer]`, onde [caminho do installer] é o caminho copiado no passo 1, e pressione enter.
4. Digite o comando `./publish-ioconnect -target [arquitetura]`, onde `[arquitetura]` é a arquitetura do processador alvo onde o IOConnect será instalado. Os valores permitidos para `[arquitetura]`são: win-x86, win-x64 e win-arm64.
5. O instalador final será encontrado na pasta `dist`, na raiz da solução.

## Adicionando novos parâmetros de configuração no instalador
O instalador do IOConnect está preparado para configurar a instalação com parâmetros padrão para diversas máquinas e clientes, bastando para isso selecionar a configuração durante a execução do instalador.
Durante a instalação, o instalador verifica qual a parametrização desejada e copia os arquivos de parãmetros em xml na pasta do IOConnect, que fará sua leitura e gravará nos bancos de dados de parâmetros os valores lidos.
Caso seja necessário incluir novos parâmetros de configuração ao instalador será necessário:
1. Criar os arquivos `Claibragem.xml`, `Colorantes.xml`, `Formulas.xml`, `Parametros.xml` e `Recircular.xml` com os valores desejados.
2. Criar uma nova pasta no diretório `IOConnect\Files` com o nome da configuração, exemplo: `EUCATEX - e-colors - Percolore AD-D8`.
3. Salvar dentro dessa pasta os arquivos de configuração criados no passo 1.
4. Ajustar os arquivos de instalação do inno setup localizados na pasta do IOConnect. Os scripts são: `IOConnectInstaller-win-arm64.iss`, `IOConnectInstaller-win-x64.iss` e `IOConnectInstaller-win-x86.iss`.

### Ajustando os scripts de instalação para comportar novas configurações
Para ajustar os scripts de instalação do IOConnect para incluir novas configurações de máquinas, siga os seguintes passos:
1. Abra o arquivo de instalação desejado e localize a seção `[Tasks]`.
2. Em `Calibrações` inclua uma nova linha informando os parâmetros `name`, `description` e `Flags`. O parâmetro *Flags* deve ter o valor `exclusive`. Siga o mesmo padrão das calibrações existentes.
3. Localize a seção `[Files]`
4. Na sub seção `Banco de Calibração`, localize os arquivos da última calibração existente.
5. Copie a última calibração existente e cole abaixo dela, tomando o cuidado de ajustar o nome da configuração, o caminho para os arquivos criandos na pasta `IOConnect\Files` e o nome da task relacionada no parâmetro `Tasks`. O nome da Task deve ser o mesmo nome criado no passo 2.
6. Salve o arquivo.
7. Repita o processo para os scripts de instalação das demais arquiteturas.

## Atualização de Compatibilidade do Gerador de Tokens

### Introdução

O **Gerador** é o componente responsável pela geração de tokens de máquina para a aplicação **IOConnect**. Recentemente, com a migração do sistema para o .NET 8, foram identificadas inconsistências na geração e validação de tokens devido a mudanças no comportamento de alguns métodos de criptografia utilizados nas versões anteriores (.NET Framework 4.6.2).

### Mudanças Implementadas

Após a migração do IOConnect para .NET 8, identificou-se que o método utilizado anteriormente para a geração de hashes de assinatura dos tokens sofreu alterações no seu comportamento, tornando a geração de tokens incompatível entre as versões antigas e novas do framework.

Para resolver este problema, o algoritmo utilizado foi substituído pelo **SHA256**, um algoritmo de criptografia moderno e amplamente utilizado para garantir a consistência e segurança na geração de hashes. Essa atualização assegura que os tokens gerados sejam confiáveis e compatíveis com as novas especificações da aplicação.

### Compatibilidade de Versões

Devido à alteração no algoritmo de geração de tokens, é importante ressaltar que **os tokens gerados nas versões anteriores do IOConnect não são compatíveis com a versão atual da aplicação (v.5 e superiores)**. Abaixo está um resumo das versões e suas respectivas dependências de gerador:

-   **IOConnect v.4 e anteriores**: Utilizam o **Gerador v.1.0.2** (.NET Framework 4.6.2)
-   **IOConnect v.5 e posteriores**: Devem utilizar o **Gerador v.2.0.0** (.NET 8)

### Impacto e Recomendações

Com essa mudança, qualquer migração da aplicação para o .NET 8 exigirá a utilização do **Gerador v.2.0.0** para garantir a correta geração de tokens. É recomendável que, após a atualização, todos os tokens antigos sejam descartados e novos tokens sejam gerados utilizando o gerador atualizado, para evitar problemas de compatibilidade.

# Troubleshooting
### Logging de Erros com Serilog

A aplicação IOConnect agora conta com um mecanismo de log de erros implementado utilizando o componente **Serilog**, o que permite capturar e armazenar logs de maneira estruturada, facilitando a identificação e resolução de problemas. O Serilog está configurado para gravar logs tanto no console quanto em arquivos de log no disco, com um modelo de rotação diária.

#### Configuração do Logging

Para configurar o comportamento de logging, ajuste o arquivo `appsettings.json` no seu projeto. Abaixo está um exemplo de configuração padrão, seguido de uma explicação de cada campo:

    "Serilog": {
      "MinimumLevel": {
        "Default": "Information",
        "Override": {
          "Microsoft": "Error",
          "System": "Error"
        }
      },
      "Using": [ "Serilog.Sinks.File" ],
      "WriteTo": [
        { "Name": "Console" },
        {
          "Name": "File",
          "Args": {
            "path": "logs/log-.log",
            "rollingInterval": "Day",
            "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}|{SourceContext}|{ConnectionId}|{ElasticApmTraceId}|{ElasticApmTransactionId}|[{Level:u4}]|{Message:lj}{NewLine}{Exception}"
          }
        }
      ]
    }

#### Explicação dos Campos

1.  **MinimumLevel**: Define o nível mínimo de severidade para os eventos que serão logados. Os níveis possíveis são: `Verbose`, `Debug`, `Information`, `Warning`, `Error`, e `Fatal`.
    
    -   **Default**: Especifica o nível padrão para logs. No exemplo, está configurado para `Information`, ou seja, qualquer evento a partir do nível "Information" será registrado.
    -   **Override**: Permite modificar o nível de log para namespaces específicos.
        -   **Microsoft** e **System**: Ambos estão configurados para `Error`, o que significa que apenas erros desses namespaces serão registrados.
2.  **Using**: Lista os pacotes que o Serilog usará. Aqui estamos utilizando o sink `Serilog.Sinks.File`, que permite gravar logs em arquivos.
    
3.  **WriteTo**: Define os destinos para onde os logs serão enviados. Nesse exemplo, estamos enviando os logs para o console e para um arquivo.
    
    -   **Console**: Especifica que os logs também serão exibidos no console da aplicação.
        
    -   **File**: Define a configuração para gravação dos logs em arquivo.
        
        -   **path**: Especifica o caminho onde os arquivos de log serão salvos. O valor `logs/log-.log` significa que os arquivos de log serão criados na pasta `logs` com nomes no formato `log-YYYYMMDD.log`, onde `YYYYMMDD` será a data do log.
        -   **rollingInterval**: Define o intervalo de rotação dos arquivos de log. No exemplo, a rotação ocorre diariamente (`Day`), o que significa que um novo arquivo de log será criado a cada dia.
        -   **outputTemplate**: Especifica o formato da mensagem de log. Os campos incluídos no template são:
            -   **Timestamp**: Data e hora do log, no formato `yyyy-MM-dd HH:mm:ss.fff zzz`.
            -   **SourceContext**: O contexto de origem do log, útil para identificar de qual parte do código o log foi gerado.
            -   **ConnectionId**, **ElasticApmTraceId**, **ElasticApmTransactionId**: Campos opcionais para identificar conexões e transações, caso estejam sendo utilizadas.
            -   **Level**: O nível de severidade do log (`Information`, `Error`, etc.).
            -   **Message**: A mensagem de log.
            -   **Exception**: Informações detalhadas sobre exceções, caso existam.

#### Valores Permitidos

-   **MinimumLevel**: Aceita os valores `Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`.
-   **Override**: Aceita os mesmos valores do campo `MinimumLevel` para namespaces específicos.
-   **rollingInterval**: Aceita `Infinite`, `Year`, `Month`, `Day`, `Hour`, `Minute`.
-   **outputTemplate**: Permite a customização com variáveis padrão do Serilog. O exemplo acima é uma configuração recomendada, mas pode ser adaptada conforme necessário.

#### Ajuste e Testes

Depois de configurar o `appsettings.json`, reinicie a aplicação para que as mudanças entrem em vigor. Acesse os arquivos de log na pasta configurada para verificar se o comportamento do log está de acordo com as expectativas.

Caso tenha problemas para configurar ou visualizar os logs, entre em contato com o suporte técnico.

#### Referência
Maiores informações sobre a configuração do Serilog podem ser encontradas no site oficial do componente em [https://serilog.net](https://serilog.net/)

# Contribuição


Esta documentação foi elaborada por **Fábio Luiz Biano**, Analista de Sistemas e proprietário da **Worksoftware Sistemas e Webdesign**.

Para mais informações sobre nossos serviços, visite nosso [site](https://worksoftware.com.br) ou conecte-se pelas nossas redes sociais:

- [Instagram](https://www.instagram.com/oficial.worksoftware)
- [Facebook](https://www.facebook.com/worksoftware)
- [LinkedIn](https://linkedin.com/company/worksoftware-sistemas)

Você também pode entrar em contato diretamente comigo:

- **LinkedIn**: [Fábio Luiz Biano](https://www.linkedin.com/in/fabio-luiz-biano)
- **Telefone/WhatsApp**: +55 11 99655-0368