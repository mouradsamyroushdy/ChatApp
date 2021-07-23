using ChatApp.DataAccess.Interfaces;
using ChatApp.Database;
using ChatApp.Database.Entities;
using ChatApp.Database.Enums;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Repositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        public ChatRepository(AppDbContext context) : base(context) { }

        public async Task<int> CreatePrivateRoom(int rootId, int targetId)
        {
            var chat = new Chat { Type = ChatType.Private };
            chat.Users.Add(new ChatUser { UserId = targetId });
            chat.Users.Add(new ChatUser { UserId = rootId });

            dbSet.Add(chat);

            await context.SaveChangesAsync();

            return chat.Id;
        }

        public async Task CreateRoom(string name, int userId)
        {
            var chat = new Chat { Name = name, Type = ChatType.Room };
            chat.Users.Add(new ChatUser { UserId = userId, Role = UserRoleType.Admin });

            dbSet.Add(chat);

            await context.SaveChangesAsync();
        }

        public async Task<bool> JoinRoom(int chatId, int userId)
        {
            var chat = await FindAsync(chatId);
            if (chat == null)
            {
                return false;
            }

            var chatUser = new ChatUser
            {
                ChatId = chatId,
                UserId = userId,
                Role = UserRoleType.Member
            };
            chat.Users.Add(chatUser);

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Chat> GetChat(int id)
        {
            return await dbSet
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Chat>> GetUserChats(int userId)
        {
            return await dbSet
                .Include(x => x.Users)
                .Where(x => !x.Users.Any(y => y.UserId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetUserPrivateChats(int userId)
        {
            return await dbSet
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private && x.Users.Any(y => y.UserId == userId))
                .ToListAsync();
        }

        public async Task<Message?> CreateMessage(int chatId, string text, int userId, string userName)
        {
            var chat = await FindAsync(chatId);

            if (chat == null)
            {
                return null;
            }

            var message = new Message
            {
                ChatId = chatId,
                Text = text,
                UserName = userName,
                UserId = userId,
                Timestamp = DateTime.Now
            };

            chat.Messages.Add(message);
            await context.SaveChangesAsync();

            return message;
        }
    }
}
