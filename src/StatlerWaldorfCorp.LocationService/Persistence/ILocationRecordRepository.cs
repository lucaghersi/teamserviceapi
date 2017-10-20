using System;
using System.Collections.Generic;
using StatlerWaldorfCorp.LocationService.Models;

namespace StatlerWaldorfCorp.LocationService.Persistence
{
    public interface ILocationRecordRepository
    {
        LocationRecord Add(LocationRecord locationRecord);
        LocationRecord Update(LocationRecord locationRecord);
        LocationRecord Get(Guid memberId, Guid recordId);
        LocationRecord Delete(Guid memberId, Guid recordId);

        LocationRecord GetLatestForMember(Guid memberId);

        ICollection<LocationRecord> AllForMember(Guid memberId);
    }
}