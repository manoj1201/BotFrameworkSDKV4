using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Search
{
    public class DocumentDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string DocumentPath { get; set; }
        public string Summary { get; set; }
        public string CreatedDateTime { get; set; }
    }
}
