using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.TeamService.LocationClient;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.Persistence;

namespace StatlerWaldorfCorp.TeamService.Controllers
{
    [Route("/teams/{teamId}/[controller]")]
    public class MembersController : Controller
    {
        private readonly ILocationClient _locationClient;
        private readonly ITeamRepository _repository;

        public MembersController(ITeamRepository repo, ILocationClient locationClient)
        {
            _repository = repo;
            _locationClient = locationClient;
        }

        [HttpGet]
        public virtual IActionResult GetMembers(Guid teamId)
        {
            Team team = _repository.Get(teamId);

            if (team == null)
                return NotFound();
            return Ok(team.Members);
        }


        [HttpGet("/teams/{teamId}/[controller]/{memberId}")]
        public virtual async Task<IActionResult> GetMember(Guid teamId, Guid memberId)
        {
            Team team = _repository.Get(teamId);

            if (team == null)
                return NotFound();
            var q = team.Members.Where(m => m.Id == memberId).ToList();

            if (!q.Any())
            {
                return NotFound();
            }
            Member member = q.First();

            return Ok(new LocatedMember
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                LastLocation = await _locationClient.GetLatestForMember(member.Id)
            });
        }

        [HttpPut("/teams/{teamId}/[controller]/{memberId}", Name = "GetById")]
        public virtual IActionResult UpdateMember([FromBody] Member updatedMember, Guid teamId, Guid memberId)
        {
            Team team = _repository.Get(teamId);

            if (team == null)
                return NotFound();
            var q = team.Members.Where(m => m.Id == memberId).ToList();

            if (!q.Any())
                return NotFound();
            team.Members.Remove(q.First());
            team.Members.Add(updatedMember);
            return Ok();
        }

        [HttpPost]
        public virtual IActionResult CreateMember([FromBody] Member newMember, Guid teamId)
        {
            newMember.Id = Guid.NewGuid();

            Team team = _repository.Get(teamId);

            if (team == null)
                return NotFound();
            team.Members.Add(newMember);
            var teamMember = new {TeamID = team.Id, MemberID = newMember.Id};
            return Created(Url.Link("GetById", new {teamId, memberId = teamMember.MemberID}), teamMember);
        }

        [HttpGet("/members/{memberId}/team")]
        public IActionResult GetTeamForMember(Guid memberId)
        {
            Guid teamId = GetTeamIdForMember(memberId);
            if (teamId != Guid.Empty)
                return Ok(new
                {
                    TeamID = teamId
                });

            return NotFound();
        }

        private Guid GetTeamIdForMember(Guid memberId)
        {
            foreach (Team team in _repository.List())
            {
                Member member = team.Members.FirstOrDefault(m => m.Id == memberId);
                if (member != null)
                    return team.Id;
            }

            return Guid.Empty;
        }
    }
}