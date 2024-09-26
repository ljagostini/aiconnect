using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectMensagem
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Mensagens.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Conteudo { get; set; }
        public string IdIdioma { get; set; }

        public static void CreateBD()
        {
            if (!File.Exists(PathFile))
            {

                SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(PathFile, false);
                connectCreate.Open();
				// Open connection to create DB if not exists.
				connectCreate.Close();
                Thread.Sleep(2000);
                if (File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [Mensagem] (Id INTEGER PRIMARY KEY, Nome TEXT NULL, Conteudo TEXT NULL, IdIdioma TEXT NULL);");
                    string createQuery = sb.ToString();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            cmd.CommandText = createQuery;
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
        }

        public static void LoadMessage()
        {
            Negocio.IdiomaResxExtensao.SetIDiomaRex();
            List<ObjectMensagem> local = List();
            SetConteudo(local);
        }
     
        public ObjectMensagem Load(int id)
        {
            ObjectMensagem aux = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Mensagem WHERE Id = " + id.ToString() + ";";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            aux = new ObjectMensagem();
                            aux.Id = int.Parse(reader["Id"].ToString());
                            aux.Nome = reader["Nome"].ToString();
                            aux.Conteudo = reader["Conteudo"].ToString();
                            break;
                        }
                    }
                }
                conn.Close();
            }

            return aux;
        }

        public static List<ObjectMensagem> List()
        {
            List<ObjectMensagem> list = new List<ObjectMensagem>();
            ObjectParametros op = ObjectParametros.Load();
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Mensagem WHERE IdIdioma = '" + op.IdIdioma.ToString() + "';";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ObjectMensagem aux = new ObjectMensagem();
                            aux.Id = int.Parse(reader["Id"].ToString());
                            aux.Nome = reader["Nome"].ToString();
                            aux.Conteudo = reader["Conteudo"].ToString();
                            aux.IdIdioma = reader["IdIdioma"].ToString();
                            list.Add(aux);
                        }
                    }
                }
                conn.Close();
            }

            op = null;
            return list;
        }

        private static void SetConteudoObj(ObjectMensagem aux)
        {
            switch (aux.Nome)
            {
                case "Autenticacao_DadosNaoInformados": { Negocio.IdiomaResxExtensao.Autenticacao_DadosNaoInformados = aux.Conteudo; break; }
                case "CalibracaoAuto_Cancelado": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Cancelado = aux.Conteudo; break; }
                case "CalibracaoAuto_ErrorRecipienteInadequado": { Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorRecipienteInadequado = aux.Conteudo; break; }
                case "CalibracaoAuto_ErrorSuporte": { Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorSuporte = aux.Conteudo; break; }
                case "CalibracaoAuto_Nova_Calibracao": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Nova_Calibracao = aux.Conteudo; break; }
                case "CalibracaoAuto_NovaCalibracao": { Negocio.IdiomaResxExtensao.CalibracaoAuto_NovaCalibracao = aux.Conteudo; break; }
                case "CalibracaoAuto_OK": { Negocio.IdiomaResxExtensao.CalibracaoAuto_OK = aux.Conteudo; break; }
                case "CalibracaoAuto_OK_NovaCalibracao": { Negocio.IdiomaResxExtensao.CalibracaoAuto_OK_NovaCalibracao = aux.Conteudo; break; }
                case "CalibracaoAutomate_Cancelado": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_Cancelado = aux.Conteudo; break; }
                case "CalibracaoAutomate_Error": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_Error = aux.Conteudo; break; }
                case "CalibracaoAutomate_OK": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_OK = aux.Conteudo; break; }
                case "fAutenticacao_Mensagem_UsuarioSenhaInvalido": { Negocio.IdiomaResxExtensao.fAutenticacao_Mensagem_UsuarioSenhaInvalido = aux.Conteudo; break; }
                case "fAutenticacao_Mensagem_UsuarioSenhaNaoInformado": { Negocio.IdiomaResxExtensao.fAutenticacao_Mensagem_UsuarioSenhaNaoInformado = aux.Conteudo; break; }
                case "frmTreinCal_Message_01": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_01 = aux.Conteudo; break; }
                case "frmTreinCal_Message_02": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_02 = aux.Conteudo; break; }
                case "frmTreinCal_Message_03": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_03 = aux.Conteudo; break; }
                case "frmTreinCal_Message_04": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_04 = aux.Conteudo; break; }
                case "frmTreinCal_Message_05": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_05 = aux.Conteudo; break; }
                case "frmTreinCal_Message_06": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_06 = aux.Conteudo; break; }
                case "frmTreinCal_Message_07": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_07 = aux.Conteudo; break; }
                case "frmTreinCal_Message_08": { Negocio.IdiomaResxExtensao.frmTreinCal_Message_08 = aux.Conteudo; break; }
                case "GerenciarFormulaPersonalizada_Falha_Dispensar": { Negocio.IdiomaResxExtensao.GerenciarFormulaPersonalizada_Falha_Dispensar = aux.Conteudo; break; }
                case "Global_CircuitoColoranteAbastecido": { Negocio.IdiomaResxExtensao.Global_CircuitoColoranteAbastecido = aux.Conteudo; break; }
                case "Global_CircuitoColoranteInexistente": { Negocio.IdiomaResxExtensao.Global_CircuitoColoranteInexistente = aux.Conteudo; break; }
                case "Global_CircuitoColoranteZerado": { Negocio.IdiomaResxExtensao.Global_CircuitoColoranteZerado = aux.Conteudo; break; }
                case "Global_Confirma_RedefinirValidade": { Negocio.IdiomaResxExtensao.Global_Confirma_RedefinirValidade = aux.Conteudo; break; }
                case "Global_Confirmar_AbastecerDosadora": { Negocio.IdiomaResxExtensao.Global_Confirmar_AbastecerDosadora = aux.Conteudo; break; }
                case "Global_Confirmar_DesejaTentarNovamente": { Negocio.IdiomaResxExtensao.Global_Confirmar_DesejaTentarNovamente = aux.Conteudo; break; }
                case "Global_Confirmar_ExcluirDados": { Negocio.IdiomaResxExtensao.Global_Confirmar_ExcluirDados = aux.Conteudo; break; }
                case "Global_Confirmar_ExecutarPorcesso": { Negocio.IdiomaResxExtensao.Global_Confirmar_ExecutarPorcesso = aux.Conteudo; break; }
                case "Global_Confirmar_Sair": { Negocio.IdiomaResxExtensao.Global_Confirmar_Sair = aux.Conteudo; break; }
                case "Global_DisensaNaoConectado": { Negocio.IdiomaResxExtensao.Global_DisensaNaoConectado = aux.Conteudo; break; }
                case "Global_DispensaExecutadaEdicaoFormula": { Negocio.IdiomaResxExtensao.Global_DispensaExecutadaEdicaoFormula = aux.Conteudo; break; }
                case "Global_DispensaExecutadaGerenciamentoFormula": { Negocio.IdiomaResxExtensao.Global_DispensaExecutadaGerenciamentoFormula = aux.Conteudo; break; }
                case "Global_DosagemAbortada": { Negocio.IdiomaResxExtensao.Global_DosagemAbortada = aux.Conteudo; break; }
                case "Global_DosagemCancelada": { Negocio.IdiomaResxExtensao.Global_DosagemCancelada = aux.Conteudo; break; }
                case "Global_DosagemConcluida": { Negocio.IdiomaResxExtensao.Global_DosagemConcluida = aux.Conteudo; break; }
                case "Global_DosagemDescricaoCorantes": { Negocio.IdiomaResxExtensao.Global_DosagemDescricaoCorantes = aux.Conteudo; break; }
                case "Global_DosagemIniciada": { Negocio.IdiomaResxExtensao.Global_DosagemIniciada = aux.Conteudo; break; }
                case "Global_Error_RessetData": { Negocio.IdiomaResxExtensao.Global_Error_RessetData = aux.Conteudo; break; }
                case "Global_Falha_CarregarDados": { Negocio.IdiomaResxExtensao.Global_Falha_CarregarDados = aux.Conteudo; break; }
                case "Global_Falha_Dispensar": { Negocio.IdiomaResxExtensao.Global_Falha_Dispensar = aux.Conteudo; break; }
                case "Global_Falha_EnviarDados": { Negocio.IdiomaResxExtensao.Global_Falha_EnviarDados = aux.Conteudo; break; }
                case "Global_Falha_Esponja": { Negocio.IdiomaResxExtensao.Global_Falha_Esponja = aux.Conteudo; break; }
                case "Global_Falha_ExcluirDados": { Negocio.IdiomaResxExtensao.Global_Falha_ExcluirDados = aux.Conteudo; break; }
                case "Global_Falha_ExecucaoPorcesso": { Negocio.IdiomaResxExtensao.Global_Falha_ExecucaoPorcesso = aux.Conteudo; break; }
                case "Global_Falha_GravarDados": { Negocio.IdiomaResxExtensao.Global_Falha_GravarDados = aux.Conteudo; break; }
                case "Global_Falha_Monitoramento": { Negocio.IdiomaResxExtensao.Global_Falha_Monitoramento = aux.Conteudo; break; }
                case "Global_Falha_NaoFoiPossivelAtualizar": { Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelAtualizar = aux.Conteudo; break; }
                case "Global_Falha_NaoFoiPossivelConectar": { Negocio.IdiomaResxExtensao.Global_Falha_NaoFoiPossivelConectar = aux.Conteudo; break; }
                case "Global_Falha_PerdaDeConexao": { Negocio.IdiomaResxExtensao.Global_Falha_PerdaDeConexao = aux.Conteudo; break; }
                case "Global_Falha_RessetData": { Negocio.IdiomaResxExtensao.Global_Falha_RessetData = aux.Conteudo; break; }
                case "Global_Falha_TestarRecipiente": { Negocio.IdiomaResxExtensao.Global_Falha_TestarRecipiente = aux.Conteudo; break; }
                case "Global_Falha_VolumeMinimoDat": { Negocio.IdiomaResxExtensao.Global_Falha_VolumeMinimoDat = aux.Conteudo; break; }
                case "Global_FalhaDosagem": { Negocio.IdiomaResxExtensao.Global_FalhaDosagem = aux.Conteudo; break; }
                case "Global_FalhaMonit": { Negocio.IdiomaResxExtensao.Global_FalhaMonit = aux.Conteudo; break; }
                case "Global_FalhaPurgar": { Negocio.IdiomaResxExtensao.Global_FalhaPurgar = aux.Conteudo; break; }
                case "Global_FalhaPurgarIndividual": { Negocio.IdiomaResxExtensao.Global_FalhaPurgarIndividual = aux.Conteudo; break; }
                case "Global_FalhaRecircularIndividual": { Negocio.IdiomaResxExtensao.Global_FalhaRecircularIndividual = aux.Conteudo; break; }
                case "Global_Fechar": { Negocio.IdiomaResxExtensao.Global_Fechar = aux.Conteudo; break; }
                case "Global_FormatoInvalido": { Negocio.IdiomaResxExtensao.Global_FormatoInvalido = aux.Conteudo; break; }
                case "Global_Informacao_AplicativoAtualizacaoBD": { Negocio.IdiomaResxExtensao.Global_Informacao_AplicativoAtualizacaoBD = aux.Conteudo; break; }
                case "Global_Informacao_AplicativoEmExecucao": { Negocio.IdiomaResxExtensao.Global_Informacao_AplicativoEmExecucao = aux.Conteudo; break; }
                case "Global_Informacao_ArquivoDatDeletado": { Negocio.IdiomaResxExtensao.Global_Informacao_ArquivoDatDeletado = aux.Conteudo; break; }
                case "Global_Informacao_ArquivoNaoInformadoOuInvalido": { Negocio.IdiomaResxExtensao.Global_Informacao_ArquivoNaoInformadoOuInvalido = aux.Conteudo; break; }
                case "Global_Informacao_DadosGravadosSucesso": { Negocio.IdiomaResxExtensao.Global_Informacao_DadosGravadosSucesso = aux.Conteudo; break; }
                case "Global_informacao_DatRenomeado": { Negocio.IdiomaResxExtensao.Global_informacao_DatRenomeado = aux.Conteudo; break; }
                case "Global_Informacao_DispensaAbortada": { Negocio.IdiomaResxExtensao.Global_Informacao_DispensaAbortada = aux.Conteudo; break; }
                case "Global_Informacao_DispensaCancelada": { Negocio.IdiomaResxExtensao.Global_Informacao_DispensaCancelada = aux.Conteudo; break; }
                case "Global_Informacao_ExpiracaoManutencao": { Negocio.IdiomaResxExtensao.Global_Informacao_ExpiracaoManutencao = aux.Conteudo; break; }
                case "Global_Informacao_NenhumColorante": { Negocio.IdiomaResxExtensao.Global_Informacao_NenhumColorante = aux.Conteudo; break; }
                case "Global_Informacao_NivelColoranteInsuficiente": { Negocio.IdiomaResxExtensao.Global_Informacao_NivelColoranteInsuficiente = aux.Conteudo; break; }
                case "Global_Informacao_NivelMinimo": { Negocio.IdiomaResxExtensao.Global_Informacao_NivelMinimo = aux.Conteudo; break; }
                case "Global_Informacao_NumeroSerialInvalido": { Negocio.IdiomaResxExtensao.Global_Informacao_NumeroSerialInvalido = aux.Conteudo; break; }
                case "Global_Informacao_ProcessoCancelado": { Negocio.IdiomaResxExtensao.Global_Informacao_ProcessoCancelado = aux.Conteudo; break; }
                case "Global_Informacao_RecipienteEsponjaNaoPosicionado": { Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteEsponjaNaoPosicionado = aux.Conteudo; break; }
                case "Global_Informacao_RecipienteNaoPosicionado": { Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado = aux.Conteudo; break; }
                case "Global_Informacao_RecipienteNaoPosicionado_2": { Negocio.IdiomaResxExtensao.Global_Informacao_RecipienteNaoPosicionado_2 = aux.Conteudo; break; }
                case "Global_Informacao_RequisitosNaoAtendidos": { Negocio.IdiomaResxExtensao.Global_Informacao_RequisitosNaoAtendidos = aux.Conteudo; break; }
                case "Global_Inic_Monitoramento_Circuitos": { Negocio.IdiomaResxExtensao.Global_Inic_Monitoramento_Circuitos = aux.Conteudo; break; }
                case "Global_InicializacaoInterrompida": { Negocio.IdiomaResxExtensao.Global_InicializacaoInterrompida = aux.Conteudo; break; }
                case "Global_LimpBicos": { Negocio.IdiomaResxExtensao.Global_LimpBicos = aux.Conteudo; break; }
                case "Global_MonitPendente": { Negocio.IdiomaResxExtensao.Global_MonitPendente = aux.Conteudo; break; }
                case "Global_MonitPendente3": { Negocio.IdiomaResxExtensao.Global_MonitPendente3 = aux.Conteudo; break; }
                case "Global_ProcessoCanceladoUsuario": { Negocio.IdiomaResxExtensao.Global_ProcessoCanceladoUsuario = aux.Conteudo; break; }
                case "Global_ProcessoDispensaIniciado": { Negocio.IdiomaResxExtensao.Global_ProcessoDispensaIniciado = aux.Conteudo; break; }
                case "Global_PurgaAbortada": { Negocio.IdiomaResxExtensao.Global_PurgaAbortada = aux.Conteudo; break; }
                case "Global_PurgaCancelada": { Negocio.IdiomaResxExtensao.Global_PurgaCancelada = aux.Conteudo; break; }
                case "Global_PurgaConcluida": { Negocio.IdiomaResxExtensao.Global_PurgaConcluida = aux.Conteudo; break; }
                case "Global_PurgaIndividualAbortada": { Negocio.IdiomaResxExtensao.Global_PurgaIndividualAbortada = aux.Conteudo; break; }
                case "Global_PurgaIndividualCancelada": { Negocio.IdiomaResxExtensao.Global_PurgaIndividualCancelada = aux.Conteudo; break; }
                case "Global_PurgaIndividualConcluida": { Negocio.IdiomaResxExtensao.Global_PurgaIndividualConcluida = aux.Conteudo; break; }
                case "Global_PurgaIndividualRealizada": { Negocio.IdiomaResxExtensao.Global_PurgaIndividualRealizada = aux.Conteudo; break; }
                case "Global_PurgaPendente": { Negocio.IdiomaResxExtensao.Global_PurgaPendente = aux.Conteudo; break; }
                case "Global_PurgaPendente3": { Negocio.IdiomaResxExtensao.Global_PurgaPendente3 = aux.Conteudo; break; }
                case "Global_RecircularIndividualAbortada": { Negocio.IdiomaResxExtensao.Global_RecircularIndividualAbortada = aux.Conteudo; break; }
                case "Global_RecircularIndividualCancelada": { Negocio.IdiomaResxExtensao.Global_RecircularIndividualCancelada = aux.Conteudo; break; }
                case "Global_RecircularIndividualConcluida": { Negocio.IdiomaResxExtensao.Global_RecircularIndividualConcluida = aux.Conteudo; break; }
                case "Global_RecircularIndividualRealizada": { Negocio.IdiomaResxExtensao.Global_RecircularIndividualRealizada = aux.Conteudo; break; }
                case "Global_Sair_Solucao": { Negocio.IdiomaResxExtensao.Global_Sair_Solucao = aux.Conteudo; break; }
                case "Global_SelecionadoMenuPurga": { Negocio.IdiomaResxExtensao.Global_SelecionadoMenuPurga = aux.Conteudo; break; }
                case "Global_SelecionadoMenuPurgaIndividual": { Negocio.IdiomaResxExtensao.Global_SelecionadoMenuPurgaIndividual = aux.Conteudo; break; }
                case "Global_Sucesso_RessetData": { Negocio.IdiomaResxExtensao.Global_Sucesso_RessetData = aux.Conteudo; break; }
                case "Global_TodosCircuitosColoranteAbastecidos": { Negocio.IdiomaResxExtensao.Global_TodosCircuitosColoranteAbastecidos = aux.Conteudo; break; }
                case "Global_TodosCircuitosColoranteZerados": { Negocio.IdiomaResxExtensao.Global_TodosCircuitosColoranteZerados = aux.Conteudo; break; }
                case "Global_TokenExpirado": { Negocio.IdiomaResxExtensao.Global_TokenExpirado = aux.Conteudo; break; }
                case "Global_TokenInvalido": { Negocio.IdiomaResxExtensao.Global_TokenInvalido = aux.Conteudo; break; }
                case "Global_TokenNaoInformado": { Negocio.IdiomaResxExtensao.Global_TokenNaoInformado = aux.Conteudo; break; }
                case "Global_Treinamento_NaofoiPossivelIniciar": { Negocio.IdiomaResxExtensao.Global_Treinamento_NaofoiPossivelIniciar = aux.Conteudo; break; }
                case "Global_MonitIniciada": { Negocio.IdiomaResxExtensao.Global_MonitIniciada = aux.Conteudo; break; }
                case "Global_PurgaIniciada": { Negocio.IdiomaResxExtensao.Global_PurgaIniciada = aux.Conteudo; break; }
                case "Init_Confirmacao_ManutencaoExpirada": { Negocio.IdiomaResxExtensao.Init_Confirmacao_ManutencaoExpirada = aux.Conteudo; break; }
                case "Init_Falha_Diretorio": { Negocio.IdiomaResxExtensao.Init_Falha_Diretorio = aux.Conteudo; break; }
                case "Init_RelogioAlterado": { Negocio.IdiomaResxExtensao.Init_RelogioAlterado = aux.Conteudo; break; }
                case "Init_ValidarDataHora": { Negocio.IdiomaResxExtensao.Init_ValidarDataHora = aux.Conteudo; break; }
                case "Licenca_Informacao_ChaveIncorreta": { Negocio.IdiomaResxExtensao.Licenca_Informacao_ChaveIncorreta = aux.Conteudo; break; }
                case "LogAutomateBackup": { Negocio.IdiomaResxExtensao.LogAutomateBackup = aux.Conteudo; break; }
                case "LogProcesso_FalhaLeituraConteudoDat": { Negocio.IdiomaResxExtensao.LogProcesso_FalhaLeituraConteudoDat = aux.Conteudo; break; }
                case "Nivel_Confirmar_AbastecimentoColorante": { Negocio.IdiomaResxExtensao.Nivel_Confirmar_AbastecimentoColorante = aux.Conteudo; break; }
                case "Nivel_Informacao_ExcedeNivelMaximo": { Negocio.IdiomaResxExtensao.Nivel_Informacao_ExcedeNivelMaximo = aux.Conteudo; break; }
                case "Nivel_Informacao_NivelMaiorZero": { Negocio.IdiomaResxExtensao.Nivel_Informacao_NivelMaiorZero = aux.Conteudo; break; }
                case "PainelControle_CancelouRecircular": { Negocio.IdiomaResxExtensao.PainelControle_CancelouRecircular = aux.Conteudo; break; }
                case "PainelControle_DesejaRecircular": { Negocio.IdiomaResxExtensao.PainelControle_DesejaRecircular = aux.Conteudo; break; }
                case "PainelControle_DispensarValorMinVolume": { Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolume = aux.Conteudo; break; }
                case "PainelControle_DispensarValorMinVolumeLog": { Negocio.IdiomaResxExtensao.PainelControle_DispensarValorMinVolumeLog = aux.Conteudo; break; }
                case "Parametros_Log_DiretorioLogProcessoInvalido": { Negocio.IdiomaResxExtensao.Parametros_Log_DiretorioLogProcessoInvalido = aux.Conteudo; break; }
                case "Parametros_Log_DiretorioLogQuantidadeDispensadaInvalido": { Negocio.IdiomaResxExtensao.Parametros_Log_DiretorioLogQuantidadeDispensadaInvalido = aux.Conteudo; break; }
                case "Pesagem_Confirma_Descarte": { Negocio.IdiomaResxExtensao.Pesagem_Confirma_Descarte = aux.Conteudo; break; }
                case "Pesagem_Confirma_MassaMedia": { Negocio.IdiomaResxExtensao.Pesagem_Confirma_MassaMedia = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_01": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_01 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_02": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_02 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_03": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_03 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_04": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_04 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_05": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_05 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_06": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_06 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_07": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_07 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_08": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_08 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_09": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_09 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_10": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_10 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_11": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_11 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_12": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_12 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_13": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_13 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_14": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_14 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_15": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_15 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Alerta_16": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Alerta_16 = aux.Conteudo; break; }
                case "PlacaMov_Erro_Movimentacao_Passos": { Negocio.IdiomaResxExtensao.PlacaMov_Erro_Movimentacao_Passos = aux.Conteudo; break; }
                case "PlacaMov_Posicionar_Recipiente_Bico": { Negocio.IdiomaResxExtensao.PlacaMov_Posicionar_Recipiente_Bico = aux.Conteudo; break; }
                case "PlacaMov_Realizar_Operacao": { Negocio.IdiomaResxExtensao.PlacaMov_Realizar_Operacao = aux.Conteudo; break; }
                case "ValidadeManutencao_DataRedefinida": { Negocio.IdiomaResxExtensao.ValidadeManutencao_DataRedefinida = aux.Conteudo; break; }


                case "Autenticacao_lblTitulo": { Negocio.IdiomaResxExtensao.Autenticacao_lblTitulo = aux.Conteudo; break; }
                case "Autenticacao_rdbSenha": { Negocio.IdiomaResxExtensao.Autenticacao_rdbSenha = aux.Conteudo; break; }
                case "Autenticacao_SenhaInvalida": { Negocio.IdiomaResxExtensao.Autenticacao_SenhaInvalida = aux.Conteudo; break; }
                case "Autenticacao_lblUsuario": { Negocio.IdiomaResxExtensao.Autenticacao_lblUsuario = aux.Conteudo; break; }
                case "Autenticacao_lblSenha": { Negocio.IdiomaResxExtensao.Autenticacao_lblSenha = aux.Conteudo; break; }
                case "CaminhoArquivo_lblDiretorio": { Negocio.IdiomaResxExtensao.CaminhoArquivo_lblDiretorio = aux.Conteudo; break; }
                case "CaminhoArquivo_lblNome": { Negocio.IdiomaResxExtensao.CaminhoArquivo_lblNome = aux.Conteudo; break; }
                case "CaminhoArquivo_lblTitulo": { Negocio.IdiomaResxExtensao.CaminhoArquivo_lblTitulo = aux.Conteudo; break; }
                case "Configuracao_lblGeralPorta": { Negocio.IdiomaResxExtensao.Configuracao_lblGeralPorta = aux.Conteudo; break; }
                case "Configuracoes_blColoranteMassa": { Negocio.IdiomaResxExtensao.Configuracoes_blColoranteMassa = aux.Conteudo; break; }
                case "Configuracoes_blColoranteNome": { Negocio.IdiomaResxExtensao.Configuracoes_blColoranteNome = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemEditar": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemEditar = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico0": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico0 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico1": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico1 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico10": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico10 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico11": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico11 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico12": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico12 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico2": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico2 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico3": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico3 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico4": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico4 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico5": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico5 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico6": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico6 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico7": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico7 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico8": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico8 = aux.Conteudo; break; }
                case "Configuracoes_btnCalibragemHistorico9": { Negocio.IdiomaResxExtensao.Configuracoes_btnCalibragemHistorico9 = aux.Conteudo; break; }
                case "Configuracoes_btnInicializacaoExecutar": { Negocio.IdiomaResxExtensao.Configuracoes_btnInicializacaoExecutar = aux.Conteudo; break; }
                case "Configuracoes_chkControlarNivel": { Negocio.IdiomaResxExtensao.Configuracoes_chkControlarNivel = aux.Conteudo; break; }
                case "Configuracoes_chkDatInterfaceDispSequencial": { Negocio.IdiomaResxExtensao.Configuracoes_chkDatInterfaceDispSequencial = aux.Conteudo; break; }
                case "Configuracoes_chkDatInterfaceDispSimultanea": { Negocio.IdiomaResxExtensao.Configuracoes_chkDatInterfaceDispSimultanea = aux.Conteudo; break; }
                case "Configuracoes_chkDispensaSequencial": { Negocio.IdiomaResxExtensao.Configuracoes_chkDispensaSequencial = aux.Conteudo; break; }
                case "Configuracoes_chkDispensaSequencialP1": { Negocio.IdiomaResxExtensao.Configuracoes_chkDispensaSequencialP1 = aux.Conteudo; break; }
                case "Configuracoes_chkDispensaSequencialP2": { Negocio.IdiomaResxExtensao.Configuracoes_chkDispensaSequencialP2 = aux.Conteudo; break; }
                case "Configuracoes_chkFormulasPersonalizadas": { Negocio.IdiomaResxExtensao.Configuracoes_chkFormulasPersonalizadas = aux.Conteudo; break; }
                case "Configuracoes_chkGeralSomarRevSteps": { Negocio.IdiomaResxExtensao.Configuracoes_chkGeralSomarRevSteps = aux.Conteudo; break; }
                case "Configuracoes_chkViewMessageProc":{ Negocio.IdiomaResxExtensao.Configuracoes_chkViewMessageProc = aux.Conteudo; break; }
                case "Configuracoes_chkHabilitarPurgaIndividual": { Negocio.IdiomaResxExtensao.Configuracoes_chkHabilitarPurgaIndividual = aux.Conteudo; break; }
                case "Configuracoes_chkIncializacaoInterface": { Negocio.IdiomaResxExtensao.Configuracoes_chkIncializacaoInterface = aux.Conteudo; break; }
                case "Configuracoes_chkInicializacaoExecutarAntesPurga": { Negocio.IdiomaResxExtensao.Configuracoes_chkInicializacaoExecutarAntesPurga = aux.Conteudo; break; }
                case "Configuracoes_chkInicializacaoExecutarAntesPurgaIndividual": { Negocio.IdiomaResxExtensao.Configuracoes_chkInicializacaoExecutarAntesPurgaIndividual = aux.Conteudo; break; }
                case "Configuracoes_chkInicializacaoMovimentoReverso": { Negocio.IdiomaResxExtensao.Configuracoes_chkInicializacaoMovimentoReverso = aux.Conteudo; break; }
                case "Configuracoes_chkModoSimulacao": { Negocio.IdiomaResxExtensao.Configuracoes_chkModoSimulacao = aux.Conteudo; break; }
                case "Configuracoes_chkMonitInterface": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitInterface = aux.Conteudo; break; }
                case "Configuracoes_chkMonitMovimentoReverso": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitMovimentoReverso = aux.Conteudo; break; }
                case "Configuracoes_chkMonitProcesso": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitProcesso = aux.Conteudo; break; }
                case "Configuracoes_chkMonitProducao": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitProducao = aux.Conteudo; break; }
                case "Configuracoes_chkMonitSincFormula": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitSincFormula = aux.Conteudo; break; }
                case "Configuracoes_chkPurgaControleExecucao": { Negocio.IdiomaResxExtensao.Configuracoes_chkPurgaControleExecucao = aux.Conteudo; break; }
                case "Configuracoes_chkPurgaInterfaceProcesso": { Negocio.IdiomaResxExtensao.Configuracoes_chkPurgaInterfaceProcesso = aux.Conteudo; break; }
                case "Configuracoes_chkTecladoVirtual": { Negocio.IdiomaResxExtensao.Configuracoes_chkTecladoVirtual = aux.Conteudo; break; }
                case "Configuracoes_chkTesteRecipiente": { Negocio.IdiomaResxExtensao.Configuracoes_chkTesteRecipiente = aux.Conteudo; break; }
                case "Configuracoes_chkUtilizarCorrespondencia": { Negocio.IdiomaResxExtensao.Configuracoes_chkUtilizarCorrespondencia = aux.Conteudo; break; }
                case "Configuracoes_Confirma_RecarregarDadosCalibragem": { Negocio.IdiomaResxExtensao.Configuracoes_Confirma_RecarregarDadosCalibragem = aux.Conteudo; break; }
                case "Configuracoes_gbFormula": { Negocio.IdiomaResxExtensao.Configuracoes_gbFormula = aux.Conteudo; break; }
                case "Configuracoes_gbProducao": { Negocio.IdiomaResxExtensao.Configuracoes_gbProducao = aux.Conteudo; break; }
                case "Configuracoes_Informacao_InformeAceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformeAceleracao = aux.Conteudo; break; }
                case "Configuracoes_Informacao_InformeDelay": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformeDelay = aux.Conteudo; break; }
                case "Configuracoes_Informacao_InformePulsos": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformePulsos = aux.Conteudo; break; }
                case "Configuracoes_Informacao_InformePulsosRev": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformePulsosRev = aux.Conteudo; break; }
                case "Configuracoes_Informacao_InformeVelocidade": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_InformeVelocidade = aux.Conteudo; break; }
                case "Configuracoes_Informacao_NumeroSerieDigitos": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_NumeroSerieDigitos = aux.Conteudo; break; }
                case "Configuracoes_Informacao_SenhaDigitos": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_SenhaDigitos = aux.Conteudo; break; }
                case "Configuracoes_Informacao_ValoresInvalidos": { Negocio.IdiomaResxExtensao.Configuracoes_Informacao_ValoresInvalidos = aux.Conteudo; break; }
                case "Configuracoes_lblAdminNumeroSerial": { Negocio.IdiomaResxExtensao.Configuracoes_lblAdminNumeroSerial = aux.Conteudo; break; }
                case "Configuracoes_lblAdminSenha": { Negocio.IdiomaResxExtensao.Configuracoes_lblAdminSenha = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemColorante": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemColorante = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemDelay": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemDelay = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemDesvio": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemDesvio = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemLegendaMassaEspec": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMassaEspec = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemLegendaMotor": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaMotor = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemMassa": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemMassa = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemMassaIdeal": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemMassaIdeal = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemPulsos": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemPulsos = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemVelocidade": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVelocidade = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemVolume": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemVolume = aux.Conteudo; break; }
                case "Configuracoes_lblColoranteCircuito": { Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCircuito = aux.Conteudo; break; }
                case "Configuracoes_lblColoranteCorrespondencia": { Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCorrespondencia = aux.Conteudo; break; }
                case "Configuracoes_lblDatBaseCircuito": { Negocio.IdiomaResxExtensao.Configuracoes_lblDatBaseCircuito = aux.Conteudo; break; }
                case "Configuracoes_lblDatCaminhoMonitoramento": { Negocio.IdiomaResxExtensao.Configuracoes_lblDatCaminhoMonitoramento = aux.Conteudo; break; }
                case "Configuracoes_lblDatCaminhoRepositorio": { Negocio.IdiomaResxExtensao.Configuracoes_lblDatCaminhoRepositorio = aux.Conteudo; break; }
                case "Configuracoes_lblDatPadraoConteudo": { Negocio.IdiomaResxExtensao.Configuracoes_lblDatPadraoConteudo = aux.Conteudo; break; }
                case "Configuracoes_lblFormulaIp": { Negocio.IdiomaResxExtensao.Configuracoes_lblFormulaIp = aux.Conteudo; break; }
                case "Configuracoes_lblFormulaPorta": { Negocio.IdiomaResxExtensao.Configuracoes_lblFormulaPorta = aux.Conteudo; break; }
                case "Configuracoes_lblGeralAceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralAceleracao = aux.Conteudo; break; }
                case "Configuracoes_lblGeralComunicacao": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralComunicacao = aux.Conteudo; break; }
                case "Configuracoes_lblGeralDelayReverso": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDelayReverso = aux.Conteudo; break; }
                case "Configuracoes_lblGeralDispositivo": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDispositivo = aux.Conteudo; break; }
                case "Configuracoes_lblGeralFuncinamentoSoftware": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralFuncinamentoSoftware = aux.Conteudo; break; }
                case "Configuracoes_lblGeralParametroDispensaGlobal": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralParametroDispensaGlobal = aux.Conteudo; break; }
                case "Configuracoes_lblGeralPulsoReverso": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralPulsoReverso = aux.Conteudo; break; }
                case "Configuracoes_lblGeralTimeout": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralTimeout = aux.Conteudo; break; }

                case "Configuracoes_lblQtdTentativasConexao": { Negocio.IdiomaResxExtensao.Configuracoes_lblQtdTentativasConexao = aux.Conteudo; break; }
                    

                case "Configuracoes_lblGeralVelocidade": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralVelocidade = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoAceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoAceleracao = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoPulsoInicial": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoPulsoInicial = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoPulsoLimite": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoPulsoLimite = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoStepVariacao": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoStepVariacao = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoVariacaoPulso": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoVariacaoPulso = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoVelocidade": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoVelocidade = aux.Conteudo; break; }
                case "Configuracoes_lblLogProcessoDispensa": { Negocio.IdiomaResxExtensao.Configuracoes_lblLogProcessoDispensa = aux.Conteudo; break; }
                case "Configuracoes_lblLogQuantidadeDispensa": { Negocio.IdiomaResxExtensao.Configuracoes_lblLogQuantidadeDispensa = aux.Conteudo; break; }
                case "Configuracoes_lblMonitAceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitAceleracao = aux.Conteudo; break; }
                case "Configuracoes_lblMonitCircuitoPorGrupo": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitCircuitoPorGrupo = aux.Conteudo; break; }
                case "Configuracoes_lblMonitCircuitoPorGrupoDesc": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitCircuitoPorGrupoDesc = aux.Conteudo; break; }
                case "Configuracoes_lblMonitDelay": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitDelay = aux.Conteudo; break; }
                case "Configuracoes_lblMonitPulsos": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitPulsos = aux.Conteudo; break; }
                case "Configuracoes_lblMonitTempo": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitTempo = aux.Conteudo; break; }
                case "Configuracoes_lblMonitTempoInicial": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitTempoInicial = aux.Conteudo; break; }
                case "Configuracoes_lblMonitVelocidade": { Negocio.IdiomaResxExtensao.Configuracoes_lblMonitVelocidade = aux.Conteudo; break; }
                case "Configuracoes_lblNivelMaximo": { Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMaximo = aux.Conteudo; break; }
                case "Configuracoes_lblNivelMinimo": { Negocio.IdiomaResxExtensao.Configuracoes_lblNivelMinimo = aux.Conteudo; break; }
                case "Configuracoes_lblNomeP1": { Negocio.IdiomaResxExtensao.Configuracoes_lblNomeP1 = aux.Conteudo; break; }
                case "Configuracoes_lblNomeP2": { Negocio.IdiomaResxExtensao.Configuracoes_lblNomeP2 = aux.Conteudo; break; }
                case "Configuracoes_lblProducaoIp": { Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoIp = aux.Conteudo; break; }
                case "Configuracoes_lblProducaoPorta": { Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoPorta = aux.Conteudo; break; }
                case "Configuracoes_lblProducaoTipo": { Negocio.IdiomaResxExtensao.Configuracoes_lblProducaoTipo = aux.Conteudo; break; }
                case "Configuracoes_lblPurgaAceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaAceleracao = aux.Conteudo; break; }
                case "Configuracoes_lblPurgaDelay": { Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaDelay = aux.Conteudo; break; }
                case "Configuracoes_lblPurgaHoras": { Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaHoras = aux.Conteudo; break; }
                case "Configuracoes_lblPurgaParametrosDispensa": { Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaParametrosDispensa = aux.Conteudo; break; }
                case "Configuracoes_lblPurgaVelocidade": { Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaVelocidade = aux.Conteudo; break; }
                case "Configuracoes_lblPurgaVolume": { Negocio.IdiomaResxExtensao.Configuracoes_lblPurgaVolume = aux.Conteudo; break; }
                case "Configuracoes_lblTitulo": { Negocio.IdiomaResxExtensao.Configuracoes_lblTitulo = aux.Conteudo; break; }
                case "Configuracoes_lblUniMedidaUnidadeExibicao": { Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeExibicao = aux.Conteudo; break; }
                case "Configuracoes_lblUniMedidaUnidadesHabilitadas": { Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadesHabilitadas = aux.Conteudo; break; }
                case "Configuracoes_rdbBaseDat00": { Negocio.IdiomaResxExtensao.Configuracoes_rdbBaseDat00 = aux.Conteudo; break; }
                case "Configuracoes_rdbDatBase01": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatBase01 = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao01": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao01 = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao02": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao02 = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao03": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao03 = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao04": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao04 = aux.Conteudo; break; }
                case "Configuracoes_SincFormula": { Negocio.IdiomaResxExtensao.Configuracoes_SincFormula = aux.Conteudo; break; }
                case "Configuracoes_tabAdmin": { Negocio.IdiomaResxExtensao.Configuracoes_tabAdmin = aux.Conteudo; break; }
                case "Configuracoes_tabCalibragem": { Negocio.IdiomaResxExtensao.Configuracoes_tabCalibragem = aux.Conteudo; break; }
                case "Configuracoes_tabColorantes": { Negocio.IdiomaResxExtensao.Configuracoes_tabColorantes = aux.Conteudo; break; }
                case "Configuracoes_tabControleVolume": { Negocio.IdiomaResxExtensao.Configuracoes_tabControleVolume = aux.Conteudo; break; }
                case "Configuracoes_tabDAT": { Negocio.IdiomaResxExtensao.Configuracoes_tabDAT = aux.Conteudo; break; }
                case "Configuracoes_tabInicializarCircuito": { Negocio.IdiomaResxExtensao.Configuracoes_tabInicializarCircuito = aux.Conteudo; break; }
                case "Configuracoes_tabLog": { Negocio.IdiomaResxExtensao.Configuracoes_tabLog = aux.Conteudo; break; }
                case "Configuracoes_tabMonit": { Negocio.IdiomaResxExtensao.Configuracoes_tabMonit = aux.Conteudo; break; }
                case "Configuracoes_tabParametrosGerais": { Negocio.IdiomaResxExtensao.Configuracoes_tabParametrosGerais = aux.Conteudo; break; }
                case "Configuracoes_tabParametrosPurga": { Negocio.IdiomaResxExtensao.Configuracoes_tabParametrosPurga = aux.Conteudo; break; }
                case "Configuracoes_tabProducao": { Negocio.IdiomaResxExtensao.Configuracoes_tabProducao = aux.Conteudo; break; }
                case "Configuracoes_tabProdutos": { Negocio.IdiomaResxExtensao.Configuracoes_tabProdutos = aux.Conteudo; break; }
                case "Configuracoes_tabUnidadeMedida": { Negocio.IdiomaResxExtensao.Configuracoes_tabUnidadeMedida = aux.Conteudo; break; }
                case "Configuracoes_UpFormula": { Negocio.IdiomaResxExtensao.Configuracoes_UpFormula = aux.Conteudo; break; }
                case "DispensaSequencial_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg01 = aux.Conteudo; break; }
                case "DispensaSequencial_lblStatus_Msg02": { Negocio.IdiomaResxExtensao.DispensaSequencial_lblStatus_Msg02 = aux.Conteudo; break; }
                case "DispensaSequencial_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "DispensaSequencial_lblSubStatus_Msg02": { Negocio.IdiomaResxExtensao.DispensaSequencial_lblSubStatus_Msg02 = aux.Conteudo; break; }
                case "DispensaSimultanea_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg01 = aux.Conteudo; break; }
                case "DispensaSimultanea_lblStatus_Msg02": { Negocio.IdiomaResxExtensao.DispensaSimultanea_lblStatus_Msg02 = aux.Conteudo; break; }
                case "DispensaSimultanea_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.DispensaSimultanea_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "FormulaPersonalizada_Informacao_ColoranteExistente": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informacao_ColoranteExistente = aux.Conteudo; break; }
                case "FormulaPersonalizada_Informacao_QuantidadeNaoInformada": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informacao_QuantidadeNaoInformada = aux.Conteudo; break; }
                case "FormulaPersonalizada_Informaco_InformarColorante": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informaco_InformarColorante = aux.Conteudo; break; }
                case "FormulaPersonalizada_Informaco_InformarFormula": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Informaco_InformarFormula = aux.Conteudo; break; }
                case "FormulaPersonalizada_Label_0009": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0009 = aux.Conteudo; break; }
                case "FormulaPersonalizada_Label_0010": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0010 = aux.Conteudo; break; }
                case "FormulaPersonalizada_Label_0011": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0011 = aux.Conteudo; break; }
                case "FormulaPersonalizada_Label_0012": { Negocio.IdiomaResxExtensao.FormulaPersonalizada_Label_0012 = aux.Conteudo; break; }
                case "GerenciarFormulaPersonalizada_Label_0007": { Negocio.IdiomaResxExtensao.GerenciarFormulaPersonalizada_Label_0007 = aux.Conteudo; break; }
                case "GerenciarFormulaPersonalizada_Label_0008": { Negocio.IdiomaResxExtensao.GerenciarFormulaPersonalizada_Label_0008 = aux.Conteudo; break; }
                case "GerenciarNivel_btnAbastecerTodos": { Negocio.IdiomaResxExtensao.GerenciarNivel_btnAbastecerTodos = aux.Conteudo; break; }
                case "GerenciarNivel_btnZerarTodos": { Negocio.IdiomaResxExtensao.GerenciarNivel_btnZerarTodos = aux.Conteudo; break; }
                case "GerenciarNivel_Confirmar_ZerarNivel": { Negocio.IdiomaResxExtensao.GerenciarNivel_Confirmar_ZerarNivel = aux.Conteudo; break; }
                case "GerenciarNivel_Informacao_ColorantesDesabilitados": { Negocio.IdiomaResxExtensao.GerenciarNivel_Informacao_ColorantesDesabilitados = aux.Conteudo; break; }
                case "GerenciarNivel_lblTitulo": { Negocio.IdiomaResxExtensao.GerenciarNivel_lblTitulo = aux.Conteudo; break; }
                case "Global_Abortar": { Negocio.IdiomaResxExtensao.Global_Abortar = aux.Conteudo; break; }
                case "Global_AcessoNegado": { Negocio.IdiomaResxExtensao.Global_AcessoNegado = aux.Conteudo; break; }
                case "Global_BaixaColoranteEfetuada": { Negocio.IdiomaResxExtensao.Global_BaixaColoranteEfetuada = aux.Conteudo; break; }
                case "Global_Cancelar": { Negocio.IdiomaResxExtensao.Global_Cancelar = aux.Conteudo; break; }
                case "Global_Circuito": { Negocio.IdiomaResxExtensao.Global_Circuito = aux.Conteudo; break; }
                case "Global_Colorante": { Negocio.IdiomaResxExtensao.Global_Colorante = aux.Conteudo; break; }
                case "Global_Dispositivo_Nenhum": { Negocio.IdiomaResxExtensao.Global_Dispositivo_Nenhum = aux.Conteudo; break; }
                case "Global_Dispositivo_Placa_1": { Negocio.IdiomaResxExtensao.Global_Dispositivo_Placa_1 = aux.Conteudo; break; }
                case "Global_Dispositivo_Placa_2": { Negocio.IdiomaResxExtensao.Global_Dispositivo_Placa_2 = aux.Conteudo; break; }
                case "Global_Dispositivo_Simulador": { Negocio.IdiomaResxExtensao.Global_Dispositivo_Simulador = aux.Conteudo; break; }
                case "Global_Iniciar": { Negocio.IdiomaResxExtensao.Global_Iniciar = aux.Conteudo; break; }
                case "Global_IOConnectInicializado": { Negocio.IdiomaResxExtensao.Global_IOConnectInicializado = aux.Conteudo; break; }
                case "Global_IOConnectInicializadoModoSimulacao": { Negocio.IdiomaResxExtensao.Global_IOConnectInicializadoModoSimulacao = aux.Conteudo; break; }
                case "Global_Label_Dias": { Negocio.IdiomaResxExtensao.Global_Label_Dias = aux.Conteudo; break; }
                case "Global_MonitAbortada": { Negocio.IdiomaResxExtensao.Global_MonitAbortada = aux.Conteudo; break; }
                case "Global_MonitCancelada": { Negocio.IdiomaResxExtensao.Global_MonitCancelada = aux.Conteudo; break; }
                case "Global_MonitConcluida": { Negocio.IdiomaResxExtensao.Global_MonitConcluida = aux.Conteudo; break; }
                case "Global_MonitoramentoPausado": { Negocio.IdiomaResxExtensao.Global_MonitoramentoPausado = aux.Conteudo; break; }
                case "Global_Monitorando": { Negocio.IdiomaResxExtensao.Global_Monitorando = aux.Conteudo; break; }
                case "Global_Nao": { Negocio.IdiomaResxExtensao.Global_Nao = aux.Conteudo; break; }
                case "Global_Sim": { Negocio.IdiomaResxExtensao.Global_Sim = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Abreviacao_Grama": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Grama = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Abreviacao_Mililitro": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Mililitro = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Abreviacao_Onca": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Onca = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Abreviacao_Shot": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Shot = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Grama": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Grama = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Mililitro": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Mililitro = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Onca": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Onca = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Shot": { Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Shot = aux.Conteudo; break; }
                case "Historico_ColunaDelay": { Negocio.IdiomaResxExtensao.Historico_ColunaDelay = aux.Conteudo; break; }
                case "Historico_ColunaDesvio": { Negocio.IdiomaResxExtensao.Historico_ColunaDesvio = aux.Conteudo; break; }
                case "Historico_ColunaMassa": { Negocio.IdiomaResxExtensao.Historico_ColunaMassa = aux.Conteudo; break; }
                case "Historico_ColunaPulsos": { Negocio.IdiomaResxExtensao.Historico_ColunaPulsos = aux.Conteudo; break; }
                case "Historico_ColunaVelocidade": { Negocio.IdiomaResxExtensao.Historico_ColunaVelocidade = aux.Conteudo; break; }
                case "Historico_lblHistoricoLegendaVolume": { Negocio.IdiomaResxExtensao.Historico_lblHistoricoLegendaVolume = aux.Conteudo; break; }
                case "Historico_Titulo": { Negocio.IdiomaResxExtensao.Historico_Titulo = aux.Conteudo; break; }
                case "IncializacaoCircuitos_ProgressBar_Msg01": { Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg01 = aux.Conteudo; break; }
                case "IncializacaoCircuitos_ProgressBar_Msg02": { Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg02 = aux.Conteudo; break; }
                case "IncializacaoCircuitos_ProgressBar_Msg03": { Negocio.IdiomaResxExtensao.IncializacaoCircuitos_ProgressBar_Msg03 = aux.Conteudo; break; }
                case "InicializacaoCircuitos_btnInicializacaoExecutar": { Negocio.IdiomaResxExtensao.InicializacaoCircuitos_btnInicializacaoExecutar = aux.Conteudo; break; }
                case "Init_ArquivoNaoEncontrado": { Negocio.IdiomaResxExtensao.Init_ArquivoNaoEncontrado = aux.Conteudo; break; }
                case "Licenca_lblChave": { Negocio.IdiomaResxExtensao.Licenca_lblChave = aux.Conteudo; break; }
                case "Licenca_lblLicenca": { Negocio.IdiomaResxExtensao.Licenca_lblLicenca = aux.Conteudo; break; }
                case "Licenca_lblTitulo": { Negocio.IdiomaResxExtensao.Licenca_lblTitulo = aux.Conteudo; break; }
                case "LogProcesso_DatRefinidoLeitura": { Negocio.IdiomaResxExtensao.LogProcesso_DatRefinidoLeitura = aux.Conteudo; break; }
                case "LogProcesso_SucessoLeituraConteudoDat": { Negocio.IdiomaResxExtensao.LogProcesso_SucessoLeituraConteudoDat = aux.Conteudo; break; }
                case "Nivel_lblTitulo": { Negocio.IdiomaResxExtensao.Nivel_lblTitulo = aux.Conteudo; break; }
                case "Nivel_lblUnidadeMedida": { Negocio.IdiomaResxExtensao.Nivel_lblUnidadeMedida = aux.Conteudo; break; }
                case "NumeroSerie_lblNumeroSerie": { Negocio.IdiomaResxExtensao.NumeroSerie_lblNumeroSerie = aux.Conteudo; break; }
                case "NumeroSerie_lblTitulo": { Negocio.IdiomaResxExtensao.NumeroSerie_lblTitulo = aux.Conteudo; break; }
                case "PainelControle_Menu_Configuracoes": { Negocio.IdiomaResxExtensao.PainelControle_Menu_Configuracoes = aux.Conteudo; break; }
                case "PainelControle_Menu_FormulasPersonalizadas": { Negocio.IdiomaResxExtensao.PainelControle_Menu_FormulasPersonalizadas = aux.Conteudo; break; }
                case "PainelControle_Menu_NivelColorante": { Negocio.IdiomaResxExtensao.PainelControle_Menu_NivelColorante = aux.Conteudo; break; }
                case "PainelControle_Menu_Purga": { Negocio.IdiomaResxExtensao.PainelControle_Menu_Purga = aux.Conteudo; break; }
                case "PainelControle_Menu_PurgaIndividual": { Negocio.IdiomaResxExtensao.PainelControle_Menu_PurgaIndividual = aux.Conteudo; break; }
                case "PainelControle_Menu_Sair": { Negocio.IdiomaResxExtensao.PainelControle_Menu_Sair = aux.Conteudo; break; }
                case "PainelControle_Menu_Sobre": { Negocio.IdiomaResxExtensao.PainelControle_Menu_Sobre = aux.Conteudo; break; }
                case "PainelControle_Menu_Treinamento": { Negocio.IdiomaResxExtensao.PainelControle_Menu_Treinamento = aux.Conteudo; break; }
                case "Pesagem_btnPesagemAdicionar": { Negocio.IdiomaResxExtensao.Pesagem_btnPesagemAdicionar = aux.Conteudo; break; }
                case "Pesagem_btnPesagemConfirmar": { Negocio.IdiomaResxExtensao.Pesagem_btnPesagemConfirmar = aux.Conteudo; break; }
                case "Pesagem_btnPesagemDispensar": { Negocio.IdiomaResxExtensao.Pesagem_btnPesagemDispensar = aux.Conteudo; break; }
                case "Pesagem_chkPesagemRedefinirPulsos": { Negocio.IdiomaResxExtensao.Pesagem_chkPesagemRedefinirPulsos = aux.Conteudo; break; }
                case "Pesagem_lblPesagemDesvio": { Negocio.IdiomaResxExtensao.Pesagem_lblPesagemDesvio = aux.Conteudo; break; }
                case "Pesagem_lblPesagemLegendaMassaIdeal": { Negocio.IdiomaResxExtensao.Pesagem_lblPesagemLegendaMassaIdeal = aux.Conteudo; break; }
                case "Pesagem_lblPesagemLegendaVolume": { Negocio.IdiomaResxExtensao.Pesagem_lblPesagemLegendaVolume = aux.Conteudo; break; }
                case "Pesagem_lblPesagemMassa": { Negocio.IdiomaResxExtensao.Pesagem_lblPesagemMassa = aux.Conteudo; break; }
                case "Purgar_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg01 = aux.Conteudo; break; }
                case "Purgar_lblStatus_Msg02": { Negocio.IdiomaResxExtensao.Purgar_lblStatus_Msg02 = aux.Conteudo; break; }
                case "Purgar_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "Purgar_lblSubStatus_Msg02": { Negocio.IdiomaResxExtensao.Purgar_lblSubStatus_Msg02 = aux.Conteudo; break; }
                case "PurgarIndividual_gbIndividual": { Negocio.IdiomaResxExtensao.PurgarIndividual_gbIndividual = aux.Conteudo; break; }
                case "PurgarIndividual_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.PurgarIndividual_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "PurgarIndividual_lblSubStatus_Msg02": { Negocio.IdiomaResxExtensao.PurgarIndividual_lblSubStatus_Msg02 = aux.Conteudo; break; }
                case "PurgarIndividualMonit_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.PurgarIndividualMonit_lblStatus_Msg01 = aux.Conteudo; break; }
                case "PurgarMonit_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.PurgarMonit_lblStatus_Msg01 = aux.Conteudo; break; }
                case "PurgarMonit_lblStatus_Msg02": { Negocio.IdiomaResxExtensao.PurgarMonit_lblStatus_Msg02 = aux.Conteudo; break; }
                case "PurgarMonit_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.PurgarMonit_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "PurgarMonit_lblSubStatus_Msg02": { Negocio.IdiomaResxExtensao.PurgarMonit_lblSubStatus_Msg02 = aux.Conteudo; break; }
                case "Sobre_Label_0000": { Negocio.IdiomaResxExtensao.Sobre_Label_0000 = aux.Conteudo; break; }
                case "Sobre_Label_0001": { Negocio.IdiomaResxExtensao.Sobre_Label_0001 = aux.Conteudo; break; }
                case "Sobre_Label_0003": { Negocio.IdiomaResxExtensao.Sobre_Label_0003 = aux.Conteudo; break; }
                case "Sobre_Label_0004": { Negocio.IdiomaResxExtensao.Sobre_Label_0004 = aux.Conteudo; break; }
                case "Sobre_Label_0005": { Negocio.IdiomaResxExtensao.Sobre_Label_0005 = aux.Conteudo; break; }
                case "Sobre_Label_0006": { Negocio.IdiomaResxExtensao.Sobre_Label_0006 = aux.Conteudo; break; }
                case "Sobre_Label_0007": { Negocio.IdiomaResxExtensao.Sobre_Label_0007 = aux.Conteudo; break; }
                case "Sobre_lnkRedefinirManutencao": { Negocio.IdiomaResxExtensao.Sobre_lnkRedefinirManutencao = aux.Conteudo; break; }
                case "ValidadeManutencao_lblNumeroSerie": { Negocio.IdiomaResxExtensao.ValidadeManutencao_lblNumeroSerie = aux.Conteudo; break; }
                case "ValidadeManutencao_lblTitulo": { Negocio.IdiomaResxExtensao.ValidadeManutencao_lblTitulo = aux.Conteudo; break; }
                case "Configuracoes_lblInicializacaoCircuitoPorGrupoDesc": { Negocio.IdiomaResxExtensao.Configuracoes_lblInicializacaoCircuitoPorGrupoDesc = aux.Conteudo; break; }
                case "Configuracoes_chkExigirExecucaoPurga": { Negocio.IdiomaResxExtensao.Configuracoes_chkExigirExecucaoPurga = aux.Conteudo; break; }
                case "Configuracoes_chkPurgaSequencial": { Negocio.IdiomaResxExtensao.Configuracoes_chkPurgaSequencial = aux.Conteudo; break; }
                case "Configuracoes_lblAceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_lblAceleracao = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemPulsosRev": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemPulsosRev = aux.Conteudo; break; }
                case "Calibragem_DelayMaiorZero": { Negocio.IdiomaResxExtensao.Calibragem_DelayMaiorZero = aux.Conteudo; break; }
                case "Calibragem_DelayMenorIgual": { Negocio.IdiomaResxExtensao.Calibragem_DelayMenorIgual = aux.Conteudo; break; }
                case "Calibragem_NaoContemValor": { Negocio.IdiomaResxExtensao.Calibragem_NaoContemValor = aux.Conteudo; break; }
                case "Calibragem_QuantidadePulsosMaiorZero": { Negocio.IdiomaResxExtensao.Calibragem_QuantidadePulsosMaiorZero = aux.Conteudo; break; }
                case "Calibragem_VelocidadeMaiorZero": { Negocio.IdiomaResxExtensao.Calibragem_VelocidadeMaiorZero = aux.Conteudo; break; }
                case "Calibragem_VelocidadeMenorIgual": { Negocio.IdiomaResxExtensao.Calibragem_VelocidadeMenorIgual = aux.Conteudo; break; }
                case "Colorantes_Circuito": { Negocio.IdiomaResxExtensao.Colorantes_Circuito = aux.Conteudo; break; }
                case "Colorantes_CorrespondenciaRepetida": { Negocio.IdiomaResxExtensao.Colorantes_CorrespondenciaRepetida = aux.Conteudo; break; }
                case "Colorantes_MassaEspecificaObrigatoria": { Negocio.IdiomaResxExtensao.Colorantes_MassaEspecificaObrigatoria = aux.Conteudo; break; }
                case "Colorantes_NomeObrigatorio": { Negocio.IdiomaResxExtensao.Colorantes_NomeObrigatorio = aux.Conteudo; break; }
                case "Parametors_Comunicacao_TimeoutFaixa": { Negocio.IdiomaResxExtensao.Parametors_Comunicacao_TimeoutFaixa = aux.Conteudo; break; }
                case "Parametros_Dat_DiretorioInvalido": { Negocio.IdiomaResxExtensao.Parametros_Dat_DiretorioInvalido = aux.Conteudo; break; }
                case "Parametros_Dat_PadraoConteudoObrigatorio": { Negocio.IdiomaResxExtensao.Parametros_Dat_PadraoConteudoObrigatorio = aux.Conteudo; break; }
                case "Parametros_Dat_RepositorioInvalido": { Negocio.IdiomaResxExtensao.Parametros_Dat_RepositorioInvalido = aux.Conteudo; break; }
                case "Parametros_Global_AceleracaoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Global_AceleracaoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Global_VelocidadeMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Global_VelocidadeMaiorZero = aux.Conteudo; break; }
                case "Parametros_Inicializacao_AceleracaoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_AceleracaoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Inicializacao_PulsoInicialMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_PulsoInicialMaiorZero = aux.Conteudo; break; }
                case "Parametros_Inicializacao_PulsoLimiteMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_PulsoLimiteMaiorZero = aux.Conteudo; break; }
                case "Parametros_Inicializacao_QtdeCircuitosGrupoFaixa": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_QtdeCircuitosGrupoFaixa = aux.Conteudo; break; }
                case "Parametros_Inicializacao_StepVariacaoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_StepVariacaoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Inicializacao_VariacaoPulsoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_VariacaoPulsoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Inicializacao_VelocidadeMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Inicializacao_VelocidadeMaiorZero = aux.Conteudo; break; }
                case "Parametros_Monit_AceleracaoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Monit_AceleracaoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Monit_DelayMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Monit_DelayMaiorZero = aux.Conteudo; break; }
                case "Parametros_Monit_PulsoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Monit_PulsoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Monit_QtdeCircuitosGrupoFaixa": { Negocio.IdiomaResxExtensao.Parametros_Monit_QtdeCircuitosGrupoFaixa = aux.Conteudo; break; }
                case "Parametros_Monit_TimerDelayIniMaiorDez": { Negocio.IdiomaResxExtensao.Parametros_Monit_TimerDelayIniMaiorDez = aux.Conteudo; break; }
                case "Parametros_Monit_TimerDelayMaiorDez": { Negocio.IdiomaResxExtensao.Parametros_Monit_TimerDelayMaiorDez = aux.Conteudo; break; }
                case "Parametros_Monit_VelocidadeMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Monit_VelocidadeMaiorZero = aux.Conteudo; break; }
                case "Parametros_Nivel_MaximoMaiorQueMInimo": { Negocio.IdiomaResxExtensao.Parametros_Nivel_MaximoMaiorQueMInimo = aux.Conteudo; break; }
                case "Parametros_Nivel_MaximoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Nivel_MaximoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Nivel_MinimoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Nivel_MinimoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Purga_AceleracaoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Purga_AceleracaoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Purga_PrazoExecucaoMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Purga_PrazoExecucaoMaiorZero = aux.Conteudo; break; }
                case "Parametros_Purga_VelocidadeMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Purga_VelocidadeMaiorZero = aux.Conteudo; break; }
                case "Parametros_Purga_VolumeMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_Purga_VolumeMaiorZero = aux.Conteudo; break; }
                case "Parametros_UnidadeMedida_ShotMaiorZero": { Negocio.IdiomaResxExtensao.Parametros_UnidadeMedida_ShotMaiorZero = aux.Conteudo; break; }
                case "Parametros_UnidadeMedida_UnidadeEntradaObrigatoria": { Negocio.IdiomaResxExtensao.Parametros_UnidadeMedida_UnidadeEntradaObrigatoria = aux.Conteudo; break; }
                case "Parametros_UnidadeMedida_UnidadeExibicaoObrigatoria": { Negocio.IdiomaResxExtensao.Parametros_UnidadeMedida_UnidadeExibicaoObrigatoria = aux.Conteudo; break; }
                case "Configurador_chkDisableFilaDat": { Negocio.IdiomaResxExtensao.Configurador_chkDisableFilaDat = aux.Conteudo; break; }
                case "Configurador_lblDisableMonitFilaDat": { Negocio.IdiomaResxExtensao.Configurador_lblDisableMonitFilaDat = aux.Conteudo; break; }
                case "fMonitoramentoFilaDat_ColunaGrid_Path": { Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_ColunaGrid_Path = aux.Conteudo; break; }
                case "fMonitoramentoFilaDat_lblStatus": { Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_lblStatus = aux.Conteudo; break; }
                case "PainelControele_Notificacao": { Negocio.IdiomaResxExtensao.PainelControele_Notificacao = aux.Conteudo; break; }
                case "fGerenciarFormula_GridColumn_CodigoCor": { Negocio.IdiomaResxExtensao.fGerenciarFormula_GridColumn_CodigoCor = aux.Conteudo; break; }
                case "fGerenciarFormula_GridColumn_DescCor": { Negocio.IdiomaResxExtensao.fGerenciarFormula_GridColumn_DescCor = aux.Conteudo; break; }
                case "fMonitoramentoFilaDat_lblTitulo": { Negocio.IdiomaResxExtensao.fMonitoramentoFilaDat_lblTitulo = aux.Conteudo; break; }
                case "fNotificacaoFilaDat_lblDesc": { Negocio.IdiomaResxExtensao.fNotificacaoFilaDat_lblDesc = aux.Conteudo; break; }
                case "Configuracoes_lblDelayMonitoramentoFilaDAT": { Negocio.IdiomaResxExtensao.Configuracoes_lblDelayMonitoramentoFilaDAT = aux.Conteudo; break; }
                case "Configuracoes_chkDisableFilaDat": { Negocio.IdiomaResxExtensao.Configuracoes_chkDisableFilaDat = aux.Conteudo; break; }
                case "Configuracoes_chkDisablePopUpDispDat": { Negocio.IdiomaResxExtensao.Configuracoes_chkDisablePopUpDispDat = aux.Conteudo; break; }

                case "Configuracoes_btnTesteComunicacao": { Negocio.IdiomaResxExtensao.Configuracoes_btnTesteComunicacao = aux.Conteudo; break; }
                case "Configuracoes_ComSuccessDispositivo1": { Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo1 = aux.Conteudo; break; }
                case "Configuracoes_ComErroDispositivo1": { Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo1 = aux.Conteudo; break; }
                case "Configuracoes_ComSuccessDispositivo2": { Negocio.IdiomaResxExtensao.Configuracoes_ComSuccessDispositivo2 = aux.Conteudo; break; }
                case "Configuracoes_ComErroDispositivo2": { Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivo2 = aux.Conteudo; break; }
                case "Configuracoes_ComErroDispositivoGeral": { Negocio.IdiomaResxExtensao.Configuracoes_ComErroDispositivoGeral = aux.Conteudo; break; }
                case "Configuracoes_btnPrimeiraCalibracao": { Negocio.IdiomaResxExtensao.Configuracoes_btnPrimeiraCalibracao = aux.Conteudo; break; }
                case "Global_Aguarde_ProgressBar": { Negocio.IdiomaResxExtensao.Global_Aguarde_ProgressBar = aux.Conteudo; break; }
                case "Configuracoes_tabCalibragemAuto": { Negocio.IdiomaResxExtensao.Configuracoes_tabCalibragemAuto = aux.Conteudo; break; }
                case "Pesagem_rd_pulsos": { Negocio.IdiomaResxExtensao.Pesagem_rd_pulsos = aux.Conteudo; break; }
                case "Pesagem_rd_pulsos_faixa": { Negocio.IdiomaResxExtensao.Pesagem_rd_pulsos_faixa = aux.Conteudo; break; }
                case "Pesagem_rd_sem_pulsos": { Negocio.IdiomaResxExtensao.Pesagem_rd_sem_pulsos = aux.Conteudo; break; }
                case "Pesagem_chb_balanca_Pesagem": { Negocio.IdiomaResxExtensao.Pesagem_chb_balanca_Pesagem = aux.Conteudo; break; }
                case "Pesagem_gbComunicacaoBalanca": { Negocio.IdiomaResxExtensao.Pesagem_gbComunicacaoBalanca = aux.Conteudo; break; }
                case "Configuracoes_Data_Instalacao": { Negocio.IdiomaResxExtensao.Configuracoes_Data_Instalacao = aux.Conteudo; break; }
                case "Configuracoes_tabCalibragemManual": { Negocio.IdiomaResxExtensao.Configuracoes_tabCalibragemManual = aux.Conteudo; break; }
                case "PainelControle_AutoTesterProtocol": { Negocio.IdiomaResxExtensao.PainelControle_AutoTesterProtocol = aux.Conteudo; break; }
                case "Configuracoes_chkLogComunicacao": { Negocio.IdiomaResxExtensao.Configuracoes_chkLogComunicacao = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoMinimoFaixas": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoMinimoFaixas = aux.Conteudo; break; }
                case "Configuracoes_ReplicarFaixasCal": { Negocio.IdiomaResxExtensao.Configuracoes_ReplicarFaixasCal = aux.Conteudo; break; }
                case "Configuracoes_AdicionarFaixasCal": { Negocio.IdiomaResxExtensao.Configuracoes_AdicionarFaixasCal = aux.Conteudo; break; }
                case "fCalibracao_lblTitulo": { Negocio.IdiomaResxExtensao.fCalibracao_lblTitulo = aux.Conteudo; break; }
                case "Global_Confirma": { Negocio.IdiomaResxExtensao.Global_Confirma = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoVolumeMaxRecipiente": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoVolumeMaxRecipiente = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoTentativaPosicionamento": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoTentativaPosicionamento = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoPesoRecipiente": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPesoRecipiente = aux.Conteudo; break; }
                case "Configuracoes_lblCalibragemLegendaUltimoPulsoRev": { Negocio.IdiomaResxExtensao.Configuracoes_lblCalibragemLegendaUltimoPulsoRev = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoIniciar": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoIniciar = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoHabilitarEditFaixa": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoHabilitarEditFaixa = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoFaixasCal": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoFaixasCal = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoPrimeiraCal": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoPrimeiraCal = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoOperacao": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoOperacao = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoMotor": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoMotor = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoDesvioAdm": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoDesvioAdm = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoIsCalibracao": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoIsCalibracao = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoNumeroTentativa": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoNumeroTentativa = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoMediaMedicao": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoMediaMedicao = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoNumeroMedia": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoNumeroMedia = aux.Conteudo; break; }
                case "Configuracoes_CalibracaoAutoVolume": { Negocio.IdiomaResxExtensao.Configuracoes_CalibracaoAutoVolume = aux.Conteudo; break; }
                case "Pesagem_Titulo": { Negocio.IdiomaResxExtensao.Pesagem_Titulo = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Titulo": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Titulo = aux.Conteudo; break; }
                case "CalibracaoAutomatica_lblCapacidadeMaxBalanca": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblCapacidadeMaxBalanca = aux.Conteudo; break; }
                case "CalibracaoAutomatica_lblDelayBalanca": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblDelayBalanca = aux.Conteudo; break; }
                case "CalibracaoAutomatica_ComBalanca": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_ComBalanca = aux.Conteudo; break; }
                case "CalibracaoAutomatica_lblTituloMassaBal": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_lblTituloMassaBal = aux.Conteudo; break; }
                case "Global_Start": { Negocio.IdiomaResxExtensao.Global_Start = aux.Conteudo; break; }
                case "Global_Stop": { Negocio.IdiomaResxExtensao.Global_Stop = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Etapa": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Etapa = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Motor": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Motor = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Tentativa": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Tentativa = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Volume": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Volume = aux.Conteudo; break; }
                case "CalibracaoAutomatica_VolumeDos": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_VolumeDos = aux.Conteudo; break; }
                case "CalibracaoAutomatica_MassaIdeal": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_MassaIdeal = aux.Conteudo; break; }
                case "CalibracaoAutomatica_MassaMedBalanca": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_MassaMedBalanca = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Desvio": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Desvio = aux.Conteudo; break; }
                case "CalibracaoAutomatica_DesvioAdmissel": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_DesvioAdmissel = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Aprovado": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Aprovado = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Executado": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Executado = aux.Conteudo; break; }
                case "CalibracaoAutomatica_PosicionamentoReciepiente": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_PosicionamentoReciepiente = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Refazer": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Refazer = aux.Conteudo; break; }
                case "CalibracaoAutomatica_Reprovado": { Negocio.IdiomaResxExtensao.CalibracaoAutomatica_Reprovado = aux.Conteudo; break; }
                case "CalibracaoAutomate_SemArquivoOuCircuito": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_SemArquivoOuCircuito = aux.Conteudo; break; }
                case "CalibracaoAutomate_ExportArquivo": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_ExportArquivo = aux.Conteudo; break; }
                case "CalibracaoAuto_MaxBalancaOuVolumeRecipiente": { Negocio.IdiomaResxExtensao.CalibracaoAuto_MaxBalancaOuVolumeRecipiente = aux.Conteudo; break; }
                case "CalibracaoAuto_ErrorReadBalanca": { Negocio.IdiomaResxExtensao.CalibracaoAuto_ErrorReadBalanca = aux.Conteudo; break; }
                case "CalibracaoAuto_PosicaoRecipiente": { Negocio.IdiomaResxExtensao.CalibracaoAuto_PosicaoRecipiente = aux.Conteudo; break; }
                case "CalibracaoAuto_PesoRecipienteExcedido": { Negocio.IdiomaResxExtensao.CalibracaoAuto_PesoRecipienteExcedido = aux.Conteudo; break; }
                case "CalibracaoAutomate_Circuit": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_Circuit = aux.Conteudo; break; }
                case "CalibracaoAutomate_lblTitulo": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblTitulo = aux.Conteudo; break; }
                case "CalibracaoAutomate_lblConfigSetup": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblConfigSetup = aux.Conteudo; break; }
                case "CalibracaoAutomate_lblModelScale": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblModelScale = aux.Conteudo; break; }
                case "CalibracaoAutomate_btnEditarDiretorio": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_btnEditarDiretorio = aux.Conteudo; break; }
                case "CalibracaoAutomate_lblExecCircuito": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblExecCircuito = aux.Conteudo; break; }
                case "CalibracaoAutomate_lblFileImport": { Negocio.IdiomaResxExtensao.CalibracaoAutomate_lblFileImport = aux.Conteudo; break; }
                case "CalibracaoAuto_Pos_Recipiente": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Pos_Recipiente = aux.Conteudo; break; }
                case "CalibracaoAuto_Trocar_Recipiente": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Trocar_Recipiente = aux.Conteudo; break; }
                case "CalibracaoAuto_Lendo_Balanca": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Lendo_Balanca = aux.Conteudo; break; }
                case "CalibracaoAuto_DispensandoVolume": { Negocio.IdiomaResxExtensao.CalibracaoAuto_DispensandoVolume = aux.Conteudo; break; }
                case "CalibracaoAuto_SelecionarPortaSerialBalanca": { Negocio.IdiomaResxExtensao.CalibracaoAuto_SelecionarPortaSerialBalanca = aux.Conteudo; break; }
                case "Global_BtnAjuda": { Negocio.IdiomaResxExtensao.Global_BtnAjuda = aux.Conteudo; break; }
                case "Global_BtnClose": { Negocio.IdiomaResxExtensao.Global_BtnClose = aux.Conteudo; break; }
                case "Configuracao_Calibracao_LogAutomateTesterProt": { Negocio.IdiomaResxExtensao.Configuracao_Calibracao_LogAutomateTesterProt = aux.Conteudo; break; }
                case "RecircularIndividual_gbIndividual": { Negocio.IdiomaResxExtensao.RecircularIndividual_gbIndividual = aux.Conteudo; break; }
                case "RecircularIndividual_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "RecircularIndividual_lblSubStatus_Msg02": { Negocio.IdiomaResxExtensao.RecircularIndividual_lblSubStatus_Msg02 = aux.Conteudo; break; }
                case "RecircularIndividualMonit_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.RecircularIndividualMonit_lblStatus_Msg01 = aux.Conteudo; break; }
                case "RecircularIndividual_lblStatus_Msg02": { Negocio.IdiomaResxExtensao.RecircularIndividual_lblStatus_Msg02 = aux.Conteudo; break; }
                case "PainelControle_Menu_RecircularProdutos": { Negocio.IdiomaResxExtensao.PainelControle_Menu_RecircularProdutos = aux.Conteudo; break; }
                case "Configuracoes_tabRecircular": { Negocio.IdiomaResxExtensao.Configuracoes_tabRecircular = aux.Conteudo; break; }
                case "Configuracoes_chkHabilitarMonitRecirculacao": { Negocio.IdiomaResxExtensao.Configuracoes_chkHabilitarMonitRecirculacao = aux.Conteudo; break; }
                case "Configuracoes_lblDelayMonitRecirculacao": { Negocio.IdiomaResxExtensao.Configuracoes_lblDelayMonitRecirculacao = aux.Conteudo; break; }
                case "Configuracoes_lblTituloRecCircuito": { Negocio.IdiomaResxExtensao.Configuracoes_lblTituloRecCircuito = aux.Conteudo; break; }
                case "Configuracoes_lblRecVolDin": { Negocio.IdiomaResxExtensao.Configuracoes_lblRecVolDin = aux.Conteudo; break; }
                case "Configuracoes_lblRecPerDias": { Negocio.IdiomaResxExtensao.Configuracoes_lblRecPerDias = aux.Conteudo; break; }
                case "Configuracoes_lblRecVol": { Negocio.IdiomaResxExtensao.Configuracoes_lblRecVol = aux.Conteudo; break; }
                case "CalibracaoAuto_lblTotalizador": { Negocio.IdiomaResxExtensao.CalibracaoAuto_lblTotalizador = aux.Conteudo; break; }
                case "CalibracaoAuto_Tipo": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Tipo = aux.Conteudo; break; }
                case "CalibracaoAuto_TipoAll": { Negocio.IdiomaResxExtensao.CalibracaoAuto_TipoAll = aux.Conteudo; break; }
                case "CalibracaoAuto_TipoIndi": { Negocio.IdiomaResxExtensao.CalibracaoAuto_TipoIndi = aux.Conteudo; break; }
                case "CalibracaoAuto_BotaoNovaCalibracao": { Negocio.IdiomaResxExtensao.CalibracaoAuto_BotaoNovaCalibracao = aux.Conteudo; break; }
                case "CalibracaoAuto_BotaoContinueCalibracao": { Negocio.IdiomaResxExtensao.CalibracaoAuto_BotaoContinueCalibracao = aux.Conteudo; break; }
                case "CalibracaoAutoSemi_PosicaoRecipiente": { Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipiente = aux.Conteudo; break; }
                case "CalibracaoAutoSemi_PosicaoRecipienteMaquina": { Negocio.IdiomaResxExtensao.CalibracaoAutoSemi_PosicaoRecipienteMaquina = aux.Conteudo; break; }
                case "CalibracaoAuto_ProcAut": { Negocio.IdiomaResxExtensao.CalibracaoAuto_ProcAut = aux.Conteudo; break; }
                case "CalibracaoAuto_ProcSemiAut": { Negocio.IdiomaResxExtensao.CalibracaoAuto_ProcSemiAut = aux.Conteudo; break; }
                case "CalibracaoAuto_Processo": { Negocio.IdiomaResxExtensao.CalibracaoAuto_Processo = aux.Conteudo; break; }
                case "Configuracoes_btnRessetarDatas": { Negocio.IdiomaResxExtensao.Configuracoes_btnRessetarDatas = aux.Conteudo; break; }
                case "Usuario_NomeObrigatorio": { Negocio.IdiomaResxExtensao.Usuario_NomeObrigatorio = aux.Conteudo; break; }
                case "Usuario_SenhaObrigatorio": { Negocio.IdiomaResxExtensao.Usuario_SenhaObrigatorio = aux.Conteudo; break; }
                case "Usuario_NomeMsg": { Negocio.IdiomaResxExtensao.Usuario_NomeMsg = aux.Conteudo; break; }
                case "Configuracao_UsuarioPermExcluir": { Negocio.IdiomaResxExtensao.Configuracao_UsuarioPermExcluir = aux.Conteudo; break; }
                case "Configuracao_btnExcluirUsuario": { Negocio.IdiomaResxExtensao.Configuracao_btnExcluirUsuario = aux.Conteudo; break; }
                case "Configuracao_UsuarioErrorRemove": { Negocio.IdiomaResxExtensao.Configuracao_UsuarioErrorRemove = aux.Conteudo; break; }
                case "Configuracao_UsuarioMaster": { Negocio.IdiomaResxExtensao.Configuracao_UsuarioMaster = aux.Conteudo; break; }
                case "Configuracao_UsuarioGerente": { Negocio.IdiomaResxExtensao.Configuracao_UsuarioGerente = aux.Conteudo; break; }
                case "Configuracao_UsuarioTecnico": { Negocio.IdiomaResxExtensao.Configuracao_UsuarioTecnico = aux.Conteudo; break; }
                case "fAutenticacao_lblSenha": { Negocio.IdiomaResxExtensao.fAutenticacao_lblSenha = aux.Conteudo; break; }
                case "fAutenticacao_lblUsuario": { Negocio.IdiomaResxExtensao.fAutenticacao_lblUsuario = aux.Conteudo; break; }
                case "fAutenticacao_btnCancelar": { Negocio.IdiomaResxExtensao.fAutenticacao_btnCancelar = aux.Conteudo; break; }
                case "fAutenticacao_btnEntrar": { Negocio.IdiomaResxExtensao.fAutenticacao_btnEntrar = aux.Conteudo; break; }
                case "fAutenticacao_Falha_ValidarDadosUsuario": { Negocio.IdiomaResxExtensao.fAutenticacao_Falha_ValidarDadosUsuario = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao05": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao05 = aux.Conteudo; break; }
                case "Log_Cod_01": { Negocio.IdiomaResxExtensao.Log_Cod_01 = aux.Conteudo; break; }
                case "Log_Cod_02": { Negocio.IdiomaResxExtensao.Log_Cod_02 = aux.Conteudo; break; }
                case "Log_Cod_03": { Negocio.IdiomaResxExtensao.Log_Cod_03 = aux.Conteudo; break; }
                case "Log_Cod_04": { Negocio.IdiomaResxExtensao.Log_Cod_04 = aux.Conteudo; break; }
                case "Log_Cod_05": { Negocio.IdiomaResxExtensao.Log_Cod_05 = aux.Conteudo; break; }
                case "Log_Cod_06": { Negocio.IdiomaResxExtensao.Log_Cod_06 = aux.Conteudo; break; }
                case "Log_Cod_07": { Negocio.IdiomaResxExtensao.Log_Cod_07 = aux.Conteudo; break; }
                case "Log_Cod_08": { Negocio.IdiomaResxExtensao.Log_Cod_08 = aux.Conteudo; break; }
                case "Log_Cod_09": { Negocio.IdiomaResxExtensao.Log_Cod_09 = aux.Conteudo; break; }
                case "Log_Cod_10": { Negocio.IdiomaResxExtensao.Log_Cod_10 = aux.Conteudo; break; }
                case "Log_Cod_11": { Negocio.IdiomaResxExtensao.Log_Cod_11 = aux.Conteudo; break; }
                case "Log_Cod_12": { Negocio.IdiomaResxExtensao.Log_Cod_12 = aux.Conteudo; break; }
                case "Log_Cod_13": { Negocio.IdiomaResxExtensao.Log_Cod_13 = aux.Conteudo; break; }
                case "Log_Cod_14": { Negocio.IdiomaResxExtensao.Log_Cod_14 = aux.Conteudo; break; }
                case "Log_Cod_15": { Negocio.IdiomaResxExtensao.Log_Cod_15 = aux.Conteudo; break; }
                case "Log_Cod_16": { Negocio.IdiomaResxExtensao.Log_Cod_16 = aux.Conteudo; break; }
                case "Log_Cod_17": { Negocio.IdiomaResxExtensao.Log_Cod_17 = aux.Conteudo; break; }
                case "Log_Cod_18": { Negocio.IdiomaResxExtensao.Log_Cod_18 = aux.Conteudo; break; }
                case "Log_Cod_19": { Negocio.IdiomaResxExtensao.Log_Cod_19 = aux.Conteudo; break; }
                case "Log_Cod_20": { Negocio.IdiomaResxExtensao.Log_Cod_20 = aux.Conteudo; break; }
                case "Log_Cod_21": { Negocio.IdiomaResxExtensao.Log_Cod_21 = aux.Conteudo; break; }
                case "Log_Cod_22": { Negocio.IdiomaResxExtensao.Log_Cod_22 = aux.Conteudo; break; }
                case "Log_Cod_23": { Negocio.IdiomaResxExtensao.Log_Cod_23 = aux.Conteudo; break; }
                case "Log_Cod_24": { Negocio.IdiomaResxExtensao.Log_Cod_24 = aux.Conteudo; break; }
                case "Log_Cod_25": { Negocio.IdiomaResxExtensao.Log_Cod_25 = aux.Conteudo; break; }
                case "Log_Cod_26": { Negocio.IdiomaResxExtensao.Log_Cod_26 = aux.Conteudo; break; }
                case "Log_Cod_27": { Negocio.IdiomaResxExtensao.Log_Cod_27 = aux.Conteudo; break; }
                case "Log_Cod_28": { Negocio.IdiomaResxExtensao.Log_Cod_28 = aux.Conteudo; break; }
                case "Log_Cod_29": { Negocio.IdiomaResxExtensao.Log_Cod_29 = aux.Conteudo; break; }
                case "Log_Cod_30": { Negocio.IdiomaResxExtensao.Log_Cod_30 = aux.Conteudo; break; }
                case "Log_Cod_31": { Negocio.IdiomaResxExtensao.Log_Cod_31 = aux.Conteudo; break; }
                case "Log_Cod_32": { Negocio.IdiomaResxExtensao.Log_Cod_32 = aux.Conteudo; break; }
                case "Log_Cod_33": { Negocio.IdiomaResxExtensao.Log_Cod_33 = aux.Conteudo; break; }
                case "Log_Cod_34": { Negocio.IdiomaResxExtensao.Log_Cod_34 = aux.Conteudo; break; }
                case "Log_Cod_35": { Negocio.IdiomaResxExtensao.Log_Cod_35 = aux.Conteudo; break; }
                case "Log_Cod_36": { Negocio.IdiomaResxExtensao.Log_Cod_36 = aux.Conteudo; break; }
                case "Log_Cod_50": { Negocio.IdiomaResxExtensao.Log_Cod_50 = aux.Conteudo; break; }
                case "Log_Cod_51": { Negocio.IdiomaResxExtensao.Log_Cod_51 = aux.Conteudo; break; }
                case "Log_Cod_52": { Negocio.IdiomaResxExtensao.Log_Cod_52 = aux.Conteudo; break; }
                case "Log_Cod_53": { Negocio.IdiomaResxExtensao.Log_Cod_53 = aux.Conteudo; break; }
                case "Log_Cod_98": { Negocio.IdiomaResxExtensao.Log_Cod_98 = aux.Conteudo; break; }
                case "Log_Cod_99": { Negocio.IdiomaResxExtensao.Log_Cod_99 = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao06": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao06 = aux.Conteudo; break; }
                case "PainelControle_Menu_PlacaMov": { Negocio.IdiomaResxExtensao.PainelControle_Menu_PlacaMov = aux.Conteudo; break; }
                case "PlacaMov_Botao_Emergencia": { Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia = aux.Conteudo; break; }
                case "PlacaMov_Disp_Parou": { Negocio.IdiomaResxExtensao.PlacaMov_Disp_Parou = aux.Conteudo; break; }
                case "PlacaMov_Trocar_Recipiente": { Negocio.IdiomaResxExtensao.PlacaMov_Trocar_Recipiente = aux.Conteudo; break; }
                case "PlacaMov_Retirar_Recipiente": { Negocio.IdiomaResxExtensao.PlacaMov_Retirar_Recipiente = aux.Conteudo; break; }
                case "PlacaMov_Usar_Copo_Esponja": { Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Esponja = aux.Conteudo; break; }
                case "PlacaMov_Usar_Copo_Dispensa": { Negocio.IdiomaResxExtensao.PlacaMov_Usar_Copo_Dispensa = aux.Conteudo; break; }
                case "PlacaMov_Recipiente_Dosadora": { Negocio.IdiomaResxExtensao.PlacaMov_Recipiente_Dosadora = aux.Conteudo; break; }
                case "PlacaMov_Botao_Emergencia_Passos": { Negocio.IdiomaResxExtensao.PlacaMov_Botao_Emergencia_Passos = aux.Conteudo; break; }
                case "PlacaMov_Condicao_Incorreta": { Negocio.IdiomaResxExtensao.PlacaMov_Condicao_Incorreta = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_01": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_01 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_02": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_02 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_03": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_03 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_04": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_04 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_05": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_05 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_06": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_06 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_07": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_07 = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_Desconhecido": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_Desconhecido = aux.Conteudo; break; }
                case "PlacaMov_Cod_Erro_00": { Negocio.IdiomaResxExtensao.PlacaMov_Cod_Erro_00 = aux.Conteudo; break; }
                case "PainelControle_Erro_Posicao_PlacaMov": { Negocio.IdiomaResxExtensao.PainelControle_Erro_Posicao_PlacaMov = aux.Conteudo; break; }
                case "Manutencao_Fechar": { Negocio.IdiomaResxExtensao.Manutencao_Fechar = aux.Conteudo; break; }
                case "Manutencao_GB_Processos": { Negocio.IdiomaResxExtensao.Manutencao_GB_Processos = aux.Conteudo; break; }
                case "Manutencao_GB_Comandos": { Negocio.IdiomaResxExtensao.Manutencao_GB_Comandos = aux.Conteudo; break; }
                case "Manutencao_BT_Abrir_Gaveta": { Negocio.IdiomaResxExtensao.Manutencao_BT_Abrir_Gaveta = aux.Conteudo; break; }
                case "Manutencao_BT_Fechar_Gaveta": { Negocio.IdiomaResxExtensao.Manutencao_BT_Fechar_Gaveta = aux.Conteudo; break; }
                case "Manutencao_BT_Valvula_Dosagem": { Negocio.IdiomaResxExtensao.Manutencao_BT_Valvula_Dosagem = aux.Conteudo; break; }
                case "Manutencao_BT_Valvula_Recirculacao": { Negocio.IdiomaResxExtensao.Manutencao_BT_Valvula_Recirculacao = aux.Conteudo; break; }
                case "Manutencao_BT_Subir_Bico": { Negocio.IdiomaResxExtensao.Manutencao_BT_Subir_Bico = aux.Conteudo; break; }
                case "Manutencao_BT_Descer_Bico": { Negocio.IdiomaResxExtensao.Manutencao_BT_Descer_Bico = aux.Conteudo; break; }
                case "Manutencao_GB_Sensores": { Negocio.IdiomaResxExtensao.Manutencao_GB_Sensores = aux.Conteudo; break; }
                case "Manutencao_LBL_Copo": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Copo = aux.Conteudo; break; }
                case "Manutencao_LBL_Esponja": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Esponja = aux.Conteudo; break; }
                case "Manutencao_LBL_Alto_Bico": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Alto_Bico = aux.Conteudo; break; }
                case "Manutencao_LBL_Baixo_Bico": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Baixo_Bico = aux.Conteudo; break; }
                case "Manutencao_LBL_Emergencia": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Emergencia = aux.Conteudo; break; }
                case "Manutencao_LBL_Alerta": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Alerta = aux.Conteudo; break; }
                case "Manutencao_LBL_Gaveta_Aberta": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Gaveta_Aberta = aux.Conteudo; break; }
                case "Manutencao_LBL_Gaveta_Fechada": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Gaveta_Fechada = aux.Conteudo; break; }
                case "Manutencao_LBL_Valvula_Dosagem": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Valvula_Dosagem = aux.Conteudo; break; }
                case "Manutencao_LBL_Valvula_Recirculacao": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Valvula_Recirculacao = aux.Conteudo; break; }
                case "Manutencao_LBL_Cod_Erro": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Cod_Erro = aux.Conteudo; break; }
                case "Manutencao_LBL_Maquina_Ligada": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Maquina_Ligada = aux.Conteudo; break; }
                case "Manutencao_BT_Leitura_Sensores": { Negocio.IdiomaResxExtensao.Manutencao_BT_Leitura_Sensores = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Novo_Proc": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Novo_Proc = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Abrindo_Gav": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Abrindo_Gav = aux.Conteudo; break; }
                case "Manutencao_Show_Emergencia": { Negocio.IdiomaResxExtensao.Manutencao_Show_Emergencia = aux.Conteudo; break; }
                case "Manutencao_Show_Erro_Nativo": { Negocio.IdiomaResxExtensao.Manutencao_Show_Erro_Nativo = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Fechando_Gav": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Fechando_Gav = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Pos_Val_Dosagem": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Dosagem = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Pos_Val_Recirculacao": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Pos_Val_Recirculacao = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Descer_Bico": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Descer_Bico = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Subir_Bico": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Subir_Bico = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Executando_Comando": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Executando_Comando = aux.Conteudo; break; }
                case "Manutencao_LBL_Status_Aguarde_Comando": { Negocio.IdiomaResxExtensao.Manutencao_LBL_Status_Aguarde_Comando = aux.Conteudo; break; }
                case "Manutencao_Show_Alerta_Mensagem": { Negocio.IdiomaResxExtensao.Manutencao_Show_Alerta_Mensagem = aux.Conteudo; break; }
                case "Manutencao_Show_Maquina_Desligada": { Negocio.IdiomaResxExtensao.Manutencao_Show_Maquina_Desligada = aux.Conteudo; break; }
                case "Manutencao_BT_Cancel": { Negocio.IdiomaResxExtensao.Manutencao_BT_Cancel = aux.Conteudo; break; }
                case "Configuracoes_tabPlacaMov": { Negocio.IdiomaResxExtensao.Configuracoes_tabPlacaMov = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Addres": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Addres = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_NomeDispositivo": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_NomeDispositivo = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Motor": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Motor = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Tempo_Alerta": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Tempo_Alerta = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Tipo_Motor": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Tipo_Motor = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_BT_Atualizar": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_BT_Atualizar = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Velocidade": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Velocidade = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Aceleracao": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Aceleracao = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Pulsos": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Pulsos = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_LBL_Delay": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_LBL_Delay = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_Item_01_Motor": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_Item_01_Motor = aux.Conteudo; break; }
                case "Configuracoes_PlacaMov_Item_02_Motor": { Negocio.IdiomaResxExtensao.Configuracoes_PlacaMov_Item_02_Motor = aux.Conteudo; break; }
                case "Configuracoes_tabRecirculacaoAuto": { Negocio.IdiomaResxExtensao.Configuracoes_tabRecirculacaoAuto = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_Habilitar_Monit": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_Habilitar_Monit = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_LBL_Tempo_Monit": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Tempo_Monit = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_LBL_Tempo_Notif": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Tempo_Notif = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_LBL_Tentativas": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Tentativas = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_LBL_Circuito": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Circuito = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_LBL_Volume": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_Volume = aux.Conteudo; break; }
                case "PainelControle_Menu_LimpezaBico": { Negocio.IdiomaResxExtensao.PainelControle_Menu_LimpezaBico = aux.Conteudo; break; }
                case "LimpezaBico_lblTitulo": { Negocio.IdiomaResxExtensao.LimpezaBico_lblTitulo = aux.Conteudo; break; }
                case "LimpezaBico_lblStatus_Msg01": { Negocio.IdiomaResxExtensao.LimpezaBico_lblStatus_Msg01 = aux.Conteudo; break; }
                case "LimpezaBico_lblStatus_Msg02": { Negocio.IdiomaResxExtensao.LimpezaBico_lblStatus_Msg02 = aux.Conteudo; break; }
                case "LimpezaBico_lblSubStatus_Msg01": { Negocio.IdiomaResxExtensao.LimpezaBico_lblSubStatus_Msg01 = aux.Conteudo; break; }
                case "LimpezaBico_lblSubStatus_Msg02": { Negocio.IdiomaResxExtensao.LimpezaBico_lblSubStatus_Msg02 = aux.Conteudo; break; }
                case "TratarAlertaP3_lblTitulo": { Negocio.IdiomaResxExtensao.TratarAlertaP3_lblTitulo = aux.Conteudo; break; }
                case "TratarAlertaP3_lblSubStatus": { Negocio.IdiomaResxExtensao.TratarAlertaP3_lblSubStatus = aux.Conteudo; break; }
                case "TratarAlertaP3_Log_Concluido": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Concluido = aux.Conteudo; break; }
                case "TratarAlertaP3_Log_Iniciado": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Iniciado = aux.Conteudo; break; }
                case "TratarAlertaP3_Log_Cancelado": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Cancelado = aux.Conteudo; break; }
                case "TratarAlertaP3_Log_Abortado": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Abortado = aux.Conteudo; break; }
                case "TratarAlertaP3_Log_Falha": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Log_Falha = aux.Conteudo; break; }
                case "TratarAlertaP3_Cod_5": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_5 = aux.Conteudo; break; }
                case "TratarAlertaP3_Cod_6": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_6 = aux.Conteudo; break; }
                case "TratarAlertaP3_Cod_14": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_14 = aux.Conteudo; break; }
                case "TratarAlertaP3_Cod_15": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_15 = aux.Conteudo; break; }
                case "TratarAlertaP3_Cod_16": { Negocio.IdiomaResxExtensao.TratarAlertaP3_Cod_16 = aux.Conteudo; break; }
                case "FalhaPortaSerial": { Negocio.IdiomaResxExtensao.FalhaPortaSerial = aux.Conteudo; break; }
                case "Configuracoes_chkIdenditicacaoCopo": { Negocio.IdiomaResxExtensao.Configuracoes_chkIdenditicacaoCopo = aux.Conteudo; break; }
                case "Global_Maquina_Desligada": { Negocio.IdiomaResxExtensao.Global_Maquina_Desligada = aux.Conteudo; break; }
                case "Global_Maquina_Ligada": { Negocio.IdiomaResxExtensao.Global_Maquina_Ligada = aux.Conteudo; break; }
                case "ColoranteConfig_lblTitulo": { Negocio.IdiomaResxExtensao.ColoranteConfig_lblTitulo = aux.Conteudo; break; }
                case "Configuracoes_lblColoranteSeguidor": { Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteSeguidor = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadraoUDCP": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadraoUDCP = aux.Conteudo; break; }
                case "Configuracoes_rdbDatPadrao07": { Negocio.IdiomaResxExtensao.Configuracoes_rdbDatPadrao07 = aux.Conteudo; break; }
                case "frmTreinCal_btnAvancar": { Negocio.IdiomaResxExtensao.frmTreinCal_btnAvancar = aux.Conteudo; break; }
                case "frmTreinCal_btnRecuar": { Negocio.IdiomaResxExtensao.frmTreinCal_btnRecuar = aux.Conteudo; break; }
                case "frmTreinCal_btnSair": { Negocio.IdiomaResxExtensao.frmTreinCal_btnSair = aux.Conteudo; break; }
                case "frmTreinCal_lblTitulo": { Negocio.IdiomaResxExtensao.frmTreinCal_lblTitulo = aux.Conteudo; break; }
                case "Configuracoes_tabLimpBicos": { Negocio.IdiomaResxExtensao.Configuracoes_tabLimpBicos = aux.Conteudo; break; }
                case "Configuracoes_lblGeralDelayLimpBicos": { Negocio.IdiomaResxExtensao.Configuracoes_lblGeralDelayLimpBicos = aux.Conteudo; break; }
                case "Configuracoes_chkGeralHabLimpBicos": { Negocio.IdiomaResxExtensao.Configuracoes_chkGeralHabLimpBicos = aux.Conteudo; break; }
                case "Configuracoes_chkMonitToken": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitToken = aux.Conteudo; break; }
                case "Configuracoes_gbToken": { Negocio.IdiomaResxExtensao.Configuracoes_gbToken = aux.Conteudo; break; }
                case "Configuracoes_lblTokenIp": { Negocio.IdiomaResxExtensao.Configuracoes_lblTokenIp = aux.Conteudo; break; }
                case "Configuracoes_lblTokenPorta": { Negocio.IdiomaResxExtensao.Configuracoes_lblTokenPorta = aux.Conteudo; break; }
                case "ValidadeManutencao_OnLine_Cod1": { Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod1 = aux.Conteudo; break; }
                case "ValidadeManutencao_OnLine_Cod2": { Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod2 = aux.Conteudo; break; }
                case "ValidadeManutencao_OnLine_Cod3": { Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod3 = aux.Conteudo; break; }
                case "ValidadeManutencao_OnLine_Cod4": { Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Cod4 = aux.Conteudo; break; }
                case "ValidadeManutencao_OnLine_Fail_Connect": { Negocio.IdiomaResxExtensao.ValidadeManutencao_OnLine_Fail_Connect = aux.Conteudo; break; }
                case "Configuracoes_gbBkpCalibragem": { Negocio.IdiomaResxExtensao.Configuracoes_gbBkpCalibragem = aux.Conteudo; break; }
                case "Configuracoes_chkMonitBkpCalibragem": { Negocio.IdiomaResxExtensao.Configuracoes_chkMonitBkpCalibragem = aux.Conteudo; break; }
                case "Configuracoes_lblUniMedidaUnidadeAbastCont": { Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbastCont = aux.Conteudo; break; }
                case "Configuracoes_lblUniMedidaUnidadeAbastUnidade": { Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbastUnidade = aux.Conteudo; break; }
                case "Configuracoes_btnClearAbastecimento": { Negocio.IdiomaResxExtensao.Configuracoes_btnClearAbastecimento = aux.Conteudo; break; }
                case "Configuracoes_btnAtualizarAbastecimento": { Negocio.IdiomaResxExtensao.Configuracoes_btnAtualizarAbastecimento = aux.Conteudo; break; }
                case "Configuracoes_btnExcluirAbastecimento": { Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirAbastecimento = aux.Conteudo; break; }
                case "Configuracoes_btnNovoAbastecimento": { Negocio.IdiomaResxExtensao.Configuracoes_btnNovoAbastecimento = aux.Conteudo; break; }
                case "Configuracao_AbastecimentoPermExcluir": { Negocio.IdiomaResxExtensao.Configuracao_AbastecimentoPermExcluir = aux.Conteudo; break; }
                case "Abastecimento_NomeObrigatorio": { Negocio.IdiomaResxExtensao.Abastecimento_NomeObrigatorio = aux.Conteudo; break; }
                case "Abastecimento_ConteudoObrigatorio": { Negocio.IdiomaResxExtensao.Abastecimento_ConteudoObrigatorio = aux.Conteudo; break; }
                case "Configuracao_AbastecimentoErrorRemove": { Negocio.IdiomaResxExtensao.Configuracao_AbastecimentoErrorRemove = aux.Conteudo; break; }
                case "Configuracoes_lblUniMedidaUnidadeAbast": { Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbast = aux.Conteudo; break; }
                case "Configuracoes_lblUniMedidaUnidadeAbastCad": { Negocio.IdiomaResxExtensao.Configuracoes_lblUniMedidaUnidadeAbastCad = aux.Conteudo; break; }
                case "PainelControle_Menu_ConnectPlaca": { Negocio.IdiomaResxExtensao.PainelControle_Menu_ConnectPlaca = aux.Conteudo; break; }
                case "PainelControle_Menu_ConnectPlaca_On": { Negocio.IdiomaResxExtensao.PainelControle_Menu_ConnectPlaca_On = aux.Conteudo; break; }
                case "PainelControle_Menu_ConnectPlaca_Off": { Negocio.IdiomaResxExtensao.PainelControle_Menu_ConnectPlaca_Off = aux.Conteudo; break; }
                case "NameRemoteAccess": { Negocio.IdiomaResxExtensao.NameRemoteAccess = aux.Conteudo; break; }

                case "LimpBicosHorarioObrigatorio": { Negocio.IdiomaResxExtensao.LimpBicosHorarioObrigatorio = aux.Conteudo; break; }
                case "LimpBicosFormatoInvalido": { Negocio.IdiomaResxExtensao.LimpBicosFormatoInvalido = aux.Conteudo; break; }
                case "LimpBicosExcluirPeriodo": { Negocio.IdiomaResxExtensao.LimpBicosExcluirPeriodo = aux.Conteudo; break; }
                case "LimpBicosExcluir": { Negocio.IdiomaResxExtensao.LimpBicosExcluir = aux.Conteudo; break; }
                case "LimpBicosErrorExcluirPeriodo": { Negocio.IdiomaResxExtensao.LimpBicosErrorExcluirPeriodo = aux.Conteudo; break; }
                case "LimpBicosErrorAtualizarPeriodo": { Negocio.IdiomaResxExtensao.LimpBicosErrorAtualizarPeriodo = aux.Conteudo; break; }
                case "LimpBicosErrorInserirPeriodo": { Negocio.IdiomaResxExtensao.LimpBicosErrorInserirPeriodo = aux.Conteudo; break; }
                case "Configuracoes_lblLimpBicosPeriodosConfig": { Negocio.IdiomaResxExtensao.Configuracoes_lblLimpBicosPeriodosConfig = aux.Conteudo; break; }
                case "Configuracoes_lblLimpBicosCadConfig": { Negocio.IdiomaResxExtensao.Configuracoes_lblLimpBicosCadConfig = aux.Conteudo; break; }
                case "Configuracoes_lblLimpBicosPeriodo": { Negocio.IdiomaResxExtensao.Configuracoes_lblLimpBicosPeriodo = aux.Conteudo; break; }
                case "Configuracoes_btnClearLimpBicosConfig": { Negocio.IdiomaResxExtensao.Configuracoes_btnClearLimpBicosConfig = aux.Conteudo; break; }
                case "Configuracoes_btnAtualizarLimpBicosConfig": { Negocio.IdiomaResxExtensao.Configuracoes_btnAtualizarLimpBicosConfig = aux.Conteudo; break; }
                case "Configuracoes_btnNovoLimpBicosConfig": { Negocio.IdiomaResxExtensao.Configuracoes_btnNovoLimpBicosConfig = aux.Conteudo; break; }
                case "Configuracoes_btnExcluirLimpBicosConfig": { Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirLimpBicosConfig = aux.Conteudo; break; }

                case "Configuracoes_lblTipoLimpBicos": { Negocio.IdiomaResxExtensao.Configuracoes_lblTipoLimpBicos = aux.Conteudo; break; }
                case "Configuracoes_cbTipoLimpBicosIntervalo": { Negocio.IdiomaResxExtensao.Configuracoes_cbTipoLimpBicosIntervalo = aux.Conteudo; break; }
                case "Configuracoes_cbTipoLimpBicosConfig": { Negocio.IdiomaResxExtensao.Configuracoes_cbTipoLimpBicosConfig = aux.Conteudo; break; }


                case "Configuracoes_gbBaseDados": { Negocio.IdiomaResxExtensao.Configuracoes_gbBaseDados = aux.Conteudo; break; }
                case "Configuracoes_lblTipoBaseDados": { Negocio.IdiomaResxExtensao.Configuracoes_lblTipoBaseDados = aux.Conteudo; break; }
                case "Configuracoes_lblPathBaseDados": { Negocio.IdiomaResxExtensao.Configuracoes_lblPathBaseDados = aux.Conteudo; break; }
                case "Configuracoes_msgSalvarCalibracao":{ Negocio.IdiomaResxExtensao.Configuracoes_msgSalvarCalibracao = aux.Conteudo; break; }


                case "Configuracoes_lblTipoDosagemExec": { Negocio.IdiomaResxExtensao.Configuracoes_lblTipoDosagemExec = aux.Conteudo; break; }
                case "Configuracoes_TipoDosagemCircuito": { Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemCircuito = aux.Conteudo; break; }
                case "Configuracoes_TipoDosagemBase": { Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemBase = aux.Conteudo; break; }
                case "Configuracoes_TipoDosagemColorante": { Negocio.IdiomaResxExtensao.Configuracoes_TipoDosagemColorante = aux.Conteudo; break; }

                case "Configuracoes_Dat_Prefixo": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_Prefixo = aux.Conteudo; break; }
                case "Configuracoes_Dat_1Campo": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_1Campo = aux.Conteudo; break; }
                case "Configuracoes_Dat_2Campo": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_2Campo = aux.Conteudo; break; }
                case "Configuracoes_Dat_Virgula": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_Virgula = aux.Conteudo; break; }
                case "Configuracoes_Dat_Ponto": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_Ponto = aux.Conteudo; break; }
                case "Configuracoes_Dat_GBPadra05": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_GBPadra05 = aux.Conteudo; break; }
                case "Configuracoes_Dat_GBPadra06": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_GBPadra06 = aux.Conteudo; break; }
                case "Configuracoes_Dat_Habilitado": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_Habilitado = aux.Conteudo; break; }
                case "Configuracoes_Dat_Separador": { Negocio.IdiomaResxExtensao.Configuracoes_Dat_Separador = aux.Conteudo; break; }


                case "PainelControle_DesejaRecircularAuto": { Negocio.IdiomaResxExtensao.PainelControle_DesejaRecircularAuto = aux.Conteudo; break; }
                case "PainelControle_CancelouRecircularAuto": { Negocio.IdiomaResxExtensao.PainelControle_CancelouRecircularAuto = aux.Conteudo; break; }
                case "RecirculacaoAuto_Titulo": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Titulo = aux.Conteudo; break; }
                case "RecirculacaoAuto_SendoRealizada": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_SendoRealizada = aux.Conteudo; break; }
                case "RecirculacaoAuto_Iniciada": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Iniciada = aux.Conteudo; break; }
                case "RecirculacaoAuto_Concluido": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Concluido = aux.Conteudo; break; }
                case "RecirculacaoAuto_Cancelado": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Cancelado = aux.Conteudo; break; }
                case "RecirculacaoAuto_Abortada": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Abortada = aux.Conteudo; break; }
                case "RecirculacaoAuto_Falha": { Negocio.IdiomaResxExtensao.RecirculacaoAuto_Falha = aux.Conteudo; break; }


                case "Configuracoes_chkDesabilitarVolumeMinimoDat": { Negocio.IdiomaResxExtensao.Configuracoes_chkDesabilitarVolumeMinimoDat = aux.Conteudo; break; }
                case "Configuracoes_lblVolumeMinimoDat": { Negocio.IdiomaResxExtensao.Configuracoes_lblVolumeMinimoDat = aux.Conteudo; break; }
                case "Configuracoes_chkLogSerialMenu": { Negocio.IdiomaResxExtensao.Configuracoes_chkLogSerialMenu = aux.Conteudo; break; }
                case "Configuracoes_lblUsuarios": { Negocio.IdiomaResxExtensao.Configuracoes_lblUsuarios = aux.Conteudo; break; }
                case "Configuracoes_lblCadastroUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_lblCadastroUsuario = aux.Conteudo; break; }
                case "Configuracoes_lblNomeUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_lblNomeUsuario = aux.Conteudo; break; }
                case "Configuracoes_lblSenhaUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_lblSenhaUsuario = aux.Conteudo; break; }
                case "Configuracoes_lblTipoUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_lblTipoUsuario = aux.Conteudo; break; }
                case "Configuracoes_btnAtualizarUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_btnAtualizarUsuario = aux.Conteudo; break; }
                case "Configuracoes_btnExcluirUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_btnExcluirUsuario = aux.Conteudo; break; }
                case "Configuracoes_btnNovoUsuario": { Negocio.IdiomaResxExtensao.Configuracoes_btnNovoUsuario = aux.Conteudo; break; }
                case "Configuracoes_btnClearUsuarios": { Negocio.IdiomaResxExtensao.Configuracoes_btnClearUsuarios = aux.Conteudo; break; }
                case "Pesagem_btnTutorial": { Negocio.IdiomaResxExtensao.Pesagem_btnTutorial = aux.Conteudo; break; }
                case "Global_UnidadeMedida_Abreviacao_Fraction":{ Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Fraction = aux.Conteudo; break; }
                case "Configuracoes_lblColoranteCor": { Negocio.IdiomaResxExtensao.Configuracoes_lblColoranteCor = aux.Conteudo; break; }

                case "Configuracoes_lblRecValula": { Negocio.IdiomaResxExtensao.Configuracoes_lblRecValula = aux.Conteudo; break; }
                case "Configuracoes_lblRecAutomatico": { Negocio.IdiomaResxExtensao.Configuracoes_lblRecAutomatico = aux.Conteudo; break; }
                case "Configuracoes_RecirculacaoAuto_LBL_TempoUn": { Negocio.IdiomaResxExtensao.Configuracoes_RecirculacaoAuto_LBL_TempoUn = aux.Conteudo; break; }

                case "ProdutoBicoIndividual": { Negocio.IdiomaResxExtensao.ProdutoBicoIndividual = aux.Conteudo; break; }
                case "ProdutoNivelBicoIndividual": { Negocio.IdiomaResxExtensao.ProdutoNivelBicoIndividual = aux.Conteudo; break; }
                case "Configuracao_LogStatusMaquina": { Negocio.IdiomaResxExtensao.Configuracao_LogStatusMaquina = aux.Conteudo; break; }

                default:
                    {
                        break;
                    }

            }
        }

        //public string ProdutoBicoIndividual { get; set; } = string.Empty;
        //public string ProdutoNivelBicoIndividual { get; set; } = string.Empty;

        private static void SetConteudo(List<ObjectMensagem> laux)
        {
            foreach (ObjectMensagem aux in laux)
            {
                SetConteudoObj(aux);              

            }
        }

        public static void Persist(List<ObjectMensagem> laux, bool forceMessage = false)
        {
            int count = 0;
            List<ObjectMensagem> objList = List();
            foreach (ObjectMensagem aux in laux)
            {
                ObjectMensagem objM = objList.Find(o => o.Nome == aux.Nome);
                //Update
                if(objM != null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("UPDATE Mensagem SET "); // (Motor, Nome, MassaEspecifica, Habilitado, Volume, Correspondencia, Dispositivo, NivelMinimo, NivelMaximo) VALUES (");
                            sb.Append("Nome = '" + aux.Nome + "', ");
                            sb.Append("Conteudo = '" + aux.Conteudo + "', ");
                            sb.Append("IdIdioma = '" + aux.IdIdioma + "' ");
                            sb.Append(" WHERE Id = '" + objM.Id.ToString() + "';");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }

                }
                //Insert
                else
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("INSERT INTO Mensagem (Nome, Conteudo, IdIdioma) VALUES (");
                            sb.Append("'" + aux.Nome + "', ");
                            sb.Append("'" + aux.Conteudo + "', ");
                            sb.Append("'" + aux.IdIdioma + "' ");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }

                }
                if (forceMessage)
                {
                    if (count > 10)
                    {
                        count = 0;
                        System.Windows.Forms.Application.DoEvents();
                        Thread.Sleep(200);
                        System.Windows.Forms.Application.DoEvents();
                        
                    }
                    count++;
                }
            }
        }
    }
}