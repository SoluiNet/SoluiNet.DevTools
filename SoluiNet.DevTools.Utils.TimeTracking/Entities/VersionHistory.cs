using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    [Table("VersionHistory")]
    public class VersionHistory
    {
        public int VersionHistoryId { get; set; }

        public string VersionNumber { get; set; }

        public DateTime AppliedDateTime { get; set; }
    }
}
