using ChatApp.Database.Enums;

namespace ChatApp.Database.Entities
{
    public class ChatUser
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public UserRoleType Role { get; set; }
    }
}
