using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StatlerWaldorfCorp.LocationService.Models;

namespace StatlerWaldorfCorp.LocationService.Persistence
{
    public interface ILocationRecordRepository
    {
        Task<LocationRecord> Add(LocationRecord locationRecord);
        Task<LocationRecord> Update(LocationRecord locationRecord);
        LocationRecord Get(Guid recordId);
        Task Delete(Guid recordId);

        LocationRecord GetLatestForMember(Guid memberId);

        ICollection<LocationRecord> AllForMember(Guid memberId);
    }
}