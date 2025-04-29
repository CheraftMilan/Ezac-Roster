using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.Qualification
{
    public class QualificationUpdateRequestModel : QualificationCreateRequestModel
    {
        public int Id { get; set; }
    }
}
