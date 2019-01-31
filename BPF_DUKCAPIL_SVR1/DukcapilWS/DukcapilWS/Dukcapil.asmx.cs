using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using DukcapilWS.Utility;
using DukcapilWS.ApiServices.Entity;
using DukcapilWS.ApiServices;
using System.Collections;

namespace DukcapilWS
{

    /// <summary>
    /// Summary description for Dukcapil
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    //=========================================================================================//

    public class Dukcapil : System.Web.Services.WebService
    {
        public string Update(string NIK)
        {
            SqlConnection con = new SqlConnection("Server=10.162.61.3\\DUKCAPIL;Database=DUKCAPIL;User ID=sa;Password=admin123");
            con.Open();
            string sqlDelete = "UPDATE DATA_PENDUDUK SET IS_MOBILE = 1 where NIK='" + NIK + "'";
            SqlCommand cmdd = new SqlCommand(sqlDelete, con);
            cmdd.ExecuteNonQuery();
            con.Close();
            string s = "IsMobile Updated !";
            return s;
        }

        [WebMethod]
        public void BPF_Dukcapil(string nik, string PassKey)
        {
           string key = "NaAsTYMvZzNURPXO6m3RCNOROK7RMb5mG7vUk1p7hCG5JLieFlxJbg7ypG6ayndYNdGJA1RzP";
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Data_Penduduk> ListDataPenduduk = new List<Data_Penduduk>();
            List<JSonArray> ListDataKTP = new List<JSonArray>();
            string cs = ConfigurationManager.ConnectionStrings["dbdukcapil"].ConnectionString;
            if (PassKey != key)
            {
                Context.Response.Write("PassKey Not valid !");
            }
            else
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand();

                    cmd.Connection = con;
                    cmd.CommandText = "select * from [DATA_PENDUDUK] where NIK=@NIK";
                    cmd.Parameters.Add("@NIK", System.Data.SqlDbType.VarChar).Value = nik;
                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Data_Penduduk DataPenduduk = new Data_Penduduk();
                            DataPenduduk.NO_KK = rdr["NO_KK"].ToString();
                            DataPenduduk.NIK = rdr["NIK"].ToString();
                            DataPenduduk.NAMA_LGKP = rdr["NAMA_LGKP"].ToString();
                            DataPenduduk.KAB_NAME = rdr["KAB_NAME"].ToString();
                            DataPenduduk.AGAMA = rdr["AGAMA"].ToString();
                            DataPenduduk.NO_RW = rdr["NO_RW"].ToString();
                            DataPenduduk.KEC_NAME = rdr["KEC_NAME"].ToString();
                            DataPenduduk.JENIS_PKRJN = rdr["JENIS_PKRJN"].ToString();
                            DataPenduduk.NO_RT = rdr["NO_RT"].ToString();
                            DataPenduduk.NO_KEL = rdr["NO_KEL"].ToString();
                            DataPenduduk.ALAMAT = rdr["ALAMAT"].ToString();
                            DataPenduduk.NO_KEC = rdr["NO_KEC"].ToString();
                            DataPenduduk.TMPT_LHR = rdr["TMPT_LHR"].ToString();
                            DataPenduduk.STATUS_KAWIN = rdr["STATUS_KAWIN"].ToString();
                            DataPenduduk.NO_PROP = rdr["NO_PROP"].ToString();
                            DataPenduduk.NAMA_LGKP_IBU = rdr["NAMA_LGKP_IBU"].ToString();
                            DataPenduduk.PROP_NAME = rdr["PROP_NAME"].ToString();
                            DataPenduduk.NO_KAB = rdr["NO_KAB"].ToString();
                            DataPenduduk.KEL_NAME = rdr["KEL_NAME"].ToString();
                            DataPenduduk.JENIS_KLMIN = rdr["JENIS_KLMIN"].ToString();
                            DataPenduduk.TGL_LHR = Convert.ToDateTime(rdr["TGL_LHR"]);
                            ListDataPenduduk.Add(DataPenduduk);
                        }
                        JavaScriptSerializer js2 = new JavaScriptSerializer();
                        Context.Response.Write(js2.Serialize(ListDataPenduduk));
                        //Context.Response.Write(serializer.Serialize(ListDataPenduduk));
                        cmd.Dispose();
                        con.Close();
                        Update(nik);

                    }
                    else
                    {
                        WS1.DukcapilWS wsCall = new WS1.DukcapilWS();
                        JSonArray DataKTP = new JSonArray();

                        string data = wsCall.DukcapilWebServices(nik, PassKey);
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        //this.Context.Response.ContentType = "application/json; charset=utf-8";

                        Context.Response.Write(data);
                        Context.Response.Write(" Row(s) Inserted ");
                       
                    }
                }

            }
        }
    }
}
