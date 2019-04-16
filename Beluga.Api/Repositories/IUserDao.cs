using Sendeazy.Api.Helpers;
using System.Threading.Tasks;
using WebApi.Entities;

namespace Sendeazy.Api.Repositories
{
    public interface IUserDao
    {
        User Authenticate(string username, string password, string connectionString);
        Task<PagedList<User>> GetAll(UserParams userParams);
        Task<User> GetById(int id);
        Task<User> GetUserByLogin(string login);
        User Create(User user);
        int Update(User user);
        void Delete(int id);
        void UpdateProfilePicture(int userId, string picturePath, string connectionString);
        Task UpdateUserLastActive(User user);
    }
}
