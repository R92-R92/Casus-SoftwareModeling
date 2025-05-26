using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManager
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public string Notes { get; set; }
        public List<Concept> Concepts { get; set; }


        public Project(int id, string name, DateTime deadline, string notes, List<Concept> concepts)
        {
            Id = id;
            Name = name;
            Deadline = deadline;
            Notes = notes;
            Concepts = concepts;
        }
    }
}
