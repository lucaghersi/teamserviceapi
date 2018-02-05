using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.TeamService.LocationClient;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.Persistence;

namespace StatlerWaldorfCorp.TeamService.Controllers
{
    [Route("api/teams/{teamId}/[controller]")]
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
        public virtual async Task<IActionResult> GetMembers(Guid teamId)
        {
            var teamMembers = await _repository.GetMembersByTeam(teamId);
            return Ok(teamMembers);
        }


        [HttpGet("/teams/{teamId}/[controller]/{memberId}")]
        public virtual async Task<IActionResult> GetMember(Guid teamId, Guid memberId)
        {
            var member = await _repository.GetMemberInTeam(teamId, memberId);
            if (member == null) return NotFound();

            var location = await _locationClient.GetLatestForMember(member.Id);
            return Ok(new LocatedMember
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                LastLocation = location
            });
        }

        [HttpPut("/teams/{teamId}/[controller]/{memberId}", Name = "GetById")]
        public virtual async Task<IActionResult> UpdateMember([FromBody] Member updatedMember, Guid teamId, Guid memberId)
        {
            updatedMember.TeamId = teamId;
            updatedMember.Id = memberId;

            await _repository.UpdateMember(updatedMember);
            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateMember([FromBody] Member newMember, Guid teamId)
        {
            newMember.TeamId = teamId;
                
            var member = await _repository.CreateMember(newMember);
            return Created(Url.Link("GetById", new {teamId, memberId = member.Id}), member);
        }

        [HttpGet("/members/{memberId}/team")]
        public async Task<IActionResult> GetTeamForMember(Guid memberId)
        {
            Member member = await _repository.GetMember(memberId);

            if (member != null)
                return Ok(member.TeamId);

            return NotFound();
        }
    }
}