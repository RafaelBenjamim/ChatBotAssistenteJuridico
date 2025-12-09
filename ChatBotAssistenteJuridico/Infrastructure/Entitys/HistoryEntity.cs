namespace ChatBotAssistenteJuridico.Infrastructure.Entitys
{
    public class HistoryEntity
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public DateTime SendData { get; set; } = DateTime.UtcNow;
    }
}
