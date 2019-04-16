using System.Threading.Tasks;
using SendeoApi.Entities;

namespace Sendeazy.Api.Repositories
{
    public interface IAdvertDao
    {
    Task<int> CreateAdvert(Advert advertToSave, string connectionString);
    Task<Advert> GetAdvert(int id, string connectionString);
    }
}
