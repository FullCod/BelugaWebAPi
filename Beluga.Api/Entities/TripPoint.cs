namespace SendeoApi.Entities
{
  public class TripPoint
  {
    public string City { get; set; }
    public string Country { get; set; }
    public string StreetAddress { get; set; }
    public TripDirection TripDirection { get; set; }
  }

  public enum TripDirection
  {
    Depart,
    Destination
  }
}
