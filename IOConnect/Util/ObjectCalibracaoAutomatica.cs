using Percolore.Core;
using Percolore.IOConnect.Negocio;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Util
{
    public class ObjectCalibracaoAutomatica
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "CalibragemAuto.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public Util.ObjectCalibragem _calibragem { get; set;}
        public Util.ObjectColorante _colorante { get; set; }

        public double CapacideMaxBalanca { get; set; }
        public double MaxMassaAdmRecipiente { get; set; }
        public double VolumeMaxRecipiente { get; set; }
        public int NumeroMaxTentativaRec { get; set; }
        public double MinMassaAdmRecipiente { get; set; }

        public List<OperacaoAutomatica> listOperacaoAutomatica { get; set; }


        #region Métodos
        public static void CreateBD()
        {
            try
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
                        sb.Append("CREATE TABLE IF NOT EXISTS [Recipiente] (CapacideMaxBalanca TEXT NULL, MaxMassaAdmRecipiente TEXT NULL, VolumeMaxRecipiente TEXT NULL, NumeroMaxTentativaRec TEXT NULL, MinMassaAdmRecipiente TEXT NULL);");
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
                        sb.Append(" CREATE TABLE IF NOT EXISTS [Operacao] (Motor TEXT NULL, IsPrimeiraCalibracao TEXT NULL, Volume TEXT NULL, DesvioAdmissivel TEXT NULL, ");
                        sb.Append(" IsCalibracaoAutomatica TEXT NULL, NumeroMaxTentativa TEXT NULL, IsRealizarMediaMedicao TEXT NULL, NumeroDosagemTomadaMedia TEXT NULL);");
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
            catch
            { }
        }

        public static ObjectCalibracaoAutomatica Load(int motor)
        {
            ObjectCalibracaoAutomatica c = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Recipiente ;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                c = new ObjectCalibracaoAutomatica();
                                c.CapacideMaxBalanca = double.Parse(reader["CapacideMaxBalanca"].ToString(), CultureInfo.InvariantCulture);
                                c.MaxMassaAdmRecipiente = double.Parse(reader["MaxMassaAdmRecipiente"].ToString(), CultureInfo.InvariantCulture);
                                c.VolumeMaxRecipiente = double.Parse(reader["VolumeMaxRecipiente"].ToString(), CultureInfo.InvariantCulture);
                                c.NumeroMaxTentativaRec = int.Parse(reader["NumeroMaxTentativaRec"].ToString());
                                try
                                {
                                    c.MinMassaAdmRecipiente = double.Parse(reader["MinMassaAdmRecipiente"].ToString(), CultureInfo.InvariantCulture);
                                }
                                catch
                                { }
                                c.listOperacaoAutomatica = new List<OperacaoAutomatica>();
                                break;
                            }
                            reader.Close();
                        }
                    }
                    conn.Close();
                }
                if (c != null)
                {
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
                                    OperacaoAutomatica valores = new OperacaoAutomatica();

                                  
                                    int _Motor = 0;
                                    int _IsPrimeiraCalibracao = 0;
                                    double _Volume = 0;
                                    double _DesvioAdmissivel = 0;
                                    int _IsCalibracaoAutomatica = 0;
                                    int _NumeroMaxTentativa = 0;
                                    int _IsRealizarMediaMedicao = 0;
                                    int _NumeroDosagemTomadaMedia = 0;

                                    try
                                    {
                                        _Motor = int.Parse(reader["Motor"].ToString());
                                        _IsPrimeiraCalibracao = int.Parse(reader["IsPrimeiraCalibracao"].ToString());
                                        _Volume = double.Parse(reader["Volume"].ToString(), CultureInfo.InvariantCulture);
                                        _DesvioAdmissivel = double.Parse(reader["DesvioAdmissivel"].ToString(), CultureInfo.InvariantCulture);
                                        _IsCalibracaoAutomatica = int.Parse(reader["IsCalibracaoAutomatica"].ToString());
                                        _NumeroMaxTentativa = int.Parse(reader["NumeroMaxTentativa"].ToString());
                                        _IsRealizarMediaMedicao = int.Parse(reader["IsRealizarMediaMedicao"].ToString());
                                        _NumeroDosagemTomadaMedia = int.Parse(reader["NumeroDosagemTomadaMedia"].ToString());
                                    }
                                    catch
                                    {

                                    }
                                    valores.Motor = _Motor;
                                    valores.IsPrimeiraCalibracao = _IsPrimeiraCalibracao;
                                    valores.Volume = _Volume;
                                    valores.DesvioAdmissivel = _DesvioAdmissivel;
                                    valores.IsCalibracaoAutomatica = _IsCalibracaoAutomatica;
                                    valores.NumeroMaxTentativa = _NumeroMaxTentativa;
                                    valores.IsRealizarMediaMedicao = _IsRealizarMediaMedicao;
                                    valores.NumeroDosagemTomadaMedia = _NumeroDosagemTomadaMedia;
                                    if(c.listOperacaoAutomatica == null )
                                    {
                                        c.listOperacaoAutomatica = new List<OperacaoAutomatica>();
                                    }
                                    c.listOperacaoAutomatica.Add(valores);
                                }
                                reader.Close();
                            }
                        }
                        conn.Close();
                    }
                }
                if (c != null)
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibragem.PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "SELECT * FROM Calibragem WHERE Motor = '" + motor.ToString() + "';";

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    c._calibragem = new ObjectCalibragem();
                                    c._calibragem.Motor = int.Parse(reader["Motor"].ToString());
                                    c._calibragem.UltimoPulsoReverso = int.Parse(reader["UltimoPulsoReverso"].ToString());

                                    c._calibragem.Valores = new List<ValoresVO>();
                                    break;
                                }
                                reader.Close();
                            }
                        }
                        conn.Close();
                    }
                }
                if (c != null)
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectCalibragem.PathFile, false))
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

                                    try
                                    {
                                        PulsoHorario = int.Parse(reader["Pulsos"].ToString());
                                        Velocidade = int.Parse(reader["Velocidade"].ToString());
                                        Delay = int.Parse(reader["ReverseDelay"].ToString());
                                        Volume = double.Parse(reader["Volume"].ToString(), CultureInfo.InvariantCulture);
                                        MassaMedia = double.Parse(reader["MassaMedia"].ToString(), CultureInfo.InvariantCulture);
                                        DesvioMedio = double.Parse(reader["DesvioMedio"].ToString(), CultureInfo.InvariantCulture);

                                    }
                                    catch
                                    {

                                    }
                                    try
                                    {
                                        PulsoReverso = int.Parse(reader["PulsoReverso"].ToString());
                                        Aceleracao = int.Parse(reader["Aceleracao"].ToString());
                                    }
                                    catch
                                    { }

                                    valores.Volume = Volume;
                                    valores.PulsoHorario = PulsoHorario;
                                    valores.Velocidade = Velocidade;
                                    valores.Delay = Delay;
                                    valores.MassaMedia = MassaMedia;
                                    valores.DesvioMedio = DesvioMedio;
                                    valores.PulsoReverso = PulsoReverso;
                                    valores.Aceleracao = Aceleracao;
                                    c._calibragem.Valores.Add(valores);
                                }
                                reader.Close();
                            }
                        }
                        conn.Close();
                    }
                }
           
                return c;
            }
            catch
            {
                throw;
            }
        }      

        public static void Add(ObjectCalibracaoAutomatica c, bool _att = false)
        {
            try
            {
                if (c != null)
                {
                    ObjectCalibracaoAutomatica cN = Load(c._calibragem.Motor);
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
                                sb.Append("'" + c.CapacideMaxBalanca.ToString().Replace(",", ".") + "', ");
                                sb.Append("'" + c.MaxMassaAdmRecipiente.ToString().Replace(",", ".") + "', ");
                                sb.Append("'" + c.VolumeMaxRecipiente.ToString().Replace(",", ".") + "', ");
                                sb.Append("'" + c.NumeroMaxTentativaRec.ToString() + "', ");
                                sb.Append("'" + c.MinMassaAdmRecipiente.ToString() + "'");
                                sb.Append(");");

                                cmd.CommandText = sb.ToString();

                                cmd.ExecuteNonQuery();

                                conn.Close();
                            }
                        }

                        sb = new StringBuilder();
                        foreach (OperacaoAutomatica val in c.listOperacaoAutomatica)
                        {
                            sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("INSERT INTO Operacao (Motor, IsPrimeiraCalibracao, Volume, DesvioAdmissivel, IsCalibracaoAutomatica, NumeroMaxTentativa, IsRealizarMediaMedicao, NumeroDosagemTomadaMedia) VALUES (");
                                    sb.Append("'" + c._calibragem.Motor.ToString() + "', ");
                                    sb.Append("'" + val.IsPrimeiraCalibracao.ToString() + "', ");
                                    sb.Append("'" + val.Volume.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + val.DesvioAdmissivel.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + val.IsCalibracaoAutomatica.ToString() + "', ");
                                    sb.Append("'" + val.NumeroMaxTentativa.ToString() + "', ");
                                    sb.Append("'" + val.IsRealizarMediaMedicao.ToString() + "', ");
                                    sb.Append("'" + val.NumeroDosagemTomadaMedia.ToString() + "' ");
                                    sb.Append(");");

                                    cmd.CommandText = sb.ToString();

                                    cmd.ExecuteNonQuery();

                                    conn.Close();
                                }
                            }
                        }
                    }
                    else if(cN != null && (cN.listOperacaoAutomatica == null || cN.listOperacaoAutomatica.Count == 0))
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
                        foreach (OperacaoAutomatica val in c.listOperacaoAutomatica)
                        {
                            sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("INSERT INTO Operacao (Motor, IsPrimeiraCalibracao, Volume, DesvioAdmissivel, IsCalibracaoAutomatica, NumeroMaxTentativa, IsRealizarMediaMedicao, NumeroDosagemTomadaMedia) VALUES (");
                                    sb.Append("'" + c._calibragem.Motor.ToString() + "', ");
                                    sb.Append("'" + val.IsPrimeiraCalibracao.ToString() + "', ");
                                    sb.Append("'" + val.Volume.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + val.DesvioAdmissivel.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + val.IsCalibracaoAutomatica.ToString() + "', ");
                                    sb.Append("'" + val.NumeroMaxTentativa.ToString() + "', ");
                                    sb.Append("'" + val.IsRealizarMediaMedicao.ToString() + "', ");
                                    sb.Append("'" + val.NumeroDosagemTomadaMedia.ToString() + "' ");
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
                        foreach (OperacaoAutomatica _vBack in cN.listOperacaoAutomatica)
                        {
                            bool achou = false;
                            foreach (OperacaoAutomatica _vAt in c.listOperacaoAutomatica)
                            {
                                if (_vBack.Volume.ToString() == _vAt.Volume.ToString())
                                {
                                    achou = true;
                                    break;
                                }
                            }

                            if (!achou)
                            {
                                Delete(c._calibragem.Motor, _vBack.Volume);
                            }
                        }
                        //Inserindo ValoresVO Inexistentes
                        foreach (OperacaoAutomatica _vAt in c.listOperacaoAutomatica)
                        {
                            bool achou = false;
                            foreach (OperacaoAutomatica _vBack in cN.listOperacaoAutomatica)
                            {
                                if (_vBack.Volume.ToString() == _vAt.Volume.ToString())
                                {
                                    achou = true;
                                    break;
                                }
                            }

                            if (!achou)
                            {
                                InsertOperacao(c._calibragem.Motor, _vAt);
                            }
                        }

                        Update(c, _att);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void Update(ObjectCalibracaoAutomatica c, bool _att = false)
        {
            try
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

                foreach (OperacaoAutomatica val in c.listOperacaoAutomatica)
                {
                    sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            if (val.IsPrimeiraCalibracao == 0)
                            {
                                sb.Append("UPDATE Operacao SET "); //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao     
                                sb.Append("DesvioAdmissivel = '" + val.DesvioAdmissivel.ToString().Replace(",", ".") + "', ");
                                sb.Append("IsCalibracaoAutomatica = '" + val.IsCalibracaoAutomatica.ToString() + "', ");
                                sb.Append("NumeroMaxTentativa = '" + val.NumeroMaxTentativa.ToString() + "', ");
                                sb.Append("IsRealizarMediaMedicao = '" + val.IsRealizarMediaMedicao.ToString() + "', ");
                                sb.Append("NumeroDosagemTomadaMedia = '" + val.NumeroDosagemTomadaMedia.ToString() + "' ");
                                sb.Append(" WHERE Motor = '" + c._calibragem.Motor.ToString() + "' ");
                                sb.Append(" AND IsPrimeiraCalibracao = '" + val.IsPrimeiraCalibracao.ToString() + "' ");
                                sb.Append(" AND Volume = '" + val.Volume.ToString().Replace(",", ".") + "';");
                            }
                            else
                            {
                                sb.Append("UPDATE Operacao SET "); //Pulsos, Velocidade, ReverseDelay, MassaMedia, DesvioMedio, PulsoReverso, Aceleracao     
                                sb.Append("DesvioAdmissivel = '" + val.DesvioAdmissivel.ToString().Replace(",", ".") + "', ");
                                sb.Append("IsCalibracaoAutomatica = '" + val.IsCalibracaoAutomatica.ToString() + "', ");
                                sb.Append("NumeroMaxTentativa = '" + val.NumeroMaxTentativa.ToString() + "', ");
                                sb.Append("IsRealizarMediaMedicao = '" + val.IsRealizarMediaMedicao.ToString() + "', ");
                                sb.Append("NumeroDosagemTomadaMedia = '" + val.NumeroDosagemTomadaMedia.ToString() + "', ");
                                sb.Append("Volume = '" + val.Volume.ToString().Replace(",", ".") + "' ");
                                sb.Append(" WHERE Motor = '" + c._calibragem.Motor.ToString() + "' ");
                                sb.Append(" AND IsPrimeiraCalibracao = '" + val.IsPrimeiraCalibracao.ToString() + "';");
                            }

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void InsertOperacao(int motor, OperacaoAutomatica _val)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("INSERT INTO Operacao (Motor, IsPrimeiraCalibracao, Volume, DesvioAdmissivel, IsCalibracaoAutomatica, NumeroMaxTentativa, IsRealizarMediaMedicao, NumeroDosagemTomadaMedia) VALUES (");
                        sb.Append("'" + motor.ToString() + "', ");
                        sb.Append("'" + _val.IsPrimeiraCalibracao.ToString() + "', ");
                        sb.Append("'" + _val.Volume.ToString().Replace(",", ".") + "', ");
                        sb.Append("'" + _val.DesvioAdmissivel.ToString().Replace(",", ".") + "', ");
                        sb.Append("'" + _val.IsCalibracaoAutomatica.ToString() + "', ");
                        sb.Append("'" + _val.NumeroMaxTentativa.ToString() + "', ");
                        sb.Append("'" + _val.IsRealizarMediaMedicao.ToString() + "', ");
                        sb.Append("'" + _val.NumeroDosagemTomadaMedia.ToString() + "' ");
                        sb.Append(");");

                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }

            }
            catch
            {
                throw;
            }
        }

        public static bool Delete(int motor, double volume)
        {
            bool retorno = false;
            var commands = new[] {
                        "DELETE FROM Operacao WHERE IsPrimeiraCalibracao = '0' AND Motor = '" + motor +"' AND Volume = '"+ volume.ToString().Replace(",",".") +"'; "
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
            var commands = new[] {
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
