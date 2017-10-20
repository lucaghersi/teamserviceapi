using System;
using System.Collections.Generic;
using System.Linq;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.Persistence
{
    public class MemoryTeamRepository : ITeamRepository
    {
        protected static ICollection<Team> Teams;

        public MemoryTeamRepository()
        {
            if (Teams == null)
                Teams = new List<Team>();
        }

        public MemoryTeamRepository(ICollection<Team> teams)
        {
            Teams = teams;
        }

        public IEnumerable<Team> List()
        {
            return Teams;
        }

        public Team Get(Guid id)
        {
            return Teams.FirstOrDefault(t => t.Id == id);
        }

        public Team Update(Team t)
        {
            Team team = Delete(t.Id);

            if (team != null)
                team = Add(t);

            return team;
        }

        public Team Add(Team team)
        {
            Teams.Add(team);
            return team;
        }

        public Team Delete(Guid id)
        {
            var q = Teams.Where(t => t.Id == id).ToList();
            Team team = null;

            if (q.Any())
            {
                team = q.First();
                Teams.Remove(team);
            }

            return team;
        }
    }
}