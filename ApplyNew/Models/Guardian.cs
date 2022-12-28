using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplyNew.Resources;

namespace ApplyNew.Models
{
    public class Guardian
    {
        public int ContactID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Relationship { get; set; }
        public string Gemail { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string IndustryType { get; set; }

        //public string[] Relationships = new[] { Resource.Father, Resource.Mother, Resource.Uncle, Resource.Aunt, Resource.Guardian, Resource.brother, Resource.sister };

        public List<SelectListItem> Relationships = new List<SelectListItem>() {
                                new SelectListItem {
                                    Text = Resource.Father, Value = "Father"
                                },
                                new SelectListItem {
                                    Text = Resource.Mother, Value = "Mother"
                                },
                                new SelectListItem {
                                    Text = Resource.Uncle, Value = "Uncle"
                                },
                                new SelectListItem {
                                    Text = Resource.Aunt, Value = "Aunt"
                                },
                                new SelectListItem {
                                    Text = Resource.Guardian, Value = "Guardian"
                                },
                                new SelectListItem {
                                    Text = Resource.brother, Value = "Brother"
                                },
                                new SelectListItem {
                                    Text = Resource.sister, Value = "Sister"
                                },                                
                            };
    }
}