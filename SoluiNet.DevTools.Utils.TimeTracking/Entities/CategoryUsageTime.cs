using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    [Table("Category_UsageTime")]
    public class CategoryUsageTime
    {
        public int UsageTimeId { get; set; }

        public int CategoryId { get; set; }

        public int Duration { get; set; }
    }
}
