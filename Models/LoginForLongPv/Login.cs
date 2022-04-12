using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB_Session_Login.Models.LoginForLongPv
{
    public class Login
    {
        public DateTime Time { get; set; }
        public TauthUserlogin User { get; set; }
        public TauthClientsession ClientSession { get; set; }
        public TauthClientsessionlog ClientSessionLog { get; set; }
    }
}
