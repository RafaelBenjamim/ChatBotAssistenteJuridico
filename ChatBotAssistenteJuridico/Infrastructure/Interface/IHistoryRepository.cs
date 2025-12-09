using ChatBotAssistenteJuridico.Infrastructure.Entitys;

namespace ChatBotAssistenteJuridico.Infrastructure.Interface
{
    public interface IHistoryRepository
    {
        Task AddMessage(long chadId, string role, string message);
        Task<List<HistoryEntity>> GetHistory(long chatId);
    }
}
