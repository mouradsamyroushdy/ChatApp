using ChatApp.DataAccess.Interfaces;
using ChatApp.Database.Entities;
using ChatApp.Hubs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IChatRepository _repo;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(ILogger<ChatsController> logger, IUnitOfWork uow)
        {
            _uow = uow;
            _repo = uow.Chats;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IEnumerable<Chat>> Index()
        {
            return await _repo.GetUserChats(GetUserId());
        }

        [HttpGet("private")]
        public async Task<IEnumerable<Chat>> Private()
        {
            return await _repo.GetUserPrivateChats(GetUserId());
        }

        [HttpGet("{id}")]
        public async Task<Chat> Chat(int id)
        {
            return await _repo.GetChat(id);
        }

        [HttpPost("room/private")]
        public async Task<int> CreatePrivateRoom(int userId)
        {
            return await _repo.CreatePrivateRoom(GetUserId(), userId);
        }

        [HttpPost("room")]
        public async Task CreateRoom(string name)
        {
            await _repo.CreateRoom(name, GetUserId());
        }

        [HttpGet("room")]
        public async Task<bool> JoinRoom(int id)
        {
            return await _repo.JoinRoom(id, GetUserId());
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage(
            int roomId,
            string text,
            [FromServices] IHubContext<ChatHub> chat)
        {
            var message = await _repo.CreateMessage(roomId, text, GetUserId(), GetUserName());

            await chat.Clients.Group(roomId.ToString())
                .SendAsync("RecieveMessage", new
                {
                    message.Text,
                    message.UserName,
                    Timestamp = message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss")
                });

            return Ok();
        }
    }
}