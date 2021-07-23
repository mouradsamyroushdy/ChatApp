using ChatApp.DataAccess.Interfaces;
using ChatApp.Database;
using ChatApp.Database.Entities;

using System;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context) { }
            }
}
