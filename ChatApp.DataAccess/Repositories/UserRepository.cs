using ChatApp.DataAccess.Interfaces;
using ChatApp.Database;
using ChatApp.Database.Entities;
using ChatApp.Database.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            return await _context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(userName));
        }
    }
}
