using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatlerWaldorfCorp.TeamService.Models
{
    public class LocatedMember : Member
    {
        public LocationRecord LastLocation { get; set; }
    }
}
