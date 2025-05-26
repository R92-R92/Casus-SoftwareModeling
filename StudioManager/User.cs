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

        public User(Address home)
        {
            Home = home;
        }
    }
}
