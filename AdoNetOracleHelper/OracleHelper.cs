using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace AdoNetOracleHelper
{
    public class OracleHelper
    {
        // Required
        // Install Oracle.ManagedDataAccess From Tool -> NuGet Package


        // You need to change connection string from Web.config
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DbConnecion"].ConnectionString;
        /// <summary>
        /// Execute Select query and return results as a DataTable
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteQuery(string cmdText, CommandType cmdType, OracleParameter[] parameters)
        {
            DataTable table = new DataTable();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(cmdText, con))
                    {
                        con.Open();
                        cmd.CommandType = cmdType;
                        cmd.BindByName = true;
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                // Save ex to logger
                string error = ex.Message;

                return null;
            }
            return table;
        }
        /// <summary>
        ///  Executes a SQL statement and returns the number of rows affected. NonQuery (Insert, update, and delete)
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>bool</returns>
        public static bool ExecuteNonQuery(string cmdText, CommandType cmdType, OracleParameter[] parameters)
        {
            var value = 0;
            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(cmdText, con))
                    {
                        con.Open();
                        cmd.CommandType = cmdType;
                        cmd.BindByName = true;
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        value = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Save ex to logger
                string error = ex.Message;
                return false;
            }
            if (value < 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query.
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>string</returns>
        public static string ExecuteScalar(string cmdText, CommandType cmdType, OracleParameter[] parameters)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(cmdText, con))
                    {
                        con.Open();
                        cmd.CommandType = cmdType;
                        cmd.BindByName = true;
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        string value = cmd.ExecuteScalar().ToString();

                        return value;
                    }
                }
            }
            catch (Exception ex)
            {
                // Save ex to logger
                string error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Executes the query, and returns multiple tables
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteQueryDS(string cmdText, CommandType cmdType, OracleParameter[] parameters)
        {
            DataSet tables = new DataSet();
            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(cmdText, con))
                    {
                        con.Open();
                        cmd.CommandType = cmdType;
                        cmd.BindByName = true;
                        
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        adapter.Fill(tables);
                    }
                }
            }
            catch (Exception ex)
            {
                // Save ex to logger
                string error = ex.Message;
                return null;
            }
            return tables;
        }
        /// <summary>
        /// Execute multiple SQL statements such as Insert, update, and delete, which all SQL statements in a single transaction, rolling back if an error has occurred
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="paramerters"></param>
        /// <returns>bool</returns>
        public static bool ExecuteTransaction(ArrayList cmdText, List<OracleParameter[]> paramerters)
        {

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                OracleTransaction transaction = null;
                cmd.Connection = con;

                try
                {
                    con.Open();

                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                    // Assign transaction object for a pending local transaction.
                    cmd.Connection = con;
                    cmd.Transaction = transaction;
                    cmd.BindByName = true;
                    for (int i = 0; i < cmdText.Count; i++)
                    {
                        cmd.CommandText = cmdText[i].ToString();
                        if (paramerters[i].Length > 0)
                            cmd.Parameters.AddRange(paramerters[i]);

                        int flag = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        if (flag < 1)
                            return false;
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Save ex to logger
                    string error = ex.Message;
                    return false;
                }
            }
        }
    }
}