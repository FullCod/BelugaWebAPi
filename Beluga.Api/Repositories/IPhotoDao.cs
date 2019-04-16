using Sendeazy.Api.Entities;
using System.Threading.Tasks;

namespace Sendeazy.Api.Repositories
{
  public interface IPhotoDao
  {
    void Save(Photo photo);
    Task<Photo> GetPhoto(int id);
    Task<Photo> GetMainPhotoForUser(int userId);
    Task<bool> UpdatePhoto(Photo photoToUpdate);
    Task<int> DeletePhoto(Photo photoFromRepo);
  }
}
