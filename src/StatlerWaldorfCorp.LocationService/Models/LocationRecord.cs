using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StatlerWaldorfCorp.LocationService.Models
{
    public class LocationRecord
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("memberId")]
        public Guid MemberId { get; set; }
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        [JsonProperty("altitude")]
        public float Altitude { get; set; }
        [JsonProperty("creationDate")]
        public DateTimeOffset CreationDate { get; set; }
    }

    public class LocationRecordComparer : Comparer<LocationRecord>
    {
        public override int Compare(LocationRecord x, LocationRecord y)
        {
            return x.CreationDate.CompareTo(y.CreationDate);
        }
    }
}