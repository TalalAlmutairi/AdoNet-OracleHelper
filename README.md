# AdoNet-OracleHelper
A class for applying database operations to Oracle such as query, executing statements, transactions, and returning multi tables.


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
    
Oracle DB script
      
    CREATE TABLE EMP_TS.EMPLOYEES
    (
      EMPID      INTEGER,
      FIRSTNAME  VARCHAR2(20 BYTE),
      LASTNAME   VARCHAR2(20 BYTE),
      AGE        INTEGER,
      COUNTRYID  INTEGER
    )
    TABLESPACE EMP_TS
    PCTUSED    0
    PCTFREE    10
    INITRANS   1
    MAXTRANS   255
    STORAGE    (
                PCTINCREASE      0
                BUFFER_POOL      DEFAULT
               )
    LOGGING 
    NOCOMPRESS 
    NOCACHE
    NOPARALLEL
    MONITORING;


    CREATE UNIQUE INDEX EMP_TS.EMPLOYEES_PK ON EMP_TS.EMPLOYEES
    (EMPID)
    LOGGING
    TABLESPACE TS
    PCTFREE    10
    INITRANS   2
    MAXTRANS   255
    STORAGE    (
                PCTINCREASE      0
                BUFFER_POOL      DEFAULT
               )
    NOPARALLEL;


    ALTER TABLE EMP_TS.EMPLOYEES ADD (
      CONSTRAINT EMPLOYEES_PK
     PRIMARY KEY
     (EMPID));



     CREATE TABLE EMP_TS.COUNTRY
    (
      COUNTRYID    INTEGER,
      COUNTRYDESC  VARCHAR2(20 BYTE)
    )
    TABLESPACE EMP_TS
    PCTUSED    0
    PCTFREE    10
    INITRANS   1
    MAXTRANS   255
    STORAGE    (
                PCTINCREASE      0
                BUFFER_POOL      DEFAULT
               )
    LOGGING 
    NOCOMPRESS 
    NOCACHE
    NOPARALLEL
    MONITORING;


    CREATE UNIQUE INDEX EMP_TS.COUNTRY_PK ON EMP_TS.COUNTRY
    (COUNTRYID)
    LOGGING
    TABLESPACE EMP_TS
    PCTFREE    10
    INITRANS   2
    MAXTRANS   255
    STORAGE    (
                PCTINCREASE      0
                BUFFER_POOL      DEFAULT
               )
    NOPARALLEL;


    Insert into COUNTRY
       (COUNTRYID, COUNTRYDESC)
     Values
       (1, 'Saudi Arabia');
    Insert into COUNTRY
       (COUNTRYID, COUNTRYDESC)
     Values
       (2, 'Kuwait');
    Insert into COUNTRY
       (COUNTRYID, COUNTRYDESC)
     Values
       (3, 'United Arab Emirates');


    CREATE SEQUENCE EMP_TS.SEQ_EMPID
    START WITH 1
    INCREMENT BY 1
    MINVALUE 0
    MAXVALUE 9999999
    NOCACHE 
    NOCYCLE 
    NOORDER 


    CREATE OR REPLACE PACKAGE EMP_TS.GET_MULTIPLE_TABLES AS
    TYPE refcur IS REF CURSOR;
    PROCEDURE GET_TABLES(CurEmp OUT GET_MULTIPLE_TABLES.refcur);
    END GET_MULTIPLE_TABLES;

    CREATE OR REPLACE PACKAGE BODY EMP_TS.GET_MULTIPLE_TABLES IS 
    PROCEDURE GET_TABLES(CurEmp OUT GET_MULTIPLE_TABLES.refcur,CurCountry OUT GET_MULTIPLE_TABLES.refcur) IS
    BEGIN
    OPEN CurEmp FOR SELECT * FROM Employees;
    OPEN CurCountry FOR SELECT * FROM Country;
    END GET_TABLES;
    END;
    /


    CREATE OR REPLACE PACKAGE EMP_TS.GET_ALL_EMPLOYEES AS
    TYPE refcur IS REF CURSOR;
    PROCEDURE GET_EMPLOYEES_INFO(CurEmp OUT GET_ALL_EMPLOYEES.refcur);
    END GET_ALL_EMPLOYEES;

    CREATE OR REPLACE PACKAGE BODY EMP_TS.GET_ALL_EMPLOYEES IS 
    PROCEDURE GET_EMPLOYEES_INFO(CurEmp OUT GET_ALL_EMPLOYEES.refcur) IS
    BEGIN
    OPEN CurEmp FOR SELECT * FROM Employees;
    END GET_EMPLOYEES_INFO;
    END;
    /
    
Example of using SqlHelper.cs

        string sql = "SELECT MAX(Age) FROM Employees";
        lbMsg.Text = OracleHelper.ExecuteScalar(sql, CommandType.Text, null);

      // Insert
      string sql = "INSERT INTO Employees VALUES(SEQ_EMPID.NEXTVAL,:pFName,:pLName,:pAge,:pCountryID)";
      OracleParameter[] parametersList = new OracleParameter[]{
                        new OracleParameter (":pFName",txtFName.Value),
                        new OracleParameter (":pLName",txtLName.Value),
                        new OracleParameter (":pAge",txtAge.Value),
                        new OracleParameter (":pCountryID",ddlCouontries.Value),
                   };

      if (OracleHelper.ExecuteNonQuery(sql, CommandType.Text, parametersList))
                        lbMsg.Text = "Inserted successfully";
      else
                        lbMsg.Text = "Error";
