using Sendeazy.Api.Helpers;
using System.Threading.Tasks;
using WebApi.Entities;

namespace SendeoApi.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password, string connectionString);
        Task<PagedList<User>> GetAll(UserParams userParams);
        Task<User> GetById(int id);
        User Create(User user, string password);
        Task<int> Update(User user);
        Task<bool> UserExists(User user);
        void Delete(int id);
        void UpdateProfilePicture(int userId, string picturePath, string connectionString);
    }
}
