using DukcapilWS.ApiServices.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace DukcapilWS.ApiServices
{
    public class Services
    {
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

        public KTP KtpPickup(string NIK, string UserID, string Pass, string Ip)
        {
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
                }
            }
            catch (Exception ex)
            {
                data = null;
            }

            return data;
        }
    }
}
