using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.Service
{
    public class ServiceUpdateRequestModel : ServiceCreateRequestModel
    {
        public int Id { get; set; }
    }
}
