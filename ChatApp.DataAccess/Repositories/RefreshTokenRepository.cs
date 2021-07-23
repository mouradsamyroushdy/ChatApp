using ChatApp.DataAccess.Interfaces;
using ChatApp.Database;
using ChatApp.Database.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<RefreshToken> GetRefreshToken(User user, string token, DateTime? expiresAt = null)
        {
            expiresAt ??= DateTime.Now;
            return await context.RefreshTokens
                   .Where(f => f.UserId == user.Id && f.Token == token && f.ExpiresAt >= expiresAt)
                   .FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenByUser(int userId, DateTime? expiresAt = null)
        {
            expiresAt ??= DateTime.Now;
            return await context.RefreshTokens
                   .Where(f => f.UserId == userId && f.ExpiresAt >= expiresAt)
                   .FirstOrDefaultAsync();
        }
    }
}
