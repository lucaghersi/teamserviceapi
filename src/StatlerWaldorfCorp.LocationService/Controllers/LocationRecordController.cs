using System;
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
        public IActionResult AddLocation(Guid memberId, [FromBody] LocationRecord locationRecord)
        {
            locationRecord.MemberId = memberId;
                
            _locationRepository.Add(locationRecord);
            return Created(Url.Link("GetLastLocationForMember", new {memberId}), locationRecord);
        }

        [HttpGet(Name="GetLocationForMember")]
        public IActionResult GetLocationsForMember(Guid memberId)
        {
            return Ok(_locationRepository.AllForMember(memberId));
        }

        [HttpGet("latest", Name="GetLastLocationForMember")]
        public IActionResult GetLatestForMember(Guid memberId)
        {
            return Ok(_locationRepository.GetLatestForMember(memberId));
        }
    }
}