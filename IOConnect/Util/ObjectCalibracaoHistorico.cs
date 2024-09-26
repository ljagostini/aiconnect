using Percolore.IOConnect.Negocio;
using System.Data.SQLite;
using System.Globalization;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectCalibracaoHistorico
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "CalibragemAutoHist.db");
        public static readonly string FileName = Path.GetFileName(PathFile);
        public Util.ObjectCalibracaoAutomatica _calibracaoAuto { get; set; }

        public double CapacideMaxBalanca { get; set; }
        public double MaxMassaAdmRecipiente { get; set; }
        public double VolumeMaxRecipiente { get; set; }
        public int NumeroMaxTentativaRec { get; set; }
        public double MinMassaAdmRecipiente { get; set; }

        public List<OperacaoAutoHist> listOperacaoAutoHist { get; set; }

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
                    sb.Append("CREATE TABLE IF NOT EXISTS [Recipiente] (CapacideMaxBalanca TEXT NULL, MaxMassaAdmRecipiente TEXT NULL, VolumeMaxRecipiente TEXT NULL, NumeroMaxTentativaRec TEXT NULL);");
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
                    sb.Append(" CREATE TABLE IF NOT EXISTS [Operacao] (Motor TEXT NULL, Etapa TEXT NULL, Etapa_Tentativa TEXT NULL, Volume TEXT NULL, ");
                    sb.Append(" MassaIdeal TEXT NULL, MassaMedBalanca TEXT NULL, VolumeDosado TEXT NULL, Desvio TEXT NULL, Aprovado TEXT NULL, Executado TEXT NULL);");
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

        public static ObjectCalibracaoHistorico Load(int motor)
        {
            ObjectCalibracaoHistorico c = null;
            
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();

                    cmd.CommandText = "SELECT * FROM Recipiente;";

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            c = new ObjectCalibracaoHistorico();
                            c.CapacideMaxBalanca = double.Parse(reader["CapacideMaxBalanca"].ToString(), CultureInfo.InvariantCulture);
                            c.MaxMassaAdmRecipiente = double.Parse(reader["MaxMassaAdmRecipiente"].ToString(), CultureInfo.InvariantCulture);
                            c.VolumeMaxRecipiente = double.Parse(reader["VolumeMaxRecipiente"].ToString(), CultureInfo.InvariantCulture);
                            c.NumeroMaxTentativaRec = int.Parse(reader["NumeroMaxTentativaRec"].ToString());
                            c.MinMassaAdmRecipiente = double.Parse(reader["MinMassaAdmRecipiente"].ToString(), CultureInfo.InvariantCulture);
                            c.listOperacaoAutoHist = new List<OperacaoAutoHist>();
                            break;
                        }
                        reader.Close();
                    }
                }
                conn.Close();
            }
            if (c != null)
            {
                c._calibracaoAuto = Util.ObjectCalibracaoAutomatica.Load(motor);
                c._calibracaoAuto._colorante = Util.ObjectColorante.Load(motor);

                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Operacao WHERE Motor = '" + motor.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OperacaoAutoHist valores = new OperacaoAutoHist();

                                int _Motor = 0;
                                int _Etapa = 0;
                                int _Etapa_Tentativa = 0;
                                string _Volume_str = "";
                                double _Volume = 0;
                                string _MassaIdeal_str = "";
                                double _MassaIdeal = 0;
                                string _MassaMedBalanca_str = "";
                                string _MassaMedBalancaMedia_str = "";
                                double _MassaMedBalanca = 0;
                                string _VolumeDosado_str = "";
                                string _VolumeDosadoMedia_str = "";
                                double _VolumeDosado = 0;
                                string _Desvio_str = "";
                                double _Desvio = 0;
                                int _Aprovado = 0;
                                int _Executado = 0;

                                _Volume_str = reader["Volume"] != DBNull.Value ? reader["Volume"].ToString() : "";
                                _MassaIdeal_str = reader["MassaIdeal"] != DBNull.Value ? reader["MassaIdeal"].ToString() : "";
                                _MassaMedBalanca_str = reader["MassaMedBalanca"] != DBNull.Value ? reader["MassaMedBalanca"].ToString() : "";
                                _VolumeDosado_str = reader["VolumeDosado"] != DBNull.Value ? reader["VolumeDosado"].ToString() : "";
                                _Desvio_str = reader["Desvio"] != DBNull.Value ? reader["Desvio"].ToString() : "";

                                _Motor = int.Parse(reader["Motor"].ToString());
                                _Etapa = int.Parse(reader["Etapa"].ToString());
                                _Etapa_Tentativa = int.Parse(reader["Etapa_Tentativa"].ToString());
                                _Aprovado = int.Parse(reader["Aprovado"].ToString());
                                _Executado = int.Parse(reader["Executado"].ToString());
                                _Volume = double.Parse(_Volume_str, CultureInfo.InvariantCulture);
                                _MassaIdeal = double.Parse(_MassaIdeal_str, CultureInfo.InvariantCulture);
                                
                                if(_MassaMedBalanca_str.Contains("#"))
                                {
                                    string[] _str = _MassaMedBalanca_str.Split('#');
                                    double media = 0;
                                    int count = 0;
                                    foreach(string _st in _str)
                                    {
                                        if (_st != null && _st.Length > 0)
                                        {
                                            media += double.Parse(_st, CultureInfo.InvariantCulture);
                                            count++;
                                        }
                                    }
                                    media = media / count;
                                    _MassaMedBalancaMedia_str = media.ToString();
                                }
                                else
                                {
                                    _MassaMedBalancaMedia_str = _MassaMedBalanca_str;
                                }

                                _MassaMedBalanca = double.Parse(_MassaMedBalancaMedia_str, CultureInfo.InvariantCulture);

                                if (_VolumeDosado_str.Contains("#"))
                                {
                                    string[] _str = _VolumeDosado_str.Split('#');
                                    double media = 0;
                                    int count = 0;
                                    foreach (string _st in _str)
                                    {
                                        if (_st != null && _st.Length > 0)
                                        {
                                            media += double.Parse(_st, CultureInfo.InvariantCulture);
                                            count++;
                                        }
                                    }
                                    media = media / count;
                                    _VolumeDosadoMedia_str = media.ToString();
                                }
                                else
                                {
                                    _VolumeDosadoMedia_str = _VolumeDosado_str;
                                }
                                _VolumeDosado = double.Parse(_VolumeDosadoMedia_str, CultureInfo.InvariantCulture);
                                
                                _Desvio = double.Parse(_Desvio_str, CultureInfo.InvariantCulture);

                                valores.Motor = _Motor;
                                valores.Etapa = _Etapa;
                                valores.Etapa_Tentativa = _Etapa_Tentativa;
                                valores.Volume = _Volume;
                                valores.Volume_str = _Volume_str;
                                valores.MassaIdeal = _MassaIdeal;
                                valores.MassaIdeal_str = _MassaIdeal_str;
                                valores.VolumeDosado = _VolumeDosado;
                                valores.VolumeDosado_str = _VolumeDosado_str;
                                valores.VolumeDosadoMedia_str = _VolumeDosadoMedia_str;
                                valores.MassaMedBalanca = _MassaMedBalanca;
                                valores.MassaMedBalanca_str = _MassaMedBalanca_str;
                                valores.MassaMedBalancaMedia_str = _MassaMedBalancaMedia_str;
                                valores.Desvio = _Desvio;
                                valores.Desvio_str = _Desvio_str;
                                valores.Aprovado = _Aprovado;
                                valores.Executado = _Executado;

                                if (c.listOperacaoAutoHist == null)
                                {
                                    c.listOperacaoAutoHist = new List<OperacaoAutoHist>();
                                }
                                c.listOperacaoAutoHist.Add(valores);
                            }
                            reader.Close();
                        }
                    }
                    conn.Close();
                }
            }

            return c;
        }

        public static void Add(ObjectCalibracaoHistorico c, bool _att = false)
        {
            if (c != null)
            {
                ObjectCalibracaoHistorico cN = Load(c._calibracaoAuto._colorante.Circuito);
                if (cN == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            //[Recipiente] (CapacideMaxBalanca TEXT NULL, MaxMassaAdmRecipiente TEXT NULL, VolumeMaxRecipiente TEXT NULL, NumeroMaxTentativaRec
                            sb.Append("INSERT INTO Recipiente (CapacideMaxBalanca, MaxMassaAdmRecipiente, VolumeMaxRecipiente, NumeroMaxTentativaRec, MinMassaAdmRecipiente) VALUES (");
                            sb.Append("'" + c._calibracaoAuto.CapacideMaxBalanca.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + c._calibracaoAuto.MaxMassaAdmRecipiente.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + c._calibracaoAuto.VolumeMaxRecipiente.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + c._calibracaoAuto.NumeroMaxTentativaRec.ToString() + "', ");
                            sb.Append("'" + c._calibracaoAuto.MinMassaAdmRecipiente.ToString() + "' ");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }

                    sb = new StringBuilder();
                    foreach (OperacaoAutoHist val in c.listOperacaoAutoHist)
                    {
                        sb = new StringBuilder();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {

                                //sb.Append(" CREATE TABLE IF NOT EXISTS [Operacao] (Motor TEXT NULL, Etapa TEXT NULL, Etapa_Teantativa TEXT NULL, Volume TEXT NULL, ");
                                //sb.Append(" MassaIdeal TEXT NULL, MassaMedBalanca TEXT NULL, VolumeDosado TEXT NULL, Desvio TEXT NULL, Aprovado TEXT NULL, Executado TEXT NULL);");

                                conn.Open();
                                sb.Append("INSERT INTO Operacao (Motor, Etapa, Etapa_Tentativa, Volume, MassaIdeal, MassaMedBalanca, VolumeDosado, Desvio, Aprovado, Executado) VALUES (");
                                sb.Append("'" + c._calibracaoAuto._colorante.Circuito.ToString() + "', ");
                                sb.Append("'" + val.Etapa.ToString() + "', ");
                                sb.Append("'" + val.Etapa_Tentativa.ToString() + "', ");
                                sb.Append("'" + val.Volume_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.MassaIdeal_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.MassaMedBalanca_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.VolumeDosado_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.Desvio_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.Aprovado.ToString() + "', ");
                                sb.Append("'" + val.Executado.ToString() + "' ");
                                sb.Append(");");

                                cmd.CommandText = sb.ToString();

                                cmd.ExecuteNonQuery();

                                conn.Close();
                            }
                        }
                    }
                }
                else if (cN != null && (cN.listOperacaoAutoHist == null || cN.listOperacaoAutoHist.Count == 0))
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            //[Recipiente] (CapacideMaxBalanca TEXT NULL, MaxMassaAdmRecipiente TEXT NULL, VolumeMaxRecipiente TEXT NULL, NumeroMaxTentativaRec
                            sb.Append("UPDATE Recipiente SET ");
                            sb.Append("CapacideMaxBalanca ='" + c.CapacideMaxBalanca.ToString().Replace(",", ".") + "', ");
                            sb.Append("MaxMassaAdmRecipiente = '" + c.MaxMassaAdmRecipiente.ToString().Replace(",", ".") + "', ");
                            sb.Append("VolumeMaxRecipiente = '" + c.VolumeMaxRecipiente.ToString().Replace(",", ".") + "', ");
                            sb.Append("NumeroMaxTentativaRec = '" + c.NumeroMaxTentativaRec.ToString() + "', ");
                            sb.Append("MinMassaAdmRecipiente = '" + c.MinMassaAdmRecipiente.ToString() + "'; ");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }

                    sb = new StringBuilder();
                    foreach (OperacaoAutoHist val in c.listOperacaoAutoHist)
                    {
                        sb = new StringBuilder();
                        using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(conn))
                            {

                                //sb.Append(" CREATE TABLE IF NOT EXISTS [Operacao] (Motor TEXT NULL, Etapa TEXT NULL, Etapa_Teantativa TEXT NULL, Volume TEXT NULL, ");
                                //sb.Append(" MassaIdeal TEXT NULL, MassaMedBalanca TEXT NULL, VolumeDosado TEXT NULL, Desvio TEXT NULL, Aprovado TEXT NULL, Executado TEXT NULL);");

                                conn.Open();
                                sb.Append("INSERT INTO Operacao (Motor, Etapa, Etapa_Tentativa, Volume, MassaIdeal, MassaMedBalanca, VolumeDosado, Desvio, Aprovado, Executado) VALUES (");
                                sb.Append("'" + c._calibracaoAuto._colorante.Circuito.ToString() + "', ");
                                sb.Append("'" + val.Etapa.ToString() + "', ");
                                sb.Append("'" + val.Etapa_Tentativa.ToString() + "', ");
                                sb.Append("'" + val.Volume_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.MassaIdeal_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.MassaMedBalanca_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.VolumeDosado_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.Desvio_str.Replace(",", ".") + "', ");
                                sb.Append("'" + val.Aprovado.ToString() + "', ");
                                sb.Append("'" + val.Executado.ToString() + "' ");
                                sb.Append(");");

                                cmd.CommandText = sb.ToString();

                                cmd.ExecuteNonQuery();

                                conn.Close();
                            }
                        }
                    }
                }
                else
                {
                    //removendo ValoresVO Inexistentes
                    foreach (OperacaoAutoHist _vBack in cN.listOperacaoAutoHist)
                    {
                        bool achou = false;
                        foreach (OperacaoAutoHist _vAt in c.listOperacaoAutoHist)
                        {
                            if (_vBack.Volume.ToString() == _vAt.Volume.ToString() && _vBack.Etapa == _vAt.Etapa )
                            {
                                achou = true;
                                break;
                            }
                        }

                        if (!achou)
                        {
                            Delete(c._calibracaoAuto._colorante.Circuito, _vBack.Volume, _vBack.Etapa);
                        }
                    }
                    //Inserindo ValoresVO Inexistentes
                    foreach (OperacaoAutoHist _vAt in c.listOperacaoAutoHist)
                    {
                        bool achou = false;
                        foreach (OperacaoAutoHist _vBack in cN.listOperacaoAutoHist)
                        {
                            if (_vBack.Volume.ToString() == _vAt.Volume.ToString() && _vBack.Etapa == _vAt.Etapa)
                            {
                                achou = true;
                                break;
                            }
                        }

                        if (!achou)
                        {
                            InsertOperacao(c._calibracaoAuto._colorante.Circuito, _vAt);
                        }
                    }

                    Update(c, _att);
                }
            }
        }

        public static void Update(ObjectCalibracaoHistorico c, bool _att = false)
        {
            StringBuilder sb = new StringBuilder();
            if (_att)
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        //sb.Append("CREATE TABLE IF NOT EXISTS [Recipiente] (CapacideMaxBalanca TEXT NULL, MaxMassaAdmRecipiente TEXT NULL, VolumeMaxRecipiente TEXT NULL, NumeroMaxTentativaRec);");
                        sb.Append("UPDATE Recipiente SET "); //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao                            
                        sb.Append("CapacideMaxBalanca = '" + c.CapacideMaxBalanca.ToString() + "', ");
                        sb.Append("MaxMassaAdmRecipiente = '" + c.MaxMassaAdmRecipiente.ToString().Replace(",", ".") + "', ");
                        sb.Append("VolumeMaxRecipiente = '" + c.VolumeMaxRecipiente.ToString() + "', ");
                        sb.Append("NumeroMaxTentativaRec = '" + c.NumeroMaxTentativaRec.ToString() + "', ");
                        sb.Append("MinMassaAdmRecipiente = '" + c.MinMassaAdmRecipiente.ToString() + "';");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }

            foreach (OperacaoAutoHist val in c.listOperacaoAutoHist)
            {
                //sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        string query = "";
                        //Operacao(Motor, Etapa, Etapa_Tentativa, Volume, MassaIdeal, MassaMedBalanca, VolumeDosado, Desvio, Aprovado, Executado)
                        query += "UPDATE Operacao SET "; //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao     
                        query += "MassaIdeal = '" + (val.MassaIdeal_str != null ? val.MassaIdeal_str.Replace(",", ".") : "" ) + "', ";
                        query += "MassaMedBalanca = '" + (val.MassaMedBalanca_str != null ? val.MassaMedBalanca_str.Replace(",", ".") : "") + "', ";
                        query += "VolumeDosado = '" +  (val.VolumeDosado_str != null ? val.VolumeDosado_str.Replace(",", ".") : "") + "', ";
                        query += "Desvio = '" + (val.Desvio_str != null ? val.Desvio_str.Replace(",", ".") : "") + "', ";
                        query += "Aprovado = '" + val.Aprovado.ToString() + "', ";
                        query += "Executado = '" + val.Executado.ToString() + "' ";
                        query += " WHERE Motor = '" + c._calibracaoAuto._colorante.Circuito.ToString() + "' ";
                        query += " AND Etapa = '" + val.Etapa.ToString() + "' ";
                        query += " AND Etapa_Tentativa = '" + val.Etapa_Tentativa.ToString() + "' ";
                        query += " AND Volume = '" + val.Volume.ToString().Replace(",", ".") + "';";


                        cmd.CommandText = query;

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
        }

        public static void InsertOperacao(int motor, OperacaoAutoHist _val)
        {
            StringBuilder sb = new StringBuilder();
            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    conn.Open();
                    sb.Append("INSERT INTO Operacao (Motor, Etapa, Etapa_Tentativa, Volume, MassaIdeal, MassaMedBalanca, VolumeDosado, Desvio, Aprovado, Executado) VALUES (");
                    sb.Append("'" + motor.ToString() + "', ");
                    sb.Append("'" + _val.Etapa.ToString() + "', ");
                    sb.Append("'" + _val.Etapa_Tentativa.ToString() + "', ");
                    sb.Append("'" + _val.Volume_str.Replace(",", ".") + "', ");
                    sb.Append("'" + _val.MassaIdeal_str.Replace(",", ".") + "', ");
                    sb.Append("'" + _val.MassaMedBalanca_str.Replace(",", ".") + "', ");
                    sb.Append("'" + _val.VolumeDosado_str.Replace(",", ".") + "', ");
                    sb.Append("'" + _val.Desvio_str.Replace(",", ".") + "', ");
                    sb.Append("'" + _val.Aprovado.ToString() + "', ");
                    sb.Append("'" + _val.Executado.ToString() + "' ");
                    sb.Append(");");

                    cmd.CommandText = sb.ToString();

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        public static bool Delete(int motor, double volume, int etapa)
        {
            bool retorno = false;
            var commands = new[]
            {
                "DELETE FROM Operacao WHERE Etapa = '" + etapa + "' AND Motor = '" + motor +"' AND Volume = '"+ volume.ToString().Replace(",",".") +"'; "
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
            return retorno;
        }

        public static bool DeleteFaixa(int motor)
        {
            bool retorno = false;
            var commands = new[]
            {
                "DELETE FROM Operacao WHERE Motor = '" + motor +"'; "
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
            return retorno;
        }
        #endregion
    }
}