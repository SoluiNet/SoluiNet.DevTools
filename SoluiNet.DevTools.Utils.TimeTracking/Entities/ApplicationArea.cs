using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    [Table("ApplicationArea")]
    public class ApplicationArea
    {
        public int ApplicationAreaId { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }
    }
}
