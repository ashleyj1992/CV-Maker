using System;
using System.Collections;
using System.Collections.Generic;

namespace CV_Maker.Models
{
    public class Education
    {
        public string EducationName { get; set; }
        public string CourseName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ICollection<Module> Modules { get; set; }
    }
}
