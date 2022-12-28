using ApplyNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.ViewModel
{
    public class ACViewModel
    {
        public ACContact contact { get; set; }     

    }
    public class ACViewModelsecondary
    {
        public ACContactsecondary contact { get; set; }

    }

    public class Eloquacontact
    {
        public ACContactsample contact { get; set; }
    }

    public class Eloquacontactsecondary
    {
        public ACContactsecondaryeloqua contact { get; set; }
    }    
}