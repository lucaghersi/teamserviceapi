using Xunit;
using System.Collections.Generic;
using StatlerWaldorfCorp.TeamService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using StatlerWaldorfCorp.TeamService.Controllers;
using StatlerWaldorfCorp.TeamService.Persistence;
using StatlerWaldorfCorp.TeamService.Tests;

// ReSharper disable once CheckNamespace
namespace StatlerWaldorfCorp.TeamService
{
    public class TeamsControllerTest
    {
        private TeamsController GetController(ITeamRepository repository)
        {
            var controller = new TeamsController(repository) { Url = new UrlHelper()};
            return controller;
        }

        [Fact]
        public void QueryTeamListReturnsCorrectTeams()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            var rawTeams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> teams = new List<Team>(rawTeams);
            Assert.Equal(teams.Count, 2);
            Assert.Equal(teams[0].Name, "one");
            Assert.Equal(teams[1].Name, "two");
        }

        [Fact]
        public void GetTeamRetrievesTeam()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            string sampleName = "sample";
            Guid id = Guid.NewGuid();
            Team sampleTeam = new Team(sampleName, id);
            controller.CreateTeam(sampleTeam);

            Team retrievedTeam = (Team)(controller.GetTeam(id) as ObjectResult).Value;
            Assert.Equal(retrievedTeam.Name, sampleName);
            Assert.Equal(retrievedTeam.Id, id);
        }

        [Fact]
        public void GetNonExistentTeamReturnsNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            Guid id = Guid.NewGuid();
            var result = controller.GetTeam(id);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void CreateTeamAddsTeamToList()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> original = new List<Team>(teams);

            Team t = new Team("sample");
            var result = controller.CreateTeam(t);
            //TODO: also assert that the destination URL of the new team reflects the team's GUID
            Assert.Equal((result as ObjectResult).StatusCode, 201);

            var actionResult = controller.GetAllTeams() as ObjectResult;
            var newTeamsRaw = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> newTeams = new List<Team>(newTeamsRaw);
            Assert.Equal(newTeams.Count, original.Count + 1);
            var sampleTeam = newTeams.FirstOrDefault(target => target.Name == "sample");
            Assert.NotNull(sampleTeam);
        }

        [Fact]
        public void UpdateTeamModifiesTeamToList()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> original = new List<Team>(teams);

            Guid id = Guid.NewGuid();
            Team t = new Team("sample", id);
            var result = controller.CreateTeam(t);

            Team newTeam = new Team("sample2", id);
            controller.UpdateTeam(newTeam, id);

            var newTeamsRaw = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> newTeams = new List<Team>(newTeamsRaw);
            var sampleTeam = newTeams.FirstOrDefault(target => target.Name == "sample");
            Assert.Null(sampleTeam);

            Team retrievedTeam = (Team)(controller.GetTeam(id) as ObjectResult).Value;
            Assert.Equal(retrievedTeam.Name, "sample2");
        }

        [Fact]
        public void UpdateNonExistentTeamReturnsNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> original = new List<Team>(teams);

            Team someTeam = new Team("Some Team", Guid.NewGuid());
            controller.CreateTeam(someTeam);

            Guid newTeamId = Guid.NewGuid();
            Team newTeam = new Team("New Team", newTeamId);
            var result = controller.UpdateTeam(newTeam, newTeamId);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void DeleteTeamRemovesFromList()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            int ct = teams.Count();

            string sampleName = "sample";
            Guid id = Guid.NewGuid();
            Team sampleTeam = new Team(sampleName, id);
            controller.CreateTeam(sampleTeam);

            teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            sampleTeam = teams.FirstOrDefault(target => target.Name == sampleName);
            Assert.NotNull(sampleTeam);

            controller.DeleteTeam(id);

            teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            sampleTeam = teams.FirstOrDefault(target => target.Name == sampleName);
            Assert.Null(sampleTeam);
        }

        [Fact]
        public void DeleteNonExistentTeamReturnsNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            var controller = GetController(repository);

            Guid id = Guid.NewGuid();

            var result = controller.DeleteTeam(id);
            Assert.True(result is NotFoundResult);
        }
    }
}