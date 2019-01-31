using DukcapilWebServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public class Shared
{
    public Shared()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public static Control FindControlRecursive(Control control, string id)
    {
        if (control == null) return null;
        //try to find the control at the current level
        Control ctrl = control.FindControl(id);

        if (ctrl == null)
        {
            //search the children
            foreach (Control child in control.Controls)
            {
                ctrl = FindControlRecursive(child, id);

                if (ctrl != null) break;
            }
        }
        return ctrl;
    }


    public static string CurrentUID
    {
        get
        {
            return ((DataRow)HttpContext.Current.Session[SessionKey.CURRENT_USER_SESSION_KEY])["ID"].ToString();
        }
    }

    public static string CurrentEmpName
    {
        get
        {
            return ((DataRow)HttpContext.Current.Session[SessionKey.CURRENT_USER_SESSION_KEY])["EMP_NAME"].ToString();
        }
    }

    public static string CurrentUserType
    {
        get
        {
            return ((DataRow)HttpContext.Current.Session[SessionKey.CURRENT_USER_SESSION_KEY])["USER_TYPE"].ToString();
        }
    }

    public static bool IsAdmin
    {
        get
        {
            if (((DataRow)HttpContext.Current.Session[SessionKey.CURRENT_USER_SESSION_KEY])["IS_ADMIN"].ToString() == "0")
                return false;
            else
                return true;
        }
    }

    public static string CurrentIPAddress
    {
        get
        {
            return HttpContext.Current.Session[SessionKey.CURRENT_USER_IP_ADDRESS_SESSION_KEY].ToString();
        }
    }

    public static string DefaultErrorMessage
    {
        get { return "This is strange! Something is not right with the system. Please check the tehnical error message below."; }
    }

    public static string DefaultSuccessMessage
    {
        get { return "Your data is at the safe place now"; }
    }

    public static string DefaultErrorDukcapil
    {
        get { return "Combination between your username, password and ip please contact support"; }
    }

    public static string DefaultExitingError
    {
        get { return "NIK Is already exis"; }
    }

    public static string DefaultNotFoundError
    {
        get { return "NIK Not Found."; }
    }

    public static string DefaultLengthError
    {
        get { return "Legth is not 16 character"; }
    }

    public static string DefaultSuccessTitle
    {
        get { return "Success"; }
    }

    public static string DefaultErrorTitle
    {
        get { return "Error"; }
    }

    public static void ApplyDefaultProp(Hashtable ht)
    {
        ht["p_cre_date"] = ht["p_mod_date"] = DateTime.Now;
        ht["p_cre_by"] = ht["p_mod_by"] = CurrentUID;
        ht["p_cre_ip_address"] = ht["p_mod_ip_address"] = CurrentIPAddress;
    }

    public static void ShowSuccessGritter(Page p, string NextURL)
    {
        ScriptManager.RegisterStartupScript(p, p.GetType(), "fy", String.Format("fnShowGritter('{0}', '{1}'); location.href='{2}';", Shared.DefaultSuccessTitle, Shared.DefaultSuccessMessage, NextURL), true);
    }

    public static void ShowDukcapilError(Page p)
    {
        ScriptManager.RegisterStartupScript(p, p.GetType(), "fy", String.Format("fnShowErrorNotif('{0}', '{1}');", Shared.DefaultErrorTitle, Shared.DefaultErrorDukcapil), true);
    }

    public static void ShowExitingError(Page p)
    {
        ScriptManager.RegisterStartupScript(p, p.GetType(), "fy", String.Format("fnShowErrorNotif('{0}', '{1}');", Shared.DefaultErrorTitle, Shared.DefaultExitingError), true);
    }

    public static void ShowNotFoundError(Page p)
    {
        ScriptManager.RegisterStartupScript(p, p.GetType(), "fy", String.Format("fnShowErrorNotif('{0}', '{1}');", Shared.DefaultErrorTitle, Shared.DefaultNotFoundError), true);
    }

    public static void ShowLengthError(Page p)
    {
        ScriptManager.RegisterStartupScript(p, p.GetType(), "fy", "fnShowErrorNotif('" + Shared.DefaultErrorMessage + "','" + Shared.DefaultLengthError + "');", true);
    }

    public static void ShowErrorDialog(Page p, Exception ex)
    {
        ScriptManager.RegisterStartupScript(p, p.GetType(), "fy", "fnShowErrorNotif('" + Shared.DefaultErrorMessage + "', '" + Shared.MakeSingleLine(ex) + "');", true);
    }


    public static void BindGeneralCode(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("MASTER_GENERAL_CODE", _ht);
            ddl.DataTextField = "DESCRIPTION";
            ddl.DataValueField = "CODE";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }

    public static void BindingQuickView(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("", "xsp_collection_main_quickview", _ht); //_dal.GetRows("COLLECTION_MAIN", _ht);
            ddl.DataTextField = "collector";
            ddl.DataValueField = "collector_code";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }

    public static void BindGeneralSubCode(DropDownList ddl, string GeneralCode)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";
            _ht["p_general_code"] = GeneralCode;

            ddl.DataSource = _dal.GetRows("MASTER_GENERAL_SUBCODE", _ht);
            ddl.DataTextField = "DESCRIPTION";
            ddl.DataValueField = "CODE";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }

    public static void BindBranch(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("MASTER_BRANCH", _ht);
            ddl.DataTextField = "DESCRIPTION";
            ddl.DataValueField = "CODE";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }

    public static void BindEmployee(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("EMPLOYEE_MAIN", _ht);
            ddl.DataTextField = "EMP_NAME";
            ddl.DataValueField = "EMP_CODE";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }
    public static void BindRole(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("MASTER_ROLE_SEC", _ht);
            ddl.DataTextField = "NAME";
            ddl.DataValueField = "CODE";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }
    public static void BindSurveyMain(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("SURVEY_MAIN", _ht);
            ddl.DataTextField = "TRX_NO";
            ddl.DataValueField = "TRX_NO";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }
    public static void BindDevice(DropDownList ddl)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";

            ddl.DataSource = _dal.GetRows("MASTER_MEREK", _ht);
            ddl.DataTextField = "DESCRIPTION";
            ddl.DataValueField = "CODE";
            ddl.DataBind();
        }
        catch (Exception ex)
        {
        }
    }


    private static HorizontalAlign ConvertToHorizontalAlignEnum(string s)
    {
        if (s.Equals("Left"))
            return HorizontalAlign.Left;
        else if (s.Equals("Right"))
            return HorizontalAlign.Right;
        else
            return HorizontalAlign.Center;
    }

    public static void BindLookUp(GridView gvw, string LookUpCode, ref string SPName)
    {
        string sSPName = "";

        DataTable dt = GetLookUp(LookUpCode, ref sSPName);
        ArrayList alDataKeyNames = new ArrayList();
        SPName = sSPName;
        gvw.Columns.Clear();

        if (dt != null)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["IS_DATAKEY"].ToString().Equals("0"))
                {
                    if (dr["IS_VISIBLE"].ToString().Equals("1"))
                        gvw.Columns.Add(new BoundField { DataField = dr["FIELD_NAME"].ToString(), HeaderText = dr["HEADER_NAME"].ToString(), DataFormatString = string.Format("{0}", dr["FORMAT"]), ItemStyle = { Width = Unit.Percentage(Int32.Parse(dr["WIDTH_PCT"].ToString())), HorizontalAlign = ConvertToHorizontalAlignEnum(dr["ALIGNMENT"].ToString()) } });
                }
                else
                {
                    if (dr["IS_VISIBLE"].ToString().Equals("1"))
                        gvw.Columns.Add(new BoundField { DataField = dr["FIELD_NAME"].ToString(), HeaderText = dr["HEADER_NAME"].ToString(), DataFormatString = string.Format("{0}", dr["FORMAT"]), ItemStyle = { Width = Unit.Percentage(Int32.Parse(dr["WIDTH_PCT"].ToString())), HorizontalAlign = ConvertToHorizontalAlignEnum(dr["ALIGNMENT"].ToString()) } });

                    alDataKeyNames.Add(dr["FIELD_NAME"].ToString());
                }
            }

            gvw.DataKeyNames = (string[])alDataKeyNames.ToArray(Type.GetType("System.String"));

            gvw.Columns.Add(new CommandField { ShowSelectButton = true });
            //
            gvw.DataSource = ExecRawSP(sSPName);
            gvw.DataBind();
            //
        }
    }

    private static DataTable GetLookUp(string LookUpCode, ref string SPName)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;
        DataTable _dt = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = "";
            _ht["p_lookup_code"] = LookUpCode;

            _dt = _dal.GetRows("MASTER_LOOKUP_COLUMN", _ht);

            if (_dt != null && _dt.Rows.Count > 0)
            {
                SPName = _dt.Rows[0]["SP_NAME"].ToString();
            }
            else
                throw new Exception("Fail to execute MASTER_LOOKUP_COLUMN");

            return _dt;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public static string GenerateLookUpClearString(string data)
    {
        string queryString = data;
        string script = "";
        string[] items = queryString.Split('&');

        foreach (string item in items)
        {
            string[] xyz = item.Split('=');
            string ctrlID = xyz[1];

            if (xyz[0].Contains("col_"))
            {
                string[] abc = xyz[0].Split('_');
                int iColIndex = Int32.Parse(abc[1]);

                if (ctrlID.Contains("lbl"))
                    script += "parent.document.getElementById('" + ctrlID + "').innerHTML = '';";
                else
                    script += "parent.document.getElementById('" + ctrlID + "').value = '';";
            }
        }

        script += "parent.$('#ModalPopup').modal('hide');";

        return script;
    }


    public static void BindSubscription(string SubscribeCode, ref string TableSource, ref string TableTarget, ref string SPSaveName,
        ref string SPSourceToTarget, ref string SPTargetToSource, ref string SPParameterCode, ref string SPParameterUserCode)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;
        DataTable _dt = null;

        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();

            _ht["p_keywords"] = string.Empty;
            _ht["p_subscribe_code"] = SubscribeCode;

            _dt = _dal.GetRows("MASTER_SUBSCRIPTION", _ht);

            if (_dt.Rows.Count > 0)
            {
                TableSource = _dt.Rows[0]["SP_TABLE_SOURCE"].ToString();
                TableTarget = _dt.Rows[0]["SP_TABLE_TARGET"].ToString();
                SPSaveName = _dt.Rows[0]["SP_SAVE_NAME"].ToString();
                SPSourceToTarget = _dt.Rows[0]["SP_SOURCE_TO_TARGET"].ToString();
                SPTargetToSource = _dt.Rows[0]["SP_TARGET_TO_SOURCE"].ToString();
                SPParameterCode = _dt.Rows[0]["SP_PARAMETER_CODE"].ToString();
                SPParameterUserCode = _dt.Rows[0]["SP_PARAMETER_USER_CODE"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private static DataTable ExecRawSP(string SPName)
    {
        GeneralDAL _dal = null;
        Hashtable _ht = null;


        try
        {
            _dal = new GeneralDAL();
            _ht = new Hashtable();
            _ht["p_keywords"] = "";

            return _dal.GetRows("", SPName, _ht);
        }
        catch (Exception ex)
        {
            return null; ;
        }
    }

    public static string GenerateLookUpReturnString(string data, GridView gvw)
    {
        string queryString = data;
        string script = "";
        string[] items = queryString.Split('&');

        foreach (string item in items)
        {
            string[] xyz = item.Split('=');
            string ctrlID = xyz[1];

            if (xyz[0].Contains("col_"))
            {
                string[] abc = xyz[0].Split('_');
                int iColIndex = Int32.Parse(abc[1]);

                if (ctrlID.Contains("lbl"))
                    script += "parent.document.getElementById('" + ctrlID + "').innerHTML = '" + gvw.SelectedDataKey[iColIndex].ToString() + "';";
                else
                    script += "parent.document.getElementById('" + ctrlID + "').value = '" + gvw.SelectedDataKey[iColIndex].ToString() + "';";
            }
        }


        script += "parent.$('#ModalPopup').modal('hide');";

        return script;
    }
    public static string MakeSingleLine(Exception ex)
    {
        string err = "";
        Exception exx = ex;

        while (exx != null)
        {
            err += exx.Message + " - ";
            exx = exx.InnerException;
        }

        return err.Replace("'", "").Replace("\n", "").Replace("\r", "");
    }

    public static string HashingPassword(string p)
    {
        return p;
    }

    public static DataTable ReadExcelFile(string filename, string ext, string sheetname)
    {
        OleDbConnection _conn = null;
        OleDbCommand _comm = null;
        OleDbDataAdapter _adapter = null;
        DataSet _ds = null;

        string sQuery = "SELECT * FROM " + sheetname;
        string sConnString = "";

        if (ext == ".xls")
            sConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        else
            sConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

        try
        {
            //connect
            _conn = new OleDbConnection(sConnString);

            //command
            _comm = new OleDbCommand(sQuery, _conn);

            //adapter
            _adapter = new OleDbDataAdapter(_comm);

            //dataset
            _ds = new DataSet();

            //open connection
            if (_conn.State == ConnectionState.Closed)
                _conn.Open();

            //execute query and convert result to dataset
            _adapter.Fill(_ds);

            //disposing
            _adapter.Dispose();
            _comm.Dispose();
            _conn.Close();
            _conn.Dispose();

            if (_ds != null && _ds.Tables.Count > 0)
                return _ds.Tables[0];
            else
                return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static DateTime ConvertToDateTime(string Datetime)
    {
        System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();

        try
        {
            dtfi.ShortDatePattern = "dd/MM/yyyy HH:mm:ss";
            return DateTime.Parse(Datetime, dtfi);
        }
        catch (Exception ex)
        {
            return new DateTime(1990, 1, 1);
        }
    }
}