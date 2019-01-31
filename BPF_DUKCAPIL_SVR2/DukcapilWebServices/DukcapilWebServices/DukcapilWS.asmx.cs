using DukcapilWebServices;
using DukcapilWebServices.ApiServices;
using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using DukcapilWebServices.ApiServices.Entity;
using System.Net;
using System.Net.Sockets;
using DukcapilWebServices.Utility;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DukcapilWebServices
{
    /// <summary>
    /// Summary description for DukcapilWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]

    public class DukcapilWS : System.Web.Services.WebService
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
        //----------------------------------------------------------------
        [WebMethod]
        public string DukcapilWebServices(string NIK, string PassKey)
        {
            string Key = "NaAsTYMvZzNURPXO6m3RCNOROK7RMb5mG7vUk1p7hCG5JLieFlxJbg7ypG6ayndYNdGJA1RzP";
            string JsonData = "";
            string s = "nik data input or passkey must be correct";
            string usr = "243347402lelyd";
            string pass = "9WoojVYw";
            string ip = "10.162.61.3";
            if (PassKey == Key && NIK != "")
            {
                //return s;
                Services sv = new Services();
                KTP data = sv.KtpPickup(NIK, usr, pass, ip);
                System.Web.Script.Serialization.JavaScriptSerializer serializer2 = new System.Web.Script.Serialization.JavaScriptSerializer();
                JsonData = serializer2.Serialize(data);
            }
            else
            {
                return s;
            }
            return JsonData;
        }
    }
}
