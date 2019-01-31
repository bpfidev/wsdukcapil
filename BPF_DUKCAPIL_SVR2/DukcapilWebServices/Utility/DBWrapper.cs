using System;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;

using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Collections;
using System.Web.Configuration;

namespace DukcapilWebServices.Utility
{
    public class DBWrapper
    {
        #region constructors / finalizer methods

        public enum PROVIDER_TYPE { PROVIDER_OLEDB, PROVIDER_SQLCLIENT, PROVIDER_ODBC, /*PROVIDER_ORACLE,*/ PROVIDER_OTHER, PROVIDER_NONE }

        // Do not allow constructing the object directly
        protected DBWrapper()
        {
            m_oConnection = null;
            m_oCommand = null;
            m_oTransaction = null;
            m_sConnectionString = null;
            m_nCommandTimeout = 0;
            m_nRetryConnect = 3;
            m_bDisposed = false;
            m_bConnected = false;
            m_sProviderAssembly = null;
            m_sProviderConnectionClass = null;
            m_sProviderCommandBuilderClass = null;
            m_eProvider = PROVIDER_TYPE.PROVIDER_NONE;
            m_sDBErrorMessage = null;
        }

        ~DBWrapper()
        {
            this.Dispose(false);
        }

        #endregion

        #region private/protected members

        protected System.Data.IDbConnection m_oConnection;
        protected System.Data.IDbCommand m_oCommand;
        protected System.Data.IDbTransaction m_oTransaction;
        protected string m_sConnectionString;
        protected int m_nCommandTimeout;
        protected int m_nRetryConnect;
        protected bool m_bDisposed;
        protected bool m_bConnected;
        protected string m_sProviderAssembly;
        protected string m_sProviderConnectionClass;
        protected string m_sProviderCommandBuilderClass;
        protected PROVIDER_TYPE m_eProvider;
        protected string m_sDBErrorMessage;

        #endregion

        #region Databaes connect / DisConnect / Transaction methods
        protected bool ValidateConnection()
        {
            if (m_bConnected)
                return true;
            else
                return Connect();
        }

        protected bool Connect()
        {
            // Check for valid connection string
            if (m_sConnectionString == null || m_sConnectionString.Length == 0)
                throw (new Exception("Invalid database connection string"));

            // Disconnect if already connected
            Disconnect();

            // Get ADONET connection object
            m_oConnection = GetConnection();
            m_oConnection.ConnectionString = this.ConnectionString;

            // Implement connection retries
            for (int i = 0; i <= m_nRetryConnect; i++)
            {
                try
                {
                    m_oConnection.Open();

                    if (m_oConnection.State == ConnectionState.Open)
                    {
                        m_bConnected = true;
                        break;
                    }
                }
                catch
                {
                    if (i == m_nRetryConnect)
                        throw;

                    // Wait for 1 second and try again
                    Thread.Sleep(1000);
                }
            }

            // Get command object
            m_oCommand = m_oConnection.CreateCommand();
            m_oCommand.CommandTimeout = m_nCommandTimeout;

            return m_bConnected;
        }

        protected void Disconnect()
        {
            // Disconnect can be called from Dispose and should guarantee no errors
            if (!m_bConnected)
                return;

            if (m_oTransaction != null)
                RollbackTransaction(false);

            if (m_oCommand != null)
            {
                m_oCommand.Dispose();
                m_oCommand = null;
            }

            if (m_oConnection != null)
            {
                try
                {
                    m_oConnection.Close();
                }
                catch
                {
                }
                m_oConnection.Dispose();
                m_oConnection = null;
            }

            m_bConnected = false;
        }

        protected void BeginTransaction()
        {
            ValidateConnection();

            m_oTransaction = m_oConnection.BeginTransaction();
            m_oCommand.Transaction = m_oTransaction;

            return;
        }

        protected void CommitTransaction()
        {
            if (m_oTransaction == null)
                throw (new Exception("BeginTransaction must be called before commit or rollback. No open transactions found"));

            m_oTransaction.Commit();
            m_oTransaction.Dispose();
            m_oTransaction = null;
        }

        protected void RollbackTransaction()
        {
            RollbackTransaction(true);
        }

