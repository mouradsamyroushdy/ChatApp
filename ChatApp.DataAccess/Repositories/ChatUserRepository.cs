using ChatApp.DataAccess.Interfaces;
using ChatApp.Database;
using ChatApp.Database.Entities;
using ChatApp.Database.Enums;

using System.Threading.Tasks;

namespace ChatApp.DataAccess.Repositories
{
    public class ChatUserRepository : GenericRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserRepository(AppDbContext context) : base(context) { }
    }
}
