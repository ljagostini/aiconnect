using System.Data;
using System.Data.OleDb;

namespace Percolore.Instalacao
{
	/// <summary>
	/// Connects to an access database and sends queries
	/// </summary>
	public class AccessDatabase
    {
        private const string connectInfo = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=";
        private string connString;
        private OleDbConnection dbAccessConn;
        private OleDbCommand dbAccessCommand;
        private OleDbDataAdapter dbDataAdapter;
        private DataSet dbData;
        int rowsAffected;
   
        public AccessDatabase(string dbfile)
        {
            connString = connectInfo + dbfile;
        }

        public string ConnString()
        {
            return connString;
        }

        public DataSet Query(string query)
        {
            //Connect to db
            try
            {
                dbAccessConn = new OleDbConnection(connString);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to database -- E001 -- \n" + e.Message);
            }

            //Send query
            try
            {
                dbAccessCommand = new OleDbCommand(query, dbAccessConn);
            }
            catch (Exception e)
            {
                throw new Exception("Could not send query -- E002 -- \n" + e.Message);
            }

            //Read query result
            try
            {
                dbAccessConn.Open();
                dbDataAdapter = new OleDbDataAdapter(dbAccessCommand);
                dbData = new DataSet();
                dbData.EnforceConstraints = false;
                dbDataAdapter.Fill(dbData);
            }
            catch (Exception e)
            {
                throw new Exception("Could not read query result. \n" + e.Message);
            }
            finally
            {
                dbAccessConn.Close();
            }

            return dbData;
        }

        public DataTable QueryDatatable(string query)
        {
            DataTable dataTable;

            //Connect to db
            try
            {
                dbAccessConn = new OleDbConnection(connString);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to database. \n" + e.Message);
            }

            //Send query
            try
            {
                dbAccessCommand = new OleDbCommand(query, dbAccessConn);
            }
            catch (Exception e)
            {
                throw new Exception("Could not send query. \n" + e.Message);
            }

            //Read query result
            try
            {
                dbAccessConn.Open();
                dbDataAdapter = new OleDbDataAdapter(dbAccessCommand);
                dataTable = new DataTable();
                dataTable.BeginLoadData();
                dbDataAdapter.Fill(dataTable);
                dataTable.EndLoadData();
            }
            catch (Exception e)
            {
                throw new Exception("Could not read query result. \n" + e.Message);
            }
            finally
            {
                dbAccessConn.Close();
            }

            return dataTable;
        }

        public int Execute(string query)
        {
            //Connect to db
            try
            {
                dbAccessConn = new OleDbConnection(connString);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to database. \n" + e.Message);
            }

            //Send query
            try
            {
                dbAccessCommand = new OleDbCommand(query, dbAccessConn);

            }
            catch (Exception e)
            {
                throw new Exception("Could not send query. \n" + e.Message);
            }

            //Read query result
            try
            {
                dbAccessConn.Open();
                rowsAffected = dbAccessCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Could not read query result. \n" + e.Message);
            }
            finally
            {
                dbAccessConn.Close();
            }

            return rowsAffected;
        }

        public int Execute(string query, OleDbParameter[] parametros)
        {
            //Connect to db
            try
            {
                dbAccessConn = new OleDbConnection(connString);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to database. \n" + e.Message);
            }

            //Send query
            try
            {
                dbAccessCommand = new OleDbCommand(query, dbAccessConn);

                foreach (OleDbParameter p in parametros)
                    dbAccessCommand.Parameters.Add(p);

            }
            catch (Exception e)
            {
                throw new Exception("Could not send query. \n" + e.Message);
            }

            //Read query result
            try
            {
                dbAccessConn.Open();
                rowsAffected = dbAccessCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Could not read query result. \n" + e.Message);
            }
            finally
            {
                dbAccessConn.Close();
            }

            return rowsAffected;
        }

        public int Execute(OleDbConnection connection, string query, ref OleDbTransaction oleDbTransaction)
        {

            try
            {
                dbAccessCommand = new OleDbCommand(query, connection);

                //insere comnado na transação
                dbAccessCommand.Transaction = oleDbTransaction;

            }
            catch (Exception e)
            {
                oleDbTransaction.Rollback();
                throw new Exception("Could not send query. \n" + e.Message);
            }

            //Read query result
            try
            {
                rowsAffected = dbAccessCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                oleDbTransaction.Rollback();
                throw new Exception("Could not read query result. \n" + e.Message);
            }

            return rowsAffected;
        }

        public int Execute(OleDbConnection connection, string query, OleDbParameter[] parametros, ref OleDbTransaction oleDbTransaction)
        {

            try
            {
                dbAccessCommand = new OleDbCommand(query, connection);

                // Insere comnado na transação
                dbAccessCommand.Transaction = oleDbTransaction;

                foreach (OleDbParameter p in parametros)
                    dbAccessCommand.Parameters.Add(p);

            }
            catch (Exception e)
            {
                oleDbTransaction.Rollback();
                throw new Exception("Could not send query -- E002 -- \n" + e.Message);
            }

            //Read query result
            try
            {
                rowsAffected = dbAccessCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                oleDbTransaction.Rollback();
                throw new Exception("Could not read query result. \n" + e.Message);
            }

            return rowsAffected;
        }

        public object ExecuteScalar(string query)
        {
            object retorno;

            //Connect to db
            try
            {
                dbAccessConn = new OleDbConnection(connString);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to database. \n" + e.Message);
            }

            //Send query
            try
            {
                dbAccessCommand = new OleDbCommand(query, dbAccessConn);

            }
            catch (Exception e)
            {
                throw new Exception("Could not send query. \n" + e.Message);
            }

            //Read query result
            try
            {
                dbAccessConn.Open();
                retorno = dbAccessCommand.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw new Exception("Could not read query result. \n" + e.Message);
            }
            finally
            {
                dbAccessConn.Close();
            }

            return retorno;
        }
    }
}