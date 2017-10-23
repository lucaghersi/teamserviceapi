using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using StatlerWaldorfCorp.LocationService.Models;

namespace StatlerWaldorfCorp.LocationService.Persistence
{
    public class LocationServiceContext : ILocationRecordRepository
    {
        private readonly IDocumentClient _documentClient;
        private const string DB = "teamservice";
        private const string COLLECTION = "locations";

        public LocationServiceContext(IDocumentClient documentClient)
        {
            _documentClient = documentClient;
        }

        private static Uri GetLocationsCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DB, COLLECTION);
        }

        private static Uri GetLocationsDocumentUri(Guid locationId)
        {
            return UriFactory.CreateDocumentUri(DB, COLLECTION, locationId.ToString());
        }

        public async Task<LocationRecord> Add(LocationRecord locationRecord)
        {
            locationRecord.Id = Guid.NewGuid();
            locationRecord.CreationDate = DateTimeOffset.UtcNow;
            
            await _documentClient.CreateDocumentAsync(GetLocationsCollectionUri(), locationRecord);
            return locationRecord;
        }

        public async Task<LocationRecord> Get(Guid memberId, Guid recordId)
        {
            var result = await _documentClient.ReadDocumentAsync(GetLocationsDocumentUri(recordId),
                new RequestOptions { PartitionKey = new PartitionKey(memberId.ToString()) });
            return (LocationRecord)(dynamic)result.Resource;
        }

        public async Task Delete(Guid memberId, Guid recordId)
        {
            await _documentClient.DeleteDocumentAsync(GetLocationsDocumentUri(recordId));
        }

        public async Task<LocationRecord> GetLatestForMember(Guid memberId)
        {
            IDocumentQuery<LocationRecord> query = _documentClient.CreateDocumentQuery<LocationRecord>(
                    GetLocationsCollectionUri(), new FeedOptions {PartitionKey = new PartitionKey(memberId.ToString())})
                .Where(t => t.MemberId == memberId)
                .OrderByDescending(f => f.CreationDate)
                .Take(1)
                .AsDocumentQuery();

            return (await query.ExecuteNextAsync<LocationRecord>()).SingleOrDefault();
        }

        public async Task<ICollection<LocationRecord>> AllForMember(Guid memberId)
        {
            IDocumentQuery<LocationRecord> query = _documentClient.CreateDocumentQuery<LocationRecord>(
                    GetLocationsCollectionUri(), new FeedOptions { PartitionKey = new PartitionKey(memberId.ToString()) })
                .Where(t => t.MemberId == memberId)
                .AsDocumentQuery();

            List<LocationRecord> results = new List<LocationRecord>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<LocationRecord>());
            }

            return results;
        }
    }
}
