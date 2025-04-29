using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Models.Calendar
{
    public class ServiceCheckBoxModel
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
