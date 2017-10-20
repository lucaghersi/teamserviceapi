using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.TeamService.Controllers;
using StatlerWaldorfCorp.TeamService.LocationClient;
using StatlerWaldorfCorp.TeamService.Models;
using StatlerWaldorfCorp.TeamService.Persistence;
using StatlerWaldorfCorp.TeamService.Tests;
using Xunit;

[assembly: CollectionBehavior(MaxParallelThreads = 1)]

// ReSharper disable once CheckNamespace
namespace StatlerWaldorfCorp.TeamService
{
    public class MembersControllerTest
    {
        private MembersController GetController(ITeamRepository repository)
        {
            var controller = new MembersController(repository, new MemoryLocationClient()) {Url = new UrlHelper()};
            return controller;
        }

        [Fact]
        public void CreateMemberAddsTeamToList()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestController", teamId);
            repository.Add(team);

            Guid newMemberId = Guid.NewGuid();
            var newMember = new Member(newMemberId);
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);
            Assert.True(team.Members.Contains(newMember));
        }

        [Fact]
        public void CreateMembertoNonexistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();

            Guid newMemberId = Guid.NewGuid();
            var newMember = new Member(newMemberId);
            IActionResult result = controller.CreateMember(newMember, teamId);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void GetExistingMemberReturnsMember()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            Team debugTeam = repository.Add(team);

            Guid memberId = Guid.NewGuid();
            var newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            controller.CreateMember(newMember, teamId);

            Member member = (Member)(await controller.GetMember(teamId, memberId) as ObjectResult).Value;
            Assert.Equal(member.Id, newMember.Id);
        }

        [Fact]
        public void GetMembersForNewTeamIsEmpty()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            Team debugTeam = repository.Add(team);

            var members = (ICollection<Member>) (controller.GetMembers(teamId) as ObjectResult).Value;
            Assert.Empty(members);
        }

        [Fact]
        public void GetMembersForNonExistantTeamReturnNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            IActionResult result = controller.GetMembers(Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void GetMembersReturnsMembers()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            Team debugTeam = repository.Add(team);

            Guid firstMemberId = Guid.NewGuid();
            var newMember = new Member(firstMemberId);
            newMember.FirstName = "Jim";
            newMember.LastName = "Smith";
            controller.CreateMember(newMember, teamId);

            Guid secondMemberId = Guid.NewGuid();
            newMember = new Member(secondMemberId);
            newMember.FirstName = "John";
            newMember.LastName = "Doe";
            controller.CreateMember(newMember, teamId);

            var members = (ICollection<Member>) (controller.GetMembers(teamId) as ObjectResult).Value;
            Assert.Equal(2, members.Count());
            Assert.NotNull(members.First(m => m.Id == firstMemberId).Id);
            Assert.NotNull(members.First(m => m.Id == secondMemberId).Id);
        }

        [Fact]
        public async void GetNonExistantMemberReturnsNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            Team debugTeam = repository.Add(team);

            IActionResult result = await controller.GetMember(teamId, Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void GetNonExistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            IActionResult result = await controller.GetMember(Guid.NewGuid(), Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void UpdateMemberOverwrites()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            Team debugTeam = repository.Add(team);

            Guid memberId = Guid.NewGuid();
            var newMember = new Member(memberId)
            {
                FirstName = "Jim",
                LastName = "Smith"
            };
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);

            var updatedMember = new Member(memberId)
            {
                FirstName = "Bob",
                LastName = "Jones"
            };
            controller.UpdateMember(updatedMember, teamId, memberId);

            team = repository.Get(teamId);
            Member testMember = team.Members.First(m => m.Id == memberId);

            Assert.Equal(testMember.FirstName, "Bob");
            Assert.Equal(testMember.LastName, "Jones");
        }

        [Fact]
        public void UpdateMembertoNonexistantMemberReturnsNoMatch()
        {
            ITeamRepository repository = new MemoryTeamRepositoryTest();
            MembersController controller = GetController(repository);

            Guid teamId = Guid.NewGuid();
            var team = new Team("TestController", teamId);
            repository.Add(team);

            Guid memberId = Guid.NewGuid();
            var newMember = new Member(memberId);
            newMember.FirstName = "Jim";
            controller.CreateMember(newMember, teamId);

            Guid nonMatchedGuid = Guid.NewGuid();
            var updatedMember = new Member(nonMatchedGuid);
            updatedMember.FirstName = "Bob";
            IActionResult result = controller.UpdateMember(updatedMember, teamId, nonMatchedGuid);

            Assert.True(result is NotFoundResult);
        }
    }
}