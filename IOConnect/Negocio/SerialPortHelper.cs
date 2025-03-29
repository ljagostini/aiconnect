using Percolore.Core.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Percolore.IOConnect.Negocio
{
    /// <summary>
    /// Enumeração dos possíveis tipos de conexão para uma porta serial.
    /// </summary>
    public enum SerialPortConnectionType
    {
        Unknown,
        Bluetooth,
        USB,
        PhysicalRS232 // PCI, ACPI, etc.
    }

    /// <summary>
    /// Classe auxiliar para obter informações sobre portas seriais.
    /// </summary>
    public static class SerialPortHelper
    {
        /// <summary>
        /// Obtém o tipo de conexão de uma porta serial específica pelo nome (ex: "COM3").
        /// </summary>
        /// <param name="portName">O nome da porta (ex: "COM3").</param>
        /// <returns>O tipo de conexão da porta como SerialPortConnectionType.</returns>
        public static SerialPortConnectionType GetPortConnectionType(string portName, int timeout = 5000)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LogManager.LogInformation($"[SerialPortHelper.GetPortConnectionType] Iniciado para a porta '{portName}'.");

            SerialPortConnectionType connectionType = SerialPortConnectionType.Unknown;
            string instanceId = null;

            try
            {
                // Validação básica do nome da porta
                if (string.IsNullOrWhiteSpace(portName))
                {
                    LogManager.LogWarning("[SerialPortHelper.GetPortConnectionType] Nome da porta é nulo ou vazio.");
                    return SerialPortConnectionType.Unknown;
                }

                string jsonOutput = string.Empty;
                string errors = string.Empty;
                int exitCode = -1;

                // Padrão para encontrar o FriendlyName que contém o nome da porta
                string filterPattern = $"*({portName})*";
                filterPattern = filterPattern.Replace("'", "''");

                // Comando PowerShell para obter o InstanceId da porta
                string powerShellCommand = $"Get-PnpDevice -Class Ports | Where-Object {{ $_.FriendlyName -like '{filterPattern}' }} | Select-Object InstanceId | ConvertTo-Json -Compress";

                // Configuração do processo PowerShell
                var processStartInfo = new ProcessStartInfo("powershell.exe")
                {
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{powerShellCommand}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                // Execução do processo PowerShell
                using (var process = new Process { StartInfo = processStartInfo })
                {
                    LogManager.LogDebug($"[SerialPortHelper.GetPortConnectionType] Executando PowerShell: {processStartInfo.FileName} {processStartInfo.Arguments}");
                    process.Start();

                    // Leitura síncrona da saída e erro
                    jsonOutput = process.StandardOutput.ReadToEnd().Trim();
                    errors = process.StandardError.ReadToEnd().Trim();

                    // Aguarda o término do processo com um timeout
                    if (!process.WaitForExit(timeout))
                    {
                        LogManager.LogError($"[SerialPortHelper.GetPortConnectionType] Timeout ao executar o PowerShell para a porta '{portName}'. Abortando o processo.", null);
                        try
                        {
                            process.Kill();
                            return SerialPortConnectionType.Unknown;
                        }
                        catch
                        {
                            return SerialPortConnectionType.Unknown;
                        }
                    }
                    exitCode = process.ExitCode;

                    // Verificação do resultado da execução
                    if (exitCode != 0)
                    {
                        LogManager.LogError($"[SerialPortHelper.GetPortConnectionType] Processo PowerShell finalizou com código {exitCode} para a porta '{portName}'. Erros: {errors}", null);
                    }
                    else if (!string.IsNullOrEmpty(errors))
                    {
                        LogManager.LogWarning($"[SerialPortHelper.GetPortConnectionType] Processo PowerShell para '{portName}' reportou erros/avisos: {errors}");
                    }

                    // Analisa a saída JSON se o processo foi bem-sucedido
                    if (exitCode == 0 && !string.IsNullOrWhiteSpace(jsonOutput))
                    {
                        try
                        {
                            // Analisa o JSON para extrair o InstanceId
                            using (JsonDocument document = JsonDocument.Parse(jsonOutput))
                            {
                                // Espera-se um objeto JSON com a propriedade "InstanceId"
                                if (document.RootElement.TryGetProperty("InstanceId", out JsonElement instanceIdElement))
                                {
                                    instanceId = instanceIdElement.GetString();
                                }
                                else
                                {
                                    LogManager.LogWarning($"[SerialPortHelper.GetPortConnectionType] JSON recebido não continha a propriedade 'InstanceId'. JSON: {jsonOutput}");
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            LogManager.LogError($"[SerialPortHelper.GetPortConnectionType] Falha ao analisar a saída JSON do PowerShell para '{portName}'. JSON: {jsonOutput}", ex);
                            instanceId = null;
                        }
                    }
                    else if (exitCode == 0 && string.IsNullOrWhiteSpace(jsonOutput))
                    {
                        // Comando executou com sucesso, mas não retornou dados (porta não encontrada)
                        LogManager.LogWarning($"[SerialPortHelper.GetPortConnectionType] Comando PowerShell bem-sucedido para '{portName}', mas não retornou dados (porta não encontrada ou filtro incorreto).");
                    }
                } // Fim using process

                // Determina o tipo de conexão baseado no InstanceId obtido
                if (!string.IsNullOrEmpty(instanceId))
                {
                    LogManager.LogDebug($"[SerialPortHelper.GetPortConnectionType] InstanceId encontrado para '{portName}': '{instanceId}'");
                    connectionType = ParsePnpDeviceId(instanceId);
                }
                else
                {
                    // Se instanceId for nulo ou vazio após a execução (seja por erro, timeout, ou não encontrado)
                    LogManager.LogWarning($"[SerialPortHelper.GetPortConnectionType] Não foi possível obter o InstanceId para a porta '{portName}'.");
                    connectionType = SerialPortConnectionType.Unknown;
                }
            }
            catch (Exception ex)
            {
                // Captura exceções gerais ao tentar executar o processo ou analisar os dados
                LogManager.LogError($"[SerialPortHelper.GetPortConnectionType] Erro inesperado ao processar a porta '{portName}'.", ex);
                connectionType = SerialPortConnectionType.Unknown;
            }
            finally
            {
                // Log final da operação
                sw.Stop();
                LogManager.LogInformation($"[SerialPortHelper.GetPortConnectionType] Finalizado para a porta '{portName}'. InstanceId='{(instanceId ?? "N/A")}'. Tipo='{connectionType}'. Tempo decorrido: {sw.ElapsedMilliseconds} ms.");
            }

            return connectionType;
        }

        /// <summary>
        /// Analisa a string PNPDeviceID (InstanceId) para determinar o tipo de conexão.
        /// </summary>
        /// <param name="pnpDeviceId">A string PNPDeviceID.</param>
        /// <returns>O tipo de conexão como SerialPortConnectionType.</returns>
        private static SerialPortConnectionType ParsePnpDeviceId(string pnpDeviceId)
        {
            if (string.IsNullOrEmpty(pnpDeviceId))
            {
                return SerialPortConnectionType.Unknown;
            }

            // Verifica prefixos comuns para identificar o tipo de barramento/enumerador
            if (pnpDeviceId.StartsWith("BTHENUM\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.Bluetooth;
            if (pnpDeviceId.StartsWith("USB\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.USB;
            if (pnpDeviceId.StartsWith("FTDIBUS\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.USB;
            if (pnpDeviceId.Contains("\\VID_") && pnpDeviceId.Contains("&PID_") &&
                !pnpDeviceId.StartsWith("PCI\\", StringComparison.OrdinalIgnoreCase) &&
                !pnpDeviceId.StartsWith("ACPI\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.USB;
            if (pnpDeviceId.StartsWith("VCP\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.USB;
            if (pnpDeviceId.StartsWith("PCI\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.PhysicalRS232;
            if (pnpDeviceId.StartsWith("ACPI\\", StringComparison.OrdinalIgnoreCase))
                return SerialPortConnectionType.PhysicalRS232;

            // Log se o prefixo não for reconhecido
            LogManager.LogDebug($"[SerialPortHelper.ParsePnpDeviceId] Prefixo PNPDeviceID não reconhecido: {pnpDeviceId}");
            return SerialPortConnectionType.Unknown;
        }

        /// <summary>
        /// Obtém a descrição textual para um tipo de conexão.
        /// </summary>
        /// <param name="type">O tipo de conexão.</param>
        /// <returns>Uma string descritiva.</returns>
        public static string GetConnectionTypeDescription(SerialPortConnectionType type)
        {
            return type switch
            {
                SerialPortConnectionType.Bluetooth => "Bluetooth",
                SerialPortConnectionType.USB => "USB",
                SerialPortConnectionType.PhysicalRS232 => "Physical (RS-232/PCI/ACPI)",
                _ => "Unknown"
            };
        }
    }
}