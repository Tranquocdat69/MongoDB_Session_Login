using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB_Session_Login.Models.LoginForLongPv
{
    public class UserTest
    {
        [Key]
        public int ID { get; set; }
        public string ALOGINNAME { get; set; }
        public string ATOKEN { get; set; }
        public string AIPSERVER { get; set; }
        public string AIPCLIENT { get; set; }
        public string AUSERAGENT { get; set; }
        public string ABROWSER { get; set; }
        public DateTime ALOGINTIME { get; set; }
        public string ALOGOUTTIME { get; set; }
        public string ASESSIONNO { get; set; }
        public int ASESSIONFIRSTLOGIN { get; set; }
        public string AISMOBILE { get; set; }
        public string ABROWSERNAME { get; set; }
        public string ABROWSERVERS { get; set; }
        public int ACHKPASS2 { get; set; }
    }
}