        protected void RollbackTransaction(bool bThrowError)
        {
            if (m_oTransaction == null)
            {
                if (bThrowError)
                    throw (new Exception("BeginTransaction must be called before commit or rollback. No open transactions found"));
            }

            try
            {
                m_oTransaction.Rollback();
            }
            catch
            {
                if (bThrowError)
                    throw;
            }
            finally
            {
                if (m_oTransaction != null)
                    m_oTransaction.Dispose();
                m_oTransaction = null;
            }
        }

        #endregion

        #region Wraper methods for ADO.NET

        public IDataReader ExecuteReader(string sSQL)
        {
            return this.ExecuteReader(sSQL, CommandType.Text);
        }

        public IDataReader ExecuteReader(string sSQL, CommandType oType)
        {
            ValidateConnection();

            m_oCommand.CommandText = sSQL;
            m_oCommand.CommandType = oType;

            return m_oCommand.ExecuteReader();

        }

        public DataSet GetDataSet(string sSQL)
        {
            DataSet oData = new DataSet();
            return GetDataSet(sSQL, CommandType.Text, oData);
        }

        public DataSet GetDataSet(string sSQL, CommandType oType)
        {
            DataSet oData = new DataSet();
            return GetDataSet(sSQL, oType, oData);
        }

        public DataSet GetDataSet(string sSQL, CommandType oType, DataSet oData)
        {
            ValidateConnection();

            m_oCommand.CommandType = oType;
            m_oCommand.CommandText = sSQL;

            IDataAdapter oAdpt = GetDataAdapter(sSQL);
            oAdpt.Fill(oData);
            Disconnect();

            return oData;
        }

        public object ExecuteScalar(string sSQL)
        {
            return ExecuteScalar(sSQL, CommandType.Text);
        }

        public object ExecuteScalar(string sSQL, CommandType oType)
        {
            object o;
            ValidateConnection();

            m_oCommand.CommandText = sSQL;
            m_oCommand.CommandType = oType;
            o = m_oCommand.ExecuteScalar();
            Disconnect();

            return o;
        }

        public object ExecuteNonQuery(string sSQL)
        {
            return ExecuteNonQuery(sSQL, CommandType.Text);
        }

        public object ExecuteNonQuery(string sSQL, CommandType oType)
        {
            object o;
            ValidateConnection();

            m_oCommand.CommandText = sSQL;
            m_oCommand.CommandType = oType;
            o = m_oCommand.ExecuteNonQuery();
            Disconnect();

            return o;
        }

        public bool ExecuteSP(string sSP, Hashtable parameters)
        {
            bool IsSuccess = true;

            try
            {
                BeginTransaction();

                IDataParameterCollection dpc = DeriveParameters(sSP, CommandType.StoredProcedure);

                if (parameters != null)
                {
                    for (int i = 0; i < dpc.Count; i++)
                    {
                        SqlParameter param = (SqlParameter)dpc[i];
                        if (param.Direction == System.Data.ParameterDirection.Input)
                            ((SqlParameter)dpc[i]).Value = parameters[((SqlParameter)dpc[i]).ParameterName.Substring(1)];
                    }
                }

                m_oCommand.ExecuteNonQuery();

                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                IsSuccess = false;
                m_sDBErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    Disconnect();
                }
                catch (Exception)
                {
                    //
                }
            }

            return IsSuccess;
        }

        public bool ExecuteSPWithoutBeginTrancaction(string sSP, Hashtable parameters)
        {
            bool IsSuccess = true;

            try
            {
                ValidateConnection();

                IDataParameterCollection dpc = DeriveParameters(sSP, CommandType.StoredProcedure);

                if (parameters != null)
                {
                    for (int i = 0; i < dpc.Count; i++)
                    {
                        SqlParameter param = (SqlParameter)dpc[i];
                        if (param.Direction == System.Data.ParameterDirection.Input)
                            ((SqlParameter)dpc[i]).Value = parameters[((SqlParameter)dpc[i]).ParameterName.Substring(1)];
                    }
                }

                m_oCommand.ExecuteNonQuery();
                //CommitTransaction();

            }
            catch (Exception ex)
            {
                //RollbackTransaction();
                IsSuccess = false;
                m_sDBErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    Disconnect();
                }
                catch (Exception)
                {
                    //
                }
            }

