using System;
using Newtonsoft.Json;

namespace StatlerWaldorfCorp.TeamService.Models
{
    public class Member
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("teamId")]
        public Guid TeamId { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}