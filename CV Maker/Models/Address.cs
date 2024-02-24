using System;
using System.Collections.Generic;
using System.Linq;

namespace CV_Maker.Models
{
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string CountryNumericCode { get; set; }

        public virtual ISO3166.Country Country
        {
            get => ISO3166.Country.List.Single(x => x.NumericCode == CountryNumericCode);
            set => ISO3166.Country.List.Single(x => x.NumericCode == CountryNumericCode);
        }

        public override string ToString()
        {
            var addressString = new List<string>();
            if (!string.IsNullOrWhiteSpace(Line1))
                addressString.Add(Line1);

            if (!string.IsNullOrWhiteSpace(Line2))
                addressString.Add(Line2);

            if (!string.IsNullOrWhiteSpace(Line3))
                addressString.Add(Line3);

            if (!string.IsNullOrWhiteSpace(City))
                addressString.Add(City);

            if (!string.IsNullOrWhiteSpace(County))
                addressString.Add(County);

            if (!string.IsNullOrWhiteSpace(PostCode))
                addressString.Add(PostCode);

            if (!string.IsNullOrWhiteSpace(Country.Name))
                addressString.Add(Country.Name);

            return string.Join($"{Environment.NewLine}", addressString);
        }
    }
}
