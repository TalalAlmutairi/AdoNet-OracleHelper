using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdoNetOracleHelper
{
    public partial class Examples : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        // Getting all employees data
        protected void btnLoadData_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Employees";
            GridView1.DataSource = OracleHelper.ExecuteQuery(sql, CommandType.Text, null);
            GridView1.DataBind();
        }

        //Selecting only the employee with id=2
        protected void btnSqlWhere_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Employees WHERE EmpID=:pID";

            // OracleParameter uses for security to prevent SQL injection
            //:pID in the SQL above must match the parameter in OracleParameter
            OracleParameter[] parametersList = new OracleParameter[]{
                new OracleParameter (":pID","2"), // you can read it from input or Textbox
           };

            GridView1.DataSource = OracleHelper.ExecuteQuery(sql, CommandType.Text, parametersList);
            GridView1.DataBind();
        }

        //Getting the maximum salary of all employees as only one value
        protected void btnExecuteScalar_Click(object sender, EventArgs e)
        {
            string sql = "SELECT MAX(Age) FROM Employees";
            lbMsg.Text = OracleHelper.ExecuteScalar(sql, CommandType.Text, null);

        }

        // Oracle PACKAGE

        //CREATE OR REPLACE PACKAGE EMP_TS.EMPLOYEES .GET_ALL_EMPLOYEES AS
        //TYPE refcur IS REF CURSOR;
        //PROCEDURE GET_EMPLOYEES_INFO(CurEmp OUT GET_ALL_EMPLOYEES.refcur);
        //END GET_ALL_EMPLOYEES;
        ///

        //CREATE OR REPLACE PACKAGE BODY EMP_TS.EMPLOYEES .GET_ALL_EMPLOYEES IS
        //PROCEDURE GET_EMPLOYEES_INFO(CurEmp OUT GET_ALL_EMPLOYEES.refcur) IS
        //BEGIN
        //OPEN CurEmp FOR SELECT * FROM Employees;
        //END GET_EMPLOYEES_INFO;
        //END;
        ///

        //Execute stored procedure
        protected void btnSP_Click(object sender, EventArgs e)
        {
            string PKG_SP_Name = "GET_ALL_EMPLOYEES.GET_EMPLOYEES_INFO";
            OracleParameter[] parametersList = new OracleParameter[]{
                new OracleParameter ("CurEmp",OracleDbType.RefCursor,ParameterDirection.Output),
           };

            GridView1.DataSource = OracleHelper.ExecuteQuery(PKG_SP_Name, CommandType.StoredProcedure, parametersList);
            GridView1.DataBind();
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            // Id = SEQ_EMPID.NEXTVAL
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
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string sql = "UPDATE Employees SET FirstName=:pFName,LastName=:pLName,Age=:pAge,CountryID=:pCountryID WHERE EmpID =:pID";
            OracleParameter[] parametersList = new OracleParameter[]{
                new OracleParameter (":pID",txtEmpID.Value),
                new OracleParameter (":pFName",txtFName.Value),
                new OracleParameter (":pLName",txtLName.Value),
                new OracleParameter (":pAge",txtAge.Value),
                new OracleParameter (":pCountryID",ddlCouontries.Value),
           };

            if (OracleHelper.ExecuteNonQuery(sql, CommandType.Text, parametersList))
                lbMsg.Text = "Updated successfully";
            else
                lbMsg.Text = "Error";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string sql = "DELETE FROM Employees WHERE EmpID =:pID";

            OracleParameter[] parametersList = new OracleParameter[]{
                    new OracleParameter (":pID",txtEmpID.Value),
                };

            if (OracleHelper.ExecuteNonQuery(sql, CommandType.Text, parametersList))
                lbMsg.Text = "Deleted successfully";
            else
                lbMsg.Text = "Error";
        }

        //'Execute two SQL statements Insert and update, which all SQL statements in a single transaction, rolling back if an error has occurred
        protected void btnExecuteTransaction_Click(object sender, EventArgs e)
        {
            ArrayList listOfSQLs = new ArrayList();
            List<OracleParameter[]> listOfParamerters = new List<OracleParameter[]>();

            string sql1 = "INSERT INTO Employees VALUES(SEQ_EMPID.NEXTVAL,:pFName,:pLName,:pAge,:pCountryID)";
            OracleParameter[] parameters1 = new OracleParameter[]{
                new OracleParameter (":pFName","Test F Name"),
                new OracleParameter (":pLName","Test L Name"),
                new OracleParameter (":pAge",25),
                new OracleParameter (":pCountryID",1),
           };

            listOfSQLs.Add(sql1);
            listOfParamerters.Add(parameters1);


            string sql2 = "UPDATE Employees SET FirstName=:pFName,LastName=:pLName,Age=:pAge,CountryID=:pCountryID WHERE EmpID =:pID";
            OracleParameter[] parameters2 = new OracleParameter[]{                
                new OracleParameter (":pFName","New F Name"),
                new OracleParameter (":pLName","New L Name"),
                new OracleParameter (":pAge",30),
                new OracleParameter (":pCountryID",2),
                new OracleParameter (":pID",4), // This number for testing, make sure a record with id=4 is exist
           };


            listOfSQLs.Add(sql2);
            listOfParamerters.Add(parameters2);

            if (OracleHelper.ExecuteTransaction(listOfSQLs, listOfParamerters))
                lbMsg.Text = "All SQL statements executed successfully";
            else
                lbMsg.Text = "Error";

        }


        // Execute two select queries, and returns employees and country tables
        protected void btnReturnDS_Click(object sender, EventArgs e)
        {
            // make sure parameter name of Cursor (CurEmp and CurCountry ) same as the name in the oracle PACKAGE 
            string PKG_SP_Name = "GET_MULTIPLE_TABLES.GET_TABLES";
            OracleParameter[] parametersList = new OracleParameter[]{
                new OracleParameter ("CurEmp",OracleDbType.RefCursor,ParameterDirection.Output),
                new OracleParameter ("CurCountry",OracleDbType.RefCursor,ParameterDirection.Output),
           };
            DataSet ds = OracleHelper.ExecuteQueryDS(PKG_SP_Name, CommandType.StoredProcedure, parametersList);
            GridViewEmp.DataSource = ds.Tables[0];
            GridViewEmp.DataBind();

            GridViewCountry.DataSource = ds.Tables[1];
            GridViewCountry.DataBind();
        }
    }
}