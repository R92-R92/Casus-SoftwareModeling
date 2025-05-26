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
        public DateTime Date { get; set; }
        public Address Location { get; set; }
        public string Contract { get; set; }
        public bool IsContractSigned { get; set; }
        public List<Concept> Concepts { get; set; }

        public Shoot(int id, DateTime date, Address location, string contract, bool isContractSigned, List<Concept> concepts)
        {
            Id = id;
            Date = date;
            Location = location;
            Contract = contract;
            IsContractSigned = isContractSigned;
            Concepts = concepts;
        }
    }
}
