namespace ChatBotAssistenteJuridico.Infrastructure.Entitys
{
    public class CategoryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ResponseEntity> Perguntas { get; set; } = new();
    }
}
