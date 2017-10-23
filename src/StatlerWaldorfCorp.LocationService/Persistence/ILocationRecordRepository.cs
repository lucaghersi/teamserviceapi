using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StatlerWaldorfCorp.LocationService.Models;

namespace StatlerWaldorfCorp.LocationService.Persistence
{
    public interface ILocationRecordRepository
    {
        Task<LocationRecord> Add(LocationRecord locationRecord);
        Task<LocationRecord> Get(Guid memberId, Guid recordId);
        Task Delete(Guid memberId, Guid recordId);

        Task<LocationRecord> GetLatestForMember(Guid memberId);

        Task<ICollection<LocationRecord>> AllForMember(Guid memberId);
    }
}