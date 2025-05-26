using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Address
    {
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }


        public Address(string street, string houseNumber, string postalCode, string city, string country)

        {
            Street = street;
            HouseNumber = houseNumber;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }
    }
}
