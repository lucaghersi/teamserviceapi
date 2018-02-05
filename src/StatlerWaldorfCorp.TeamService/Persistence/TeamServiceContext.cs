using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.IdentityModel.Tokens;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.Persistence
{
    public class TeamServiceContext : ITeamRepository
    {
        private readonly DocumentClient _documentClient;
        private const string DB = "teamservice";
        private const string TEAM_COLLECTION = "teams";
        private const string MEMBER_COLLECTION = "members";

        private Uri GetDatabaseUri()
        {
            return UriFactory.CreateDatabaseUri(DB);
        }

        private async Task<Uri> GetTeamCollectionUri()
        {
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(GetDatabaseUri(),
                new DocumentCollection
                {
                    Id = TEAM_COLLECTION,
                    PartitionKey = new PartitionKeyDefinition {Paths = new Collection<string> {"/id"}}
                });
            return UriFactory.CreateDocumentCollectionUri(DB, TEAM_COLLECTION);
        }

        private async Task<Uri> GetMemberCollectionUri()
        {
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(GetDatabaseUri(),
                new DocumentCollection
                {
                    Id = MEMBER_COLLECTION,
                    PartitionKey = new PartitionKeyDefinition {Paths = new Collection<string> {"/id"}}
                });
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

        public TeamServiceContext(DocumentClient documentClient)
        {
            _documentClient = documentClient;
        }

        public async Task<IEnumerable<Team>> List()
        {
            var collectionUri = await GetTeamCollectionUri();
            return _documentClient
                .CreateDocumentQuery<Team>(collectionUri, new FeedOptions {EnableCrossPartitionQuery = true})
                .ToList();
        }

        public async Task<Team> Get(Guid id)
        {
            try
            {
                var result = await _documentClient.ReadDocumentAsync(GetTeamDocumentUri(id),
                    new RequestOptions {PartitionKey = new PartitionKey(id.ToString())});
                return (Team) (dynamic) result.Resource;
            }
            catch (DocumentClientException e)
            {
                return null;
            }
        }

        public async Task<Team> Add(Team team)
        {
            team.Id = Guid.NewGuid();

            var collectionUri = await GetTeamCollectionUri();
            return await _documentClient.CreateDocumentAsync(collectionUri, team)
                .ContinueWith(task => (Team) (dynamic) task.Result.Resource);
        }

        public async Task<Team> Update(Team team)
        {
            await _documentClient.ReplaceDocumentAsync(GetTeamDocumentUri(team.Id), team);
            return team;
        }

        public async Task Delete(Guid id)
        {
            try
            {
                await _documentClient.DeleteDocumentAsync(GetTeamDocumentUri(id),
                    new RequestOptions {PartitionKey = new PartitionKey(id.ToString())});
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode.HasValue && e.StatusCode.Value == HttpStatusCode.NotFound) return;
                throw;
            }    
        }

        public async Task<IEnumerable<Member>> GetMembers()
        {
            var collectionUri = await GetMemberCollectionUri();
            return _documentClient
                .CreateDocumentQuery<Member>(collectionUri,
                    new FeedOptions { EnableCrossPartitionQuery = true })
                .ToList();
        }

        public async Task<IEnumerable<Member>> GetMembersByTeam(Guid teamId)
        {
            var collectionUri = await GetMemberCollectionUri();
            return _documentClient
                .CreateDocumentQuery<Member>(collectionUri,
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

        public async Task<Member> GetMemberInTeam(Guid teamId, Guid memberId)
        {
            // ReSharper disable once ReplaceWithSingleCallToSingleOrDefault
            var collectionUri = await GetMemberCollectionUri();
            Member member = _documentClient
                .CreateDocumentQuery<Member>(collectionUri,
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

            var collectionUri = await GetMemberCollectionUri();
            await _documentClient.CreateDocumentAsync(collectionUri, member);
            return member;
        }
    }
}
