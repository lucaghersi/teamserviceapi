using System;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.Persistence;

namespace StatlerWaldorfCorp.TeamService.Controllers
{
    [Route("[controller]")]
    public class TeamsController : Controller
    {
        private readonly ITeamRepository _repository;

        public TeamsController(ITeamRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public virtual IActionResult GetAllTeams()
        {
            return Ok(_repository.List());
        }

        [HttpGet("{id}", Name = "GetById")]
        public IActionResult GetTeam(Guid id)
        {
            Team team = _repository.Get(id);

            if (team != null)
                return Ok(team);
            return NotFound();
        }

        [HttpPost]
        public virtual IActionResult CreateTeam([FromBody] Team newTeam)
        {
            _repository.Add(newTeam);
            return Created(Url.Link("GetById", newTeam.Id), newTeam);
        }

        [HttpPut("{id}")]
        public virtual IActionResult UpdateTeam([FromBody] Team team, Guid id)
        {
            team.Id = id;

            if (_repository.Update(team) == null)
                return NotFound();
            return Ok(team);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult DeleteTeam(Guid id)
        {
            Team team = _repository.Delete(id);

            if (team == null)
                return NotFound();
            return Ok(team.Id);
        }
    }
}