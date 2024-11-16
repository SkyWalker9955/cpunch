using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpunchApp
{
    /// <summary>
    /// Represents a type of work to be tracked.
    /// </summary>
    public class WorkType
    {
        public string Name { get; set; } = null!;
        public decimal HourlyRate { get; set; }
    }
}
