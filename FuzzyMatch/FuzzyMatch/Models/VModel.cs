using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuzzyMatch.Models
{
    public class VModel
    {
        public List<string> header1 { get; set; }
        public List<string[]> data1 { get; set; }
        public List<string> header2 { get; set; }
        public List<string[]> data2 { get; set; }
        public List<string> headerResult { get; set; }
        public List<string[]> dataResult { get; set; }
    }
}