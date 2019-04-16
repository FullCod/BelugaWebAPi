using System;

namespace SendeoApi.Entities
{
  public class TripModel
  {
    public int Id { get; set; }
    public TripPoint VilleDepart { get; set; }
    public TripPoint VilleArrivee { get; set; }
    public Driver Driver { get; set; }
    public int Seats { get; set; }
    public decimal Price { get; set; }
    public DateTime TripDate { get; set; }
  }

  public class Driver
  {
    public string Lastname { get; set; }
    public String Firstname { get; set; }
    public int Age { get; set; }
    public int Experience { get; set; }
  }
}
