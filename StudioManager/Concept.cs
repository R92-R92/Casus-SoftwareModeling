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
        public string Sketch { get; set; }
        public List<string> Pictures { get; set; } = new List<string>();
        public List<Contact> Models { get; set; } = new List<Contact>();
        public List<Prop> Props { get; set; }
        public Shoot? Shoot { get; set; }
        new DAL dal = new DAL();


        public Concept(int id, string description, string sketch, List<Prop> props, Shoot shoot)
        { 
            Id = id;
            Description = description;
            Sketch = sketch;
            Props = props;
            Shoot = shoot;
        }

        public void AddPictures(string picture)
        {
            Pictures.Add(picture);
        }

        public void Create()
        {
            dal.AddConcept(this);
        }
    }

}
