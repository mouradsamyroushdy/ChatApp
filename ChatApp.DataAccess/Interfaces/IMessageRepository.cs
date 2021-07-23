using ChatApp.Database.Entities;

using System.Threading.Tasks;

namespace ChatApp.DataAccess.Interfaces
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
    }
}
