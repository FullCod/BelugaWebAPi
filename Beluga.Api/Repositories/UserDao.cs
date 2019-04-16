using Dapper;
using Microsoft.Extensions.Options;
using Sendeazy.Api.Entities;
using Sendeazy.Api.Helpers;
using SendeoApi.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;

namespace Sendeazy.Api.Repositories
{
    public class UserDao : IUserDao
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        private readonly IPhotoDao _photoDao;
        //private IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["contactsDB"].ConnectionString);
        //string conString = Microsoft
        //    .Extensions
        //    .Configuration
        //    .ConfigurationExtensions
        //    .GetConnectionString(this.Configuration, "DefaultConnection");

        public UserDao(IOptions<ConnectionStrings> connectionStrings, IPhotoDao photoDao)
        {
            _connectionStrings = connectionStrings;
            _photoDao = photoDao;
        }
        public User Authenticate(string username, string password, string connectionString)
        {
            User user = new User();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@login", username);
                parameters.Add("@password", password);
                user = db.Query<User>("dbo.P_Login", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (user != null)
                {
                    var parameters2 = new DynamicParameters();
                    parameters2.Add("@userId", user.Id);
                    user.Photos = db.Query<Photo>("dbo.P_GetUserPhotos", parameters2, commandType: CommandType.StoredProcedure).ToList();
                    user.PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url;
                }
            }
            return user;
        }

        public async Task<User> GetUserByLogin(string login)
        {
            User user = new User();
            using (IDbConnection db = new SqlConnection(_connectionStrings.Value.SendyConnectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Login", login);
                user = await db.QueryFirstOrDefaultAsync<User>("dbo.P_GetUserByLogin", parameters, commandType: CommandType.StoredProcedure);

                if (user != null)
                {
                    var parameters2 = new DynamicParameters();
                    parameters2.Add("@userId", user.Id);
                    user.Photos = db.Query<Photo>("dbo.P_GetUserPhotos", parameters2, commandType: CommandType.StoredProcedure).ToList();
                    user.PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url;
                }

            }
            return user;
        }

        public User Create(User user)
        {
            using (IDbConnection db = new SqlConnection(_connectionStrings.Value.SendyConnectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Login", user.UserName);
                parameters.Add("@passwordhash", user.PasswordHash);
                parameters.Add("@passwordSalt", user.PasswordSalt);
                parameters.Add("@firstName", user.FirstName);
                parameters.Add("@lastName", user.LastName);
                parameters.Add("@Email", user.Email);
                parameters.Add("@PhoneNumber", user.PhoneNumber);
                int userId = db.Query<int>("dbo.P_CreateUser", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                user.Id = userId;
            }
            return user;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateProfilePicture(int userId, string picturePath, string connectionString)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId);
                parameters.Add("@profilepicture", picturePath);
                int Id = db.Query<int>("dbo.UpdateProfilePicture", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
        }






        public async Task<PagedList<User>> GetAll(UserParams userParams)
        {
            using (IDbConnection db = new SqlConnection(_connectionStrings.Value.SendyConnectionString))
            {
                var users = db.Query<User>("dbo.P_GetUsers", commandType: CommandType.StoredProcedure).ToAsyncEnumerable();
                users = users.Where(u => u.Id != userParams.UserId && u.Gender == userParams.Gender);

                if (userParams.MinAge != 18 || userParams.MaxAge != 99)
                {
                    var minDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                    var maxDoB = DateTime.Today.AddYears(-userParams.MaxAge);
                    users = users.Where(u => u.DateOfBirth >= minDoB && u.DateOfBirth <= maxDoB);
                }


                return await PagedList<User>.CreateAsync(users, userParams.pageNumber, userParams.PageSize);
            }
        }

        public async Task<User> GetById(int id)
        {
            using (IDbConnection db = new SqlConnection(_connectionStrings.Value.SendyConnectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", id);
                var user = db.QueryFirstAsync<User>("dbo.P_GetUserById", parameters, commandType: CommandType.StoredProcedure).Result;
                if (user != null)
                {
                    var parameters2 = new DynamicParameters();
                    parameters2.Add("@userId", id);
                    user.Photos = db.Query<Photo>("dbo.P_GetUserPhotos", parameters2, commandType: CommandType.StoredProcedure)
                      .ToList();
                    user.PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url;
                }

                return user;
            }
        }

        public int Update(User user)
        {
            int updateId;
            using (IDbConnection db = new SqlConnection(_connectionStrings.Value.SendyConnectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", user.Id);
                parameters.Add("@login", user.UserName);
                parameters.Add("@firstName", user.FirstName);
                parameters.Add("@lastName", user.LastName);
                parameters.Add("@email", user.Email);
                parameters.Add("@phoneNumber", user.PhoneNumber);

                updateId = db.Execute("dbo.P_UpdateUser", parameters, commandType: CommandType.StoredProcedure);
            }

            foreach (var photo in user.Photos.Where(p => p.Id <= 0))
            {
                _photoDao.Save(photo);
            }
            return updateId;
        }

        public async Task UpdateUserLastActive(User user)
        {
            int updateId;
            using (IDbConnection db = new SqlConnection(_connectionStrings.Value.SendyConnectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", user.Id);
                parameters.Add("@lastActive", user.LastActive);
                updateId = db.Execute("dbo.P_UpdateUserLastActive", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
