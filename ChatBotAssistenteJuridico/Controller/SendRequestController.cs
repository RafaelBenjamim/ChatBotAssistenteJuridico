using ChatBotAssistenteJuridico.Domain.Interfaces;
using GenerativeAI;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ChatBotAssistenteJuridico.Controller
{
    [ApiController]
    [Route("api/Update")]
    public class SendRequestController : ControllerBase
    {
        private readonly ISendRequestApplication _telegramBotApplication;
        private readonly ILogger<SendRequestController> _logger;
        public SendRequestController( ISendRequestApplication telegramApplication, ILogger<SendRequestController> logger)
        {
            _telegramBotApplication = telegramApplication;
            _logger = logger;
            
        }

        [HttpPost]
        public async Task<IActionResult> Resquest([FromBody] Update update)
        {
            _logger.LogWarning("Recebendo pergunta");

            await _telegramBotApplication.SendRequest(update);

            return Ok();
        }
    }
}
