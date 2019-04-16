using Dapper;
using Microsoft.Extensions.Options;
using Sendeazy.Api.Entities;
using SendeoApi.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sendeazy.Api.Repositories
{
  public class PhotoDao : IPhotoDao
  {
    private readonly IOptions<ConnectionStrings> _connectionString;

    public PhotoDao(IOptions<ConnectionStrings> connectionString)
    {
      _connectionString = connectionString;
    }
    public void Save(Photo photo)
    {
      int updateId;
      using (IDbConnection db = new SqlConnection(_connectionString.Value.SendyConnectionString))
      {
        var parameters = new DynamicParameters();
        parameters.Add("@userId", photo.UserId);
        parameters.Add("@url", photo.Url);
        parameters.Add("@description", photo.Description);
        parameters.Add("@isMain", photo.IsMain);
        parameters.Add("@publicId", photo.PublicId);
        parameters.Add("@dateAdded", photo.DateAdded);

        updateId = db.Execute("dbo.P_InsertPhoto", parameters, commandType: CommandType.StoredProcedure);
      }

    }

    public async Task<Photo> GetPhoto(int id)
    {
      using (IDbConnection db = new SqlConnection(_connectionString.Value.SendyConnectionString))
      {
        var parameters = new DynamicParameters();
        parameters.Add("@photoId", id);

        var photo = await db.QueryFirstOrDefaultAsync<Photo>("dbo.P_GetPhoto", parameters, commandType: CommandType.StoredProcedure);
        return photo;
      }
    }

    public async Task<Photo> GetMainPhotoForUser(int userId)
    {
      using (IDbConnection db = new SqlConnection(_connectionString.Value.SendyConnectionString))
      {
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);

        var photo = await db.QueryFirstOrDefaultAsync<Photo>("dbo.P_GetMainPhotoForUser", parameters, commandType: CommandType.StoredProcedure);
        return photo;
      }
    }

    public async Task<bool> UpdatePhoto(Photo photoToUpdate)
    {
      using (IDbConnection db = new SqlConnection(_connectionString.Value.SendyConnectionString))
      {
        var parameters = new DynamicParameters();
        parameters.Add("@userId", photoToUpdate.UserId);
        parameters.Add("@id", photoToUpdate.Id);
        parameters.Add("@description", photoToUpdate.Description);
        parameters.Add("@isMain", photoToUpdate.IsMain);

        var saved = await db.ExecuteAsync("dbo.P_UpdatePhoto", parameters, commandType: CommandType.StoredProcedure);
        return true;
      }
    }

    public async Task<int> DeletePhoto(Photo photoToDelete)
    {
      using (IDbConnection db = new SqlConnection(_connectionString.Value.SendyConnectionString))
      {
        var parameters = new DynamicParameters();
        parameters.Add("@photoId", photoToDelete.Id);
        var deletedId = await db.ExecuteAsync("dbo.P_DeletePhoto", parameters, commandType: CommandType.StoredProcedure);
        return deletedId;
      }
    }
  }
}