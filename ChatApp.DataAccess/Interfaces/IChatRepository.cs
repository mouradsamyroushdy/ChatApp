using ChatApp.Database.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.DataAccess.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<int> CreatePrivateRoom(int rootId, int targetId);
        Task CreateRoom(string name, int userId);
        Task<Chat> GetChat(int id);
        Task<IEnumerable<Chat>> GetUserChats(int userId);
        Task<IEnumerable<Chat>> GetUserPrivateChats(int userId);
        Task<bool> JoinRoom(int chatId, int userId);
        Task<Message?> CreateMessage(int chatId, string text, int userId, string userName);
    }
}
