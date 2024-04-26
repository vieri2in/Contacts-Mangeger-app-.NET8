using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsMangeger.Core.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be in proper email address format")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password cannot be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
