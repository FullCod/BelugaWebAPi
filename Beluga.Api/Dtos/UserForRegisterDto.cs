using System;
using System.ComponentModel.DataAnnotations;

namespace Sendeazy.Api.Dtos
{
  public class UserForRegisterDto
  {
   [Required]
    public string UserName { get; set; }
    [Required]
    [StringLength(8, MinimumLength = 4, ErrorMessage = "Password length must be between 4 and 8")]
    public string Password { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Gender { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
   
    public string Country { get; set; }
  
    public string City { get; set; }

  }
}
