using Percolore.Core;
using Percolore.Core.Persistence.WindowsRegistry;
using System.Data.SQLite;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectCalibragem
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Calibragem.db");
        public static readonly string FileName = Path.GetFileName(PathFile);
        public static string zipPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "bkp" + Path.DirectorySeparatorChar;

        public int Motor { get; set; }
        public int UltimoPulsoReverso { get; set; }
        public List<ValoresVO> Valores { get; set; }
        public int MinimoFaixas = 5;

        #region Métodos
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
                    sb.Append("CREATE TABLE IF NOT EXISTS [Calibragem] (Motor TEXT NULL, UltimoPulsoReverso TEXT NULL, MinimoFaixas TEXT NULL);");
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
                    sb = new StringBuilder();
                    sb.Append(" CREATE TABLE IF NOT EXISTS [Valores] (Motor TEXT NULL, Volume TEXT NULL, Pulsos TEXT NULL, Velocidade TEXT NULL, ReverseDelay TEXT NULL, ");
                    sb.Append(" MassaMedia TEXT NULL, DesvioMedio TEXT NULL, PulsoReverso TEXT NULL, Aceleracao TEXT NULL);");
                    createQuery = sb.ToString();
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

        public static ObjectCalibragem Load(int motor)
        {
            ObjectCalibragem c = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Calibragem WHERE Motor = '" + motor.ToString() + "';";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            c = new ObjectCalibragem();
                            c.Motor = int.Parse(reader["Motor"].ToString());
                            c.UltimoPulsoReverso = int.Parse(reader["UltimoPulsoReverso"].ToString());
                            c.MinimoFaixas = int.Parse(reader["MinimoFaixas"].ToString());
                            c.Valores = new List<ValoresVO>();
                            break;
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            if(c != null)
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Valores WHERE Motor = '" + motor.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ValoresVO valores = new ValoresVO();

                                double Volume = 0;
                                int PulsoHorario = 0;
                                int Velocidade = 0;
                                int Delay = 0;
                                double MassaMedia = 0;
                                double DesvioMedio = 0;
                                int PulsoReverso = 0;
                                int Aceleracao = 0;

                                PulsoHorario = int.Parse(reader["Pulsos"].ToString());
                                Velocidade = int.Parse(reader["Velocidade"].ToString());
                                Delay = int.Parse(reader["ReverseDelay"].ToString());
                                Volume = double.Parse(reader["Volume"].ToString(),  CultureInfo.InvariantCulture);
                                MassaMedia = double.Parse(reader["MassaMedia"].ToString(), CultureInfo.InvariantCulture);
                                DesvioMedio = double.Parse(reader["DesvioMedio"].ToString(), CultureInfo.InvariantCulture);
                                PulsoReverso = int.Parse(reader["PulsoReverso"].ToString());
                                Aceleracao = int.Parse(reader["Aceleracao"].ToString());
                                    
                                valores.Volume = Volume;
                                valores.PulsoHorario = PulsoHorario;
                                valores.Velocidade = Velocidade;
                                valores.Delay = Delay;
                                valores.MassaMedia = MassaMedia;
                                valores.DesvioMedio = DesvioMedio;
                                valores.PulsoReverso = PulsoReverso;
                                valores.Aceleracao = Aceleracao;
                                c.Valores.Add(valores);
                            }
                            reader.Close();
                        }
                    }
                    conn.Close();
                }
            }

            return c;
        }

        public static List<ObjectCalibragem> List()
        {
            List<ObjectCalibragem> retorno = new List<ObjectCalibragem>();
            ObjectCalibragem c = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Calibragem;";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            c = new ObjectCalibragem();
                            c.Motor = int.Parse(reader["Motor"].ToString());
                            c.UltimoPulsoReverso = int.Parse(reader["UltimoPulsoReverso"].ToString());
                            c.MinimoFaixas = int.Parse(reader["MinimoFaixas"].ToString());
                            c.Valores = new List<ValoresVO>();
                            retorno.Add(c);                                
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            if (retorno.Count > 0)
            {
                foreach (ObjectCalibragem objcal in retorno)
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "SELECT * FROM Valores WHERE Motor = '" + objcal.Motor.ToString() + "';";

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ValoresVO valores = new ValoresVO();

                                    double Volume = 0;
                                    int PulsoHorario = 0;
                                    int Velocidade = 0;
                                    int Delay = 0;
                                    double MassaMedia = 0;
                                    double DesvioMedio = 0;
                                    int PulsoReverso = 0;
                                    int Aceleracao = 0;

                                    PulsoHorario = int.Parse(reader["Pulsos"].ToString());
                                    Velocidade = int.Parse(reader["Velocidade"].ToString());
                                    Delay = int.Parse(reader["ReverseDelay"].ToString());
                                    Volume = double.Parse(reader["Volume"].ToString(), CultureInfo.InvariantCulture);
                                    MassaMedia = double.Parse(reader["MassaMedia"].ToString(), CultureInfo.InvariantCulture);
                                    DesvioMedio = double.Parse(reader["DesvioMedio"].ToString(), CultureInfo.InvariantCulture);
                                    PulsoReverso = int.Parse(reader["PulsoReverso"].ToString());
                                    Aceleracao = int.Parse(reader["Aceleracao"].ToString());

                                    valores.Volume = Volume;
                                    valores.PulsoHorario = PulsoHorario;
                                    valores.Velocidade = Velocidade;
                                    valores.Delay = Delay;
                                    valores.MassaMedia = MassaMedia;
                                    valores.DesvioMedio = DesvioMedio;
                                    valores.PulsoReverso = PulsoReverso;
                                    valores.Aceleracao = Aceleracao;
                                    objcal.Valores.Add(valores);
                                }
                                reader.Close();
                            }
                        }
                        conn.Close();
                    }
                }
            }

            return retorno.OrderBy(o=>o.Motor).ToList();
        }

        public static bool Validate(ObjectCalibragem c, out string outMsg)
        {
            #region Valida parâmetros

            if (c == null)
                throw new ArgumentNullException();

            if (c.Valores == null)
                throw new ArgumentNullException("");

            if (c.Valores.Count == 0)
            {
                outMsg = Negocio.IdiomaResxExtensao.Calibragem_NaoContemValor;
                return false;
            }

            #endregion

            StringBuilder validacoes = new StringBuilder();
            StringBuilder validacao = new StringBuilder();

            for (int i = 0; i < c.Valores.Count; i++)
            {
                validacao.Clear();
                ValoresVO valores = c.Valores[i];

                //Pulsos
                if (valores.PulsoHorario == 0)
                    validacao.AppendLine(Negocio.IdiomaResxExtensao.Calibragem_QuantidadePulsosMaiorZero);

                //Velocidade
                if (valores.Velocidade < 0)
                {
                    validacao.AppendLine(Negocio.IdiomaResxExtensao.Calibragem_VelocidadeMaiorZero);
                }
                else
                {
                    /*
                    //[A velocidade não pode ser maior que a velocidade para o volume 
                    //na posição imediatamente acima]
                    if (i > 0)
                    {
                        int limite = c.Valores[i - 1].Velocidade;
                        if (valores.Velocidade > limite)
                        {
                            string texto =
                                string.Format(Properties.UI.Calibragem_VelocidadeMenorIgual, limite.ToString());
                            validacao.AppendLine(texto);
                        }
                    }
                    */

                }

                //Delay
                if (valores.Delay < 0)
                {
                    validacao.AppendLine(Negocio.IdiomaResxExtensao.Calibragem_DelayMaiorZero);
                }
                else
                {
                    /*
                    //[A velocidade não pode ser maior que a velocidade para o volume 
                    //na posição imediatamente acima]
                    if (i > 0)
                    {
                        int limite = c.Valores[i - 1].Delay;
                        if (valores.Delay > limite)
                        {
                            string texto =
                                string.Format(Properties.UI.Calibragem_DelayMenorIgual, limite.ToString());
                            validacao.AppendLine(texto);
                        }
                    }
                    */
                }

                if (validacao.Length > 0)
                {
                    string MililitroAbrev = Negocio.IdiomaResxExtensao.Global_UnidadeMedida_Abreviacao_Mililitro;
                    validacoes.AppendLine("[" + valores.Volume.ToString() + MililitroAbrev);
                    validacoes.Append(validacao.ToString());
                    validacoes.AppendLine();
                }
            }

            outMsg = validacoes.ToString();
            return (validacoes.Length == 0);
        }

        public static void Add(ObjectCalibragem c)
        {
            if( c!= null)
            {
                ObjectCalibragem cN = Load(c.Motor);
                if(cN == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("INSERT INTO Calibragem (Motor, UltimoPulsoReverso, MinimoFaixas) VALUES (");
                            sb.Append("'" + c.Motor.ToString() + "', ");
                            sb.Append("'" + c.UltimoPulsoReverso.ToString() + "', ");
                            sb.Append("'" + c.MinimoFaixas.ToString() + "' ");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                    foreach(ValoresVO val in c.Valores)
                    {
                        InsertValores(c.Motor, val);
                        /*
                        sb = new StringBuilder();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {
                                conn.Open();
                                sb.Append("INSERT INTO Valores (Motor, Volume, Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao) VALUES (");
                                sb.Append("'" + c.Motor.ToString() + "', ");
                                sb.Append("'" + val.Volume.ToString().Replace(",",".") + "', ");
                                sb.Append("'" + val.PulsoHorario.ToString() + "', ");
                                sb.Append("'" + val.Velocidade.ToString() + "', ");
                                sb.Append("'" + val.Delay.ToString() + "', ");
                                sb.Append("'" + val.MassaMedia.ToString().Replace(",", ".") + "', ");
                                sb.Append("'" + val.DesvioMedio.ToString().Replace(",", ".") + "', ");                                   
                                sb.Append("'" + val.PulsoReverso.ToString() + "', ");
                                sb.Append("'" + val.Aceleracao.ToString() + "' ");
                                sb.Append(");");

                                cmd.CommandText = sb.ToString();

                                cmd.ExecuteNonQuery();

                                conn.Close();
                            }
                        }
                        */
                    }

                    gerarEventoAlterarCalibracao(0, "Circuito inserido:" + c.Motor.ToString());
                }
                else
                {
                    bool jaFezbkp = false;
                    //removendo ValoresVO Inexistentes
                    foreach(ValoresVO _vBack in cN.Valores)
                    {
                        bool achou = false;
                        foreach(ValoresVO _vAt in c.Valores)
                        {
                            if(_vBack.Volume.ToString() == _vAt.Volume.ToString())
                            {
                                achou = true;
                                break;
                            }
                        }

                        if(!achou)
                        {
                            if (!jaFezbkp)
                            {
                                jaFezbkp = true;
                            }
                            Delete(c.Motor, _vBack.Volume);
                        }
                    }

                    //Inserindo ValoresVO Inexistentes
                    foreach (ValoresVO _vAt in c.Valores)                           
                    {
                        bool achou = false;
                        foreach (ValoresVO _vBack in cN.Valores)
                        {
                            if (_vBack.Volume.ToString() == _vAt.Volume.ToString())
                            {
                                achou = true;
                                break;
                            }
                        }

                        if (!achou)
                        {
                            if (!jaFezbkp)
                            {
                                gerarBkpCalibragem();
                                jaFezbkp = true;
                            }
                            InsertValores(c.Motor, _vAt);
                        }
                    }
                    if(ExisteAlteracao(cN, c))
                    {
                        if (!jaFezbkp)
                        {
                            gerarBkpCalibragem();
                            jaFezbkp = true;
                        }
                        gerarEventoAlterarCalibracao(0, "Circuito Alterado:" + cN.Motor.ToString());
                    }
                    Update(c);
                }
            }
        }

        private static bool ExisteAlteracao(ObjectCalibragem c_last, ObjectCalibragem c_new)
        {
            bool retorno = false;
            
            if (c_last.Valores.Count != c_new.Valores.Count)
            {
                retorno = true;
            }
            else
            {
                foreach (ValoresVO _vAt in c_new.Valores)
                {
                    bool achou = false;
                    foreach (ValoresVO _vBack in c_last.Valores)
                    {
                        if (_vBack.Volume.ToString() == _vAt.Volume.ToString())
                        {
                            if(_vAt.Aceleracao == _vBack.Aceleracao && 
                                _vAt.Delay == _vBack.Delay &&                                    
                                _vAt.MassaIdeal == _vBack.MassaIdeal &&
                                _vAt.MassaMedia == _vBack.MassaMedia &&
                                _vAt.Velocidade == _vBack.Velocidade &&
                                _vAt.PulsoHorario == _vBack.PulsoHorario 
                                )
                            {
                                achou = true;
                            }
                            break;
                        }
                    }
                    if(!achou)
                    {
                        retorno = true;
                        break;
                    }
                } 
            }

            return retorno;
        }

        public static void Update(ObjectCalibragem c)
        {
            UpdateMinimoFaixas(c.Motor, c.MinimoFaixas);
            foreach (ValoresVO val in c.Valores)
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("UPDATE Valores SET "); //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao                            
                        sb.Append("Pulsos = '" + val.PulsoHorario.ToString() + "', ");
                        sb.Append("Velocidade = '" + val.Velocidade.ToString() + "', ");
                        sb.Append("ReverseDelay = '" + val.Delay.ToString() + "', ");
                        sb.Append("MassaMedia = '" + val.MassaMedia.ToString().Replace(",", ".") + "', ");
                        sb.Append("DesvioMedio = '" + val.DesvioMedio.ToString().Replace(",", ".") + "', ");
                        sb.Append("PulsoReverso = '" + val.PulsoReverso.ToString() + "', ");
                        sb.Append("Aceleracao = '" + val.Aceleracao.ToString() + "' ");
                        sb.Append(" WHERE Motor = '" + c.Motor.ToString()+"' ");
                        sb.Append(" AND Volume = '" + val.Volume.ToString().Replace(",",".") + "';");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
        }

        private static void InsertValores(int motor, ValoresVO _val)
        {
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("INSERT INTO Valores (Motor, Volume, Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao) VALUES (");
                    sb.Append("'" + motor.ToString() + "', ");
                    sb.Append("'" + _val.Volume.ToString().Replace(",", ".") + "', ");
                    sb.Append("'" + _val.PulsoHorario.ToString() + "', ");
                    sb.Append("'" + _val.Velocidade.ToString() + "', ");
                    sb.Append("'" + _val.Delay.ToString() + "', ");
                    sb.Append("'" + _val.MassaMedia.ToString().Replace(",", ".") + "', ");
                    sb.Append("'" + _val.DesvioMedio.ToString().Replace(",", ".") + "', ");
                    sb.Append("'" + _val.PulsoReverso.ToString() + "', ");
                    sb.Append("'" + _val.Aceleracao.ToString() + "' ");
                    sb.Append(");");

                    cmd.CommandText = sb.ToString();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }

            Negocio.OperacaoAutomatica inCalAUto = new Negocio.OperacaoAutomatica();
            inCalAUto.Motor = motor;
            inCalAUto.Volume = _val.Volume;
            inCalAUto.DesvioAdmissivel = 1;
            inCalAUto.IsCalibracaoAutomatica = 0;
            inCalAUto.IsPrimeiraCalibracao = 0;
            inCalAUto.IsRealizarMediaMedicao = 0;
            inCalAUto.NumeroDosagemTomadaMedia = 0;
            inCalAUto.NumeroMaxTentativa = 3;
            Util.ObjectCalibracaoAutomatica.InsertOperacao(motor, inCalAUto);

            gerarEventoAlterarCalibracao(0, "Circuito inserido:" + motor.ToString());
        }

        public static void UpdatePulsosRev(int motor, int pulsosRev)
        {
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("UPDATE Calibragem SET "); //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao                            
                    sb.Append(" UltimoPulsoReverso  = '" + pulsosRev.ToString() + "' ");
                    sb.Append(" WHERE Motor = '" + motor.ToString() + "'; ");

                    cmd.CommandText = sb.ToString();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public static void UpdateMinimoFaixas(int motor, int MinimoFaixas)
        {
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("UPDATE Calibragem SET "); //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao                            
                    sb.Append(" MinimoFaixas  = '" + MinimoFaixas.ToString() + "' ");
                    sb.Append(" WHERE Motor = '" + motor.ToString() + "'; ");

                    cmd.CommandText = sb.ToString();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public static bool Delete(int motor)
        {
            bool retorno = false;
            gerarBkpCalibragem();
            var commands = new[]
            {
                "DELETE FROM Calibragem WHERE Motor = '" + motor +"';",
                "DELETE FROM Valores WHERE Motor = '" + motor +"';"
            };

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    foreach (var command in commands)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(command);
                        string createQuery = sb.ToString();
                        cmd.CommandText = createQuery;
                        cmd.ExecuteNonQuery();
                        retorno = true;
                    }
                    conn.Close();
                }
            }
            if (retorno)
            {
                gerarEventoAlterarCalibracao(0, "Circuito excluido:" + motor.ToString());
                Util.ObjectCalibracaoAutomatica.DeleteFaixa(motor);
            }
            return retorno;
        }

        public static bool Delete(int motor, double volume)
        {
            bool retorno = false;
            gerarBkpCalibragem();
            var commands = new[]
            {
                "DELETE FROM Valores WHERE Motor = '" + motor +"' AND Volume = '"+ volume.ToString().Replace(",",".") +"'; "
            };

            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    foreach (var command in commands)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(command);
                        string createQuery = sb.ToString();
                        cmd.CommandText = createQuery;
                        cmd.ExecuteNonQuery();
                        retorno = true;
                    }
                    conn.Close();
                }
            }
            if(retorno)
            {
                gerarEventoAlterarCalibracao(0, "Circuito excluido:" + motor.ToString());
                Util.ObjectCalibracaoAutomatica.Delete(motor, volume);
            }
            return retorno;
        }

        private static int gerarEventoAlterarCalibracao(int result, string detalhes = "")
        {
            int retorno = 0;
            
            #region gravar Evento Alterada Calibra;'ao
            Util.ObjectEventos objEvt = new Util.ObjectEventos();
            objEvt.DATAHORA = DateTime.Now;
            objEvt.COD_EVENTO = (int)IOConnect.Core.PercoloreEnum.Eventos.AlteradaCalibracao;
            objEvt.DETALHES = result.ToString() + ";" + detalhes;
            objEvt.INTEGRADO = false;
            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            {
                objEvt.NUMERO_SERIE = percRegistry.GetSerialNumber();
            }
            retorno = Util.ObjectEventos.InsertEvento(objEvt);
            #endregion

            return retorno;
        }

        public static void gerarBkpCalibragem()
        {
            string n_serial = "";
            using (PercoloreRegistry percRegistry = new PercoloreRegistry())
            {
                n_serial = percRegistry.GetSerialNumber();
            }
            string local_bkp = zipPath + n_serial + "_Calibragem_" + string.Format("{0:dd_MM_yyyy_HH_mm_ss_fff}", DateTime.Now) + ".zip";
            //ZipFile.CreateFromDirectory(PathFile, local_bkp);

            using (FileStream fs = new FileStream(local_bkp, FileMode.Create))
            {
                using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    arch.CreateEntryFromFile(PathFile, FileName);                        
                }
                fs.Close();
            }
        }

        #endregion
    }
}