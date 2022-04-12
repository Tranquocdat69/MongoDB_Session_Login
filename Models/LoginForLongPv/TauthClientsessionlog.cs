using System.ComponentModel.DataAnnotations;

namespace MongoDB_Session_Login.Models.LoginForLongPv
{
    public partial class TauthClientsessionlog
    {
        [Key]
        public long? ATBLID { get; set; }
        public string ALOGINNAME { get; set; }

        public string AERRCODE { get; set; }

        public string AERRMESSAGE { get; set; }

        public bool? ASOURCE { get; set; }

        public string AIPSERVER { get; set; }

        public string AIPCLIENT { get; set; }

        public string AREFERER { get; set; }
        public string AUSERAGENT { get; set; }

        public string ABROWSER { get; set; }

        public DateTime ALOGTIME { get; set; }

        public string AACTIVITY { get; set; }

        public string AACTIVITYUSR { get; set; }

        public string AACTIVITYDSC { get; set; }
        public string AACTIVITYBTNTYPE { get; set; }

        public string AISMOBILE { get; set; }

        public string ABROWSERNAME { get; set; }
        public string ABROWSERVERS { get; set; }

        public string ABRKID { get; set; }
    }
}
