using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    public class UsageTime
    {
        public int UsageTimeId { get; set; }

        public string ApplicationIdentification { get; set; }

        public DateTime StartTime { get; set; }

        public int Duration { get; set; }
    }
}
