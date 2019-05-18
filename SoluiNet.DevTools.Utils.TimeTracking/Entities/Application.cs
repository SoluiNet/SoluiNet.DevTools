using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.Utils.TimeTracking.Entities
{
    [Table("Application")]
    public class Application
    {
        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }
    }
}
