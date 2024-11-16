using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpunchApp
{
    /// <summary>
    /// Punch record is a represetation of a record of the punch made.
    /// </summary>
    public record PunchRecord
    {
        public string WorkTypeName { get; set; } = null!;
        public DateTime PunchInTime { get; set; }
        public DateTime? PunchOutTime { get; set; }
    }
}
