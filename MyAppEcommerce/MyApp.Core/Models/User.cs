using MyApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyApp.Core.Models
{
    public class User : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Insert an username")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Insert an Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Insert an password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters")]
        public string Password { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage = "Insert an role")]
        public string Role { get; set; }
        public User() { }
        public User(object[] ItemArray)
        {
            Id = int.Parse(ItemArray[0].ToString());
            Username = ItemArray[1].ToString();
            Email = ItemArray[2].ToString();
            Password = ItemArray[3].ToString();
            Role = ItemArray[4].ToString();
        }
    }
}