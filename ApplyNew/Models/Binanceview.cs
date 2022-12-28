using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class Binanceview
    {
        public int MyProperty { get; set; }
    }

    public class BinanceExternalData
    {
        public string referenceNumber { get; set; }
        public decimal amount { get; set; }
        public string goodsName { get; set; }
        public string goodsDetail { get; set; }
        public string successUrl { get; set; }
        public string failureUrl { get; set; }
    }

    public class BinanceResponse
    {
        public string status { get; set; }
        public string code { get; set; }
        public data data { get; set; }
        public string errorMessage { get; set; }
    }

    public class data
    {
        public string prepayId { get; set; }
        public string terminalType { get; set; }
        public string expireTime { get; set; }
        public string qrcodeLink { get; set; }
        public string qrContent { get; set; }
        public string checkoutUrl { get; set; }
        public string deeplink { get; set; }
        public string universalUrl { get; set; }
    }


}