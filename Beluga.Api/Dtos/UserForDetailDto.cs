using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sendeazy.Api.Dtos
{
  public class UserForDetailDto
  {
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Gender { get; set; }
    public string PhotoUrl { get; set; }
    public DateTime Age { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTime LastActive { get; set; }
  }
}
