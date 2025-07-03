using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Country (int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
