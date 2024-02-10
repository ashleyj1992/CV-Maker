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
        public IList<Job> Jobs { get; set; }
        public IList<Link> Links { get; set; }
        public IList<Education> Education { get; set; }
        public IList<Skill> Skills { get; set; }
    }
}
