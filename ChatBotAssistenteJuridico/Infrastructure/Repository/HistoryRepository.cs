using ChatBotAssistenteJuridico.Infrastructure.Data;
using ChatBotAssistenteJuridico.Infrastructure.Entitys;
using ChatBotAssistenteJuridico.Infrastructure.Interface;
using GenerativeAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Telegram.Bot.Types;

namespace ChatBotAssistenteJuridico.Infrastructure.Repository
{
    public class HistoryRepository(RequestContext _context) : IHistoryRepository
    {
        public async Task AddMessage(long chatId, string role, string message)
        {
            var history = new HistoryEntity
            {
                ChatId = chatId,
                Role = role,
                Message = message,
                SendData = DateTime.UtcNow
            };

            await _context.History.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<HistoryEntity>> GetHistory(long chatId)
        {
            var history = await _context.History.Where(x => x.ChatId == chatId).OrderByDescending(x => x.SendData).Take(10).ToListAsync();

            return history.OrderBy(x => x.SendData).ToList();

        }
    }
}
