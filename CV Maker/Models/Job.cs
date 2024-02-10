using System;
using System.Collections.Generic;

namespace CV_Maker.Models
{
    public class Job
    {
        public string CompanyName { get; set; }
        public Address Address { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IList<Role> Roles { get; set; }
    }
}
