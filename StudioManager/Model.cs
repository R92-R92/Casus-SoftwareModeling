using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    class Model : Contact
    {
        public string Picture { get; set; }
        public Model(string firstname, string lastname, string phone, string email, string socialmedia, string picture, Address address)
            : base(0, firstname, lastname, phone, email, socialmedia, picture, "Model", false, address)
        {
        }

        public void AddPicture(string picture)
        {
            Picture = picture;
        }

    }
}
