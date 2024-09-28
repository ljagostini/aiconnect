using Percolore.Core.Logging;
using System.Data.SQLite;
using System.Text;

namespace Percolore.IOConnect.Util
{
	public class ObjectLogBD
    {
        public static string PathFile = null;
        public static string FileName = null;

        public int Id { get; set; }
        public string DETALHES { get; set; } = "";

        public DateTime DATAHORA { get; set; }

        public ObjectLogBD(string pathLog, string nameFile)
        {
             Util.ObjectParametros p = Util.ObjectParametros.Load();
            string[] arrayStr = pathLog.Split(Path.DirectorySeparatorChar);
            string strPath = "";
            for (int i = 0; i < arrayStr.Length - 1; i++)
            {
                strPath += arrayStr[i] + Path.DirectorySeparatorChar;
            }

            ObjectLogBD.PathFile = Path.Combine(strPath, nameFile);
            ObjectLogBD.FileName = Path.GetFileName(PathFile);
            CreateBD();
        }

        public static void CreateBD()
        {
            try
            {
                string createQuery =
                        @"CREATE TABLE IF NOT EXISTS [Logs] (
                            [Id]     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [DATAHORA]   TEXT NULL,
                            [DETALHES] TEXT NULL)";

                if (File.Exists(PathFile))
                {
                    Util.ObjectParametros _par = Util.ObjectParametros.Load();
                    bool existeFile = true;
                    if (_par.LogAutomateBackup)
                    {
                        FileInfo info = new FileInfo(PathFile);
                        double length = (info.Length / 1048576.0);
                        if (length > 20.0)
                        {
                            string[] arrayPath = PathFile.Split(Path.DirectorySeparatorChar);
                            if (arrayPath.Length > 0)
                            {
                                string nameFile = string.Format("{0:dd-MM-yyyy}", DateTime.Now) + "_" + arrayPath[arrayPath.Length - 1];
                                string nPathFile = "";
                                for (int i = 0; i < arrayPath.Length - 1; i++)
                                {
                                    if (arrayPath[i].Length > 0)
                                    {
                                        nPathFile += arrayPath[i] + Path.DirectorySeparatorChar;
                                    }
                                }
                                if (nPathFile.Length > 0)
                                {
                                    nPathFile += nameFile;
                                }
                                File.Move(PathFile, nPathFile);
                                existeFile = false;
                            }
                        }
                    }
                    if(!existeFile)
                    {
                        StringBuilder sb = new StringBuilder();

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
                else
                {
                    StringBuilder sb = new StringBuilder();

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
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectLogBD).Name}: ", e);
			}
		}

        public static int InsertLog(ObjectLogBD objLog)
        {
            int retorno = 0;
            try
            {
                StringBuilder sb = new StringBuilder();
                using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        conn.Open();
                        sb.Append("INSERT INTO Logs (DETALHES, DATAHORA) VALUES (");
                      
                        sb.Append("'" + objLog.DETALHES + "', ");
                        sb.Append("'" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", objLog.DATAHORA) + "'");
                        sb.Append(");");
                        cmd.CommandText = sb.ToString();

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT last_insert_rowid();";
                        Int64 LastRowID64 = (Int64)cmd.ExecuteScalar();
                        retorno = (int)LastRowID64;

                        conn.Close();
                    }
                }
            }
			catch (Exception e)
			{
				LogManager.LogError($"Erro no módulo {typeof(ObjectLogBD).Name}: ", e);
			}

			return retorno;
        }
    }
}