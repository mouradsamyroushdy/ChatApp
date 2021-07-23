using ChatApp.DataAccess.Interfaces;
using ChatApp.DataAccess.Repositories;
using ChatApp.Database;
using ChatApp.Database.Entities;

using System;
using System.Threading.Tasks;

namespace ChatApp.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IChatRepository _chatRepository;
        private IUserRepository _userRepository;
        private IRefreshTokenRepository _resfreshTokenRepository;
        private bool _disposed = false;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IChatRepository Chats
        {
            get
            {

                if (_chatRepository == null)
                {
                    _chatRepository = new ChatRepository(_context);
                }
                return _chatRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {

                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public IRefreshTokenRepository RefreshTokens
        {
            get
            {
                if (_resfreshTokenRepository == null)
                {
                    _resfreshTokenRepository = new RefreshTokenRepository(_context);
                }
                return _resfreshTokenRepository;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}