using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Infrastructure.Models.Result
{
    public class ResultModel<T>
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Value { get; set; }
    }
}
