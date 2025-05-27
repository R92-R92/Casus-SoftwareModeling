using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class User
    {
        public Address Home { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User(Address home, string firstName, string lastName)
        {
            Home = home;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
