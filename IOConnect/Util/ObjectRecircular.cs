using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Util
{
    public class ObjectRecircular
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "Recircular.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Circuito { get; set; }      
        public bool Habilitado { get; set; } = true;
        public double VolumeDin { get; set; } = 0;
        public int Dias { get; set; } = 10;
        public double VolumeRecircular { get; set; } = 0;
        public double VolumeDosado { get; set; } = 0;
        public DateTime DtInicio { get; set; } = DateTime.Now;
        public bool isValve { get; set; } = false;
        public bool isAuto { get; set; } = false;

        public ObjectColorante _colorante;

        public ObjectRecircular()
        {

        }

        public static void CreateBD()
        {
            try
            {
                if (!File.Exists(PathFile))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CREATE TABLE IF NOT EXISTS [Recircular] (Circuito TEXT NULL, Habilitado TEXT NULL, VolumeDin TEXT NULL, Dias TEXT NULL, VolumeRecircular TEXT NULL, VolumeDosado TEXT NULL, DtInicio TEXT NULL, isValve TEXT NULL, isAuto TEXT NULL);");

                    string createQuery = sb.ToString();

                    SQLiteConnection connectCreate = Util.SQLite.CreateSQLiteConnection(PathFile, false);
                    connectCreate.Open();
					// Open connection to create DB if not exists.
					connectCreate.Close();
                    Thread.Sleep(2000);
                    if (File.Exists(PathFile))
                    {
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

        public static ObjectRecircular Load(int Circuito)
        {
            ObjectRecircular recircular = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Recircular WHERE Circuito = '" + Circuito.ToString() + "';";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                recircular = new ObjectRecircular();
                                recircular.Circuito = int.Parse(reader["Circuito"].ToString());
                                recircular.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                                recircular.VolumeRecircular = double.Parse(reader["VolumeRecircular"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                recircular.VolumeDin = double.Parse(reader["VolumeDin"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                recircular.VolumeDosado = double.Parse(reader["VolumeDosado"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                recircular.Dias = Convert.ToInt32(reader["Dias"].ToString());
                                recircular.DtInicio = Convert.ToDateTime(reader["DtInicio"].ToString());
                                recircular._colorante = Util.ObjectColorante.Load(recircular.Circuito);
                                try
                                {
                                    recircular.isValve = Convert.ToBoolean(reader["isValve"].ToString());
                                }
                                catch
                                { recircular.isValve = false; }
                                try
                                {
                                    recircular.isAuto = Convert.ToBoolean(reader["isAuto"].ToString());
                                }
                                catch
                                { recircular.isAuto = false; }

                                if (recircular._colorante != null && !recircular._colorante.Habilitado)
                                {
                                    recircular.Habilitado = false;
                                }
                                break;
                            }
                        }
                    }
                    conn.Close();
                }


                return recircular;
            }
            catch
            {
                throw;
            }
        }

        public static List<ObjectRecircular> List()
        {
            List<ObjectRecircular> list = new List<ObjectRecircular>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Recircular;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectRecircular recircular = new ObjectRecircular();
                                recircular.Circuito = int.Parse(reader["Circuito"].ToString());
                                recircular.Habilitado = Convert.ToBoolean(reader["Habilitado"].ToString());
                                recircular.VolumeRecircular = double.Parse(reader["VolumeRecircular"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                recircular.VolumeDin = double.Parse(reader["VolumeDin"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                recircular.VolumeDosado = double.Parse(reader["VolumeDosado"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                recircular.Dias = Convert.ToInt32(reader["Dias"].ToString());
                                recircular.DtInicio = Convert.ToDateTime(reader["DtInicio"].ToString());
                                try
                                {
                                    recircular.isValve = Convert.ToBoolean(reader["isValve"].ToString());
                                }
                                catch
                                { recircular.isValve = false; }
                                try
                                {
                                    recircular.isAuto = Convert.ToBoolean(reader["isAuto"].ToString());
                                }
                                catch
                                { recircular.isAuto = false; }
                                recircular._colorante = Util.ObjectColorante.Load(recircular.Circuito);
                                if (recircular._colorante != null && !recircular._colorante.Habilitado)
                                {
                                    recircular.Habilitado = false;
                                }
                                list.Add(recircular);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {

            }
            return list.OrderBy(o=>o.Circuito).ToList();

        }

        public static void Persist(ObjectRecircular recircular)
        {
            try
            {
                ObjectRecircular objc = Load(recircular.Circuito);
                //Insert
                if (objc == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                           
                            conn.Open();
                            sb.Append("INSERT INTO Recircular (Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, VolumeDosado, DtInicio, isValve, isAuto) VALUES (");
                            sb.Append("'" + recircular.Circuito.ToString() + "', ");
                            sb.Append("'" + (recircular.Habilitado ? "True" : "False") + "', ");
                            sb.Append("'" + recircular.VolumeDin.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + recircular.Dias.ToString() + "', ");
                            sb.Append("'" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                            sb.Append("'" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "', ");
                            sb.Append("'" + (recircular.isValve ? "True" : "False") + "', ");
                            sb.Append("'" + (recircular.isAuto ? "True" : "False") + "' ");
                            sb.Append(");");

                            cmd.CommandText = sb.ToString();

                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }
                //Update
                else
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("UPDATE Recircular SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                            sb.Append("Habilitado = '" + (recircular.Habilitado ? "True" : "False") + "', ");
                            sb.Append("VolumeDin = '" + recircular.VolumeDin.ToString().Replace(",", ".") + "', ");
                            sb.Append("Dias ='" + recircular.Dias.ToString() + "', ");
                            sb.Append("VolumeRecircular = '" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                            sb.Append("VolumeDosado = '" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                            sb.Append("DtInicio = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "', ");
                            sb.Append("isValve = '" + (recircular.isValve ? "True" : "False") + "', ");
                            sb.Append("isAuto = '" + (recircular.isAuto ? "True" : "False") + "' ");
                            sb.Append(" WHERE Circuito = '" + recircular.Circuito.ToString() + "';");

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

        public static void Persist(List<ObjectRecircular> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectRecircular recircular in lista)
                    {
                        ObjectRecircular objc = Load(recircular.Circuito);
                        //Insert
                        if (objc == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("INSERT INTO Recircular (Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, VolumeDosado, DtInicio, isValve, isAuto) VALUES (");
                                    sb.Append("'" + recircular.Circuito.ToString() + "', ");
                                    sb.Append("'" + (recircular.Habilitado ? "True" : "False") + "', ");
                                    sb.Append("'" + recircular.VolumeDin.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + recircular.Dias.ToString() + "', ");
                                    sb.Append("'" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                                    sb.Append("'" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "', ");
                                    sb.Append("'" + (recircular.isValve ? "True" : "False") + "', ");
                                    sb.Append("'" + (recircular.isAuto ? "True" : "False") + "' ");
                                    sb.Append(");");

                                    cmd.CommandText = sb.ToString();

                                    cmd.ExecuteNonQuery();

                                    conn.Close();
                                }
                            }
                        }
                        //Update
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("UPDATE Recircular SET "); // Circuito, Habilitado, VolumeDin, Dias, VolumeRecircular, DtInicio;
                                    sb.Append("Habilitado = '" + (recircular.Habilitado ? "True" : "False") + "', ");
                                    sb.Append("VolumeDin = '" + recircular.VolumeDin.ToString().Replace(",", ".") + "', ");
                                    sb.Append("Dias ='" + recircular.Dias.ToString() + "', ");
                                    sb.Append("VolumeRecircular = '" + recircular.VolumeRecircular.ToString().Replace(",", ".") + "', ");
                                    sb.Append("VolumeDosado = '" + recircular.VolumeDosado.ToString().Replace(",", ".") + "', ");
                                    sb.Append("DtInicio = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", recircular.DtInicio) + "', ");
                                    sb.Append("isValve = '" + (recircular.isValve ? "True" : "False") + "', ");
                                    sb.Append("isAuto = '" + (recircular.isAuto ? "True" : "False") + "' ");
                                    sb.Append(" WHERE Circuito = '" + recircular.Circuito.ToString() + "';");

                                    cmd.CommandText = sb.ToString();

                                    cmd.ExecuteNonQuery();

                                    conn.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static bool Validate(List<ObjectRecircular> lista, out string outMsg)
        {
            bool retorno = true;
            outMsg = "";
            foreach(ObjectRecircular _rec in lista)
            {
                if (_rec.Habilitado)
                {
                    if (_rec.Dias < 1)
                    {
                        outMsg += string.Format("Recirculação, circuito {0} possui Dia inferior a 1", _rec.Circuito);
                        retorno = false;

                    }
                    if (_rec.VolumeDin == 0)
                    {
                        outMsg += string.Format("Recirculação, circuito {0} possui Volume de limite inferior com valor igual ou abaixo de zero", _rec.Circuito);
                        retorno = false;
                    }
                    if (_rec.VolumeRecircular == 0)
                    {
                        outMsg += string.Format("Recirculação, circuito {0} possui Volume para Recirculação com valor igual ou abaixo de zero", _rec.Circuito);
                        retorno = false;
                    }

                    if (!retorno)
                    {
                        break;
                    }
                }
            }

            return retorno;
        }

        public static void UpdateVolumeDosado(int circuito, double volumeDos)
        {
            try
            {
                ObjectRecircular _rec = Load(circuito);
                if (_rec != null && _rec.Habilitado)
                {
                    _rec.VolumeDosado += volumeDos;
                    ObjectRecircular.Persist(_rec);
                }
            }
            catch
            {
                throw;
            }
        }

        public static bool UpdateRessetDate(DateTime data_hora)
        {
            bool retorno = false;
            try
            {
                if (File.Exists(PathFile))
                {
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();

                            cmd.CommandText = "UPDATE Recircular SET DtInicio = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", data_hora) + "';";
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            retorno = true;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return retorno;
        }


        public static bool Delete(int circuito)
        {
            bool retorno = false;
            try
            {
                var commands = new[] {
                            "DELETE FROM Recircular WHERE Circuito = '" + circuito +"';"
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
            }
            catch
            { }
            return retorno;
        }

        public static DataSet getTables()
        {
            DataSet ds = new DataSet();
            if (File.Exists(PathFile))
            {
                using (SQLiteConnection con = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    con.Open();
                    DataTable dt = con.GetSchema("Tables");
                    foreach (DataRow row in dt.Rows)
                    {
                        string tablename = (string)row[2];
                        DataTable tbl = new DataTable(tablename);
                        DataTable columnsTable = con.GetSchema("Columns", new[] { null, null, tablename, null });

                        bool incluir = false;

                        //foreach (DataRowCollection row in columnsTable.Rows)
                        //for(int i = 0; i < columnsTable.Rows.Count; i++)
                        foreach (DataRow rownm in columnsTable.Rows)
                        {
                            //DataRow rownm = columnsTable.Rows[i];
                            string colname = rownm["COLUMN_NAME"].ToString();
                            DataColumn col = new DataColumn(colname);
                            string data_type = rownm["DATA_TYPE"].ToString();
                            if (data_type != "")
                            {
                                if (data_type == "integer")
                                {
                                    col.DataType = typeof(System.Int32);
                                }
                                else if (data_type == "text")
                                {
                                    col.DataType = typeof(System.String);
                                }
                                incluir = true;
                                tbl.Columns.Add(col);
                            }
                        }
                        if (incluir)
                        {
                            ds.Tables.Add(tbl);
                        }

                    }
                    con.Close();
                }
            }

            return ds;
        }

    }
}
