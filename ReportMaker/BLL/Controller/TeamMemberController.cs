using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public static class TeamMemberController
    {
        public static TeamMember GetTeamMember(int id)
        {
            return TryGet(id);
        }
        public static List<TeamMember> GetAll()
        {
            var result = new List<TeamMember>();
            foreach (var item in TeamMember.ProjectTeam)
            {
                result.Add(item.Value);
            }
            return result;
        }
        public static int AddTeamMember(string name)
        {
            return new TeamMember(name).Id;
        }
        public static void EditTeamMemberName(int id, string name)
        {
            TryGet(id).Name = name;
        }
        public static void EditTeamMemberLead(int id, TeamMember lead)
        {
            TryGet(id).Lead = lead;
        }
        public static void AddToTeam(int leadId, int teamId)
        {
            GetTeamMember(leadId).Team.Add(teamId, GetTeamMember(teamId));
            GetTeamMember(teamId).Lead = GetTeamMember(leadId);
        }
        public static void RemoveFromTeam(int id, TeamMember teamMember)
        {
            TeamMember lead = TryGet(id);
            if (!lead.Team.ContainsKey(teamMember.Id))
            {
                throw new Exception($"No such user {teamMember.Name} in team with {lead.Name}");
            }
            lead.Team.Remove(teamMember.Id);
        }
        private static TeamMember TryGet(int id)
        {
            if (!TeamMember.ProjectTeam.ContainsKey(id))
            {
                throw new Exception($"No TeamMember found with id {id}");
            }
            return TeamMember.ProjectTeam[id];
        }
    }
}
