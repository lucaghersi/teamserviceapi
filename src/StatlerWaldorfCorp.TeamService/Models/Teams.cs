using System;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace StatlerWaldorfCorp.TeamService.Models
{
    public class Team
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}