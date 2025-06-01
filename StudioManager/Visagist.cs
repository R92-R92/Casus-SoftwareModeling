using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    class Visagist : Contact
    {
        public Visagist(string firstname, string lastname, string phone, string email, string socialmedia, string picture, Address address) :
        base(0, firstname, lastname, phone, email, socialmedia, picture, address)
        {
        }
    }
}
