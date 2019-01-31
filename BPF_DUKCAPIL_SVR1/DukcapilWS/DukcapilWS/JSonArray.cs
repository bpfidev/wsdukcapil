using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DukcapilWS
{
    public class JSonArray
    {
        [JsonProperty("NO_KK")]
        public string NO_KK { get; set; }

        [JsonProperty("NIK")]
        public string NIK { get; set; }

        [JsonProperty("NAMA_LGKP")]
        public string NAMA_LGKP { get; set; }

        [JsonProperty("KAB_NAME")]
        public string KAB_NAME { get; set; }

        [JsonProperty("AGAMA")]
        public string AGAMA { get; set; }

        [JsonProperty("NO_RW")]
        public string NO_RW { get; set; }

        [JsonProperty("KEC_NAME")]
        public string KEC_NAME { get; set; }

        [JsonProperty("JENIS_PKRJN")]
        public string JENIS_PKRJN { get; set; }

        [JsonProperty("NO_RT")]
        public string NO_RT { get; set; }

        [JsonProperty("NO_KEL")]
        public string NO_KEL { get; set; }

        [JsonProperty("ALAMAT")]
        public string ALAMAT { get; set; }

        [JsonProperty("NO_KEC")]
        public string NO_KEC { get; set; }

        [JsonProperty("TMPT_LHR")]
        public string TMPT_LHR { get; set; }

        [JsonProperty("STATUS_KAWIN")]
        public string STATUS_KAWIN { get; set; }

        [JsonProperty("NO_PROP")]
        public string NO_PROP { get; set; }

        [JsonProperty("NAMA_LGKP_IBU")]
        public string NAMA_LGKP_IBU { get; set; }

        [JsonProperty("PROP_NAME")]
        public string PROP_NAME { get; set; }

        [JsonProperty("NO_KAB")]
        public string NO_KAB { get; set; }

        [JsonProperty("KEL_NAME")]
        public string KEL_NAME { get; set; }

        [JsonProperty("JENIS_KLMIN")]
        public string JENIS_KLMIN { get; set; }

        [JsonProperty("TGL_LHR")]
        public DateTime TGL_LHR { get; set; }
    }
}