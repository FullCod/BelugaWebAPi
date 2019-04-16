using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Sendeazy.Api.Repositories;
using SendeoApi.Entities;

namespace SendeoApi.Repositories
{
  public class AdvertDao : IAdvertDao
  {
    public async Task<int> CreateAdvert(Advert advertToSave, string connectionString)
    {
      using (IDbConnection db = new SqlConnection(connectionString))
      {
        var parameters = new DynamicParameters();
        parameters.Add("@userId ", advertToSave.UserId);
        parameters.Add("@villeDepart", advertToSave.VilleDepart);
        parameters.Add("@villeDestination", advertToSave.VilleDestination);
        parameters.Add("@intitule", advertToSave.Intitule);
        parameters.Add("@poids", advertToSave.Poids);
        parameters.Add("@description", advertToSave.Description);
        parameters.Add("@dateLimite", advertToSave.DateLimit);
        parameters.Add("@pourboire", advertToSave.Pourboire);
        return await db.QuerySingleOrDefaultAsync("dbo.P_CreateAdvert", parameters, commandType: CommandType.StoredProcedure).Result;
      }
    }

    public Task<Advert> GetAdvert(int id, string connectionString)
    {
      throw new NotImplementedException();
    }
  }
}