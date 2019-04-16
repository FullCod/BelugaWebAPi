using Microsoft.AspNetCore.Mvc;
using SendeoApi.Dtos;
using SendeoApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SendeoApi.Helpers;
using SendeoApi.Services;

namespace SendeoApi.Controllers
{
  [Produces("application/json")]
  [Route("api/[controller]")]
  public class TripsController : Controller
  {
    private IAdvertService _advertService;
    private readonly ConnectionStrings _connectionStrings;
    private IMapper _mapper;
    public TripsController(IAdvertService advertService, IOptions<ConnectionStrings> connectionStrings, IMapper mapper)
    {
      _advertService = advertService;
      _mapper = mapper;
      _connectionStrings = connectionStrings.Value;
    }
    [HttpPost("FindTrips")]
    [Authorize]
    public IActionResult FindTrips([FromBody]tripSearchDto searchCriteria)
    {

      List<TripModel> listTrips = new List<TripModel>();
    //  return Ok(listTrips);
      //for (int i = 0; i < 5; i++)
      //{
      //  var currentTrip = new TripModel()
      //  {
      //    Id = i,
      //    VilleDepart = new TripPoint()
      //    {
      //      City = "Paris",
      //      Country = "France",
      //      StreetAddress = "15 rue de la Bonaparte 75001",
      //      TripDirection = TripDirection.Depart
      //    },
      //    VilleArrivee = new TripPoint()
      //    {
      //      City = "Metz",
      //      Country = "France",
      //      StreetAddress = "15 rue de la Louis 14 75004",
      //      TripDirection = TripDirection.Destination
      //    },
      //    Driver = new Driver()
      //    {
      //      Firstname = "Bob" + i,
      //      Lastname = "Alexandre",
      //      Age = 29,
      //      Experience = 2
      //    },
      //    Seats = 3,
      //    Price = 50
      //  };
      //  listTrips.Add(currentTrip);
      //}
      return Ok(listTrips);
    }
   [HttpGet("{id}")]
   [Authorize]
    public async Task<IActionResult> GetTrips(int id)
    {
      List<TripModel> listTrips = new List<TripModel>();
      //for (int i = 0; i < 5; i++)
      //{
      //  var currentTrip = new TripModel()
      //  {
      //    Id = i,
      //    VilleDepart = new TripPoint()
      //    {
      //      City = "Paris",
      //      Country = "France",
      //      StreetAddress = "15 rue de la Bonaparte 75001",
      //      TripDirection = TripDirection.Depart
      //    },
      //    VilleArrivee = new TripPoint()
      //    {
      //      City = "Metz",
      //      Country = "France",
      //      StreetAddress = "15 rue de la Louis 14 75004",
      //      TripDirection = TripDirection.Destination
      //    },
      //    Driver = new Driver()
      //    {
      //      Firstname = "Bob" + i,
      //      Lastname = "Alexandre",
      //      Age = 29,
      //      Experience = 2
      //    },
      //    Seats = 3,
      //    Price = 50
      //  };
      //  listTrips.Add(currentTrip);
      //}

      var advert = listTrips.Where(t => t.Id == id).FirstOrDefault();
     // var advert = await _advertService.GetAdvert(id, _connectionStrings.SendyConnectionString);
      return Ok(advert);
    }
    [HttpPost("CreateAdvert")]
    public async Task<IActionResult> CreateAdvert([FromBody] AdvertDto advertToSave)
    {
      Advert advert = _mapper.Map<Advert>(advertToSave);
      int advertId = await _advertService.CreateAdvert(advert, _connectionStrings.SendyConnectionString);
      return Ok(advertId);
    }
  }
}