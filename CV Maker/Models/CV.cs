using System;
using System.Collections.Generic;

namespace CV_Maker.Models
{
    public class CV
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool CanDrive { get; set; }
        public bool OwnCar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Link> Links { get; set; }
        public ICollection<Education> Education { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public string BackgroundDescription { get; set; }
    }
}
