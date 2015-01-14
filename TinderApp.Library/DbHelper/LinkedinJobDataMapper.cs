using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinderApp.DbHelper
{
    public class Tb_LinkedinJob
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id {get; set;}

        public string CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string DescriptionSnippet { get; set; }

        public string JobId { get; set; }

        public string JobPosterFirstName { get; set; }

        public string LocationDescription { get; set; }
    }
     
}
