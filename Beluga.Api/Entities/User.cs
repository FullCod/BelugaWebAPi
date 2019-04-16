using System;
using System.Collections.Generic;
using Sendeazy.Api.Entities;
using Sendeazy.Api.Helpers;

namespace WebApi.Entities
{
  public class User
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string ProfilePicture { get; set; }
    public string PhotoUrl { get; set; }
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string Interest { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photos { get; set; }
      public int Age
      {
          get { return DateOfBirth.CalculateAge(); }
      }

    public User()
    {
      Photos = new List<Photo>();
    }
  }
}