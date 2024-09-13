using System.Data;
using System.Data.SQLite;

namespace UpdateConfig.Util
{
	public class ObjectParametros
    {
        public static readonly string PathFile = Path.Combine(@"C:\Percolore\IOConnect\", "Parametros.db");

        public static DataSet GetTables()
        {
            DataSet ds = new();
            if (File.Exists(PathFile))
            {
                using SQLiteConnection con = Util.SQLite.CreateSQLiteConnection(PathFile, false);
				con.Open();
				DataTable dt = con.GetSchema("Tables");
				foreach (DataRow row in dt.Rows)
				{
					string tablename = (string)row[2];
					DataTable tbl = new(tablename);
					DataTable columnsTable = con.GetSchema("Columns", [null, null, tablename, null]);

					bool incluir = false;

					foreach (DataRow rownm in columnsTable.Rows)
					{
						string colname = rownm["COLUMN_NAME"].ToString();
						DataColumn col = new(colname);
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

            return ds;
        }

        public static bool SetExecucaoParameter(string namepar, string value, DataSet dsTables)
        {
            bool retorno = false;
            try
            {
                if (File.Exists(PathFile))
                {
                    bool existeColum = false;
                    if (dsTables != null)
                    {
                        foreach (DataTable dt in dsTables.Tables)
                        {
                            foreach (DataColumn dc in dt.Columns)
                            {
                                if(dc.ColumnName == namepar)
                                {
                                    existeColum = true;
                                }
                            }
                        }
                    }
                    if(!existeColum)
                    {
						using SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(Util.ObjectParametros.PathFile, false);
						using SQLiteCommand cmd = new(conn);
						conn.Open();

						cmd.CommandText = "ALTER TABLE Parametros ADD " + namepar + " TEXT NULL;";
						cmd.ExecuteNonQuery();
						conn.Close();

					}


                    using (SQLiteConnection conn = Util.SQLite.CreateSQLiteConnection(PathFile, false))
                    {
						using SQLiteCommand cmd = new(conn);
						conn.Open();

						cmd.CommandText = "UPDATE Parametros SET " + namepar + " = " + value + ";";

						cmd.ExecuteNonQuery();

						conn.Close();
						retorno = true;
					}
                }
            }
            catch
            {
                
            }
            return retorno;
        }

    }
}
