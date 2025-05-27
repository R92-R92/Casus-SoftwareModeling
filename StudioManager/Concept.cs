using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace StudioManager
{
    public class Concept
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Address Location { get; set; }
        public string Sketch { get; set; }
        public string Picture { get; set; }
        public List<Contact> Models { get; set; } = new List<Contact>();
        public List<Prop> Props { get; set; }
        public Shoot? Shoot { get; set; }

        public Concept(int id, string description, Address location, string sketch, string picture, List<Prop> props, Shoot shoot)
        { 
            Id = id;
            Description = description;
            Location = location;
            Sketch = sketch;
            Picture = picture;
            Props = props;
            Shoot = shoot;
        }
    }
}
