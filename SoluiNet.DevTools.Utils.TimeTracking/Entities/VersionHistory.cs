using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    public class VersionHistory
    {
        public int VersionHistoryId { get; set; }

        public string VersionNumber { get; set; }

        public DateTime AppliedDateTime { get; set; }
    }
}
