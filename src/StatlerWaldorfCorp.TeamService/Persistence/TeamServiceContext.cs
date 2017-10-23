using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.Persistence
{
    public class TeamServiceContext : ITeamRepository
    {
        private readonly IDocumentClient _documentClient;
        private const string DB = "teamservice";
        private const string TEAM_COLLECTION = "teams";
        private const string MEMBER_COLLECTION = "members";

        private static Uri GetTeamCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DB, TEAM_COLLECTION);
        }

        private static Uri GetMemberCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DB, MEMBER_COLLECTION);
        }

        private static Uri GetTeamDocumentUri(Guid teamId)
        {
            return UriFactory.CreateDocumentUri(DB, TEAM_COLLECTION, teamId.ToString());
        }

        private static Uri GetMemberDocumentUri(Guid memberId)
        {
            return UriFactory.CreateDocumentUri(DB, MEMBER_COLLECTION, memberId.ToString());
        }

        public TeamServiceContext(IDocumentClient documentClient)
        {
            _documentClient = documentClient;
        }

        public IEnumerable<Team> List()
        {
            return _documentClient
                .CreateDocumentQuery<Team>(GetTeamCollectionUri(), new FeedOptions {EnableCrossPartitionQuery = true})
                .ToList();
        }

        public async Task<Team> Get(Guid id)
        {
            var result = await _documentClient.ReadDocumentAsync(GetTeamDocumentUri(id),
                new RequestOptions {PartitionKey = new PartitionKey(id.ToString()) });
            return (Team)(dynamic)result.Resource;
        }

        public async Task<Team> Add(Team team)
        {
            var result = await _documentClient.CreateDocumentAsync(GetTeamCollectionUri(), team);
            return (Team)(dynamic)result.Resource;
        }

        public async Task<Team> Update(Team team)
        {
            await _documentClient.ReplaceDocumentAsync(GetTeamDocumentUri(team.Id), team);
            return team;
        }

        public async Task Delete(Guid id)
        {
            await _documentClient.DeleteDocumentAsync(GetTeamDocumentUri(id));
        }

        public IEnumerable<Member> GetMembers()
        {
            return _documentClient
                .CreateDocumentQuery<Member>(GetMemberCollectionUri(),
                    new FeedOptions { EnableCrossPartitionQuery = true })
                .ToList();
        }

        public IEnumerable<Member> GetMembersByTeam(Guid teamId)
        {
            return _documentClient
                .CreateDocumentQuery<Member>(GetMemberCollectionUri(),
                    new FeedOptions {EnableCrossPartitionQuery = true})
                .Where(m => m.TeamId == teamId)
                .ToList();
        }

        public async Task<Member> GetMember(Guid id)
        {
            var result = await _documentClient.ReadDocumentAsync(GetMemberDocumentUri(id),
                new RequestOptions { PartitionKey = new PartitionKey(id.ToString()) });
            return (Member)(dynamic)result.Resource;
        }

        public Member GetMemberInTeam(Guid teamId, Guid memberId)
        {
            // ReSharper disable once ReplaceWithSingleCallToSingleOrDefault
            Member member = _documentClient
                .CreateDocumentQuery<Member>(GetMemberCollectionUri(),
            new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(m => m.Id == memberId).ToList()
                .SingleOrDefault();
            return member?.TeamId == teamId ? member : null;
        }

        public async Task<Member> UpdateMember(Member member)
        {
            await _documentClient.ReplaceDocumentAsync(GetMemberDocumentUri(member.Id), member);
            return member;
        }

        public async Task<Member> CreateMember(Member member)
        {
            member.Id = Guid.NewGuid();
            await _documentClient.CreateDocumentAsync(GetMemberCollectionUri(), member);
            return member;
        }
    }
}
