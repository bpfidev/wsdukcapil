using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using DukcapilWebServices.Utility;

namespace DukcapilWebServices
{
    public class GeneralDAL
    {
        #region Insert
        public void Insert(string TableName, Hashtable parameters, ref int id)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_insert", parameters, ref id))
            {
                throw new Exception("Fail to execute xsp_" + TableName + "_insert", new Exception(dbw.DBErrorMessage));
            }
        }

        public void Insert(string TableName, Hashtable parameters, ref string id)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_insert", parameters, ref id))
            {
                throw new Exception("Fail to execute xsp_" + TableName + "_insert", new Exception(dbw.DBErrorMessage));
            }
        }

        public void Insert(string TableName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_insert", parameters))
            {
                throw new Exception("Fail to execute xsp_" + TableName + "_insert", new Exception(dbw.DBErrorMessage));
            }
        }

        public void Insert(string TableName, string SPName, Hashtable parameters, ref int id)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters, ref id))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
        }

        public void Insert(string TableName, string SPName, Hashtable parameters, ref string id)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters, ref id))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
        }

        public void Insert(string TableName, string SPName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
        }
        #endregion

        #region Update
        public void Update(string TableName, string SPName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
        }

        public void Update(string TableName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_update", parameters))
            {

                throw new Exception("Fail to execute xsp_" + TableName + "_update", new Exception(dbw.DBErrorMessage));
            }
        }
        #endregion

        #region Delete
        public void Delete(string TableName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_delete", parameters))
            {
                throw new Exception("Fail to execute xsp_" + TableName + "_delete", new Exception(dbw.DBErrorMessage));
            }
        }
        #endregion

        #region GetRows
        public DataTable GetRows(string TableName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            DataSet ds = new DataSet();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_getrows", parameters, ds))
            {
                throw new Exception("Fail to xsp_" + TableName + "_getrows", new Exception(dbw.DBErrorMessage));
            }
            else
            {
                if (ds.Tables.Count <= 0)
                    throw new Exception("Fail to xsp_" + TableName + "_getrows. No row found.");
                else
                    return ds.Tables[0];
            }
        }

        public DataTable GetRows(string TableName, string SPName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            DataSet ds = new DataSet();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters, ds))
            {
                throw new Exception("Fail to " + SPName, new Exception(dbw.DBErrorMessage));
            }
            else
            {
                if (ds.Tables.Count <= 0)
                    throw new Exception("Fail to " + SPName + ". No row found.");
                else
                    return ds.Tables[0];
            }
        }
        #endregion

        #region GetRow
        public DataRow GetRow(string TableName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            DataSet ds = new DataSet();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP("xsp_" + TableName + "_getrow", parameters, ds))
            {
                throw new Exception("Fail to execute xsp_" + TableName + "_getrow", new Exception(dbw.DBErrorMessage));
            }
            else
            {
                if (ds.Tables.Count <= 0)
                    throw new Exception("Fail to xsp_" + TableName + "_getrow. No row found.");
                else
                    return ds.Tables[0].Rows[0];
            }
        }

        public DataRow GetRow(string TableName, string SPName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            DataSet ds = new DataSet();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters, ds))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
            else
            {
                if (ds.Tables.Count <= 0)
                    throw new Exception("Fail to " + SPName + ". No row found.");
                else
                    return ds.Tables[0].Rows[0];
            }
        }
        #endregion

        public void ExecRawSP(string SPName, Hashtable parameters)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
        }

        public void ExecRawSP(string SPName, Hashtable parameters, ref string ReturnValue)
        {
            DBWrapper dbw = DBWrapper.GetSqlClientWrapper();
            dbw.ConnectionString = Shared.ConnectionString;
            if (!dbw.ExecuteSP(SPName, parameters, ref ReturnValue))
            {
                throw new Exception("Fail to execute " + SPName, new Exception(dbw.DBErrorMessage));
            }
        }
    }
}
