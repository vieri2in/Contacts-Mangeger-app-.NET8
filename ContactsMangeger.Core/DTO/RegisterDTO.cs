using ContactsMangeger.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsMangeger.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name cannot be blank")]
        public string PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be in proper email address format")]
        [Remote(action: "IsEmailValid", controller: "Account", ErrorMessage = "Email address is already in use")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone cannot be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should only contain numbers")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Password cannot be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Comfirm password cannot be blank")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password and confirm password should be the same")]
        public string ConfirmPassword { get; set; }
        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
    }
}
