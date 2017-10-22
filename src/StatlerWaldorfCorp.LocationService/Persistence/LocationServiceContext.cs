using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
            locationRecord.Timestamp = DateTimeOffset.UtcNow;
            
            await _documentClient.CreateDocumentAsync(GetLocationsCollectionUri(), locationRecord);
            return locationRecord;
        }

        public async Task<LocationRecord> Update(LocationRecord locationRecord)
        {
            locationRecord.Timestamp = DateTimeOffset.UtcNow;
            await _documentClient.ReplaceDocumentAsync(GetLocationsDocumentUri(locationRecord.Id), locationRecord);
            return locationRecord;
        }

        public LocationRecord Get(Guid recordId)
        {
            return _documentClient
                .CreateDocumentQuery<LocationRecord>(GetLocationsCollectionUri())
                .SingleOrDefault(t => t.Id == recordId);
        }

        public async Task Delete(Guid recordId)
        {
            await _documentClient.DeleteDocumentAsync(GetLocationsDocumentUri(recordId));
        }

        public LocationRecord GetLatestForMember(Guid memberId)
        {
            return _documentClient
                .CreateDocumentQuery<LocationRecord>(GetLocationsCollectionUri(), new FeedOptions{EnableCrossPartitionQuery = true})
                .Where(t => t.MemberId == memberId)
                .ToList()
                .OrderByDescending(t => t.Timestamp).Take(1).SingleOrDefault();
        }

        public ICollection<LocationRecord> AllForMember(Guid memberId)
        {
            return _documentClient
                .CreateDocumentQuery<LocationRecord>(GetLocationsCollectionUri())
                .Where(t => t.MemberId == memberId).ToList();
        }
    }
}
