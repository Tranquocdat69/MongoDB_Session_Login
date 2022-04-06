using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB_Session_Login.Models.LoginForLongPv
{
    public class Permit
    {
        [Key]
        public int Id { get; set; }
        public string EzTradeChargeRate { get; set; }
        public string EzTrade { get; set; }
        public string EzTransfer { get; set; }
        public string EzAdvance { get; set; }
        public string EzMargin { get; set; }
        public string EzMortgage { get; set; }
        public string EzOddlot { get; set; }
        public string EzMarginPro { get; set; }
        public string EzFuture { get; set; }
        public string EzTvdt { get; set; }
        public string vTblid { get; set; }
        public string vFeeUP { get; set; }
        public string vFeeUP_CCQ { get; set; }
        public string vFeeLISTED_CP { get; set; }
        public string vFeeHSX_CP { get; set; }
        public string vFeeRate_TP { get; set; }
        public string vFeeLISTED_ETF { get; set; }
        public string vFeeLISTED_CCQ { get; set; }
        public string vFeeHSX_CCQ { get; set; }
        public string vFeeHSX_CQ { get; set; }
        public string vFeeHSX_ETF { get; set; }
        public string vFeeLISTED_CQ { get; set; }
        public int UserId { get; set; }
    }
}
