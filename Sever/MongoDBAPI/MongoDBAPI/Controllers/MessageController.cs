using Microsoft.AspNetCore.Mvc;
using MongoDBAPI.Classes;
using MongoDBAPI.Services;

namespace MongoDBAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService MessagesService) =>
            _messageService = MessagesService;

        [HttpGet]
        public async Task<List<Message>> Get() =>
            await _messageService.GetAsync();

        [HttpGet("GetSender/{id}")]
        public async Task<ActionResult<List<Message>>> GetSender(string id)
        {
            var Message = await _messageService.GetAsyncFromUser(id);

            if (Message is null)
            {
                return NotFound();
            }

            return Message;
        }

        [HttpGet("GetReciever/{id}")]
        public async Task<ActionResult<List<Message>>> GetReciever(string id)
        {
            var Message = await _messageService.GetAsyncToUser(id);

            if (Message is null)
            {
                return NotFound();
            }

            return Message;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Message newMessage)
        {
            
            await _messageService.CreateAsync(newMessage);

            return CreatedAtAction(nameof(Get), new { id = newMessage.Id }, newMessage);
        }
    }
}
