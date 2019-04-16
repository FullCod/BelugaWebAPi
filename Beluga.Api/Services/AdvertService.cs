using System;
using System.Threading.Tasks;
using Sendeazy.Api.Repositories;
using SendeoApi.Entities;
using SendeoApi.Repositories;

namespace SendeoApi.Services
{
  public class AdvertService : IAdvertService
  {
    private readonly IAdvertDao _advertDao;

    public AdvertService(IAdvertDao advertDao)
    {
      _advertDao = advertDao;
    }

    public async Task<int> CreateAdvert(Advert advertToAdvert, string connectionString)
    {
      return await _advertDao.CreateAdvert(advertToAdvert, connectionString);
    }

    public async Task<Advert> GetAdvert(int id, string connectionString)
    {
      return await _advertDao.GetAdvert(id, connectionString);
    }
  }
}