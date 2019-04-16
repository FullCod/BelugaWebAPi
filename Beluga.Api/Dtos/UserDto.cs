using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
  public class UserDto
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required(ErrorMessage = "UserName is required")]
    [MinLength(3, ErrorMessage = "Minimun user name length is 3")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    // [Required(ErrorMessage = "Email is require")]
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastActive { get; set; }
  }
}