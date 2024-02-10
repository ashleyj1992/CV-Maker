using System;

namespace CV_Maker.Models
{
    public class Education
    {
        public string EducationName { get; set; }
        public string CourseName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
