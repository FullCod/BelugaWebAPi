using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendeoApi.Entities;

namespace SendeoApi.Services
{
  public interface IAdvertService
  {
    Task<int> CreateAdvert(Advert advertToAdvert,string connectionString);
    Task<Advert> GetAdvert(int id, string connectionStringsSendyConnectionString);
  }
}
