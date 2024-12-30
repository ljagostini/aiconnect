using System;
using Microsoft.Extensions.Configuration;
using Percolore.Core;
using Percolore.Core.Logging;
using Percolore.Core.Persistence.WindowsRegistry;
using Percolore.Core.Persistence.Xml;

namespace Percolore.IOConnect
{
    class InicializarBancoDeDados
    {
        public static void Main()
        {
            #region verificando bancos de dados obrigatórios e com conversão XML disponível

            /* Arquivos obrigatórios para primeira execução:
             * Parametros, Colorantes, Calibragem
             * 
             * Arquivos opcionais com conversão disponível:
             * Formulas, Recircular
             * 
             * Arquivos opcionais não convertidos:
             * Eventos, MotorPlacaMovimentacao, BasDat05, BasDat06, Users, CalibragemAuto, CalibragemAutoHist, RecircularAuto, PlMov
             * 
             * Arquivos não utilizados:
             * Abastecimento
             * 
             * TODO: Implementar conversão dos arquivos opcionais
             */

            // Parametros
            if (File.Exists(Parametros.PathFile) && File.Exists(Util.ObjectParametros.PathFile))
            {
                File.Delete(Util.ObjectParametros.PathFile);
                Thread.Sleep(2000);
            }
            else if (!File.Exists(Parametros.PathFile) && !File.Exists(Util.ObjectParametros.PathFile))
            {
                LogManager.LogWarning($"Um dos bancos de dados obrigatórios não foi encontrado: {Util.ObjectParametros.PathFile}");
                return;
            }

            // Colorantes
            if (File.Exists(Colorante.PathFile) && File.Exists(Util.ObjectColorante.PathFile))
            {
                File.Delete(Util.ObjectColorante.PathFile);
                Thread.Sleep(2000);
            }
            else if (!File.Exists(Colorante.PathFile) && !File.Exists(Util.ObjectColorante.PathFile))
            {
                LogManager.LogWarning($"Um dos bancos de dados obrigatórios não foi encontrado: {Util.ObjectColorante.PathFile}");
                return;
            }

            // Calibragem
            if (File.Exists(Calibragem.PathFile) && File.Exists(Util.ObjectCalibragem.PathFile))
            {
                File.Delete(Util.ObjectCalibragem.PathFile);
                Thread.Sleep(2000);
            }
            else if (!File.Exists(Calibragem.PathFile) && !File.Exists(Util.ObjectCalibragem.PathFile))
            {
                LogManager.LogWarning($"Um dos bancos de dados obrigatórios não foi encontrado: {Util.ObjectCalibragem.PathFile}");
                return;
            }

            // Formulas
            if (File.Exists(Formula.PathFile) && File.Exists(Util.ObjectFormula.PathFile))
            {
                File.Delete(Util.ObjectFormula.PathFile);
                Thread.Sleep(2000);
            }

            // Recircular
            if (File.Exists(Recircular.PathFile) && File.Exists(Util.ObjectRecircular.PathFile))
            {
                File.Delete(Util.ObjectRecircular.PathFile);
                Thread.Sleep(2000);
            }
            #endregion

            #region verificar integridade dos bancos de dados SQLite obrigatórios

            // Parametros
            if (File.Exists(Util.ObjectParametros.PathFile))
            {
                try { Util.ObjectParametros.Load(); }
                catch (Exception e) { LogManager.LogError($"Erro ao carregar o banco de dados {Util.ObjectParametros.PathFile}: ", e); return; }
            }

            // Colorantes
            if (File.Exists(Util.ObjectColorante.PathFile))
            {
                try { Util.ObjectColorante.List(); }
                catch (Exception e) { LogManager.LogError($"Erro ao carregar o banco de dados {Util.ObjectColorante.PathFile}: ", e); return; }
            }

            // Calibragem
            if (File.Exists(Util.ObjectCalibragem.PathFile))
            {
                try { Util.ObjectCalibragem.List(); }
                catch (Exception e) { LogManager.LogError($"Erro ao carregar o banco de dados {Util.ObjectCalibragem.PathFile}: ", e); return; }
            }
            #endregion

            #region métodos de conversão do banco de dados XML antigo para SQLite

            // Conversão do arquivo de parâmetros
            if (!File.Exists(Util.ObjectParametros.PathFile))
            {
                Parametros parametrosXML = Parametros.Load();

                Util.ObjectParametros.CreateBD();
                Util.ObjectParametros parametrosDB = new Util.ObjectParametros();

                // loop para copiar os valores do XML para o banco de dados
                foreach (var item in parametrosXML.GetType().GetProperties())
                {
                    if (parametrosDB.GetType().GetProperty(item.Name).GetSetMethod() != null)
                    {
                        parametrosDB.GetType().GetProperty(item.Name).SetValue(parametrosDB, item.GetValue(parametrosXML));
                    }
                    else
                    {
                        LogManager.LogWarning($"Não foi possível converter a propriedade '{item.Name}' a partir do XML.");
                    }
                }

                Util.ObjectParametros.PersistInsert(parametrosDB);

                Util.ObjectParametros.InitLoad();

                // removendo o arquivo XML
                if (File.Exists(Parametros.PathFile))
                {
                    File.Delete(Parametros.PathFile);
                    Thread.Sleep(2000);
                }
            }

            // Conversão do arquivo de colorantes
            if (!File.Exists(Util.ObjectColorante.PathFile))
            {
                List<Colorante> colorantesXML = Colorante.List();

                Util.ObjectColorante.CreateBD();
                List<Util.ObjectColorante> colorantesDB = new List<Util.ObjectColorante>();

                foreach (Colorante unitColoranteXML in colorantesXML)
                {
                    Util.ObjectColorante unitColoranteDB = new Util.ObjectColorante();
                    unitColoranteDB.Circuito = unitColoranteXML.Circuito;
                    unitColoranteDB.Correspondencia = unitColoranteXML.Correspondencia;
                    unitColoranteDB.Dispositivo = unitColoranteXML.Dispositivo;
                    unitColoranteDB.Habilitado = unitColoranteXML.Habilitado;
                    unitColoranteDB.MassaEspecifica = unitColoranteXML.MassaEspecifica;
                    unitColoranteDB.NivelMaximo = unitColoranteXML.NivelMaximo;
                    unitColoranteDB.NivelMinimo = unitColoranteXML.NivelMinimo;
                    unitColoranteDB.Nome = unitColoranteXML.Nome;
                    unitColoranteDB.Volume = unitColoranteXML.Volume;
                    unitColoranteDB.IsBase = unitColoranteXML.IsBase;
                    unitColoranteDB.Seguidor = unitColoranteXML.Seguidor;
                    unitColoranteDB.IsBicoIndividual = unitColoranteXML.IsBicoIndividual;
                    unitColoranteDB.VolumePurga = unitColoranteXML.VolumePurga;
                    unitColoranteDB.VolumeBicoIndividual = unitColoranteXML.VolumeBicoIndividual;

                    colorantesDB.Add(unitColoranteDB);
                }

                Util.ObjectParametros parametrosDB = new Util.ObjectParametros();
                Util.ObjectParametros.InitLoad();
                parametrosDB = Util.ObjectParametros.Load();

                // completar com dados padrão, caso não informados no XML
                if (parametrosDB != null)
                {
                    for (int i = colorantesXML.Count + 1; i <= 32; i++)
                    {
                        Util.ObjectColorante unitColoranteDB = new Util.ObjectColorante();
                        unitColoranteDB.Circuito = i;
                        unitColoranteDB.Correspondencia = i;
                        unitColoranteDB.Dispositivo = 2;
                        unitColoranteDB.Habilitado = false;
                        unitColoranteDB.MassaEspecifica = 0;
                        unitColoranteDB.NivelMaximo = parametrosDB.VolumeMaximo;
                        unitColoranteDB.NivelMinimo = parametrosDB.VolumeMinimo;
                        unitColoranteDB.Nome = "";
                        unitColoranteDB.Volume = 0;
                        unitColoranteDB.IsBase = false;
                        unitColoranteDB.Seguidor = -1;
                        unitColoranteDB.IsBicoIndividual = false;
                        unitColoranteDB.VolumeBicoIndividual = 0.0;
                        colorantesDB.Add(unitColoranteDB);
                    }

                    Util.ObjectColorante.Persist(colorantesDB);
                }
                else
                    LogManager.LogInformation("Não foi possível carregar os parâmetros do sistema.");

                // removendo o arquivo XML
                if (File.Exists(Colorante.PathFile))
                {
                    File.Delete(Colorante.PathFile);
                    Thread.Sleep(2000);
                }
            }

            // Conversão do arquivo de calibrações
            if (!File.Exists(Util.ObjectCalibragem.PathFile))
            {
                Util.ObjectCalibragem.CreateBD();
                Util.ObjectCalibracaoAutomatica.CreateBD();

                Util.ObjectCalibracaoAutomatica calibragemAutoDB = new Util.ObjectCalibracaoAutomatica();

                calibragemAutoDB.CapacideMaxBalanca = 420;
                calibragemAutoDB.MaxMassaAdmRecipiente = 100;
                calibragemAutoDB.NumeroMaxTentativaRec = 3;
                calibragemAutoDB.VolumeMaxRecipiente = 200;
                calibragemAutoDB._calibragem = new Util.ObjectCalibragem();
                calibragemAutoDB._calibragem.Motor = 1;
                calibragemAutoDB.listOperacaoAutomatica = new List<Negocio.OperacaoAutomatica>();
                Util.ObjectCalibracaoAutomatica.Add(calibragemAutoDB, true);

                // lendo e reparando arquivo XML
                List<Calibragem> calibragemXML = new List<Calibragem>();
                for (int i = 1; i <= 32; i++)
                {
                    try
                    {
                        Calibragem unitCalibragemXML = Calibragem.Load(i);
                        calibragemXML.Add(unitCalibragemXML);
                    }
                    catch
                    {
                        LogManager.LogInformation($"Calibração não encontrada para o motor '{i}'. Carregando última calibração válida.");

                        if (i > 16)
                        {
                            try
                            {
                                Calibragem unitCalibragemXML = Calibragem.Load(i - 16);
                                unitCalibragemXML.Motor = i;
                                calibragemXML.Add(unitCalibragemXML);
                            }
                            catch (Exception ex)
                            {
                                LogManager.LogError($"Não foi possível carregar calibração válida para o motor {i}.", ex);
                            }
                        }
                    }
                }

                // convertendo para o formato do banco de dados
                foreach (Calibragem unitCalibragemXML in calibragemXML)
                {
                    Util.ObjectCalibragem unitCalibragemDB = new Util.ObjectCalibragem();
                    unitCalibragemDB.Motor = unitCalibragemXML.Motor;
                    unitCalibragemDB.UltimoPulsoReverso = unitCalibragemXML.UltimoPulsoReverso;
                    unitCalibragemDB.Valores = new List<ValoresVO>();
                    unitCalibragemDB.MinimoFaixas = unitCalibragemXML.MinimoFaixas;

                    foreach (ValoresVO calibragemValoresXML in unitCalibragemXML.Valores)
                    {
                        ValoresVO calibragemValoresDB = new ValoresVO();

                        calibragemValoresDB.Volume = calibragemValoresXML.Volume;
                        calibragemValoresDB.PulsoHorario = calibragemValoresXML.PulsoHorario;
                        calibragemValoresDB.Velocidade = calibragemValoresXML.Velocidade;

                        if (calibragemValoresXML.Aceleracao == 0) calibragemValoresDB.Aceleracao = calibragemValoresDB.Velocidade;
                        else calibragemValoresDB.Aceleracao = calibragemValoresXML.Aceleracao;

                        calibragemValoresDB.Delay = calibragemValoresXML.Delay;

                        if (calibragemValoresXML.PulsoReverso == 0) calibragemValoresDB.PulsoReverso = 50;
                        else calibragemValoresDB.PulsoReverso = calibragemValoresXML.PulsoReverso;

                        calibragemValoresDB.MassaMedia = calibragemValoresXML.MassaMedia;
                        calibragemValoresDB.DesvioMedio = calibragemValoresXML.DesvioMedio;

                        unitCalibragemDB.Valores.Add(calibragemValoresDB);
                    }
                    Util.ObjectCalibragem.Add(unitCalibragemDB);
                }

                // removendo o arquivo XML
                if (File.Exists(Calibragem.PathFile))
                {
                    File.Delete(Calibragem.PathFile);
                    Thread.Sleep(2000);
                }
            }

            // Conversão do arquivo de fórmulas
            if (!File.Exists(Util.ObjectFormula.PathFile))
            {
                List<Formula> formulasXML = null;
                try
                {
                    formulasXML = Formula.List();
                }
                catch (Exception)
                {
                    formulasXML = new List<Formula>();
                }

                Util.ObjectFormula.CreateBD();

                foreach (Formula unitFormulaXML in formulasXML)
                {
                    Util.ObjectFormula formulasDB = new Util.ObjectFormula();
                    formulasDB.Itens = new List<Util.ObjectFormulaItem>();
                    formulasDB.Nome = unitFormulaXML.Nome;
                    foreach (FormulaItem formulaItemXML in unitFormulaXML.Itens)
                    {
                        Util.ObjectFormulaItem formulaItemDB = new Util.ObjectFormulaItem();
                        formulaItemDB.IdColorante = formulaItemXML.IdColorante;
                        formulaItemDB.Mililitros = formulaItemXML.Mililitros;
                        formulasDB.Itens.Add(formulaItemDB);
                    }
                    Util.ObjectFormula.Persist(formulasDB);
                }

                // removendo o arquivo XML
                if (File.Exists(Formula.PathFile))
                {
                    File.Delete(Formula.PathFile);
                    Thread.Sleep(2000);
                }
            }

            // Criação do arquivo de configurações de recirculação
            if (!File.Exists(Util.ObjectRecircular.PathFile))
            {
                Util.ObjectRecircular.CreateBD();
                if (File.Exists(Util.ObjectRecircular.PathFile))
                {
                    List<Util.ObjectRecircular> recircularDB = new List<Util.ObjectRecircular>();

                    // Lendo dados do XML
                    List<Recircular> recircularXML = Recircular.List();

                    foreach (Recircular unitRecircularXML in recircularXML)
                    {
                        Util.ObjectRecircular unitRecircularDB = new Util.ObjectRecircular();
                        unitRecircularDB.Circuito = unitRecircularXML.Circuito;
                        unitRecircularDB.Dias = unitRecircularXML.Dias;
                        unitRecircularDB.Habilitado = unitRecircularXML.Habilitado;
                        unitRecircularDB.VolumeDin = unitRecircularXML.VolumeDin;
                        unitRecircularDB.VolumeRecircular = unitRecircularXML.VolumeRecircular;
                        unitRecircularDB.VolumeDosado = unitRecircularXML.VolumeDosado;
                        unitRecircularDB.DtInicio = unitRecircularXML.DtInicio;
                        recircularDB.Add(unitRecircularDB);
                    }

                    Util.ObjectRecircular.Persist(recircularDB);
                }

                // removendo o arquivo XML
                if (File.Exists(Recircular.PathFile))
                {
                    File.Delete(Recircular.PathFile);
                    Thread.Sleep(2000);
                }
            }

            #endregion

            #region métodos de criação de outros bancos de dados SQLite

            // Criação do arquivo de configurações de usuário
            if (!File.Exists(Util.ObjectUser.PathFile))
            {
                Util.ObjectUser.CreateBD();

                Util.ObjectUser userInit = new Util.ObjectUser();

                userInit.Nome = "master";
                userInit.Senha = "1500k";
                userInit.Tecnico = false;
                userInit.Tipo = 1;

                Util.ObjectUser.Persist(userInit);

                Util.ObjectUser userTecnico = new Util.ObjectUser();

                userTecnico.Nome = "tec";
                userTecnico.Senha = "1234";
                userTecnico.Tecnico = true;
                userTecnico.Tipo = 2;
                Util.ObjectUser.Persist(userTecnico);

                Util.ObjectUser userMan = new Util.ObjectUser();

                userMan.Nome = "manager";
                userMan.Senha = "avsb";
                userMan.Tecnico = false;
                userMan.Tipo = 3;

                Util.ObjectUser.Persist(userMan);

            }

            // Criação do arquivo BasDat06.db
            if (!File.Exists(Util.ObjectBasDat06.PathFile))
            {
                Util.ObjectBasDat06.CreateBD();

                Util.ObjectBasDat06 Accent = new Util.ObjectBasDat06();
                Accent.Name = "ACCENT";
                Accent.Circuito = 14;
                Accent.Volume = 0.92;

                Util.ObjectBasDat06.Persist(Accent);

                Util.ObjectBasDat06 Neutral = new Util.ObjectBasDat06();
                Neutral.Name = "NEUTRAL";
                Neutral.Circuito = 15;
                Neutral.Volume = 0.82;

                Util.ObjectBasDat06.Persist(Neutral);

                Util.ObjectBasDat06 White_ = new Util.ObjectBasDat06();
                White_.Name = "WHITE";
                White_.Circuito = 16;
                White_.Volume = 0.935;

                Util.ObjectBasDat06.Persist(White_);

            }

            // Criação do arquivo PlMov.db
            if (!File.Exists(Util.ObjectMotorPlacaMovimentacao.PathFile))
            {
                Util.ObjectMotorPlacaMovimentacao.CreateBD();
                if (File.Exists(Util.ObjectMotorPlacaMovimentacao.PathFile))
                {
                    List<Util.ObjectMotorPlacaMovimentacao> lPlaMov = new List<Util.ObjectMotorPlacaMovimentacao>();
                    Util.ObjectMotorPlacaMovimentacao plMov = new Util.ObjectMotorPlacaMovimentacao();
                    plMov.Circuito = 1;
                    plMov.NameTag = "Gaveta";
                    plMov.TipoMotor = 0;
                    plMov.Pulsos = 100;
                    plMov.Aceleracao = 100;
                    plMov.Delay = 100;
                    plMov.Habilitado = true;
                    plMov.Velocidade = 100;
                    lPlaMov.Add(plMov);

                    plMov = new Util.ObjectMotorPlacaMovimentacao();
                    plMov.Circuito = 2;
                    plMov.NameTag = "Válvula";
                    plMov.TipoMotor = 0;
                    plMov.Pulsos = 100;
                    plMov.Aceleracao = 100;
                    plMov.Delay = 100;
                    plMov.Habilitado = true;
                    plMov.Velocidade = 100;
                    lPlaMov.Add(plMov);

                    plMov = new Util.ObjectMotorPlacaMovimentacao();
                    plMov.Circuito = 3;
                    plMov.NameTag = "Bico";
                    plMov.TipoMotor = 0;
                    plMov.Pulsos = 100;
                    plMov.Aceleracao = 100;
                    plMov.Delay = 100;
                    plMov.Habilitado = true;
                    plMov.Velocidade = 100;
                    lPlaMov.Add(plMov);

                    Util.ObjectMotorPlacaMovimentacao.Persist(lPlaMov);

                }
            }

            // Criação do arquivo BasDat05.db
            if (!File.Exists(Util.ObjectBasDat05.PathFile))
            {
                Util.ObjectBasDat05.CreateBD();

                Util.ObjectBasDat05 Accent = new Util.ObjectBasDat05();
                Accent.Name = "ACCENT";
                Accent.Circuito = 14;
                Accent.Volume = 0.92;

                Util.ObjectBasDat05.Persist(Accent);

                Util.ObjectBasDat05 Neutral = new Util.ObjectBasDat05();
                Neutral.Name = "NEUTRAL";
                Neutral.Circuito = 15;
                Neutral.Volume = 0.82;

                Util.ObjectBasDat05.Persist(Neutral);

                Util.ObjectBasDat05 White_ = new Util.ObjectBasDat05();
                White_.Name = "WHITE";
                White_.Circuito = 16;
                White_.Volume = 0.935;

                Util.ObjectBasDat05.Persist(White_);
            }

            // Criação do arquivo Abastecimento.db
            if (!File.Exists(Util.ObjectAbastecimento.PathFile))
            {
                Util.ObjectAbastecimento.CreateBD();
            }

            // Criação do arquivo Eventos.db
            if (!File.Exists(Util.ObjectEventos.PathFile))
            {
                Util.ObjectEventos.CreateBD();
            }

            #endregion
        }
    }
}