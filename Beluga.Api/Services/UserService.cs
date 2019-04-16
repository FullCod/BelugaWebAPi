using Sendeazy.Api.Helpers;
using Sendeazy.Api.Repositories;
using SendeoApi.Helpers;
using System;
using System.Threading.Tasks;
using WebApi.Entities;

namespace SendeoApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDao _userDao;

        public UserService(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public async Task<User> Authenticate(string username, string password, string connectionString)
        {
            var user = await _userDao.GetUserByLogin(username);
            // check if username exists
            if (user == null)
            {
                return null;
            }
            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }


        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            var usernew = _userDao.Create(user);
            return usernew;
        }


        public void Delete(int id)
        {
            throw new NotImplementedException();
        }


        public async Task<PagedList<User>> GetAll(UserParams userParams)
        {
            return await _userDao.GetAll(userParams);
        }

        public async Task<User> GetById(int id)
        {
            var user = await _userDao.GetById(id);
            return user;
        }

        public async Task<int> Update(User user)
        {
            return _userDao.Update(user);
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void UpdateProfilePicture(int userId, string picturePath, string connectionString)
        {
            _userDao.UpdateProfilePicture(userId, picturePath, connectionString);
        }

        public async Task<bool> UserExists(User user)
        {
            if (await _userDao.GetUserByLogin(user.UserName) != null)
            {
                return true;
            }
            return false;

        }
    }
}
