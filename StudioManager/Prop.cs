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
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        DAL dal = new DAL();

        public Prop(int id, string name, string? description, bool isAvailable)
        {
            Id = id;
            Name = name;
            Description = description;
            IsAvailable = isAvailable;
        }

        public bool ToggleAvailability()
        {
            IsAvailable = !IsAvailable;
            return IsAvailable;
        }

        public void Create()
        {
            dal.AddProp(this);
        }

        public void Update()
        {
            dal.UpdateProp(this);
        }

        public void Delete()
        {
            dal.DeleteProp(this.Id);
        }

    }
}
