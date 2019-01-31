using DukcapilWS.ApiServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace DukcapilWS.Utility
{
    public class Shared
    {
        public static string ConnectionString
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]; }
        }

        //public static string CurrentUID
        //{
        //    get
        //    {
        //        return ((DataRow)HttpContext.Current.Session[SessionKey.CURRENT_USER_SESSION_KEY])["ID"].ToString();
        //    }
        //}
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

        //internal static void ShowNotFoundError(DukcapilWS dukcapilWS)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static void ShowDukcapilError(DukcapilWS dukcapilWS)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static void ShowErrorDialog(DukcapilWS dukcapilWS, Exception ex)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static void ShowSuccessGritter(DukcapilWS dukcapilWS, string v)
        //{
        //    throw new NotImplementedException();
        //}

        internal static void ShowErrorDialog(Services services, Exception ex)
        {
            throw new NotImplementedException();
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
    }
}