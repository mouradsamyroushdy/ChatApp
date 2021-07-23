using ChatApp.Database.Entities;

using System.Threading.Tasks;

namespace ChatApp.DataAccess.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserByNameAsync(string userName);
    }
}