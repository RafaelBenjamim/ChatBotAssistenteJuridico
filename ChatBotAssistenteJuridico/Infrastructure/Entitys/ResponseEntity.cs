using System.ComponentModel.DataAnnotations.Schema;

namespace ChatBotAssistenteJuridico.Infrastructure.Entitys
{
    public class ResponseEntity
    {
        public int Id { get; set; }
        public string Pergunta { get; set; }
        public string Reposta { get; set; }
        public DateTime DataCriacao { get; set; }
        public string PerguntaHash { get; set; }

        [ForeignKey("category")]
        public int? CategoryId { get; set; }
        public CategoryEntity? Category { get; set; }
    }
}
