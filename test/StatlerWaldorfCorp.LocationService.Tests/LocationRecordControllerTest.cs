using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.LocationService.Controllers;
using StatlerWaldorfCorp.LocationService.Models;
using StatlerWaldorfCorp.LocationService.Persistence;
using StatlerWaldorfCorp.LocationService.Tests;
using Xunit;

// ReSharper disable once CheckNamespace
namespace StatlerWaldorfCorp.LocationService
{
    public class LocationRecordControllerTest
    {
        private LocationRecordController GetController(ILocationRecordRepository repository)
        {
            var controller = new LocationRecordController(repository) {Url = new UrlHelper()};
            return controller;
        }

        [Fact]
        public void ShouldAdd()
        {
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = GetController(repository);
            Guid memberGuid = Guid.NewGuid();

            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = Guid.NewGuid(),
                MemberId = memberGuid,
                Timestamp = 1
            });
            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = Guid.NewGuid(),
                MemberId = memberGuid,
                Timestamp = 2
            });

            Assert.Equal(2, repository.AllForMember(memberGuid).Count());
        }

        [Fact]
        public void ShouldReturnEmtpyListForNewMember()
        {
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = GetController(repository);
            Guid memberGuid = Guid.NewGuid();

            var locationRecords =
                (controller.GetLocationsForMember(memberGuid) as ObjectResult).Value as ICollection<LocationRecord>;

            Assert.Equal(0, locationRecords.Count());
        }

        [Fact]
        public void ShouldTrackAllLocationsForMember()
        {
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = GetController(repository);
            Guid memberGuid = Guid.NewGuid();

            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = Guid.NewGuid(),
                Timestamp = 1,
                MemberId = memberGuid,
                Latitude = 12.3f
            });
            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = Guid.NewGuid(),
                Timestamp = 2,
                MemberId = memberGuid,
                Latitude = 23.4f
            });
            controller.AddLocation(Guid.NewGuid(), new LocationRecord
            {
                ID = Guid.NewGuid(),
                Timestamp = 3,
                MemberId = Guid.NewGuid(),
                Latitude = 23.4f
            });

            var locationRecords =
                (controller.GetLocationsForMember(memberGuid) as ObjectResult).Value as ICollection<LocationRecord>;

            Assert.Equal(2, locationRecords.Count());
        }

        [Fact]
        public void ShouldTrackLatestLocationsForMember()
        {
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = GetController(repository);
            Guid memberGuid = Guid.NewGuid();

            Guid latestId = Guid.NewGuid();
            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = Guid.NewGuid(),
                Timestamp = 1,
                MemberId = memberGuid,
                Latitude = 12.3f
            });
            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = latestId,
                Timestamp = 3,
                MemberId = memberGuid,
                Latitude = 23.4f
            });
            controller.AddLocation(memberGuid, new LocationRecord
            {
                ID = Guid.NewGuid(),
                Timestamp = 2,
                MemberId = memberGuid,
                Latitude = 23.4f
            });
            controller.AddLocation(Guid.NewGuid(), new LocationRecord
            {
                ID = Guid.NewGuid(),
                Timestamp = 4,
                MemberId = Guid.NewGuid(),
                Latitude = 23.4f
            });

            var latest = (controller.GetLatestForMember(memberGuid) as ObjectResult).Value as LocationRecord;

            Assert.NotNull(latest);
            Assert.Equal(latestId, latest.ID);
        }

        [Fact]
        public void ShouldTrackNullLatestForNewMember()
        {
            ILocationRecordRepository repository = new InMemoryLocationRecordRepository();
            var controller = GetController(repository);
            Guid memberGuid = Guid.NewGuid();

            var latest = (controller.GetLatestForMember(memberGuid) as ObjectResult).Value as LocationRecord;

            Assert.Null(latest);
        }
    }
}