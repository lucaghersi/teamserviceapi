using System.Collections.Generic;
using StatlerWaldorfCorp.TeamService.Persistence;

// ReSharper disable once CheckNamespace
namespace StatlerWaldorfCorp.TeamService.Models
{
    public class MemoryTeamRepositoryTest : MemoryTeamRepository
    {
        public MemoryTeamRepositoryTest() : base(CreateInitialFake())
        {

        }

        private static ICollection<Team> CreateInitialFake()
        {
            var teams = new List<Team> {new Team("one"), new Team("two")};
            return teams;
        }
    }
}