using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezac.Roster.Domain.Entities
{
    public class AdminUser : IdentityUser
    {

        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";


    }
}
