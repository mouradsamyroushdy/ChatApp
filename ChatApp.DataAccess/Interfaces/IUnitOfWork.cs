
using ChatApp.DataAccess.Repositories;
using ChatApp.Database.Entities;

using System;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IChatRepository Chats { get; }
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public Task SaveChangesAsync();
    }
}
