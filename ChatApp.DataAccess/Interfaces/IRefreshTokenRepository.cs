
using ChatApp.Database.Entities;

using System;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken> GetRefreshToken(User user, string token, DateTime? epiresAt = null);
        Task<RefreshToken> GetRefreshTokenByUser(int userId, DateTime? epiresAt = null);
    }
}
