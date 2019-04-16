using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SendeoApi.Entities
{
  public class Advert
  {
    public int UserId { get; set; }
    public string VilleDepart { get; set; }
    public string VilleDestination { get; set; }
    public string Intitule { get; set; }
    public int Poids { get; set; }
    public string Description { get; set; }
    public DateTime DateLimit { get; set; }
    public int Pourboire { get; set; }

  }
}
