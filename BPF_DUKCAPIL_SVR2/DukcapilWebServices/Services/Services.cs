using Newtonsoft.Json;
using DukcapilWebServices.ApiServices.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Remoting.Contexts;
using System.Web.UI;
using DukcapilWebServices.Utility;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace DukcapilWebServices.ApiServices
{
    public class Services
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static bool CheckNIKisExists(string cekNIK)
        {
            string conStr = "Server=10.162.61.3\\DUKCAPIL;Database=DUKCAPIL;User ID=sa;Password=admin123";
            string cmdText = "SELECT * FROM DATA_PENDUDUK WHERE NIK ='" + cekNIK + "'";
            bool isExist = false;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(cmdText, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        isExist = reader.HasRows;
                    }
                }
                con.Close();
            }
            return isExist;
        }

        public KTP KtpPickup(string NIK, string UserID, string Pass, string Ip)
        {
            string x = "Error";
            KTP data = new KTP();
            try
            {
                var baseAddress = "http://172.16.160.128:8000/dukcapil/get_json/BATAVIA_FINANCE/call_nik";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseAddress);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"NIK\":\"" + NIK + "\"," +
                                  "\"user_id\":\"" + UserID + "\"," +
                                  "\"password\":\"" + Pass + "\"," +
                                  "\"ip_user\":\"" + Ip + "\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<BaseEntity>(result);

                    foreach (KTP item in items.content)
                    {
                        data.AGAMA = item.AGAMA;
                        data.ALAMAT = item.ALAMAT;
                        data.JENIS_KLMIN = item.JENIS_KLMIN;
                        data.JENIS_PKRJN = item.JENIS_PKRJN;
                        data.KAB_NAME = item.KAB_NAME;
                        data.KEC_NAME = item.KEC_NAME;
                        data.KEL_NAME = item.KEL_NAME;
                        data.NAMA_LGKP = item.NAMA_LGKP;
                        data.NAMA_LGKP_IBU = item.NAMA_LGKP_IBU;
                        data.NIK = item.NIK;
                        data.NO_KAB = item.NO_KAB;
                        data.NO_KEC = item.NO_KEC;
                        data.NO_KEL = item.NO_KEL;
                        data.NO_KK = item.NO_KK;
                        data.NO_PROP = item.NO_PROP;
                        data.NO_RT = item.NO_RT;
                        data.NO_RW = item.NO_RW;
                        data.PROP_NAME = item.PROP_NAME;
                        data.STATUS_KAWIN = item.STATUS_KAWIN;
                        data.TGL_LHR = item.TGL_LHR;
                        data.TMPT_LHR = item.TMPT_LHR;
                    }
                    //KAB_NAME,AGAMA,NO_RW,KEC_NAME,JENIS_PKRJN,NO_RT,NO_KEL,ALAMAT,NO_KEC,TMPT_LHR,STATUS_KAWIN,NO_PROP,NAMA_LGKP_IBU,PROP_NAME,NO_KAB,KEL_NAME,JENIS_KLMIN,TGL_LHR,CRE_DATE,CRE_BY,CRE_IP_ADDRESS,MOD_DATE,MOD_BY,MOD_IP_ADDRESS,IS_CHECK,IS_MOBILE,MOD_DATE_CHECK"
                }
            }
            catch (Exception ex)
            {
                data = null;
                Shared.ShowErrorDialog(this, ex);

            }
            if (CheckNIKisExists(NIK))
            {
                SqlConnection con = new SqlConnection("Server=10.162.61.3\\DUKCAPIL;Database=DUKCAPIL;User ID=sa;Password=admin123");
                con.Open();
                string sqlDelete = "delete from DATA_PENDUDUK where NIK='" + NIK + "'";
                SqlCommand cmdd = new SqlCommand(sqlDelete, con);
                cmdd.ExecuteNonQuery();
                con.Close();
                SqlConnection conn = new SqlConnection("Server=10.162.61.3\\DUKCAPIL;Database=DUKCAPIL;User ID=sa;Password=admin123");
                conn.Open();
                string hostName = Dns.GetHostName();
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                string sql = "insert into DATA_PENDUDUK (NO_KK,NIK,NAMA_LGKP,KAB_NAME,AGAMA,NO_RW,KEC_NAME,JENIS_PKRJN,NO_RT,NO_KEL,ALAMAT,NO_KEC,TMPT_LHR,STATUS_KAWIN,NO_PROP,NAMA_LGKP_IBU,PROP_NAME,NO_KAB,KEL_NAME,JENIS_KLMIN,TGL_LHR,IS_MOBILE,CRE_DATE,CRE_BY,CRE_IP_ADDRESS,MOD_IP_ADDRESS)";
                sql = sql + " VALUES('" + data.NO_KK + "','" + data.NIK + "','" + data.NAMA_LGKP + "','" + data.KAB_NAME + "','" + data.AGAMA + "','" + data.NO_RW + "','" + data.KEC_NAME + "','" + data.JENIS_PKRJN + "','" + data.NO_RT + "','" + data.NO_KEL + "','" + data.ALAMAT + "','" + data.NO_KEC + "','" + data.TMPT_LHR + "','" + data.STATUS_KAWIN + "','" + data.NO_PROP + "','" + data.NAMA_LGKP_IBU + "','" + data.PROP_NAME + "','" + data.NO_KAB + "','" + data.KEC_NAME + "','" + data.JENIS_KLMIN + "','" + data.TGL_LHR + "','" + "1" + "','" + DateTime.Now + "','" + "App" + "','" + myIP + "','" + myIP + "')";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            else
            {
                SqlConnection conn = new SqlConnection("Server=10.162.61.3\\DUKCAPIL;Database=DUKCAPIL;User ID=sa;Password=admin123");
                conn.Open();
                string hostName = Dns.GetHostName();
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                string sql = "insert into DATA_PENDUDUK (NO_KK,NIK,NAMA_LGKP,KAB_NAME,AGAMA,NO_RW,KEC_NAME,JENIS_PKRJN,NO_RT,NO_KEL,ALAMAT,NO_KEC,TMPT_LHR,STATUS_KAWIN,NO_PROP,NAMA_LGKP_IBU,PROP_NAME,NO_KAB,KEL_NAME,JENIS_KLMIN,TGL_LHR,IS_MOBILE,CRE_DATE,CRE_BY,CRE_IP_ADDRESS,MOD_IP_ADDRESS)";
                sql = sql + " VALUES('" + data.NO_KK + "','" + data.NIK + "','" + data.NAMA_LGKP + "','" + data.KAB_NAME + "','" + data.AGAMA + "','" + data.NO_RW + "','" + data.KEC_NAME + "','" + data.JENIS_PKRJN + "','" + data.NO_RT + "','" + data.NO_KEL + "','" + data.ALAMAT + "','" + data.NO_KEC + "','" + data.TMPT_LHR + "','" + data.STATUS_KAWIN + "','" + data.NO_PROP + "','" + data.NAMA_LGKP_IBU + "','" + data.PROP_NAME + "','" + data.NO_KAB + "','" + data.KEC_NAME + "','" + data.JENIS_KLMIN + "','" + data.TGL_LHR + "','" + "1" + "','" + DateTime.Now + "','" + "App" + "','" + myIP + "','" + myIP + "')";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.Serialize(data);
            return data;
        }
    }
}
