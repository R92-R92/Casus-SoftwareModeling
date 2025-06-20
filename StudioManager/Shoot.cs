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
    }
}
