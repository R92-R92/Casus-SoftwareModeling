﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Address
    {
        public int Id { get; set; }
        public string? LocationName { get; set; }
        public bool IsLocationOnly { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        DAL dal = new DAL();


        public Address(int id,string? locationName, bool isLocationOnly, string street, string houseNumber, string postalCode, string city, string country)

        {
            Id = id;
            LocationName = locationName;
            IsLocationOnly = isLocationOnly;
            Street = street;
            HouseNumber = houseNumber;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }

        public void Create()
        {
            dal.AddAddress(this);
        }

        public void Update()
        {
            dal.UpdateAddress(this);
        }

        public void Delete()
        {
            dal.DeleteAddress(this.Id);
        }

    }
}
