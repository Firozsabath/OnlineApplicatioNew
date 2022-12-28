using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplyNew.Models
{
    public class ActiveCampaignSF
    {
        public string ACProgrambasedonID(int programid)
        {
            //Dictionary<string, int> Porgrams = new Dictionary<string, int>()
            //{
            //     {"Bachelor of Arts in Creative Industries",65}

            //    ,{"Bachelor of Arts in Communication - Advertising and Integrated Marketing Comm",22}

            //    ,{"Bachelor of Arts in Communication - Digital Media and Journalism",20}

            //    ,{"Bachelor of Arts in Communication in Public Relations",21}

            //    ,{"Bachelor of Arts in Psychology",49}

            //    ,{"Bachelor of Arts in Applied Sociology (Arabic)",57}

            //    ,{"Bachelor of Arts in Psychology (Arabic)",50}

            //    ,{"Bachelor of Arts in Sociology (Arabic)",57}

            //    ,{"Bachelor of Business Administration in Accounting and Finance",10}

            //    ,{"Bachelor of Business Administration in e-Business",9}

            //    ,{"Bachelor of Business Administration in Events and Tourism",43}

            //    ,{"Bachelor of Business Administration in Forensic Accounting",44}

            //    ,{"Bachelor of Business Administration in Human Resource Management",11}

            //    ,{"Bachelor of Business Administration in International Business",12}

            //    ,{"Bachelor of Business Administration in Luxury Marketing",45}

            //   ,{"Bachelor of Business Administration in Marketing",8}

            //    ,{"Bachelor of Business Administration in Operations and Supply Management",41}

            //   ,{"Bachelor of Business Administration in Sports Management",42}

            //   ,{"Bachelor of Architecture",27}

            //   ,{"Bachelor of Science in Interior Design",1}

            //    ,{"Bachelor of Science in Electrical Engineering - Telecommunications",46}

            //    ,{"Bachelor of Science in Electrical Engineering - Electronics",47}

            //    ,{"Bachelor of Science in Electrical Engineering - Mechatronics",48}

            //    //,{"Bachelor of Science in Network Engineering ",19}

            //    ,{"Bachelor of Computer and Networking Engineering Technology",19}

            //    ,{"Bachelor of Science in Computer Science",68}

            //    ,{"Bachelor of Science in Software Design",67}

            //    ,{"Bachelor of Science in Cyber Security",69}

            //    //,{"Bachelor of Science in Health Organization Management ",26}

            //    ,{"Bachelor of Science in Environmental Health Management",6}

            //    //,{"Bachelor of Science in Health Information Management ",26}

            //     ,{"Bachelor of Science in Tourism (Arabic)",66}

            //    //,{"Master in Information Technology Management and Governance",28}
            //    ,{"Master in Information Technology Management",28}

            //    ,{"Master of Business Administration in General Management",31}

            //    ,{"Master of Business Administration in Human Resource Management",32}

            //    ,{"Master of Business Administration in Finance",30}

            //    ,{"Master of Business Administration in Marketing",37}



            //};

            //string Program = Porgrams.FirstOrDefault(y => y.Value == programid).Key;
            string Program = string.Empty;

            return Program;
        }

        public string ACTermbasedonID(int Termid)
        {
            Dictionary<string, int> Terms = new Dictionary<string, int>()
            {
                {"Spring - January 2021",56},
                {"Fall - September 2021",59},
                {"Summer - May 2023",65},
                {"Spring - January 2022",60},
                {"Fall - September 2022",61},
                {"Summer - May 2024",66},
                {"Summer - May 2022",63},
                {"Spring - January 2023",64}
            };
            string Term = Terms.FirstOrDefault(y => y.Value == Termid).Key;

            return Term;
        }

        public string OracleDGType(int typeid)
        {
            Dictionary<string, int> Terms = new Dictionary<string, int>()
            {
                {"Bachelors",3},
                {"Masters",1}                
            };
            string Term = Terms.FirstOrDefault(y => y.Value == typeid).Key;
            return Term;
        }

        public string HSCurriculum(int id)
        {
            Dictionary<string, int> Curriculumns = new Dictionary<string, int>()
            {
                {"American",3256},
                {"British",3257},
                {"Canadian",3258},
                 {"CBSE",3260},
                {"Ethiopian",3873},
                {"French",3259},
                 {"IB",3261},
                {"Iranian System",3262},
                {"MoE",3267},
                {"Nigerian",3263},
                {"Pakistani",3265},
                 {"Russian",3266},
                {"SABIS",3871},
                {"Ghana",3872},
            };

            string Curriculmn = Curriculumns.FirstOrDefault(y => y.Value == id).Key;
            return Curriculmn;
        }
    }
}