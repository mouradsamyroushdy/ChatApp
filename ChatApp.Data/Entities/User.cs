using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;

namespace ChatApp.Database.Entities
{
    public class User : IdentityUser<int>
    {
        public User() : base()
        {
            Chats = new List<ChatUser>();
        }
        public ICollection<ChatUser> Chats { get; set; }
    }
}
