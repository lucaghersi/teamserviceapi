using System;
using System.Collections.Generic;
using System.Linq;
using StatlerWaldorfCorp.LocationService.Models;

namespace StatlerWaldorfCorp.LocationService.Persistence
{
    public class InMemoryLocationRecordRepository : ILocationRecordRepository
    {
        private static Dictionary<Guid, SortedList<long, LocationRecord>> _locationRecords;

        public InMemoryLocationRecordRepository()
        {
            if (_locationRecords == null)
                _locationRecords = new Dictionary<Guid, SortedList<long, LocationRecord>>();
        }

        public LocationRecord Add(LocationRecord locationRecord)
        {
            var memberRecords = getMemberRecords(locationRecord.MemberId);

            memberRecords.Add(locationRecord.Timestamp, locationRecord);
            return locationRecord;
        }

        public ICollection<LocationRecord> AllForMember(Guid memberId)
        {
            var memberRecords = getMemberRecords(memberId);
            return memberRecords.Values.Where(l => l.MemberId == memberId).ToList();
        }

        public LocationRecord Delete(Guid memberId, Guid recordId)
        {
            var memberRecords = getMemberRecords(memberId);
            LocationRecord lr = memberRecords.Values.FirstOrDefault(l => l.ID == recordId);

            if (lr != null)
                memberRecords.Remove(lr.Timestamp);

            return lr;
        }

        public LocationRecord Get(Guid memberId, Guid recordId)
        {
            var memberRecords = getMemberRecords(memberId);

            LocationRecord lr = memberRecords.Values.FirstOrDefault(l => l.ID == recordId);
            return lr;
        }

        public LocationRecord Update(LocationRecord locationRecord)
        {
            return Delete(locationRecord.MemberId, locationRecord.ID);
        }

        public LocationRecord GetLatestForMember(Guid memberId)
        {
            var memberRecords = getMemberRecords(memberId);

            LocationRecord lr = memberRecords.Values.LastOrDefault();
            return lr;
        }

        private SortedList<long, LocationRecord> getMemberRecords(Guid memberId)
        {
            if (!_locationRecords.ContainsKey(memberId))
                _locationRecords.Add(memberId, new SortedList<long, LocationRecord>());

            var list = _locationRecords[memberId];
            return list;
        }
    }
}