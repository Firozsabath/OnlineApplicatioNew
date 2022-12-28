using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class Optional
    {
        public string NeededVisa { get; set; }
        public string NeededHousing { get; set; }
        public string NeededSpecial { get; set; }
        public string NeededSpecialNotes { get; set; }
        public string NeededTransportation { get; set; }
        public string TransferFromHigherInstitution { get; set; }
        public string TransferCredit { get; set; }            
        public string TransferToCanada { get; set; }
        public string UsCitizen { get; set; }        
        public string UAEID { get; set; }
        public string UAEIDExpDate { get; set; }
        public string Gpagroupdid { get; set; }
    }
}