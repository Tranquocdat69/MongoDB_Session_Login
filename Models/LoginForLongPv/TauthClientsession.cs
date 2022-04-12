using System.ComponentModel.DataAnnotations;

namespace MongoDB_Session_Login.Models.LoginForLongPv
{
    public partial class TauthClientsession
    {
        [Key]
        public string ALOGINNAME { get; set; }
        public string ATOKEN { get; set; }
        public string AIPSERVER { get; set; }
        public string AIPCLIENT { get; set; }
        public string AUSERAGENT { get; set; }
        public string ABROWSER { get; set; }
        public DateTime ALOGINTIME { get; set; }
        public DateTime ALOGOUTTIME { get; set; }
        public string ASESSIONNO { get; set; }
        public bool? ASESSIONFIRSTLOGIN { get; set; }
        public string AISMOBILE { get; set; }
        public string ABROWSERNAME { get; set; }
        public string ABROWSERVERS { get; set; }
        public bool? ACHKPASS2 { get; set; }
    }
}
