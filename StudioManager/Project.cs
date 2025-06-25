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
        public DateTime? Deadline { get; set; }
        public string? Notes { get; set; }
        public List<Concept> Concepts { get; set; } = new List<Concept>();
        new DAL dal = new DAL();

        public enum ProjectState { Created, Active, OnHold, Completed, Cancelled, Archived }
        public ProjectState State { get;  set; } = ProjectState.Created;

        public Project(int id, string name, DateTime? deadline, string? notes)
        {
            Id = id;
            Name = name;
            Deadline = deadline;
            Notes = notes;
            State = ProjectState.Created;
        }

        public void Create()
        {
            
            State = ProjectState.Created;
            dal.AddProject(this);
        }

        public void Update()
        {
            dal.UpdateProject(this);
        }

        public void Delete()
        {
            dal.DeleteProject(this.Id);
        }
        public void Activate()
        {
            if (State != ProjectState.Created)
                throw new InvalidOperationException($"Cannot activate a project when it is in the {State} state.");
            State = ProjectState.Active;
            Update();
        }

        public void PutOnHold()
        {
            if (State != ProjectState.Active)
                throw new InvalidOperationException($"Cannot put project on hold when in state {State}");
            State = ProjectState.OnHold;
            Update();
        }

        public void Resume()
        {
            if (State != ProjectState.OnHold)
                throw new InvalidOperationException($"Cannot resume project when in state {State}");
            State = ProjectState.Active;
            Update();
        }
        public void Complete()
        {
            if (State != ProjectState.Active)
                throw new InvalidOperationException($"Cannot Complete() when in state {State}");
            State = ProjectState.Completed;
            Update();
        }

        public void Cancel()
        {
            if (State == ProjectState.Archived)
                throw new InvalidOperationException("Cannot Cancel() an archived project");
            State = ProjectState.Cancelled;
            Update();
        }

        public void Archive()
        {
            if (State != ProjectState.Completed && State != ProjectState.Cancelled)
                throw new InvalidOperationException($"Cannot Archive() when in state {State}");
            State = ProjectState.Archived;
            Update();
        }

    }
}
