using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }

        public Location(int id, string name)
        {
            id = Id;
            Name = name;
        }

        public void AddAddress(Address address)
        {
            Address = address;
        }
    }
}
