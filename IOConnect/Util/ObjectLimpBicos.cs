using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Percolore.IOConnect.Util
{
    public class ObjectLimpBicos
    {
        public static readonly string PathFile = Path.Combine(Environment.CurrentDirectory, "LimpezaBicos.db");
        public static readonly string FileName = Path.GetFileName(PathFile);

        public int Id { get; set; }
        public TimeSpan? Horario { get; set; } = null;

        public ObjectLimpBicos()
        { }

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
                        sb.Append("CREATE TABLE IF NOT EXISTS [Periodos] (Id INTEGER PRIMARY KEY, Horario TEXT NULL);");
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
                        sb.Append("CREATE TABLE IF NOT EXISTS [Executado] (Id INTEGER PRIMARY KEY, Horario TEXT NULL);");
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
                        string dataLastHorario = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now.AddDays(-1));
                        sb = new StringBuilder();
                        sb.Append("INSERT INTO Executado (Horario) VALUES ('" + dataLastHorario + "');");
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

        public static ObjectLimpBicos Load(int id)
        {
            ObjectLimpBicos per = null;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Periodos WHERE Id = " + id.ToString() + ";";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                per = new ObjectLimpBicos();
                                per.Id = int.Parse(reader["Id"].ToString());
                                per.Horario = TimeSpan.Parse(reader["Horario"].ToString());                                
                                break;
                            }
                        }
                    }
                    conn.Close();
                }


                return per;
            }
            catch
            {
                throw;
            }
        }
     

        public static List<ObjectLimpBicos> List()
        {
            List<ObjectLimpBicos> list = new List<ObjectLimpBicos>();

            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Periodos;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ObjectLimpBicos per = new ObjectLimpBicos();
                                per.Id = int.Parse(reader["Id"].ToString());
                                per.Horario = TimeSpan.Parse(reader["Horario"].ToString());
                               
                                list.Add(per);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch
            {

            }
            return list;

        }

        public static bool Validate(List<ObjectLimpBicos> lista, out string outMsg)
        {
            if (lista == null)
                throw new ArgumentNullException();

            StringBuilder validaItem = new StringBuilder();
            StringBuilder validaLista = new StringBuilder();

            //Seleciona somente habilitados           
            foreach (ObjectLimpBicos per in lista)
            {
                validaItem.Clear();

                if (per.Horario == null)
                    validaItem.AppendLine(Negocio.IdiomaResxExtensao.LimpBicosFormatoInvalido);/* "Formato: HH:mm:ss");*/

                if (validaItem.Length > 0)
                {
                    string texto = Negocio.IdiomaResxExtensao.LimpBicosHorarioObrigatorio; /*"Horário obrigatório";*/
                    validaLista.AppendLine(texto);
                    validaLista.AppendLine(validaItem.ToString());
                }
            }

            outMsg = validaLista.ToString();
            return
                (validaLista.Length == 0);
        }

        public static void Persist(ObjectLimpBicos per)
        {
            try
            {
                ObjectLimpBicos objc = null;
                if (per.Id > 0)
                {
                    objc = Load(per.Id);
                }
                //Insert
                if (objc == null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(conn))
                        {
                            conn.Open();
                            sb.Append("INSERT INTO Periodos (Horario) VALUES (");                         
                            sb.Append("'" + per.Horario.Value.ToString(@"hh\:mm\:ss") + "' ");
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
                            sb.Append("UPDATE Periodos SET "); // 
                            sb.Append("Horario = '" + per.Horario.Value.ToString(@"hh\:mm\:ss") + "'");                            
                            sb.Append(" WHERE Id = " + per.Id.ToString() + ";");

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

        public static void Persist(List<ObjectLimpBicos> lista)
        {
            try
            {
                if (lista != null && lista.Count > 0)
                {
                    foreach (ObjectLimpBicos per in lista)
                    {
                        ObjectLimpBicos objc = null;
                        if (per.Id > 0)
                        {
                            objc = Load(per.Id);
                        }
                        //Insert
                        if (objc == null)
                        {
                            StringBuilder sb = new StringBuilder();
                            using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                                {
                                    conn.Open();
                                    sb.Append("INSERT INTO Periodos (Horario) VALUES (");
                                    sb.Append("'" + per.Horario.Value.ToString(@"hh\:mm\:ss") + "' ");
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
                                    sb.Append("UPDATE Periodos SET "); // 
                                    sb.Append("Horario = '" + per.Horario.Value.ToString(@"hh\:mm\:ss") + "'");
                                    sb.Append(" WHERE Id = " + per.Id.ToString() + ";");

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

        public static bool LimpBicos_Delete(int id)
        {
            bool retorno = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("Delete From Periodos WHERE Id = " + id.ToString() + ";");
                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }

                retorno = true;
            }
            catch
            {
            }
            return retorno;
        }

        public static void UpdateExecutado(DateTime dtExecutado)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("UPDATE Executado SET "); // 
                        sb.Append("Horario = '" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", dtExecutado) + "';");
                       

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

        public static DateTime LoadExecutado()
        {
            DateTime dtExecutado = DateTime.Now;
            try
            {
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();

                        cmd.CommandText = "SELECT * FROM Executado;";

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dtExecutado = Convert.ToDateTime(reader["Horario"].ToString());
                                break;
                            }
                        }
                    }
                    conn.Close();
                }


                return dtExecutado;
            }
            catch
            {
                throw;
            }
        }


        #endregion
    }
}
