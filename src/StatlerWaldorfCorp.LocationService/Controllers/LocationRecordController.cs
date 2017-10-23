using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.LocationService.Models;
using StatlerWaldorfCorp.LocationService.Persistence;

namespace StatlerWaldorfCorp.LocationService.Controllers
{
    [Route("locations/{memberId}")]
    public class LocationRecordController : Controller
    {
        private readonly ILocationRecordRepository _locationRepository;

        public LocationRecordController(ILocationRecordRepository repository)
        {
            _locationRepository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(Guid memberId, [FromBody] LocationRecord locationRecord)
        {
            locationRecord.MemberId = memberId;

            await _locationRepository.Add(locationRecord);
            return Created(Url.Link("GetLastLocationForMember", new {memberId}), locationRecord);
        }

        [HttpGet(Name = "GetLocationForMember")]
        public async Task<IActionResult> GetLocationsForMember(Guid memberId)
        {
            var result = await _locationRepository.AllForMember(memberId);
            return Ok(result);
        }

        [HttpGet("latest", Name = "GetLastLocationForMember")]
        public async Task<IActionResult> GetLatestForMember(Guid memberId)
        {
            LocationRecord result = await _locationRepository.GetLatestForMember(memberId);
            return Ok(result);
        }
    }
}