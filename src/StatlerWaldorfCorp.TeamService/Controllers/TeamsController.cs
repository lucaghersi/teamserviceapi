﻿using System;
using System.Threading.Tasks;
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
        public IActionResult GetAllTeams()
        {
            return Ok(_repository.List());
        }

        [HttpGet("{id}", Name = "GetTeamById")]
        public IActionResult GetTeam(Guid id)
        {
            Team team = _repository.Get(id);

            if (team != null)
                return Ok(team);
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team newTeam)
        {
            newTeam = await _repository.Add(newTeam);
            return Created(Url.Link("GetTeamById", new {id = newTeam.Id}), newTeam);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam([FromBody] Team team, Guid id)
        {
            team.Id = id;

            if (await _repository.Update(team) == null)
                return NotFound();
            return Ok(team);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            await _repository.Delete(id);
            return Ok();
        }
    }
}