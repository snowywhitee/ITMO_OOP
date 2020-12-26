using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class ProjectTask : IEntity
    {
        private int _id;
        private string _name;
        private string _description;
        private ProjectTaskState _state;
        private TeamMember _owner;

        private static int idCount = 0;
        public int Id { get => _id; }
        public string Name
        {
            get => _name;
            set
            {
                //validation
                Changes.Add(new Change(_name, value));
                _name = value;
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                //validation
                Changes.Add(new Change(_description, value));
                _description = value;
            }
        }
        public ProjectTaskState State
        {
            get => _state;
            set
            {
                //validation
                Changes.Add(new Change(_state, value));
                _state = value;
            }
        }
        public List<string> Comments { get; protected set; }
        public TeamMember Owner
        {
            get => _owner;
            set
            {
                //validation
                Changes.Add(new Change(_owner, value));
                _owner = value;
            }
        }
        public List<Change> Changes { get; protected set; } = new List<Change>();

        public ProjectTask(string name, string description)
        {
            _id = idCount++;
            _name = name;
            _description = description;
            _state = ProjectTaskState.Open;
            Changes.Add(new Change(ProjectTaskState.New, _state));
        }
        public void AddComment(string comment)
        {
            Comments.Add(comment);
        }
    }
}
