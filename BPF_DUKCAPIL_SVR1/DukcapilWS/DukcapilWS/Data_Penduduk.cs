using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DukcapilWS
{
    public class Data_Penduduk
    {
       // [JsonProperty("NO_KK")]
        public string NO_KK { get; set; }

        //[JsonProperty("NIK")]
        public string NIK { get; set; }

        //[JsonProperty("NAMA_LGKP")]
        public string NAMA_LGKP { get; set; }

        //[JsonProperty("KAB_NAME")]
        public string KAB_NAME { get; set; }

        //[JsonProperty("AGAMA")]
        public string AGAMA { get; set; }

       // [JsonProperty("NO_RW")]
        public string NO_RW { get; set; }

        //[JsonProperty("KEC_NAME")]
        public string KEC_NAME { get; set; }

        //[JsonProperty("JENIS_PKRJN")]
        public string JENIS_PKRJN { get; set; }

        //[JsonProperty("NO_RT")]
        public string NO_RT { get; set; }

        //[JsonProperty("NO_KEL")]
        public string NO_KEL { get; set; }

        //[JsonProperty("ALAMAT")]
        public string ALAMAT { get; set; }

        //[JsonProperty("NO_KEC")]
        public string NO_KEC { get; set; }

        //[JsonProperty("TMPT_LHR")]
        public string TMPT_LHR { get; set; }

        //[JsonProperty("STATUS_KAWIN")]
        public string STATUS_KAWIN { get; set; }

        //[JsonProperty("NO_PROP")]
        public string NO_PROP { get; set; }

        //[JsonProperty("NAMA_LGKP_IBU")]
        public string NAMA_LGKP_IBU { get; set; }

        //[JsonProperty("PROP_NAME")]
        public string PROP_NAME { get; set; }

        //[JsonProperty("NO_KAB")]
        public string NO_KAB { get; set; }

        //[JsonProperty("KEL_NAME")]
        public string KEL_NAME { get; set; }

       // [JsonProperty("JENIS_KLMIN")]
        public string JENIS_KLMIN { get; set; }

        //[JsonProperty("TGL_LHR")]
        public DateTime TGL_LHR { get; set; }

        //public override string ToString()
        //{
        //    // return string.Format("Student Information:\n\t id: {0}, \n\t Name: {1}, \n\t Degree: {2}, \n\t Hobbies: {3}", id, Name, Degree, Hobbies);
        //    return string.Format("\n\t NO_KK: {0}, \n\t NIK: {1}, \n\t NAMA_LGKP: {2}, \n\t KAB_NAME: {3}, \n\t NAMA_LGKP_AYAH: {4}, \n\t NO_RW: {5}, \n\t KEC_NAME: {6}, \n\t JENIS_PKRJN: {7}, \n\t NO_RT: {8}, \n\t NO_KEL: {9}, \n\t ALAMAT: {10}, \n\t NO_KEC: {11}, \n\t TMPT_LHR: {12}, \n\t NO_PROP: {13}, \n\t STATUS_KAWIN: {14}, \n\t NAMA_LGKP_IBU: {15}, \n\t PROP_NAME: {16}, \n\t NO_KAB: {17}, \n\t KEL_NAME: {18}, JENIS_KLMIN: {19}, \n\t TGL_LHR: {20}");
        //}
    }
}