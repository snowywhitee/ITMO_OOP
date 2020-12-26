using System.Collections.Generic;

namespace BLL
{
    public class TeamMember
    {
        private static int idCount = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public TeamMember Lead { get; set; }
        public Dictionary<int, TeamMember> Team { get; set; } = new Dictionary<int, TeamMember>();
        public static Dictionary<int, TeamMember> ProjectTeam { get; private set; } = new Dictionary<int, TeamMember>();

        public TeamMember(string name)
        {
            this.Id = idCount++;
            this.Name = name;
            ProjectTeam.Add(Id, this);
        }
    }
}
