using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Prop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvaileable { get; set; }

        public Prop(int id, string name, string description, bool isAvaileable)
        {
            Id = id;
            Name = name;
            Description = description;
            IsAvaileable = isAvaileable;
        }
    }
}
