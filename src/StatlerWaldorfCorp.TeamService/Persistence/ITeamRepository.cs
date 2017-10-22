using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.Persistence
{
    public interface ITeamRepository
    {
        IEnumerable<Team> List();
        Team Get(Guid id);
        Task<Team> Add(Team team);
        Task<Team> Update(Team team);
        Task Delete(Guid id);

        IEnumerable<Member> GetMembers();
        IEnumerable<Member> GetMembersByTeam(Guid teamId);
        Member GetMember(Guid id);
        Member GetMemberInTeam(Guid teamId, Guid memberId);
        Task<Member> UpdateMember(Member member);
        Task<Member> CreateMember(Member member);
    }
}