            return IsSuccess;
        }

        public bool ExecuteSP(string sSP, Hashtable parameters, DataSet ds)
        {
            bool IsSuccess = true;

            try
            {
                BeginTransaction();

                IDataParameterCollection dpc = DeriveParameters(sSP, CommandType.StoredProcedure);

                if (parameters != null)
                {
                    for (int i = 0; i < dpc.Count; i++)
                    {

                        SqlParameter param = (SqlParameter)dpc[i];
                        if (param.Direction == System.Data.ParameterDirection.Input)
                            ((SqlParameter)dpc[i]).Value = parameters[((SqlParameter)dpc[i]).ParameterName.Substring(1)];

                    }
                }

                IDataAdapter adp = GetDataAdapter(sSP);
                adp.Fill(ds);

                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                IsSuccess = false;
                m_sDBErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    Disconnect();
                }
                catch (Exception)
                {
                    //
                }
            }

            return IsSuccess;
        }

        public bool ExecuteSP(string sSP, Hashtable parameters, ref string returnvalue)
        {
            bool IsSuccess = true;

            try
            {
                BeginTransaction();

                IDataParameterCollection dpc = DeriveParameters(sSP, CommandType.StoredProcedure);

                if (parameters != null)
                {
                    for (int i = 0; i < dpc.Count; i++)
                    {
                        SqlParameter param = (SqlParameter)dpc[i];
                        if (param.Direction == System.Data.ParameterDirection.Input)
                            ((SqlParameter)dpc[i]).Value = parameters[((SqlParameter)dpc[i]).ParameterName.Substring(1)];
                        else if (param.Direction == System.Data.ParameterDirection.InputOutput)
                            ((SqlParameter)dpc[i]).Value = returnvalue;
                    }
                }

                m_oCommand.ExecuteNonQuery();

                //get the treturn value
                returnvalue = ((SqlParameter)m_oCommand.Parameters[1]).Value.ToString();

                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                IsSuccess = false;
                m_sDBErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    Disconnect();
                }
                catch (Exception)
                {
                    //
                }
            }

            return IsSuccess;
        }

        public bool ExecuteSP(string sSP, Hashtable parameters, ref int returnvalue)
        {
            bool IsSuccess = true;

            try
            {
                BeginTransaction();

                IDataParameterCollection dpc = DeriveParameters(sSP, CommandType.StoredProcedure);

                if (parameters != null)
                {
                    for (int i = 0; i < dpc.Count; i++)
                    {
                        SqlParameter param = (SqlParameter)dpc[i];
                        if (param.Direction == System.Data.ParameterDirection.Input)
                            ((SqlParameter)dpc[i]).Value = parameters[((SqlParameter)dpc[i]).ParameterName.Substring(1)];
                        else if (param.Direction == System.Data.ParameterDirection.InputOutput)
                            ((SqlParameter)dpc[i]).Value = returnvalue;
                    }
                }

                m_oCommand.ExecuteNonQuery();

                //get the treturn value
                returnvalue = (int)((SqlParameter)m_oCommand.Parameters[1]).Value;

                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                IsSuccess = false;
                m_sDBErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    Disconnect();
                }
                catch (Exception)
                {
                    //
                }
            }

            return IsSuccess;
        }




        protected IDataParameterCollection GetParameters()
        {
            ValidateConnection();

            return m_oCommand.Parameters;
        }

        protected void AddParameter(IDataParameter oParam)
        {
            ValidateConnection();

            m_oCommand.Parameters.Add(oParam);
        }

        protected void ClearParameters()
        {
            if (m_oCommand != null)
                m_oCommand.Parameters.Clear();
        }

        #endregion

        #region DBWrapper Create methods for various providers

        public static DBWrapper GetOleDbWrapper()
        {
            DBWrapper oDB = new DBWrapper();
            oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_OLEDB;

            return oDB;
        }

        public static DBWrapper GetSqlClientWrapper()
        {
            DBWrapper oDB = new DBWrapper();
            oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_SQLCLIENT;

            return oDB;
        }

        public static DBWrapper GetOdbcWrapper()
        {
            DBWrapper oDB = new DBWrapper();
            oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_ODBC;

            return oDB;
        }

        //public static DBWrapper GetOracleWrapper()
        //{
        //    DBWrapper oDB = new DBWrapper();
        //    oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_ORACLE;

        //    return oDB;
        //}

        public static DBWrapper GetADONETWrapper()
        {
            DBWrapper oDB = new DBWrapper();
            oDB.ProviderAssemblyName = WebConfigurationManager.AppSettings["ADONET_ASSEMBLY"];
            oDB.ProviderConnectionClassName = WebConfigurationManager.AppSettings["ADONET_CONNECTION_CLASS"];
            oDB.ProviderCommandBuilderClassName = WebConfigurationManager.AppSettings["ADONET_COMMANDBUILDER_CLASS"];
            oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_OTHER;

            return oDB;
        }

        public static DBWrapper GetADONETWrapper(string sProviderName)
        {
            DBWrapper oDB = null;
            switch (sProviderName.Trim().ToUpper())
            {
                case "OLEDB":
                    oDB = DBWrapper.GetOleDbWrapper();
                    break;

                case "SQLCLIENT":
                    oDB = DBWrapper.GetSqlClientWrapper();
                    break;

                case "ODBC":
                    oDB = DBWrapper.GetOdbcWrapper();
                    break;

                //case "ORACLECLIENT":
                //    oDB = DBWrapper.GetOracleWrapper();
                //    break;

                default:
                    oDB = DBWrapper.GetADONETWrapper();
                    break;
            }

            return oDB;
        }

        public static DBWrapper GetADONETWrapper(string sProviderAssembly, string sConnectionClass, string sCommandBuilderClass)
        {
            DBWrapper oDB = new DBWrapper();
            oDB.ProviderAssemblyName = sProviderAssembly;
            oDB.ProviderConnectionClassName = sConnectionClass;
            oDB.ProviderCommandBuilderClassName = sCommandBuilderClass;
            oDB.m_eProvider = PROVIDER_TYPE.PROVIDER_OTHER;

            return oDB;
        }

        #endregion

        #region Implementation of IDisposable
        public void Dispose()
        {
            Dispose(true);
        }

        // Following is not IDisposable interface method. But added in this 
        // region/section as it is more related to Dispose
        protected void Dispose(bool bDisposing)
        {
            if (!m_bDisposed)
            {
                // Dispose in thie block, only managed resources
                if (bDisposing)
                {
                }

                // Free only un-managed resources here

            }

            m_bDisposed = true;
        }

        #endregion

        #region properties (get/set methods)

        public string ConnectionString
        {
            get
            {
                return m_sConnectionString;
            }
            set
            {
                m_sConnectionString = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return m_nCommandTimeout;
            }
            set
            {
                m_nCommandTimeout = value;
            }
        }

        public int ConnectionRetryCount
        {
            get
            {
                return m_nRetryConnect;
            }
            set
            {
                m_nRetryConnect = value;

                if (m_nRetryConnect <= 0)
                    m_nRetryConnect = 0;
            }
        }

        public string ProviderAssemblyName
        {
            get
            {
                return m_sProviderAssembly;
            }
            set
            {
                m_sProviderAssembly = value;
            }
        }

        public string ProviderConnectionClassName
        {
            get
            {
                return m_sProviderConnectionClass;
            }
            set
            {
                m_sProviderConnectionClass = value;
            }
        }

        public string ProviderCommandBuilderClassName
        {
            get
            {
                return m_sProviderCommandBuilderClass;
            }
            set
            {
                m_sProviderCommandBuilderClass = value;
            }
        }

        public string DBErrorMessage
        {
            get
            {
                return m_sDBErrorMessage;
            }
        }

        public PROVIDER_TYPE PROVIDER
        {
            get
            {
                return m_eProvider;
            }
        }

        #endregion

        #region Utility functions

        protected IDbConnection GetConnection()
        {
            IDbConnection oReturn = null;

            switch (this.PROVIDER)
            {
                case PROVIDER_TYPE.PROVIDER_SQLCLIENT:
                    oReturn = new SqlConnection();
                    break;

                case PROVIDER_TYPE.PROVIDER_OLEDB:
                    oReturn = new OleDbConnection();
                    break;

                case PROVIDER_TYPE.PROVIDER_ODBC:
                    oReturn = new OdbcConnection();
                    break;

                //case PROVIDER_TYPE.PROVIDER_ORACLE:
                //    oReturn = new OracleConnection();
                //    break;

                case PROVIDER_TYPE.PROVIDER_OTHER:
                    oReturn = (IDbConnection)GetADONETProviderObject(ProviderAssemblyName, ProviderCommandBuilderClassName, null);
                    break;

                default:
                    throw (new Exception("Invalid provider type"));
            }

            if (oReturn == null)
                throw (new Exception("Failed to get ADONET Connection object [IDbConnection]"));

            return oReturn;

        }


        protected IDataAdapter GetDataAdapter(string sSQL)
        {
            IDataAdapter oReturn = null;

            switch (this.PROVIDER)
            {
                case PROVIDER_TYPE.PROVIDER_SQLCLIENT:
                    oReturn = new SqlDataAdapter((SqlCommand)m_oCommand);
                    break;

                case PROVIDER_TYPE.PROVIDER_OLEDB:
                    oReturn = new OleDbDataAdapter((OleDbCommand)m_oCommand);
                    break;

                case PROVIDER_TYPE.PROVIDER_ODBC:
                    oReturn = new OdbcDataAdapter((OdbcCommand)m_oCommand);
                    break;

                //case PROVIDER_TYPE.PROVIDER_ORACLE:
                //    oReturn = new OracleDataAdapter((OracleCommand)m_oCommand);
                //    break;

                case PROVIDER_TYPE.PROVIDER_OTHER:
                    {
                        object[] oArgs = new Object[2];
                        oArgs[0] = sSQL;
                        oArgs[1] = m_oConnection;

                        oReturn = (IDbDataAdapter)GetADONETProviderObject(ProviderAssemblyName, ProviderCommandBuilderClassName, oArgs);
                        break;
                    }

                default:
                    throw (new Exception("Invalid provider type"));
            }

            if (oReturn == null)
                throw (new Exception("Failed to get ADONET Data Adapter object [IDataAdapter]"));

            return oReturn;

        }

        protected IDataParameterCollection DeriveParameters(string sStoredProcedure)
        {
            return DeriveParameters(sStoredProcedure, CommandType.StoredProcedure);
        }

        protected IDataParameterCollection DeriveParameters(string sSql, CommandType oType)
        {
            ValidateConnection();

            ClearParameters();

            m_oCommand.CommandText = sSql;
            m_oCommand.CommandType = oType;

            switch (this.PROVIDER)
            {
                case PROVIDER_TYPE.PROVIDER_SQLCLIENT:
                    SqlCommandBuilder.DeriveParameters((SqlCommand)m_oCommand);
                    break;

                case PROVIDER_TYPE.PROVIDER_OLEDB:
                    OleDbCommandBuilder.DeriveParameters((OleDbCommand)m_oCommand);
                    break;

                case PROVIDER_TYPE.PROVIDER_ODBC:
                    OdbcCommandBuilder.DeriveParameters((OdbcCommand)m_oCommand);
                    break;

                //case PROVIDER_TYPE.PROVIDER_ORACLE:
                //    OracleCommandBuilder.DeriveParameters((OracleCommand)m_oCommand);
                //    break;

                case PROVIDER_TYPE.PROVIDER_OTHER:
                    {
                        Type oCmdBuilderType = Type.GetType(ProviderCommandBuilderClassName);

                        MethodInfo oMth = oCmdBuilderType.GetMethod("DeriveParameters");

                        if (oMth == null)
                            throw (new Exception("DeriveParameters method is not suppored by the selected provider"));

                        object[] oParams = new Object[1];
                        oParams[0] = m_oCommand;

                        // DeriveParameters is static method
                        oMth.Invoke(null, oParams);

                        break;
                    }

                default:
                    throw (new Exception("Invalid provider type"));
            }

            return m_oCommand.Parameters;
        }

        protected object GetADONETProviderObject(string sAssembly, string sClass, object[] oArgs)
        {
            if (sAssembly == null || sAssembly.Trim().Length == 0)
                throw (new Exception("Invalid provider assembly name"));

            if (sClass == null || sClass.Trim().Length == 0)
                throw (new Exception("Invalid provider connection class name"));

            Assembly oSrc = Assembly.Load(sAssembly); //Assembly.LoadWithPartialName(sAssembly);
            if (oArgs == null)
            {
                return oSrc.CreateInstance(sClass, true);
            }
            else
            {
                Type oType = oSrc.GetType(sClass, true, true);
                Type[] arTypes = new Type[oArgs.Length];
                for (int i = 0; i < oArgs.Length; i++)
                {
                    arTypes[i] = oArgs[0].GetType();
                }
                ConstructorInfo oConstr = oType.GetConstructor(arTypes);

                return oConstr.Invoke(oArgs);
            }
        }

        #endregion
    }
}