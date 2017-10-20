using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatlerWaldorfCorp.TeamService.Models;

namespace StatlerWaldorfCorp.TeamService.LocationClient
{
    public class MemoryLocationClient : ILocationClient
    {
        public MemoryLocationClient()
        {
            MemberLocationHistory = new Dictionary<Guid, SortedList<long, LocationRecord>>();
        }

        public Dictionary<Guid, SortedList<long, LocationRecord>> MemberLocationHistory { get; set; }

        public async Task<LocationRecord> AddLocation(Guid memberId, LocationRecord locationRecord)
        {
            return await Task.Run(() =>
            {
                if (!MemberLocationHistory.ContainsKey(memberId))
                    MemberLocationHistory.Add(memberId, new SortedList<long, LocationRecord>());

                MemberLocationHistory[memberId].Add(locationRecord.Timestamp, locationRecord);

                return locationRecord;
            });
        }

        public async Task<LocationRecord> GetLatestForMember(Guid memberId)
        {
            return await Task.Run(() =>
            {
                if (MemberLocationHistory.ContainsKey(memberId))
                    return MemberLocationHistory[memberId].Values.LastOrDefault();

                return null;
            });
        }
    }
}