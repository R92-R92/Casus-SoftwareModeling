using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Shoot
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public Address? Location { get; set; }
        public List<Concept> Concepts { get; set; } = new List<Concept>();
        public List<Contract> Contracts { get; set; } = new List<Contract>();
        public string DateText => $"{Date?.ToString("yyyy-MM-dd")}";

        DAL dal = new DAL();


        public Shoot(int id, DateTime? date, Address? location)
        {
            Id = id;
            Date = date;
            Location = location;
        }

        public void Create()
        {
            dal.AddShoot(this);
        }
        
        public void Update()
        {
            dal.UpdateShoot(this);
        }

        public void Delete()
        {
            dal.DeleteShoot(this.Id);
        }



        public string LocationText => Location != null
            ? (string.IsNullOrWhiteSpace(Location.LocationName)
                ? $"{Location.Street} {Location.HouseNumber}, {Location.PostalCode} {Location.City}"
                : Location.LocationName)
            : "–";

        public string ConceptText => Concepts != null && Concepts.Count > 0
            ? string.Join(", ", Concepts.Select(c => c.Name))
            : "–";

    }
}
