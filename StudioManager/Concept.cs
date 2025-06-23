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
        public string Name { get; set; }
        public Address? Address { get; set; }
        public string Description { get; set; }
        public string Sketch { get; set; }
        public List<string> Pictures { get; set; } = new List<string>();
        public List<Contact> Models { get; set; } = new List<Contact>();
        public List<Prop> Props { get; set; }
        public Shoot? Shoot { get; set; }
        new DAL dal = new DAL();


        public Concept(int id, string? name,Address address, string description, string sketch, List<Prop> props, Shoot shoot)
        { 
            Id = id;
            Name = name;
            Address = address;
            Description = description;
            Sketch = sketch;
            Props = props;
            Shoot = shoot;
        }

        public void AddPictures(string picture)
        {
            Pictures.Add(picture);
        }

        public void ClearPictures()
        {
            Pictures.Clear();
        }

        public void Create()
        {
            dal.AddConcept(this);
        }

        public void Update()
        {
            dal.UpdateConcept(this);
        }

        public void Delete()
        {
            dal.DeleteConcept(this.Id);
        }




        
        public string PropsText
        {
            get
            {
                if (Props == null || Props.Count == 0) return "–";
                return string.Join(", ", Props.Select(p => p.Name));
            }
        }

        public string ModelText
        {
            get
            {
                if (Models == null || Models.Count == 0) return "–";
                return string.Join(", ", Models.Select(m => m.FirstName + " " + m.LastName));
            }
        }

        public string ShootDateText => Shoot?.Date?.ToString("yyyy-MM-dd") ?? "–";

        public string Location => Shoot?.Location != null
            ? (Shoot.Location.IsLocationOnly && !string.IsNullOrWhiteSpace(Shoot.Location.LocationName)
                ? Shoot.Location.LocationName
                : $"{Shoot.Location.Street} {Shoot.Location.HouseNumber}, {Shoot.Location.PostalCode} {Shoot.Location.City}")
            : "–";

    }

}
