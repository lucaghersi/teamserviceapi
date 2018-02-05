using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.Persistence
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> List();
        Task<Team> Get(Guid id);
        Task<Team> Add(Team team);
        Task<Team> Update(Team team);
        Task Delete(Guid id);

        Task<IEnumerable<Member>> GetMembers();
        Task<IEnumerable<Member>> GetMembersByTeam(Guid teamId);
        Task<Member> GetMember(Guid id);
        Task<Member> GetMemberInTeam(Guid teamId, Guid memberId);
        Task<Member> UpdateMember(Member member);
        Task<Member> CreateMember(Member member);
    }
}