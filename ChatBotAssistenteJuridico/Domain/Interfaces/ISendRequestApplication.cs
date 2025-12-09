using Telegram.Bot.Types;

namespace ChatBotAssistenteJuridico.Domain.Interfaces
{
    public interface ISendRequestApplication
    {
        Task SendRequest(Update update);
        string GenerateHash(string Text);
        Task<bool> Commands(long chatId, string text, string userName);
        Task<bool> ValidateCache(long chatId, string hash);
        Task<(string cleanText, int? categoryId)> GetContext(long chatId, string userMessage);
    }
}
