using Sendeazy.Api.Entities;
using Sendeazy.Api.Repositories;

namespace Sendeazy.Api.Services
{
  public class PhotoService : IPhotoService
  {
    private readonly IPhotoDao _photoDao;

    public PhotoService(IPhotoDao photoDao)
    {
      _photoDao = photoDao;
    }
    public void Save(Photo photoToSave)
    {
      _photoDao.Save(photoToSave);
    }
  }
}