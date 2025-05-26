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
        public Contact Model { get; set; }
        public List<Prop> Props { get; set; }
        public Shoot Shoot { get; set; }

        public Concept(int id, string description, Address location, string sketch, string picture, Contact model, List<Prop> props, Shoot shoot)
        { 
            Id = id;
            Description = description;
            Location = location;
            Sketch = sketch;
            Picture = picture;
            Model = model;
            Props = props;
            Shoot = shoot;
        }
    }
}
